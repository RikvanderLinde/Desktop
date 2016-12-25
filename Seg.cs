using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace FileIconDemo
{
    public class Seg
    {
        [XmlIgnore]
        public Button but = new Button();
        [XmlIgnore]
        public Form window = new Form();
        [XmlIgnore]
        public ContextMenu menu = new ContextMenu();
        [XmlIgnore]
        public int action = 0; //0 = nothing , 1 = resize , 2 = moving
        [XmlIgnore]
        public Color color = Color.White;

        [XmlElement(ElementName = "color")]
        public int color_XmlSurrogate
        {
            get { return color.ToArgb(); }
            set { color = Color.FromArgb(value); }
        }

        public int X = 0, Y = 0, Width = 0 , Height = 0;
        public string Name = "";

        public Seg() { }

        public Seg(int x, int y, string n, int width = 64, int height = 64)
        {
            Name = n;
            Segmentcolor(color, permanent: true);
            but.TabStop = false;
            //but.FlatStyle = FlatStyle.Flat;
            but.FlatAppearance.BorderSize = 0;
            but.Dock = DockStyle.Fill;
            but.BackColor = color;
            but.Click += new EventHandler(click);
            window.Controls.Add(but);
            //window.TopMost = true;
            window.FormBorderStyle = FormBorderStyle.None;
            window.ShowInTaskbar = false;
            //window.BackColor = Color.LimeGreen;
            //window.TransparencyKey = Color.LimeGreen;
            window.Opacity = 0.8;
            window.Show();

            setPos(x, y);
            Size s = new Size(width,height);
            setSize(s);
            
            menu.MenuItems.Add("Move").Click += move;
            menu.MenuItems.Add("Resize").Click += size;
            menu.MenuItems.Add("Color").Click += colorChange;
            menu.MenuItems.Add("Remove").Click += remove;
            menu.Popup += enable;
            but.ContextMenu = menu;
        }

        public void enable(object sender, CancelEventArgs e)
        {
            if (!Globals.editing) e.Cancel = true;
        }

        public void click(object sender, EventArgs e)
        {
            if (Globals.selected == this)
            {
                Globals.unselect = true;
                Globals.selected.Segmentcolor(color);
                action = 0;
                if (Globals.selected is Seg_Controller)
                {
                    Seg_Controller self = (Seg_Controller)this;
                    self.clear();
                }
            }
            else if(Globals.selected is Seg_Controller)
            {
                Seg_Icon self = (Seg_Icon)this;
                if (((Seg_Controller)Globals.selected).controlledSegs.ContainsKey(self))
                {
                    ((Seg_Controller)Globals.selected).controlledSegs.Remove(self);
                    self.Segmentcolor(color);
                }
                else
                {
                    ((Seg_Controller)Globals.selected).controlledSegs.Add(self, window.Location);
                    self.Segmentcolor(Color.Purple);
                }
            }
            else
            {
                Run();
            }
        }

        public void move(object sender, EventArgs e)
        {
            //Move icon
            if (Globals.selected != this)
            {
                Globals.selected = this;
                action = 2;
            }
            else
            {
                action = 0;
            }
        }

        public void size(object sender, EventArgs e)
        {
            //Resize icon
            if (Globals.selected != this)
            {
                Globals.selected = this;
                action = 1;
            }
            else
            {
                action = 0;
            }
        }

        public void remove(object sender, EventArgs e)
        {
            //Remove icon
            window.Close();
        }

        public void colorChange(object sender, EventArgs e)
        {
            //Change the Color

            ColorDialog MyDialog = new ColorDialog();
            // Keeps the user from selecting a custom color.
            MyDialog.AllowFullOpen = false;
            // Allows the user to get help. (The default is false.)
            MyDialog.ShowHelp = true;
            // Sets the initial color select to the current text color.
            MyDialog.Color = color;

            // Update the text box color if the user clicks OK 
            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
                Segmentcolor(MyDialog.Color, permanent: true);
            }
        }

        virtual public void Run()
        {
            //Nothing, childs have new ones
        }

        public void Image(Bitmap icon)
        {
            but.BackgroundImage = icon;
            but.BackgroundImageLayout = ImageLayout.Center;
        }

        public void setSize(Size s)
        {
            window.Size = s;
            Width = s.Width;
            Height = s.Height;
        }

        public void setPos(int x, int y)
        {
            window.Location = new Point(x, y);
            X = x;
            Y = y;
        }

        public void Segmentcolor(Color c, Boolean permanent = false)
        {
            if(permanent) color = c;
            but.BackColor = c;
        }
        
        public void mousepos()
        {
            int x = (int)Math.Round((Cursor.Position.X - window.Width / 2.0) / 64) * 64;
            int y = (int)Math.Round((Cursor.Position.Y - window.Height / 2.0) / 64) * 64;
            setPos(x, y);
        }

        public void resize()
        {
            int x = window.Location.X;
            int y = window.Location.Y;
            int xx = Cursor.Position.X - window.Width;
            int yy = Cursor.Position.Y - window.Height;
            Size s = window.Size;

            if (xx > x)
            {
                s.Width += 64;
            }
            if (yy > y)
            {
                s.Height += 64;
            }
            if (xx < x - 63 && window.Width > 64)
            {
                s.Width -= 64;
            }
            if (yy < y - 63 && window.Height > 64)
            {
                s.Height -= 64;
            }

            setSize(s);
        }
    }
}
