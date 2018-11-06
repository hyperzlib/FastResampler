using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastResampler.Param;

namespace FastResampler.Note
{
    class LyricNote : NoteImpl
    {
        public new string flag = "lyric";
        
        public int overlap = 0;
        public int preutterance = 0;
        public int startcut = 0;
        public int consonant = 0;
        public int endcut = 0;
        public int velocity = 0;
        public int modulation = 0;
        public int offset = 0;
        public int volume = 0;
        public string pitchPercent = "";
        public string flags = "";
        public string sourceFile = "";
        public string tempFile = "";
        public string pitchStr = "";
        public Envelope envelope = new Envelope();
        public Pitch pitch; 
        //extend parameters
        public bool extend = false;
        public int endlap = 0;
        public int vend = 0;
        public int waveLength = 0;
        
        public void addEnvelopePoint(int pos, int vol, bool isTail)
        {
            this.envelope.Add(pos, vol, isTail);
        }

        /// <summary>
        /// 获得基准音阶对应的音高
        /// </summary>
        /// <returns>音高（Hz）</returns>
        public double getBasePitch()
        {
            int bias = 0;
            int scale = 0;
            int octave;
            double targetPitch;
            if (this.pitchPercent[1] == '#') bias = 1;
            switch (this.pitchPercent[0])
            {
                case 'C':
                    scale = -9 + bias;
                    break;
                case 'D':
                    scale = -7 + bias;
                    break;
                case 'E':
                    scale = -5;
                    break;
                case 'F':
                    scale = -4 + bias;
                    break;
                case 'G':
                    scale = -2 + bias;
                    break;
                case 'A':
                    scale = bias;
                    break;
                case 'B':
                    scale = 2;
                    break;
            }
            octave = Convert.ToInt32(this.pitchPercent[1 + bias]) - 4;
            targetPitch = 440 * Math.Pow(2.0, (double)octave) * Math.Pow(2.0, (double)scale / 12.0);
            return targetPitch;
        }

        /// <summary>
        /// 获得真实（计算上下文）的位置
        /// </summary>
        /// <returns>位置（ms）</returns>
        public int getRealPosition()
        {
            double ret;
            if (tempo != 0.0)
            {
                ret = (1000.0 * (60.0 / this.tempo)) * this.position / 480.0 - this.preutterance;
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
        /// 更新歌词音符的信息
        /// </summary>
        public override void update()
        {
            if(this.prevNote != null)
            {
                this.prevNote.endfix = this.preutterance - this.overlap;
                this.prevNote.updatePlustime();
            }
        }

        /// <summary>
        /// 更新修正时间
        /// </summary>
        public override void updatePlustime()
        {
            this.plustime = this.preutterance - this.endfix;
        }

        public string getPitchString()
        {
            return this.pitch.GetParam(this.getRealPosition(), this.getRealLength(), this.tempo, this.getBasePitch());
        }

        public string[] getResamplerParam()
        {
            string[] args = new string[16];
            args[0] = "resampler";
            args[1] = this.sourceFile;
            args[2] = this.tempFile;
            args[3] = this.pitchPercent;
            args[4] = this.velocity.ToString();
            args[5] = this.flags;
            args[6] = this.startcut.ToString();
            args[7] = this.getRealLength().ToString();
            args[8] = this.consonant.ToString();
            args[9] = this.endcut.ToString();
            args[10] = this.volume.ToString();
            args[11] = this.modulation.ToString();
            args[12] = "!" + this.tempo.ToString();
            args[13] = this.getPitchString();
            args[14] = this.vend.ToString();
            args[15] = this.preutterance.ToString();
            return args;
        }

        public override string[] getWavtoolParam()
        {
            int i;
            List<string> param = new List<string>();
            param.Add("wavtool");
            param.Add(this.global.outputFile);
            param.Add(this.tempFile);
            param.Add(this.offset.ToString());
            param.Add(this.getWavtoolLength());
            EnvelopePoint[] points = this.envelope.getWavtoolPoints();
            if (points.Length >= 4)
            {
                param.Add(points[0].pos.ToString());
                param.Add(points[1].pos.ToString());
                param.Add(points[2].pos.ToString());
                param.Add(points[0].vol.ToString());
                param.Add(points[1].vol.ToString());
                param.Add(points[2].vol.ToString());
                param.Add(points[3].vol.ToString());
                param.Add(this.overlap.ToString());
                param.Add(points[3].pos.ToString());
                if (points.Length > 4)
                {
                    //如果支持多点的wavtool会很有用
                    for (i = 4; i < points.Length; i++)
                    {
                        param.Add(points[i].pos.ToString());
                        param.Add(points[i].vol.ToString());
                    }
                }
            }
            return param.ToArray();
        }
    }
}
