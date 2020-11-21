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
        private WaveOutEvent waveOut;
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
            if (waveOut != null)
                waveOut.Stop();
        }

        public void TogglePaused()
        {
            switch (playbackState)
            {
                case PlaybackState.Paused:
                    playbackState = PlaybackState.Playing;
                    if (waveOut != null)
                        waveOut.Play();
                    break;
                case PlaybackState.Playing:
                    playbackState = PlaybackState.Paused;
                    if (waveOut != null)
                        waveOut.Pause();
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
