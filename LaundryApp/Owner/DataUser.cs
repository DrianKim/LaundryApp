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

namespace LaundryApp.Owner
{
    public partial class DataUser : Form
    {
        DataTable dtUsers;

        public DataUser()
        {
            InitializeComponent();
        }

        private void DataUser_Load(object sender, EventArgs e)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Left, screen.Top);
            this.Size = new Size(screen.Width, screen.Height);

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.PerformAutoScale();

            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query = "SELECT id, nama, username, no_hp, alamat, email FROM users WHERE role = 'admin'";

                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    dtUsers = new DataTable();
                    da.Fill(dtUsers);

                    guna2DataGridView1.DataSource = dtUsers;

                    // 🔹 Styling
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
                    guna2DataGridView1.Columns["nama"].HeaderText = "Nama Admin";
                    guna2DataGridView1.Columns["username"].HeaderText = "Username";
                    guna2DataGridView1.Columns["no_hp"].HeaderText = "No HP";
                    guna2DataGridView1.Columns["alamat"].HeaderText = "Alamat";
                    guna2DataGridView1.Columns["email"].HeaderText = "Email";

                    // 🔥 SEMBUNYIKAN ID (sesuai revisi guru)
                    guna2DataGridView1.Columns["id"].Visible = false;

                    // 🔹 Kolom ceklist
                    if (!guna2DataGridView1.Columns.Contains("Aksi"))
                    {
                        DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
                        checkColumn.Name = "Aksi";
                        checkColumn.HeaderText = "Aksi";
                        checkColumn.Width = 60;
                        guna2DataGridView1.Columns.Add(checkColumn);
                    }

                    guna2DataGridView1.ReadOnly = false;

                    foreach (DataGridViewColumn col in guna2DataGridView1.Columns)
                    {
                        col.ReadOnly = col.Name != "Aksi";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error load user: " + ex.Message);
            }
        }

        // ===========================
        // 🔥 BUTTON TAMBAH ADMIN
        // ===========================
        private void guna2ButtonTambah_Click(object sender, EventArgs e)
        {
            using (var tambahForm = new TambahUser())
            {
                if (tambahForm.ShowDialog() == DialogResult.OK)
                {
                    LoadUsers();
                }
            }
        }

        // ===========================
        // 🔥 BUTTON EDIT ADMIN
        // ===========================
        private void guna2ButtonEdit_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.CurrentRow != null)
            {
                int id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["id"].Value);
                using (var editForm = new EditUser(id))
                {
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadUsers();
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih kasir dulu bro!");
            }
        }

        // ===========================
        // 🔥 BUTTON HAPUS ADMIN
        // ===========================
        private void guna2ButtonHapus_Click(object sender, EventArgs e)
        {
            try
            {
                int id = 0;
                string nama = "";
                bool found = false;

                foreach (DataGridViewRow row in guna2DataGridView1.Rows)
                {
                    bool isChecked = Convert.ToBoolean(row.Cells["Aksi"].Value);

                    if (isChecked)
                    {
                        id = Convert.ToInt32(row.Cells["id"].Value);
                        nama = row.Cells["nama"].Value.ToString();
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    MessageBox.Show("Centang user yang ingin dihapus!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    $"Yakin hapus admin:\n\n{nama}?",
                    "Konfirmasi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    DeleteUser(id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error hapus: " + ex.Message);
            }
        }

        private void DeleteUser(int id)
        {
            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string q = "DELETE FROM users WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(q, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("User berhasil dihapus!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUsers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error delete: " + ex.Message);
            }
        }

        // ===========================
        // 🔍 SEARCH ADMIN
        // ===========================
        private void guna2TextBoxSearch_TextChanged(object sender, EventArgs e)
        {
            if (dtUsers == null) return;

            DataView dv = dtUsers.DefaultView;
            dv.RowFilter = $"nama LIKE '%{guna2TextBoxSearch.Text}%' OR username LIKE '%{guna2TextBoxSearch.Text}%'";
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

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

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
    }
}
