using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace SV
{
    public partial class livescreen : Form
    {
        Socket sock;
        public string ID = "";
        public livescreen(Socket sck, string id)
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 4;
            sock = sck; ID = id;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > -1)
            {
                try
                {
                    byte[] senddata = Form1.MyDataPacker("SCREENLIVEOPEN", System.Text.Encoding.UTF8.GetBytes(comboBox1.SelectedItem.ToString()));
                    sock.BeginSend(senddata, 0, senddata.Length, SocketFlags.None, null, null);
                }
                catch (Exception) { }
                button1.Enabled = false;
                button2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button1.Enabled == false)
            {
                try
                {
                    byte[] senddata = Form1.MyDataPacker("SCREENLIVECLOSE", System.Text.Encoding.UTF8.GetBytes("ECHO"));
                    sock.BeginSend(senddata, 0, senddata.Length, SocketFlags.None, null, null);
                }
                catch (Exception) { }
                button2.Enabled = false;
                button1.Enabled = true;
            }
        }

        private void livescreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            button2.PerformClick();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (button1.Enabled == false)
            {
                try
                {
                    byte[] senddata = Form1.MyDataPacker("SCREENQUALITY", Encoding.UTF8.GetBytes(comboBox1.SelectedItem.ToString()));
                    sock.BeginSend(senddata, 0, senddata.Length, SocketFlags.None, null, null);
                }
                catch (Exception) { }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                pictureBox1.BackColor = Color.FromArgb(33, 33, 33);
                pictureBox1.ForeColor = Color.FromArgb(144, 144, 144);
                pictureBox1.Refresh();

                panel2.BackColor = Color.FromArgb(33, 33, 33);
                panel2.ForeColor = Color.FromArgb(144, 144, 144);
                pictureBox1.Refresh();

                foreach (Control cntr in panel2.Controls)
                {
                    cntr.BackColor = Color.FromArgb(33, 33, 33);
                    cntr.ForeColor = Color.FromArgb(144, 144, 144);
                    panel2.Refresh();

                }
            }
            else
            {
                panel2.BackColor = SystemColors.ControlLight;
                panel2.ForeColor = SystemColors.WindowText;

                pictureBox1.BackColor = SystemColors.ControlLight;
                pictureBox1.ForeColor = SystemColors.WindowText;
                pictureBox1.Refresh();

                button1.BackColor = SystemColors.ControlLight;
                button2.BackColor = SystemColors.ControlLight;

                button1.ForeColor = SystemColors.WindowText;
                button2.ForeColor = SystemColors.WindowText;

                checkBox1.BackColor = SystemColors.ControlLight;
                label1.BackColor = SystemColors.ControlLight;

                checkBox1.ForeColor = SystemColors.WindowText;
                label1.ForeColor = SystemColors.WindowText;

                panel2.Refresh();
            }
        }
    }
}
