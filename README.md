### HƯỚNG DẪN CÀI ĐẶT VÀ CHẠY DỰ ÁN

**1. Khởi tạo Cơ sở dữ liệu (Database)**
Mở SQL Server Management Studio (SSMS) và thực thi (execute) các file `.sql` trong thư mục `Database` theo thứ tự sau:
- `01_create_tables.sql` (Tạo cấu trúc bảng)
- `02_business_logic.sql` (Tạo Trigger, Function, Stored Procedure, View)
- `03_insert_data.sql` (Thêm dữ liệu mẫu vào database)

---

**2. Cấu hình Backend (Visual Studio)**
Trước khi chạy ứng dụng Backend, cần mở file `appsettings.json` trong thư mục project `BackendAPI` và cập nhật lại chuỗi kết nối (`ConnectionString`) sao cho khớp với tên Server/Instance SQL Server được cài đặt trên máy.

---

**3. Thông tin tài khoản kiểm tra nhanh các chức năng**

* **Phân quyền Khách hàng:** - Username: `user_001` 
    - Email: `user_001@gmail.com` 
    - Mật khẩu: `123456`
    
* **Phân quyền Admin (Quản trị viên):** - Username: `user_037` 
    - Email: `user_037@gmail.com` 
    - Mật khẩu: `123456`
