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
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Login lg = new Login();
            lg.Show();
            this.Hide();
        }

        private void keluarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void suratPerintahKerjaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SuratPerintahKerja spk = new SuratPerintahKerja();
            spk.MdiParent = this;
            spk.Show();
        }

        private void suratPengirimanBarangToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SuratPengirimanBarang spb = new SuratPengirimanBarang();
            spb.MdiParent = this;
            spb.Show();
        }

        private void suratJalanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SuratJalan sj = new SuratJalan();
            sj.MdiParent = this;
            sj.Show();
        }

        private void laporanPengirimanBulananToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRealisasiPengirimanHarian frph = new FormRealisasiPengirimanHarian();
            frph.MdiParent = this;
            frph.Show();
        }

        private void laporanBulananToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LaporanPengirimanBulanan lpb = new LaporanPengirimanBulanan();
            lpb.MdiParent = this;
            lpb.Show();
        }

        private void Menu_Load(object sender, EventArgs e)
        {

        }
    }
}
