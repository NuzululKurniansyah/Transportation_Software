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
using System.Windows.Forms.DataVisualization.Charting;

namespace BandoApplication
{
    public partial class LaporanPengirimanBulanan : Form
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

        public LaporanPengirimanBulanan()
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
            if (dataGridView1.Rows.Count==0)
            {
                MessageBox.Show("Tidak Ada Data","Peringatan");
            }
            else
            {
                //Open the print dialog
                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = printDocument1;
                printDialog.UseEXDialog = true;

                //Get the document
                if (DialogResult.OK == printDialog.ShowDialog())
                {
                    printDocument1.DocumentName = "Laporan Pengiriman Bulanan";
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
                            e.Graphics.DrawString("Periode : "+dateTimePicker2.Text+" Sampai "+dateTimePicker3.Text, new Font(dataGridView1.Font, FontStyle.Bold),
                                    Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top -
                                    e.Graphics.MeasureString("Periode : " + dateTimePicker2.Text + " Sampai " + dateTimePicker3.Text, new Font(dataGridView1.Font,
                                    FontStyle.Bold), e.MarginBounds.Width).Height - 13);

                            String strDate = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToShortTimeString();
                            //Draw Date
                            e.Graphics.DrawString(strDate, new Font(dataGridView1.Font, FontStyle.Bold),
                                    Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width -
                                    e.Graphics.MeasureString(strDate, new Font(dataGridView1.Font,
                                    FontStyle.Bold), e.MarginBounds.Width).Width), e.MarginBounds.Top -
                                    e.Graphics.MeasureString("Periode : " + dateTimePicker2.Text + " Sampai " + dateTimePicker3.Text, new Font(new Font(dataGridView1.Font,
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


        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'bandoDataSet.dbFormRealisasiPengirimanHarian' table. You can move, or remove it, as needed.
            this.dbFormRealisasiPengirimanHarianTableAdapter.Fill(this.bandoDataSet.dbFormRealisasiPengirimanHarian);
        }

        void laporan()
        {
            conn = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|Bando.mdf;Integrated Security=True;User Instance=True");
            conn.Open();
            da = new SqlDataAdapter("select * from dbFormRealisasiPengirimanHarian where Tanggal like '" + Int32.Parse(dateTimePicker2.Text.Substring(0,2).Replace("/","")).ToString() + "%' and Tanggal like '%" + dateTimePicker2.Text.Substring(6) + "'", conn);
            SqlCommandBuilder builder = new SqlCommandBuilder(da);
            ds = new BandoDataSet();
            da.Fill(ds, "dbFormRealisasiPengirimanHarian");
            dataGridView1.DataSource = ds.Tables["dbFormRealisasiPengirimanHarian"];
            this.chart1.Titles.Clear();
            this.chart1.Titles.Add("Periode " + Int32.Parse(dateTimePicker2.Text.Substring(0, 2).Replace("/", "")).ToString() + "-" + dateTimePicker2.Text.Substring(6));
            int[] status = { 0, 0 };
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if(dataGridView1.Rows[i].Cells[3].Value.ToString().Equals("Terlambat"))
                {
                    status[0]++;
                }
                else
                {
                    status[1]++;
                }
            }
            string[] xValues = { "Terlambat", "Tepat"};
            chart1.Series["Series1"].Points.DataBindXY(xValues, status);
            chart1.Series["Series1"].Points[0].Color = Color.Red;
            chart1.Series["Series1"].Points[1].Color = Color.BlueViolet;
            chart1.Series["Series1"].ChartType = SeriesChartType.Pie;
            chart1.Series["Series1"]["PieLabelStyle"] = "Disabled";
            chart1.Legends[0].Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Clear();
            laporan();
        }
    }    
}