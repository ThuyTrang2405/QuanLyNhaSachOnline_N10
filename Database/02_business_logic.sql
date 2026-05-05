

CREATE TRIGGER trg_TruTonKhoSanPham
ON ChiTietDonHang
AFTER INSERT
AS
BEGIN

    UPDATE Sach
    SET SoLuongSP = Sach.SoLuongSP - inserted.SoLuongSP
    FROM Sach
    JOIN inserted ON Sach.MaSach = inserted.MaSach;

    IF EXISTS (
        SELECT 1 
        FROM Sach 
        JOIN inserted ON Sach.MaSach = inserted.MaSach 
        WHERE Sach.SoLuongSP < 0
    )
    BEGIN
        RAISERROR(N'Lỗi: Số lượng sách trong kho không đủ để bán!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END


CREATE FUNCTION fn_TinhTongTienDonHang (@MaDonDH INT)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @TongTien DECIMAL(18,2);
    
    SELECT @TongTien = SUM(SoLuongSP * DonGia)
    FROM ChiTietDonHang
    WHERE MaDonDH = @MaDonDH;

    RETURN ISNULL(@TongTien, 0); 
END

CREATE PROCEDURE sp_TaoDonHang_CoTransaction
    @MaKH INT, 
    @DiaChiGiaoHang NVARCHAR(MAX),
    @GioHangJSON NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN; 

        INSERT INTO DonDatHang (MaKH, DiaChiGiaoHang, TrangThai, NgayLapDon)
        VALUES (@MaKH, @DiaChiGiaoHang, N'Chờ xử lý', GETDATE());
        
        DECLARE @MaDonVuaTao INT = SCOPE_IDENTITY(); 

        INSERT INTO HoaDon (MaHD, TrangThaiThanhToan, NgayTao)
        VALUES (@MaDonVuaTao, N'Chưa thanh toán', GETDATE());


        INSERT INTO ChiTietDonHang (MaDonDH, MaSach, SoLuongSP, DonGia)
        SELECT 
            @MaDonVuaTao, 
            CAST(JSON_VALUE(value, '$.MaSach') AS INT), 
            CAST(JSON_VALUE(value, '$.SoLuongSP') AS INT), 
            CAST(JSON_VALUE(value, '$.DonGia') AS DECIMAL) 
        FROM OPENJSON(@GioHangJSON);

        COMMIT TRAN; 
        
    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRAN; 
        
        THROW; 
    END CATCH
END