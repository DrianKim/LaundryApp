using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace LaundryApp.Admin
{
    public partial class KategoriAdmin : Form
    {
        public KategoriAdmin()
        {
            InitializeComponent();
        }

        private void Kategori_Load(object sender, EventArgs e)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Left, screen.Top);
            this.Size = new Size(screen.Width, screen.Height);

            // AUTO SCALE
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.PerformAutoScale();

            LoadDataKategori();
        }

        private void LoadDataKategori()
        {
            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT id, name, tipe, created_at, updated_at 
                        FROM kategori
                        ORDER BY id DESC";

                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dtKategori = new DataTable();
                    da.Fill(dtKategori);

                    guna2DataGridViewKategori.DataSource = dtKategori;

                    // Grid bisa diedit hanya kolom "Aksi"
                    guna2DataGridViewKategori.ReadOnly = false;

                    // Ubah nama header
                    guna2DataGridViewKategori.Columns["id"].HeaderText = "ID";
                    guna2DataGridViewKategori.Columns["name"].HeaderText = "Nama Kategori";
                    guna2DataGridViewKategori.Columns["tipe"].HeaderText = "Tipe";
                    guna2DataGridViewKategori.Columns["created_at"].HeaderText = "Dibuat Pada";
                    guna2DataGridViewKategori.Columns["updated_at"].HeaderText = "Diupdate Pada";

                    // Sembunyikan ID
                    guna2DataGridViewKategori.Columns["id"].Visible = false;

                    // Tambah kolom checkbox Aksi jika belum ada
                    if (!guna2DataGridViewKategori.Columns.Contains("Aksi"))
                    {
                        DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
                        checkColumn.Name = "Aksi";
                        checkColumn.HeaderText = "Aksi";
                        checkColumn.Width = 60;
                        checkColumn.TrueValue = true;
                        checkColumn.FalseValue = false;
                        checkColumn.ReadOnly = false;
                        guna2DataGridViewKategori.Columns.Add(checkColumn);
                    }

                    // Kunci semua kolom selain Aksi
                    foreach (DataGridViewColumn col in guna2DataGridViewKategori.Columns)
                    {
                        col.ReadOnly = col.Name != "Aksi";
                    }
                }

                // STYLING (biar mirip sama LayananAdmin)
                guna2DataGridViewKategori.ColumnHeadersDefaultCellStyle.Font =
                    new Font("Segoe UI", 10, FontStyle.Bold);

                guna2DataGridViewKategori.DefaultCellStyle.Font =
                    new Font("Segoe UI", 11);

                guna2DataGridViewKategori.ColumnHeadersDefaultCellStyle.BackColor =
                    Color.FromArgb(100, 88, 255);

                guna2DataGridViewKategori.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                guna2DataGridViewKategori.EnableHeadersVisualStyles = false;

                guna2DataGridViewKategori.AlternatingRowsDefaultCellStyle.BackColor =
                    Color.FromArgb(245, 245, 255);

                guna2DataGridViewKategori.DefaultCellStyle.SelectionBackColor =
                    Color.FromArgb(180, 200, 255);

                guna2DataGridViewKategori.DefaultCellStyle.SelectionForeColor = Color.Black;

                guna2DataGridViewKategori.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                guna2DataGridViewKategori.RowHeadersVisible = false;
                guna2DataGridViewKategori.RowTemplate.Height = 30;
                guna2DataGridViewKategori.ColumnHeadersHeight = 35;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data kategori: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2ButtonTambah_Click(object sender, EventArgs e)
        {
            using (var tambahForm = new TambahKategori())
            {
                if (tambahForm.ShowDialog() == DialogResult.OK)
                {
                    LoadDataKategori(); // refresh tabel
                }
            }
        }

        private void guna2ButtonEdit_Click(object sender, EventArgs e)
        {
            if (guna2DataGridViewKategori.CurrentRow != null)
            {
                int id = Convert.ToInt32(guna2DataGridViewKategori.CurrentRow.Cells["ID"].Value);

                using (var editForm = new EditKategori(id))
                {
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadDataKategori(); // reload data tabel setelah update
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih data kategori dulu bro 😎",
                                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void guna2ButtonHapus_Click(object sender, EventArgs e)
        {
            if (guna2DataGridViewKategori.CurrentRow != null)
            {
                int id = Convert.ToInt32(guna2DataGridViewKategori.CurrentRow.Cells["ID"].Value);
                string namaKategori = guna2DataGridViewKategori.CurrentRow.Cells["Name"].Value.ToString();

                var confirm = MessageBox.Show(
                    $"Yakin mau hapus kategori '{namaKategori}'?",
                    "Konfirmasi Hapus",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (confirm == DialogResult.Yes)
                {
                    try
                    {
                        using (var conn = new DatabaseHelper().GetConnection())
                        {
                            conn.Open();

                            // Cek apakah kategori sedang dipakai layanan
                            string checkQuery = "SELECT COUNT(*) FROM layanan WHERE kategori_id = @id";

                            using (var checkCmd = new MySqlCommand(checkQuery, conn))
                            {
                                checkCmd.Parameters.AddWithValue("@id", id);
                                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                                if (count > 0)
                                {
                                    MessageBox.Show(
                                        $"Kategori '{namaKategori}' tidak bisa dihapus karena masih dipakai oleh {count} layanan 😢",
                                        "Gagal Hapus",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error
                                    );
                                    return;
                                }
                            }

                            // Baru hapus kategori
                            string deleteQuery = "DELETE FROM kategori WHERE id = @id";

                            using (var cmd = new MySqlCommand(deleteQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@id", id);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        MessageBox.Show(
                            $"Kategori '{namaKategori}' berhasil dihapus ✅",
                            "Sukses",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                        LoadDataKategori(); // refresh tabel
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal hapus kategori: " + ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih kategori dulu sebelum hapus bro 😅",
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void guna2PictureBox3_Click(object sender, EventArgs e)
        {
            KategoriAdmin kategoriAdmin = new KategoriAdmin();
            kategoriAdmin.Show();
            this.Hide();
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

        private void guna2PictureBox5_Click(object sender, EventArgs e)
        {
            ProfilAdmin profilAdmin = new ProfilAdmin();
            profilAdmin.Show();
            this.Hide();
        }
    }
}
