using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;

namespace BandoApplication
{
    public partial class FormRealisasiPengirimanHarian : Form
    {
        Model1Container md = new Model1Container();
        DataTable data = new DataTable();
        public SqlConnection conn;
        Menu mdi = new Menu();
        int kode;
        public FormRealisasiPengirimanHarian()
        {
            InitializeComponent();
            this.conn = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|Bando.mdf;Integrated Security=True;User Instance=True");
        }

        private void FormRealisasiPengirimanHarian_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'bandoDataSet.dbSuratJalan' table. You can move, or remove it, as needed.
            this.dbSuratJalanTableAdapter.Fill(this.bandoDataSet.dbSuratJalan);
            datafrph();
            disable();
        }

        void generate()
        {
            try
            {
                var id = from a in md.dbSuratPengirimanBarangs select a;
                int b = id.Count();
                b++;
                if (b < 10) textBox1.Text = "LH00" + b;
                else if (b < 10) textBox1.Text = "LH0" + b;
                else textBox1.Text = "LH" + b;
            }
            catch (Exception)
            {
                textBox1.Text = "LH001";
            }
        }

        void disable()
        {
            textBox1.Enabled = false;
            comboBox1.Enabled = false;
            dateTimePicker1.Enabled = false;
            textBox3.Enabled = false;
            comboBox2.Enabled = false;
            button1.Visible = true;
            button2.Visible = true;
            button3.Visible = true;
            button4.Visible = false;
            button5.Visible = false;
            textBox3.Text = "";
            comboBox2.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
            dateTimePicker1.Text = "";
        }

        void enable()
        {
            textBox1.Enabled = false;
            comboBox1.Enabled = true;
            dateTimePicker1.Enabled = true;
            textBox3.Enabled = true;
            comboBox2.Enabled = true;
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = true;
            button5.Visible = true;
        }

        void datafrph()
        {
            var isi = from dt in md.dbFormRealisasiPengirimanHarians select new { dt.IdFormRealisasiPengirimanHarian, dt.IdSuratJalan, dt.Tanggal, dt.Status, dt.Komentar };
            bindingSource1.DataSource = isi;
            dataGridView1.DataSource = bindingSource1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            kode = 1;
            generate();
            enable();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            kode = 2;
            enable();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Choose data");
            }
            else
            {
                if (MessageBox.Show("Hapus?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    dbFormRealisasiPengirimanHarian x = (from obj in md.dbFormRealisasiPengirimanHarians where obj.IdFormRealisasiPengirimanHarian == textBox1.Text select obj).First();
                    md.DeleteObject(x);
                    md.SaveChanges();
                    datafrph();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (kode == 1)
            {
                if (comboBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("Pilih ID Surat Jalan");
                }
                else if (comboBox2.SelectedIndex == -1)
                {
                    MessageBox.Show("Pilih Status");
                }
                else if (string.IsNullOrEmpty(dateTimePicker1.Text))
                {
                    MessageBox.Show("Pilih Tanggal");
                }
                else if (string.IsNullOrEmpty(textBox3.Text))
                {
                    MessageBox.Show("Komentar Tidak Boleh Kosong");
                }
                else
                {
                    dbFormRealisasiPengirimanHarian me = new dbFormRealisasiPengirimanHarian()
                    {
                        IdFormRealisasiPengirimanHarian = textBox1.Text,
                        IdSuratJalan = comboBox1.Text,
                        Tanggal = dateTimePicker1.Text,
                        Status = comboBox2.Text,
                        Komentar = textBox3.Text
                    };
                    md.AddTodbFormRealisasiPengirimanHarians(me);
                    md.SaveChanges();
                    MessageBox.Show("Saved");
                    datafrph();
                    disable();
                }
            }

            else if (kode == 2)
            {
                dbFormRealisasiPengirimanHarian x = (from plg in md.dbFormRealisasiPengirimanHarians where plg.IdFormRealisasiPengirimanHarian == textBox1.Text select plg).First();
                x.IdSuratJalan = comboBox1.Text;
                x.Tanggal = dateTimePicker1.Text;
                x.Status = comboBox2.Text;
                x.Komentar = textBox3.Text;
                md.SaveChanges();
                MessageBox.Show("Data Telah Dirubah");
                datafrph();
                disable();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            disable();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            textBox1.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            comboBox1.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            dateTimePicker1.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            comboBox2.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            textBox3.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            comboBox2.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
            dateTimePicker1.Text = "";
        }
    }
}
