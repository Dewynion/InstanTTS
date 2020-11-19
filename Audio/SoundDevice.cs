namespace InstanTTS.Audio
{
    public struct SoundDevice
    {
        public string Name { get; }
        public int DeviceNumber { get; }

        public SoundDevice(string name, int number)
        {
            Name = name;
            DeviceNumber = number;
        }
    }
}
