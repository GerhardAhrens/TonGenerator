//-----------------------------------------------------------------------
// <copyright file="Sound.cs" company="Lifeprojects.de">
//     Class: Sound
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>06.01.2022</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------


namespace TonGeneratorLibrary.Core
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Media;
    using System.Runtime.InteropServices;
    using System.Threading;

    using TonGeneratorLibrary.Core.DLLImport;

    public sealed class Sound : IDisposable
    {
        private const int MMSYSERR_NOERROR = 0;
        private const int CALLBACK_FUNCTION = 0x00030000;
        private const int MM_WOM_OPEN = 0x3BB;
        private const int MM_WOM_CLOSE = 0x3BC;
        private const int MM_WOM_DONE = 0x3BD;

        private static AutoResetEvent playEvent = new AutoResetEvent(false);
        private IntPtr mainWaveOut;
        private SoundBuffer[] buffers;
        private int currentBuffer;
        private bool soundPlaying;
        private int soundFrequency;
        private Thread thread;
        private WaveOutAPI.WaveDelegate bufferProc = new WaveOutAPI.WaveDelegate(SoundBuffer.WaveOutProc);

        public SoundPlayer soundPlayer;

        public Sound()
        {
            this.OpenDevice(44100);
            this.buffers = new SoundBuffer[3];
            for (int i = 0; i < this.buffers.Length; i++)
            {
                this.buffers[i] = new SoundBuffer(this.mainWaveOut, 16384, i);
            }

            this.currentBuffer = 0;
            this.soundFrequency = 440;
        }

        public int soundVolume { get; set; }

        public void Dispose()
        {
            this.soundPlaying = false;
        }

        public bool OpenDevice(int samplesPerSecond)
        {
            WaveFormat waveFormat = new WaveFormat();
            waveFormat.wFormatTag = (short)WaveFormats.Pcm;
            waveFormat.nChannels = 2;
            waveFormat.wBitsPerSample = 16;
            waveFormat.nSamplesPerSec = samplesPerSecond;
            waveFormat.nBlockAlign = (short)(waveFormat.nChannels * waveFormat.wBitsPerSample / 8);
            waveFormat.nAvgBytesPerSec = waveFormat.nSamplesPerSec * waveFormat.nBlockAlign;
            waveFormat.cbSize = 0;

            if (WaveOutAPI.WaveOutOpen(out this.mainWaveOut, 0, waveFormat, this.bufferProc, 0, CALLBACK_FUNCTION) != MMSYSERR_NOERROR)
            {
                Debug.WriteLine("Sound card cannot be opened.");
                return false;
            }

            return true;
        }

        public bool CloseDevice()
        {
            if (WaveOutAPI.WaveOutClose(this.mainWaveOut) != MMSYSERR_NOERROR)
            {
                Debug.WriteLine("Sound card cannot be closed!");
                return false;
            }

            return true;
        }

        public short[] GetBuffer0()
        {
            buffers[0].Wait();
            return buffers[0].Data;
        }

        public MemoryStream SinStream(UInt16 frequency, int msDuration, UInt16 volume = 16383)
        {
            var mStrm = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(mStrm);

            const double TAU = 2 * Math.PI;
            int formatChunkSize = 16;
            int headerSize = 8;
            short formatType = 1;
            short tracks = 1;
            int samplesPerSecond = 44100;
            short bitsPerSample = 16;
            short frameSize = (short)(tracks * ((bitsPerSample + 7) / 8));
            int bytesPerSecond = samplesPerSecond * frameSize;
            int waveSize = 4;
            int samples = (int)((decimal)samplesPerSecond * msDuration / 1000);
            int dataChunkSize = samples * frameSize;
            int fileSize = waveSize + headerSize + formatChunkSize + headerSize + dataChunkSize;
            writer.Write(0x46464952); // = encoding.GetBytes("RIFF")
            writer.Write(fileSize);
            writer.Write(0x45564157); // = encoding.GetBytes("WAVE")
            writer.Write(0x20746D66); // = encoding.GetBytes("fmt ")
            writer.Write(formatChunkSize);
            writer.Write(formatType);
            writer.Write(tracks);
            writer.Write(samplesPerSecond);
            writer.Write(bytesPerSecond);
            writer.Write(frameSize);
            writer.Write(bitsPerSample);
            writer.Write(0x61746164); // = encoding.GetBytes("data")
            writer.Write(dataChunkSize);

            double theta = frequency * TAU / (double)samplesPerSecond;
            // 'volume' is UInt16 with range 0 thru Uint16.MaxValue ( = 65 535)
            // we need 'amp' to have the range of 0 thru Int16.MaxValue ( = 32 767)
            double amp = volume >> 2; // so we simply set amp = volume / 2
            for (int step = 0; step < samples; step++)
            {
                short s = (short)(amp * Math.Sin(theta * (double)step));
                writer.Write(s);
            }

            mStrm.Seek(0, SeekOrigin.Begin);
            return mStrm;
        }

        public void Play(int frequency, int volume)
        {
            this.soundPlaying = true;
            this.soundFrequency = frequency;
            this.soundVolume = volume;
            this.thread = new Thread(new ThreadStart(PlayThread));
            this.thread.Start();
        }

        public void Play(UInt16 frequency, int msDuration, UInt16 volume = 16383)
        {
            this.soundPlayer = new SoundPlayer(SinStream(frequency, msDuration, volume));
            this.soundPlayer.Load();
            this.soundPlayer.PlaySync();
        }

        public void Play(SystemSound systemSound)
        {
            systemSound.Play();
        }

        private void FillBufferWithWave(int bufferId)
        {
            this.buffers[bufferId].FillSine(this.soundFrequency, this.soundVolume);
        }

        private void PlayThread()
        {
            playEvent.Set();
            this.FillBufferWithWave(0);
            this.FillBufferWithWave(1);
            this.buffers[0].Play(soundFrequency);
            this.buffers[1].Play(soundFrequency);
            this.currentBuffer = 2;
            while (this.soundPlaying)
            {
                this.FillBufferWithWave(currentBuffer);
                this.buffers[this.currentBuffer].Wait();
                this.buffers[this.currentBuffer].Play(this.soundFrequency);
                this.currentBuffer = this.currentBuffer == this.buffers.Length - 1 ? 0 : this.currentBuffer + 1;
            }
        }


        internal sealed class SoundBuffer : IDisposable
        {
            private IntPtr hWaveOut;
            private WaveHeader waveHeader;
            private GCHandle headerDataHandle;
            private GCHandle waveHeaderHandle;
            private int Id;
            private Random randomSeed;

            public short[] Data { get; private set; }

            public bool isPlaying { get; private set; }

            public SoundBuffer(IntPtr waveOutHandle, int buffersize, int id)
            {
                this.hWaveOut = waveOutHandle;

                Id = id;
                randomSeed = new Random();

                this.waveHeaderHandle = GCHandle.Alloc(this.waveHeader, GCHandleType.Pinned);
                this.waveHeader.dwUser = (IntPtr)GCHandle.Alloc(this);
                this.Data = new short[buffersize];
                this.headerDataHandle = GCHandle.Alloc(Data, GCHandleType.Pinned);
                this.waveHeader.lpData = this.headerDataHandle.AddrOfPinnedObject();
                this.waveHeader.dwBufferLength = buffersize;
                this.waveHeader.dwFlags = 0;
                this.waveHeader.dwLoops = 0;

                try
                {
                    WaveOutAPI.WaveOutPrepareHeader(this.hWaveOut, ref waveHeader, Marshal.SizeOf(this.waveHeader));
                }
                catch
                {
                    Debug.WriteLine("Error preparing Header!");
                }
            }

            public void Dispose()
            {
                if (this.waveHeader.lpData != IntPtr.Zero)
                {
                    WaveOutAPI.WaveOutUnprepareHeader(hWaveOut, ref this.waveHeader, Marshal.SizeOf(this.waveHeader));
                    this.waveHeaderHandle.Free();
                    this.waveHeader.lpData = IntPtr.Zero;
                }

                playEvent.Close();
                if (this.headerDataHandle.IsAllocated)
                {
                    this.headerDataHandle.Free();
                }

                GC.SuppressFinalize(this);
            }

            public static void WaveOutProc(IntPtr hdrvr, int uMsg, int dwUser, ref WaveHeader wavhdr, int dwParam2)
            {
                if (uMsg == MM_WOM_DONE)
                {
                    GCHandle h = (GCHandle)wavhdr.dwUser;
                    SoundBuffer buf = (SoundBuffer)h.Target;
                    buf.OnCompleted();
                }
            }

            public double GetRandomNumber(double minimum, double maximum)
            {
                return this.randomSeed.NextDouble() * (maximum - minimum) + minimum;
            }

            /// <summary>
            /// Remplit le buffer avec une onde sinusoïdale
            /// </summary>
            /// <param name="frequency">Fréquence (La = 440)</param>
            /// <param name="volume">Puissance du son, compris entre 0 et 100</param>
            /// <returns></returns>
            public void FillSine(int frequency, int volume)
            {
                double x;
                int nChannels = 2;
                double nSamplesPerSec = 44100;
                for (int i = 0; i < this.waveHeader.dwBufferLength; i++)
                {
                    x = Math.Sin(i * nChannels * Math.PI * (frequency) / nSamplesPerSec);
                    this.Data[i] = (short)(((double)volume / 100) * short.MaxValue * x);
                }
                System.Runtime.InteropServices.Marshal.Copy(Data, 0, this.waveHeader.lpData, this.waveHeader.dwBufferLength);
            }

            public void FillSquare(int frequency, int volume)
            {
                short[] soundData = new short[this.waveHeader.dwBufferLength];
                double multiple = 2 * frequency / 44100.0f;
                short gain = (short)(((double)volume / 100) * short.MaxValue);
                for (int i = 0; i < this.waveHeader.dwBufferLength; i++)
                {
                    soundData[i] = (((i * multiple) % 2) - 1 > 0) ? gain : (short)-gain;
                }

                Marshal.Copy(soundData, 0, this.waveHeader.lpData, this.waveHeader.dwBufferLength);
            }

            public void Clean()
            {
                for (int i = 0; i < waveHeader.dwBufferLength - 1; i++)
                {
                    Data[i] = 0;
                }
            }

            public bool Play(int frequency)
            {
                playEvent.Reset();
                playEvent.Set();
                isPlaying = WaveOutAPI.WaveOutWrite(hWaveOut, ref waveHeader, Marshal.SizeOf(waveHeader)) == MMSYSERR_NOERROR;
                return isPlaying;
            }

            public void Stop()
            {
                playEvent.Set();
            }

            public void Wait()
            {
                if (this.isPlaying)
                {
                    this.isPlaying = playEvent.WaitOne();
                    playEvent.WaitOne();
                }
                else
                {
                    Thread.Sleep(0);
                }
            }

            public void OnCompleted()
            {
                isPlaying = false;
                playEvent.Set();
            }
        }
    }
}
