using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BandoApplication
{
    public partial class Login : Form
    {
        Menu mdi = new Menu();
        Model1Container md = new Model1Container();
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var coba = md.dbUsers.Where(cobauser => cobauser.IdUser.Equals(textBox1.Text));
            var cobaa = md.dbUsers.Where(cobapass => cobapass.Password.Equals(textBox2.Text));
            if (textBox1.Text == "" || textBox2.Text == "")
            {   
                MessageBox.Show("All field must be filled", "peringatan");
            }
            else if (coba.Count() == 0 || cobaa.Count() == 0)
            {
                MessageBox.Show("Invalid data", "Warning");
                textBox2.Text = "";
            }

            else
            {
                if (textBox1.Text == "peg001")
                {
                    MessageBox.Show("Welcome ," + textBox1.Text.ToUpper());
                    Menu mc = new Menu();
                    mc.Show();
                    mc.suratPengirimanBarangToolStripMenuItem.Visible = false;
                    mc.suratJalanToolStripMenuItem.Visible = false;
                    mc.toolsToolStripMenuItem.Visible = false;
                    mc.laporanToolStripMenuItem.Visible = false;
                    this.Hide();
                }
                else if (textBox1.Text == "peg002")
                {
                    MessageBox.Show("Welcome ," + textBox1.Text.ToUpper());
                    Menu mc = new Menu();
                    mc.Show();
                    mc.suratPerintahKerjaToolStripMenuItem.Visible = false;
                    mc.suratJalanToolStripMenuItem.Visible = false;
                    mc.toolsToolStripMenuItem.Visible = false;
                    mc.laporanToolStripMenuItem.Visible = false;
                    this.Hide();
                }
                else if (textBox1.Text == "peg003")
                {
                    MessageBox.Show("Welcome ," + textBox1.Text.ToUpper());
                    Menu mc = new Menu();
                    mc.Show();
                    this.Hide();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
        }
    }
}
