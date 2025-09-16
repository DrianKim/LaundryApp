USE laundrydb;

-- users (kasir & owner)
CREATE TABLE users (
    id INT PRIMARY KEY IDENTITY,
    username VARCHAR(50) NOT NULL,
    password VARCHAR(100) NOT NULL,
    role VARCHAR(20) CHECK (role IN ('owner','kasir')) NOT NULL,
    nama VARCHAR(100),
    no_hp VARCHAR(20),
    alamat VARCHAR(100)
);

-- pelanggan
CREATE TABLE pelanggan (
    id INT PRIMARY KEY IDENTITY,
    nama VARCHAR(100) NOT NULL,
    alamat VARCHAR(200),
    no_hp VARCHAR(20)
);

-- layanan
CREATE TABLE layanan (
    id INT PRIMARY KEY IDENTITY,
    nama_layanan VARCHAR(50),
    harga_perkg DECIMAL(10,2),
    harga_satuan DECIMAL(10,2),
    estimasi_hari INT
);

-- transaksi
CREATE TABLE transaksi (
    id INT PRIMARY KEY IDENTITY,
    pelanggan_id INT,
    tanggal_masuk DATETIME DEFAULT GETDATE(),
    tanggal_selesai DATETIME,
    status VARCHAR(20) 
        CHECK (status IN ('proses', 'selesai', 'diambil')) 
        DEFAULT 'proses',
    kasir_id INT,
    FOREIGN KEY (pelanggan_id) REFERENCES pelanggan(id),
    FOREIGN KEY (kasir_id) REFERENCES users(id)
);

-- detail_transaksi
CREATE TABLE detail_transaksi (
    id INT PRIMARY KEY IDENTITY,
    transaksi_id INT,
    layanan_id INT,
    berat DECIMAL(10,2),
    qty INT,
    harga DECIMAL(10,2),
    subtotal DECIMAL(10,2),
    FOREIGN KEY (transaksi_id) REFERENCES transaksi(id),
    FOREIGN KEY (layanan_id) REFERENCES layanan(id)
);

-- pembayaran
CREATE TABLE pembayaran (
    id INT PRIMARY KEY IDENTITY,
    transaksi_id INT,
    metode VARCHAR(20) CHECK (metode IN ('tunai','non-tunai')),
    total DECIMAL(10,2),
    bayar DECIMAL(10,2),
    kembalian DECIMAL(10,2),
    FOREIGN KEY (transaksi_id) REFERENCES transaksi(id)
);
