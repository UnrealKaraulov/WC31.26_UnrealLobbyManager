#include <Windows.h>
#include <vector>
#include <TlHelp32.h>
#include <fstream>


using namespace std;

HANDLE LobbyWatcherId = 0;
int GameDll = 0;


LPVOID TlsValue;
DWORD TlsIndex;
DWORD _W3XTlsIndex;

DWORD GetIndex()
{
	return *(DWORD*)(_W3XTlsIndex);
}

DWORD GetW3TlsForIndex(DWORD index)
{
	DWORD pid = GetCurrentProcessId();
	THREADENTRY32 te32;
	HANDLE hSnap = CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD, pid);
	te32.dwSize = sizeof(THREADENTRY32);

	if (Thread32First(hSnap, &te32))
	{
		do
		{
			if (te32.th32OwnerProcessID == pid)
			{
				HANDLE hThread = OpenThread(THREAD_ALL_ACCESS, false, te32.th32ThreadID);
				CONTEXT ctx = { CONTEXT_SEGMENTS };
				LDT_ENTRY ldt;
				GetThreadContext(hThread, &ctx);
				GetThreadSelectorEntry(hThread, ctx.SegFs, &ldt);
				DWORD dwThreadBase = ldt.BaseLow | (ldt.HighWord.Bytes.BaseMid <<
					16) | (ldt.HighWord.Bytes.BaseHi << 24);
				CloseHandle(hThread);
				if (dwThreadBase == NULL)
					continue;
				DWORD* dwTLS = *(DWORD**)(dwThreadBase + 0xE10 + 4 * index);
				if (dwTLS == NULL)
					continue;
				return (DWORD)dwTLS;
			}
		} while (Thread32Next(hSnap, &te32));
	}

	return NULL;
}

void SetTlsForMe()
{
	TlsIndex = GetIndex();
	LPVOID tls = (LPVOID)GetW3TlsForIndex(TlsIndex);
	TlsValue = tls;
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
	WarcraftFramesClass(const char * framename)
	{
		if (!framename)
		{
			SetAllAddrs();
			FrameAddr = GetFrameAddress(framename, 0);
		}
	}

	WarcraftFramesClass(const char * framename, int frameid)
	{
		SetAllAddrs();
		FrameAddr = GetFrameAddress(framename, frameid);
	}

	WarcraftFramesClass(const char * framename, FRAME_TYPE FrameType)
	{
		SetAllAddrs();
		FrameAddr = GetFrameAddress(framename, 0);
		this->FrameType = FrameType;
	}

	WarcraftFramesClass(const char * framename, int frameid, FRAME_TYPE FrameType)
	{
		SetAllAddrs();
		FrameAddr = GetFrameAddress(framename, frameid);
		this->FrameType = FrameType;
	}

	WarcraftFramesClass(const char * framename, FRAME_TYPE FrameType, int GameDll)
	{
		SetAllAddrs();
		FrameAddr = GetFrameAddress(framename, 0);
		this->FrameType = FrameType;
		this->GameDll = GameDll;
	}

	WarcraftFramesClass(const char * framename, int frameid, FRAME_TYPE FrameType, int GameDll)
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
			return *(int *)(FrameAddr + 0x1EC) ? FrameAddr + 0x1EC : 0;
		case FRAME_MENU:
			int offset = *(int *)(FrameAddr + 0x1E4);
			if (offset)
			{
				offset = *(int *)(offset + 0x1E4);
				if (offset)
				{
					return *(int *)(offset + 0x1EC) ? offset + 0x1EC : 0;
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
			int offset = *(int *)(FrameAddr + 0x1E4);
			if (offset)
			{
				offset = *(int *)(offset + 0x1E4);
				return offset;
			}
			break;
		}
		return 0;
	}

	const char * FrameGetText()
	{
		int textaddr = FrameGetTextAddr();
		if (!textaddr)
		{
			return NULL;
		}
		const char * rettext = (char *)*(int*)textaddr;
		if (!rettext)
			return NULL;
		return rettext;
	}

	void WriteTextSafe(const char * text)
	{
		if (lockedframe) return;

		int textaddr = FrameGetTextAddr();
		if (!textaddr)
		{
			return;
		}

		if (text != NULL &&  FrameGetText() != NULL)
		{
			if (_stricmp(text, FrameGetText()) == 0)
				return;
		}

		SetFrameText(FrameGetTextFrameAddr(), text);

	}

	void WriteTextSafe(const char * text, int len)
	{
		if (lockedframe) return;

		int textaddr = FrameGetTextAddr();
		if (!textaddr)
		{
			return;
		}

		if (text != NULL &&  FrameGetText() != NULL)
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
			int offset = *(int *)(FrameAddr + 0x1E4);
			if (offset)
			{
				offset = *(int *)(offset + 0x1E4);
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

	typedef int(__fastcall * GetFrameAddress_p)(const char * name, int id);
	GetFrameAddress_p GetFrameAddress = 0;

	typedef void *(__thiscall * UpdateFrameTextSizep)(void *a1, int a2);
	UpdateFrameTextSizep UpdateFrameTextSize = 0;

	typedef void **(__thiscall * SetFrameText_p)(int frameaddr, const char * newtext);
	SetFrameText_p SetFrameText = 0;

	typedef int(__thiscall * FreeFrameText_p)(int frameaddr, int freetype);
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

	const char * hexstr = "0123456789ABCDEFabcdef";
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

DWORD __stdcall SetFrameDataText(SetFrameDataStruct * framedatatext)
{
	if (!NeedEject)
	{
		SetTlsForMe();
		
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


BOOL APIENTRY DllMain(HINSTANCE hinstDLL, DWORD reason, LPVOID)
{
	if (reason == DLL_PROCESS_ATTACH)
	{
		GameDll = (int)GetModuleHandle("Game.dll");
		_W3XTlsIndex = 0xAB7BF4 + GameDll;


		if (FileExist("printplayerinfo.txt"))
		{
			FILE * f;
			fopen_s(&f, "printplayerinfo.txt", "w");
			fprintf(f, "%X->%X", &GlobalPlayerStruct.players, &GlobalPlayerStruct);
			fclose(f);
		}

	}
	else if (reason == DLL_PROCESS_DETACH)
	{
		NeedEject = TRUE;
		Sleep(1500);
		memset(&GlobalPlayerStruct, 0, sizeof(PlayerDataStruct));
		
	}
	return TRUE;
}