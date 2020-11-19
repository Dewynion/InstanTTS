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
        public SoundDevice Device { get; }

        public SpeechString(string text, InstalledVoice voice, int rate, int volume, SoundDevice device)
        {
            Text = text;
            Voice = voice;
            Rate = rate;
            Volume = volume;
            Device = device;
        }
    }
}
