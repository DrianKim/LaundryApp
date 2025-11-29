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
    public partial class BuatPesananBaru : Form
    {
        // 🔹 Variable buat nyimpen data pelanggan (kalau dari TambahPelanggan)
        private int pelangganId;
        private string pelangganNama;

        // 🔹 Variable buat nyimpen user ID yang sedang login (kasir)
        private int loggedInUserId; // Lo harus set ini dari session/login

        // 1️⃣ Default (dipanggil dari menu kasir)
        public BuatPesananBaru(int userId)
        {
            InitializeComponent();
            this.loggedInUserId = userId;
        }

        // 2️⃣ Panggil dari halaman pelanggan (lengkap)
        public BuatPesananBaru(int userId, int idPelanggan, string nama, string alamat, string noHp)
            : this(userId)
        {
            this.pelangganId = idPelanggan;
            this.pelangganNama = nama;
        }

        // Tambahkan constructor ini
        public BuatPesananBaru(int userId, int idPelanggan, string nama)
            : this(userId, idPelanggan, nama, "", "")
        {
        }

        private void BuatPesananBaru_Load(object sender, EventArgs e)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Left, screen.Top);
            this.Size = new Size(screen.Width, screen.Height);

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.PerformAutoScale();

            if (pelangganId > 0)
            {
                txtNamaPelanggan.Text = pelangganNama;
                txtNamaPelanggan.ReadOnly = true;
                txtNamaPelanggan.BackColor = Color.FromArgb(240, 240, 240);
            }

            // 🔥 Daftarkan event SEBELUM load kategori
            cmbKategori.SelectedIndexChanged += cmbKategori_SelectedIndexChanged;

            // 🔥 Baru load kategori
            LoadKategori();

            // Kosongkan layanan dulu
            cmbLayanan.DataSource = null;

            numKuantitas.Minimum = 1;
            numKuantitas.Maximum = 999;
            numKuantitas.Value = 1;
        }

        private void cmbKategori_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbKategori.SelectedValue == null) return;
            if (cmbKategori.SelectedIndex == -1) return;

            int kategoriId;

            // Kadang SelectedValue itu DataRowView → perlu try parse
            if (int.TryParse(cmbKategori.SelectedValue.ToString(), out kategoriId))
            {
                LoadLayananByKategori(kategoriId);
            }
        }

        private void LoadKategori()
        {
            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query = "SELECT id, name, tipe FROM kategori ORDER BY name";

                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Buat kolom gabungan name + tipe
                    dt.Columns.Add("display_name", typeof(string));

                    foreach (DataRow row in dt.Rows)
                    {
                        row["display_name"] = $"{row["name"]} ({row["tipe"]})";
                    }

                    cmbKategori.DataSource = dt;
                    cmbKategori.DisplayMember = "display_name";
                    cmbKategori.ValueMember = "id";
                    cmbKategori.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load kategori: " + ex.Message);
            }
        }

        private void LoadLayananByKategori(int kategoriId)
        {
            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT id, nama_layanan, harga 
                FROM layanan
                WHERE kategori_id = @kat
                ORDER BY nama_layanan";

                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@kat", kategoriId);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbLayanan.DataSource = dt;
                    cmbLayanan.DisplayMember = "nama_layanan";
                    cmbLayanan.ValueMember = "id";
                    cmbLayanan.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load layanan: " + ex.Message);
            }
        }

        // 🔹 Method untuk load data layanan ke ComboBox
        private void LoadLayanan()
        {
            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query = "SELECT id, nama_layanan, harga FROM layanan ORDER BY nama_layanan";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbLayanan.DataSource = dt;
                    cmbLayanan.DisplayMember = "nama_layanan";
                    cmbLayanan.ValueMember = "id";
                    cmbLayanan.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error load layanan: " + ex.Message);
            }
        }

        // 🔹 Method untuk clear form
        private void ClearForm()
        {
            cmbLayanan.SelectedIndex = -1;
            numKuantitas.Value = 1;
            txtTambahanDetail.Clear();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            // Validasi
            if (pelangganId <= 0)
            {
                MessageBox.Show("Pilih pelanggan terlebih dahulu!");
                return;
            }
            if (cmbKategori.SelectedIndex == -1)
            {
                MessageBox.Show("Pilih kategori!");
                return;
            }
            if (cmbLayanan.SelectedIndex == -1)
            {
                MessageBox.Show("Pilih layanan!");
                return;
            }
            if (numKuantitas.Value < 1)
            {
                MessageBox.Show("Minimal 1!");
                return;
            }

            int layananId = Convert.ToInt32(cmbLayanan.SelectedValue);
            string tambahan = txtTambahanDetail.Text;
            int qty = Convert.ToInt32(numKuantitas.Value);

            // Ambil harga
            int harga = GetHargaLayanan(layananId);
            int totalHarga = harga * qty;

            // 🔹 MODAL PEMBAYARAN
            Pembayaran bayar = new Pembayaran(totalHarga);
            var result = bayar.ShowDialog(this);

            if (result != DialogResult.OK)
            {
                MessageBox.Show("Transaksi dibatalkan.");
                return;
            }

            // INSERT PESANAN
            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();

                    string query = @"
                INSERT INTO pesanan 
                (pelanggan_id, layanan_id, total_pesanan, total_harga, tambahan, metode_bayar, dibuat_pada, user_id)
                VALUES
                (@pelanggan_id, @layanan_id, @qty, @total, @tambahan, @metode, NOW(), @user)
            ";

                    long lastInsertId = 0;

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@pelanggan_id", pelangganId);
                        cmd.Parameters.AddWithValue("@layanan_id", layananId);
                        cmd.Parameters.AddWithValue("@qty", qty);
                        cmd.Parameters.AddWithValue("@total", totalHarga);
                        cmd.Parameters.AddWithValue("@tambahan", tambahan);
                        cmd.Parameters.AddWithValue("@metode", bayar.MetodeBayar);
                        cmd.Parameters.AddWithValue("@user", loggedInUserId);

                        cmd.ExecuteNonQuery();

                        // Ambil ID pesanan yang baru dibuat
                        lastInsertId = cmd.LastInsertedId;
                    }

                    MessageBox.Show("Pesanan berhasil disimpan!");

                    // --------- Perubahan utama ----------
                    // 1) Buka DataPesanan (form daftar) terlebih dahulu
                    // 2) Setelah DataPesanan tampil, panggil Struk sebagai modal dengan owner = DataPesanan
                    this.Hide();                         // sembunyikan form BuatPesananBaru
                    DataPesanan dp = new DataPesanan();
                    dp.Show();                           // tampilkan daftar pesanan

                    // Buat Struk dan tampilkan modal di atas DataPesanan
                    using (var struk = new Struk(lastInsertId))
                    {
                        struk.ShowDialog(dp);            // modal dengan parent dp
                    }

                    // setelah struk ditutup, kita bisa menutup form ini sepenuhnya
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private int GetHargaLayanan(int layananId)
        {
            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query = "SELECT harga FROM layanan WHERE id = @id LIMIT 1";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", layananId);

                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        private void ResetForm()
        {
            cmbLayanan.SelectedIndex = -1;
            numKuantitas.Value = 1;
            txtTambahanDetail.Clear();
        }

        private void guna2PictureBox4_Click(object sender, EventArgs e)
        {
            
        }

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {
            DataPelanggan dataPelanggan = new DataPelanggan();
            dataPelanggan.Show();
            this.Hide();
        }

        private void guna2PictureBox3_Click(object sender, EventArgs e)
        {
            DataPesanan dataPesananForm = new DataPesanan();
            dataPesananForm.Show();
            this.Hide();
        }

        private void guna2PictureBox5_Click(object sender, EventArgs e)
        {
            ProfilKasir profilKaryawan = new ProfilKasir();
            profilKaryawan.Show();
            this.Hide();
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            DashboardKasir dashboard = new DashboardKasir();
            dashboard.Show();
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

        private void txtTambahanDetail_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmbLayanan_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
