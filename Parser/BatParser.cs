using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using FastResampler.Note;
using FastResampler.Param;

namespace FastResampler.Parser
{
    class BatParser
    {
        public string batData;
        public NoteList noteList;
        public GlobalParam globalParam;
        public Dictionary<string, string> settings = new Dictionary<string, string>();
        private LangPack lang;

        public BatParser(string batFile)
        {
            this.lang = Global.lang;
            this.batData = Utils.file_get_contents(batFile);
            this.parse();
        }

        private void parse()
        {
            string[] lines = this.batData.Replace("\r\n", "\n").Split('\n');
            foreach (string line in lines)
            {
                string[] args = Utils.parseArgs(line);
                if (args.Length > 0)
                {
                    string[] str;
                    LyricNote lyricNote;
                    RestNote restNote;
                    switch (args[0])
                    {
                        case "@set":
                            //将cmd变量存入临时变量
                            string[] t = line.Substring(5).Split('=');
                            if (t.Length == 2)
                            {
                                this.settings[t[0]] = t[1].Trim('"');
                            }
                            break;
                        case "@call":
                            str = Utils.parseArgs(this.cmdFormat(string.Format("\"{0}\" \"{1}\" {2} {3} {4} {5} {6} {7} {8} {9}", args[2], settings["temp"], args[3], settings["vel"], settings["flag"], args[6], args[7], args[8], args[9], settings["params"])));
                            lyricNote = new LyricNote();
                            lyricNote.lyric = Utils.getGen(str[0]);
                            lyricNote.sourceFile = str[0];
                            lyricNote.tempFile = str[1];
                            lyricNote.pitchPercent = str[2];
                            lyricNote.velocity = Convert.ToInt32(str[3]);
                            lyricNote.flags = str[4];
                            lyricNote.startcut = Convert.ToInt32(str[5]);
                            lyricNote.consonant = Convert.ToInt32(str[7]);
                            lyricNote.endcut = Convert.ToInt32(str[8]);
                            lyricNote.volume = Convert.ToInt32(str[9]);
                            lyricNote.modulation = Convert.ToInt32(str[10]);
                            //lyricNote.tempo = str[11];
                            if (str.Length > 12)
                                lyricNote.pitchStr = str[12];
                            else
                                lyricNote.pitchStr = "";
                            str = Utils.parseArgs(this.cmdFormat(string.Format("\"{0}\" \"{1}\" {2} {3} {4}", settings["output"], settings["temp"], settings["stp"], args[4], settings["env"])));
                            globalParam.outputFile = str[0];
                            //lyricNote.tempFile = str[1];
                            lyricNote.offset = Convert.ToInt32(str[2]);
                            lyricNote.setLength(str[3]);
                            lyricNote.envelope.Add(Convert.ToInt32(str[4]), Convert.ToInt32(str[7])); //p1
                            lyricNote.envelope.Add(Convert.ToInt32(str[5]), Convert.ToInt32(str[8])); //p2
                            lyricNote.envelope.Add(Convert.ToInt32(str[6]), Convert.ToInt32(str[9])); //p3
                            if(str.Length > 14)
                            {
                                lyricNote.envelope.Add(Convert.ToInt32(str[13]), Convert.ToInt32(str[14])); //p5
                            }
                            lyricNote.envelope.Add(str.Length > 12 ? Convert.ToInt32(str[12]) : 0, Convert.ToInt32(str[10]), true); //p4
                            if (str.Length > 11)
                                lyricNote.overlap = Convert.ToInt32(str[11]);
                            else
                                lyricNote.overlap = 0;
                            lyricNote.global = this.globalParam;
                            this.noteList.Add(lyricNote);
                            break;
                        case "@%tool%":
                            str = Utils.parseArgs(this.cmdFormat(string.Format("\"{0}\" \"{1}\" {2} {3} {4} {5}", args[1], this.settings["oto"] + "\\R.wav", args[3], args[4], args[5], args[6])));
                            restNote = new RestNote(str[3]);
                            this.noteList.Add(restNote);
                            break;
                        default:
                            break;
                    }
                }
            }
            this.noteList.update();
        }

        public string cmdFormat(string cmd)
        {
            char[] charList = cmd.Replace("\\\\", "\\").ToCharArray();
            string tempName = "";
            bool inArg = false;
            string output = "";
            foreach (char one in charList)
            {
                if (one == '%')
                {
                    if (inArg)
                    {
                        if (this.settings.Keys.Contains(tempName))
                        {
                            output += this.settings[tempName];
                        }
                        tempName = "";
                        inArg = false;
                    }
                    else
                    {
                        inArg = true;
                    }
                }
                else
                {
                    if (inArg)
                    {
                        tempName += one;
                    }
                    else
                    {
                        output += one;
                    }
                }
            }
            return output;
        }
    }
}
