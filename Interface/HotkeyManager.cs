using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InstanTTS.Interface
{
    internal class HotkeyManager
    {
        public static HotkeyManager Instance { get; private set; }
        public Dictionary<HotkeyData, string> Hotkeys { get; private set; }

        public HotkeyManager()
        {
            Hotkeys = new Dictionary<HotkeyData, string>();
            Instance = this;
        }

        public bool HasHotkey(Key key, ModifierKeys modifierKeys)
        {
            // since HotkeyData is a struct, this is fine
            return Hotkeys.ContainsKey(new HotkeyData(key, modifierKeys));
        }

        public void AddHotkey(Key key, ModifierKeys modifierKeys, string text)
        {
            if (!HasHotkey(key, modifierKeys))
            {
                Hotkeys.Add(new HotkeyData(key, modifierKeys), text);
            }
        }

        public string GetFor(Key key, ModifierKeys modifierKeys)
        { 
            Hotkeys.TryGetValue(new HotkeyData(key, modifierKeys), out string text);
            return text;
        }
    }
}
