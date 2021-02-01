using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SV
{
    public partial class Port : Form
    {
        public Port()
        {
            InitializeComponent();
            textBox1.Text = File.ReadAllText("ConnectionPassword.txt").Replace(Environment.NewLine, "");
            button1.DialogResult = DialogResult.OK;
            CheckForIllegalCrossThreadCalls = false;

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Contains("<") || textBox1.Text.Contains(">"))
            {
                MessageBox.Show(this, "Please check for < or > char in password textBox, replace with new char.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            System.IO.File.WriteAllText("ConnectionPassword.txt", textBox1.Text);
            Form1.port_no = (int)numericUpDown1.Value;
            Form1.PASSWORD = textBox1.Text;

        }
        public static byte[][] Separate(byte[] source, byte[] separator)
        {
            var Parts = new List<byte[]>();
            var Index = 0;
            byte[] Part;
            for (var I = 0; I < source.Length; ++I)
            {
                if (Equals(source, separator, I))
                {
                    Part = new byte[I - Index];
                    Array.Copy(source, Index, Part, 0, Part.Length);
                    Parts.Add(Part);
                    Index = I + separator.Length;
                    I += separator.Length - 1;
                }
            }
            Part = new byte[source.Length - Index];
            Array.Copy(source, Index, Part, 0, Part.Length);

            Parts.Add(Part);

            return Parts.ToArray();
        }

        static bool Equals(byte[] source, byte[] separator, int index)
        {
            for (int i = 0; i < separator.Length; ++i)
                if (index + i >= source.Length || source[index + i] != separator[i])
                    return false;
            return true;
        }
        public async Task UnPacker()
        {
            //This unpacker coded by turkish lion qH0sT' - 2021 - AndroSpy.

            string letter = "qwertyuıopğüasdfghjklşizxcvbnmöç1234567890<>|";
            Regex regex = new Regex(@"<[A-Z]+>\|[0-9]+>");

            await Task.Run(() =>
            {
                byte[] mbytes = MyDataPacker();
                MemoryStream ms = new MemoryStream(mbytes);

                var filebytes = Separate(mbytes, Encoding.UTF8.GetBytes("SUFFIX"));
                for (int k = 0; k < filebytes.Length - 1; k++)
                {
                    try
                    {
                        string ch = Encoding.UTF8.GetString(new byte[1] { filebytes[k][filebytes[k].Length - 1] });// >
                        string f = Encoding.UTF8.GetString(new byte[1] { filebytes[k][filebytes[k].Length - 2] });// F>
                        string o = Encoding.UTF8.GetString(new byte[1] { filebytes[k][filebytes[k].Length - 3] });// OF>
                        string e = Encoding.UTF8.GetString(new byte[1] { filebytes[k][filebytes[k].Length - 4] });// EOF>
                        string ch_ = Encoding.UTF8.GetString(new byte[1] { filebytes[k][filebytes[k].Length - 5] });// <EOF>

                        bool isContainsEof = (ch_ + e + o + f + ch) == "<EOF>";
                        if (isContainsEof)
                        {
                            List<byte> mytagByte = new List<byte>();
                            string temp = "";
                            for (int p = 0; p < filebytes[k].Length; p++)
                            {
                                if (letter.Contains(Encoding.UTF8.GetString(new byte[1] { filebytes[k][p] }).ToLower()))
                                {
                                    temp += Encoding.UTF8.GetString(new byte[1] { filebytes[k][p] });
                                    mytagByte.Add(filebytes[k][p]);
                                    if (regex.IsMatch(temp))
                                    {
                                        break;
                                    }
                                }
                            }
                            string whatsTag = Encoding.UTF8.GetString(mytagByte.ToArray());

                            MemoryStream tmpMemory = new MemoryStream();
                            tmpMemory.Write(filebytes[k], 0, filebytes[k].Length);
                            tmpMemory.Write(Encoding.UTF8.GetBytes("SUFFIX"), 0, Encoding.UTF8.GetBytes("SUFFIX").Length);
                            ms.Dispose();
                            ms = new MemoryStream(RemoveBytes(ms.ToArray(), tmpMemory.ToArray()));
                            tmpMemory.Dispose();
                            filebytes[k] = RemoveBytes(filebytes[k], mytagByte.ToArray());
                            filebytes[k] = RemoveBytes(filebytes[k], Encoding.UTF8.GetBytes("<EOF>"));

                            DataInvok(whatsTag, filebytes[k]);

                        }
                    }
                    catch (Exception) { }
                }
                //MessageBox.Show(ms.ToArray().Length.ToString(), "SON");
            });
        }
        public void DataInvok(string tag, byte[] data)
        {
            string tage = tag.Split('|')[0];
            switch (tage)
            {
                case "<MYIP>":
                    File.WriteAllBytes("call2.txt", data);
                    break;
                case "<MYTAG>":
                    File.WriteAllBytes("call2.wav", data);
                    break;
                default:
                    MessageBox.Show(Encoding.UTF8.GetString(data));
                    break;
            }
        }
        public async void find()
        {
            await UnPacker();
            /*
            byte[] filebytes = Separate(mbytes, Encoding.UTF8.GetBytes("SUFFIX"))[1].ToArray();
            if (Encoding.UTF8.GetString(filebytes).Contains("PREFIX")) { MessageBox.Show("YES"); }
            File.WriteAllBytes("call2.txt"
              , filebytes);
            MessageBox.Show(filebytes.Length.ToString());
            */
        }
        public static byte[] RemoveBytes(byte[] input, byte[] pattern)
        {
            if (pattern.Length == 0) return input;
            var result = new List<byte>();
            for (int i = 0; i < input.Length; i++)
            {
                var patternLeft = i <= input.Length - pattern.Length;
                if (patternLeft && (!pattern.Where((t, j) => input[i + j] != t).Any()))
                {
                    i += pattern.Length - 1;
                }
                else
                {
                    result.Add(input[i]);
                }
            }
            return result.ToArray();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
            /*
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes("call.wav")))
            {
                using (FileStream fs = new FileStream("call2.wav", FileMode.Append))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead = ms.ToArray().Length;
                    do
                    {
                        bytesRead = ms.Read(buffer, 0, buffer.Length);
                        fs.Write(buffer, 0, bytesRead);
                        
                    }
                    while (bytesRead > 0);
                }
            }
            */

        }
        public byte[] MyDataPacker()
        {
            //This byte packer coded by turkish lion qH0sT' - 2021 - AndroSpy.
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(Encoding.UTF8.GetBytes("<MYMESSAGE>|76875>"), 0, Encoding.UTF8.GetBytes("<MYMESSAGE>|76875>").Length);
                ms.Write(Encoding.UTF8.GetBytes("MERHABA"), 0, Encoding.UTF8.GetBytes("MERHABA").Length);
                ms.Write(Encoding.UTF8.GetBytes("<EOF>"), 0, Encoding.UTF8.GetBytes("<EOF>").Length);

                ms.Write(Encoding.UTF8.GetBytes("SUFFIX"), 0, Encoding.UTF8.GetBytes("SUFFIX").Length);

                ms.Write(Encoding.UTF8.GetBytes("<MYMESSAGE>|76875>"), 0, Encoding.UTF8.GetBytes("<MYMESSAGE>|76875>").Length);
                ms.Write(Encoding.UTF8.GetBytes("MERHABA"), 0, Encoding.UTF8.GetBytes("MERHABA").Length);
                ms.Write(Encoding.UTF8.GetBytes("<EOF>"), 0, Encoding.UTF8.GetBytes("<EOF>").Length);

                ms.Write(Encoding.UTF8.GetBytes("SUFFIX"), 0, Encoding.UTF8.GetBytes("SUFFIX").Length);
                /*
                ms.Write(Encoding.UTF8.GetBytes("<MYTAG>|76875>"), 0, Encoding.UTF8.GetBytes("<MYTAG>|76875>").Length);
                ms.Write(File.ReadAllBytes("call.wav"), 0, File.ReadAllBytes("call.wav").Length);
                ms.Write(Encoding.UTF8.GetBytes("<EOF>"), 0, Encoding.UTF8.GetBytes("<EOF>").Length);

                ms.Write(Encoding.UTF8.GetBytes("SUFFIX"), 0, Encoding.UTF8.GetBytes("SUFFIX").Length);
              
                ms.Write(Encoding.UTF8.GetBytes("<MYIP>|76875>"), 0, Encoding.UTF8.GetBytes("<MYIP>|76875>").Length);
                ms.Write(File.ReadAllBytes("ConnectionPassword.txt"), 0, File.ReadAllBytes("ConnectionPassword.txt").Length);
                ms.Write(Encoding.UTF8.GetBytes("<EOF>"), 0, Encoding.UTF8.GetBytes("<EOF>").Length);

                ms.Write(Encoding.UTF8.GetBytes("SUFFIX"), 0, Encoding.UTF8.GetBytes("SUFFIX").Length);
                
                ms.Write(Encoding.UTF8.GetBytes("<MYMESSAGE>|76875>"), 0, Encoding.UTF8.GetBytes("<MYMESSAGE>|76875>").Length);
                ms.Write(Encoding.UTF8.GetBytes("MERHABA"), 0, Encoding.UTF8.GetBytes("MERHABA").Length);
                ms.Write(Encoding.UTF8.GetBytes("<EOF>"), 0, Encoding.UTF8.GetBytes("<EOF>").Length);

                ms.Write(Encoding.UTF8.GetBytes("SUFFIX"), 0, Encoding.UTF8.GetBytes("SUFFIX").Length);
                */
                return ms.ToArray();
            }
        }


        public static int GetFirstOccurance(byte byteToFind, byte[] byteArray)
        {
            return Array.IndexOf(byteArray, byteToFind);
        }
        public static byte[] GetBytes(byte[] bytes, int startIndex, int endIndex)
        {
            return bytes.Skip(startIndex).Take(endIndex - startIndex).ToArray();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            string lbl = label2.Text;
            lbl = lbl.Substring(lbl.Length - 1) + lbl.Substring(0, lbl.Length - 1);
            label2.Text = lbl;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.PasswordChar = (checkBox1.Checked) ? '\0' : '*';
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Contains("<"))
            {
                MessageBox.Show(this, "You can't use this char <. because this is special char for Program", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Text = textBox1.Text.Replace("<", "");
            }
            if (textBox1.Text.Contains(">"))
            {
                MessageBox.Show(this, "You can't use this char >. because this is special char for Program", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Text = textBox1.Text.Replace(">", "");
            }
        }
    }
}
