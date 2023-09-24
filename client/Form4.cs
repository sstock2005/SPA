using SPA;
using System;
using System.Windows.Forms;

namespace PasswordManager
{
    public partial class Form4 : Form
    {
        private Password pdata;
        public Form4(Password data)
        {
            InitializeComponent();
            pdata = data;
            textBox1.Text = data.url;
            textBox2.Text = data.useremail;
            textBox3.Text = Security.GetPassVal(data.password);
        }

        private void Form4_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form3.ActiveForm.Show();
            this.Close();
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                MessageBox.Show("Text Copied!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Clipboard.SetText(textBox.Text);
            }
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                MessageBox.Show("Text Copied!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Clipboard.SetText(textBox.Text);
            }
        }

        private void textBox3_Click(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                MessageBox.Show("Text Copied!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Clipboard.SetText(textBox.Text);
            }
        }
    }
}
