using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using NAudio.Wave;

namespace InstanTTS.Audio
{
    internal class AudioManager : IDisposable
    {
        // Instance of this singleton class.
        public static AudioManager Instance { get; private set; }

        // should be self-explanatory
        public static IReadOnlyCollection<SoundDevice> OutDevices;
        public static IReadOnlyCollection<SoundDevice> InDevices;

        // class-level in case I end up needing them for something later
        private WaveOutEvent waveOut;
        private WaveFileReader waveReader;

        public AudioManager()
        {
            // set up output devices
            List<SoundDevice> outDevices = new List<SoundDevice>();
            for (int i = -1; i < WaveOut.DeviceCount; i++)
            {
                WaveOutCapabilities device = WaveOut.GetCapabilities(i);
                Console.WriteLine("{0} | {1}", i, device.ProductName);
                outDevices.Add(new SoundDevice(device.ProductName, i));
            }
            OutDevices = outDevices.AsReadOnly();

            // set up input devices
            List<SoundDevice> inDevices = new List<SoundDevice>();
            for (int i = -1; i < WaveIn.DeviceCount; i++)
            {
                WaveInCapabilities device = WaveIn.GetCapabilities(i);
                Console.WriteLine("{0} | {1}", i, device.ProductName);
                inDevices.Add(new SoundDevice(device.ProductName, i));
            }
            InDevices = inDevices.AsReadOnly();

            Instance = this;
        }

        public async Task Play(MemoryStream stream, int deviceNumber = -1)
        {
            // Make sure the memory stream is read from the start.
            stream.Seek(0, SeekOrigin.Begin);
            
            // These are disposed of after the using block.
            // The reason they need to be instantiated anew every time Play is called is
            // because the reader must be recreated with the new memory stream, while the
            // player must have a device number assigned before Init() is called.
            using (waveReader = new WaveFileReader(stream))
            using (waveOut = new WaveOutEvent())
            {
                // Set the device number to play through. Defaults to system default (-1).
                waveOut.DeviceNumber = deviceNumber;
                // Initialize the player with the .wav data produced by the reader.
                waveOut.Init(waveReader);
                waveOut.Play();
                // Suspend this task for the duration of the audio clip.
                await Task.Delay(Convert.ToInt32(waveReader.TotalTime.TotalMilliseconds));
            }
        }

        public void Dispose()
        {
            // this actually does nothing right now, because no class variables require disposal.
        }
    }
}
