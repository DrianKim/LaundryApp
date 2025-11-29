using LaundryApp.Admin;
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
    public partial class DataKasir : Form
    {
        public DataKasir()
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
                    string query = "SELECT id, nama, username, email, alamat, no_hp FROM users WHERE role='karyawan'";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    guna2DataGridView1.DataSource = dt;

                    // SEMBUNYIKAN KOLOM ID SESUAI REVISI GURU
                    guna2DataGridView1.Columns["id"].Visible = false;

                    // Biar grid bisa dibuka buat atur readOnly per kolom
                    guna2DataGridView1.ReadOnly = false;

                    // Styling
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

                    guna2DataGridView1.Columns["nama"].HeaderText = "Nama Kasir";
                    guna2DataGridView1.Columns["username"].HeaderText = "Username";
                    guna2DataGridView1.Columns["email"].HeaderText = "Email";
                    guna2DataGridView1.Columns["alamat"].HeaderText = "Alamat";
                    guna2DataGridView1.Columns["no_hp"].HeaderText = "No HP";

                    // Tambah kolom checkbox kalo belum ada
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

                    // Kunci semua kolom kecuali Aksi
                    foreach (DataGridViewColumn col in guna2DataGridView1.Columns)
                    {
                        col.ReadOnly = col.Name != "Aksi";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error load data: " + ex.Message);
            }
        }


        private void DataKasir_Load(object sender, EventArgs e)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Left, screen.Top);
            this.Size = new Size(screen.Width, screen.Height);

            // AUTO SCALE
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.PerformAutoScale();

            LoadData();
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void guna2ButtonTambah_Click(object sender, EventArgs e)
        {
            using (var tambahForm = new TambahKasir())
            {
                if (tambahForm.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void guna2ButtonEdit_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.CurrentRow != null)
            {
                int id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["id"].Value);
                using (var editForm = new EditKasir(id))
                {
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadData(); 
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih kasir dulu bro!");
            }
        }

        private void guna2ButtonHapus_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.CurrentRow != null)
            {
                // Ambil ID dan Nama kasir dari DataGridView
                int id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["id"].Value);
                string namaKasir = guna2DataGridView1.CurrentRow.Cells["nama"].Value.ToString();

                // Konfirmasi dengan nama kasir
                var confirm = MessageBox.Show($"Yakin mau hapus kasir dengan nama '{namaKasir}'?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    try
                    {
                        using (var conn = new DatabaseHelper().GetConnection())
                        {
                            conn.Open();
                            string query = "DELETE FROM users WHERE id=@id";
                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Kasir berhasil dihapus!");
                        LoadData(); // refresh grid
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih kasir dulu bro!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void guna2PictureBox3_Click(object sender, EventArgs e)
        {
            DataPesanan dataPesananForm = new DataPesanan();
            dataPesananForm.Show();
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

        private void guna2Panel4_Paint(object sender, PaintEventArgs e)
        {

        }
        private void guna2Panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox3_Click_1(object sender, EventArgs e)
        {
            KategoriAdmin kategoriAdmin = new KategoriAdmin();
            kategoriAdmin.Show();
            this.Hide();
        }
    }
}
