using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Virtel
{
    public partial class Form1 : Form
    {
        PrivateFontCollection f = new PrivateFontCollection();
        public bool work = false;

        public char? result = null;
        public bool wait = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*
            f.AddFontFile("pico8.ttf");
            this.Font = new Font(f.Families[0], 9);
            this.menuStrip1.Font = new Font(f.Families[0], 9);
            */
        }

        private void label2_Enter(object sender, EventArgs e)
        {
            this.Focus();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (wait)
            {
                result = e.KeyChar;
                wait = false;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            
        }
    }
}
