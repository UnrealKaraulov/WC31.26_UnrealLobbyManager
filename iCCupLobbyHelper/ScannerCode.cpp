#define WINDOWS_IGNORE_PACKING_MISMATCH
#define _WIN32_WINNT 0x0501 
#define WINVER 0x0501 
#define NTDDI_VERSION 0x05010000
#define WIN32_LEAN_AND_MEAN
#define PSAPI_VERSION 1

#include <Windows.h>
#include <vector>
#include <TlHelp32.h>
#include <fstream>
#include <filesystem>

using namespace std;

HANDLE LobbyWatcherId = 0;
int GameDll = 0;

int IsGame()
{
	if (!GameDll)
		return 0;

	unsigned char* _GameUI = (unsigned char*)(GameDll + 0x93631C);
	unsigned char* InGame = (unsigned char*)(GameDll + 0xACE66C);

	return *(unsigned char**)InGame && **(unsigned char***)InGame == _GameUI;
}


BOOL NeedEject = FALSE;



class WarcraftFramesClass
{

public:
	// Supported frames
	enum FRAME_TYPE :int
	{
		FRAME_TEXT,
		FRAME_MENU,
		FRAME_BUTTON,
		FRAME_UNKNOWN
	};


	~WarcraftFramesClass()
	{
		Destructor();
	}

	WarcraftFramesClass()
	{
		SetAllAddrs();
		FrameAddr = 0;
	}
	WarcraftFramesClass(const char* framename)
	{
		if (!framename)
		{
			SetAllAddrs();
			FrameAddr = GetFrameAddress(framename, 0);
		}
	}

	WarcraftFramesClass(const char* framename, int frameid)
	{
		SetAllAddrs();
		FrameAddr = GetFrameAddress(framename, frameid);
	}

	WarcraftFramesClass(const char* framename, FRAME_TYPE FrameType)
	{
		SetAllAddrs();
		FrameAddr = GetFrameAddress(framename, 0);
		this->FrameType = FrameType;
	}

	WarcraftFramesClass(const char* framename, int frameid, FRAME_TYPE FrameType)
	{
		SetAllAddrs();
		FrameAddr = GetFrameAddress(framename, frameid);
		this->FrameType = FrameType;
	}

	WarcraftFramesClass(const char* framename, FRAME_TYPE FrameType, int GameDll)
	{
		SetAllAddrs();
		FrameAddr = GetFrameAddress(framename, 0);
		this->FrameType = FrameType;
		this->GameDll = GameDll;
	}

	WarcraftFramesClass(const char* framename, int frameid, FRAME_TYPE FrameType, int GameDll)
	{
		SetAllAddrs();
		FrameAddr = GetFrameAddress(framename, frameid);
		this->FrameType = FrameType;
		this->GameDll = GameDll;
	}

	void SetGameDllAddr(int GameDll)
	{
		this->GameDll = GameDll;
	}

	void SetGetFrameAddr(int GetFrameAddress)
	{
		this->GetFrameAddress = (GetFrameAddress_p)GetFrameAddress;
	}

	void SetFrameType(FRAME_TYPE FrameType)
	{
		this->FrameType = FrameType;
	}

	int GetFrameAddr()
	{
		return FrameAddr;
	}

	int FrameGetTextAddr()
	{
		if (!FrameAddr)
			return 0;

		switch (FrameType)
		{
		case FRAME_TEXT:
		case FRAME_BUTTON:
			return *(int*)(FrameAddr + 0x1EC) ? FrameAddr + 0x1EC : 0;
		case FRAME_MENU:
			int offset = *(int*)(FrameAddr + 0x1E4);
			if (offset)
			{
				offset = *(int*)(offset + 0x1E4);
				if (offset)
				{
					return *(int*)(offset + 0x1EC) ? offset + 0x1EC : 0;
				}
			}
			break;
		}
		return 0;
	}


	int FrameGetTextFrameAddr()
	{
		if (!FrameAddr)
			return 0;

		switch (FrameType)
		{
		case FRAME_TEXT:
		case FRAME_BUTTON:
			return FrameAddr;
		case FRAME_MENU:
			int offset = *(int*)(FrameAddr + 0x1E4);
			if (offset)
			{
				offset = *(int*)(offset + 0x1E4);
				return offset;
			}
			break;
		}
		return 0;
	}

	const char* FrameGetText()
	{
		int textaddr = FrameGetTextAddr();
		if (!textaddr)
		{
			return NULL;
		}
		const char* rettext = (char*)*(int*)textaddr;
		if (!rettext)
			return NULL;
		return rettext;
	}

	void WriteTextSafe(const char* text)
	{
		if (lockedframe) return;

		int textaddr = FrameGetTextAddr();
		if (!textaddr)
		{
			return;
		}

		if (text != NULL && FrameGetText() != NULL)
		{
			if (_stricmp(text, FrameGetText()) == 0)
				return;
		}

		SetFrameText(FrameGetTextFrameAddr(), text);

	}

	void WriteTextSafe(const char* text, int len)
	{
		if (lockedframe) return;

		int textaddr = FrameGetTextAddr();
		if (!textaddr)
		{
			return;
		}

		if (text != NULL && FrameGetText() != NULL)
		{
			if (_stricmp(text, FrameGetText()) == 0)
				return;
		}

		SetFrameText(FrameGetTextFrameAddr(), text);
	}

	int GetFrameTextAddr()
	{
		if (!FrameAddr)
			return 0;

		switch (FrameType)
		{
		case FRAME_TEXT:
		case FRAME_BUTTON:
			return FrameAddr;
		case FRAME_MENU:
			int offset = *(int*)(FrameAddr + 0x1E4);
			if (offset)
			{
				offset = *(int*)(offset + 0x1E4);
				if (offset)
				{
					return offset;
				}
			}
			break;
		}
		return 0;
	}

	bool IsLocked()
	{
		return lockedframe;
	}

	void Lock()
	{
		lockedframe = true;
	}

	void UnLock()
	{
		lockedframe = false;
	}

	void Reset()
	{
		Destructor();
		SetAllAddrs();
	}

private:

	typedef int(__fastcall* GetFrameAddress_p)(const char* name, int id);
	GetFrameAddress_p GetFrameAddress = 0;

	typedef void* (__thiscall* UpdateFrameTextSizep)(void* a1, int a2);
	UpdateFrameTextSizep UpdateFrameTextSize = 0;

	typedef void** (__thiscall* SetFrameText_p)(int frameaddr, const char* newtext);
	SetFrameText_p SetFrameText = 0;

	typedef int(__thiscall* FreeFrameText_p)(int frameaddr, int freetype);
	FreeFrameText_p FreeFrameText = 0;

	int GameDll = 0;
	int FrameAddr = 0;

	FRAME_TYPE FrameType = FRAME_UNKNOWN;

	bool laststringcmp = false;
	bool sizechanged = false;
	bool lockedframe = false;

	void SetAllAddrs()
	{
		hexstrlen = strlen(hexstr);

		if (GameDll == 0)
		{
			GameDll = (int)GetModuleHandle("Game.dll");
		}

		if (GetFrameAddress == 0)
		{
			GetFrameAddress = (GetFrameAddress_p)(GameDll + 0x5FA970);
		}

		if (UpdateFrameTextSize == 0)
		{
			UpdateFrameTextSize = (UpdateFrameTextSizep)(GameDll + 0x605CC0);
		}

		if (SetFrameText == 0)
		{
			SetFrameText = (SetFrameText_p)(GameDll + 0x611D40);
		}

		if (FreeFrameText == 0)
		{
			FreeFrameText = (FreeFrameText_p)(GameDll + 0x611A80);
		}

	}

	void Destructor()
	{

		FrameAddr = 0;
		GetFrameAddress = 0;
		UpdateFrameTextSize = 0;
		GameDll = 0;
		laststringcmp = false;
		sizechanged = false;
		lockedframe = false;
		hexstrlen = 0;

		FrameType = FRAME_UNKNOWN;
	}

	const char* hexstr = "0123456789ABCDEFabcdef";
	int hexstrlen = 0;

	bool IsHex(char c)
	{
		for (int i = 0; i < hexstrlen; i++)
		{
			if (hexstr[i] == c)
				return true;
		}
		return false;
	}

};


struct PlayerDataStruct
{
	int players;
	int playersdata[15];
};


PlayerDataStruct GlobalPlayerStruct;

char GLOBAL_FrameName[200];
char GLOBAL_Text[512];
int GLOBAL_FrameId;
WarcraftFramesClass::FRAME_TYPE GLOBAL_ftype;

DWORD LastFrameMissingTime = 0;

DWORD __stdcall SetFrameDataText(int)
{
	if (!IsGame())
	{
		try
		{
			WarcraftFramesClass* tmpclass3 = new WarcraftFramesClass("NameMenu", 0, WarcraftFramesClass::FRAME_MENU);
			WarcraftFramesClass* tmpclass = new WarcraftFramesClass(GLOBAL_FrameName, GLOBAL_FrameId, GLOBAL_ftype);
			WarcraftFramesClass* tmpclass2 = new WarcraftFramesClass("NameMenu", GLOBAL_FrameId, WarcraftFramesClass::FRAME_MENU);
			if (tmpclass->GetFrameAddr() > 0)
			{
				if (tmpclass2->GetFrameAddr() > 0)
				{
					if (tmpclass2->FrameGetText() != NULL)
					{
						if (GetTickCount() - LastFrameMissingTime > 1500)
							tmpclass->WriteTextSafe(GLOBAL_Text);
					}
				}
			}

			if (!tmpclass3)
			{
				LastFrameMissingTime = GetTickCount();
			}

			delete tmpclass;
			delete tmpclass2;
			delete tmpclass3;
		}
		catch (...)
		{

		}
	}
	return 0;
}



//
//unsigned char* GetBnetChatSock()
//{
//	int netaddr = sub_6F53E6B0();
//	int bnetChat = GetBnetSockStr(GameDll + 0xAD0090, 0, netaddr, 0, &netaddr, 0, 1);
//	if (bnetChat > 0)
//	{
//		return *(unsigned char**)(bnetChat + 0x414);
//	}
//	return 0;
//}
//

unsigned char* GetHostBotChatSock()
{
	int hostbotsock = *(int*)(GameDll + 0xACFFA4);
	if (hostbotsock > 0)
	{
		hostbotsock = *(int*)(hostbotsock + 0x148);
		if (hostbotsock > 0)
		{
			return *(unsigned char**)(hostbotsock + 0x3C);
		}
	}
	return 0;
}
typedef void(__fastcall* GAME_SendPacketDir_p) (unsigned char* tcpoffs, unsigned char* zero, unsigned char* packetData, int len);
GAME_SendPacketDir_p GAME_SendPacketDir;

void SendData(unsigned char* sockstr, unsigned char header, unsigned char packetid, unsigned char* data, int datalen)
{
	if (!sockstr)
		return;

	std::vector<unsigned char> send_data_buf;
	short totallen = datalen + 4;
	send_data_buf.push_back(header);
	send_data_buf.push_back(packetid);
	send_data_buf.push_back(((unsigned char*)&totallen)[0]);
	send_data_buf.push_back(((unsigned char*)&totallen)[1]);

	for (int i = 0; i < datalen; i++)
	{
		send_data_buf.push_back(data[i]);
	}

	if (sockstr && *(int*)sockstr > 0 && *(int*)(*(int*)sockstr + 44) > 0)
		GAME_SendPacketDir(sockstr, 0, &send_data_buf[0], (int)send_data_buf.size());
}

unsigned char GetLocalPid()
{
	int hostbotsock = *(int*)(GameDll + 0xACFFA4);
	if (hostbotsock > 0)
	{
		hostbotsock = *(int*)(hostbotsock + 0x148);
		if (hostbotsock > 0)
		{
			return *(unsigned char*)(hostbotsock + 0xB4);
		}
	}
	return 0;
}

void SendMapSize(unsigned char flags, unsigned int mapsize)
{
	unsigned char* hostbotsocket = GetHostBotChatSock();
	if (hostbotsocket)
	{
		std::vector<unsigned char> senddata;
		senddata.push_back(((unsigned char*)&mapsize)[0]);
		senddata.push_back(((unsigned char*)&mapsize)[1]);
		senddata.push_back(((unsigned char*)&mapsize)[2]);
		senddata.push_back(((unsigned char*)&mapsize)[3]);
		senddata.push_back(flags);
		senddata.push_back(((unsigned char*)&mapsize)[0]);
		senddata.push_back(((unsigned char*)&mapsize)[1]);
		senddata.push_back(((unsigned char*)&mapsize)[2]);
		senddata.push_back(((unsigned char*)&mapsize)[3]);
		SendData(hostbotsocket, 0xF7, 0x3F, &senddata[0], (int)senddata.size());
	}
}

void StartPongToPing()
{
	unsigned char* hostbotsocket = GetHostBotChatSock();
	if (hostbotsocket)
	{
		unsigned int data = rand();
		std::vector<unsigned char> senddata;
		senddata.push_back(((unsigned char*)&data)[0]);
		senddata.push_back(((unsigned char*)&data)[1]);
		senddata.push_back(((unsigned char*)&data)[2]);
		senddata.push_back(((unsigned char*)&data)[3]);
		SendData(hostbotsocket, 0xF7, 0x46, &senddata[0], (int)senddata.size());
	}
}

void StartMapDownload()
{
	unsigned char* hostbotsocket = GetHostBotChatSock();
	if (hostbotsocket)
	{
		std::vector<unsigned char> senddata;
		SendData(hostbotsocket, 0xF7, 0x3F, &senddata[0], (int)senddata.size());
	}
}

void SendGproxyReconnect()
{
	unsigned char* hostbotsocket = GetHostBotChatSock();
	if (hostbotsocket)
	{
		std::vector<unsigned char> senddata;
		SendData(hostbotsocket, 0xF8, 1, &senddata[0], (int)senddata.size());
	}
}

void StopMapDownload()
{
	unsigned char* hostbotsocket = GetHostBotChatSock();
	if (hostbotsocket)
	{
		std::vector<unsigned char> senddata;
		SendData(hostbotsocket, 0xF7, 0x23, &senddata[0], (int)senddata.size());
	}
}

void ChangeHandicap(unsigned char percent)
{
	unsigned char* hostbotsocket = GetHostBotChatSock();
	if (hostbotsocket)
	{
		std::vector<unsigned char> senddata;
		senddata.push_back(16);
		// msg and flags
		for (int i = 0; i < 16; i++)
		{
			senddata.push_back(i);
		}
		senddata.push_back(GetLocalPid());
		senddata.push_back(0x14);
		// percent
		senddata.push_back(percent);
		SendData(hostbotsocket, 0xF7, 0x28, &senddata[0], (int)senddata.size());
	}
}


void ChangeTeam(unsigned char team)
{
	unsigned char* hostbotsocket = GetHostBotChatSock();
	if (hostbotsocket)
	{
		//F728090001FF021100
		std::vector<unsigned char> senddata;
		senddata.push_back(16);
		// msg and flags
		for (int i = 0; i < 16; i++)
		{
			senddata.push_back(i);
		}
		senddata.push_back(GetLocalPid());
		senddata.push_back(0x11);
		// team
		senddata.push_back(team);
		SendData(hostbotsocket, 0xF7, 0x28, &senddata[0], (int)senddata.size());
	}
}

void ChangeColour(unsigned char color)
{
	unsigned char* hostbotsocket = GetHostBotChatSock();
	if (hostbotsocket)
	{
		std::vector<unsigned char> senddata;
		senddata.push_back(16);
		// msg and flags
		for (int i = 0; i < 16; i++)
		{
			senddata.push_back(i);
		}
		senddata.push_back(GetLocalPid());
		senddata.push_back(0x12);
		// color
		senddata.push_back(color);
		SendData(hostbotsocket, 0xF7, 0x28, &senddata[0], (int)senddata.size());
	}
}

void ChangeExtraFlags(unsigned int flags, std::string msg)
{
	unsigned char* hostbotsocket = GetHostBotChatSock();
	if (hostbotsocket)
	{
		std::vector<unsigned char> senddata;
		senddata.push_back(16);
		// msg and flags
		for (int i = 0; i < 16; i++)
		{
			senddata.push_back(i);
		}
		senddata.push_back(GetLocalPid());
		senddata.push_back(0x12);
		// flags
		senddata.push_back(((unsigned char*)&flags)[0]);
		senddata.push_back(((unsigned char*)&flags)[1]);
		senddata.push_back(((unsigned char*)&flags)[2]);
		senddata.push_back(((unsigned char*)&flags)[3]);
		for (auto& c : msg)
			senddata.push_back(c);
		senddata.push_back(0x00);


		SendData(hostbotsocket, 0xF7, 0x28, &senddata[0], (int)senddata.size());
	}
}
// sub_6F676C70 (sock?, packid, playerid, toplayeridoffset, toplayeridsize, packeddata, packetsize);
void SendChatMessageBot(const std::string& msg)
{
	unsigned char* hostbotsocket = GetHostBotChatSock();
	if (hostbotsocket)
	{
		std::vector<unsigned char> senddata;
		senddata.push_back(16);
		// msg and flags
		for (int i = 0; i < 16; i++)
		{
			senddata.push_back(i);
		}
		senddata.push_back(GetLocalPid());

		senddata.push_back(0x10);

	/*	senddata.push_back(3);
		senddata.push_back(3);
		senddata.push_back(3);
		senddata.push_back(3);*/

		// msg
		for (auto& c : msg)
			senddata.push_back(c);
		senddata.push_back(0x00);

		SendData(hostbotsocket, 0xF7, 0x28, &senddata[0], (int)senddata.size());

		//senddata.clear();
		//senddata.push_back(0x01);
		//// msg and flags
		//senddata.push_back(0x02);
		//senddata.push_back(0x01);
		//senddata.push_back(0x10);
		//// msg
		//for (auto& c : msg)
		//	senddata.push_back(c);
		//senddata.push_back(0x00);

		//SendData(hostbotsocket, 0xF7, 0x0F, &senddata[0], (int)senddata.size());
	}
}


DWORD __stdcall SendStringToAll(int)
{
	SendChatMessageBot(GLOBAL_Text);
	return 0;
}

DWORD __stdcall UpdatePlayerNames(LPVOID)
{
	if (!NeedEject)
	{
		memset(&GlobalPlayerStruct, 0, sizeof(PlayerDataStruct));

		if (!IsGame())
		{
			//16014
			for (int i = 0; i < 15; i++)
			{
				WarcraftFramesClass tmpclass = WarcraftFramesClass("NameMenu", i, WarcraftFramesClass::FRAME_MENU);
				if (tmpclass.GetFrameAddr() > 0)
				{
					GlobalPlayerStruct.players++;
					GlobalPlayerStruct.playersdata[i] = tmpclass.FrameGetTextAddr();
				}
			}
		}
	}
	return 0;
}


bool FileExist(const std::string& name) {
	ifstream f(name.c_str());
	return f.good();
}

bool LoadSuccess = false;
BOOL __stdcall DllMain(HINSTANCE hDLL, unsigned int r, LPVOID)
{
	if (r == DLL_PROCESS_ATTACH)
	{
		GameDll = (int)GetModuleHandle("Game.dll");
		if (!GetModuleHandleA("Game.dll"))
		{
			return FALSE;
		}

		GAME_SendPacketDir = (GAME_SendPacketDir_p)(GameDll + 0x6DF040);

		char fullPath[1024];
		GetModuleFileNameA(hDLL, fullPath, 1024);

		std::string detectorConfigPath = std::filesystem::path(fullPath).remove_filename().string();

		if (FileExist(detectorConfigPath + "printplayerinfo.txt"))
		{
			FILE* f = NULL;
			fopen_s(&f, (detectorConfigPath + "printplayerinfo.txt").c_str(), "w");
			if (f != NULL)
			{
				fprintf(f, "%X->%X\n", (int)&GlobalPlayerStruct.players - (int)hDLL, (int)&GlobalPlayerStruct - (int)hDLL);
				fprintf(f, "%X\n", (int)&GLOBAL_FrameName[0] - (int)hDLL);
				fprintf(f, "%X\n", (int)&GLOBAL_ftype - (int)hDLL);
				fprintf(f, "%X\n", (int)&GLOBAL_FrameId - (int)hDLL);
				fprintf(f, "%X\n", (int)&GLOBAL_Text[0] - (int)hDLL);
				fclose(f);
			}
		}
		LoadSuccess = true;
	}
	else if (r == DLL_PROCESS_DETACH)
	{
		memset(&GlobalPlayerStruct, 0, sizeof(PlayerDataStruct));
	}

	return TRUE;
}