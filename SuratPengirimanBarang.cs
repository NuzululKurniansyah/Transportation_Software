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
    public partial class SuratPengirimanBarang : Form
    {
        Model1Container md = new Model1Container();
        DataTable data = new DataTable();
        public SqlConnection conn;
        Menu mdi = new Menu();
        int kode;
        public SuratPengirimanBarang()
        {
            InitializeComponent();
            this.conn = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|Bando.mdf;Integrated Security=True;User Instance=True");
        }

        private void SuratPengirimanBarang_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'bandoDataSet.dbSuratPerintahKerja' table. You can move, or remove it, as needed.
            this.dbSuratPerintahKerjaTableAdapter.Fill(this.bandoDataSet.dbSuratPerintahKerja);
            datasuratpengirimanbarang();
            disable();
        }

        void generate()
        {
            try
            {
                var id = from a in md.dbSuratPengirimanBarangs select a;
                int b = id.Count();
                b++;
                if (b < 10) textBox1.Text = "SP00" + b;
                else if (b < 10) textBox1.Text = "SP0" + b;
                else textBox1.Text = "SP" + b;
            }
            catch (Exception)
            {
                textBox1.Text = "SP001";
            }
        }

        void disable()
        {
            textBox1.Enabled = false;
            comboBox1.Enabled = false;
            dateTimePicker1.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            button1.Visible = true;
            button2.Visible = true;
            button3.Visible = true;
            button4.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            dateTimePicker1.Text = "";
        }

        void enable()
        {
            textBox1.Enabled = false;
            comboBox1.Enabled = true;
            dateTimePicker1.Enabled = true;
            textBox3.Enabled = true;
            textBox4.Enabled = true;
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
        }

        void datasuratpengirimanbarang()
        {
            var isi = from dt in md.dbSuratPengirimanBarangs select new { dt.IdSuratPengirimanBarang, dt.IdSuratPerintahKerja, dt.Tanggal, dt.NamaPelanggan, dt.Tujuan };
            bindingSource1.DataSource = isi;
            dataGridView1.DataSource = bindingSource1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlCommand spk = new SqlCommand("select dbp.IdPelanggan, dbp.Nama, dbp.Alamat from dbPelanggan dbp, dbPemesanan dbps, dbSuratPerintahKerja dbspk where dbspk.IdSuratPerintahKerja ='" + comboBox1.Text + "' and dbps.IdPemesanan = dbspk.IdPemesanan and dbps.IdPelanggan = dbp.IdPelanggan", conn);
            this.conn.Open();
            SqlDataReader r = spk.ExecuteReader();
            while (r.Read())
            {
                textBox3.Text = r.GetString(1);
                textBox4.Text = r.GetString(2);
            }
            r.Close();
            this.conn.Close();
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
                    datasuratpengirimanbarang();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (kode == 1)
            {
                if (comboBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("Pilih ID Surat Perintah Kerja");
                }
                else if (string.IsNullOrEmpty(dateTimePicker1.Text))
                {
                    MessageBox.Show("Pilih Tanggal");
                }
                else if (string.IsNullOrEmpty(textBox3.Text))
                {
                    MessageBox.Show("Nama Pelanggan Tidal Boleh Kosong");
                }
                else if (string.IsNullOrEmpty(textBox4.Text))
                {
                    MessageBox.Show("Tujuan Tidak Boleh Kosong");
                }
                else
                {
                    dbSuratPengirimanBarang me = new dbSuratPengirimanBarang()
                    {
                        IdSuratPengirimanBarang = textBox1.Text,
                        IdSuratPerintahKerja = comboBox1.Text,
                        Tanggal = dateTimePicker1.Text,
                        NamaPelanggan = textBox3.Text,
                        Tujuan = textBox4.Text
                    };
                    md.AddTodbSuratPengirimanBarangs(me);
                    md.SaveChanges();
                    MessageBox.Show("Saved");
                    datasuratpengirimanbarang();
                    disable();
                }
            }

            else if (kode == 2)
            {
                dbSuratPengirimanBarang x = (from plg in md.dbSuratPengirimanBarangs where plg.IdSuratPengirimanBarang == textBox1.Text select plg).First();
                x.IdSuratPerintahKerja = comboBox1.Text;
                x.Tanggal = dateTimePicker1.Text;
                x.NamaPelanggan = textBox3.Text;
                x.NamaPelanggan = textBox4.Text;
                md.SaveChanges();
                MessageBox.Show("Data Telah Dirubah");
                datasuratpengirimanbarang();
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
            textBox3.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            textBox4.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            dateTimePicker1.Text = "";
        }
    }
}
