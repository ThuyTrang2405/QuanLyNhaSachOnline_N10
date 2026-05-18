
--Các bảng độc lập ko có khóa ngoại
CREATE DATABASE BookstoreDB;
GO

-- 2. Chỉ định SQL Server sử dụng Database vừa tạo
USE BookstoreDB;
GO

CREATE TABLE TheLoai (
    MaTL INT IDENTITY(1,1) PRIMARY KEY,
    TenTL NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE TacGia (
    MaTG INT IDENTITY(1,1) PRIMARY KEY,
    TenTG NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE NhaXuatBan (
    MaNXB INT IDENTITY(1,1) PRIMARY KEY,
    TenNXB NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE NguoiDung (
    MaND INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    GioiTinh NVARCHAR(10),
    Email VARCHAR(100) UNIQUE NOT NULL,
    Sdt VARCHAR(15),
    DiaChi NVARCHAR(255),
    TenND VARCHAR(50) UNIQUE NOT NULL,
    MatKhau VARCHAR(255) NOT NULL
);
GO

--Các bảng kế thừa và phụ thuộc

-- Bảng Khách Hàng (Kế thừa từ Người Dùng)
CREATE TABLE KhachHang (
    MaND INT PRIMARY KEY, -- Khóa chính là Khóa ngoại
    TrangThaiTK BIT DEFAULT 1, -- 1: Đang hoạt động, 0: Bị khóa
    FOREIGN KEY (MaND) REFERENCES NguoiDung(MaND)
);
GO

-- Bảng Người Quản Trị (Kế thừa từ Người Dùng)
CREATE TABLE NguoiQuanTri (
    MaND INT PRIMARY KEY,
    NgayVaoLam DATE DEFAULT GETDATE(),
    FOREIGN KEY (MaND) REFERENCES NguoiDung(MaND)
);
GO

-- Bảng Sách (Liên kết với Thể Loại, Tác Giả, NXB)
CREATE TABLE Sach (
    MaSach INT IDENTITY(1,1) PRIMARY KEY,
    TenSach NVARCHAR(255) NOT NULL,
    GiaGoc DECIMAL(18,2) NOT NULL,
    HinhAnh VARCHAR(500),
    MoTa NVARCHAR(MAX),
    SLTon INT DEFAULT 0,
    TrangThaiS BIT DEFAULT 1, -- 1: Còn sách, 0: Hết sách
    MaTL INT,
    MaTG INT,
    MaNXB INT,
    FOREIGN KEY (MaTL) REFERENCES TheLoai(MaTL),
    FOREIGN KEY (MaTG) REFERENCES TacGia(MaTG),
    FOREIGN KEY (MaNXB) REFERENCES NhaXuatBan(MaNXB)
);
GO

--Các bảng nghiệp vụ giỏ hàng, đơn hàng

-- Bảng Giỏ Hàng (Thể hiện mối quan hệ N-N giữa Khách Hàng và Sách)
CREATE TABLE GioHang (
    MaND INT,
    MaSach INT,
    TongSLSach INT DEFAULT 1,
    PRIMARY KEY (MaND, MaSach), -- Khóa chính tổ hợp
    FOREIGN KEY (MaND) REFERENCES KhachHang(MaND),
    FOREIGN KEY (MaSach) REFERENCES Sach(MaSach)
);
GO

-- Bảng Đơn Đặt Hàng
CREATE TABLE DonDatHang (
    MaDH INT IDENTITY(1,1) PRIMARY KEY,
    NgayDat DATETIME DEFAULT GETDATE(),
    TrangThaiDon INT DEFAULT 0, -- 0: Chờ xử lý, 1: Đang giao, 2: Hoàn thành, 3: Đã hủy
    MaVanDon VARCHAR(50),
    DiaChiGH NVARCHAR(255) NOT NULL,
    MaND INT,
    FOREIGN KEY (MaND) REFERENCES KhachHang(MaND)
);
GO

-- Bảng Chi Tiết Đơn Đặt Hàng (Mối quan hệ "Bao gồm" giữa Đơn Hàng và Sách)
CREATE TABLE ChiTietDonDatHang (
    MaDH INT,
    MaSach INT,
    SLSach INT NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL, -- Bắt buộc lưu đơn giá để tính tổng tiền
    PRIMARY KEY (MaDH, MaSach),
    FOREIGN KEY (MaDH) REFERENCES DonDatHang(MaDH),
    FOREIGN KEY (MaSach) REFERENCES Sach(MaSach)
);
GO

-- Bảng Hóa Đơn (Liên kết 1-1 với Đơn Đặt Hàng)
CREATE TABLE HoaDon (
    MaHD INT IDENTITY(1,1) PRIMARY KEY,
    NgayTao DATETIME DEFAULT GETDATE(),
    TrangThaiTT NVARCHAR(50),
    GhiChu NVARCHAR(255),
    MaDH INT UNIQUE NOT NULL, --Đảm bảo quan hệ 1-1
    FOREIGN KEY (MaDH) REFERENCES DonDatHang(MaDH)
);
GO