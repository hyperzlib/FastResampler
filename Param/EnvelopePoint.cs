using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastResampler.Param
{
    public class EnvelopePoint
    {
        public int pos;
        public int vol;
        public bool tail;

        public EnvelopePoint()
        {
            this.pos = 0;
            this.vol = 0;
            this.tail = false;
        }

        public EnvelopePoint(int pos, int vol, bool isTail = false)
        {
            this.pos = pos;
            this.vol = vol;
            this.tail = isTail;
        }
    }
}
