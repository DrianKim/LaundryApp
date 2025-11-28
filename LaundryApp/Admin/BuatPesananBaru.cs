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

        // 🔹 Constructor default
        public BuatPesananBaru()
        {
            InitializeComponent();
        }

        // 🔹 Constructor dengan parameter (dipanggil dari TambahPelanggan)
        public BuatPesananBaru(int id, string nama, string alamat, string noHp) : this()
        {
            this.pelangganId = id;
            this.pelangganNama = nama;
        }

        private void BuatPesananBaru_Load(object sender, EventArgs e)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Left, screen.Top);
            this.Size = new Size(screen.Width, screen.Height);

            // AUTO SCALE
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.PerformAutoScale();

            loggedInUserId = 1; 

            if (pelangganId > 0)
            {
                txtNamaPelanggan.Text = pelangganNama;
                txtNamaPelanggan.ReadOnly = true; 
                txtNamaPelanggan.BackColor = Color.FromArgb(240, 240, 240); 
            }

            LoadLayanan();

            // 🔹 Set default value untuk kuantitas (total_kg)
            numKuantitas.Minimum = 1;
            numKuantitas.Maximum = 999;
            numKuantitas.Value = 1;
        }

        // 🔹 Method untuk load data layanan ke ComboBox
        private void LoadLayanan()
        {
            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query = "SELECT id, nama_layanan, harga_per_kg FROM layanan ORDER BY nama_layanan";
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

        // 🔹 Button SIMPAN - Insert pesanan ke database
        private void btnSimpan_Click(object sender, EventArgs e)
        {
            // 🔹 Validasi input - Cek apakah pelanggan ID udah ada
            if (pelangganId <= 0)
            {
                MessageBox.Show("Data pelanggan tidak valid!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbLayanan.SelectedIndex == -1)
            {
                MessageBox.Show("Pilih layanan terlebih dahulu!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbLayanan.Focus();
                return;
            }

            if (numKuantitas.Value < 1)
            {
                MessageBox.Show("Kuantitas minimal 1!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numKuantitas.Focus();
                return;
            }

            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();

                    // 🔹 Ambil harga_per_kg dari layanan yang dipilih
                    DataRowView row = (DataRowView)cmbLayanan.SelectedItem;
                    decimal hargaPerKg = Convert.ToDecimal(row["harga_per_kg"]);
                    decimal totalKg = numKuantitas.Value;
                    decimal totalHarga = hargaPerKg * totalKg;

                    // 🔹 INSERT pesanan ke database
                    // Tabel: id, user_id, pelanggan_id, layanan_id, total_kg, total_harga, metode_bayar, dibuat_pada, diambil_pada
                    string query = @"INSERT INTO pesanan 
                                   (user_id, pelanggan_id, layanan_id, total_kg, total_harga, metode_bayar, dibuat_pada) 
                                   VALUES (@user_id, @pelanggan_id, @layanan_id, @total_kg, @total_harga, @metode_bayar, NOW())";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user_id", loggedInUserId);
                    cmd.Parameters.AddWithValue("@pelanggan_id", pelangganId);
                    cmd.Parameters.AddWithValue("@layanan_id", cmbLayanan.SelectedValue);
                    cmd.Parameters.AddWithValue("@total_kg", totalKg);
                    cmd.Parameters.AddWithValue("@total_harga", totalHarga);

                    // 🔹 Metode bayar bisa dari txtTambahanDetail atau hardcode
                    string metodeBayar = string.IsNullOrWhiteSpace(txtTambahanDetail.Text) ? "Tunai" : txtTambahanDetail.Text.Trim();
                    cmd.Parameters.AddWithValue("@metode_bayar", metodeBayar);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"Pesanan berhasil disimpan!\n\nPelanggan: {pelangganNama}\nTotal: Rp {totalHarga:N0}", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Clear form
                        ClearForm();

                        // Redirect ke halaman lain (opsional)
                        // DataPesanan dataPesanan = new DataPesanan();
                        // dataPesanan.Show();
                        // this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Pesanan gagal disimpan!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat menyimpan pesanan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Data pelanggan tidak valid!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmbLayanan.SelectedIndex == -1)
            {
                MessageBox.Show("Pilih layanan terlebih dahulu!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbLayanan.Focus();
                return;
            }
            if (numKuantitas.Value < 1)
            {
                MessageBox.Show("Kuantitas minimal 1!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numKuantitas.Focus();
                return;
            }

            try
            {
                // ambil harga dan hitung total
                DataRowView row = (DataRowView)cmbLayanan.SelectedItem;
                decimal hargaPerKg = Convert.ToDecimal(row["harga_per_kg"]);
                decimal totalKg = numKuantitas.Value;
                decimal totalHarga = hargaPerKg * totalKg;

                // tampilkan modal pembayaran (modal centered karena ShowDialog(this) + StartPosition set)
                using (var payForm = new Pembayaran(totalHarga))
                {
                    var dr = payForm.ShowDialog(this);
                    if (dr != DialogResult.OK)
                    {
                        // user batal bayar
                        return;
                    }

                    decimal paid = payForm.PaidAmount;
                    decimal change = paid - totalHarga;

                    // setelah bayar valid, simpan pesanan ke DB
                    using (var conn = new DatabaseHelper().GetConnection())
                    {
                        conn.Open();

                        string query = @"INSERT INTO pesanan 
                                 (user_id, pelanggan_id, layanan_id, total_kg, total_harga, tambahan, metode_bayar, dibuat_pada) 
                                 VALUES (@user_id, @pelanggan_id, @layanan_id, @total_kg, @total_harga, @tambahan, @metode_bayar, NOW())";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@user_id", loggedInUserId);
                            cmd.Parameters.AddWithValue("@pelanggan_id", pelangganId);
                            cmd.Parameters.AddWithValue("@layanan_id", cmbLayanan.SelectedValue);
                            cmd.Parameters.AddWithValue("@total_kg", totalKg);
                            cmd.Parameters.AddWithValue("@total_harga", totalHarga);

                            // tambahan (opsional) — isi dari txtTambahanDetail
                            string tambahan = string.IsNullOrWhiteSpace(txtTambahanDetail.Text) ? "" : txtTambahanDetail.Text.Trim();
                            cmd.Parameters.AddWithValue("@tambahan", tambahan);

                            // metode bayar: otomatis "Tunai" kalau kosong / singkat
                            string metodeBayar = "tunai";
                            cmd.Parameters.AddWithValue("@metode_bayar", metodeBayar);

                            cmd.ExecuteNonQuery();

                            // ambil id pesanan yg baru saja disimpan (opsional)
                            long insertedId = cmd.LastInsertedId;
                        }
                    }

                    // kasih feedback & kembalian
                    MessageBox.Show(
                        $"Pesanan berhasil disimpan!\n\nPelanggan: {pelangganNama}\nTotal: Rp {totalHarga:N0}\nDibayar: Rp {paid:N0}\nKembalian: Rp {change:N0}",
                        "Sukses",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    // langsung pindah ke DataPesanan
                    DataPesanan dataPesananForm = new DataPesanan();
                    dataPesananForm.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat menyimpan pesanan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
    }
}
