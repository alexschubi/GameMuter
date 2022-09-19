using System.Runtime.InteropServices;
using CSCore.CoreAudioAPI;
using System.Diagnostics;
using System.Timers;
using System.Media;

[DllImport("user32.dll")]
static extern short GetAsyncKeyState(int ArrowKeys);

bool muted = false;
bool ingame = false;
MMDevice defaultAudio = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
muted = !IsAudioPlaying();


//FIRST INPUT FOR CONFIG
Console.Write("mute timeout in seconds (20):");
var inputTimeout = Console.ReadLine();
int timeout = 20;
try { 
    timeout = Convert.ToInt32(inputTimeout); 
} catch {
    Console.Write("Wrong Input!");
}

//CHECK IF INGAME
//Countdown
CountdownEvent countdown = new CountdownEvent(timeout);

//CancellationTokenSource cancelToken;
void runCheck() {
    while (true) {
        //muted = !IsAudioPlaying();
        Console.WriteLine("is muted:" + muted.ToString());
        Thread.Sleep(1000);
        if ((GetAsyncKeyState(0xA0) < 0) && (GetAsyncKeyState(0x57) < 0) || ( (GetAsyncKeyState(0x41) < 0) ^ (GetAsyncKeyState(0x44) < 0)) ) {
            Console.WriteLine("pressing W and lShift and (A or D)");
            ingame = true;
            mute();
            countdown.Reset();

        }

        if (countdown.CurrentCount == 0) { 
            play();
        } else {
            countdown.Signal(); 
        }
    }
}
runCheck();


//CONTROL MUTE / PLAY
//win32 dll
[DllImport("user32.dll")]
static extern IntPtr SendNotifyMessageA(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

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
    SendNotifyMessageA((IntPtr)0xffff, 0x319, (IntPtr)0xffff, (IntPtr)0xE0000);
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