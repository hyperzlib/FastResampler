using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using NAudio.Wave;
using NAudio.CoreAudioApi;

namespace FastResampler
{
    public class StreamPlayer
    {
        private MemoryStream ms = new MemoryStream();
        private BinaryWriter bw;
        private WaveStream wavStream;
        private long startTime; //单位：0.001s
        private Thread TaskThread;
        public WaveOut waveOut;
        public bool Loaded = false;
        public bool FullLoaded = false;
        public long Length = 0;
        public int FrameLength = 1024 * 100;
        private long nowSeek;
        private byte[] FillWaveData;
        private bool autoStart;

        public NAudio.Wave.PlaybackState PlaybackState
        {
            get
            {
                return waveOut.PlaybackState;
            }
        }

        public long streamPos
        {
            get
            {
                return ms.Position;
            }
        }

        public StreamPlayer()
        {
            FillWaveData = new byte[FrameLength];
            for (int i = 0; i < FrameLength; i++)
            {
                FillWaveData[i] = 0;
            }
            bw = new BinaryWriter(ms);
            waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback());
        }

        public long Push(byte[] wavData, int length = -1)
        {
            if (length == -1)
            {
                length = wavData.Length;
            }
            lock (ms)
            {
                nowSeek = ms.Position;
                ms.Seek(Length, SeekOrigin.Begin);
                ms.Write(wavData, 0, length);
                Length += wavData.Length;
                ms.Seek(nowSeek, SeekOrigin.Begin);
            }
            if (autoStart)
            {
                this.DoPlay();
            }
            return Length;
        }

        public long[] parseRiffHeader(byte[] wavData)
        {
            long[] ret = new long[2];
            string RIFF = System.Text.Encoding.Default.GetString(wavData, 0, 4);
            if (RIFF == "RIFF")
            {

            }
            else
            {
                ret[0] = 0;
                ret[1] = 0;
            }
            return ret;
        }

        private void Task()
        {
            if (!FullLoaded && Length - ms.Position < FrameLength)
            {
                lock (ms)
                {

                    nowSeek = ms.Position;
                    ms.Seek(Length, SeekOrigin.Begin);
                    ms.Write(FillWaveData, 0, FrameLength);
                    ms.Seek(nowSeek, SeekOrigin.Begin);
                }
            }
        }

        private void ThreadRun()
        {
            while (this.PlaybackState == PlaybackState.Playing)
            {
                this.Task();
                Thread.Sleep(100);
            }
        }

        private void DoPlay()
        {
            if (!Loaded)
            {
                this.InitWaveStream();
                Loaded = true;
            }
            waveOut.Play();
            TaskThread = new Thread(new ThreadStart(this.ThreadRun));
            TaskThread.Start();
            startTime = Convert.ToInt64(time());
        }

        public bool Play()
        {
            if (Length > 1024)
            {
                this.DoPlay();
                return true;
            }
            else
            {
                autoStart = true;
                return false;
            }
        }

        public void Pause()
        {
            waveOut.Pause();
        }

        public void Stop()
        {
            waveOut.Stop();
        }

        public void Resume()
        {
            waveOut.Resume();
        }

        private double time()
        {
            TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1);
            return ts.TotalMilliseconds;
        }

        private byte[] modifyRiffHeader(byte[] header, int length, int channels, int fs, int nbit)
        {
            int samples = length * channels * fs * nbit / 8;
            BitConverter.GetBytes(Convert.ToInt32(samples * 2 + 36)).CopyTo(header, 4);
            BitConverter.GetBytes(Convert.ToInt32(samples)).CopyTo(header, 40);
            return header;
        }

        private byte[] makeRiffHeader(int samples, int channels, int fs, int nbit)
        {
            byte[] header = new byte[44];
            System.Text.Encoding.Default.GetBytes("RIFF").CopyTo(header, 0);
            BitConverter.GetBytes(Convert.ToInt32(samples * 2 + 36)).CopyTo(header, 4);
            System.Text.Encoding.Default.GetBytes("WAVE").CopyTo(header, 8);
            System.Text.Encoding.Default.GetBytes("fmt ").CopyTo(header, 12);
            BitConverter.GetBytes(Convert.ToInt32(16)).CopyTo(header, 16);
            BitConverter.GetBytes(Convert.ToInt16(1)).CopyTo(header, 20);
            BitConverter.GetBytes(Convert.ToUInt16(channels)).CopyTo(header, 22);
            BitConverter.GetBytes(Convert.ToUInt32(fs)).CopyTo(header, 24);
            BitConverter.GetBytes(Convert.ToUInt32(fs * nbit / 8)).CopyTo(header, 28);
            BitConverter.GetBytes(Convert.ToUInt16(nbit / 8)).CopyTo(header, 32);
            BitConverter.GetBytes(Convert.ToUInt16(nbit)).CopyTo(header, 34);
            System.Text.Encoding.Default.GetBytes("data").CopyTo(header, 36);
            BitConverter.GetBytes(Convert.ToInt32(samples)).CopyTo(header, 40);
            return header;
        }

        private void InitWaveStream()
        {
            ms.Seek(0, SeekOrigin.Begin);
            BinaryReader binaryReader = new BinaryReader(ms);
            //WaveFormat format = new WaveFormat(binaryReader);
            byte[] tempHeader = new byte[44];
            ms.Seek(0, SeekOrigin.Begin);
            ms.Read(tempHeader, 0, Convert.ToInt32(44));
            //byte[] header = modifyRiffHeader(tempHeader, 3600, format.Channels, format.SampleRate, format.BitsPerSample);
            byte[] header = makeRiffHeader(360 * 1 * 44100 * 16 / 8, 1, 44100, 16);
            tempHeader = null;
            ms.Seek(0, SeekOrigin.Begin);
            ms.Write(header, 0, header.Length);
            header = null;
            ms.Seek(0, SeekOrigin.Begin);
            this.wavStream = new WaveFileReader(ms);
            waveOut.Init(wavStream);
        }
    }
}
