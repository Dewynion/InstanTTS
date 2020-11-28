using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using NAudio.Wave;

namespace InstanTTS.Audio
{
    internal enum PlaybackState
    {
        Playing,
        Paused,
        Stopped
    }

    internal class AudioManager : IDisposable
    {
        // Instance of this singleton class.
        public static AudioManager Instance { get; private set; }

        // should be self-explanatory
        public static IReadOnlyCollection<SoundDevice> OutDevices;
        public static IReadOnlyCollection<SoundDevice> InDevices;

        // class-level in case I end up needing them for something later
        private WaveOutEvent waveOut1;
        private WaveOutEvent waveOut2;
        private WaveFileReader waveReader;
        private PlaybackState playbackState = PlaybackState.Stopped;

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

        public async Task Play(MemoryStream stream, int primaryDeviceNumber = -1, int secondaryDeviceNumber = -1)
        {
            // Make sure the memory stream is read from the start.
            stream.Seek(0, SeekOrigin.Begin);

            // These are disposed of after the using block.
            // The reason they need to be instantiated anew every time Play is called is
            // because the reader must be recreated with the new memory stream, while the
            // player must have a device number assigned before Init() is called.
            using (waveReader = new WaveFileReader(stream))
            using (waveOut1 = new WaveOutEvent())
            using (waveOut2 = new WaveOutEvent())
            {
                // Set the device number to play through. Defaults to system default (-1).
                waveOut1.DeviceNumber = primaryDeviceNumber;
                // Initialize the player with the .wav data produced by the reader.
                waveOut1.Init(waveReader);
                waveOut1.Play();
                // don't play if they're on the same device
                if (primaryDeviceNumber != secondaryDeviceNumber)
                {
                    waveOut2.DeviceNumber = secondaryDeviceNumber;
                    waveOut2.Init(waveReader);
                    waveOut2.Play();
                }
                playbackState = PlaybackState.Playing;

                int endTime = Environment.TickCount + Convert.ToInt32(waveReader.TotalTime.TotalMilliseconds);
                while (Environment.TickCount < endTime)
                {
                    if (playbackState == PlaybackState.Stopped)
                        break;
                    else if (playbackState == PlaybackState.Paused)
                        endTime = Environment.TickCount + RemainingTimeMillis();
                }
                playbackState = PlaybackState.Stopped;
            }
        }

        public void Stop()
        {
            playbackState = PlaybackState.Stopped;
            waveOut1?.Stop();
            waveOut2?.Stop();
        }

        public void TogglePaused()
        {
            switch (playbackState)
            {
                case PlaybackState.Paused:
                    playbackState = PlaybackState.Playing;
                    waveOut1?.Play();
                    waveOut2?.Play();
                    break;
                case PlaybackState.Playing:
                    playbackState = PlaybackState.Paused;
                    waveOut1?.Pause();
                    waveOut2?.Pause();
                    break;
                default:
                    break;
            }
        }

        public PlaybackState GetPlaybackState()
        {
            return playbackState;
        }

        public int RemainingTimeMillis()
        {
            if (waveReader != null)
                return Convert.ToInt32(waveReader.TotalTime.TotalMilliseconds) - Convert.ToInt32(waveReader.CurrentTime.TotalMilliseconds);
            return 0;
        }

        public void Dispose()
        {
            // this actually does nothing right now, because no class variables require disposal.
        }
    }
}
