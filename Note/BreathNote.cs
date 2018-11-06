using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastResampler.Note
{
    class BreathNote : NoteImpl
    {
        public new string flag = "breath";

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
