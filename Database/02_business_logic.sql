USE QuanLyNhaSach;
GO

CREATE OR ALTER TRIGGER trg_TruTonKhoSanPham
ON ChiTietDonDatHang  
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE S
    SET S.SLTon = S.SLTon - I.SLSach,
        S.TrangThaiS = CASE WHEN (S.SLTon - I.SLSach) <= 0 THEN 0 ELSE S.TrangThaiS END
    FROM Sach S
    JOIN inserted I ON S.MaSach = I.MaSach;

    IF EXISTS (
        SELECT 1 
        FROM Sach S
        JOIN inserted I ON S.MaSach = I.MaSach
        WHERE S.SLTon < 0
    )
    BEGIN
        RAISERROR(N'Lỗi: Số lượng sách trong kho không đủ để bán!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

CREATE OR ALTER FUNCTION fn_TinhTongTienDonHang (@MaDH INT)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @TongTien DECIMAL(18,2);
    
    SELECT @TongTien = SUM(SLSach * DonGia)
    FROM ChiTietDonDatHang
    WHERE MaDH = @MaDH;

    RETURN ISNULL(@TongTien, 0); 
END
GO

CREATE OR ALTER PROCEDURE sp_TaoDonHang_CoTransaction
    @MaKH INT, 
    @DiaChiGiaoHang NVARCHAR(MAX),
    @GioHangJSON NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN; 
        DECLARE @TenSachLoi NVARCHAR(255);
        DECLARE @KhoCon INT;
        DECLARE @KhachDat INT;

        SELECT TOP 1 
            @TenSachLoi = S.TenSach,
            @KhoCon = S.SLTon,
            @KhachDat = GH.SoLuongSP
        FROM OPENJSON(@GioHangJSON)
        WITH (
            MaSach INT '$.MaSach',
            SoLuongSP INT '$.SoLuongSP'
        ) AS GH
        JOIN Sach S ON GH.MaSach = S.MaSach
        WHERE GH.SoLuongSP > S.SLTon;

        IF @TenSachLoi IS NOT NULL
        BEGIN
            DECLARE @ErrMsgEx NVARCHAR(2048) = N'Sách "' + @TenSachLoi + N'" hiện chỉ còn ' + CAST(@KhoCon AS NVARCHAR) + N' cuốn, không đủ để đặt ' + CAST(@KhachDat AS NVARCHAR) + N' cuốn!';
            ROLLBACK TRAN;
            THROW 50001, @ErrMsgEx, 1;
            RETURN;
        END;

        INSERT INTO DonDatHang (MaND, DiaChiGH, TrangThaiDon, NgayDat)
        VALUES (@MaKH, @DiaChiGiaoHang, 0, GETDATE());
        
        DECLARE @MaDonVuaTao INT = SCOPE_IDENTITY(); 

        INSERT INTO HoaDon (MaDH, TrangThaiTT, NgayTao)
        VALUES (@MaDonVuaTao, N'Chưa thanh toán', GETDATE());

        INSERT INTO ChiTietDonDatHang (MaDH, MaSach, SLSach, DonGia)
        SELECT 
            @MaDonVuaTao, 
            CAST(JSON_VALUE(value, '$.MaSach') AS INT), 
            CAST(JSON_VALUE(value, '$.SoLuongSP') AS INT), 
            CAST(JSON_VALUE(value, '$.DonGia') AS DECIMAL(18,2))
        FROM OPENJSON(@GioHangJSON);

        DECLARE @TongTienDon DECIMAL(18,2);

        SET @TongTienDon = dbo.fn_TinhTongTienDonHang(@MaDonVuaTao);

        UPDATE DonDatHang
        SET TongTien = @TongTienDon
        WHERE MaDH = @MaDonVuaTao;

        COMMIT TRAN; 
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN; 
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE sp_ThongKeDoanhThu
    @Nam  INT = NULL,   
    @Thang INT = NULL  
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        YEAR(hd.NgayTao)                        AS Nam,
        MONTH(hd.NgayTao)                       AS Thang,
        COUNT(DISTINCT hd.MaHD)                 AS SoDonHang,
        SUM(ddh.TongTien)                       AS DoanhThu
    FROM HoaDon hd
    INNER JOIN DonDatHang ddh ON hd.MaDH = ddh.MaDH
    WHERE
        ddh.TrangThaiDon = 3
        AND (@Nam   IS NULL OR YEAR(hd.NgayTao)  = @Nam)
        AND (@Thang IS NULL OR MONTH(hd.NgayTao) = @Thang)
    GROUP BY
        YEAR(hd.NgayTao),
        MONTH(hd.NgayTao)
    ORDER BY
        Nam DESC,
        Thang DESC;
END
GO


CREATE OR ALTER PROCEDURE sp_ThongKeDoanhThuTheoNgay
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        CAST(HD.NgayTao AS DATE) AS Ngay,
        COUNT(DISTINCT HD.MaHD) AS SoDonHang,
        SUM(CT.SLSach * CT.DonGia) AS DoanhThu
    FROM HoaDon HD
    JOIN DonDatHang DDH ON HD.MaDH = DDH.MaDH
    JOIN ChiTietDonDatHang CT ON DDH.MaDH = CT.MaDH
    WHERE DDH.TrangThaiDon = 3 
    GROUP BY CAST(HD.NgayTao AS DATE)
    ORDER BY Ngay DESC;
END
GO

CREATE OR ALTER VIEW vw_TopSachBanChay AS
SELECT TOP 10
    S.MaSach,
    S.TenSach,
    SUM(CT.SLSach) AS TongSoLuongBan
FROM Sach S
JOIN ChiTietDonDatHang CT ON S.MaSach = CT.MaSach
JOIN DonDatHang DDH ON CT.MaDH = DDH.MaDH
WHERE DDH.TrangThaiDon = 3 
GROUP BY S.MaSach, S.TenSach
ORDER BY TongSoLuongBan DESC;
GO
