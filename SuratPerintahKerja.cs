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
    public partial class SuratPerintahKerja : Form
    {
        Model1Container md = new Model1Container();
        DataTable data = new DataTable();
        public SqlConnection conn;
        Menu mdi = new Menu();
        int kode;
        public SuratPerintahKerja()
        {
            InitializeComponent();
            this.conn = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|Bando.mdf;Integrated Security=True;User Instance=True");
        }

        private void SuratPerintahKerja_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'bandoDataSet.dbPemesanan' table. You can move, or remove it, as needed.
            this.dbPemesananTableAdapter.Fill(this.bandoDataSet.dbPemesanan);
            datasuratperintahkerja();
            disable();
            datapemesanan();
        }

        void datapemesanan()
        {
            var isi = from dt in md.dbPemesanans select new {dt.IdBarang, dt.NamaBarang, dt.JenisBarang};
            bindingSource2.DataSource = isi;
            dataGridView2.DataSource = bindingSource2;
        }

        void generate()
        {
            try
            {
                var id = from a in md.dbSuratPerintahKerjas select a;
                int b = id.Count();
                b++;
                if (b < 10) textBox1.Text = "SK00" + b;
                else if (b < 10) textBox1.Text = "SK0" + b;
                else textBox1.Text = "SK" + b;
            }
            catch (Exception)
            {
                textBox1.Text = "SK001";
            }
        }

        void disable()
        {
            textBox1.Enabled = false;
            comboBox1.Enabled = false;
            dateTimePicker1.Enabled = false;
            button1.Visible = true;
            button2.Visible = true;
            button3.Visible = true;
            button4.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            comboBox1.SelectedIndex = 0;
            dateTimePicker1.Text = "";
        }

        void enable()
        {
            textBox1.Enabled = false;
            comboBox1.Enabled = true;
            dateTimePicker1.Enabled = true;
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
        }

        void datasuratperintahkerja() 
        {
            var isi = from dt in md.dbSuratPerintahKerjas select new { dt.IdSuratPerintahKerja, dt.IdPemesanan, dt.Tanggal};
            bindingSource1.DataSource = isi;
            dataGridView1.DataSource = bindingSource1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var isi = from dt in md.dbPemesanans orderby dt.IdPemesanan.Contains(comboBox1.Text) select new { dt.IdBarang, dt.NamaBarang, dt.JenisBarang };
            bindingSource2.DataSource = isi;
            dataGridView2.DataSource = bindingSource2;
        }

        void namaadmin()
        { 
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
                    dbSuratPerintahKerja x = (from obj in md.dbSuratPerintahKerjas where obj.IdSuratPerintahKerja == textBox1.Text select obj).First();
                    md.DeleteObject(x);
                    md.SaveChanges();
                    datasuratperintahkerja();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (kode == 1)
            {
                if (comboBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("Pilih ID Pemesanan");
                }
                else if (string.IsNullOrEmpty(dateTimePicker1.Text))
                {
                    MessageBox.Show("Pilih Tanggal");
                }
                else
                {   
                    dbSuratPerintahKerja me = new dbSuratPerintahKerja()
                    {
                        IdSuratPerintahKerja = textBox1.Text,
                        IdPemesanan = comboBox1.Text,
                        Tanggal = dateTimePicker1.Text,
                    };
                    md.AddTodbSuratPerintahKerjas(me);
                    md.SaveChanges();
                    MessageBox.Show("Saved");
                    datasuratperintahkerja();
                    disable();
                }
            }

            else if (kode == 2)
            {
                dbSuratPerintahKerja x = (from plg in md.dbSuratPerintahKerjas where plg.IdSuratPerintahKerja == textBox1.Text select plg).First();
                x.IdSuratPerintahKerja = textBox1.Text;
                x.IdPemesanan = comboBox1.Text;
                x.Tanggal = dateTimePicker1.Text;
                md.SaveChanges();
                MessageBox.Show("Data Telah Dirubah");
                datasuratperintahkerja();
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
        }

        private void button6_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            dateTimePicker1.Text = null;
        }
    }
}
