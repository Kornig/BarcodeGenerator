

using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using ZXing;

namespace BarcodeGenerator
{
    public partial class Form1 : Form
    {
        private const byte _numOfStr = 20;
        private const byte _strLen = 16;
        private const string _charRange = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private const char _requestMsg = 'G';
        private const byte _incomingMsgLth = 15;

        private SerialPort _port1 = new()
        {
            BaudRate = 9600
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //combo box
            SerialPortsCmo.Items.AddRange(SerialPort.GetPortNames());
            SerialPortsCmo.SelectedIndex = -1;

            //get button
            GetBtn.Enabled = false;

            //data grid view columns
            dataGridView1.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewColumn()
                {
                    HeaderText = "Value",
                    Name = "Value",
                    CellTemplate = new DataGridViewTextBoxCell(),
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                },

                new DataGridViewImageColumn()
                {
                    HeaderText = "Barcode",
                    Name = "Barcode",
                    CellTemplate = new DataGridViewImageCell(),
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                }
            });
        }

        //Combo box selecting events
        private void SerialPortsCmo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //MessageBox.Show("Selection change commited!");

            GetBtn.Enabled = false;

            if (_port1.IsOpen)
            {
                try
                {
                    _port1.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Port closing error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            //changing name to currently selected
            _port1.PortName = SerialPortsCmo.Text;

            GetBtn.Enabled = true;
        }

        private void GetBtn_Click(object sender, EventArgs e)
        {
            string readResult;

            //opening port
            try
            {
                _port1.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Port opening error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //sending request
            try
            {
                _port1.Write(new char[] { _requestMsg }, 0, 1);
                Debug.WriteLine("FLAG1");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Data requesting error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //waiting for response
            try
            {
                while (_port1.BytesToRead < _incomingMsgLth) ;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Incoming data busy loop waiting error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                readResult = _port1.ReadExisting();
            }
            catch (Exception ex)
            {
                readResult = "";
                MessageBox.Show(ex.Message, "Reading from COM income buffer error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                _port1.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Port closing error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //
            //formatting received data, and putting it into dataGridView
            //

            BarcodeWriterGeneric bw = new()
            {
                Format = BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions()
                {
                    PureBarcode = true,
                    Height = 80,
                    Width = 400,
                    NoPadding = true
                }
            };

            if (readResult is not null && readResult != "")
                dataGridView1.Rows.Add(readResult, bw.WriteAsBitmap(readResult));
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            barcodePB.Image = dataGridView1.SelectedRows[0].Cells[1].Value as Bitmap;
        }
    }
}