using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastResampler.Param;

namespace FastResampler.Note
{
    public class NoteImpl
    {
        public string flag = "impl";

        public int position;
        public int length;
        public double tempo;
        public double plustime;
        public int endfix = 0;

        public GlobalParam global;

        public NoteImpl prevNote;
        public NoteImpl nextNote;

        public string lyric;
        public NoteImpl()
        {

        }

        public NoteImpl(int position, int tempo, int length, string lyric)
        {
            this.position = position;
            this.length = length;
            this.lyric = lyric;
        }

        public string getWavtoolLength()
        {
            StringBuilder ret = new StringBuilder();
            ret.Append(this.length.ToString());
            ret.Append('@');
            ret.Append(this.tempo.ToString());
            if(this.plustime > 0)
            {
                ret.Append('+');
            }
            ret.Append(this.plustime.ToString());
            return ret.ToString();
        }

        /// <summary>
        /// 获取音符的长度
        /// </summary>
        /// <returns>长度（ms）</returns>
        public int getLength()
        {
            double ret;
            if (tempo != 0.0)
            {
                ret = (1000.0 * (60.0 / this.tempo)) * this.length / 480.0;
            }
            else
            {
                ret = plustime;
            }
            if (ret < 0.0)
            {
                ret = 0.0;
            }
            return (int)Math.Ceiling(ret);
        }

        /// <summary>
        /// 获取音符的位置
        /// </summary>
        /// <returns>位置（ms）</returns>
        public int getPosition()
        {
            double ret;
            if (tempo != 0.0)
            {
                ret = (1000.0 * (60.0 / this.tempo)) * this.position / 480.0;
            }
            else
            {
                ret = plustime;
            }
            if (ret < 0.0)
            {
                ret = 0.0;
            }
            return (int)Math.Ceiling(ret);
        }



        /// <summary>
        /// 获得真实（计算上下文）的合成时间
        /// </summary>
        /// <returns>时间（ms）</returns>
        public int getRealLength()
        {
            double ret;
            if (tempo != 0.0)
            {
                ret = (1000.0 * (60.0 / this.tempo)) * this.length / 480.0 + this.plustime;
            }
            else
            {
                ret = plustime;
            }
            if (ret < 0.0)
            {
                ret = 0.0;
            }
            return (int)Math.Ceiling(ret);
        }

        /// <summary>
        /// 更新当前音符的信息
        /// </summary>
        public virtual void update(){}

        public virtual void updatePlustime()
        {
            this.plustime = - this.endfix;
        }

        public void setLength(string lengthStr)
        {
            double duration, tempo, plustime;
            string curstr = lengthStr;
            double ret = 0.0;

            string[] AtSpt = lengthStr.Split('@');
            if (AtSpt.Length > 0)
            {
                if (AtSpt[0].IndexOfAny(new char[] { '+', '-' }) > 0)
                {
                    return;
                }
                else
                {
                    duration = Convert.ToDouble(AtSpt[0]);
                    curstr = AtSpt[1];
                }
            }
            else
            {
                return;
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
            this.length = Convert.ToInt32(duration);
            this.tempo = tempo;
            this.plustime = plustime;
        }

        public virtual string[] getWavtoolParam() { return null; }
    }
}
