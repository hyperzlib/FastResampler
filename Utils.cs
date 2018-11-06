using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;


namespace FastResampler
{
    class Utils
    {
        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("kernel32.dll")]
        public static extern IntPtr _lopen(string lpPathName, int iReadWrite);
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);
        public const int OF_READWRITE = 2;
        public const int OF_SHARE_DENY_NONE = 0x40;
        public static readonly IntPtr HFILE_ERROR = new IntPtr(-1);
        public static string tempTitle = "fastresampler - " + GetRandomString(10, true, true, true, false, "");
        public static string tempTitle2 = "";
        public static void hideBat()
        {
            IntPtr intptr = FindWindow("ConsoleWindowClass", tempTitle);
            if (intptr != IntPtr.Zero)
            {
                ShowWindow(intptr, 0);//隐藏这个窗口
            }
            intptr = FindWindow("ConsoleWindowClass", tempTitle2);
            if (intptr != IntPtr.Zero)
            {
                ShowWindow(intptr, 0);//隐藏这个窗口
            }
        }

        public static void showBat()
        {
            Console.Title = tempTitle;
            IntPtr intptr;
            intptr = FindWindow("ConsoleWindowClass", tempTitle2);
            if (intptr != IntPtr.Zero)
            {
                ShowWindow(intptr, 1);
            }
        }

        public static void killBat()
        {
            bool finded = false;
            Process[] procList;
            procList = Process.GetProcessesByName("cmd");
            foreach (Process p in procList)
            {
                if (p.MainWindowTitle == tempTitle2)
                {
                    finded = true;
                    p.Kill();
                }
            }
            if (!finded)
            {
                procList = Process.GetProcesses();
                foreach (Process p in procList)
                {
                    if (p.MainWindowTitle == tempTitle2)
                    {
                        finded = true;
                        p.Kill();
                    }
                }
            }
        }

        public static string getGen(string filename)
        {
            return Path.GetFileNameWithoutExtension(filename);
        }

        public static void setProgress(int now, int max){
            Console.Write("\r");
            Console.Write("{0}/{1}", now, max);
        }

        public static string file_get_contents(string filename)
        {
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);//初始化文件流
                byte[] array = new byte[fs.Length];//初始化字节数组
                fs.Read(array, 0, array.Length);//读取流中数据到字节数组中
                fs.Close();//关闭流
                string str = Encoding.Default.GetString(array);//将字节数组转化为字符串
                return str;
            }
            catch (IOException e)
            {
                Utils.log("error: " + string.Format("{0}, {1}", e.Message, e.StackTrace));
                return "";
            }
        }

        public static byte[] file_get_bytes(string filename)
        {
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);//初始化文件流
                byte[] array = new byte[fs.Length];//初始化字节数组
                fs.Read(array, 0, array.Length);//读取流中数据到字节数组中
                fs.Close();//关闭流
                return array;
            }
            catch (IOException e)
            {
                byte[] array = new byte[0];
                Utils.log("error: " + string.Format("{0}, {1}", e.Message, e.StackTrace));
                return array;
            }
        }

        public static byte[] file_get_bytes(string filename, long offset, long length)
        {
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);//初始化文件流
                fs.Seek(offset, SeekOrigin.Begin);
                byte[] array = new byte[length];//初始化字节数组
                fs.Read(array, 0, array.Length);//读取流中数据到字节数组中
                fs.Close();//关闭流
                return array;
            }
            catch (IOException e)
            {
                byte[] array = new byte[0];
                Utils.log("error: " + string.Format("{0}, {1}", e.Message, e.StackTrace));
                return array;
            }
        }

        public static bool file_put_contents(string filename, string content)
        {
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);//初始化文件流
                byte[] array = System.Text.Encoding.UTF8.GetBytes(content);//初始化字节数组
                fs.Write(array, 0, array.Length);
                fs.Close();//关闭流
                return true;
            }
            catch (IOException e)
            {
                Utils.log("error: " + string.Format("{0}, {1}", e.Message, e.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// 解析cmd参数
        /// </summary>
        /// <param name="cmd">cmd命令</param>
        /// <returns>参数列表</returns>
        public static string[] parseArgs(string cmd)
        {
            List<string> args = new List<string>();
            char[] charList = cmd.ToCharArray();
            bool inStringFlag = false;
            string tempArg = "";
            foreach (char one in charList)
            {
                switch (one)
                {
                    case '"':
                        inStringFlag = !inStringFlag;
                        break;
                    case ' ':
                        if (inStringFlag)
                        {
                            tempArg += ' ';
                        }
                        else
                        {
                            args.Add(tempArg);
                            tempArg = "";
                        }
                        break;
                    default:
                        tempArg += one;
                        break;
                }
            }
            args.Add(tempArg);
            return args.ToArray();
        }

        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <param name="length">长度</param>
        /// <param name="useNum">使用数字</param>
        /// <param name="useLow">使用小写</param>
        /// <param name="useUpp">使用大写</param>
        /// <param name="useSpe">使用空格</param>
        /// <param name="custom"></param>
        /// <returns>生成的字符串</returns>
        public static string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        } 

        /// <summary>
        /// 执行cmd指令
        /// </summary>
        /// <param name="cmd">cmd指令</param>
        /// <returns>返回值</returns>
        public static string system(string cmd)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WorkingDirectory = System.Environment.CurrentDirectory;
            p.Start();
            p.StandardInput.WriteLine(cmd + "&exit");
            p.StandardInput.AutoFlush = true;
            string strOuput = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
            return strOuput;
        }

        /// <summary>
        /// 截取部分数组
        /// </summary>
        /// <param name="arr">数组</param>
        /// <param name="start">开始部分</param>
        /// <returns>截取后的数组</returns>
        public static string[] subarr(string[] arr, int start)
        {
            string[] output = new string[arr.Length - start];
            int index = 0;
            for (int i = start; i < arr.Length; i++)
            {
                output[index++] = arr[i];
            }
            return output;
        }

        /// <summary>
        /// UTAU 长度字符串转毫秒
        /// </summary>
        /// <param name="lenstr">长度字符串</param>
        /// <returns>毫秒时间</returns>
        public static double parseLength(string lenstr)
        {
            double duration, tempo, plustime;
            string curstr = lenstr;
            double ret = 0.0;

            string[] AtSpt = lenstr.Split('@');
            if (AtSpt.Length > 0)
            {
                if (AtSpt[0].IndexOfAny(new char[] { '+', '-' }) > 0)
                {
                    return -1.0;
                }
                else
                {
                    duration = Convert.ToDouble(AtSpt[0]);
                    curstr = AtSpt[1];
                }
            }
            else
            {
                ret = Convert.ToDouble(lenstr);
                return ret > 0 ? ret : 0.0;
            }

            int indexofPlus = curstr.IndexOfAny(new char[] { '+', '-' });
            if (indexofPlus == -1)
            {
                plustime = 0;
                tempo = Convert.ToDouble(curstr);
            }
            else
            {
                string s1 = curstr.Substring(0, indexofPlus);
                string s2 = curstr.Substring(indexofPlus);
                tempo = Convert.ToDouble(s1);
                plustime = Convert.ToDouble(s2);
            }

            if (tempo != 0.0)
            {
                ret = (1000.0 * (60.0 / tempo)) * duration / 480.0 + plustime;
            }
            else
            {
                ret = plustime;
            }
            if (ret < 0.0)
            {
                ret = 0.0;
            }
            return ret;
        }

        /// <summary>
        /// 创建wav头部
        /// </summary>
        /// <param name="samples">长度samples</param>
        /// <param name="channels">声道数量</param>
        /// <param name="fs">采样率</param>
        /// <param name="nbit">比特</param>
        /// <returns>wave头部</returns>
        public static byte[] makeRiffHeader(int samples, int channels, int fs, int nbit)
        {
            byte[] header = new byte[44];
            System.Text.Encoding.Default.GetBytes("RIFF").CopyTo(header, 0);
            BitConverter.GetBytes(Convert.ToInt32(samples * channels + 36)).CopyTo(header, 4);
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
            BitConverter.GetBytes(Convert.ToInt32(samples * channels)).CopyTo(header, 40);
            return header;
        }

        /// <summary>
        /// 生成空白wav音频
        /// </summary>
        /// <param name="length">时长（ms）</param>
        /// <param name="filename">保存文件名</param>
        /// <returns></returns>
        public static bool generateWhiteWave(double length, string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);
            int sampleRate = 44100;
            int nbit = 16;
            int channels = 1;
            int samples = Convert.ToInt32(sampleRate * channels * nbit * length / 8);
            byte[] header = makeRiffHeader(samples, channels, sampleRate, nbit);
            fs.Write(header, 0, header.Length);
            byte[] tempBuffer = new byte[sampleRate];
            int loopNum = Convert.ToInt32(Math.Floor(Convert.ToDouble(samples) / sampleRate));
            int fillEnd = samples % sampleRate;
            int i;
            for(i = 0; i < sampleRate; i++)
            {
                tempBuffer[i] = 0;
            }
            for(i = 0; i < loopNum; i++)
            {
                fs.Write(tempBuffer, 0, sampleRate);
            }
            tempBuffer = null;
            tempBuffer = new byte[fillEnd];
            for (i = 0; i < fillEnd; i++)
            {
                tempBuffer[i] = 0;
            }
            fs.Write(tempBuffer, 0, fillEnd);
            fs.Close();
            return true;
        }

        public static bool isOpened(string filename)
        {
            if (!File.Exists(filename))
            {
                return false;
            }
            IntPtr vHandle = _lopen(filename, OF_READWRITE | OF_SHARE_DENY_NONE);
            if (vHandle == HFILE_ERROR)
            {
                return true;
            }
            CloseHandle(vHandle);
            return false;
        }

        public static long filesize(string filename)
        {
            FileInfo fi = new FileInfo(filename);
            return fi.Length;
        }

        public static double time()
        {
            TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1);
            return ts.TotalMilliseconds;
        }

        public static void log(string msg)
        {
            Console.WriteLine(msg);
            file_put_contents("debug.log", msg + "\r\n");
        }
    }
}
