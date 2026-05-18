using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaSachAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixForeignKey_DonDatHang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    MaND = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Sdt = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TenND = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NguoiDun__2725D7246BA112F9", x => x.MaND);
                });

            migrationBuilder.CreateTable(
                name: "NhaXuatBan",
                columns: table => new
                {
                    MaNXB = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenNXB = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NhaXuatB__3A19482C8C8E520E", x => x.MaNXB);
                });

            migrationBuilder.CreateTable(
                name: "TacGia",
                columns: table => new
                {
                    MaTG = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTG = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TacGia__272500740870C41F", x => x.MaTG);
                });

            migrationBuilder.CreateTable(
                name: "TheLoai",
                columns: table => new
                {
                    MaTL = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTL = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TheLoai__272500717CF3806F", x => x.MaTL);
                });

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    MaND = table.Column<int>(type: "int", nullable: false),
                    TrangThaiTK = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KhachHan__2725D7248678DFF4", x => x.MaND);
                    table.ForeignKey(
                        name: "FK__KhachHang__MaND__2F10007B",
                        column: x => x.MaND,
                        principalTable: "NguoiDung",
                        principalColumn: "MaND");
                });

            migrationBuilder.CreateTable(
                name: "NguoiQuanTri",
                columns: table => new
                {
                    MaND = table.Column<int>(type: "int", nullable: false),
                    NgayVaoLam = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NguoiQua__2725D7248A7FF95D", x => x.MaND);
                    table.ForeignKey(
                        name: "FK__NguoiQuanT__MaND__32E0915F",
                        column: x => x.MaND,
                        principalTable: "NguoiDung",
                        principalColumn: "MaND");
                });

            migrationBuilder.CreateTable(
                name: "Sach",
                columns: table => new
                {
                    MaSach = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenSach = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    GiaGoc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HinhAnh = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SLTon = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    TrangThaiS = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    MaTL = table.Column<int>(type: "int", nullable: true),
                    MaTG = table.Column<int>(type: "int", nullable: true),
                    MaNXB = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Sach__B235742DD46005CB", x => x.MaSach);
                    table.ForeignKey(
                        name: "FK__Sach__MaNXB__398D8EEE",
                        column: x => x.MaNXB,
                        principalTable: "NhaXuatBan",
                        principalColumn: "MaNXB");
                    table.ForeignKey(
                        name: "FK__Sach__MaTG__38996AB5",
                        column: x => x.MaTG,
                        principalTable: "TacGia",
                        principalColumn: "MaTG");
                    table.ForeignKey(
                        name: "FK__Sach__MaTL__37A5467C",
                        column: x => x.MaTL,
                        principalTable: "TheLoai",
                        principalColumn: "MaTL");
                });

            migrationBuilder.CreateTable(
                name: "DonDatHang",
                columns: table => new
                {
                    MaDH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayDat = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    TrangThaiDon = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    MaVanDon = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    DiaChiGH = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MaND = table.Column<int>(type: "int", nullable: true),
                    MaNguoiDuyet = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DonDatHa__2725866111C3ACAF", x => x.MaDH);
                    table.ForeignKey(
                        name: "FK_DonDatHang_NguoiQuanTri_MaNguoiDuyet",
                        column: x => x.MaNguoiDuyet,
                        principalTable: "NguoiQuanTri",
                        principalColumn: "MaND");
                    table.ForeignKey(
                        name: "FK__DonDatHang__MaND__4316F928",
                        column: x => x.MaND,
                        principalTable: "KhachHang",
                        principalColumn: "MaND");
                });

            migrationBuilder.CreateTable(
                name: "GioHang",
                columns: table => new
                {
                    MaND = table.Column<int>(type: "int", nullable: false),
                    MaSach = table.Column<int>(type: "int", nullable: false),
                    TongSLSach = table.Column<int>(type: "int", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GioHang__EC068066B8D4412E", x => new { x.MaND, x.MaSach });
                    table.ForeignKey(
                        name: "FK__GioHang__MaND__3D5E1FD2",
                        column: x => x.MaND,
                        principalTable: "KhachHang",
                        principalColumn: "MaND");
                    table.ForeignKey(
                        name: "FK__GioHang__MaSach__3E52440B",
                        column: x => x.MaSach,
                        principalTable: "Sach",
                        principalColumn: "MaSach");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonDatHang",
                columns: table => new
                {
                    MaDH = table.Column<int>(type: "int", nullable: false),
                    MaSach = table.Column<int>(type: "int", nullable: false),
                    SLSach = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietD__EC06D12342A4BA5F", x => new { x.MaDH, x.MaSach });
                    table.ForeignKey(
                        name: "FK__ChiTietDo__MaSac__46E78A0C",
                        column: x => x.MaSach,
                        principalTable: "Sach",
                        principalColumn: "MaSach");
                    table.ForeignKey(
                        name: "FK__ChiTietDon__MaDH__45F365D3",
                        column: x => x.MaDH,
                        principalTable: "DonDatHang",
                        principalColumn: "MaDH");
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    MaHD = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayTao = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    TrangThaiTT = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MaDH = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HoaDon__2725A6E03D1FDACB", x => x.MaHD);
                    table.ForeignKey(
                        name: "FK__HoaDon__MaDH__4BAC3F29",
                        column: x => x.MaDH,
                        principalTable: "DonDatHang",
                        principalColumn: "MaDH");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonDatHang_MaSach",
                table: "ChiTietDonDatHang",
                column: "MaSach");

            migrationBuilder.CreateIndex(
                name: "IX_DonDatHang_MaND",
                table: "DonDatHang",
                column: "MaND");

            migrationBuilder.CreateIndex(
                name: "IX_DonDatHang_MaNguoiDuyet",
                table: "DonDatHang",
                column: "MaNguoiDuyet");

            migrationBuilder.CreateIndex(
                name: "IX_GioHang_MaSach",
                table: "GioHang",
                column: "MaSach");

            migrationBuilder.CreateIndex(
                name: "UQ__HoaDon__27258660682DFF8C",
                table: "HoaDon",
                column: "MaDH",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__NguoiDun__4CF9B49A554A5111",
                table: "NguoiDung",
                column: "TenND",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__NguoiDun__A9D10534E8753DD0",
                table: "NguoiDung",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sach_MaNXB",
                table: "Sach",
                column: "MaNXB");

            migrationBuilder.CreateIndex(
                name: "IX_Sach_MaTG",
                table: "Sach",
                column: "MaTG");

            migrationBuilder.CreateIndex(
                name: "IX_Sach_MaTL",
                table: "Sach",
                column: "MaTL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietDonDatHang");

            migrationBuilder.DropTable(
                name: "GioHang");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "Sach");

            migrationBuilder.DropTable(
                name: "DonDatHang");

            migrationBuilder.DropTable(
                name: "NhaXuatBan");

            migrationBuilder.DropTable(
                name: "TacGia");

            migrationBuilder.DropTable(
                name: "TheLoai");

            migrationBuilder.DropTable(
                name: "NguoiQuanTri");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "NguoiDung");
        }
    }
}
