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

namespace LaundryApp
{
    public partial class DashboardAdmin : Form
    {
        public DashboardAdmin()
        {
            InitializeComponent();
        }

        private void LoadPesananTerbaru()
        {
            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    p.id AS 'ID',
                    pel.nama AS 'Pelanggan',
                    l.nama_layanan AS 'Layanan',
                    p.total_kg AS 'Total KG',
                    CONCAT('Rp ', FORMAT(p.total_harga, 0)) AS 'Total Harga',
                    CASE 
                        WHEN p.diambil_pada IS NULL THEN 'Belum Diambil'
                        ELSE 'Sudah Diambil'
                    END AS 'Status',
                    DATE_FORMAT(p.dibuat_pada, '%d %M %Y') AS 'Tanggal'
                FROM pesanan p
                JOIN pelanggan pel ON p.pelanggan_id = pel.id
                JOIN layanan l ON p.layanan_id = l.id
                ORDER BY p.dibuat_pada DESC
                LIMIT 15;
            ";

                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    guna2DataGridView1.DataSource = dt;
                }

                // 🔥 ID disembunyiin di grid YANG BENER
                guna2DataGridView1.Columns["ID"].Visible = false;

                // 🔥 Styling
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
                guna2DataGridView1.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load pesanan terbaru: " + ex.Message);
            }
        }


        private void LoadPendapatan()
        {
            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    'Minggu Ini' AS Periode,
                    COUNT(*) AS 'Total Transaksi',
                    CONCAT('Rp ', FORMAT(SUM(total_harga), 0)) AS 'Pendapatan'
                FROM pesanan
                WHERE YEARWEEK(dibuat_pada) = YEARWEEK(NOW())
                UNION
                SELECT 
                    'Bulan Ini',
                    COUNT(*),
                    CONCAT('Rp ', FORMAT(SUM(total_harga), 0))
                FROM pesanan
                WHERE MONTH(dibuat_pada) = MONTH(NOW()) AND YEAR(dibuat_pada) = YEAR(NOW())
                UNION
                SELECT 
                    'Tahun Ini',
                    COUNT(*),
                    CONCAT('Rp ', FORMAT(SUM(total_harga), 0))
                FROM pesanan
                WHERE YEAR(dibuat_pada) = YEAR(NOW());
            ";

                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    guna2DataGridView3.DataSource = dt;
                }

                guna2DataGridView3.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                guna2DataGridView3.DefaultCellStyle.Font = new Font("Segoe UI", 11);
                guna2DataGridView3.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(100, 88, 255);
                guna2DataGridView3.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                guna2DataGridView3.EnableHeadersVisualStyles = false;
                guna2DataGridView3.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 255);
                guna2DataGridView3.DefaultCellStyle.SelectionBackColor = Color.FromArgb(180, 200, 255);
                guna2DataGridView3.DefaultCellStyle.SelectionForeColor = Color.Black;
                guna2DataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                guna2DataGridView3.RowHeadersVisible = false;

                guna2DataGridView3.RowTemplate.Height = 30;
                guna2DataGridView3.ColumnHeadersHeight = 35;
                guna2DataGridView3.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load pendapatan: " + ex.Message);
            }
        }


        private void LoadLaporan()
        {
            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT 'Total Order' AS Kategori, COUNT(*) AS Jumlah 
                FROM pesanan 
                UNION
                SELECT 'Belum Diambil', COUNT(*) 
                FROM pesanan 
                WHERE diambil_pada IS NULL
                UNION
                SELECT 'Sudah Diambil', COUNT(*) 
                  FROM pesanan 
                WHERE diambil_pada IS NOT NULL;

                ";

                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    guna2DataGridView2.DataSource = dt;
                }


                guna2DataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                guna2DataGridView2.DefaultCellStyle.Font = new Font("Segoe UI", 11);
                guna2DataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(100, 88, 255);
                guna2DataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                guna2DataGridView2.EnableHeadersVisualStyles = false;
                guna2DataGridView2.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 255);
                guna2DataGridView2.DefaultCellStyle.SelectionBackColor = Color.FromArgb(180, 200, 255);
                guna2DataGridView2.DefaultCellStyle.SelectionForeColor = Color.Black;
                guna2DataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                guna2DataGridView2.RowHeadersVisible = false;

                guna2DataGridView2.RowTemplate.Height = 30;
                guna2DataGridView2.ColumnHeadersHeight = 35;
                guna2DataGridView2.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load laporan: " + ex.Message);
            }
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            DashboardAdmin dashboard = new DashboardAdmin();
            dashboard.Show();
            this.Hide();
        }

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {
            DataKasir dataKasir = new DataKasir();
            dataKasir.Show();
            this.Hide();
        }

        private void guna2PictureBox4_Click(object sender, EventArgs e)
        {
            LayananAdmin layananAdmin = new LayananAdmin();
            layananAdmin.Show();
            this.Hide();
        }

        private void guna2PictureBox5_Click(object sender, EventArgs e)
        {
            ProfilAdmin profilAdmin = new ProfilAdmin();
            profilAdmin.Show();
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

        private void DashboardAdmin_Load(object sender, EventArgs e)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Left, screen.Top);
            this.Size = new Size(screen.Width, screen.Height);

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.PerformAutoScale();

            LoadPesananTerbaru();
            LoadPendapatan();
            LoadLaporan();
            LoadTotalPelanggan();
            LoadTotalPesananHariIni();
        }
        private void LoadTotalPelanggan()
        {
            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM pelanggan";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    int total = Convert.ToInt32(cmd.ExecuteScalar());

                    labelTotalPelanggan.Text = total.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal ngambil total pelanggan: " + ex.Message);
            }
        }

        private void LoadTotalPesananHariIni()
        {
            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();

                    string query = @"
                SELECT COUNT(*) 
                FROM pesanan 
                WHERE DATE(dibuat_pada) = CURDATE()
            ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    int total = Convert.ToInt32(cmd.ExecuteScalar());

                    labelTotalPesananHariIni.Text = total.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal ngambil total pesanan hari ini: " + ex.Message);
            }
        }



        // EVENT HANDLER UNTUK PESANAN TERBARU
        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        // EVENT HANDLER UNTUK PENDAPATAN
        private void guna2DataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        // EVENT HANDLER UNTUK LAPORAN
        private void guna2DataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void labelTotalPelanggan_Click(object sender, EventArgs e)
        {

        }

        private void labelTotalPesananHariIni_Click(object sender, EventArgs e)
        {

        }

    }
}