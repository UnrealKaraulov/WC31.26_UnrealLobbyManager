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

namespace UnrealLobbyManager
{
    public partial class LobbyManager : Form
    {
        const string BanListFilePath = ".\\banlist.dat";


        public LobbyManager()
        {
            InitializeComponent();
        }

        bool ICCUP_MODE = true;

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
            public string ban_reason;
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

        bool IfBlacklisted(string username)
        {
            foreach (string str1 in BlackList)
            {
                string str3 = Encoding.Default.GetString(Encoding.UTF8.GetBytes(str1));
                if (str3.ToLower() == username.ToLower())
                    return true;
            }
            return false;
        }

        void AddToBlackList(string username)
        {
            if (!IfBlacklisted(username))
            {
                BlackList.Add(Encoding.UTF8.GetString(Encoding.Default.GetBytes(username)));
            }
        }

        bool IfBanlisted(string username, out string reason)
        {
            foreach (string str1 in BanList)
            {
                string str3 = Encoding.Default.GetString(Encoding.UTF8.GetBytes(str1));
                if (str3.ToLower().StartsWith((username + " ").ToLower())
                    || (str1.Length == username.Length && str3.ToLower().StartsWith((username + " ").ToLower())))
                {
                    reason = str3.Remove(0, username.Length + 1);
                    return true;
                }
            }
            reason = "";
            return false;
        }

        void AddToBanList(string username, string reason)
        {
            string zeroreason;
            if (!IfBanlisted(username, out zeroreason))
            {
                BanList.Add(Encoding.UTF8.GetString(Encoding.Default.GetBytes(username + " " + reason)));
            }
        }

        void DelFromBanList(string username)
        {
            List<string> outList = new List<string>();
            foreach (string str1 in BanList)
            {
                string str3 = Encoding.Default.GetString(Encoding.UTF8.GetBytes(str1));
                if (!str3.ToLower().StartsWith((username + " ").ToLower()))
                {
                    outList.Add(str1);
                }
            }

            BanList = outList;

            if (BanList.Count != outList.Count)
                RefreshBanList();
        }

        string BoolIs3x3(bool Is3x3)
        {
            if (Is3x3)
                return "_______________[3x3].dat";
            return "_______________[5x5].dat";
        }

        string DownloadUrl_ICCUP(string url, ref string resuri)
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


        void FillGeneralPlayerInfo_ICCUP(ref PlayerInfoStruct str)
        {
            string regex1 = @"i-pts\"">(.*?)<.*?\""k-num\"">K\s+(.*?)<.*>(\d+)\s+\/\s+(\d+)\s+\/\s+(\d+)<.*<td>(\d+)<\/td>.*<td>(-?\d+)<\/td>.*?desktop-view";
            string url1 = "https://iccup.com/dota/gamingprofile/" + Uri.EscapeUriString(str.PlayerName.ToLower()) + "/";
            string data = "";
            float waitcount = 1.0f;
            str.allokay = false;
        trynow:
            try
            {
                string retaddr = "";
                data = DownloadUrl_ICCUP(url1, ref retaddr);
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


        void FillOtherPlayerInfo_ICCUP(ref PlayerInfoStruct str)
        {
            str.allotherokay = false;
            if (!ActivateOtherInfo.Checked)
            {
                str.flag = string.Empty;
                return;
            }

            string regex1 = @"profile-header.*?alt=\""(\w+)\""\s+class=\""user--flag\""";

            string url1 = "https://iccup.com/dota/profile/view/" + str.PlayerName + "/";

            float waitcount = 1.0f;
            string data = "";

        trynow:
            try
            {
                string retaddr = "";
                data = DownloadUrl_ICCUP(url1, ref retaddr);
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
                FillGeneralPlayerInfo_ICCUP(ref TempPlayerInfoStruct);
                Thread.Sleep(50 + (new Random().Next(25, 125)));
                FillOtherPlayerInfo_ICCUP(ref TempPlayerInfoStruct);
                Thread.Sleep(10);
                if (!TempPlayerInfoStruct.allokay)
                {
                    if (!IfBlacklisted(playername))
                    {
                        AddToBlackList(playername);
                    }
                }
            }
            catch
            {
                if (!IfBlacklisted(playername))
                {
                    AddToBlackList(playername);
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

            if (IfBanlisted(playername, out TempPlayerInfoStruct.ban_reason))
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

            if (ICCUP_MODE)
                TempPlayerInfoStruct = LoadPlayerInfoStructFromIccup(playername, Is3x3);
            if (TempPlayerInfoStruct.allokay || !ICCUP_MODE)
            {
                AllPlayers.Add(TempPlayerInfoStruct);
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

        string PlayerGeneralInfoString(ref PlayerInfoStruct playerinfo)
        {
            string result = string.Empty;
            if (PlayerInfoStringTicks <= 20)
            {
                result += "|c00FF8000PTS : [ |r|c0000FFFF" + playerinfo.PTS + "|r|c00FF8000 ] |c0020E000KDA : [ |r|c0000FFFF" + (playerinfo.KDA / 10.0f) + "|r|c0020E000 ] |r";
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
                result += "|c00FF8000PTS : [ |r|c0000FFFF" + playerinfo.PTS + "|r|c00FF8000 ] |c0020E000KDA : [ |r|c0000FFFF" + (playerinfo.KDA / 10.0f) + "|r|c0020E000 ] |r";
            }

            return result;
        }

        string PlayerOtherInfoString(PlayerInfoStruct playerinfo)
        {
            string result = string.Empty;

            if (playerinfo.banned)
            {
                if (PlayerInfoStringTicks <= 20)
                {
                    result += "|c00FF2000BANNED!!!!|r";
                }
                else if (PlayerInfoStringTicks <= 40)
                {
                    if (playerinfo.ban_reason.Length > 8)
                    {
                        playerinfo.ban_reason = playerinfo.ban_reason[playerinfo.ban_reason.Length - 1].ToString() + playerinfo.ban_reason;
                        playerinfo.ban_reason = playerinfo.ban_reason.Remove(playerinfo.ban_reason.Length - 1);
                    }
                    result += "|c00FF4000" + playerinfo.ban_reason + "|r";
                }
                else if (PlayerInfoStringTicks <= 60)
                {
                    result += "|c00FF2000BANNED!!!!|r";
                }
                else if (PlayerInfoStringTicks <= 80)
                {
                    if (playerinfo.ban_reason.Length > 8)
                    {
                        playerinfo.ban_reason = playerinfo.ban_reason[playerinfo.ban_reason.Length - 1].ToString() + playerinfo.ban_reason;
                        playerinfo.ban_reason = playerinfo.ban_reason.Remove(playerinfo.ban_reason.Length - 1);
                    }
                    result += "|c00FF4000" + playerinfo.ban_reason + "|r";
                }
                else if (PlayerInfoStringTicks <= 100)
                {
                    result += "|c00FF2000BANNED!!!!|r";
                }
                else if (PlayerInfoStringTicks <= 120)
                {
                    if (playerinfo.ban_reason.Length > 8)
                    {
                        playerinfo.ban_reason = playerinfo.ban_reason[playerinfo.ban_reason.Length - 1].ToString() + playerinfo.ban_reason;
                        playerinfo.ban_reason = playerinfo.ban_reason.Remove(playerinfo.ban_reason.Length - 1);
                    }
                    result += "|c00FF4000" + playerinfo.ban_reason + "|r";
                }
                else
                {
                    result += "|c00FF2000BANNED!!!!|r";
                }
            }
            else if (playerinfo.allotherokay && ICCUP_MODE)
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

        void UpdateFormPlayerInfo(int slotid, string username, string maininfo, string otherinfo, bool isbanned)
        {
            try
            {
                username = Regex.Replace(username, @"\|c\w\w\w\w\w\w\w\w", string.Empty);
                username = Regex.Replace(username, @"\|r", string.Empty);
            }
            catch
            {

            }
            try
            {
                maininfo = Regex.Replace(maininfo, @"\|c\w\w\w\w\w\w\w\w", string.Empty);
                maininfo = Regex.Replace(maininfo, @"\|r", string.Empty);
            }
            catch
            {

            }
            try
            {
                otherinfo = Regex.Replace(otherinfo, @"\|c\w\w\w\w\w\w\w\w", string.Empty);
                otherinfo = Regex.Replace(otherinfo, @"\|r", string.Empty);

                otherinfo = otherinfo.Remove(14);
            }
            catch
            {

            }
            this.BeginInvoke((ThreadStart)delegate ()
            {

                switch (slotid)
                {
                    case 0:
                        name_lab1.Text = username;
                        mainInf_lab1.Text = maininfo;
                        other_lab1.Text = otherinfo;
                        ban_btn1.Text = isbanned ? "UNBAN" : "BAN";
                        break;
                    case 1:
                        name_lab2.Text = username;
                        mainInf_lab2.Text = maininfo;
                        other_lab2.Text = otherinfo;
                        ban_btn2.Text = isbanned ? "UNBAN" : "BAN";
                        break;
                    case 2:
                        name_lab3.Text = username;
                        mainInf_lab3.Text = maininfo;
                        other_lab3.Text = otherinfo;
                        ban_btn3.Text = isbanned ? "UNBAN" : "BAN";
                        break;
                    case 3:
                        name_lab4.Text = username;
                        mainInf_lab4.Text = maininfo;
                        other_lab4.Text = otherinfo;
                        ban_btn4.Text = isbanned ? "UNBAN" : "BAN";
                        break;
                    case 4:
                        name_lab5.Text = username;
                        mainInf_lab5.Text = maininfo;
                        other_lab5.Text = otherinfo;
                        ban_btn5.Text = isbanned ? "UNBAN" : "BAN";
                        break;
                    case 5:
                        name_lab6.Text = username;
                        mainInf_lab6.Text = maininfo;
                        other_lab6.Text = otherinfo;
                        ban_btn6.Text = isbanned ? "UNBAN" : "BAN";
                        break;
                    case 6:
                        name_lab7.Text = username;
                        mainInf_lab7.Text = maininfo;
                        other_lab7.Text = otherinfo;
                        ban_btn7.Text = isbanned ? "UNBAN" : "BAN";
                        break;
                    case 7:
                        name_lab8.Text = username;
                        mainInf_lab8.Text = maininfo;
                        other_lab8.Text = otherinfo;
                        ban_btn8.Text = isbanned ? "UNBAN" : "BAN";
                        break;
                    case 8:
                        name_lab9.Text = username;
                        mainInf_lab9.Text = maininfo;
                        other_lab9.Text = otherinfo;
                        ban_btn9.Text = isbanned ? "UNBAN" : "BAN";
                        break;
                    case 9:
                        name_lab10.Text = username;
                        mainInf_lab10.Text = maininfo;
                        other_lab10.Text = otherinfo;
                        ban_btn10.Text = isbanned ? "UNBAN" : "BAN";
                        break;
                    case 10:
                        name_lab11.Text = username;
                        mainInf_lab11.Text = maininfo;
                        other_lab11.Text = otherinfo;
                        ban_btn11.Text = isbanned ? "UNBAN" : "BAN";
                        break;
                    case 11:
                        name_lab12.Text = username;
                        mainInf_lab12.Text = maininfo;
                        other_lab12.Text = otherinfo;
                        ban_btn12.Text = isbanned ? "UNBAN" : "BAN";
                        break;
                    default:
                        break;
                }
            });
        }

        bool NeedPrintToolName = false;
        bool[] PrintStats = Enumerable.Repeat(false, 12).ToArray();


        int PrintStatsTick = 0;

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
                                if (war3mem.DllImageAddress("UnrealLobbyHelper.res") <= 0)
                                {
                                    war3inject.EjectOnDispose = false;
                                    war3inject.InjectLibraryW(Directory.GetCurrentDirectory() + "\\UnrealLobbyHelper.res");

                                    ICCUP_MODE = war3mem.DllImageAddress("iccwc3.icc") > 0;

                                    if (war3mem.DllImageAddress("UnrealLobbyHelper.res") <= 0)
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


                    if (war3mem == null || war3mem.DllImageAddress("UnrealLobbyHelper.res") <= 0)
                    {
                        FirstFind = true;
                        Thread.Sleep(1000);
                        continue;
                    }

                    int DllAddress = war3mem.DllImageAddress("UnrealLobbyHelper.res");

                    int PlayersOffset = 0x36E68;
                    int FrameNameOffset = 0x36EA8;
                    int FrameTypeOffset = 0x36E64;
                    int FrameIdOffset = 0x36C50;
                    int FrameTextOffset = 0x36C58;

                    war3inject.CallExport<int>("UnrealLobbyHelper.res", "UpdatePlayerNames", 0);
                    // printplayerinfo.txt offset
                    int PlayerInfoAddr = DllAddress + PlayersOffset;
                    int PlayerCount = war3mem.ReadInt(PlayerInfoAddr);

                    PrintStatsTick++;

                    PlayerInfoStringTicks++;
                    PlayerInfoStringTicks++;

                    if (new Random().Next(0, 200) > 100)
                        PlayerInfoStringTicks++;

                    if ( PlayerInfoStringTicks > 120)
                        PlayerInfoStringTicks = 0;

                    for (int i = 0; i < 12; i++)
                    {
                        if (i >= PlayerCount)
                        {
                            PrintStats[i] = false;
                            UpdateFormPlayerInfo(i, "Пусто", string.Empty, string.Empty, false);
                            continue;
                        }

                        int PlayerNameOffset = war3mem.ReadInt(PlayerInfoAddr + 4 + 4 * i);
                        if (PlayerNameOffset < 1)
                        {
                            PrintStats[i] = false;
                            UpdateFormPlayerInfo(i, "Пусто", string.Empty, string.Empty, false);
                            continue;
                        }
                        PlayerNameOffset = war3mem.ReadInt(PlayerNameOffset);
                        if (PlayerNameOffset < 1)
                        {
                            PrintStats[i] = false;
                            UpdateFormPlayerInfo(i, "Пусто", string.Empty, string.Empty, false);
                            continue;
                        }
                        string PlayerName = war3mem.ReadStringWarcraft(PlayerNameOffset, 250);

                        PlayerName = Regex.Replace(PlayerName, @"\|c\w\w\w\w\w\w\w\w", string.Empty);
                        PlayerName = Regex.Replace(PlayerName, @"\|r", string.Empty);

                        if (PlayerName.Length == 0)
                        {
                            PrintStats[i] = false;
                            UpdateFormPlayerInfo(i, "Пусто", string.Empty, string.Empty, false);
                            continue;
                        }
                        if (PlayerName.Length > 15 || PlayerName.Length < 2)
                        {
                            PrintStats[i] = false;
                            UpdateFormPlayerInfo(i, "Пусто", string.Empty, string.Empty, false);
                            continue;
                        }

                        PlayerInfoStruct CurrentPlayerData = LoadPlayerInfoStruct(PlayerName, false);

                        if (!CurrentPlayerData.allokay)
                        {
                            PrintStats[i] = false;
                            UpdateFormPlayerInfo(i, "Нет данных", string.Empty, string.Empty, false);
                            AddToBlackList(PlayerName.ToLower());
                            continue;
                        }

                        int AddrOffset = DllAddress + FrameNameOffset;
                        war3mem.WriteStringWarcraft(AddrOffset, "NameMenu");

                        AddrOffset = DllAddress + FrameTypeOffset;
                        war3mem.WriteInt(AddrOffset, Convert.ToInt32(FRAME_TYPE.FRAME_MENU));

                        AddrOffset = DllAddress + FrameIdOffset;
                        war3mem.WriteInt(AddrOffset, i);

                        string usernamestr = "";

                        if (FireMode.Checked)
                            usernamestr = GetFireUserName(PlayerName, ref CurrentPlayerData.FireOffset);
                        else
                            usernamestr = GetColorByWinRate(CurrentPlayerData.WinRate) + PlayerName + "|r";

                        AddrOffset = DllAddress + FrameTextOffset;
                        war3mem.WriteStringWarcraft(AddrOffset, usernamestr);

                        string mainuserstring = "";
                        string otheruserstring = "";

                        war3inject.CallExport<int>("UnrealLobbyHelper.res", "SetFrameDataText", 0);

                        if (ICCUP_MODE)
                        {
                            AddrOffset = DllAddress + FrameNameOffset;
                            war3mem.WriteStringWarcraft(AddrOffset, "RaceMenu");

                            mainuserstring = PlayerGeneralInfoString(ref CurrentPlayerData);

                            AddrOffset = DllAddress + FrameTextOffset;
                            war3mem.WriteStringWarcraft(AddrOffset, mainuserstring);

                            war3inject.CallExport<int>("UnrealLobbyHelper.res", "SetFrameDataText", 0);
                        }

                        string OtherInfo = PlayerOtherInfoString(CurrentPlayerData);

                        if (OtherInfo != string.Empty)
                        {
                            otheruserstring = OtherInfo;
                            AddrOffset = DllAddress + FrameNameOffset;
                            war3mem.WriteStringWarcraft(AddrOffset, "HandicapMenu");

                            AddrOffset = DllAddress + FrameTextOffset;
                            war3mem.WriteStringWarcraft(AddrOffset, otheruserstring);

                            war3inject.CallExport<int>("UnrealLobbyHelper.res", "SetFrameDataText", 0);
                        }

                        UpdateFormPlayerInfo(i, usernamestr, mainuserstring, otheruserstring, CurrentPlayerData.banned);
                        UpdatePlayerStructIfExist(CurrentPlayerData);


                        if (PrintStatsTick > 6)
                        {
                            if (NeedPrintToolName)
                            {
                                PrintStatsTick = 4;
                                NeedPrintToolName = false;
                                war3mem.WriteStringWarcraft(AddrOffset, " [ UnrealLobbyManager ] https://github.com/UnrealKaraulov/WC31.26_UnrealLobbyManager ");

                                war3inject.CallExport<int>("UnrealLobbyHelper.res", "SendStringToAll", 0);
                            }
                            else if (PrintStats[i])
                            {
                                PrintStats[i] = false;
                                AddrOffset = DllAddress + FrameTextOffset;

                                string printstats =
                                    "[ " + PlayerName + " ] : " +
                                    "PTS [ " + CurrentPlayerData.PTS + " ] " +
                                    "KDA [ " + (CurrentPlayerData.KDA / 10.0f) + " ] " +
                                    "WINRATE [ " + CurrentPlayerData.WinRate + " ] " +
                                    "LEAVES [ " + CurrentPlayerData.Leaves + " ] " +
                                    "FLAG [ " + CurrentPlayerData.flag + " ] ";

                                war3mem.WriteStringWarcraft(AddrOffset, printstats);

                                war3inject.CallExport<int>("UnrealLobbyHelper.res", "SendStringToAll", 0);
                                PrintStatsTick = 0;
                            }
                        }

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


            if (File.Exists(BanListFilePath))
            {
                BanList.AddRange(File.ReadAllLines(BanListFilePath));
            }


            new Thread(DataFinder_Tick).Start();
        }

        private void CacheCleaner_Tick(object sender, EventArgs e)
        {
        }


        private void LobbyManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (war3inject != null)
                try
                {
                    war3inject.EjectLibraryXXX("UnrealLobbyHelper.res");
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
                    CommandLineText.Text = "/printstats";
                    break;
                case 3:
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
                if (CommandLineText.Text.ToLower().IndexOf("/ban ") >= 0)
                {
                    string username = CommandLineText.Text.Replace("/ban ", "");
                    if (username.Split(" ".ToArray(), 2).Length == 1)
                        username += " NO REASON";
                    AddToBanList(username.Split(" ".ToArray(), 2)[0], username.Split(" ".ToArray(), 2)[1]);
                    RefreshBanList();
                }
                else if (CommandLineText.Text.ToLower().IndexOf("/unban ") >= 0)
                {
                    string username = CommandLineText.Text.Replace("/unban ", "");
                    DelFromBanList(username);
                    RefreshBanList();
                }
                else if (CommandLineText.Text.ToLower().IndexOf("/printstats") >= 0)
                {
                    PrintStats = Enumerable.Repeat(true, 12).ToArray();
                    NeedPrintToolName = true;
                }
                else if (CommandLineText.Text.ToLower().IndexOf("/exit") >= 0)
                {
                    if (war3inject != null)
                    {
                        try
                        {
                            war3inject.EjectLibraryXXX("UnrealLobbyHelper.res");
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

        private void ban_btn1_Click(object sender, EventArgs e)
        {
            if (name_lab1.Text.Length > 1)
            {
                CommandLineText.Text = "/" + ban_btn1.Text.ToLower() + " " + name_lab1.Text.ToLower();
            }
        }

        private void ban_btn2_Click(object sender, EventArgs e)
        {
            if (name_lab2.Text.Length > 1)
            {
                CommandLineText.Text = "/" + ban_btn2.Text.ToLower() + " " + name_lab2.Text.ToLower();
            }
        }

        private void ban_btn3_Click(object sender, EventArgs e)
        {
            if (name_lab3.Text.Length > 1)
            {
                CommandLineText.Text = "/" + ban_btn3.Text.ToLower() + " " + name_lab3.Text.ToLower();
            }
        }

        private void ban_btn4_Click(object sender, EventArgs e)
        {
            if (name_lab4.Text.Length > 1)
            {
                CommandLineText.Text = "/" + ban_btn4.Text.ToLower() + " " + name_lab4.Text.ToLower();
            }
        }

        private void ban_btn5_Click(object sender, EventArgs e)
        {
            if (name_lab5.Text.Length > 1)
            {
                CommandLineText.Text = "/" + ban_btn5.Text.ToLower() + " " + name_lab5.Text.ToLower();
            }
        }

        private void ban_btn6_Click(object sender, EventArgs e)
        {
            if (name_lab6.Text.Length > 1)
            {
                CommandLineText.Text = "/" + ban_btn6.Text.ToLower() + " " + name_lab6.Text.ToLower();
            }
        }

        private void ban_btn7_Click(object sender, EventArgs e)
        {
            if (name_lab7.Text.Length > 1)
            {
                CommandLineText.Text = "/" + ban_btn7.Text.ToLower() + " " + name_lab7.Text.ToLower();
            }
        }

        private void ban_btn8_Click(object sender, EventArgs e)
        {
            if (name_lab8.Text.Length > 1)
            {
                CommandLineText.Text = "/" + ban_btn8.Text.ToLower() + " " + name_lab8.Text.ToLower();
            }
        }

        private void ban_btn9_Click(object sender, EventArgs e)
        {
            if (name_lab9.Text.Length > 1)
            {
                CommandLineText.Text = "/" + ban_btn9.Text.ToLower() + " " + name_lab9.Text.ToLower();
            }
        }

        private void ban_btn10_Click(object sender, EventArgs e)
        {
            if (name_lab10.Text.Length > 1)
            {
                CommandLineText.Text = "/" + ban_btn10.Text.ToLower() + " " + name_lab10.Text.ToLower();
            }
        }

        private void ban_btn11_Click(object sender, EventArgs e)
        {
            if (name_lab11.Text.Length > 1)
            {
                CommandLineText.Text = "/" + ban_btn11.Text.ToLower() + " " + name_lab11.Text.ToLower();
            }
        }

        private void ban_btn12_Click(object sender, EventArgs e)
        {
            if (name_lab12.Text.Length > 1)
            {
                CommandLineText.Text = "/" + ban_btn12.Text.ToLower() + " " + name_lab12.Text.ToLower();
            }
        }
    }
}
