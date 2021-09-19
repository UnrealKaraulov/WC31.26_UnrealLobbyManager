using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;
using System.Threading;

namespace iCCupUnrealLobbyManager
{
    public partial class LobbyManager : Form
    {
        const string cachedir = ".\\cachedir";
        const string BlackListFilePath = ".\\blacklist.dat";
        WebClient client = new WebClient();

        public LobbyManager()
        {
            InitializeComponent();
        }

        enum FRAME_TYPE : int
        {
            FRAME_TEXT,
            FRAME_MENU,
            FRAME_BUTTON,
            FRAME_UNKNOWN
        }
        int PlayerInfoStringTicks = 0;
        struct PlayerInfoStruct
        {
            public int PTS;
            public int WinRate;
            public int Win;
            public int Lose;
            public int Leaves;
            public int WinStreak;
            public int CurStreak;
            public string PlayerName;
            public string skype;
            public string flag;
            public string OldPlayerName;
            public bool Is3x3;
            public bool allokay;
            public bool allotherokay;
            public bool needkick;
            public int FireOffset;
        }


        List<PlayerInfoStruct> AllPlayers = new List<PlayerInfoStruct>();
        List<string> BlackList = new List<string>();


        string GetFireUserName(string username, ref int offset)
        {
            offset++;
            if (offset > 9)
                offset = 0;

            int localoffset = offset;
            string result = string.Empty;
            string fullfirestring = "|c00FF80000|c00FF8E001|c00FF9C002|c00FFA9003|c00FFB7004|c00FFC5015|c00FFD3016|c00FFE0017|c00FFEE018|c00FFFC019";

            for (int i = 0; i < username.Length; i++)
            {
                localoffset++;
                if (localoffset > 9)
                    localoffset = 0;

                string regexstr = @"(\|c\w\w\w\w\w\w\w\w)" + localoffset.ToString();
                Match GetColor = Regex.Match(fullfirestring, regexstr);
                if (GetColor.Success)
                {
                    result += GetColor.Groups[1].Value + username[i].ToString();
                }
            }
            result += "|r";
            return result;
        }

        bool IfBlacklisted(string str2)
        {
            foreach (string str1 in BlackList)
            {
                string str3 = Encoding.Default.GetString(Encoding.UTF8.GetBytes(str1));
                if (str3.ToLower() == str2.ToLower())
                    return true;
            }
            return false;
        }

        void AddToBlackList(string str)
        {
            BlackList.Add(Encoding.UTF8.GetString(Encoding.Default.GetBytes(str)));
        }

        string BoolIs3x3(bool Is3x3)
        {
            if (Is3x3)
                return "_______________[3x3].dat";
            return "_______________[5x5].dat";
        }

        void SavePlayerInfoStructToCache(PlayerInfoStruct str)
        {
            string FilePlayerInfoPath = cachedir + "\\" + str.PlayerName + BoolIs3x3(str.Is3x3);

            if (File.Exists(FilePlayerInfoPath))
                return;

            File.Create(FilePlayerInfoPath).Close();
            File.AppendAllText(FilePlayerInfoPath, str.PTS.ToString() + "\r\n");
            File.AppendAllText(FilePlayerInfoPath, str.WinRate.ToString() + "\r\n");
            File.AppendAllText(FilePlayerInfoPath, str.Win.ToString() + "\r\n");
            File.AppendAllText(FilePlayerInfoPath, str.Lose.ToString() + "\r\n");
            File.AppendAllText(FilePlayerInfoPath, str.Leaves.ToString() + "\r\n");
            File.AppendAllText(FilePlayerInfoPath, str.WinStreak.ToString() + "\r\n");
            File.AppendAllText(FilePlayerInfoPath, str.CurStreak.ToString() + "\r\n");
            if (str.allotherokay)
            {
                File.AppendAllText(FilePlayerInfoPath, str.skype.ToString() + "\r\n");
                File.AppendAllText(FilePlayerInfoPath, str.flag.ToString() + "\r\n");
            }
        }

        void FillGeneralPlayerInfo(ref PlayerInfoStruct str)
        {
            string regex1 = @"\<div\s+class\s*=\s*\""p-body.*?\""\s+id\s*=\s*\""main-stata-" + (str.Is3x3 ? "3x3" : "5x5") + @"\"".*?i-pts\""\>\s*(-?\+?\d+).*?Leave.*?\<td\>\s*(-?\+?\d+)\s*\/\s*(-?\+?\d+)\s*\/\s*(-?\+?\d+)\s*.*?побед.*?\<td\>\s*(-?\+?\d+).*?екущий.*?\<td\>\s*(-?\+?\d+)";
            string url1 = "http://iccup.com/dota/gamingprofile/" + Uri.EscapeUriString(str.PlayerName.ToLower()) + ".html";
            string data = "";

            trynow:
            try
            {
                data = client.DownloadString( url1 );
            }
            catch
            {
                Thread.Sleep( 1000 );
                goto trynow;
            }
            // File.WriteAllText(".\\data.txt", data);
            //  File.WriteAllText(".\\data_regex.txt", regex1);
            Match PlayerDataFromUrl = Regex.Match(data, regex1, RegexOptions.Singleline);


            if (PlayerDataFromUrl.Success)
            {


                str.PTS = int.Parse(PlayerDataFromUrl.Groups[1].Value);
                str.Win = int.Parse(PlayerDataFromUrl.Groups[2].Value);
                str.Lose = int.Parse(PlayerDataFromUrl.Groups[3].Value);
                str.Leaves = int.Parse(PlayerDataFromUrl.Groups[4].Value);


                str.WinStreak = int.Parse(PlayerDataFromUrl.Groups[5].Value);
                str.CurStreak = int.Parse(PlayerDataFromUrl.Groups[6].Value);
                if (str.Win == 0 && str.Lose == 0)
                    str.WinRate = 0;
                else
                    str.WinRate = Convert.ToInt32(Convert.ToSingle(str.Win) / Convert.ToSingle(str.Win + str.Lose) * 100.0f);
                str.allokay = true;


            }
            else
            {


                str.allokay = false;
            }
        }


        void FillOtherPlayerInfo(ref PlayerInfoStruct str)
        {

            if (!ActivateOtherInfo.Checked)
            {
                str.flag = string.Empty;
                str.skype = string.Empty;
                str.allotherokay = false;
                return;
            }

            string regex1 = @"ls-inside.*?flags\/(\w\w).*?skype\:(\w+)";
            string regex2 = @"ls-inside.*?flags\/(\w\w)";

            string url1 = "http://iccup.com/dota/profile/view/" + str.PlayerName;
            string data = client.DownloadString(url1);
            Match PlayerDataFromUrl = Regex.Match(data, regex1, RegexOptions.Singleline);
            if (PlayerDataFromUrl.Success)
            {
                str.flag = PlayerDataFromUrl.Groups[1].Value;
                str.skype = PlayerDataFromUrl.Groups[2].Value;
                str.allotherokay = true;
            }
            else
            {
                PlayerDataFromUrl = Regex.Match(data, regex2, RegexOptions.Singleline);
                if (PlayerDataFromUrl.Success)
                {
                    str.flag = PlayerDataFromUrl.Groups[1].Value;
                    str.skype = string.Empty;
                    str.allotherokay = true;
                }
                else
                    str.allotherokay = false;
            }
        }

        void RefreshBlackList()
        {
            if (File.Exists(BlackListFilePath))
            {
                File.Delete(BlackListFilePath);
            }

            File.WriteAllLines(BlackListFilePath, BlackList.ToArray());

        }

        bool SkipUpdate = false;
        PlayerInfoStruct LoadPlayerInfoStructFromIccup(string playername, bool Is3x3)
        {
            PlayerInfoStruct TempPlayerInfoStruct = new PlayerInfoStruct();
            TempPlayerInfoStruct.PlayerName = playername;
            TempPlayerInfoStruct.OldPlayerName = playername;
            TempPlayerInfoStruct.Is3x3 = Is3x3;
            // MessageBox.Show(playername);

            try
            {
                FillGeneralPlayerInfo(ref TempPlayerInfoStruct);
                Thread.Sleep(50);
                FillOtherPlayerInfo(ref TempPlayerInfoStruct);
                Thread.Sleep(100);
                SkipUpdate = true;
            }
            catch
            {
                if (!IfBlacklisted(playername))
                {
                    AddToBlackList(playername);
                    RefreshBlackList();
                    return TempPlayerInfoStruct;
                }
            }
            if (!TempPlayerInfoStruct.allokay)
            {
                if (!IfBlacklisted(playername))
                {
                    AddToBlackList(playername);
                    RefreshBlackList();
                }
            }

            return TempPlayerInfoStruct;
        }


        void UpdatePlayerStructIfExist(PlayerInfoStruct strct)
        {
            for (int i = 0; i < AllPlayers.Count; i++)
            {
                if (AllPlayers[i].PlayerName.ToLower() == strct.PlayerName.ToLower() && AllPlayers[i].Is3x3 == strct.Is3x3)
                {
                    AllPlayers[i] = strct;
                    return;
                }
            }
        }

        PlayerInfoStruct LoadPlayerInfoStruct(string playername, bool Is3x3)
        {

            PlayerInfoStruct TempPlayerInfoStruct = new PlayerInfoStruct();

            if (IfBlacklisted(playername))
            {
                TempPlayerInfoStruct.allokay = false;
                TempPlayerInfoStruct.PlayerName = playername;
                TempPlayerInfoStruct.OldPlayerName = playername;
                return TempPlayerInfoStruct;
            }

            foreach (var strone in AllPlayers)
            {
                if (strone.PlayerName.ToLower() == playername.ToLower() && strone.Is3x3 == Is3x3)
                {
                    return strone;
                }
            }

            string FilePlayerInfoPath = cachedir + "\\" + playername + BoolIs3x3(Is3x3);
            TempPlayerInfoStruct.OldPlayerName = playername;
            TempPlayerInfoStruct.PlayerName = playername;
            TempPlayerInfoStruct.Is3x3 = Is3x3;

            if (File.Exists(FilePlayerInfoPath))
            {
                string[] PlayerInfoFileData = File.ReadAllLines(FilePlayerInfoPath);
                TempPlayerInfoStruct.PTS = int.Parse(PlayerInfoFileData[0]);
                TempPlayerInfoStruct.WinRate = int.Parse(PlayerInfoFileData[1]);
                TempPlayerInfoStruct.Win = int.Parse(PlayerInfoFileData[2]);
                TempPlayerInfoStruct.Lose = int.Parse(PlayerInfoFileData[3]);
                TempPlayerInfoStruct.Leaves = int.Parse(PlayerInfoFileData[4]);
                TempPlayerInfoStruct.WinStreak = int.Parse(PlayerInfoFileData[5]);
                TempPlayerInfoStruct.CurStreak = int.Parse(PlayerInfoFileData[6]);
                TempPlayerInfoStruct.allokay = true;
                if (PlayerInfoFileData.Length > 7)
                {
                    TempPlayerInfoStruct.skype = PlayerInfoFileData[7];
                    TempPlayerInfoStruct.flag = PlayerInfoFileData[8];
                    TempPlayerInfoStruct.allotherokay = true;
                }
                AllPlayers.Add(TempPlayerInfoStruct);
            }
            else
            {
                TempPlayerInfoStruct = LoadPlayerInfoStructFromIccup(playername, Is3x3);
                if (TempPlayerInfoStruct.allokay)
                {
                    SavePlayerInfoStructToCache(TempPlayerInfoStruct);
                    AllPlayers.Add(TempPlayerInfoStruct);
                }
            }


            return TempPlayerInfoStruct;
        }

        bool FillPlayerNameAndTypeByFilepath(ref PlayerInfoStruct str, string filepath)
        {
            string regex1 = @"(\w+)_______________\[3x3\].dat";
            string regex2 = @"(\w+)_______________\[5x5\].dat";
            string FileName = Path.GetFileName(filepath);
            Match GetPlayerAndTypeMatch = Regex.Match(FileName, regex1);
            if (GetPlayerAndTypeMatch.Success)
            {
                str.PlayerName = GetPlayerAndTypeMatch.Groups[1].Value;
                str.OldPlayerName = GetPlayerAndTypeMatch.Groups[1].Value;
                str.Is3x3 = true;
                return true;
            }
            GetPlayerAndTypeMatch = Regex.Match(FileName, regex2);
            if (GetPlayerAndTypeMatch.Success)
            {
                str.PlayerName = GetPlayerAndTypeMatch.Groups[1].Value;
                str.OldPlayerName = GetPlayerAndTypeMatch.Groups[1].Value;
                str.Is3x3 = false;
                return true;
            }

            return false;
        }

        PlayerInfoStruct LoadPlayerInfoStructFromCache(string filepath)
        {
            PlayerInfoStruct TempPlayerInfoStruct = new PlayerInfoStruct();

            string FilePlayerInfoPath = filepath;
            string[] PlayerInfoFileData = File.ReadAllLines(FilePlayerInfoPath);

            TempPlayerInfoStruct.PTS = int.Parse(PlayerInfoFileData[0]);
            TempPlayerInfoStruct.WinRate = int.Parse(PlayerInfoFileData[1]);
            TempPlayerInfoStruct.Win = int.Parse(PlayerInfoFileData[2]);
            TempPlayerInfoStruct.Lose = int.Parse(PlayerInfoFileData[3]);
            TempPlayerInfoStruct.Leaves = int.Parse(PlayerInfoFileData[4]);
            TempPlayerInfoStruct.WinStreak = int.Parse(PlayerInfoFileData[5]);
            TempPlayerInfoStruct.CurStreak = int.Parse(PlayerInfoFileData[6]);

            if (PlayerInfoFileData.Length > 7)
            {
                TempPlayerInfoStruct.skype = PlayerInfoFileData[7];
                TempPlayerInfoStruct.flag = PlayerInfoFileData[8];
                TempPlayerInfoStruct.allotherokay = true;
            }

            if (FillPlayerNameAndTypeByFilepath(ref TempPlayerInfoStruct, filepath))
                TempPlayerInfoStruct.allokay = true;

            return TempPlayerInfoStruct;
        }


        // FLAG+SKYPE  @"ls-inside.*?flags\/(\w\w).*?skype\:(\w+)";

        // 5x5 @"\<div\s+class\s*=\s*\""p-body.*?\""\s+id\s*=\s*\""main-stata-5x5\"".*?i-pts\""\>\s*(\w+).*?Leave.*?\<td\>\s*(\w)\s*\/\s*(\w)\s*\/\s*(\w)\s*.*?побед.*?\<td\>\s*(\w+).*?екущий.*?\<td\>\s*(\w+)";

        // Regex myRegex = new Regex(strRegex, RegexOptions.Singleline);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct SetFrameDataStruct
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 200)]
            public string FrameName;
            public int FrameId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
            public string Text;
            public FRAME_TYPE FrameType;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string PlayerName;
        }


        SetFrameDataStruct GlobalSetFrameDataStruct = new SetFrameDataStruct();


        string PlayerGeneralInfoString(ref PlayerInfoStruct playerinfo)
        {
            string result = string.Empty;
            PlayerInfoStringTicks++;
            if (PlayerInfoStringTicks <= 20)
            {
                result += "|c00FF8000PTS : [ |r|c0000FFFF" + playerinfo.PTS + "|r|c00FF8000 ] |r";
            }
            else if (PlayerInfoStringTicks <= 40)
            {
                result += "|c0000FF00WinRate : [ |r" + GetColorByWinRate(playerinfo.WinRate) + playerinfo.WinRate + "|r|c0000FF00 ] |r";

            }
            else if (PlayerInfoStringTicks <= 60)
            {
                result += "|c00FF8000WIN/LOOSE : [ |r|c0000FFFF" + playerinfo.Win + " / " + playerinfo.Lose + "|r|c00FF8000 ] |r";

            }
            else if (PlayerInfoStringTicks <= 80)
            {
                result += "|c0000FF00Leaves : [ |r|c00F86207" + playerinfo.Leaves + "|r|c0000FF00 ] |r";
            }
            else if (PlayerInfoStringTicks <= 100)
            {
                if (playerinfo.allotherokay && playerinfo.skype.Length > 1)
                    result += "|c00FF8000SKYPE: [ |r|c0000FFFF" + playerinfo.skype + "|r|c00FF8000 ] |r";
                else
                    result += "|c0000FF00WinRate : [ |r" + GetColorByWinRate(playerinfo.WinRate) + playerinfo.WinRate + "|r|c0000FF00 ] |r";
            }
            else if (PlayerInfoStringTicks <= 120)
            {
                result += "|c00FF8000Cur/Max streak: [ |r|c0000FFFF" + playerinfo.CurStreak + "/" + playerinfo.WinStreak + "|r|c00FF8000 ] |r";
            }
            else
            {
                PlayerInfoStringTicks = 0;
                result += "|c00FF8000PTS : [ |r|c0000FFFF" + playerinfo.PTS + "|r|c00FF8000 ] |r";
            }

            return result;
        }

        string PlayerOtherInfoString(PlayerInfoStruct playerinfo)
        {
            string result = string.Empty;
            if (playerinfo.allotherokay)
            {
                result += "|c00FF8000FLAG:[|r|c0000FFFF" + playerinfo.flag + "|r|c00FF8000]|r";

            }
            return result;
        }


        Process war3proc = null;
        Syringe.Injector war3inject = null;
        ProcessMemory war3mem = null;
        bool FirstFind = true;

        string GetColorByWinRate(int WinRate)
        {
            float WinRateMulted = Convert.ToSingle(WinRate) * 2.55f;
            byte RedByte = Convert.ToByte(255 - Convert.ToInt32(WinRateMulted));
            byte GreenByte = Convert.ToByte(Convert.ToInt32(WinRateMulted));
            return "|cBB" + RedByte.ToString("X2") + GreenByte.ToString("X2") + "10";
        }


        private void DataFinder_Tick()
        {
            FirstFind = true;
            while (true)
            {
                try
                {
                    SkipUpdate = false;
                    if (FirstFind)
                    {
                        if (Process.GetProcessesByName("war3").Length > 0)
                        {

                            if (Process.GetProcessesByName("war3").Length > 1)
                            {
                                MessageBox.Show("NEED ONLY ONE war3.exe PROC", "DOWN DETECTED :D");
                                Ticked = false;
                                Thread.Sleep(2000);
                                continue;
                            }

                            Thread.Sleep(2000);
                            if (Process.GetProcessesByName("war3").Length > 0)
                            {

                                if (Process.GetProcessesByName("war3").Length > 1)
                                {
                                    MessageBox.Show("NEED ONLY ONE war3.exe PROC", "DOWN DETECTED :D");
                                    Ticked = false;
                                    Thread.Sleep(2000);
                                    continue;
                                }



                                war3proc = Process.GetProcessesByName("war3")[0];
                                war3inject = new Syringe.Injector(war3proc);
                                war3mem = new ProcessMemory(war3proc.Id, war3proc.ProcessName);
                                war3mem.StartProcess();



                                Thread.Sleep(500);
                                if (war3mem.DllImageAddress("iCCupLobbyHelper.dll") <= 0)
                                {

                                    war3inject.EjectOnDispose = false;
                                    war3inject.InjectLibraryW(Directory.GetCurrentDirectory() + "\\iCCupLobbyHelper.dll");

                                    if (war3mem.DllImageAddress("iCCupLobbyHelper.dll") <= 0)
                                    {
                                        MessageBox.Show("Dll not found!");
                                    }
                                }

                                FirstFind = false;

                            }

                        }
                        else
                        {
                            Ticked = false;
                            Thread.Sleep(2000);
                            continue;
                        }
                    }

                    war3inject.CallExport<int>("iCCupLobbyHelper.dll", "UpdatePlayerNames", 0);

                    int PlayerInfoAddr = war3mem.DllImageAddress("iCCupLobbyHelper.dll") + 0x28AC0;
                    int PlayerCount = war3mem.ReadInt(PlayerInfoAddr);

                    if (war3mem.DllImageAddress("iCCupLobbyHelper.dll") <= 0)
                    {
                        FirstFind = true;
                        Thread.Sleep(1000);
                        continue;
                    }


                    for (int i = 0; i < PlayerCount; i++)
                    {
                        int PlayerNameOffset = war3mem.ReadInt(PlayerInfoAddr + 4 + 4 * i);
                        if (PlayerNameOffset < 1)
                            continue;
                        PlayerNameOffset = war3mem.ReadInt(PlayerNameOffset);
                        if (PlayerNameOffset < 1)
                            continue;
                        string PlayerName = war3mem.ReadStringWarcraft(PlayerNameOffset, 250);

                        PlayerName = Regex.Replace(PlayerName, @"\|c\w\w\w\w\w\w\w\w", string.Empty);
                        PlayerName = Regex.Replace(PlayerName, @"\|r", string.Empty);


                        if (PlayerName.Length == 0)
                            continue;
                        if (IfBlacklisted(PlayerName))
                            continue;


                        if ( PlayerName.Length > 15 || PlayerName.Length < 2 )
                        {
                            if (!IfBlacklisted(PlayerName))
                            {
                                AddToBlackList(PlayerName);
                                RefreshBlackList();
                            }
                            continue;
                        }

                        //    MessageBox.Show(PlayerName);
                        PlayerInfoStruct CurrentPlayerData = LoadPlayerInfoStruct(PlayerName, Is3x3Lobby.Checked);

                        if (!CurrentPlayerData.allokay)
                        {
                            AddToBlackList(PlayerName.ToLower());
                            RefreshBlackList();
                            continue;
                        }

                        if (SkipUpdate)
                            continue;

                        GlobalSetFrameDataStruct.FrameName = "NameMenu";
                        GlobalSetFrameDataStruct.FrameType = FRAME_TYPE.FRAME_MENU;
                        GlobalSetFrameDataStruct.FrameId = i;

                        GlobalSetFrameDataStruct.PlayerName = CurrentPlayerData.PlayerName;

                        if (FireMode.Checked)
                            GlobalSetFrameDataStruct.Text = GetFireUserName(PlayerName, ref CurrentPlayerData.FireOffset);
                        else
                            GlobalSetFrameDataStruct.Text = GetColorByWinRate(CurrentPlayerData.WinRate) + PlayerName + "|r";

                        CurrentPlayerData.OldPlayerName = GlobalSetFrameDataStruct.Text;

                        war3inject.CallExport<SetFrameDataStruct>("iCCupLobbyHelper.dll", "SetFrameDataText", GlobalSetFrameDataStruct);

                        GlobalSetFrameDataStruct.FrameName = "RaceMenu";

                        GlobalSetFrameDataStruct.Text = PlayerGeneralInfoString(ref CurrentPlayerData);
                        war3inject.CallExport<SetFrameDataStruct>("iCCupLobbyHelper.dll", "SetFrameDataText", GlobalSetFrameDataStruct);

                        string OtherInfo = PlayerOtherInfoString(CurrentPlayerData);
                        if (OtherInfo != string.Empty)
                        {
                            GlobalSetFrameDataStruct.FrameName = "HandicapMenu";

                            GlobalSetFrameDataStruct.Text = OtherInfo;
                            war3inject.CallExport<SetFrameDataStruct>("iCCupLobbyHelper.dll", "SetFrameDataText", GlobalSetFrameDataStruct);

                        }

                        UpdatePlayerStructIfExist(CurrentPlayerData);
                    }
                }
                catch
                {
                    FirstFind = true;
                }
                Thread.Sleep(80);
            }
        }

        private void LobbyManager_Load(object sender, EventArgs e)
        {
            client.Encoding = Encoding.UTF8;
            if (Directory.Exists(cachedir))
            {
                foreach (string file in Directory.GetFiles(cachedir))
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastAccessTime < DateTime.Now.AddHours(-5))
                        fi.Delete();
                    else
                    {
                        PlayerInfoStruct TempPlayerInfoStruct = LoadPlayerInfoStructFromCache(file);
                        if (!TempPlayerInfoStruct.allokay)
                        {
                            fi.Delete();
                        }
                        else
                        {
                            AllPlayers.Add(TempPlayerInfoStruct);
                        }
                    }
                }
            }
            else
                Directory.CreateDirectory(cachedir);

            if (File.Exists(BlackListFilePath))
            {
                BlackList.AddRange(File.ReadAllLines(BlackListFilePath));
            }


            new Thread(DataFinder_Tick).Start();
        }

        private void CacheCleaner_Tick(object sender, EventArgs e)
        {
            if (Directory.Exists(cachedir))
            {
                foreach (string file in Directory.GetFiles(cachedir))
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastAccessTime < DateTime.Now.AddHours(-5))
                        fi.Delete();
                }
            }
            else
                Directory.CreateDirectory(cachedir);
        }

        private void TestPlayerDataStr_Click(object sender, EventArgs e)
        {
            LoadPlayerInfoStruct(EnterPlayerNameForTest.Text, Is3x3Lobby.Checked);
        }

        private void LobbyManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            Ticked = true;
            if (war3inject != null)
                try
                {
                    war3inject.EjectLibraryXXX("iCCupLobbyHelper.dll");
                }
                catch
                {

                }
            Environment.Exit(0);

        }

        private void CheckForKick_Tick(object sender, EventArgs e)
        {

        }
    }
}
