using System;
using System.Windows;
using System.Windows.Controls;
using System.Speech.Synthesis;
using System.Windows.Input;

using InstanTTS.Speech;
using InstanTTS.Audio;

namespace InstanTTS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            // set up voices and output devices according to those registered with the system.
            // TODO: custom voices?
            speechVoices.ItemsSource = SpeechSynthManager.Voices;
            if (SpeechSynthManager.Voices.Count > 0)
                speechVoices.SelectedItem = speechVoices.Items[0];
            
            settingsOutputDevice1.ItemsSource = AudioManager.OutDevices;
            settingsOutputDevice2.ItemsSource = AudioManager.OutDevices;
            if (AudioManager.OutDevices.Count > 0)
            {
                settingsOutputDevice1.SelectedItem = settingsOutputDevice1.Items[0];
                settingsOutputDevice2.SelectedItem = settingsOutputDevice2.Items[0];
            }

            speechHistory.ItemsSource = SpeechSynthManager.Instance.TTSHistory;
            speechQueue.ItemsSource = SpeechSynthManager.Instance.TTSQueue;

            // set up sliders with their appropriate range of values
            // it's not always good practice to do this in code, but I made an exception here because
            // I don't like tying program constants to interface values.
            SetupSlider(speechRateSlider, SpeechSynthManager.MIN_RATE, SpeechSynthManager.MAX_RATE);
            SetupSlider(speechVolumeSlider, SpeechSynthManager.MIN_VOLUME, SpeechSynthManager.MAX_VOLUME);
        }

        /// <summary>
        /// Retrieves the user's desired voice from the interface.
        /// </summary>
        /// <returns>The <see cref="InstalledVoice"/> selected by the user through <see cref="speechVoices"/>.</returns>
        public InstalledVoice GetCurrentVoice()
        {
            var voice = speechVoices.SelectedItem;
            if (voice.GetType() != typeof(InstalledVoice))
            {
                Console.WriteLine("Selected voice does not exist.");
                return null;
            }
            return (InstalledVoice)voice;
        }

        public int GetSpeechRate()
        {
            return Convert.ToInt32(speechRateSlider.Value);
        }

        public int GetSpeechVolume()
        {
            return Convert.ToInt32(speechVolumeSlider.Value);
        }

        /// <summary>
        /// Retrieves the selected output device from the interface.
        /// </summary>
        /// <returns>The <see cref="SoundDevice"/> selected by the user through <see cref="settingsOutputDevice1"/>.</returns>
        public SoundDevice GetOutputDevice1()
        {
            var device = settingsOutputDevice1.SelectedItem;
            if (device.GetType() != typeof(SoundDevice))
            {
                Console.WriteLine("Selected sound device does not exist.");
                return default;
            }
            return (SoundDevice)device;
        }

        public int GetOutputDevice1Number()
        {
            return GetOutputDevice1().DeviceNumber;
        }

        public SoundDevice GetOutputDevice2()
        {
            var device = settingsOutputDevice2.SelectedItem;
            if (device.GetType() != typeof(SoundDevice))
            {
                Console.WriteLine("Selected sound device does not exist.");
                return default;
            }
            return (SoundDevice)device;
        }

        public int GetOutputDevice2Number()
        {
            return GetOutputDevice2().DeviceNumber;
        }

        /// <summary>
        /// Helper method to set the minimum and maximum values of a slider.
        /// Also sets the slider's current value to halfway between the two
        /// ((min + max) / 2).
        /// </summary>
        /// <param name="slider">The slider to act on.</param>
        /// <param name="min">The slider's new minimum value.</param>
        /// <param name="max">The slider's new maximum value.</param>
        private void SetupSlider(Slider slider, int min, int max)
        {
            slider.Minimum = min;
            slider.Maximum = max;
            slider.Value = (min + max) / 2;
        }

        /// <summary>
        /// Speaks the content in the text entry field, assuming there is anything to say.
        /// </summary>
        private void Speak()
        {
            if (speechContent.Text.Trim() != "" && !SpeechSynthManager.Instance.IsQueueFull())
            {
                SpeechSynthManager.Instance.QueueTTS(speechContent.Text, GetOutputDevice1(), GetOutputDevice2(), GetCurrentVoice(), GetSpeechRate(), GetSpeechVolume());
                speechContent.Text = "";
            }
        }

        private void speechButton_Click(object sender, RoutedEventArgs e)
        {
            Speak();
        }

        private void speechRate_LostFocus(object sender, RoutedEventArgs e)
        {
            syncValues(speechRateSlider, speechRate);
        }

        private void speechVolume_LostFocus(object sender, RoutedEventArgs e)
        {
            syncValues(speechVolumeSlider, speechVolume);
        }

        private void syncValues(Slider slider, TextBox textBox)
        {
            int sliderVal = Convert.ToInt32(Math.Round(slider.Value));
            try
            {
                int value = int.Parse(textBox.Text);
                value = Convert.ToInt32(Math.Max(slider.Minimum, Math.Min(value, slider.Maximum)));
                slider.Value = value;
            }
            catch (FormatException)
            {
                textBox.Text = sliderVal.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void speechContent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Speak();
        }

        private void speechSkip_Click(object sender, RoutedEventArgs e)
        {
            AudioManager.Instance.Stop();
        }

        private void speechPause_Click(object sender, RoutedEventArgs e)
        {
            AudioManager.Instance.TogglePaused();
        }
    }
}
