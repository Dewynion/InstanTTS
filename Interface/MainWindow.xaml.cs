using System;
using System.Windows;
using System.Windows.Controls;
using System.Speech.Synthesis;
using System.Windows.Input;

using InstanTTS.Speech;
using InstanTTS.Audio;
using InstanTTS.Interface;
using System.Text;

namespace InstanTTS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;

        private const string NewHotkeyDefault = "Click to set";
        private const string NewHotkeyCapture = "Press a key...";

        private bool captureInput = false;
        private bool assignedNewHotkey = false;
        private HotkeyData newHotkeyData;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            // set up voices and output devices according to those registered with the system.
            // TODO: custom voices?
            speechVoices.ItemsSource = SpeechSynthManager.Voices;
            if (speechVoices.Items.Count > 0)
                speechVoices.SelectedItem = speechVoices.Items[0];

            settingsOutputDevices.ItemsSource = AudioManager.OutDevices;
            if (settingsOutputDevices.Items.Count > 0)
                settingsOutputDevices.SelectedItem = settingsOutputDevices.Items[0];

            speechHistory.ItemsSource = SpeechSynthManager.Instance.TTSHistory;
            speechQueue.ItemsSource = SpeechSynthManager.Instance.TTSQueue;
            settingsHotkeys.ItemsSource = HotkeyManager.Instance.Hotkeys;

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
        /// <returns>The <see cref="SoundDevice"/> selected by the user through <see cref="settingsOutputDevices"/>.</returns>
        public SoundDevice GetOutputDevice()
        {
            var device = settingsOutputDevices.SelectedItem;
            if (device.GetType() != typeof(SoundDevice))
            {
                Console.WriteLine("Selected sound device does not exist.");
                return default;
            }
            return (SoundDevice)device;
        }

        public int GetOutputDeviceNumber()
        {
            return GetOutputDevice().DeviceNumber;
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
                SpeechSynthManager.Instance.QueueTTS(speechContent.Text, GetOutputDevice(), GetCurrentVoice(), GetSpeechRate(), GetSpeechVolume());
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

        private void settingsNewHotkeyKey_Click(object sender, RoutedEventArgs e)
        {
            captureInput = !captureInput;
            if (captureInput)
                settingsNewHotkeyKey.Content = NewHotkeyCapture;
            else
                settingsNewHotkeyKey.Content = NewHotkeyDefault;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {            
            if (captureInput)
            {
                e.Handled = true;
                Key key = e.Key == Key.System ? e.SystemKey : e.Key;
                if (key == Key.LeftShift || key == Key.RightShift
                    || key == Key.LeftCtrl || key == Key.RightCtrl
                    || key == Key.LeftAlt || key == Key.RightAlt
                    || key == Key.LWin || key == Key.RWin)
                    return;
                // ignore all system keys as base input
                StringBuilder hotkeyName = new StringBuilder();
                if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
                    hotkeyName.Append("Ctrl+");
                if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                    hotkeyName.Append("Shift+");
                if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0)
                    hotkeyName.Append("Alt+");
                hotkeyName.Append(key.ToString());
                settingsNewHotkeyKey.Content = hotkeyName;
                newHotkeyData = new HotkeyData(key, Keyboard.Modifiers);
                captureInput = false;
                assignedNewHotkey = true;
            }
        }

        private void settingsAddHotkey_Click(object sender, RoutedEventArgs e)
        {
            if (assignedNewHotkey && settingsNewHotkeyText.Text != null)
            {
                HotkeyManager.Instance.AddHotkey(newHotkeyData.Key, newHotkeyData.ModifierKeys, settingsNewHotkeyText.Text);
                settingsNewHotkeyText.Text = "";
                settingsNewHotkeyKey.Content = NewHotkeyDefault;
                assignedNewHotkey = false;
            }
        }
    }
}
