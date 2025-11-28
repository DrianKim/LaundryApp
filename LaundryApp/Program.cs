using LaundryApp.Owner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaundryApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Login());
            Application.Run(new DashboardOwner());
            //Application.Run(new DashboardKasir());
            //Application.Run(new DashboardAdmin());
            //Application.Run(new DataPelanggan());
            //Application.Run(new DataPesanan());
            //Application.Run(new TambahPelanggan());
            //Application.Run(new DataKasir());
            //Application.Run(new ProfilAdmin());
            //Application.Run(new LayananAdmin());
            //Application.Run(new TambahLayanan());
            //Application.Run(new Coba());
        }
    }
}
