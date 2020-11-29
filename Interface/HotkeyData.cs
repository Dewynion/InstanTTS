using System.Windows.Input;

namespace InstanTTS.Interface
{
    internal struct HotkeyData
    {
        public Key Key { get; private set; }
        public ModifierKeys ModifierKeys;
        public string Display
        {
            get
            {
                return (ModifierKeys != ModifierKeys.None ? this.ModifierKeys.ToString() + "+" : "") + Key.ToString();
            }
        }

        public HotkeyData(Key key, ModifierKeys modifiers)
        {
            Key = key;
            ModifierKeys = modifiers;
        }
    }
}
