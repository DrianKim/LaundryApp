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
    public partial class DataPesanan : Form
    {
        DataTable dtPelanggan;
        public DataPesanan()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    p.id,
                    pel.nama AS pelanggan,
                    l.nama_layanan AS layanan,
                    p.total_kg,
                    p.total_harga,
                    p.tambahan,
                    p.metode_bayar,
                    p.dibuat_pada,
                    p.diambil_pada
                FROM pesanan p
                INNER JOIN pelanggan pel ON p.pelanggan_id = pel.id
                INNER JOIN layanan l ON p.layanan_id = l.id
                ORDER BY p.id DESC;
            ";

                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    dtPelanggan = new DataTable();
                    da.Fill(dtPelanggan);
                    guna2DataGridView1.DataSource = dtPelanggan;

                    // === Styling ===
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

                    guna2DataGridView1.RowTemplate.Height = 32;
                    guna2DataGridView1.ColumnHeadersHeight = 35;

                    // Header kolom
                    guna2DataGridView1.Columns["pelanggan"].HeaderText = "Nama Pelanggan";
                    guna2DataGridView1.Columns["layanan"].HeaderText = "Layanan";
                    guna2DataGridView1.Columns["total_kg"].HeaderText = "Total KG";
                    guna2DataGridView1.Columns["total_harga"].HeaderText = "Total Harga (Rp)";
                    guna2DataGridView1.Columns["tambahan"].HeaderText = "Tambahan";
                    guna2DataGridView1.Columns["metode_bayar"].HeaderText = "Metode Bayar";
                    guna2DataGridView1.Columns["dibuat_pada"].HeaderText = "Dibuat Pada";
                    guna2DataGridView1.Columns["diambil_pada"].HeaderText = "Diambil Pada";

                    // === SEMBUNYIKAN ID ===
                    if (guna2DataGridView1.Columns.Contains("id"))
                        guna2DataGridView1.Columns["id"].Visible = false;

                    // === Tambah kolom Aksi ===
                    if (!guna2DataGridView1.Columns.Contains("Aksi"))
                    {
                        DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
                        checkColumn.Name = "Aksi";
                        checkColumn.HeaderText = "Aksi";
                        checkColumn.Width = 60;
                        checkColumn.FalseValue = false;
                        checkColumn.TrueValue = true;
                        guna2DataGridView1.Columns.Add(checkColumn);
                    }

                    // Boleh dicentang, selain itu readonly
                    guna2DataGridView1.ReadOnly = false;
                    foreach (DataGridViewColumn col in guna2DataGridView1.Columns)
                    {
                        col.ReadOnly = col.Name != "Aksi";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error load data pesanan: " + ex.Message);
            }
        }

        private void DataPesanan_Load(object sender, EventArgs e)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Left, screen.Top);
            this.Size = new Size(screen.Width, screen.Height);

            // AUTO SCALE
            this.AutoScaleMode = AutoScaleMode.Dpi; // bisa juga Font
            this.PerformAutoScale();

            LoadData();
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            DashboardKasir dashBoardKaryawanForm = new DashboardKasir();
            dashBoardKaryawanForm.Show();
            this.Close();
        }

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {
            DataPelanggan dataPelangganForm = new DataPelanggan();
            dataPelangganForm.Show();
            this.Close();
        }

        private void guna2PictureBox3_Click(object sender, EventArgs e)
        {
            DataPesanan dataPesananForm = new DataPesanan();
            dataPesananForm.Show();
            this.Close();
        }

        private void guna2PictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox5_Click(object sender, EventArgs e)
        {
            ProfilKasir profilKaryawan = new ProfilKasir();
            profilKaryawan.Show();
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

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Pilih dulu satu pesanannya ya 😆");
                return;
            }

            bool cek = guna2DataGridView1.CurrentRow.Cells["Aksi"].Value != DBNull.Value
                       && guna2DataGridView1.CurrentRow.Cells["Aksi"].Value != null
                       && Convert.ToBoolean(guna2DataGridView1.CurrentRow.Cells["Aksi"].Value);

            if (!cek)
            {
                MessageBox.Show("Ceklis dulu kolom 'Aksi' sebelum konfirmasi ✅", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string namaPelanggan = guna2DataGridView1.CurrentRow.Cells["pelanggan"].Value.ToString();
            DateTime tanggalBuat = Convert.ToDateTime(guna2DataGridView1.CurrentRow.Cells["dibuat_pada"].Value);

            int pesananId = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["id"].Value);

            var confirm = MessageBox.Show(
                $"Pesanan atas nama: {namaPelanggan}\n" +
                $"Dibuat pada: {tanggalBuat:dd MMM yyyy - HH:mm}\n\n" +
                $"Yakin sudah diambil?",
                "Konfirmasi",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm == DialogResult.No) return;

            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE pesanan SET diambil_pada = NOW() WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", pesananId);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Mantap! Pesanan sudah di-set jadi 'Sudah Diambil' ✅", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal update: " + ex.Message);
            }
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (dtPelanggan == null) return;

            DataView dv = dtPelanggan.DefaultView;
            dv.RowFilter = $"pelanggan LIKE '%{guna2TextBox1.Text.Replace("'", "''")}%'";
        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2ButtonCetakStruk_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Pilih dulu pesanan mana yang mau dicetak struk 😆");
                return;
            }

            bool cek = guna2DataGridView1.CurrentRow.Cells["Aksi"].Value != DBNull.Value
                       && guna2DataGridView1.CurrentRow.Cells["Aksi"].Value != null
                       && Convert.ToBoolean(guna2DataGridView1.CurrentRow.Cells["Aksi"].Value);

            if (!cek)
            {
                MessageBox.Show("Ceklis dulu kolom 'Aksi' untuk cetak struk 😎", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["id"].Value);

            // Ambil semua data untuk dimasukin ke struk
            string pelanggan = guna2DataGridView1.CurrentRow.Cells["pelanggan"].Value.ToString();
            string layanan = guna2DataGridView1.CurrentRow.Cells["layanan"].Value.ToString();
            string tambahan = guna2DataGridView1.CurrentRow.Cells["tambahan"].Value.ToString();
            string metode = guna2DataGridView1.CurrentRow.Cells["metode_bayar"].Value.ToString();
            decimal totalKg = Convert.ToDecimal(guna2DataGridView1.CurrentRow.Cells["total_kg"].Value);
            decimal totalHarga = Convert.ToDecimal(guna2DataGridView1.CurrentRow.Cells["total_harga"].Value);
            DateTime dibuat = Convert.ToDateTime(guna2DataGridView1.CurrentRow.Cells["dibuat_pada"].Value);

            // Kirim ke form Struk
            var frm = new Struk(
                id, pelanggan, layanan, totalKg, totalHarga, tambahan, metode, dibuat
            );

            frm.ShowDialog();
        }
    }
}
