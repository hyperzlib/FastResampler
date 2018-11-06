using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastResampler.Param;
using System.IO;
using FastResampler;

namespace FastResampler.Note
{
    public class NoteList
    {
        private LangPack lang;
        private Oto oto;
        public List<NoteImpl> noteList = new List<NoteImpl>();
        public Pitch pitch = new Pitch();
        public long length = 0;
        public string voiceSource = "";

        public NoteList(string voiceSource)
        {
            this.lang = Global.lang;
            this.voiceSource = voiceSource;
            this.oto = new Oto(voiceSource, lang);
        }

        public void Add(NoteImpl note)
        {
            if(note.flag == "lyric")
            {
                LyricNote lyricNote = (LyricNote)note;
                this.pitch.Append(lyricNote.pitchStr, lyricNote.getRealLength(), lyricNote.getBasePitch(), lyricNote.overlap);
                lyricNote.pitch = this.pitch;
            }
            else
            {
                List<int> tempPitch = new List<int>();
                tempPitch.Add(0);
                this.pitch.Append(tempPitch, note.getLength());
            }
            if(this.noteList.Count > 0)
            {
                note.prevNote = this.noteList[this.noteList.Count - 1];
                this.noteList[this.noteList.Count - 1].nextNote = note;
            }
            this.noteList.Add(note);
            this.length += note.getRealLength();
        }

        public string[] getResamplerParam(int index)
        {

            if (index >= 0 && index <= this.noteList.Count && this.noteList[index].flag == "lyric")
            {
                LyricNote note = (LyricNote)this.noteList[index];
                return note.getResamplerParam();
            }
            else
            {
                return null;
            }
        }

        public string[][] getResamplerParams()
        {
            List<string[]> paramList = new List<string[]>();
            int i;
            for (i = 0; i < noteList.Count; i ++)
            {
                if(noteList[i].flag == "lyric")
                {
                    paramList.Add(this.getResamplerParam(i));
                }
            }
            return paramList.ToArray();
        }

        public void update()
        {
            if (this.oto != null)
            {
                for (int i = 0; i < noteList.Count; i++)
                {
                    if (noteList[i].flag == "lyric")
                    {
                        LyricNote tempNote = (LyricNote)noteList[i];
                        otodata tempData = this.oto.getToneData(noteList[i].lyric);
                        otodata nextData;
                        int lengthMsec, osLengthMsec, edLengthMsec, signLen, eLen;
                        int st, es, ed, ve, ws, cf, free;
                        int edBegin, edEnd, endlap;
                        double velocity, vRatio = 1.0;
                        if (!string.IsNullOrEmpty(tempData.file))
                        {
                            tempNote.sourceFile = voiceSource + "\\" + tempData.file;

                            tempNote.startcut = Convert.ToInt32(tempData.offset);
                            tempNote.consonant = Convert.ToInt32(tempData.consonant);
                            tempNote.preutterance = Convert.ToInt32(tempData.preutterance);
                            //计算endlap
                            velocity = tempNote.velocity;
                            vRatio = Math.Pow(2.0, (1.0 - (velocity / 100.0)));
                            if (tempData.endlap != 0)
                            {
                                lengthMsec = Convert.ToInt32(tempNote.getRealLength());
                                osLengthMsec = Convert.ToInt32(tempData.offset);
                                edLengthMsec = Convert.ToInt32(tempData.length) - Convert.ToInt32(tempData.cutoff);
                                signLen = edLengthMsec - osLengthMsec;
                                st = Convert.ToInt32(tempData.consonant); //辅音长度
                                es = Convert.ToInt32(tempData.vend); //元音尾长度
                                ws = signLen - st - es; //白区长度
                                ve = signLen - es; //元音尾的绝对长度
                                cf = Convert.ToInt32(tempData.cutoff);
                                free = Convert.ToInt32(lengthMsec - (st / vRatio) - ws);

                                if (tempNote.nextNote == null || tempNote.nextNote.flag != "lyric") //结尾时，范围是endlap到结束
                                {
                                    endlap = Convert.ToInt32(signLen - tempData.endlap - ve);
                                    ed = signLen - ve;
                                    endlap += (ed - endlap) / 2;
                                }
                                else
                                {
                                    nextData = this.oto.getToneData(tempNote.nextNote.lyric);
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
                                    tempNote.endcut = Convert.ToInt32(Math.Max(0, signLen - (ve + edBegin * vRatio)) + cf);
                                    tempNote.vend = Convert.ToInt32(edBegin * vRatio);
                                }
                                else if (free > edBegin && free < edEnd)
                                {
                                    tempNote.endcut = Convert.ToInt32(Math.Max(0, signLen - (ve + free * vRatio)) + cf);
                                    tempNote.vend = Convert.ToInt32(free * vRatio);
                                }
                                else
                                {
                                    tempNote.endcut = Convert.ToInt32(Math.Max(0, signLen - (ve + edEnd * vRatio)) + cf);
                                    tempNote.vend = Convert.ToInt32(edEnd * vRatio);
                                }
                            }
                            tempNote.overlap = Convert.ToInt32(tempData.overlap * vRatio);
                        }
                    }
                }
            }
            for (int i = 0; i < noteList.Count; i++)
            {
                if(noteList[i].flag == "lyric" && note)
                noteList[i].update();
            }
            for (int i = 0; i < noteList.Count; i++)
            {
                noteList[i].update();
            }
        }
    }
}
