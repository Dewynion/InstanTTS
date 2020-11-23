using System.Windows.Input;

namespace InstanTTS.Interface
{
    internal struct HotkeyData
    {
        public string Text { get; private set; }
        public Key Key { get; private set; }
        public HotkeyData(string text, Key key)
        {
            Text = text;
            Key = key;
        }
    }
}
