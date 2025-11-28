using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static LaundryApp.Login;

namespace LaundryApp
{
    public partial class ProfilKasir : Form
    {
        public ProfilKasir()
        {
            InitializeComponent();
        }

        private void ProfilKaryawan_Load(object sender, EventArgs e)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Left, screen.Top);
            this.Size = new Size(screen.Width, screen.Height);

            // AUTO SCALE
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.PerformAutoScale();

            guna2TextBoxUsername.Text = UserSession.Username;
            guna2TextBoxNama.Text = UserSession.Nama;
            guna2TextBoxAlamat.Text = UserSession.Alamat;
            guna2TextBoxNoHP.Text = UserSession.NoHP;
            guna2TextBoxPassword.Text = UserSession.Password;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            EditProfilKasir editProfil= new EditProfilKasir();
            editProfil.Show();
            this.Hide();
        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {

        }
        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            DashboardKasir dashBoardKasir = new DashboardKasir();
            dashBoardKasir.Show();
            this.Hide();
        }
        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {
            DataPelanggan dataPelangganForm = new DataPelanggan();
            dataPelangganForm.Show();
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
            
        }

        private void guna2PictureBox5_Click(object sender, EventArgs e)
        {
            ProfilKasir profilKasir= new ProfilKasir();
            profilKasir.Show();
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
