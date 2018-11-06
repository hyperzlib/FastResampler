using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using FastResampler.Note;
using FastResampler.Param;

namespace FastResampler
{
    public partial class Form1 : Form
    {
        /*public delegate void setProgressMax(int max);
        public delegate void setProgressNow(int now);
        public event setProgressMax setProgressMaxEvent;
        public event setProgressNow setProgressNowEvent;*/
        public int progress1MaxNum;
        public int progress1NowNum;
        public int progress2MaxNum;
        public int progress2NowNum;
        public string status1 = "", status2 = "";
        public NotePainter painter;
        public string rootDir = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        
        public Form1()
        {
            InitializeComponent();
            Global.config = new Config(this.rootDir + "FastResampler.xml");
            Global.lang = new LangPack(rootDir + "language.xml", Global.config);
        }

        public void SetProgress1MaxNum(int max)
        {
            this.progress1MaxNum = max;
        }

        public void SetProgress1NowNum(int now)
        {
            this.progress1NowNum = now;
            int progress = Convert.ToInt32(Math.Round(Convert.ToDouble(this.progress1NowNum) / Convert.ToDouble(this.progress1MaxNum) * 100));
            labelStatus.Text = string.Format("{0} ({1}/{2}) {3}%", this.status1, this.progress1NowNum, this.progress1MaxNum, progress);
            progressResampler.Value = progress;
        }

        public void SetProgress2MaxNum(int max)
        {
            this.progress2MaxNum = max;
        }

        public void SetProgress2NowNum(int now, bool multiThread = false)
        {
            this.progress2NowNum = now;
            int progress = Convert.ToInt32(Math.Round(Convert.ToDouble(this.progress2NowNum) / Convert.ToDouble(this.progress2MaxNum) * 100));
            string statusString = string.Format("{0} ({1}/{2}) {3}%", this.status2, this.progress2NowNum, this.progress2MaxNum, progress);
            if (multiThread == false)
                labelStatus.Text = statusString;
            else
                labelStatusWavtool.Text = statusString;
            progressWavtool.Value = progress;
        }

        public void SetStatus1(string status)
        {
            this.status1 = status;
        }

        public void SetStatus2(string status)
        {
            this.status2 = status;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = Global.lang.fetch("正在合成……");
            label1.Text = Global.lang.fetch("合成进度：");
            label2.Text = Global.lang.fetch("拼接进度：");
            labelStatus.Text = Global.lang.fetch("加载中……");
            Control.CheckForIllegalCrossThreadCalls = false;
            #if DEBUG
            //System.Environment.CurrentDirectory = @"C:\Users\Administrator\AppData\Local\Temp\utau1";
            #endif
            string batFile = System.Environment.CurrentDirectory + "\\temp.bat";
            if (!Global.config.showConsole)
            {
                Utils.hideBat();
            }
            if (File.Exists(batFile))
            {
                labelStatus.Text = Global.lang.fetch("正在解析数据……");
                try
                {
                    NoteList noteList = new NoteList();
                    UtauBat bat = new UtauBat(batFile);
                    //this.painter = new NotePainter(noteCanvas.CreateGraphics(), noteCanvas.Width, noteCanvas.Height, bat);
                    Resampler resampler = new Resampler(bat, this);
                    Thread thread = new Thread(new ThreadStart(resampler.synthetise));
                    thread.Start();
                }
                catch (FileNotFoundException fe)
                {
                    MessageBox.Show(string.Format("{0}{1}", lang.fetch("文件不存在："), fe.Message), lang.fetch("错误"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.SafeClose();
                }
                catch (Exception ex)
                {
                    Utils.log("error: " + string.Format("{0}, {1}", ex.Message, ex.StackTrace));
                    MessageBox.Show(string.Format("{0}, {1}", ex.Message, ex.StackTrace));
                }
            }
            else
            {
                MessageBox.Show(Global.lang.fetch("请在UTAU中调用！"), Global.lang.fetch("错误"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.SafeClose();
            }
        }

        public void VHide()
        {
            this.WindowState = FormWindowState.Minimized;
        }

        public void VShow()
        {
            this.WindowState = FormWindowState.Normal;
        }

        public void stop()
        {
            this.SafeClose();
        }

        public void SafeClose()
        {
            if (!Global.config.showConsole)
            {
                Utils.showBat();
            }
            Utils.killBat();
            this.Close();
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Global.config.showConsole)
            {
                Utils.showBat();
            }
            Utils.killBat();
        }

        private void progressBar2_Click(object sender, EventArgs e)
        {

        }
    }
}
