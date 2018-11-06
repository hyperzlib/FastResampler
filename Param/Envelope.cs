using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastResampler.Param
{
    public class Envelope
    {
        public List<EnvelopePoint> pointList = new List<EnvelopePoint>();
        public Envelope()
        {

        }

        public void Add(int pos, int vol, bool isTail = false)
        {
            pointList.Add(new EnvelopePoint(pos, vol, isTail));
        }

        public void Sort()
        {
            pointList.Sort((left, right) =>
            {
                if (left.tail && !right.tail)
                {
                    return -1;
                }
                else if (!left.tail && right.tail)
                {
                    return 1;
                }
                else if (!left.tail && !right.tail)
                {
                    if (left.pos > right.pos)
                    {
                        return -1;
                    }
                    else if (left.pos == right.pos)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else if (left.tail && right.tail)
                {
                    if (left.pos > right.pos)
                    {
                        return 1;
                    }
                    else if (left.pos == right.pos)
                    {
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return 0;
                }
                
            });
        }

        public EnvelopePoint[] getWavtoolPoints()
        {
            List<EnvelopePoint> ret = new List<EnvelopePoint>();
            if (this.pointList.Count == 4)
            {
                ret.Add(this.pointList[0]); //p1
                ret.Add(this.pointList[1]); //p2
                ret.Add(this.pointList[2]); //p3
                ret.Add(this.pointList[3]); //p4
            }
            else if (this.pointList.Count > 5)
            {
                ret.Add(this.pointList[0]); //p1
                ret.Add(this.pointList[1]); //p2
                ret.Add(this.pointList[this.pointList.Count - 2]); //p3
                ret.Add(this.pointList[this.pointList.Count - 1]); //p4
                for (int i = 2; i < this.pointList.Count - 2; i++)
                    ret.Add(this.pointList[i]); //p5+
            }
            else
            {
                ret.Add(new EnvelopePoint(0, 0)); //p1
                ret.Add(new EnvelopePoint(5, 100)); //p2
                ret.Add(new EnvelopePoint(35, 100, true)); //p3
                ret.Add(new EnvelopePoint(0, 0, true)); //p4
            }
            return ret.ToArray();
        }
    }
}
