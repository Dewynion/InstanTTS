using System.Windows;
using System.Windows.Input;

using InstanTTS.Audio;
using InstanTTS.Speech;

namespace InstanTTS
{
    /// <summary>
    /// Base class for the application. Handles startup and exit tasks.
    /// </summary>
    public partial class App : Application
    {
        public static App Instance { get; private set; }

        private SpeechSynthManager speechSynthManager;
        private AudioManager audioManager;
        private GlobalKeyboardHook globalKeyboardHook;
        private MainWindow applicationWindow;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            audioManager = new AudioManager();
            speechSynthManager = new SpeechSynthManager();
            globalKeyboardHook = new GlobalKeyboardHook();
            globalKeyboardHook.KeyboardPressed += OnKeyPressed;

            Instance = this;

            applicationWindow = new MainWindow();
            applicationWindow.Show();
        }

        private void OnKeyPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyUp
                && e.KeyboardData.Key == Key.H)
                SpeechSynthManager.Instance.QueueTTS("Give us a sign.", applicationWindow.GetOutputDevice(), applicationWindow.GetCurrentVoice());
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Dispose of all resources used by audio/speech synth.
            audioManager.Dispose();
            speechSynthManager.Dispose();
        }
    }
}
