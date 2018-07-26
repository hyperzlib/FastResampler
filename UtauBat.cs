using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace FastResampler
{
    public struct resamParam
    {
        public string gen;
        public string genfile;
        public int toolParamId;
        public string temp;
        public string pitchPercent;
        public double velocity;
        public string flags;
        public double offset;
        public double lengthReq;
        public double fix;
        public double blank;
        public double volume;
        public double modulation;
        public double pre;
        public double vend;
        public string tempo;
        public string pit;
        public bool isStart;
        public bool isEnd;
    }

    public struct breathParam
    {
        public int position;
        public int length;
        public int intensity; //力度
        public int resamParamId; //呼气时对应的参数
        public bool exhale; //是否为呼气
    }

    public struct toolParam
    {
        public int len;
        public int resamParamId;
        public string outfile;
        public string infile;
        public double offset;
        public string length;
        public double p1;
        public double p2;
        public double p3;
        public double v1;
        public double v2;
        public double v3;
        public double v4;
        public double ovr;
        public double p4;
        public double p5;
        public double v5;
    }

    public class UtauBat
    {
        public string batData;
        public List<resamParam> resamParams = new List<resamParam>();
        public List<toolParam> toolParams = new List<toolParam>();
        public List<breathParam> breathParams = new List<breathParam>();
        public Dictionary<string, string> settings = new Dictionary<string, string>();
        private LangPack lang;
        public double length;
        public Oto oto;
        
        public UtauBat(string batfile, LangPack lang)
        {
            this.lang = lang;
            this.batData = Utils.file_get_contents(batfile);
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
                    resamParam p1;
                    toolParam p2;
                    bool nextIsStart = true;
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
                            p1 = new resamParam();
                            p1.gen = Utils.getGen(str[0]);
                            p1.genfile = str[0];
                            p1.toolParamId = toolParams.Count;
                            p1.temp = str[1];
                            p1.pitchPercent = str[2];
                            p1.velocity = Convert.ToDouble(str[3]);
                            p1.flags = str[4];
                            p1.offset = Convert.ToDouble(str[5]);
                            p1.lengthReq = Convert.ToDouble(str[6]);
                            p1.fix = Convert.ToDouble(str[7]);
                            p1.blank = Convert.ToDouble(str[8]);
                            p1.volume = Convert.ToDouble(str[9]);
                            p1.modulation = Convert.ToDouble(str[10]);
                            p1.tempo = str[11];
                            if (str.Length > 12)
                                p1.pit = str[12];
                            else
                                p1.pit = "";
                            p1.pre = 0;
                            p1.vend = 0;
                            p1.isStart = nextIsStart;
                            nextIsStart = false;
                            p1.isEnd = false;
                            resamParams.Add(p1);
                            str = Utils.parseArgs(this.cmdFormat(string.Format("\"{0}\" \"{1}\" {2} {3} {4}", settings["output"], settings["temp"], settings["stp"], args[4], settings["env"])));
                            p2 = new toolParam();
                            p2.resamParamId = resamParams.Count - 1;
                            p2.len = str.Length;
                            p2.outfile = str[0];
                            p2.infile = str[1];
                            p2.offset = Convert.ToDouble(str[2]);
                            p2.length = str[3];
                            p2.p1 = Convert.ToDouble(str[4]);
                            p2.p2 = Convert.ToDouble(str[5]);
                            p2.p3 = Convert.ToDouble(str[6]);
                            p2.v1 = Convert.ToDouble(str[7]);
                            p2.v2 = Convert.ToDouble(str[8]);
                            p2.v3 = Convert.ToDouble(str[9]);
                            p2.v4 = Convert.ToDouble(str[10]);
                            if (str.Length > 11)
                                p2.ovr = Convert.ToDouble(str[11]);
                            else
                                p2.ovr = 0;
                            if (str.Length > 12)
                                p2.p4 = Convert.ToDouble(str[12]);
                            else
                                p2.p4 = 0;
                            if (str.Length > 13)
                                p2.p5 = Convert.ToDouble(str[13]);
                            else
                                p2.p5 = 0;
                            if (str.Length > 14)
                                p2.v5 = Convert.ToDouble(str[14]);
                            else
                                p2.v5 = 0;
                            toolParams.Add(p2);
                            break;
                        case "@%tool%":
                            str = Utils.parseArgs(this.cmdFormat(string.Format("\"{0}\" \"{1}\" {2} {3} {4} {5}", args[1], this.settings["oto"] + "\\R.wav", args[3], args[4], args[5], args[6])));
                            p2 = new toolParam();
                            p2.resamParamId = -1;
                            p2.len = str.Length;
                            p2.outfile = str[0];
                            p2.infile = str[1];
                            p2.offset = Convert.ToDouble(str[2]);
                            p2.length = str[3];
                            p2.p1 = Convert.ToDouble(str[4]);
                            p2.p2 = Convert.ToDouble(str[5]);
                            p2.p3 = 0;
                            p2.v1 = 0;
                            p2.v2 = 0;
                            p2.v3 = 0;
                            p2.v4 = 0;
                            p2.ovr = 0;
                            p2.p4 = 0;
                            p2.p5 = 0;
                            p2.v5 = 0;
                            toolParams.Add(p2);
                            if (resamParams.Count - 1 >= 0)
                            {
                                resamParam tempParam = resamParams[resamParams.Count - 1];
                                tempParam.isEnd = true;
                                resamParams[resamParams.Count - 1] = tempParam;
                            }
                            nextIsStart = true;
                            break;
                        default:
                            break;
                    }
                }
            }
            this.update();
        }

        private void update()
        {
            if(this.settings.ContainsKey("oto") && File.Exists(this.settings["oto"] + @"\oto.xml")){
                this.oto = new Oto(this.settings["oto"] + @"\oto.xml", this.lang);
                for (int i = 0; i < resamParams.Count; i ++)
                {
                    otodata tempData = this.oto.getToneData(resamParams[i].gen);
                    otodata nextData;
                    int lengthMsec, osLengthMsec, edLengthMsec, signLen, eLen;
                    int st, es, ed, ve, ws, cf, free;
                    int edBegin, edEnd, endlap;
                    double velocity, vRatio = 1.0;
                    resamParam tempResamData = resamParams[i];
                    toolParam tempToolData = toolParams[tempResamData.toolParamId];
                    if(i == resamParams.Count - 1){
                        tempResamData.isEnd = true;
                    }
                    if (!string.IsNullOrEmpty(tempData.file))
                    {
                        tempResamData.genfile = this.settings["oto"] + "\\" + tempData.file;
                        tempResamData.offset = tempData.offset;
                        tempResamData.fix = tempData.consonant;
                        tempResamData.pre = tempData.preutterance;
                        //计算endlap
                        velocity = tempResamData.velocity;
                        vRatio = Math.Pow(2.0, (1.0 - (velocity / 100.0)));
                        if (tempData.endlap != 0)
                        {
                            lengthMsec = Convert.ToInt32(tempResamData.lengthReq);
                            osLengthMsec = Convert.ToInt32(tempData.offset);
                            edLengthMsec = Convert.ToInt32(tempData.length) - Convert.ToInt32(tempData.cutoff);
                            signLen = edLengthMsec - osLengthMsec;
                            st = Convert.ToInt32(tempData.consonant); //辅音长度
                            es = Convert.ToInt32(tempData.vend); //元音尾长度
                            ws = signLen - st - es; //白区长度
                            ve = signLen - es; //元音尾的绝对长度
                            cf = Convert.ToInt32(tempData.cutoff);
                            free = Convert.ToInt32(lengthMsec - (st / vRatio) - ws);

                            if (tempResamData.isEnd) //结尾时，范围是endlap到结束
                            {
                                endlap = Convert.ToInt32(signLen - tempData.endlap - ve);
                                ed = signLen - ve;
                                endlap += (ed - endlap) / 2;
                            }
                            else
                            {
                                nextData = this.oto.getToneData(resamParams[i + 1].gen);
                                if (nextData.overlap <= 0) //爆破音，和结尾音符一样
                                {
                                    endlap = Convert.ToInt32(signLen - tempData.endlap - ve);
                                    ed = Convert.ToInt32((signLen - ve) / 2);
                                }
                                else //正常音：范围是endlap + 下一个overlap到endlap和结尾的一半
                                {
                                    endlap = Convert.ToInt32(signLen - tempData.endlap + nextData.overlap - ve);
                                    ed = Convert.ToInt32((signLen - endlap) / 2);
                                }
                            }
                            edBegin = Convert.ToInt32(endlap / vRatio);
                            edEnd = Convert.ToInt32(ed / vRatio);
                            //开始判断元音尾的长度
                            if (free <= edBegin)
                            {
                                tempResamData.blank = Math.Max(0, signLen - (ve + edBegin * vRatio)) + cf;
                                tempResamData.vend = edBegin * vRatio;
                            }
                            else if (free > edBegin && free < edEnd)
                            {
                                tempResamData.blank = Math.Max(0, signLen - (ve + free * vRatio)) + cf;
                                tempResamData.vend = free * vRatio;
                            }
                            else
                            {
                                tempResamData.blank = Math.Max(0, signLen - (ve + edEnd * vRatio)) + cf;
                                tempResamData.vend = edEnd * vRatio;
                            }
                        }
                        tempToolData.ovr = tempData.overlap * vRatio;
                    }
                    resamParams[i] = tempResamData;
                    toolParams[tempResamData.toolParamId] = tempToolData;
                }
            }
            double nowTime = 0;
            for (int i = 0; i < toolParams.Count; i++)
            {
                nowTime += Math.Ceiling(Utils.parseLength(toolParams[i].length));
                nowTime -= toolParams[i].ovr;
            }
            this.length = nowTime;
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
