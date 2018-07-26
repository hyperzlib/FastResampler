using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xaml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FastResampler
{
    public struct note {
        public string lyric;
        public int position;
        public int length;
    }

    public class NotePainter
    {
        public Canvas canvas;
        public UtauBat bat;
        public List<note> noteList = new List<note>();

        public int width, height;
        public NotePainter(Canvas canvas, int width, int height, UtauBat bat)
        {
            this.canvas = canvas;
            this.width = width;
            this.height = height;
            this.bat = bat;
            this.initNote();
        }

        public void initNote()
        {
            double nowPos = 0;
            foreach (toolParam tempParam in bat.toolParams)
            {
                note oneNote = new note();
                oneNote.position = Convert.ToInt32(nowPos);
                oneNote.length = Convert.ToInt32(nowPos + Utils.parseLength(tempParam.length) - nowPos);
                if(tempParam.resamParamId == -1){
                    oneNote.lyric = "R";
                } else {
                    oneNote.lyric = bat.resamParams[tempParam.resamParamId].gen;
                }
                noteList.Add(oneNote);
                nowPos += Utils.parseLength(tempParam.length);
            }
        }

        public void drawNote(int nowSeek = 0)
        {
            int i;
            int maxWidth = this.width;
            double tWidth;
            double l2w = 0.3;
            int nowPos = 0;
            int centerNote = noteList.Count - 1;
            /*Pen pen = new Pen(Color.Black);
            Pen redPen = new Pen(Color.Red);
            Font lrcFont = new Font("Microsoft Yahei", 12, FontStyle.Bold);
            Brush blackBush = new SolidBrush(Color.Black);
            Brush aquaBush = new SolidBrush(Color.Aqua);
            Brush greenBush = new SolidBrush(Color.LightGreen);*/
            string nowLyric = "";
            double playedInNote = 0;
            int startNote = 0;
            for (i = 0; i < noteList.Count; i++)
            { //计算当前时间轴所在的音符位置
                if (noteList[i].position <= nowSeek && (noteList[i].position + noteList[i].length) > nowSeek)
                {
                    centerNote = i;
                    break;
                }
            }
            //绘制前面的音符
            //这里nowPos指的是音符尾部
            playedInNote = (nowSeek - noteList[centerNote].position) * l2w;
            nowPos = (width / 2) - Convert.ToInt32(playedInNote);
            startNote = 0;
            for (i = centerNote - 1; i >= 0; i--)
            {
                //推定开始的音符及位置
                nowPos -= Convert.ToInt32(noteList[i].length * l2w);
                if (nowPos < 0)
                {
                    startNote = i;
                    break;
                }
            }
            this.canvas.Children.Clear();
            /*for (i = startNote; i < centerNote; i++)
            {
                nowLyric = noteList[i].lyric;
                tWidth = noteList[i].length * l2w;
                Rectangle rect1 = new Rectangle();
                rect1.Stroke = Brushes.Black;
                rect1.StrokeThickness = 1;
                rect1.Fill = Brushes.LightGreen;
                rect1.Width = Convert.ToInt32(tWidth);
                rect1.Height = height - 2;
                rect1.
                this.canvas.Children.Add(rect1);
                canvas.DrawRectangle(pen, nowPos, 0, , height - 2);
                canvas.FillRectangle(greenBush, nowPos + 1, 1, Convert.ToInt32(tWidth), height - 3);
                canvas.DrawString(nowLyric, lrcFont, blackBush, Convert.ToInt32(nowPos) + 3, 0);
                nowPos += Convert.ToInt32(tWidth);
            }
            //绘制中心音符
            nowPos = (width / 2) - Convert.ToInt32(playedInNote);
            nowLyric = noteList[centerNote].lyric;
            canvas.DrawRectangle(pen, nowPos, 0, Convert.ToInt32(playedInNote), height - 2);
            canvas.FillRectangle(greenBush, nowPos + 1, 1, Convert.ToInt32(playedInNote), height - 3);
            canvas.DrawRectangle(pen, width / 2, 0, Convert.ToInt32(noteList[centerNote].length * l2w - playedInNote), height - 2);
            canvas.DrawLine(redPen, width / 2, 0, width / 2, height);
            canvas.FillRectangle(aquaBush, width / 2 + 1, 1, Convert.ToInt32(noteList[centerNote].length * l2w - playedInNote) - 1, height - 3);
            canvas.DrawString(nowLyric, lrcFont, blackBush, Convert.ToInt32(nowPos) + 3, 0);
            nowPos += Convert.ToInt32(noteList[centerNote].length * l2w);
            for (i = centerNote + 1; i < noteList.Count; i++)
            {
                nowLyric = noteList[i].lyric;
                tWidth = noteList[i].length * l2w;
                canvas.DrawRectangle(pen, nowPos, 0, Convert.ToInt32(tWidth), height - 2);
                canvas.FillRectangle(aquaBush, nowPos + 1, 1, Convert.ToInt32(tWidth), height - 3);
                canvas.DrawString(nowLyric, lrcFont, blackBush, Convert.ToInt32(nowPos) + 3, 0);
                nowPos += Convert.ToInt32(tWidth);
            }*/
            /*if (nowPos < width)
            {
                Brush tb = new SolidBrush(Control.DefaultBackColor);
                canvas.FillRectangle(tb, Convert.ToInt32(nowPos), 0, Convert.ToInt32(width - nowPos), height);
            }*/
        }
    }
}
