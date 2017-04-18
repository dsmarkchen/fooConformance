using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace FooConformance
{
    public partial class WaitForm : Form
    {
        private int _maximum = 10;
        private int _step = 1;
        private string _prompt = "Prompt";
        public WaitForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            
            _maximum = 60 * 2;
            _prompt = "";
        }
        public WaitForm(string prompt)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            _prompt = prompt;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if(progressBar1.Value != _maximum) {
                progressBar1.Value += _step;
                this.Text = "Wait " + (_maximum - progressBar1.Value) + " seconds.";
            }
            else {
                timer1.Stop();
                //this.DialogResult = System.Windows.Forms.DialogResult.OK;
                //this.Close();
                
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            progressBar1.Maximum = 10; // 60 * 2;
            _maximum = 10;
            this.Text = "Wait ...";
            timer1.Enabled = true;
            timer1.Start();
            timer1.Interval = 1000; // time per tick
            label1.Text = _prompt;

            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);
        }
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSE = 0xF060;


        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Pause")
            {
                button1.Text = "Resume";
                timer1.Stop();
                
            }
            else {
                button1.Text = "Pause";
                if (progressBar1.Value == _maximum) progressBar1.Value = 0;
                timer1.Start();
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void WaitForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
