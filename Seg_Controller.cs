using System;
using System.Drawing;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FileIconDemo
{
    public class Seg_Controller : Seg
    {
        public Boolean expanded = true;
        public SerializableDictionary<Seg_Icon, Point> controlledSegs = new SerializableDictionary<Seg_Icon, Point>();

        public Seg_Controller() { }

        public Seg_Controller(int x, int y, string name, int width = 64, int height = 64) : base(x, y, name, width, height)
        {
            menu.MenuItems.Add("Setup").Click += new EventHandler(setup);
        }

        public void setup(object sender, EventArgs e)
        {
            if (Globals.selected != this)
            {
                Globals.selected = this;
                Segmentcolor(Color.Red);
            }
            foreach (var icon in controlledSegs)
            {
                icon.Key.Segmentcolor(Color.RoyalBlue);
            }
        }

        public void clear()
        {
            foreach (var icon in controlledSegs)
            {
                icon.Key.Segmentcolor(icon.Key.color);
            }
        }

        override public void Run()
        {
            //Expand controllforeach (var icon in tempSegs)
            Dictionary<Seg_Icon, Point> tempSegs = new Dictionary<Seg_Icon, Point>(controlledSegs);
            Boolean update = true;
            foreach (var icon in tempSegs)
            {
                if (icon.Key.relocate)
                    update = false;
            }
            if (update)
            {
                if (expanded)
                {
                    controlledSegs.Clear();
                    foreach (var icon in tempSegs)
                    {
                        icon.Key.nextpos(window.Location);//Collapse

                        Point p = icon.Key.window.Location;
                        Seg_Icon i = icon.Key;
                        controlledSegs.Add(i, p);
                    }

                }
                else
                {
                    foreach (var icon in controlledSegs)
                    {
                        icon.Key.nextpos(icon.Value);//Expand
                    }
                }
                expanded = !expanded;
            }
        }
    }
}
