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
		laststringcmp = 0;
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

struct SetFrameDataStruct
{
	char FrameName[200];
	int FrameId;
	char Text[512];
	WarcraftFramesClass::FRAME_TYPE ftype;
	char PlayerName[100];
};

DWORD __stdcall SetFrameDataText(SetFrameDataStruct* framedatatext)
{
	try
	{
		WarcraftFramesClass tmpclass = WarcraftFramesClass(framedatatext->FrameName, framedatatext->FrameId, framedatatext->ftype);
		WarcraftFramesClass tmpclass2 = WarcraftFramesClass("NameMenu", framedatatext->FrameId, WarcraftFramesClass::FRAME_MENU);
		if (tmpclass.GetFrameAddr() > 0)
		{
			if (tmpclass2.GetFrameAddr() > 0)
			{
				if (tmpclass2.FrameGetText() != NULL && framedatatext->PlayerName != NULL)
				{
					tmpclass.WriteTextSafe(framedatatext->Text);
				}
			}
		}

		VirtualFree(framedatatext, sizeof(SetFrameDataStruct), MEM_RELEASE);
	}
	catch (...)
	{

	}
	return 0;
}


struct PlayerDataStruct
{
	int players;
	int playersdata[15];
};

PlayerDataStruct GlobalPlayerStruct;

DWORD __stdcall UpdatePlayerNames(LPVOID)
{
	if (!NeedEject)
	{
		memset(&GlobalPlayerStruct, 0, sizeof(PlayerDataStruct));
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
		char fullPath[1024];
		GetModuleFileNameA(hDLL, fullPath, 1024);

		std::string detectorConfigPath = std::filesystem::path(fullPath).remove_filename().string();

		if (FileExist(detectorConfigPath+"printplayerinfo.txt"))
		{
			FILE* f = NULL;
			fopen_s(&f, (detectorConfigPath + "printplayerinfo.txt").c_str(), "w");
			if (f != NULL)
			{
				fprintf(f, "%X->%X", (int)&GlobalPlayerStruct.players - (int)hDLL, (int)&GlobalPlayerStruct - (int)hDLL);
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