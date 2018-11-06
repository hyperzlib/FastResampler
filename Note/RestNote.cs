using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastResampler.Note
{
    class RestNote : NoteImpl
    {
        public new string flag = "rest";

        public RestNote(int position, int length)
        {
            this.position = position;
            this.length = length;
        }

        public RestNote(string lengthStr)
        {
            this.setLength(lengthStr);
        }

        public RestNote() { }

        public override void updatePlustime()
        {
            this.plustime = -this.endfix;
        }

        public override void update()
        {
            if (this.prevNote != null)
            {
                this.prevNote.endfix = 0;
                this.prevNote.updatePlustime();
            }
        }

        public override string[] getWavtoolParam()
        {
            string[] param = new string[7];
            param[0] = "wavtool";
            param[1] = this.global.outputFile;
            param[2] = this.global.oto + "/R.wav";
            param[3] = "0";
            param[4] = this.getWavtoolLength();
            param[5] = "0";
            param[6] = "0";
            return param;
        }
    }
}
