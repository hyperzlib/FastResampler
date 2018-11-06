using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastResampler.Param
{
    public class Pitch
    {
        //密度：1ms/point
        /// <summary>
        /// 音高列表（单位：Hz）
        /// </summary>
        public List<double> pitchList = new List<double>();

        public Pitch()
        {

        }

        public void Append(string pitchData, int length, double basePitch = 0, int offset = 0)
        {
            List<int> points = PitchParamUtils.Decode(pitchData);
            this.Append(points, length, basePitch, offset);
            points = null;
        }

        public void Append(List<int> pitchData, int length, double basePitch = 0, int offset = 0)
        {
            int start;
            int i;
            List<double> points = new List<double>();
            if (offset < 0)
            {
                start = pitchList.Count;
                int fillSize = -offset;
                for(i = 0; i < fillSize; i++)
                {
                    points.Add(basePitch);
                }
            }
            else
            {
                start = pitchList.Count - offset;
            }
            int len = pitchData.Count;
            double multiple = (double)pitchData.Count / length;
            for (i = 0; i < length; i++)
            {
                if (i < offset)
                {
                    points[start + i] = ((double)pitchData[Math.Min(Convert.ToInt32(Math.Floor(multiple * i)), len)] / 10) + basePitch;
                }
                else
                {
                    points.Add(((double)pitchData[Math.Min(Convert.ToInt32(Math.Floor(multiple * i)), len)] / 10) + basePitch);
                }
            }
            this.pitchList.AddRange(points);
        }

        public string GetParam(int start, int length, double tempo, double basePitch = 0)
        {
            int end = Math.Min(start + length, this.pitchList.Count);
            int pLen = this.getPointLength(length, tempo);
            int i;
            double multiple = pLen / length;
            List<int> points = new List<int>();
            if(start >= 0 && end <= this.pitchList.Count)
            {
                for(i = 0; i < length; i++)
                {
                    if (this.pitchList[start + (int)(i * multiple)] == 0)
                    {
                        points.Add(Convert.ToInt32(basePitch * 10));
                    }
                    else
                    {
                        points.Add(Convert.ToInt32((this.pitchList[start + (int)(i * multiple)] - basePitch) * 10));
                    }
                }
                return PitchParamUtils.Encode(points);
            }
            return null;
        }

        public int getPointLength(int length, double tempo)
        {
            return (int)Math.Ceiling(length / 44100 * (60.0 / 96.0 / tempo * 44100 + 0.5) + 1);
        }
    }
}
