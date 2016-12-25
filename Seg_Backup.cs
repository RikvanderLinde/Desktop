using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;

namespace FileIconDemo
{
    public class segment
    {
        public int xnext, ynext;
        public Button but = new Button();
        public Form window = new Form();
        public string Name = "";
        public Boolean sizing = false, moving = false, relocate = false, controller = false, expanded = false;
        public Color color = Color.Black;
        Random random = new Random();
        public Dictionary<segment, Point> controlledSegs = new Dictionary<segment, Point>();

        public segment(int x, int y, string name, int width = 64, int height = 64, Boolean cont = false)
        {
            Name = name;
            controller = cont;

            but.TabStop = false;
            //but.FlatStyle = FlatStyle.Flat;
            but.FlatAppearance.BorderSize = 0;
            but.Click += new EventHandler(click);
            but.Dock = DockStyle.Fill;
            but.BackColor = color;

            window.Controls.Add(but);
            window.TopMost = true;
            window.FormBorderStyle = FormBorderStyle.None;
            window.ShowInTaskbar = false;
            //window.BackColor = Color.LimeGreen;
            //window.TransparencyKey = Color.LimeGreen;
            window.Opacity = 0.8;
            window.Show();

            pos(x, y);
            size(width, height);

            ContextMenu menu = new ContextMenu();
            if (controller)
            {
                
                Segmentcolor(Color.Cyan, permanent: true);
            }
            menu.MenuItems.Add("Select").Click += new EventHandler(select);
            menu.MenuItems.Add("Move").Click += new EventHandler(move);
            menu.MenuItems.Add("Resize").Click += new EventHandler(size);
            menu.MenuItems.Add("Remove").Click += new EventHandler(remove);
            but.ContextMenu = menu;
        }

        segment() { }
        ~segment(){ }

        public void click(object sender, EventArgs e)
        {
            //Execute icon
            Segmentcolor(Color.Purple);
            if (controller)
            {
                if (expanded)
                {
                    expanded = !expanded;
                    foreach (var icon in controlledSegs)
                    {
                        icon.Key.nextpos(icon.Value);
                    }
                }
                else
                {
                    expanded = !expanded;
                    foreach (var icon in controlledSegs)
                    {
                        icon.Key.nextpos(window.Location);
                    }
                }
            }
            else
            {
                //Run file
            }
        }

        public void move(object sender, EventArgs e)
        {
            //Move icon
            if (Globals.selected != this) Globals.selected = this;
            moving = true;
            sizing = false;
        }

        public void size(object sender, EventArgs e)
        {
            //Resize icon
            if (Globals.selected != this) Globals.selected = this;
            sizing = true;
            moving = false;
        }

        public void remove(object sender, EventArgs e)
        {
            //Remove icon
            Globals.delete = this;
            window.Close();
        }

        public void select(object sender, EventArgs e)
        {
            //Select segment
            if (Globals.selected != this)// Segment is not self
            {
                if (Globals.selected != null && Globals.selected.controller) // Segment is a controller and set
                {
                    if (!Globals.selected.controlledSegs.ContainsKey(this))
                    {
                        Globals.selected.controlledSegs.Remove(this);
                    }
                    Globals.selected.controlledSegs.Add(this, window.Location);
                    
                }
                else // Segment is not a controller
                {
                    Segmentcolor(Color.Red);
                    Globals.selected = this;
                    sizing = false;
                    moving = false;
                }
            }
            else//Segment is self
            {
                Segmentcolor(color);
                Globals.unselect = true;
                sizing = false;
                moving = false;
            }
        }

        public void Image(Bitmap icon)
        {
            but.BackgroundImage = icon;
            but.BackgroundImageLayout = ImageLayout.Center;
        }

        public void size(int width, int height)
        {
            window.Size = new Size(width, height);
        }

        public void pos(int x, int y)
        {
            window.Location = new Point(x, y);
        }

        public void Segmentcolor(Color c, Boolean permanent = false)
        {
            if(permanent) color = c;
            but.BackColor = c;
        }

        public void nextpos(int x, int y)
        {
            xnext = x;
            ynext = y;
            relocate = true;
        }

        public void nextpos(Point p)
        {
            xnext = p.X;
            ynext = p.Y;
            relocate = true;
        }

        public void moveSegment()
        {
            if (relocate)
            {
                int x = window.Location.X;
                int y = window.Location.Y;
                int count = Application.OpenForms.Count / 10;

                if ((xnext - x) < ((int)Math.Log(xnext - x) * count) || Math.Abs(xnext - x) < 5) x = xnext;
                if ((ynext - y) < ((int)Math.Log(ynext - y) * count) || Math.Abs(ynext - y) < 5) y = ynext;
                if (x == xnext && y == ynext)
                {
                    relocate = false;
                }
                else
                {
                    if (x < xnext) x += (int)Math.Log(xnext - x) * count;
                    else if (x > xnext) x -= (int)Math.Log(x - xnext) * count;
                    if (y < ynext) y += (int)Math.Log(ynext - y) * count;
                    else if (y > ynext) y -= (int)Math.Log(y - ynext) * count;
                }

                window.Location = new Point(x, y);
            }
        }

        public void orbit(int a)
        {
            int x = window.Location.X;
            int y = window.Location.Y;

            int xcent = Screen.PrimaryScreen.Bounds.Width / 2;
            int ycent = Screen.PrimaryScreen.Bounds.Height / 2;
            int count = Application.OpenForms.Count / 5;

            int r = (int)Math.Sqrt((x - xcent) * (x - xcent) + (y - ycent) * (y - ycent));
            
            //int a = (int)(Math.Atan2(sin, cos) * (180 / Math.PI));

            x = (int)(xcent + (r * Math.Cos(a))); // * (Math.PI/180))
            y = (int)(ycent + (r * Math.Sin(a)));
            
            window.Location = new Point(x, y);
        }

        public void mousepos()
        {
            int x = (int)Math.Round((Cursor.Position.X - window.Width / 2.0) / 64) * 64;
            int y = (int)Math.Round((Cursor.Position.Y - window.Height / 2.0) / 64) * 64;
            pos(x, y);
        }

        public void resize()
        {
            int x = window.Location.X;
            int y = window.Location.Y;
            int xx = Cursor.Position.X - window.Width;
            int yy = Cursor.Position.Y - window.Height;

            if (xx > x)
            {
                window.Width += 64;
            }
            if (yy > y)
            {
                window.Height += 64;
            }
            if (xx < x - 63 && window.Width > 64)
            {
                window.Width -= 64;
            }
            if (yy < y - 63 && window.Height > 64)
            {
                window.Height -= 64;
            }
        }

        public void rand()
        {
            nextpos(random.Next(Screen.PrimaryScreen.Bounds.Width-window.Width), random.Next(Screen.PrimaryScreen.Bounds.Height-window.Height));
        }
    }
}
