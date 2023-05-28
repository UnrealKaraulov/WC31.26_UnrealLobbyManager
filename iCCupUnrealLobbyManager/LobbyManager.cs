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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Globalization;

namespace iCCupUnrealLobbyManager
{
    public partial class LobbyManager : Form
    {
        const string cachedir = ".\\cachedir";
        const string BlackListFilePath = ".\\blacklist.dat";
        const string BanListFilePath = ".\\banlist.dat";

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
            public int KDA;
            public int WinRate;
            public int Win;
            public int Lose;
            public int Leaves;
            public int WinStreak;
            public int CurStreak;
            public string PlayerName;
            public string flag;
            public string OldPlayerName;
            public bool Is3x3;
            public bool allokay;
            public bool allotherokay;
            public bool banned;
            public int FireOffset;
        }


        List<PlayerInfoStruct> AllPlayers = new List<PlayerInfoStruct>();
        List<string> BlackList = new List<string>();
        List<string> BanList = new List<string>();

        public CookieContainer cookieContainer_0 = new CookieContainer();
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
            if (!IfBlacklisted(str))
            {
                BlackList.Add(Encoding.UTF8.GetString(Encoding.Default.GetBytes(str)));
            }
        }

        bool IfBanlisted(string str2)
        {
            foreach (string str1 in BanList)
            {
                string str3 = Encoding.Default.GetString(Encoding.UTF8.GetBytes(str1));
                if (str3.ToLower() == str2.ToLower())
                    return true;
            }
            return false;
        }

        void AddToBanList(string str)
        {
            if (!IfBanlisted(str))
            {
                BanList.Add(Encoding.UTF8.GetString(Encoding.Default.GetBytes(str)));
            }
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
            File.AppendAllText(FilePlayerInfoPath, str.KDA.ToString() + "\r\n");
            File.AppendAllText(FilePlayerInfoPath, str.WinRate.ToString() + "\r\n");
            File.AppendAllText(FilePlayerInfoPath, str.Win.ToString() + "\r\n");
            File.AppendAllText(FilePlayerInfoPath, str.Lose.ToString() + "\r\n");
            File.AppendAllText(FilePlayerInfoPath, str.Leaves.ToString() + "\r\n");
            File.AppendAllText(FilePlayerInfoPath, str.WinStreak.ToString() + "\r\n");
            File.AppendAllText(FilePlayerInfoPath, str.CurStreak.ToString() + "\r\n");
            if (str.allotherokay)
            {
                File.AppendAllText(FilePlayerInfoPath, str.flag.ToString() + "\r\n");
            }
        }
        string DownloadUrl(string url, ref string resuri)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "GET";
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            httpWebRequest.Referer = "https://iccup.com/";
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";
            httpWebRequest.CookieContainer = cookieContainer_0;
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Headers.Add("DNT", "1");
            httpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
            httpWebRequest.Headers.Add("Origin", "https://iccup.com");
            httpWebRequest.Headers.Add("Upgrade-Insecure-Requests", "1");
            httpWebRequest.ReadWriteTimeout = 5000;
            httpWebRequest.Timeout = 5000;

            //Thread.Sleep(100);
            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream);
            string end = streamReader.ReadToEnd();
            streamReader.Close();
            responseStream.Close();
            resuri = response.ResponseUri.AbsoluteUri;
            response.Close();
            return end;
        }


        void FillGeneralPlayerInfo(ref PlayerInfoStruct str)
        {
            string regex1 = @"i-pts\"">(.*?)<.*?\""k-num\"">K\s+(.*?)<.*>(\d+)\s+\/\s+(\d+)\s+\/\s+(\d+)<.*<td>(\d+)<\/td>.*<td>(-?\d+)<\/td>.*?desktop-view";
            string url1 = "https://iccup.com/dota/gamingprofile/" + Uri.EscapeUriString(str.PlayerName.ToLower());
            string data = "";
            float waitcount = 1.0f;
            str.allokay = false;
        trynow:
            try
            {
                string retaddr = "";
                data = DownloadUrl(url1, ref retaddr);
                if (retaddr.IndexOf("wrongid") > 0)
                {
                    return;
                }

                if (data.IndexOf("503 Service Temporarily Unavailable") > 0)
                {
                    Thread.Sleep(Convert.ToInt32(new Random().Next(500, 1000) * waitcount));
                    waitcount += 0.5f;

                    if (waitcount > 2.0f)
                    {
                        return;
                    }
                    goto trynow;
                }
            }
            catch
            {
                return;
            }
            // File.WriteAllText(".\\data.txt", data);
            //  File.WriteAllText(".\\data_regex.txt", regex1);
            Match PlayerDataFromUrl = Regex.Match(data, regex1, RegexOptions.Singleline);


            if (PlayerDataFromUrl.Success)
            {
                for (int i = 1; i < 7; i++)
                {
                    if (PlayerDataFromUrl.Groups[i].Value.Length <= 0)
                    {
                        str.allokay = false;
                        return;
                    }
                }
                str.PTS = int.Parse(PlayerDataFromUrl.Groups[1].Value);
                str.KDA = Convert.ToInt32(float.Parse(PlayerDataFromUrl.Groups[2].Value, CultureInfo.InvariantCulture) * 10);
                str.Win = int.Parse(PlayerDataFromUrl.Groups[3].Value);
                str.Lose = int.Parse(PlayerDataFromUrl.Groups[4].Value);
                str.Leaves = int.Parse(PlayerDataFromUrl.Groups[5].Value);


                str.WinStreak = int.Parse(PlayerDataFromUrl.Groups[6].Value);
                str.CurStreak = int.Parse(PlayerDataFromUrl.Groups[7].Value);
                if (str.Win == 0 && str.Lose == 0)
                    str.WinRate = 0;
                else
                    str.WinRate = Convert.ToInt32(Convert.ToSingle(str.Win) / Convert.ToSingle(str.Win + str.Lose) * 100.0f);
                str.allokay = true;
            }
        }


        void FillOtherPlayerInfo(ref PlayerInfoStruct str)
        {
            str.allotherokay = false;
            if (!ActivateOtherInfo.Checked)
            {
                str.flag = string.Empty;
                return;
            }

            string regex1 = @"alt=\""(\w+)\""\s+class=\""user--flag\""";

            string url1 = "https://iccup.com/dota/profile/view/" + str.PlayerName;

            float waitcount = 1.0f;
            string data = "";

        trynow:
            try
            {
                string retaddr = "";
                data = DownloadUrl(url1, ref retaddr);
                if (retaddr.IndexOf("wrongid") > 0)
                {
                    return;
                }
                if (data.IndexOf("503 Service Temporarily Unavailable") > 0)
                {
                    Thread.Sleep(Convert.ToInt32(new Random().Next(500, 1000) * waitcount));
                    waitcount += 0.5f;

                    if (waitcount > 2.0f)
                    {
                        return;
                    }
                    goto trynow;
                }
            }
            catch
            {
                return;
            }


            Match PlayerDataFromUrl = Regex.Match(data, regex1, RegexOptions.Singleline);
            if (PlayerDataFromUrl.Success)
            {
                str.flag = PlayerDataFromUrl.Groups[1].Value;
                str.allotherokay = true;
            }
        }

        void RefreshBlackList()
        {
            if (File.Exists(BlackListFilePath))
            {
                File.Delete(BlackListFilePath);
            }

            File.WriteAllLines(BlackListFilePath, BlackList.ToArray(), Encoding.UTF8);
        }
        void RefreshBanList()
        {
            if (File.Exists(BanListFilePath))
            {
                File.Delete(BanListFilePath);
            }

            File.WriteAllLines(BanListFilePath, BanList.ToArray(), Encoding.UTF8);
        }

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
                Thread.Sleep(50 + (new Random().Next(0, 100)));
                FillOtherPlayerInfo(ref TempPlayerInfoStruct);
                Thread.Sleep(10);
                if (!TempPlayerInfoStruct.allokay)
                {
                    if (!IfBlacklisted(playername))
                    {
                        AddToBlackList(playername);
                        RefreshBlackList();
                    }
                }
            }
            catch
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

            if (IfBanlisted(playername))
            {
                TempPlayerInfoStruct.allokay = true;
                TempPlayerInfoStruct.banned = true;
                TempPlayerInfoStruct.allotherokay = false;
                TempPlayerInfoStruct.PlayerName = playername;
                TempPlayerInfoStruct.OldPlayerName = playername;
                return TempPlayerInfoStruct;
            }

            if (IfBlacklisted(playername))
            {
                TempPlayerInfoStruct.allokay = false;
                TempPlayerInfoStruct.allotherokay = false;
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
                TempPlayerInfoStruct.KDA = int.Parse(PlayerInfoFileData[1]);
                TempPlayerInfoStruct.WinRate = int.Parse(PlayerInfoFileData[2]);
                TempPlayerInfoStruct.Win = int.Parse(PlayerInfoFileData[3]);
                TempPlayerInfoStruct.Lose = int.Parse(PlayerInfoFileData[4]);
                TempPlayerInfoStruct.Leaves = int.Parse(PlayerInfoFileData[5]);
                TempPlayerInfoStruct.WinStreak = int.Parse(PlayerInfoFileData[6]);
                TempPlayerInfoStruct.CurStreak = int.Parse(PlayerInfoFileData[7]);
                TempPlayerInfoStruct.allokay = true;
                if (PlayerInfoFileData.Length > 7)
                {
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
            TempPlayerInfoStruct.KDA = int.Parse(PlayerInfoFileData[1]);
            TempPlayerInfoStruct.WinRate = int.Parse(PlayerInfoFileData[2]);
            TempPlayerInfoStruct.Win = int.Parse(PlayerInfoFileData[3]);
            TempPlayerInfoStruct.Lose = int.Parse(PlayerInfoFileData[4]);
            TempPlayerInfoStruct.Leaves = int.Parse(PlayerInfoFileData[5]);
            TempPlayerInfoStruct.WinStreak = int.Parse(PlayerInfoFileData[6]);
            TempPlayerInfoStruct.CurStreak = int.Parse(PlayerInfoFileData[7]);

            if (PlayerInfoFileData.Length > 7)
            {
                TempPlayerInfoStruct.flag = PlayerInfoFileData[7];
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
            if (PlayerInfoStringTicks <= 20)
            {
                result += "|c00FF8000PTS : [ |r|c0000FFFF" + playerinfo.PTS + "|r|c00FF8000 ] |c0020E000KDA : [ |r|c0000FFFF" + (playerinfo.KDA/10.0f) + "|r|c0020E000 ] |r";
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

            if (playerinfo.banned)
            {
                result += "|c00FF2000BANNED!!!!|r";
            }
            else if (playerinfo.allotherokay)
            {
                result += "[|r|c0000FFFF" + playerinfo.flag + "|r]";
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
                    if (FirstFind)
                    {
                        if (Process.GetProcessesByName("war3").Length > 0)
                        {

                            if (Process.GetProcessesByName("war3").Length > 1)
                            {
                                MessageBox.Show("NEED ONLY ONE war3.exe PROC", "DOWN DETECTED :D");
                                Thread.Sleep(2000);
                                continue;
                            }

                            Thread.Sleep(2000);
                            if (Process.GetProcessesByName("war3").Length > 0)
                            {

                                if (Process.GetProcessesByName("war3").Length > 1)
                                {
                                    MessageBox.Show("NEED ONLY ONE war3.exe PROC", "DOWN DETECTED :D");
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
                            Thread.Sleep(2000);
                            continue;
                        }
                    }

                    if (war3mem.DllImageAddress("iCCupLobbyHelper.dll") <= 0)
                    {
                        FirstFind = true;
                        Thread.Sleep(1000);
                        continue;
                    }

                    war3inject.CallExport<int>("iCCupLobbyHelper.dll", "UpdatePlayerNames", 0);
                    // printplayerinfo.txt offset
                    int PlayerInfoAddr = war3mem.DllImageAddress("iCCupLobbyHelper.dll") + 0x36C58;
                    int PlayerCount = war3mem.ReadInt(PlayerInfoAddr);

                    PlayerInfoStringTicks++;

                    for (int i = 0; i < PlayerCount; i++)
                    {
                        int PlayerNameOffset = war3mem.ReadInt(PlayerInfoAddr + 4 + 4 * i);
                        if (PlayerNameOffset < 1)
                        {
                            continue;
                        }
                        PlayerNameOffset = war3mem.ReadInt(PlayerNameOffset);
                        if (PlayerNameOffset < 1)
                        {
                            continue;
                        }
                        string PlayerName = war3mem.ReadStringWarcraft(PlayerNameOffset, 250);

                        PlayerName = Regex.Replace(PlayerName, @"\|c\w\w\w\w\w\w\w\w", string.Empty);
                        PlayerName = Regex.Replace(PlayerName, @"\|r", string.Empty);

                        if (PlayerName.Length == 0)
                            continue;

                        if (PlayerName.Length > 15 || PlayerName.Length < 2)
                        {
                            continue;
                        }

                        PlayerInfoStruct CurrentPlayerData = LoadPlayerInfoStruct(PlayerName, Is3x3Lobby.Checked);

                        if (!CurrentPlayerData.allokay)
                        {
                            AddToBlackList(PlayerName.ToLower());
                            RefreshBlackList();
                            continue;
                        }

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
                Thread.Sleep(20);
            }
        }

        private void LobbyManager_Load(object sender, EventArgs e)
        {
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

            if (File.Exists(BanListFilePath))
            {
                BanList.AddRange(File.ReadAllLines(BanListFilePath));
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
            for (int i = 0; i < AllPlayers.Count; i++)
            {

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    CommandLineText.Text = "/ban ";
                    break;
                case 1:
                    CommandLineText.Text = "/unban ";
                    break;
                case 2:
                    CommandLineText.Text = "/exit";
                    break;
                default:
                    break;
            }
        }

        private void ActivateGodMode_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void CommandLineText_TextChanged(object sender, EventArgs e)
        {

        }

        private void CommandLineText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (CommandLineText.Text.IndexOf("/ban ") >= 0)
                {
                    string username = CommandLineText.Text.Replace("/ban ", "");
                    AddToBanList(username);
                    RefreshBanList();
                }
                else if (CommandLineText.Text.IndexOf("/unban ") >= 0)
                {
                    string username = CommandLineText.Text.Replace("/unban ", "");
                    BanList.Remove(username);
                    RefreshBanList();
                }
                else if (CommandLineText.Text.IndexOf("/exit") >= 0)
                {
                    if (war3inject != null)
                    {
                        try
                        {
                            war3inject.EjectLibraryXXX("iCCupLobbyHelper.dll");
                        }
                        catch
                        {

                        }
                    }
                    Environment.Exit(0);
                }
            }
        }

        private void WinStreakMax_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
