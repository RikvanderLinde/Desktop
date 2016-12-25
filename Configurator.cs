using System;
using System.Windows.Forms;
using Etier.IconHelper;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Drawing;

namespace FileIconDemo
{
    public class Configurator : Form
    {
        //Buttons
        private Button butOne;
        private Button butAlot;
        private Button butClose;
        private Button butSave;
        private Button butLoad;
        private Button butRandom;
        private Button butAddcontrol;
        private RadioButton radioButEdit;

        //Vars
        Thread UpdateThread;
        public string saveFile = @"C:\icons.xml";

        //Lists
        public static List<Seg_Icon> AllIcons    = new List<Seg_Icon>();
        public static List<Seg_Controller> AllControls = new List<Seg_Controller>();

        //Random
        private NotifyIcon SystemTray;
        private System.ComponentModel.IContainer components;

        public Configurator()
		{
            InitializeComponent();
            UpdateThread = new Thread(updater);
            UpdateThread.Start();

            ContextMenu menu = new ContextMenu();
            menu.MenuItems.Add("Add Controller").Click += new EventHandler(butAddcontrol_Click);
            menu.MenuItems.Add("Add Icons").Click += new EventHandler(addAlotButton_Click);
            //menu.MenuItems.Add("Open Config").Click += new EventHandler(remove);
            menu.MenuItems.Add("Save").Click += new EventHandler(SaveButton_Click);
            menu.MenuItems.Add("Load").Click += new EventHandler(LoadButton_Click);
            menu.MenuItems.Add("Close").Click += new EventHandler(CloseButton_Click);

            SystemTray.ContextMenu = menu;
        }
        
        private void load()
        {
            // Remove everything
            foreach (var icon in AllIcons)
            {
                icon.remove(null, null);
            }
            AllIcons.Clear();
            foreach (var control in AllControls)
            {
                control.remove(null, null);
            }
            AllControls.Clear();

            string file = saveFile;
            XmlSerializer Iconformatter = new XmlSerializer(AllIcons.GetType());
            XmlSerializer Controlformatter = new XmlSerializer(AllControls.GetType());
            FileStream aFile = new FileStream(file, FileMode.Open);
            byte[] buffer = new byte[aFile.Length];
            aFile.Read(buffer, 0, (int)aFile.Length);

            int iconLength = 0;

            Console.WriteLine(aFile.Length);
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == (byte)'~')
                {
                    iconLength = i;
                    Console.WriteLine(i);
                    break;
                }
            }
            byte[] bufferIcon = new byte[iconLength];
            byte[] bufferControl = new byte[aFile.Length - iconLength];
            Array.Copy(buffer, 0, bufferIcon, 0, iconLength);
            Array.Copy(buffer, iconLength + 1, bufferControl, 0, aFile.Length - iconLength - 1);

            MemoryStream streamIcon = new MemoryStream(bufferIcon);
            MemoryStream streamControl = new MemoryStream(bufferControl);

            List<Seg_Icon> NewIcons = new List<Seg_Icon>();
            List<Seg_Controller> NewControls = new List<Seg_Controller>();
            
            NewIcons = (List<Seg_Icon>)Iconformatter.Deserialize(streamIcon);
            foreach (var icon in NewIcons)
            {
                Seg_Icon seg = new Seg_Icon(icon.X, icon.Y, icon.Name,icon.Width, icon.Height);
                seg.Image(IconReader.GetFileIcon(icon.Name, 0, false).ToBitmap());
                seg.Segmentcolor(icon.color, permanent : true);
                AllIcons.Add(seg);
            }
            
            NewControls = (List<Seg_Controller>)Controlformatter.Deserialize(streamControl);
            foreach (var control in NewControls)
            {
                Seg_Controller seg = new Seg_Controller(control.X, control.Y, control.Name, control.Width, control.Height);
                foreach (var item in control.controlledSegs)
                {
                    Seg_Icon i = AllIcons.Find(thing => thing.Name == item.Key.Name);
                    Point p = new Point(item.Value.X, item.Value.Y);
                    seg.controlledSegs.Add(i,p);
                }
                seg.Segmentcolor(control.color, permanent: true);
                seg.expanded = control.expanded;
                AllControls.Add(seg);
            }
            aFile.Close();
        }
        
        private void save()
        {
            FileStream outFile = File.Create(saveFile);
            XmlSerializer formatter = new XmlSerializer(AllIcons.GetType());
            formatter.Serialize(outFile, AllIcons);
            outFile.WriteByte((byte)'~');
            formatter = new XmlSerializer(AllControls.GetType());
            formatter.Serialize(outFile, AllControls);
            outFile.Close();
        }

        static void Main() 
		{
			Application.Run(new Configurator());
		}
        
        private void updater()
        {
            while (true)
            {
                List<Seg_Icon> AllFakeElements = new List<Seg_Icon>(AllIcons);
                
                foreach (var segments in AllFakeElements )
                {
                    Invoke(new MethodInvoker(delegate { segments.moveSegment(); }));
                }

                if (Globals.unselect)
                {
                    Globals.selected = null;
                    Globals.unselect = false;
                }
                else
                {
                    if (Globals.selected != null)
                    {
                        if (Globals.selected.action == 1) Invoke(new MethodInvoker(delegate { Globals.selected.resize(); }));
                        else if (Globals.selected.action == 2) Invoke(new MethodInvoker(delegate { Globals.selected.mousepos(); }));
                    }
                    if (Globals.delete != null)
                    {
                        Invoke(new MethodInvoker(delegate { Globals.delete.remove(null,null); }));
                        Globals.delete = null;
                    }
                }
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Configurator));
            this.butOne = new System.Windows.Forms.Button();
            this.butAlot = new System.Windows.Forms.Button();
            this.butClose = new System.Windows.Forms.Button();
            this.butSave = new System.Windows.Forms.Button();
            this.butLoad = new System.Windows.Forms.Button();
            this.butRandom = new System.Windows.Forms.Button();
            this.butAddcontrol = new System.Windows.Forms.Button();
            this.SystemTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.radioButEdit = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // butOne
            // 
            this.butOne.Location = new System.Drawing.Point(3, 3);
            this.butOne.Name = "butOne";
            this.butOne.Size = new System.Drawing.Size(75, 23);
            this.butOne.TabIndex = 1;
            this.butOne.Text = "Add one";
            this.butOne.UseVisualStyleBackColor = true;
            this.butOne.Click += new System.EventHandler(this.addOneButton_Click);
            // 
            // butAlot
            // 
            this.butAlot.Location = new System.Drawing.Point(3, 32);
            this.butAlot.Name = "butAlot";
            this.butAlot.Size = new System.Drawing.Size(75, 23);
            this.butAlot.TabIndex = 2;
            this.butAlot.Text = "Add alot";
            this.butAlot.UseVisualStyleBackColor = true;
            this.butAlot.Click += new System.EventHandler(this.addAlotButton_Click);
            // 
            // butClose
            // 
            this.butClose.Location = new System.Drawing.Point(84, 119);
            this.butClose.Name = "butClose";
            this.butClose.Size = new System.Drawing.Size(75, 23);
            this.butClose.TabIndex = 3;
            this.butClose.Text = "Close";
            this.butClose.UseVisualStyleBackColor = true;
            this.butClose.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // butSave
            // 
            this.butSave.Location = new System.Drawing.Point(84, 3);
            this.butSave.Name = "butSave";
            this.butSave.Size = new System.Drawing.Size(75, 23);
            this.butSave.TabIndex = 4;
            this.butSave.Text = "Save";
            this.butSave.UseVisualStyleBackColor = true;
            this.butSave.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // butLoad
            // 
            this.butLoad.Location = new System.Drawing.Point(84, 32);
            this.butLoad.Name = "butLoad";
            this.butLoad.Size = new System.Drawing.Size(75, 23);
            this.butLoad.TabIndex = 5;
            this.butLoad.Text = "Load";
            this.butLoad.UseVisualStyleBackColor = true;
            this.butLoad.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // butRandom
            // 
            this.butRandom.Location = new System.Drawing.Point(3, 119);
            this.butRandom.Name = "butRandom";
            this.butRandom.Size = new System.Drawing.Size(75, 23);
            this.butRandom.TabIndex = 6;
            this.butRandom.Text = "Random";
            this.butRandom.UseVisualStyleBackColor = true;
            this.butRandom.Click += new System.EventHandler(this.butRandom_Click);
            // 
            // butAddcontrol
            // 
            this.butAddcontrol.Location = new System.Drawing.Point(3, 61);
            this.butAddcontrol.Name = "butAddcontrol";
            this.butAddcontrol.Size = new System.Drawing.Size(75, 23);
            this.butAddcontrol.TabIndex = 8;
            this.butAddcontrol.Text = "Add Control";
            this.butAddcontrol.UseVisualStyleBackColor = true;
            this.butAddcontrol.Click += new System.EventHandler(this.butAddcontrol_Click);
            // 
            // SystemTray
            // 
            this.SystemTray.Icon = ((System.Drawing.Icon)(resources.GetObject("SystemTray.Icon")));
            this.SystemTray.Text = "SystemTray";
            this.SystemTray.Visible = true;
            // 
            // radioButEdit
            // 
            this.radioButEdit.AutoSize = true;
            this.radioButEdit.Location = new System.Drawing.Point(3, 90);
            this.radioButEdit.Name = "radioButEdit";
            this.radioButEdit.Size = new System.Drawing.Size(82, 17);
            this.radioButEdit.TabIndex = 9;
            this.radioButEdit.TabStop = true;
            this.radioButEdit.Text = "radioButEdit";
            this.radioButEdit.UseVisualStyleBackColor = true;
            this.radioButEdit.CheckedChanged += new System.EventHandler(this.radioButEdit_CheckedChanged);
            // 
            // Configurator
            // 
            this.ClientSize = new System.Drawing.Size(166, 150);
            this.Controls.Add(this.radioButEdit);
            this.Controls.Add(this.butOne);
            this.Controls.Add(this.butAlot);
            this.Controls.Add(this.butClose);
            this.Controls.Add(this.butSave);
            this.Controls.Add(this.butLoad);
            this.Controls.Add(this.butRandom);
            this.Controls.Add(this.butAddcontrol);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Configurator";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UpdateThread.Abort();
        }

        private void addOneButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgOpenFile = new OpenFileDialog();
            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                Seg_Icon seg = new Seg_Icon(0, 0, dlgOpenFile.FileName);
                seg.Image(IconReader.GetFileIcon(dlgOpenFile.FileName, 0, false).ToBitmap());
                AllIcons.Add(seg);
            }
        }

        private void addAlotButton_Click(object sender, EventArgs e)
        {
            string[] Files = Directory.GetFiles("C:\\Users\\Rik\\Desktop\\Test");

            foreach (String file in Files)
            {
                Seg_Icon seg = new Seg_Icon(0, 0, file);
                seg.Image(IconReader.GetFileIcon(file, 0, false).ToBitmap());
                AllIcons.Add(seg);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            save();
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            load();
        }

        private void butRandom_Click(object sender, EventArgs e)
        {
            foreach (var segments in AllIcons)
            {
                Invoke(new MethodInvoker(delegate { segments.rand(); }));
            }
        }

        private void butAddcontrol_Click(object sender, EventArgs e)
        {
            Seg_Controller seg = new Seg_Controller(0, 0,"Games");
            AllControls.Add(seg);
        }

        private void radioButEdit_CheckedChanged(object sender, EventArgs e)
        {
            Globals.editing = !Globals.editing;
        }
    }
}
