USE laundrydb;
GO

-- User default (Owner & Kasir)
INSERT INTO dbo.users (username, password, role, nama, no_hp, alamat) VALUES
('admin', '123456', 'owner', 'Owner Laundry', '08123456789', 'Rawabadak'),
('kasir1', '123456', 'kasir', 'Kasir Pertama', '08122222222', 'Rawarawa');

-- Pelanggan contoh
INSERT INTO dbo.pelanggan (nama, alamat, no_hp) VALUES
('Budi Santoso', 'Jl. Merdeka No.1', '0811111111'),
('Siti Aminah', 'Jl. Sudirman No.5', '0822222222'),
('Agus Pratama', 'Jl. Gatot Subroto No.7', '0833333333');

-- Layanan default
INSERT INTO dbo.layanan (nama_layanan, harga_perkg, harga_satuan, estimasi_hari) VALUES
('Cuci Kiloan', 7000, NULL, 2),
('Cuci + Setrika', 10000, NULL, 3),
('Setrika Aja', 8000, NULL, 2),
('Cuci Satuan', NULL, 15000, 3);
