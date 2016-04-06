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
    public partial class SuratJalan : Form
    {
        #region Member Variables
        StringFormat strFormat; //Used to format the grid rows.
        ArrayList arrColumnLefts = new ArrayList();//Used to save left coordinates of columns
        ArrayList arrColumnWidths = new ArrayList();//Used to save column widths
        int iCellHeight = 0; //Used to get/set the datagridview cell height
        int iTotalWidth = 0; //
        int iRow = 0;//Used as counter
        bool bFirstPage = false; //Used to check whether we are printing first page
        bool bNewPage = false;// Used to check whether we are printing a new page
        int iHeaderHeight = 0; //Used for the header height
        #endregion

        Model1Container md = new Model1Container();
        DataTable data = new DataTable();
        public SqlConnection conn;
        SqlDataAdapter da;
        DataSet ds;
        Menu mdi = new Menu();
        int kode;
        public SuratJalan()
        {
            InitializeComponent();
            this.conn = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|Bando.mdf;Integrated Security=True;User Instance=True");
        }

        #region Print Button Click Event
        /// <summary>
        /// Handles the print button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (textBox10.Text == "")
            {
                MessageBox.Show("Input Jarak Total", "Peringatan");
            }
            else
            {
                button4.Enabled = true;
                //Open the print dialog
                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = printDocument1;
                printDialog.UseEXDialog = true;

                //Get the document
                if (DialogResult.OK == printDialog.ShowDialog())
                {
                    printDocument1.DocumentName = "Surat Jalan : " + textBox1.Text;
                    printDocument1.Print();
                }

                //Open the print preview dialog
                //PrintPreviewDialog objPPdialog = new PrintPreviewDialog();
                //objPPdialog.Document = printDocument1;
                //objPPdialog.ShowDialog();
            }
            
        }
        #endregion

        #region Begin Print Event Handler
        /// <summary>
        /// Handles the begin print event of print document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printDocument1_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                strFormat = new StringFormat();
                strFormat.Alignment = StringAlignment.Near;
                strFormat.LineAlignment = StringAlignment.Center;
                strFormat.Trimming = StringTrimming.EllipsisCharacter;

                arrColumnLefts.Clear();
                arrColumnWidths.Clear();
                iCellHeight = 0;
                iRow = 0;
                bFirstPage = true;
                bNewPage = true;

                // Calculating Total Widths
                iTotalWidth = 0;
                foreach (DataGridViewColumn dgvGridCol in dataGridView1.Columns)
                {
                    iTotalWidth += dgvGridCol.Width;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Print Page Event
        /// <summary>
        /// Handles the print page event of print document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                //Set the left margin
                int iLeftMargin = e.MarginBounds.Left;
                //Set the top margin
                int iTopMargin = e.MarginBounds.Top;
                //Whether more pages have to print or not
                bool bMorePagesToPrint = false;
                int iTmpWidth = 0;             
                
                //For the first page to print set the cell width and header height
                if (bFirstPage)
                {
                    foreach (DataGridViewColumn GridCol in dataGridView1.Columns)
                    {
                        iTmpWidth = (int)(Math.Floor((double)((double)GridCol.Width /
                                       (double)iTotalWidth * (double)iTotalWidth *
                                       ((double)e.MarginBounds.Width / (double)iTotalWidth))));

                        iHeaderHeight = (int)(e.Graphics.MeasureString(GridCol.HeaderText,
                                    GridCol.InheritedStyle.Font, iTmpWidth).Height) + 11;

                        // Save width and height of headres
                        arrColumnLefts.Add(iLeftMargin);
                        arrColumnWidths.Add(iTmpWidth);
                        iLeftMargin += iTmpWidth;
                    }
                }
                //Loop till all the grid rows not get printed
                while (iRow <= dataGridView1.Rows.Count - 1)
                {
                    DataGridViewRow GridRow = dataGridView1.Rows[iRow];
                    //Set the cell height
                    iCellHeight = GridRow.Height + 5;
                    int iCount = 0;
                    //Check whether the current page settings allo more rows to print
                    if (iTopMargin + iCellHeight >= e.MarginBounds.Height + e.MarginBounds.Top)
                    {
                        bNewPage = true;
                        bFirstPage = false;
                        bMorePagesToPrint = true;
                        break;
                    }
                    else
                    {
                        if (bNewPage)
                        {
                            //Draw Header
                            e.Graphics.DrawString("Jarak Total : "+textBox10.Text, new Font(dataGridView1.Font, FontStyle.Bold),
                                    Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top -
                                    e.Graphics.MeasureString("Jarak Total : " + textBox10.Text, new Font(dataGridView1.Font,
                                    FontStyle.Bold), e.MarginBounds.Width).Height - 13);

                            String strDate = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToShortTimeString();
                            //Draw Date
                            e.Graphics.DrawString(strDate, new Font(dataGridView1.Font, FontStyle.Bold),
                                    Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width -
                                    e.Graphics.MeasureString(strDate, new Font(dataGridView1.Font,
                                    FontStyle.Bold), e.MarginBounds.Width).Width), e.MarginBounds.Top -
                                    e.Graphics.MeasureString("Jarak Total : " + textBox10.Text, new Font(new Font(dataGridView1.Font,
                                    FontStyle.Bold), FontStyle.Bold), e.MarginBounds.Width).Height - 13);

                            //Draw Columns                 
                            iTopMargin = e.MarginBounds.Top;
                            foreach (DataGridViewColumn GridCol in dataGridView1.Columns)
                            {
                                e.Graphics.FillRectangle(new SolidBrush(Color.LightGray),
                                    new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                    (int)arrColumnWidths[iCount], iHeaderHeight));

                                e.Graphics.DrawRectangle(Pens.Black,
                                    new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                    (int)arrColumnWidths[iCount], iHeaderHeight));

                                e.Graphics.DrawString(GridCol.HeaderText, GridCol.InheritedStyle.Font,
                                    new SolidBrush(GridCol.InheritedStyle.ForeColor),
                                    new RectangleF((int)arrColumnLefts[iCount], iTopMargin,
                                    (int)arrColumnWidths[iCount], iHeaderHeight), strFormat);
                                iCount++;
                            }
                            bNewPage = false;
                            iTopMargin += iHeaderHeight;
                        }
                        iCount = 0;
                        //Draw Columns Contents                
                        foreach (DataGridViewCell Cel in GridRow.Cells)
                        {
                            if (Cel.Value != null)
                            {                               
                                e.Graphics.DrawString(Cel.Value.ToString(), Cel.InheritedStyle.Font,
                                            new SolidBrush(Cel.InheritedStyle.ForeColor),
                                            new RectangleF((int)arrColumnLefts[iCount], (float)iTopMargin,
                                            (int)arrColumnWidths[iCount], (float)iCellHeight), strFormat);
                            }
                            //Drawing Cells Borders 
                            e.Graphics.DrawRectangle(Pens.Black, new Rectangle((int)arrColumnLefts[iCount],
                                    iTopMargin, (int)arrColumnWidths[iCount], iCellHeight));

                            iCount++;
                        }
                    }
                    iRow++;
                    iTopMargin += iCellHeight;                    
                }                

                //If more lines exist, print another page.
                if (bMorePagesToPrint)
                    e.HasMorePages = true;
                else
                    e.HasMorePages = false;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'bandoDataSet.dbSuratPengirimanBarang' table. You can move, or remove it, as needed.
            this.dbSuratPengirimanBarangTableAdapter.Fill(this.bandoDataSet.dbSuratPengirimanBarang);
            datasuratjalan();
            disable();
            //datasuratpengirimanbarang();
        }

        void generate()
        {
            try
            {
                var id = from a in md.dbSuratJalans select a;
                int b = id.Count();
                b++;
                if (b < 10) textBox1.Text = "SJ00" + b;
                else if (b < 10) textBox1.Text = "SJ0" + b;
                else textBox1.Text = "SJ" + b;
            }
            catch (Exception)
            {
                textBox1.Text = "SJ001";
            }
        }

        void datasuratjalan()
        {
            var isi = from dt in md.dbSuratJalans select new { dt.IdSuratJalan, dt.IdSuratPengirimanBarang, dt.Tanggal, dt.NamaSupir, dt.RutePengiriman, dt.JarakTotal };
            bindingSource1.DataSource = isi;
            dataGridView2.DataSource = bindingSource1;
        }

        void datasuratpengirimanbarang()
        {
            conn = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|Bando.mdf;Integrated Security=True;User Instance=True");
            conn.Open();
            da = new SqlDataAdapter("select * from dbSuratPengirimanBarang", conn);
            SqlCommandBuilder builder = new SqlCommandBuilder(da);
            ds = new BandoDataSet();
            da.Fill(ds, "dbSuratPengirimanBarang");
            dataGridView3.DataSource = ds.Tables["dbSuratPengirimanBarang"];
        }

        void disable()
        {
            textBox1.Enabled = false;
            comboBox1.Enabled = false;
            dateTimePicker1.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            textBox2.Enabled = false;
            textBox6.Enabled = false;
            textBox7.Enabled = false;
            textBox8.Enabled = false;
            textBox9.Enabled = false;
            textBox10.Enabled = false;
            dateTimePicker1.Enabled = false;
            button8.Visible = true;
            button7.Visible = true;
            button6.Visible = true;
            button5.Visible = false;
            button4.Visible = false;
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button9.Visible = false;
            button10.Visible = false;
            textBox4.Text = "";
            comboBox1.SelectedIndex = 0;
            textBox3.Text = "";
            textBox4.Text = "";
            dateTimePicker1.Text = "";
            textBox3.Text = "";
            btnPrint.Enabled = false;
        }

        void enable()
        {
            textBox1.Enabled = false;
            comboBox1.Enabled = true;
            dateTimePicker1.Enabled = true;
            textBox3.Enabled = true;
            textBox4.Enabled = false;
            textBox2.Enabled = true;
            textBox6.Enabled = true;
            textBox7.Enabled = true;
            textBox8.Enabled = true;
            textBox9.Enabled = true;
            textBox10.Enabled = true;
            dateTimePicker1.Enabled = true;
            button8.Visible = false;
            button7.Visible = false;
            button6.Visible = false;
            button4.Visible = true;
            button5.Visible = true;
            button1.Visible = false;
            button2.Visible = false;
            button9.Visible = true;
            button10.Visible = true;
            button3.Visible = true;
            btnPrint.Enabled = false;
        }

        void dis()
        {
            comboBox1.SelectedIndex = 0;
            dateTimePicker1.Text = "";
            textBox3.Enabled = false;
            textBox4.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals("") || comboBox1.Text.Equals("") || dateTimePicker1.Text.Equals("") || textBox3.Text.Equals("") || textBox4.Text.Equals(""))
            {
                MessageBox.Show("Silahkan Lengkapi Data", "Peringatan");
            }
            else if (dataGridView1.RowCount == 7)
            {
                btnPrint.Enabled = true;
                button4.Enabled = true;
                MessageBox.Show("Data Sudah Maksimal", "Peringatan");
            }
            else
            {
                string dua = comboBox1.Text;
                string tiga = dateTimePicker1.Text;
                string empat = textBox3.Text;
                string lima = textBox4.Text;
                string[] row = {dua, tiga, empat, lima };
                dataGridView1.Rows.Add(row);
                textBox2.Text = textBox1.Text;
                textBox6.Text = "";
                textBox7.Text = tiga;
                textBox8.Text = empat;
                textBox9.Text = "";
                dis();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            kode = 1;
            generate();
            enable();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            kode = 2;
            enable();
            comboBox1.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            dateTimePicker1.Enabled = false;
            textBox1.Enabled = false;
            button1.Visible = false;
            button2.Visible = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Choose data");
            }
            else
            {
                if (MessageBox.Show("Hapus?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    dbSuratJalan x = (from obj in md.dbSuratJalans where obj.IdSuratJalan == textBox1.Text select obj).First();
                    md.DeleteObject(x);
                    md.SaveChanges();
                    datasuratjalan();
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            disable();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            textBox2.Text = dataGridView2.CurrentRow.Cells[0].Value.ToString();
            textBox6.Text = dataGridView2.CurrentRow.Cells[1].Value.ToString();
            textBox7.Text = dataGridView2.CurrentRow.Cells[2].Value.ToString();
            textBox8.Text = dataGridView2.CurrentRow.Cells[3].Value.ToString();
            textBox9.Text = dataGridView2.CurrentRow.Cells[4].Value.ToString();
            textBox10.Text = dataGridView2.CurrentRow.Cells[5].Value.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells[0].RowIndex.Equals(null))
            {
                MessageBox.Show("Pilih Data Yang Akan Dihapus", "Peringatan");
            }
            else 
            {
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedCells[0].RowIndex);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (kode == 1)
            {
                if (string.IsNullOrEmpty(textBox2.Text))
                {
                    MessageBox.Show("ID Surat Jalan Tidak Boleh Kosong");
                }
                else if (string.IsNullOrEmpty(textBox6.Text))
                {
                    MessageBox.Show("ID SPB Tidak Boleh Kosong");
                }
                else if (string.IsNullOrEmpty(textBox7.Text))
                {
                    MessageBox.Show("Tanggal Tidak Boleh Kosong");
                }
                else if (string.IsNullOrEmpty(textBox8.Text))
                {
                    MessageBox.Show("Nama Supir Tidak Boleh Kosong");
                }
                else if (string.IsNullOrEmpty(textBox9.Text))
                {
                    MessageBox.Show("Rute Pengiriman Tidak Boleh Kosong");
                }
                else if (string.IsNullOrEmpty(textBox10.Text))
                {
                    MessageBox.Show("Jarak Total Tidak Boleh Kosong");
                }
                else
                {

                    dbSuratJalan me = new dbSuratJalan()
                    {
                        IdSuratJalan = textBox2.Text,
                        IdSuratPengirimanBarang = textBox6.Text,
                        Tanggal = textBox7.Text,
                        NamaSupir = textBox8.Text,
                        RutePengiriman = textBox9.Text,
                        JarakTotal = textBox10.Text
                    };
                    md.AddTodbSuratJalans(me);
                    md.SaveChanges();
                    MessageBox.Show("Saved");
                    datasuratjalan();
                    disable();
                    dis();
                }
            }

            else if (kode == 2)
            {
                dbSuratJalan x = (from plg in md.dbSuratJalans where plg.IdSuratJalan == textBox2.Text select plg).First();
                x.IdSuratPengirimanBarang = textBox6.Text;
                x.Tanggal = textBox7.Text;
                x.NamaSupir = textBox8.Text;
                x.RutePengiriman = textBox9.Text;
                x.JarakTotal = textBox10.Text;
                md.SaveChanges();
                MessageBox.Show("Data Telah Dirubah");
                datasuratjalan();
                disable();
                dis();
            }
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            textBox6.Text = dataGridView1.Rows[0].Cells[0].Value.ToString() + " " + dataGridView1.Rows[1].Cells[0].Value.ToString() + " " + dataGridView1.Rows[2].Cells[0].Value.ToString() + " " + dataGridView1.Rows[3].Cells[0].Value.ToString() + " " + dataGridView1.Rows[4].Cells[0].Value.ToString() + " " + dataGridView1.Rows[5].Cells[0].Value.ToString();
            textBox9.Text = dataGridView1.Rows[0].Cells[3].Value.ToString() + " - " + dataGridView1.Rows[1].Cells[3].Value.ToString() + " - " + dataGridView1.Rows[2].Cells[3].Value.ToString() + " - " + dataGridView1.Rows[3].Cells[3].Value.ToString() + " - " + dataGridView1.Rows[4].Cells[3].Value.ToString() + " - " + dataGridView1.Rows[5].Cells[3].Value.ToString();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("select * from dbSuratPengirimanBarang where IdSuratPengirimanBarang ='" + comboBox1.Text + "'", conn);
            this.conn.Open();
            SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                textBox4.Text = r.GetString(3);
                //datasuratpengirimanbarang();
            }
            r.Close();
            this.conn.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            textBox9.Text = "";
            textBox10.Text = "";
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            button10.Visible = true;
        }

        

        private void button9_Click(object sender, EventArgs e)
        {
            dbSuratPengirimanBarang x = (from obj in md.dbSuratPengirimanBarangs where obj.IdSuratPengirimanBarang == comboBox1.Text select obj).First();
            string a = x.IdSuratPengirimanBarang;
            string b = x.IdSuratPerintahKerja;
            string c = x.Tanggal;
            string d = x.NamaPelanggan;
            string f = x.Tujuan;
            string[] row = { a, b, c, d, f };
            if (dataGridView3.RowCount == 7)
            {
                button1.Visible = true;
                button2.Visible = true;
                button9.Visible = false;
                button10.Visible = false;
                MessageBox.Show("Data Sudah Maksimal", "Peringatan");
            }
            else
            {
                
                dataGridView3.Rows.Add(row);
                //dis();
            }
            
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (dataGridView3.SelectedCells[0].RowIndex.Equals(null))
            {
                MessageBox.Show("Pilih Data Yang Akan Dihapus", "Peringatan");
            }
            else
            {
                dataGridView3.Rows.RemoveAt(dataGridView1.SelectedCells[0].RowIndex);
                button10.Visible = false;
            }
        }
    }    
}