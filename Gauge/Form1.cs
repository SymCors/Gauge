using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;

namespace Gauge
{
    public partial class Form1 : Form
    {

        char[] spearator = { ' ' };
        Int16 count = 2;
        Int16 degree = 0, pressure = 0, rpm = 0;
        const string message = "There is no available port";
        const string caption = "Error";

        public Form1()
        {
            InitializeComponent();
            backgroundWorker1.WorkerSupportsCancellation = true;
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true)
                addIntoComboBox();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = portsBox.Text;
                serialPort1.BaudRate = 9600;
                serialPort1.Open();
                backgroundWorker1.RunWorkerAsync();
                button1.BackColor = Color.Green;
                button2.BackColor = Color.Green;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(message, caption,
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
            }
            catch (Exception ex) { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.WorkerSupportsCancellation == true)
            {
                backgroundWorker1.CancelAsync();
                serialPort1.Close();
                button1.BackColor = Color.FromArgb(0, 192, 192);
                button2.BackColor = Color.FromArgb(0, 192, 192);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                try
                {
                    string read = serialPort1.ReadLine();
                    string[] splitted = read.Split(spearator, count);

                    degree = Int16.Parse(splitted[0]);
                    pressure = Int16.Parse(splitted[1]);
                    rpm = Int16.Parse(splitted[2]);

                    label1.Text = degree.ToString() + " °C";
                    label2.Text = pressure.ToString() + " kPa";
                    label3.Text = rpm.ToString() + " Rpm";

                    if (degree > 100)
                        degree = 100;
                    if (pressure > 100)
                        pressure = 100;
                    if (rpm > 5000)
                        rpm = 5000;

                    aGauge1.Value = degree;
                    aGauge2.Value = pressure;
                    aGauge3.Value = rpm;
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }

        private void addIntoComboBox()
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string comport in ports)
                portsBox.Items.Add(comport);
            if (ports.Length != 0)
                portsBox.SelectedItem = ports[0];
        }
    }
}
