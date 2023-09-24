using SPA;
using System;
using System.Windows.Forms;

namespace PasswordManager
{
    public partial class Password_Manager : Form
    {
        public Password_Manager()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length != 0)
            {
                if (textBox2.Text.Length != 0)
                {
                    if (textBox3.Text.Length != 0)
                    {
                        if (Security.Register(textBox1.Text, textBox2.Text, textBox3.Text))
                        {
                            Form2 form = new Form2();
                            this.Hide();
                            form.ShowDialog();
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Master Key cannot be empty!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Password cannot be empty!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Username cannot be empty!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length != 0)
            {
                if (textBox2.Text.Length != 0)
                {
                    if (textBox3.Text.Length != 0)
                    {
                        if (Security.Login(textBox1.Text, textBox2.Text, textBox3.Text))
                        {
                            Form2 form = new Form2();
                            this.Hide();
                            form.ShowDialog();
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Master Key cannot be empty!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Password cannot be empty!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Username cannot be empty!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
