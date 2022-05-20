using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Chat_App
{
    public partial class Form1 : Form
    {
        TcpClient clientSocket = new TcpClient("192.168.1.103", 11000);
        StreamReader reader;
        StreamWriter writer;
        string readData = null;
        string randNum;
        Random rand = new Random();
        string name;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            randNum = Convert.ToString(rand.Next(1, 10));

            writer = new StreamWriter(clientSocket.GetStream());

            readData = "Chat App Server is Connected Successfully...";
            msg();

            name = textBox1.Text;

            string outStream = textBox1.Text + "$" + randNum + "$" + name;
            writer.WriteLine(outStream);
            writer.Flush();

            Thread ctThread = new Thread(getMessage);
            ctThread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            randNum = Convert.ToString(rand.Next(1, 10));

            writer = new StreamWriter(clientSocket.GetStream());

            string outStream = textBox3.Text + "$" + randNum;
            writer.WriteLine(outStream);
            writer.Flush();
        }

        private void getMessage()
        {
            reader = new StreamReader(clientSocket.GetStream());

            while (true)
            {
                string returndata = reader.ReadLine();
                readData = "" + returndata;
                msg();
            }
        }

        private void msg()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(msg));
            else
                textBox2.Text = textBox1.Text + Environment.NewLine + " >> " + readData;
        }
    }
}
