using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

using InstanTTS.Audio;
using InstanTTS.Interface;
using InstanTTS.Speech;

namespace InstanTTS
{
    /// <summary>
    /// Base class for the application. Handles startup and exit tasks.
    /// </summary>
    public partial class App : Application
    {
        public static App Instance { get; private set; }

        private const int HotkeyCooldown = 100;

        private SpeechSynthManager speechSynthManager;
        private AudioManager audioManager;
        private HotkeyManager hotkeyManager;
        private GlobalKeyboardHook globalKeyboardHook;
        private MainWindow applicationWindow;

        private int lastHotkeyTime = 0;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            audioManager = new AudioManager();
            speechSynthManager = new SpeechSynthManager();
            hotkeyManager = new HotkeyManager();
            globalKeyboardHook = new GlobalKeyboardHook();
            globalKeyboardHook.KeyboardPressed += OnKeyPressed;

            Instance = this;

            applicationWindow = new MainWindow();
            applicationWindow.Show();
        }
        
        private void OnKeyPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (!ApplicationIsActivated() && (lastHotkeyTime + HotkeyCooldown) <= Environment.TickCount)
            {
                Key key = KeyInterop.KeyFromVirtualKey(e.KeyboardData.VirtualCode);
                string text = HotkeyManager.Instance.GetFor(key, Keyboard.Modifiers);
                if (text != null)
                {
                    SpeechSynthManager.Instance.QueueTTS(text, applicationWindow.GetOutputDevice(), applicationWindow.GetCurrentVoice(), applicationWindow.GetSpeechRate(), applicationWindow.GetSpeechVolume());
                    lastHotkeyTime = Environment.TickCount;
                }
            }
        }
        
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Dispose of all resources used by audio/speech synth.
            audioManager.Dispose();
            speechSynthManager.Dispose();
        }

        /// <summary>Returns true if the current application has focus, false otherwise</summary>
        public static bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);
    }
}
