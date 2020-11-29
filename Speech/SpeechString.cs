using System.Speech.Synthesis;
using InstanTTS.Audio;

namespace InstanTTS.Speech
{
    internal struct SpeechString
    {
        public string Text { get; }
        public InstalledVoice Voice { get; }
        public int Rate { get; }
        public int Volume { get; }
        public SoundDevice PrimaryDevice { get; }
        public SoundDevice SecondaryDevice { get; }

        public SpeechString(string text, InstalledVoice voice, int rate, int volume, SoundDevice device1, SoundDevice device2)
        {
            Text = text;
            Voice = voice;
            Rate = rate;
            Volume = volume;
            PrimaryDevice = device1;
            SecondaryDevice = device2;
        }
    }
}
