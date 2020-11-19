using System.Windows;

using InstanTTS.Audio;
using InstanTTS.Speech;

namespace InstanTTS
{
    /// <summary>
    /// Base class for the application. Handles startup and exit tasks.
    /// </summary>
    public partial class App : Application
    {
        private SpeechSynthManager _speechSynthManager;
        private AudioManager _audioManager;
        private MainWindow _applicationWindow;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _audioManager = new AudioManager();
            _speechSynthManager = new SpeechSynthManager();
            _applicationWindow = new MainWindow();
            _applicationWindow.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Dispose of all resources used by audio/speech synth.
            _audioManager.Dispose();
            _speechSynthManager.Dispose();
        }
    }
}
