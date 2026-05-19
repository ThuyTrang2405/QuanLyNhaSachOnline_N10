using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaSachAPI.Models;

public partial class QuanLyNhaSachContext : DbContext
{
    public QuanLyNhaSachContext()
    {
    }

    public QuanLyNhaSachContext(DbContextOptions<QuanLyNhaSachContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietDonDatHang> ChiTietDonDatHangs { get; set; }

    public virtual DbSet<DonDatHang> DonDatHangs { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NguoiQuanTri> NguoiQuanTris { get; set; }

    public virtual DbSet<NhaXuatBan> NhaXuatBans { get; set; }

    public virtual DbSet<Sach> Saches { get; set; }

    public virtual DbSet<TacGium> TacGia { get; set; }

    public virtual DbSet<TheLoai> TheLoais { get; set; }

     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietDonDatHang>(entity =>
        {
            entity.HasKey(e => new { e.MaDh, e.MaSach }).HasName("PK__ChiTietD__EC06D12342A4BA5F");

            entity.ToTable("ChiTietDonDatHang");

            entity.Property(e => e.MaDh).HasColumnName("MaDH");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Slsach).HasColumnName("SLSach");

            entity.HasOne(d => d.MaDhNavigation).WithMany(p => p.ChiTietDonDatHangs)
                .HasForeignKey(d => d.MaDh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDon__MaDH__45F365D3");

            entity.HasOne(d => d.MaSachNavigation).WithMany(p => p.ChiTietDonDatHangs)
                .HasForeignKey(d => d.MaSach)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__MaSac__46E78A0C");
        });

        modelBuilder.Entity<DonDatHang>(entity =>
        {
            entity.HasKey(e => e.MaDh).HasName("PK__DonDatHa__2725866111C3ACAF");

            entity.ToTable("DonDatHang");

            entity.Property(e => e.MaDh).HasColumnName("MaDH");
            entity.Property(e => e.DiaChiGh)
                .HasMaxLength(255)
                .HasColumnName("DiaChiGH");
            entity.Property(e => e.MaNd).HasColumnName("MaND");
            entity.Property(e => e.MaVanDon)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NgayDat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThaiDon).HasDefaultValue(0);

            entity.HasOne(d => d.MaNdNavigation).WithMany(p => p.DonDatHangs)
                .HasForeignKey(d => d.MaNd)
                .HasConstraintName("FK__DonDatHang__MaND__4316F928");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => new { e.MaNd, e.MaSach }).HasName("PK__GioHang__EC068066B8D4412E");

            entity.ToTable("GioHang");

            entity.Property(e => e.MaNd).HasColumnName("MaND");
            entity.Property(e => e.TongSlsach)
                .HasDefaultValue(1)
                .HasColumnName("TongSLSach");

            entity.HasOne(d => d.MaNdNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaNd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHang__MaND__3D5E1FD2");

            entity.HasOne(d => d.MaSachNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaSach)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHang__MaSach__3E52440B");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHd).HasName("PK__HoaDon__2725A6E03D1FDACB");

            entity.ToTable("HoaDon");

            entity.HasIndex(e => e.MaDh, "UQ__HoaDon__27258660682DFF8C").IsUnique();

            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.MaDh).HasColumnName("MaDH");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThaiTt)
                .HasMaxLength(50)
                .HasColumnName("TrangThaiTT");

            entity.HasOne(d => d.MaDhNavigation).WithOne(p => p.HoaDon)
                .HasForeignKey<HoaDon>(d => d.MaDh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__MaDH__4BAC3F29");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaNd).HasName("PK__KhachHan__2725D7248678DFF4");

            entity.ToTable("KhachHang");

            entity.Property(e => e.MaNd)
                .ValueGeneratedNever()
                .HasColumnName("MaND");
            entity.Property(e => e.TrangThaiTk)
                .HasDefaultValue(true)
                .HasColumnName("TrangThaiTK");

            entity.HasOne(d => d.MaNdNavigation).WithOne(p => p.KhachHang)
                .HasForeignKey<KhachHang>(d => d.MaNd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KhachHang__MaND__2F10007B");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNd).HasName("PK__NguoiDun__2725D7246BA112F9");

            entity.ToTable("NguoiDung");

            entity.HasIndex(e => e.TenNd, "UQ__NguoiDun__4CF9B49A554A5111").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__NguoiDun__A9D10534E8753DD0").IsUnique();

            entity.Property(e => e.MaNd).HasColumnName("MaND");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TenNd)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TenND");
        });

        modelBuilder.Entity<NguoiQuanTri>(entity =>
        {
            entity.HasKey(e => e.MaNd).HasName("PK__NguoiQua__2725D7248A7FF95D");

            entity.ToTable("NguoiQuanTri");

            entity.Property(e => e.MaNd)
                .ValueGeneratedNever()
                .HasColumnName("MaND");
            entity.Property(e => e.NgayVaoLam).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaNdNavigation).WithOne(p => p.NguoiQuanTri)
                .HasForeignKey<NguoiQuanTri>(d => d.MaNd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NguoiQuanT__MaND__32E0915F");
        });

        modelBuilder.Entity<NhaXuatBan>(entity =>
        {
            entity.HasKey(e => e.MaNxb).HasName("PK__NhaXuatB__3A19482C8C8E520E");

            entity.ToTable("NhaXuatBan");

            entity.Property(e => e.MaNxb).HasColumnName("MaNXB");
            entity.Property(e => e.TenNxb)
                .HasMaxLength(100)
                .HasColumnName("TenNXB");
        });

        modelBuilder.Entity<Sach>(entity =>
        {
            entity.HasKey(e => e.MaSach).HasName("PK__Sach__B235742DD46005CB");

            entity.ToTable("Sach");

            entity.Property(e => e.GiaGoc).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.HinhAnh)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.MaNxb).HasColumnName("MaNXB");
            entity.Property(e => e.MaTg).HasColumnName("MaTG");
            entity.Property(e => e.MaTl).HasColumnName("MaTL");
            entity.Property(e => e.Slton)
                .HasDefaultValue(0)
                .HasColumnName("SLTon");
            entity.Property(e => e.TenSach).HasMaxLength(255);
            entity.Property(e => e.TrangThaiS).HasDefaultValue(true);

            entity.HasOne(d => d.MaNxbNavigation).WithMany(p => p.Saches)
                .HasForeignKey(d => d.MaNxb)
                .HasConstraintName("FK__Sach__MaNXB__398D8EEE");

            entity.HasOne(d => d.MaTgNavigation).WithMany(p => p.Saches)
                .HasForeignKey(d => d.MaTg)
                .HasConstraintName("FK__Sach__MaTG__38996AB5");

            entity.HasOne(d => d.MaTlNavigation).WithMany(p => p.Saches)
                .HasForeignKey(d => d.MaTl)
                .HasConstraintName("FK__Sach__MaTL__37A5467C");
        });

        modelBuilder.Entity<TacGium>(entity =>
        {
            entity.HasKey(e => e.MaTg).HasName("PK__TacGia__272500740870C41F");

            entity.Property(e => e.MaTg).HasColumnName("MaTG");
            entity.Property(e => e.TenTg)
                .HasMaxLength(100)
                .HasColumnName("TenTG");
        });

        modelBuilder.Entity<TheLoai>(entity =>
        {
            entity.HasKey(e => e.MaTl).HasName("PK__TheLoai__272500717CF3806F");

            entity.ToTable("TheLoai");

            entity.Property(e => e.MaTl).HasColumnName("MaTL");
            entity.Property(e => e.TenTl)
                .HasMaxLength(100)
                .HasColumnName("TenTL");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
