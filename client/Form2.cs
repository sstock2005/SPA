using SPA;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PasswordManager
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            label4.Visible = false;
            var passwords = Security.Passwords(User.username);
            if (passwords != null)
            {
                foreach (var pass in passwords)
                {
                    LinkLabel linkLabel = new LinkLabel();
                    linkLabel.TextAlign = ContentAlignment.MiddleCenter;
                    linkLabel.Text = pass.url;
                    linkLabel.Tag = pass;
                    linkLabel.LinkClicked += linkLabel1_LinkClicked;
                    flowLayoutPanel1.Controls.Add(linkLabel);
                }
            }
            else
            {
                label4.Visible = true;
            }
        }

        public void Update2()
        {
            label4.Visible = false;
            var passwords = Security.Passwords(User.username);
            if (passwords != null)
            {
                foreach (var pass in passwords)
                {
                    LinkLabel linkLabel = new LinkLabel();
                    linkLabel.TextAlign = ContentAlignment.MiddleCenter;
                    linkLabel.Text = pass.url;
                    linkLabel.Tag = pass;
                    linkLabel.LinkClicked += linkLabel1_LinkClicked;
                    flowLayoutPanel1.Controls.Add(linkLabel);
                }
            }
            else
            {
                label4.Visible = true;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel linkLabel = (LinkLabel)sender;
            if (linkLabel.Tag is Password data)
            {
                Form4 form = new Form4(data);
                form.ShowDialog();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 form = new Form3();
            form.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Coming Soon!", "SBA", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
