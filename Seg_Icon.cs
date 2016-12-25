using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace FileIconDemo
{
    public class Seg_Icon: Seg
    {
        [XmlIgnore]
        Random random = new Random();
        [XmlIgnore]
        public int xnext, ynext;
        [XmlIgnore]
        public Boolean relocate = false;
        
        public Seg_Icon() { }

        public Seg_Icon(int x, int y, string n, int width = 64, int height = 64) : base(x, y, n, width, height)
        {
            Name = n;
        }

        override public void Run()
        {
            //Run program
            System.Diagnostics.Process.Start(Name);
        }

        public void nextpos(int x, int y)
        {
            if (!relocate)
            {
                xnext = x;
                ynext = y;
                relocate = true;
            }
        }

        public void nextpos(Point p)
        {
            if (!relocate)
            {
                xnext = p.X;
                ynext = p.Y;
                relocate = true;
            }
        }

        public void moveSegment()
        {
            if (relocate)
            {
                int x = window.Location.X;
                int y = window.Location.Y;
                double count = Application.OpenForms.Count/8;

                if (Math.Abs(xnext - x) < (int)Math.Abs((Math.Log(xnext - x) * count) + 1) || Math.Abs(xnext - x) < 5) x = xnext;
                if (Math.Abs(ynext - y) < (int)Math.Abs((Math.Log(ynext - y) * count) + 1) || Math.Abs(ynext - y) < 5) y = ynext;
                if (x == xnext && y == ynext)
                {
                    relocate = false;
                }
                else
                {
                    if (x < xnext) x += (int)(Math.Log(xnext - x) * count)+1;
                    else if (x > xnext) x -= (int)(Math.Log(x - xnext) * count)+1;
                    if (y < ynext) y += (int)(Math.Log(ynext - y) * count)+1;
                    else if (y > ynext) y -= (int)(Math.Log(y - ynext) * count)+1;
                }
                setPos(x, y);
            }
        }
        
        public void rand()
        {
            nextpos(random.Next(Screen.PrimaryScreen.Bounds.Width-window.Width), random.Next(Screen.PrimaryScreen.Bounds.Height-window.Height));
        }
    }
}
