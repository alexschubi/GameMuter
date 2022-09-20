using System.Runtime.InteropServices;
using CSCore.CoreAudioAPI;
using System.Diagnostics;
using System.Timers;
using System.Media;

[DllImport("user32.dll")]
static extern short GetAsyncKeyState(int ArrowKeys);
[DllImport("user32.dll")]
static extern IntPtr SendMessageW(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
[DllImport("user32.dll")]
static extern IntPtr SendMessageA(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
[DllImport("user32.dll")]
static extern IntPtr PostMessageA(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

bool muted = false;
bool ingame = false;
MMDevice defaultAudio = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
muted = !IsAudioPlaying();



//GET Music APP
IntPtr getHandle(string name) {
    IntPtr hWnd = (IntPtr)0xffff;
    foreach (Process pList in Process.GetProcesses()) {
        if (pList.MainWindowTitle.Contains(name)) {
            hWnd = pList.MainWindowHandle;
        }
    }
    return hWnd;
}
Console.Write("Music-App-Name:");
string appname = "";
appname = Console.ReadLine();
if (appname == "") { Console.WriteLine(" using ALL APPS"); }
IntPtr hWnd = getHandle(appname);


//GET TIMEOUT
Console.Write("Timeout (in seconds):");
var inputTimeout = Console.ReadLine();
int timeout = 20;
try { 
    timeout = Convert.ToInt32(inputTimeout); 
} catch {
    Console.WriteLine(" using 20s");
}

//CHECK IF INGAME
int countdown = timeout;
IntPtr message = (IntPtr)1;
void runCheck() {
    while (true) {
        Thread.Sleep(1000);
        if ((GetAsyncKeyState(0xA0) < 0) && (GetAsyncKeyState(0x57) < 0) && ( (GetAsyncKeyState(0x41) < 0) ^ (GetAsyncKeyState(0x44) < 0)) ) {
            Console.WriteLine("pressing W and lShift and (A or D)");
            countdown = timeout;
            ingame = true;
        }
        if (countdown == 0 ) { 
            ingame = false;
            countdown = timeout;
        } else if (ingame == true) {
            countdown--; 
        }
        muted = !IsAudioPlaying();//TODO call not every time and it has delay
        switch ((ingame, muted)) {
            case (true, false): //MUTE
                Console.Write("Muting...");
                togglePlayState();
                Console.WriteLine(" success");
                break;
            case (false, true): //PLAY
                Console.Write("Playing...");
                togglePlayState();
                Console.WriteLine(" success");
                break;
        }
        //if (message != IntPtr.Zero) { muted = !IsAudioPlaying(); }
        Console.WriteLine($"countdown:{countdown}" + $" and ingame:{ingame}" + $" and muted:{muted}");
    }
}
runCheck();


//CONTROL MUTE / PLAY
void mute() {
    //SendMessageW((IntPtr)0xffff, 0x319, (IntPtr)0xffff, (IntPtr)0x2F000);
    if (!muted) {
        muted = true;
        togglePlayState();
        Console.WriteLine("Muting");
    }
}
void play() {
    //SendMessageW((IntPtr)0xffff, 0x319, (IntPtr)0xffff, (IntPtr)0x2E000);
    if (muted) {
        muted = false;
        togglePlayState();
        Console.WriteLine("Playing");
    }
}
void togglePlayState() {
    //from https://stackoverflow.com/questions/7181978/special-keys-on-keyboards/7182076#7182076 
    //SendNotifyMessage((IntPtr)0xffff, 0x319, (IntPtr)0xffff, (IntPtr)0xE0000);
    PostMessageA(hWnd, 0x319, hWnd, (IntPtr)0xE0000);

}



//HELPCLASS CHECK AUDIO PLAY STATE

// Gets the default device for the system
static MMDevice GetDefaultRenderDevice() {
    using (var enumerator = new MMDeviceEnumerator()) {
        return enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
    }
}
// Checks if audio is playing on a certain device
bool IsAudioPlaying() {
    using (var meter = AudioMeterInformation.FromDevice(defaultAudio)) {
        return meter.PeakValue > 0;
    }
}





//APP COMMANDS
int WM_APPCOMMAND = 0x0319;
void AppCommand(AppComandCode commandCode) {
    int CommandID = (int)commandCode << 16;
    SendMessageW(hWnd, WM_APPCOMMAND, hWnd, (IntPtr)CommandID);
}
enum AppComandCode : uint {
    BASS_BOOST = 20,
    BASS_DOWN = 19,
    BASS_UP = 21,
    BROWSER_BACKWARD = 1,
    BROWSER_FAVORITES = 6,
    BROWSER_FORWARD = 2,
    BROWSER_HOME = 7,
    BROWSER_REFRESH = 3,
    BROWSER_SEARCH = 5,
    BROWSER_STOP = 4,
    LAUNCH_APP1 = 17,
    LAUNCH_APP2 = 18,
    LAUNCH_MAIL = 15,
    LAUNCH_MEDIA_SELECT = 16,
    MEDIA_NEXTTRACK = 11,
    MEDIA_PLAY_PAUSE = 14,
    MEDIA_PREVIOUSTRACK = 12,
    MEDIA_STOP = 13,
    TREBLE_DOWN = 22,
    TREBLE_UP = 23,
    VOLUME_DOWN = 9,
    VOLUME_MUTE = 8,
    VOLUME_UP = 10,
    MICROPHONE_VOLUME_MUTE = 24,
    MICROPHONE_VOLUME_DOWN = 25,
    MICROPHONE_VOLUME_UP = 26,
    CLOSE = 31,
    COPY = 36,
    CORRECTION_LIST = 45,
    CUT = 37,
    DICTATE_OR_COMMAND_CONTROL_TOGGLE = 43,
    FIND = 28,
    FORWARD_MAIL = 40,
    HELP = 27,
    MEDIA_CHANNEL_DOWN = 52,
    MEDIA_CHANNEL_UP = 51,
    MEDIA_FASTFORWARD = 49,
    MEDIA_PAUSE = 47,
    MEDIA_PLAY = 46,
    MEDIA_RECORD = 48,
    MEDIA_REWIND = 50,
    MIC_ON_OFF_TOGGLE = 44,
    NEW = 29,
    OPEN = 30,
    PASTE = 38,
    PRINT = 33,
    REDO = 35,
    REPLY_TO_MAIL = 39,
    SAVE = 32,
    SEND_MAIL = 41,
    SPELL_CHECK = 42,
    UNDO = 34,
    DELETE = 53,
    DWM_FLIP3D = 54
}