use master
DROP DATABASE QL_BOOKSTORE



CREATE DATABASE QL_BOOKSTORE
GO
USE QL_BOOKSTORE
GO
CREATE TABLE TACGIA
(
	MATG INT PRIMARY KEY,
	TENTG NVARCHAR(100),
	DIACHI NVARCHAR(100),
	TIEUSU NVARCHAR(100),
	DIENTHOAI VARCHAR(20),
)
CREATE TABLE NHAXUATBAN
(
	MANXB INT PRIMARY KEY,
	TENNXH NVARCHAR(100),
	DIACHI NVARCHAR(100),
	DIENTHOAI VARCHAR(20),
)
CREATE TABLE CHUDE
(
	MACD INT PRIMARY KEY,
	TENCHUDE NVARCHAR(100)
)
CREATE TABLE SACH
(
	MASACH INT PRIMARY KEY,
	TENSACH NVARCHAR(100),
	GIABAN DECIMAL,
	MOTA NTEXT,
	ANHBIA NVARCHAR(100),
	NGAYCAPNHAT DATETIME,
	SOLUONGBAN INT,
	MACD INT,
	MANXB INT,
	MOI BIT,
	FOREIGN KEY (MACD) REFERENCES CHUDE(MACD),
	FOREIGN KEY (MANXB) REFERENCES NHAXUATBAN(MANXB)
)
CREATE TABLE THAMGIA
(
	MATG INT,
	MASACH INT,
	VAITRO NVARCHAR(50),
	VITRI INT,
	PRIMARY KEY (MATG, MASACH),
	FOREIGN KEY (MATG) REFERENCES TACGIA(MATG),
	FOREIGN KEY (MASACH) REFERENCES SACH(MASACH),
)
CREATE TABLE KHACHHANG
(
	MAKH INT PRIMARY KEY,
	HOTEN NVARCHAR(100),
	TAIKHOAN NVARCHAR(100),
	MATKHAU VARCHAR(100),
	EMAIL VARCHAR(100),
	DIACHI NVARCHAR(100),
	DIENTHOAI VARCHAR(20),
	NGAYSINH DATE
)
CREATE TABLE DONDATHANG
(
	MADONHANG INT PRIMARY KEY,
	DATHANHTOAN BIT,
	TINHTRANGGIAOHANG NVARCHAR(100),
	NGAYDAT DATE,
	NGAYGIAO DATE,
	MAKH INT,
	FOREIGN KEY (MAKH) REFERENCES KHACHHANG(MAKH)
)
CREATE TABLE CHITIETDONHANG
(
	MADONHANG INT,
	MASACH INT,
	SOLUONG INT,
	DONGIA DECIMAL,
	PRIMARY KEY(MADONHANG, MASACH),
	FOREIGN KEY (MADONHANG) REFERENCES DONDATHANG(MADONHANG),
	FOREIGN KEY (MASACH) REFERENCES SACH(MASACH),
)
-- ====== TÁC GIẢ ======
INSERT INTO TACGIA VALUES 
(1, N'Nguyễn Nhật Ánh', N'TP.HCM', N'Tác giả nổi tiếng với truyện thiếu nhi', '0909123456'),
(2, N'Trần Đăng Khoa', N'Hà Nội', N'Nhà thơ nổi tiếng', '0912345678'),
(3, N'Hồ Anh Thái', N'Đà Nẵng', N'Nhà văn hiện đại Việt Nam', '0987654321'),
(4, N'Nguyễn Ngọc Tư', N'Cà Mau', N'Tác giả của Cánh đồng bất tận', '0909234567'),
(5, N'J.K. Rowling', N'Anh Quốc', N'Tác giả của Harry Potter', '0812345678'),
(6, N'Haruki Murakami', N'Nhật Bản', N'Tiểu thuyết gia nổi tiếng', '0701234567'),
(7, N'Dan Brown', N'Mỹ', N'Tác giả của Mật mã Da Vinci', '0666543210'),
(8, N'Nguyễn Văn A', N'TP.HCM', N'Tác giả trẻ mới nổi', '0911222333'),
(9, N'Jack Canfield', N'Mỹ', N'Đồng tác giả bộ Hạt Giống Tâm Hồn', '0666111222'),
(10, N'Mark Victor Hansen', N'Mỹ', N'Đồng tác giả bộ Hạt Giống Tâm Hồn', '0666333444'),
(11, N'First News – Trí Việt', N'TP.HCM', N'Nhóm biên soạn và phát hành tại Việt Nam', '02838222222');

-- ====== NHÀ XUẤT BẢN ======
INSERT INTO NHAXUATBAN VALUES
(1, N'NXB Trẻ', N'TP.HCM', '02838291234'),
(2, N'NXB Kim Đồng', N'Hà Nội', '02439456789'),
(3, N'NXB Văn Học', N'Hà Nội', '02437654321'),
(4, N'NXB Giáo Dục', N'Hà Nội', '02437373737'),
(5, N'NXB Tổng Hợp', N'TP.HCM', '02833445566'),
(6, N'NXB Hội Nhà Văn', N'Hà Nội', '02432221111');

-- ====== CHỦ ĐỀ ======
INSERT INTO CHUDE VALUES
(1, N'Truyện Thiếu Nhi'),
(2, N'Truyện Ngắn'),
(3, N'Tiểu Thuyết'),
(4, N'Khoa Học'),
(5, N'Tâm Lý - Kỹ Năng Sống'),
(6, N'Văn Học Nước Ngoài');

-- ====== SÁCH ======
INSERT INTO SACH VALUES
(1, N'Mắt Biếc', 85000, N'Câu chuyện tình cảm học trò', N'matbiec.png', '2023-05-01', 500, 3, 1, 1),
(2, N'Kính Vạn Hoa', 120000, N'Truyện dài nhiều tập hấp dẫn', N'kinhvanhoa.png', '2023-08-10', 300, 1, 2, 0),
(3, N'Hồ Quý Ly', 150000, N'Tiểu thuyết lịch sử đặc sắc', N'hoquyly.png', '2024-01-15', 200, 3, 3, 0),
(4, N'Cánh đồng bất tận', 95000, N'Truyện ngắn nổi tiếng của Nguyễn Ngọc Tư', N'canhdongbattan.png', '2023-07-12', 400, 2, 3, 1),
(5, N'Harry Potter và Hòn đá Phù thủy', 180000, N'Tập 1 trong series Harry Potter', N'harrypotter1.png', '2024-03-01', 600, 6, 2, 1),
(6, N'Rừng Nauy', 135000, N'Tiểu thuyết nổi tiếng của Haruki Murakami', N'rungnauy.png', '2024-02-10', 350, 6, 3, 0),
(7, N'Mật mã Da Vinci', 160000, N'Tiểu thuyết trinh thám nổi tiếng thế giới', N'matmadavinci.png', '2024-04-15', 500, 6, 3, 1),
(8, N'Hạt Giống Tâm Hồn', 90000, N'Tuyển tập truyền cảm hứng từ cuộc sống', N'hatgiongtamhon.png', '2023-11-30', 800, 5, 4, 0),
(9, N'Truyện cổ tích Việt Nam', 70000, N'Tuyển tập truyện dân gian', N'truyencotich.png', '2023-12-25', 700, 1, 1, 1),
(10, N'Giáo trình Toán 12', 60000, N'Sách giáo khoa nâng cao lớp 12', N'toan12.png', '2022-09-01', 1000, 4, 4, 0),
(11, N'Tôi thấy hoa vàng trên cỏ xanh', 110000, N'Tác phẩm nổi tiếng của Nguyễn Nhật Ánh', N'hoavang.png', '2024-01-25', 450, 3, 1, 1),
(12, N'Sống chậm lại, yêu thương nhiều hơn', 95000, N'Sách kỹ năng sống ý nghĩa', N'songcham.png', '2023-10-20', 370, 5, 5, 0),
(13, N'7 Thói quen của người thành đạt', 145000, N'Sách kỹ năng sống nổi tiếng thế giới', N'7thoiquen.png', '2024-05-30', 520, 5, 5, 1);

-- ====== THAMGIA ======
INSERT INTO THAMGIA VALUES
(1, 1, N'Tác giả', 1),
(1, 2, N'Tác giả', 1),
(3, 3, N'Tác giả', 1),
(4, 4, N'Tác giả', 1),
(5, 5, N'Tác giả', 1),
(6, 6, N'Tác giả', 1),
(7, 7, N'Tác giả', 1),
(9, 8, N'Đồng tác giả', 1),
(10, 8, N'Đồng tác giả', 2),
(11, 8, N'Biên dịch & phát hành', 3),
(2, 9, N'Tuyển chọn', 3),
(1, 9, N'Người kể lại', 2),
(4, 10, N'Chủ biên', 1),
(8, 11, N'Tác giả', 1),
(3, 12, N'Người hiệu đính', 2),
(4, 12, N'Tác giả', 1),
(7, 13, N'Tác giả', 1),
(6, 13, N'Hiệu đính', 2);

-- ====== KHÁCH HÀNG ======
INSERT INTO KHACHHANG VALUES
(1, N'Lê Minh Tâm', N'tamle', '123456', 'tamle@gmail.com', N'Hà Nội', '0905001234', '1998-07-10'),
(2, N'Phạm Thị Hoa', N'hoapt', 'abcdef', 'hoa.pt@gmail.com', N'TP.HCM', '0912005678', '2000-11-20'),
(3, N'Ngô Văn An', N'anngo', 'pass123', 'an.ngo@gmail.com', N'Đà Nẵng', '0989888777', '1995-03-15');

-- ====== ĐƠN ĐẶT HÀNG ======
INSERT INTO DONDATHANG VALUES
(1, 1, N'Đã giao', '2024-09-01', '2024-09-03', 1),
(2, 0, N'Đang giao', '2024-10-10', NULL, 2),
(3, 1, N'Đã giao', '2024-08-20', '2024-08-25', 3);

-- ====== CHI TIẾT ĐƠN HÀNG ======
INSERT INTO CHITIETDONHANG VALUES
(1, 1, 1, 85000),
(2, 2, 2, 120000),
(3, 3, 1, 150000);

select * from SACH
select * from TACGIA
select * from THAMGIA