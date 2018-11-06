using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastResampler.Param
{
    //from MixResampler
    public class PitchParamUtils
    {
        class Encoder
        {
            const string CharArray = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
            public static string Enc(int e)
            {
                int i = e;
                int jz = CharArray.Length;
                string ret = "";
                ret = CharArray[i % jz] + ret;
                i = i / jz;
                ret = CharArray[i % jz] + ret;
                return ret;
            }
            public static int Dec(string d)
            {
                if (d.Length == 0) return 0;
                char[] arr = d.ToCharArray();
                Array.Reverse(arr);
                int ret = 0;
                if (d.Length > 0)
                {
                    ret = ret + CharArray.IndexOf(arr[0]);
                }
                if (d.Length > 1)
                {
                    ret = ret + CharArray.IndexOf(arr[1])*CharArray.Length;
                }
                return ret;
            }
            public static int Fox2Single(int Fox)
            {
                if (Fox >= 2048)
                {
                    return Fox - 4096;
                }
                else
                {
                    return Fox;
                }
            }
            public static int Single2Fox(int Single)
            {
                int Ret = Single;
                if (Ret > 2047) return 2047;
                if (Ret < 0) return Ret + 4096;
                else return Ret;
            }
        }
        public static string Encode(List<int> Points)
        {
            string Ret = "";
            int Cnt = 0;
            for (int i = 1; i < Points.Count; i++)
            {
                int Prev = Points[i-1];
                int Curr = Points[i];
                if (Prev != Curr)
                {
                    Ret += Encoder.Enc(Encoder.Single2Fox(Prev));
                    if (Cnt > 0)
                    {
                        Ret += "#"+(Cnt+1).ToString()+"#";
                    }
                    Cnt = 0;
                }
                else
                {
                    Cnt++;
                }
            }
            Ret += Encoder.Enc(Encoder.Single2Fox(Points[Points.Count-1]));
            if (Cnt > 0)
            {
                Ret += "#" + (Cnt + 1).ToString() + "#";
            }
            return Ret;
        }
        public static List<int> Decode(string ParamStr)
        {
            List<int> Ret = new List<int>();
            string[] Sr = ParamStr.Split('#');
            for (int i = 0; i < Sr.Length; i++)
            {
                if (i % 2 == 0)
                {
                    //决算器
                    string total = Sr[i];
                    while (total.Length > 0)
                    {
                        string num = total.Substring(0, 2);
                        int cur = Encoder.Dec(num);
                        Ret.Add(Encoder.Fox2Single(cur));
                        total = total.Substring(2);
                    }
                }
                else
                {
                    //计数器
                    int Cnt = 0;
                    if (int.TryParse(Sr[i], out Cnt))
                    {
                        if (Ret.Count > 0)
                        {
                            int r = Ret[Ret.Count - 1];
                            for (int j = 0; j < Cnt - 1; j++)
                            {
                                Ret.Add(r);
                            }
                        }
                    }
                }
            }
            return Ret;
        }
    }
}
