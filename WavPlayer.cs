using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;
using System.IO;
using System.ComponentModel;
using System.Threading;

namespace FastResampler
{
    class WavPlayer
    {
        public SoundPlayer player = new SoundPlayer();
        public MemoryStream stream = new MemoryStream();
        public bool isLoaded = false;
        public int seek = 0;
        public NotePainter painter;
        public event AsyncCompletedEventHandler LoadCompleted;

        public WavPlayer(NotePainter painter)
        {
            this.painter = painter;
            player.Stream = stream;
            player.LoadCompleted += loaded;
        }

        public void loaded(Object sender, AsyncCompletedEventArgs e)
        {
            Thread t = new Thread(new ThreadStart(inProgress));
            t.Start();
        }

        public void inProgress()
        {
            double nowTime = 0.0;
            while (true)
            {
                Thread.Sleep(10);
                painter.drawNote(Convert.ToInt32(nowTime * 1000));
                nowTime += 0.015;
            }
        }

        public void push(byte[] wavData)
        {
            stream.Write(wavData, seek, wavData.Length);
            seek += wavData.Length;
            if (!isLoaded)
            {
                player.Play();
            }
        }

        public void pushFile(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            player.Stream = fs;
            if (!isLoaded)
            {
                player.Play();
            }
        }
    }
}
