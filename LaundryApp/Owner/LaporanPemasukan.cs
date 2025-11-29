using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using System.IO;

namespace LaundryApp.Owner
{
    public partial class LaporanPemasukan : Form
    {
        public LaporanPemasukan()
        {
            InitializeComponent();
        }

        private void LoadPemasukan()
        {
            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();

                    string query = @"
                SELECT 
                    p.id,
                    u.nama AS pelanggan,
                    p.total_pesanan,
                    p.tambahan,
                    p.metode_bayar,
                    p.total_harga,
                    p.dibuat_pada
                FROM pesanan p
                LEFT JOIN users u ON p.user_id = u.id
                WHERE DATE(p.dibuat_pada) BETWEEN @mulai AND @selesai
                ORDER BY p.dibuat_pada DESC
            ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@mulai", DateTimePickerTanggalMulai.Value.Date);
                    cmd.Parameters.AddWithValue("@selesai", DateTimePickerTanggalSelesai.Value.Date);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    guna2DataGridView1.DataSource = dt;

                    // --- STYLING ---
                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    guna2DataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 11);

                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(100, 88, 255);
                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    guna2DataGridView1.EnableHeadersVisualStyles = false;

                    guna2DataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 255);
                    guna2DataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(180, 200, 255);
                    guna2DataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

                    guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    guna2DataGridView1.RowHeadersVisible = false;

                    guna2DataGridView1.RowTemplate.Height = 30;
                    guna2DataGridView1.ColumnHeadersHeight = 35;

                    guna2DataGridView1.Columns["id"].HeaderText = "ID";
                    guna2DataGridView1.Columns["pelanggan"].HeaderText = "User";
                    guna2DataGridView1.Columns["total_pesanan"].HeaderText = "Total Pesanan";
                    guna2DataGridView1.Columns["tambahan"].HeaderText = "Tambahan";
                    guna2DataGridView1.Columns["metode_bayar"].HeaderText = "Metode Bayar";
                    guna2DataGridView1.Columns["total_harga"].HeaderText = "Total Harga";
                    guna2DataGridView1.Columns["dibuat_pada"].HeaderText = "Tanggal";

                    // 🔥 Sembunyikan ID
                    guna2DataGridView1.Columns["id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error load pemasukan: " + ex.Message);
            }
        }

        private void DateTimePickerTanggalMulai_ValueChanged(object sender, EventArgs e)
        {
            DateTimePickerTanggalSelesai.MinDate = DateTimePickerTanggalMulai.Value;

            if (DateTimePickerTanggalSelesai.Value < DateTimePickerTanggalMulai.Value)
            {
                DateTimePickerTanggalSelesai.Value = DateTimePickerTanggalMulai.Value;
                MessageBox.Show("Tanggal selesai tidak boleh lebih awal dari tanggal mulai!");
            }

            LoadPemasukan();
        }

        private void DateTimePickerTanggalSelesai_ValueChanged(object sender, EventArgs e)
        {
            if (DateTimePickerTanggalSelesai.Value < DateTimePickerTanggalMulai.Value)
            {
                DateTimePickerTanggalSelesai.Value = DateTimePickerTanggalMulai.Value;
                MessageBox.Show("Tanggal selesai harus minimal sama dengan tanggal mulai!");
            }

            LoadPemasukan();
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            DashboardOwner dashboard = new DashboardOwner();
            dashboard.Show();
            this.Hide();
        }

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {
            DataUser dataPelanggan = new DataUser();
            dataPelanggan.Show();
            this.Hide();
        }

        private void guna2PictureBox3_Click(object sender, EventArgs e)
        {
            LaporanPemasukan laporanPemasukan = new LaporanPemasukan();
            laporanPemasukan.Show();
            this.Hide();
        }

        private void guna2PictureBox6_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show
            (
                "Apakah Anda Yakin Ingin logout?",
                "Konfirmasi Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm == DialogResult.Yes)
            {
                // Hapus session user
                Login.UserSession.Id = 0;
                Login.UserSession.Username = null;
                Login.UserSession.Nama = null;
                Login.UserSession.Email = null;
                Login.UserSession.Alamat = null;
                Login.UserSession.NoHP = null;
                Login.UserSession.Password = null;
                Login.UserSession.Role = null;

                // Buka form login lagi
                Login loginForm = new Login();
                loginForm.Show();

                // Tutup form sekarang (dashboard)
                this.Hide();
            }
        }

        private void LaporanPemasukan_Load(object sender, EventArgs e)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Left, screen.Top);
            this.Size = new Size(screen.Width, screen.Height);

            // AUTO SCALE
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.PerformAutoScale();

            LoadPemasukan();
        }

        private void guna2ButtonExport_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

        private void ExportToExcel()
        {
            if (guna2DataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Tidak ada data untuk diexport.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Excel File (*.xlsx)|*.xlsx";
            save.FileName = "Laporan_Pemasukan_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";

            if (save.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        DataTable dt = new DataTable();

                        // Buat kolom
                        foreach (DataGridViewColumn col in guna2DataGridView1.Columns)
                        {
                            if (col.Visible) // hanya kolom yang tampil
                                dt.Columns.Add(col.HeaderText);
                        }

                        // Isi baris
                        foreach (DataGridViewRow row in guna2DataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                DataRow dr = dt.NewRow();
                                int index = 0;

                                foreach (DataGridViewColumn col in guna2DataGridView1.Columns)
                                {
                                    if (col.Visible)
                                    {
                                        dr[index] = row.Cells[col.Name].Value;
                                        index++;
                                    }
                                }

                                dt.Rows.Add(dr);
                            }
                        }

                        // Tambah sheet
                        wb.Worksheets.Add(dt, "Laporan");

                        // Save file
                        wb.SaveAs(save.FileName);
                    }

                    MessageBox.Show("Export berhasil!\nFile tersimpan di:\n" + save.FileName,
                        "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal export: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
