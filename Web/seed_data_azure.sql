DECLARE @U0 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @U1 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 1 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @U2 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 2 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @U3 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 3 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @U4 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 4 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @U5 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 5 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @U6 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 6 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @U7 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 7 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @U8 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 8 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @U9 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 9 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @U10 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 10 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @U11 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 11 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @U12 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 12 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @U13 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 13 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @U14 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 14 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @U15 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 15 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @U16 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 16 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @U17 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 17 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @U18 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 18 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @U19 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 19 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @U20 NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET 20 ROWS FETCH NEXT 1 ROWS ONLY);


-- ============================================================
-- XÓA DỮ LIỆU CŨ (giữ nguyên Users)
-- ============================================================
DELETE FROM Reports;
DELETE FROM Answers;
DELETE FROM Questions;

-- ============================================================
-- QUESTIONS
-- ============================================================
INSERT INTO Questions (Content, CreatedAt, UserId) VALUES
(N'Con trỏ trong C hoạt động như thế nào? Sự khác biệt giữa con trỏ và biến thông thường là gì?',         DATEADD(DAY,-20,GETDATE()), @U0),
(N'Làm thế nào để debug chương trình Java hiệu quả? Có tool nào tốt không?',                               DATEADD(DAY,-19,GETDATE()), @U19),
(N'Sự khác biệt giữa Abstract Class và Interface trong Java là gì? Khi nào nên dùng cái nào?',             DATEADD(DAY,-18,GETDATE()), @U17),
(N'List comprehension trong Python là gì? Dùng khi nào thì hiệu quả hơn vòng lặp for thông thường?',      DATEADD(DAY,-17,GETDATE()), @U7),
(N'Cách tối ưu vòng lặp trong C++ để chạy nhanh hơn? Bài tập đang bị TLE trên Codeforces.',               DATEADD(DAY,-16,GETDATE()), @U6),
(N'JOIN trong SQL: INNER JOIN, LEFT JOIN, RIGHT JOIN khác nhau ở chỗ nào? Cho ví dụ cụ thể.',              DATEADD(DAY,-15,GETDATE()), @U15),
(N'Normalization (chuẩn hóa CSDL) là gì? 1NF, 2NF, 3NF khác nhau ra sao?',                                DATEADD(DAY,-14,GETDATE()), @U18),
(N'Mô hình OSI có 7 tầng, TCP/IP chỉ có 4 tầng. Vậy học OSI để làm gì?',                                  DATEADD(DAY,-13,GETDATE()), @U9),
(N'HTTPS khác HTTP ở điểm nào? SSL/TLS hoạt động như thế nào?',                                            DATEADD(DAY,-12,GETDATE()), @U8),
(N'Overfitting trong Machine Learning là gì? Cách phòng tránh như thế nào?',                               DATEADD(DAY,-11,GETDATE()), @U14),
(N'Gradient Descent hoạt động như thế nào? Tại sao cần learning rate?',                                    DATEADD(DAY,-10,GETDATE()), @U4),
(N'Design Pattern là gì? Singleton, Factory, Observer khác nhau ra sao?',                                  DATEADD(DAY,-9,GETDATE()),  @U5),
(N'Git flow là gì? Quy trình làm việc nhóm với Git như thế nào cho đúng chuẩn?',                           DATEADD(DAY,-8,GETDATE()),  @U10),
(N'SQL Injection là gì? Cách phòng chống SQL Injection trong ASP.NET Core?',                                DATEADD(DAY,-7,GETDATE()),  @U12),
(N'JWT Token là gì? Cách dùng JWT để xác thực trong REST API như thế nào?',                                 DATEADD(DAY,-6,GETDATE()),  @U3),
(N'Dependency Injection trong ASP.NET Core là gì? Tại sao cần dùng DI thay vì new trực tiếp?',             DATEADD(DAY,-5,GETDATE()),  @U11),
(N'Entity Framework Core vs Dapper: cái nào nên dùng cho project vừa và nhỏ?',                             DATEADD(DAY,-4,GETDATE()),  @U1),
(N'Làm thế nào để deploy ứng dụng ASP.NET Core lên Docker? Cần Dockerfile như thế nào?',                   DATEADD(DAY,-3,GETDATE()),  @U16),
(N'Giới hạn (limit) trong giải tích là gì? Tại sao lim(x→0) sin(x)/x = 1?',                               DATEADD(DAY,-2,GETDATE()),  @U13),
(N'Agile và Waterfall khác nhau như thế nào? Dự án sinh viên nên dùng mô hình nào?',                       DATEADD(DAY,-1,GETDATE()),  @U2);

-- ============================================================
-- ANSWERS
-- ============================================================

-- Q1: Con trỏ trong C
INSERT INTO Answers (Content, CreatedAt, QuestionId, UserId) VALUES
(N'Con trỏ là biến lưu địa chỉ bộ nhớ của biến khác. Ví dụ: int x=5; int* p=&x; thì *p=5. Khác biến thường ở chỗ cho phép thao tác trực tiếp vùng nhớ, dùng nhiều trong cấp phát động và truyền tham chiếu.',
    DATEADD(DAY,-19,GETDATE()), (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Con trỏ trong C%'), @U19),
(N'Hình dung con trỏ như tờ giấy ghi địa chỉ nhà. Dùng & để lấy địa chỉ, dùng * để đến nhà đó lấy giá trị. Luôn khởi tạo NULL trước khi dùng để tránh dangling pointer.',
    DATEADD(DAY,-18,GETDATE()), (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Con trỏ trong C%'), @U20);

-- Q2: Debug Java
INSERT INTO Answers (Content, CreatedAt, QuestionId, UserId) VALUES
(N'Dùng IntelliJ IDEA: click vào số dòng để đặt breakpoint, chạy Debug Mode (Shift+F9), Step Over (F8) để chạy từng bước, xem giá trị biến trong panel Variables.',
    DATEADD(DAY,-18,GETDATE()), (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%debug chương trình Java%'), @U17),
(N'Ngoài IDE có thể dùng System.out.println() tạm để in giá trị. Hoặc dùng Log4j/SLF4J để ghi log có level (DEBUG, INFO, ERROR) chuyên nghiệp hơn.',
    DATEADD(DAY,-17,GETDATE()), (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%debug chương trình Java%'), @U7);

-- Q3: Abstract Class vs Interface
INSERT INTO Answers (Content, CreatedAt, QuestionId, UserId) VALUES
(N'Abstract Class: dùng khi các class con có chung implementation (có thể kế thừa code). Interface: định nghĩa contract, ai implement phải có đủ method. Một class chỉ extends 1 abstract class nhưng implement được nhiều interface.',
    DATEADD(DAY,-17,GETDATE()), (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Abstract Class và Interface%'), @U6),
(N'Nguyên tắc: dùng Interface khi các class không liên quan (Bird và Airplane đều implement Flyable). Dùng Abstract Class cho quan hệ is-a (Dog is-a Animal).',
    DATEADD(DAY,-16,GETDATE()), (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Abstract Class và Interface%'), @U20);

-- Q6: JOIN trong SQL
INSERT INTO Answers (Content, CreatedAt, QuestionId, UserId) VALUES
(N'INNER JOIN: chỉ lấy hàng khớp ở cả 2 bảng. LEFT JOIN: lấy tất cả bảng trái, bảng phải NULL nếu không khớp. RIGHT JOIN: ngược lại. Thực tế INNER và LEFT được dùng nhiều nhất.',
    DATEADD(DAY,-14,GETDATE()), (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%JOIN trong SQL%'), @U15),
(N'Ví dụ LEFT JOIN: SELECT u.FullName, r.Reason FROM AspNetUsers u LEFT JOIN Reports r ON u.Id = r.TargetUserId -- lấy tất cả user kể cả chưa bị báo cáo, r.Reason sẽ NULL nếu chưa có report.',
    DATEADD(DAY,-13,GETDATE()), (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%JOIN trong SQL%'), @U20);

-- Q10: Overfitting
INSERT INTO Answers (Content, CreatedAt, QuestionId, UserId) VALUES
(N'Overfitting: model học thuộc training data nhưng không generalize. Nhận biết: train accuracy cao, val accuracy thấp. Cách fix: tăng data, thêm Dropout, dùng L1/L2 regularization, giảm độ phức tạp model.',
    DATEADD(DAY,-10,GETDATE()), (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Overfitting%'), @U4),
(N'Early stopping rất hiệu quả: dừng training khi validation loss bắt đầu tăng. Ngoài ra dùng k-fold cross validation để đánh giá model chính xác hơn thay vì chỉ một tập val.',
    DATEADD(DAY,-9,GETDATE()),  (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Overfitting%'), @U20);

-- Q12: Design Pattern
INSERT INTO Answers (Content, CreatedAt, QuestionId, UserId) VALUES
(N'Singleton: đảm bảo chỉ 1 instance trong app. Factory: tạo object mà không cần biết class cụ thể. Observer: khi object thay đổi, tự notify tất cả subscriber. Design Pattern chia 3 nhóm: Creational, Structural, Behavioral.',
    DATEADD(DAY,-8,GETDATE()),  (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Design Pattern%'), @U10),
(N'Học Design Pattern trên refactoring.guru rất dễ hiểu, có ví dụ bằng nhiều ngôn ngữ. Trong ASP.NET Core bạn đang dùng Repository Pattern và Dependency Injection rồi đó.',
    DATEADD(DAY,-7,GETDATE()),  (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Design Pattern%'), @U20);

-- Q14: SQL Injection
INSERT INTO Answers (Content, CreatedAt, QuestionId, UserId) VALUES
(N'SQL Injection: attacker chèn SQL vào input để thao túng DB. Ví dụ: nhập '' OR ''1''=''1 để bypass login. Phòng chống: dùng Parameterized Queries hoặc ORM như EF Core tự động xử lý.',
    DATEADD(DAY,-6,GETDATE()),  (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%SQL Injection%'), @U3),
(N'Trong EF Core: .Where(u => u.Email == email) tự động parameterize an toàn. Nếu cần raw SQL thì dùng FromSqlRaw("SELECT * FROM Users WHERE Email = {0}", email) chứ không bao giờ nối chuỗi.',
    DATEADD(DAY,-5,GETDATE()),  (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%SQL Injection%'), @U20);

-- Q16: Dependency Injection
INSERT INTO Answers (Content, CreatedAt, QuestionId, UserId) VALUES
(N'DI là pattern giúp class không tự tạo dependency mà nhận từ bên ngoài. Trong ASP.NET Core đăng ký bằng builder.Services.AddScoped/Transient/Singleton rồi inject qua constructor. Giúp code dễ test và mở rộng hơn.',
    DATEADD(DAY,-4,GETDATE()),  (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Dependency Injection%'), @U1),
(N'AddScoped: tạo mới mỗi HTTP request. AddTransient: tạo mới mỗi lần inject. AddSingleton: chỉ tạo 1 lần suốt vòng đời app. Repository và Service nên dùng AddScoped là chuẩn nhất.',
    DATEADD(DAY,-3,GETDATE()),  (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Dependency Injection%'), @U20);

-- Q20: Agile vs Waterfall
INSERT INTO Answers (Content, CreatedAt, QuestionId, UserId) VALUES
(N'Waterfall: làm tuần tự từng giai đoạn (Req → Design → Code → Test → Deploy), khó quay lại. Agile: chia nhỏ thành Sprint 2 tuần, liên tục deliver và nhận feedback. Project sinh viên nên dùng Agile vì requirement hay thay đổi.',
    DATEADD(DAY,-1,GETDATE()),  (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Agile và Waterfall%'), @U16);

-- ============================================================
-- REPORTS
-- ============================================================

-- Đã xử lý → hiện tab "Lịch sử xử lý"
INSERT INTO Reports (ReporterUserId, TargetUserId, Reason, IsResolved, ActionTaken, CreatedAt, QuestionId, AnswerId, DocumentId) VALUES
(@U15, @U0,
 N'Người dùng đăng câu hỏi spam link quảng cáo không liên quan học tập.',
 1, N'Admin đã xóa bài vi phạm, trừ 10 điểm và cảnh cáo.', DATEADD(DAY,-18,GETDATE()), NULL, NULL, NULL),

(@U18, @U19,
 N'Bình luận chứa từ ngữ thô tục, xúc phạm người dùng khác trong thread.',
 1, N'Admin đã trừ 10 điểm và ghi nhận 1 lần cảnh cáo.', DATEADD(DAY,-14,GETDATE()), NULL, NULL, NULL),

(@U9, @U17,
 N'Tài liệu đăng có nội dung copy từ nguồn khác, không ghi rõ bản quyền.',
 1, N'Admin đã bỏ qua báo cáo này.', DATEADD(DAY,-10,GETDATE()), NULL, NULL, NULL),

(NULL, @U0,
 N'[AI Phát hiện] Nội dung câu hỏi chứa từ ngữ không phù hợp với cộng đồng.',
 1, N'Hệ thống (AI) tự động phạt trừ 10 điểm và tăng 1 gậy cảnh cáo.', DATEADD(DAY,-8,GETDATE()), NULL, NULL, NULL),

(@U8, @U19,
 N'Người dùng liên tục đăng câu hỏi trùng lặp, gây spam diễn đàn.',
 1, N'Admin đã xóa bài vi phạm, trừ 10 điểm và cảnh cáo.', DATEADD(DAY,-5,GETDATE()), NULL, NULL, NULL),

(@U14, @U7,
 N'Câu trả lời cố tình đưa thông tin sai lệch về thuật toán để đánh lừa người học.',
 1, N'Admin đã trừ 10 điểm và ghi nhận 1 lần cảnh cáo.', DATEADD(DAY,-3,GETDATE()), NULL, NULL, NULL),

(NULL, @U6,
 N'[AI Phát hiện] Nội dung bình luận có dấu hiệu quảng cáo dịch vụ bên ngoài.',
 1, N'Hệ thống (AI) tự động phạt trừ 10 điểm và tăng 1 gậy cảnh cáo.', DATEADD(DAY,-2,GETDATE()), NULL, NULL, NULL),

-- Đang chờ → hiện tab "Chờ xử lý"
(@U15, @U17,
 N'Tài liệu có nội dung sai kiến thức cơ bản về cấu trúc dữ liệu, gây hiểu nhầm cho sinh viên.',
 0, NULL, DATEADD(DAY,-2,GETDATE()), NULL, NULL, NULL),

(@U18, @U0,
 N'Người dùng đăng câu hỏi không liên quan đến chủ đề học tập của trang web.',
 0, NULL, DATEADD(DAY,-1,GETDATE()), NULL, NULL, NULL),

(@U9, @U19,
 N'Câu trả lời này đưa ra code mẫu bị lỗi nhưng lại khẳng định là đúng.',
 0, NULL, GETDATE(), NULL, NULL, NULL);

-- ============================================================
-- KIỂM TRA KẾT QUẢ
-- ============================================================
SELECT N'Questions'            AS [Table], COUNT(*) AS [Count] FROM Questions
UNION ALL SELECT N'Answers',              COUNT(*) FROM Answers
UNION ALL SELECT N'Reports (Resolved)',   COUNT(*) FROM Reports WHERE IsResolved = 1
UNION ALL SELECT N'Reports (Pending)',    COUNT(*) FROM Reports WHERE IsResolved = 0;
