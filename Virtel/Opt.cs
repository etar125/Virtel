using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Virtel
{
    public partial class Opt : Form
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
            IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);

        private PrivateFontCollection fonts = new PrivateFontCollection();

        public Font myFont;

        public Dictionary<string, string> adresses = new Dictionary<string, string>
        {
            {"DT", ""}, // Display Text
            {"AS", "1"}, // App Status
            {"RC", ""}, // Received key
            {"GP", "0"}, // getpart
            {"GPS", ""}, // getpart source
            {"GPR", ""}, // getpart result
            {"GPI", ""}, // getpart start index
            {"GPL", ""}, // getpart length
        };

        // {"", ""}, // 

        public void SetDefolt()
        {
            adresses = new Dictionary<string, string>
            {
                {"DT", ""}, // Display Text
                {"AS", "1"}, // App Status
                {"RC", ""}, // Received key
                {"GP", "0"}, // getpart
                {"GPS", ""}, // getpart source
                {"GPR", ""}, // getpart result
                {"GPI", ""}, // getpart start index
                {"GPL", ""}, // getpart length
            };
        }

        Form1 f = new Form1();

        public bool work = false;
        public void AdressCheck()
        {
            // Memory DELETED

            //MUpdate();

            // Display Text

            f.label2.Text = adresses["DT"];

            // App Status

            string st = adresses["AS"];

            if (st == "0")
                work = false;
            else if (st == "2")
            {
                f.wait = true;
                while (f.wait) { };
                adresses["RC"] = f.result.ToString();
            }
            else
                work = true;

            // Get Part

            if(adresses["GP"] == "1")
            {
                adresses["GPR"] = adresses["GPS"].Substring(int.Parse(adresses["GPI"]), int.Parse(adresses["GPL"]));
            }

            // Memory test DELETED

            // MUpdate();
        }

        public void MUpdate()
        {
            dataGridView1.Rows.Clear();

            for (int i = 0; i < adresses.Count; i++)
                dataGridView1.Rows.Add(new object[] { adresses.ElementAt(i).Key, adresses.ElementAt(i).Value });
        }

        public Opt()
        {
            InitializeComponent();

            byte[] fontData = Properties.Resources.ThisFont;
            IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            uint dummy = 0;
            fonts.AddMemoryFont(fontPtr, Properties.Resources.ThisFont.Length);
            AddFontMemResourceEx(fontPtr, (uint)Properties.Resources.ThisFont.Length, IntPtr.Zero, ref dummy);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);

            myFont = new Font(fonts.Families[0], 16.0F);
        }

        private void Opt_Load(object sender, EventArgs e)
        {
            f.label2.Font = myFont;
            f.Show();
            this.Hide();
            this.Show();
            backgroundWorker1.WorkerSupportsCancellation = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            work = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            work = false;
            f.label2.Text = "";
            f.Refresh();
            SetDefolt();
            //MUpdate();
            backgroundWorker1.CancelAsync();
        }

        public void Do(string[] code)
        {
            bool data = false;
            for(int i = 0; i < code.Length && work; i++)
            {
                if (code[i] == ".data")
                    data = true;
                else if (code[i] == ".text")
                    data = false;
                else
                {
                    if (data)
                    {
                        string[] mm = SplitByFirst(code[i], ' ');
                        adresses.Add(mm[0], mm[1]);
                        //Console.WriteLine(mm[0] + "|" + mm[1]);
                    }
                    else if (!code[i].StartsWith("endif") && !code[i].StartsWith(";"))
                    {
                        string[] sl = code[i].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (sl[0] == "mov")
                        {
                            adresses[sl[2]] = adresses[sl[1]];
                            adresses[sl[1]] = "";
                        }
                        else if (sl[0] == "cop")
                        {
                            adresses[sl[2]] = adresses[sl[1]];
                        }
                        else if (sl[0] == "add")
                        {
                            adresses[sl[2]] += adresses[sl[1]];
                        }
                        else if (sl[0] == "if")
                        {
                            byte num = byte.Parse(sl[1]);
                            string value1 = adresses[sl[2]];
                            string oper = sl[3];
                            string value2 = adresses[sl[4]];
                            if (oper == "==")
                            {
                                if (value1 != value2)
                                {
                                    i = ISearch(code, i, "endif " + num) + 1;
                                }
                            }
                            else if (oper == "!=")
                            {
                                if (value1 == value2)
                                {
                                    i = ISearch(code, i, "endif " + num) + 1;
                                }
                            }
                            else if (oper == "?")
                            {
                                if (!value1.Contains(value2))
                                {
                                    i = ISearch(code, i, "endif " + num) + 1;
                                }
                            }
                        }
                        else if (sl[0] == "res")
                            SetDefolt();
                        AdressCheck();
                    }
                }
            }
            work = false;
            f.label2.Text = "";
            f.Refresh();
            SetDefolt();
            //MUpdate();
            backgroundWorker1.CancelAsync();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }


        public string[] SplitByFirst(string text, char sym)
        {
            string first = "";
            string second = "";
            if (text.Contains(sym))
            {
                int m = 0;
                for (int a = 0; a < text.Length; a++)
                {
                    if (text[a] == sym)
                    {
                        m = a;
                        break;
                    }
                }
                first = text.Substring(0, m);
                second = text.Substring(m + 1);
            }
            else
                return new string[] { text, "" };
            return new string[] { first, second };
        }

        public static int ISearch(string[] where, int startIndex, string text)
        {
            for (int ia = startIndex; ia < where.Length; ia++)
                if (where[ia] == text)
                    return ia;
            return -1;
        }

        char get(byte num)
        {
            if (num == 0)
                return ' ';
            if (num == 1)
                return '\n';
            return "**qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNMйцукенгшщзхъфывапролджэячсмитьбюёЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮЁ1234567890!@#$%^&*()_+-={}[];:'\"\\|/,.<>?`~"[num];
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Do(System.IO.File.ReadAllLines(openFileDialog1.FileName));
        }

        private void Opt_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}