

using System.Diagnostics;
using System.Text;
using ZXing;

namespace BarcodeGenerator
{
    public partial class Form1 : Form
    {
        private const byte _numOfStr = 20;
        private const byte _strLen = 16;
        private const string _charRange = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        public Form1()
        {
            InitializeComponent();
        }

        private void GenerateBtn_Click(object sender, EventArgs e)
        {
            var strings = new List<string>(_numOfStr);
            Random rnd = new();
            StringBuilder tempSB = new();
            BarcodeWriterGeneric bw = new()
            {
                Format = BarcodeFormat.CODE_128
            };

            for (int i = 0; i < _numOfStr; ++i)
            {
                for (int j = 0; j < _strLen; ++j)
                {
                    tempSB.Append(_charRange[rnd.Next(_charRange.Length)]);
                }
                strings.Add(tempSB.ToString());
                tempSB.Clear();
            }

            //generating new list of objects containing anonymous type objects with 2 properties: string value and barcode image
            dataGridView1.DataSource = strings.Select(s => new { Value = s, Barcode = bw.WriteAsBitmap(s) }).ToList();

            //Debug - checking types of barcode image
            Debug.WriteLine(bw.WriteAsBitmap("123").GetType().ToString());
            Debug.WriteLine(dataGridView1.Columns[1].CellType.ToString());

        }
    }
}