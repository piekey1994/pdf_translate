using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace killenter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String oldText = textBox1.Text;
            String newText= oldText.Replace('\n', ' ').Replace('\r',' ');
            textBox2.Text = newText;
            Clipboard.SetText(textBox2.Text);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            IDataObject iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(DataFormats.Text))
            {
                textBox1.Text = (string)iData.GetData(DataFormats.UnicodeText);
                convertButton.PerformClick();
            }
            else
            {
                MessageBox.Show("目前剪贴板中数据不可转换为文本", "错误");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IDataObject iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(DataFormats.Text))
            {
                textBox1.Text = (string)iData.GetData(DataFormats.UnicodeText);
                button3.PerformClick();
            }
            else
            {
                MessageBox.Show("目前剪贴板中数据不可转换为文本", "错误");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = false;
            String oldText = textBox1.Text;
            String newText = oldText.Replace('\n', ' ').Replace('\r', ' ');
            String transText = Translate.translate(newText);
            textBox2.Text = transText;
            Clipboard.SetText(textBox2.Text);
            button2.Enabled = true;
            button3.Enabled = true;
        }
    }
}
