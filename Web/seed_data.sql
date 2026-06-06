USE PBL3;
GO

-- ============================================================
-- XÓA DỮ LIỆU CŨ (giữ nguyên Users)
-- ============================================================
DELETE FROM Reports;
DELETE FROM Answers;
DELETE FROM Questions;
GO

-- ============================================================
-- QUESTIONS
-- ============================================================
INSERT INTO Questions (Content, CreatedAt, UserId) VALUES
(N'Con trỏ trong C hoạt động như thế nào? Sự khác biệt giữa con trỏ và biến thông thường là gì?',         DATEADD(DAY,-20,GETDATE()), '5E25BD21-658A-415F-B308-6647AC9FCF7A'),
(N'Làm thế nào để debug chương trình Java hiệu quả? Có tool nào tốt không?',                               DATEADD(DAY,-19,GETDATE()), 'A7AD996F-432C-4F6A-B5DB-5AB32A45217F'),
(N'Sự khác biệt giữa Abstract Class và Interface trong Java là gì? Khi nào nên dùng cái nào?',             DATEADD(DAY,-18,GETDATE()), '9942230F-804A-4331-BB7E-2A8A7F53A125'),
(N'List comprehension trong Python là gì? Dùng khi nào thì hiệu quả hơn vòng lặp for thông thường?',      DATEADD(DAY,-17,GETDATE()), '43F5563E-CD98-49B8-8E5D-58B796480B5A'),
(N'Cách tối ưu vòng lặp trong C++ để chạy nhanh hơn? Bài tập đang bị TLE trên Codeforces.',               DATEADD(DAY,-16,GETDATE()), 'D3C97F9B-DDFF-4179-9CEB-2D475BEB7AB5'),
(N'JOIN trong SQL: INNER JOIN, LEFT JOIN, RIGHT JOIN khác nhau ở chỗ nào? Cho ví dụ cụ thể.',              DATEADD(DAY,-15,GETDATE()), '12A84878-0A0E-42B2-B1DA-B90F60C13C0B'),
(N'Normalization (chuẩn hóa CSDL) là gì? 1NF, 2NF, 3NF khác nhau ra sao?',                                DATEADD(DAY,-14,GETDATE()), '261548BF-27E5-46D2-AF3E-6678AC001850'),
(N'Mô hình OSI có 7 tầng, TCP/IP chỉ có 4 tầng. Vậy học OSI để làm gì?',                                  DATEADD(DAY,-13,GETDATE()), '046F7B16-2038-4C2B-AF07-F63CD9B8CBF1'),
(N'HTTPS khác HTTP ở điểm nào? SSL/TLS hoạt động như thế nào?',                                            DATEADD(DAY,-12,GETDATE()), 'EC9709AA-DA08-4F54-8A20-0C4EF3261BC4'),
(N'Overfitting trong Machine Learning là gì? Cách phòng tránh như thế nào?',                               DATEADD(DAY,-11,GETDATE()), 'F90DE63E-ABDC-4045-AB77-E16D51458DC0'),
(N'Gradient Descent hoạt động như thế nào? Tại sao cần learning rate?',                                    DATEADD(DAY,-10,GETDATE()), 'DE34B040-58A2-4762-AFA3-7300785A7310'),
(N'Design Pattern là gì? Singleton, Factory, Observer khác nhau ra sao?',                                  DATEADD(DAY,-9,GETDATE()),  '01A202D9-A7C2-48DD-AAE1-F425EEA5B862'),
(N'Git flow là gì? Quy trình làm việc nhóm với Git như thế nào cho đúng chuẩn?',                           DATEADD(DAY,-8,GETDATE()),  '9CBEE061-3782-4281-81EE-EBAD6224124E'),
(N'SQL Injection là gì? Cách phòng chống SQL Injection trong ASP.NET Core?',                                DATEADD(DAY,-7,GETDATE()),  'E8202BE3-F823-43D7-ABBB-B5C294913F58'),
(N'JWT Token là gì? Cách dùng JWT để xác thực trong REST API như thế nào?',                                 DATEADD(DAY,-6,GETDATE()),  '26AE5E18-F3E0-4E49-A1E6-41F15D6DDA1B'),
(N'Dependency Injection trong ASP.NET Core là gì? Tại sao cần dùng DI thay vì new trực tiếp?',             DATEADD(DAY,-5,GETDATE()),  '962AA70E-DD55-4269-A6AC-A159A6B805E2'),
(N'Entity Framework Core vs Dapper: cái nào nên dùng cho project vừa và nhỏ?',                             DATEADD(DAY,-4,GETDATE()),  '38EECF7B-AFFE-4265-848E-53CF1B8704D4'),
(N'Làm thế nào để deploy ứng dụng ASP.NET Core lên Docker? Cần Dockerfile như thế nào?',                   DATEADD(DAY,-3,GETDATE()),  '101A09FA-D842-4023-A6D8-28CC6C87421C'),
(N'Giới hạn (limit) trong giải tích là gì? Tại sao lim(x→0) sin(x)/x = 1?',                               DATEADD(DAY,-2,GETDATE()),  '0E74F859-692A-4A21-8AB1-DB56072ACCEC'),
(N'Agile và Waterfall khác nhau như thế nào? Dự án sinh viên nên dùng mô hình nào?',                       DATEADD(DAY,-1,GETDATE()),  'b0c01182-8f88-4f8d-8fa1-732af1da8a9e');
GO

-- ============================================================
-- ANSWERS
-- ============================================================

-- Q1: Con trỏ trong C
INSERT INTO Answers (Content, CreatedAt, QuestionId, UserId) VALUES
(N'Con trỏ là biến lưu địa chỉ bộ nhớ của biến khác. Ví dụ: int x=5; int* p=&x; thì *p=5. Khác biến thường ở chỗ cho phép thao tác trực tiếp vùng nhớ, dùng nhiều trong cấp phát động và truyền tham chiếu.',
    DATEADD(DAY,-19,GETDATE()), (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Con trỏ trong C%'), 'A7AD996F-432C-4F6A-B5DB-5AB32A45217F'),
(N'Hình dung con trỏ như tờ giấy ghi địa chỉ nhà. Dùng & để lấy địa chỉ, dùng * để đến nhà đó lấy giá trị. Luôn khởi tạo NULL trước khi dùng để tránh dangling pointer.',
    DATEADD(DAY,-18,GETDATE()), (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Con trỏ trong C%'), 'db9ffa33-93ef-41a7-8dbf-649a65cdb761');

-- Q2: Debug Java
INSERT INTO Answers (Content, CreatedAt, QuestionId, UserId) VALUES
(N'Dùng IntelliJ IDEA: click vào số dòng để đặt breakpoint, chạy Debug Mode (Shift+F9), Step Over (F8) để chạy từng bước, xem giá trị biến trong panel Variables.',
    DATEADD(DAY,-18,GETDATE()), (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%debug chương trình Java%'), '9942230F-804A-4331-BB7E-2A8A7F53A125'),
(N'Ngoài IDE có thể dùng System.out.println() tạm để in giá trị. Hoặc dùng Log4j/SLF4J để ghi log có level (DEBUG, INFO, ERROR) chuyên nghiệp hơn.',
    DATEADD(DAY,-17,GETDATE()), (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%debug chương trình Java%'), '43F5563E-CD98-49B8-8E5D-58B796480B5A');

-- Q3: Abstract Class vs Interface
INSERT INTO Answers (Content, CreatedAt, QuestionId, UserId) VALUES
(N'Abstract Class: dùng khi các class con có chung implementation (có thể kế thừa code). Interface: định nghĩa contract, ai implement phải có đủ method. Một class chỉ extends 1 abstract class nhưng implement được nhiều interface.',
    DATEADD(DAY,-17,GETDATE()), (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Abstract Class và Interface%'), 'D3C97F9B-DDFF-4179-9CEB-2D475BEB7AB5'),
(N'Nguyên tắc: dùng Interface khi các class không liên quan (Bird và Airplane đều implement Flyable). Dùng Abstract Class cho quan hệ is-a (Dog is-a Animal).',
    DATEADD(DAY,-16,GETDATE()), (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Abstract Class và Interface%'), 'db9ffa33-93ef-41a7-8dbf-649a65cdb761');

-- Q6: JOIN trong SQL
INSERT INTO Answers (Content, CreatedAt, QuestionId, UserId) VALUES
(N'INNER JOIN: chỉ lấy hàng khớp ở cả 2 bảng. LEFT JOIN: lấy tất cả bảng trái, bảng phải NULL nếu không khớp. RIGHT JOIN: ngược lại. Thực tế INNER và LEFT được dùng nhiều nhất.',
    DATEADD(DAY,-14,GETDATE()), (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%JOIN trong SQL%'), '12A84878-0A0E-42B2-B1DA-B90F60C13C0B'),
(N'Ví dụ LEFT JOIN: SELECT u.FullName, r.Reason FROM AspNetUsers u LEFT JOIN Reports r ON u.Id = r.TargetUserId -- lấy tất cả user kể cả chưa bị báo cáo, r.Reason sẽ NULL nếu chưa có report.',
    DATEADD(DAY,-13,GETDATE()), (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%JOIN trong SQL%'), 'db9ffa33-93ef-41a7-8dbf-649a65cdb761');

-- Q10: Overfitting
INSERT INTO Answers (Content, CreatedAt, QuestionId, UserId) VALUES
(N'Overfitting: model học thuộc training data nhưng không generalize. Nhận biết: train accuracy cao, val accuracy thấp. Cách fix: tăng data, thêm Dropout, dùng L1/L2 regularization, giảm độ phức tạp model.',
    DATEADD(DAY,-10,GETDATE()), (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Overfitting%'), 'DE34B040-58A2-4762-AFA3-7300785A7310'),
(N'Early stopping rất hiệu quả: dừng training khi validation loss bắt đầu tăng. Ngoài ra dùng k-fold cross validation để đánh giá model chính xác hơn thay vì chỉ một tập val.',
    DATEADD(DAY,-9,GETDATE()),  (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Overfitting%'), 'db9ffa33-93ef-41a7-8dbf-649a65cdb761');

-- Q12: Design Pattern
INSERT INTO Answers (Content, CreatedAt, QuestionId, UserId) VALUES
(N'Singleton: đảm bảo chỉ 1 instance trong app. Factory: tạo object mà không cần biết class cụ thể. Observer: khi object thay đổi, tự notify tất cả subscriber. Design Pattern chia 3 nhóm: Creational, Structural, Behavioral.',
    DATEADD(DAY,-8,GETDATE()),  (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Design Pattern%'), '9CBEE061-3782-4281-81EE-EBAD6224124E'),
(N'Học Design Pattern trên refactoring.guru rất dễ hiểu, có ví dụ bằng nhiều ngôn ngữ. Trong ASP.NET Core bạn đang dùng Repository Pattern và Dependency Injection rồi đó.',
    DATEADD(DAY,-7,GETDATE()),  (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Design Pattern%'), 'db9ffa33-93ef-41a7-8dbf-649a65cdb761');

-- Q14: SQL Injection
INSERT INTO Answers (Content, CreatedAt, QuestionId, UserId) VALUES
(N'SQL Injection: attacker chèn SQL vào input để thao túng DB. Ví dụ: nhập '' OR ''1''=''1 để bypass login. Phòng chống: dùng Parameterized Queries hoặc ORM như EF Core tự động xử lý.',
    DATEADD(DAY,-6,GETDATE()),  (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%SQL Injection%'), '26AE5E18-F3E0-4E49-A1E6-41F15D6DDA1B'),
(N'Trong EF Core: .Where(u => u.Email == email) tự động parameterize an toàn. Nếu cần raw SQL thì dùng FromSqlRaw("SELECT * FROM Users WHERE Email = {0}", email) chứ không bao giờ nối chuỗi.',
    DATEADD(DAY,-5,GETDATE()),  (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%SQL Injection%'), 'db9ffa33-93ef-41a7-8dbf-649a65cdb761');

-- Q16: Dependency Injection
INSERT INTO Answers (Content, CreatedAt, QuestionId, UserId) VALUES
(N'DI là pattern giúp class không tự tạo dependency mà nhận từ bên ngoài. Trong ASP.NET Core đăng ký bằng builder.Services.AddScoped/Transient/Singleton rồi inject qua constructor. Giúp code dễ test và mở rộng hơn.',
    DATEADD(DAY,-4,GETDATE()),  (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Dependency Injection%'), '38EECF7B-AFFE-4265-848E-53CF1B8704D4'),
(N'AddScoped: tạo mới mỗi HTTP request. AddTransient: tạo mới mỗi lần inject. AddSingleton: chỉ tạo 1 lần suốt vòng đời app. Repository và Service nên dùng AddScoped là chuẩn nhất.',
    DATEADD(DAY,-3,GETDATE()),  (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Dependency Injection%'), 'db9ffa33-93ef-41a7-8dbf-649a65cdb761');

-- Q20: Agile vs Waterfall
INSERT INTO Answers (Content, CreatedAt, QuestionId, UserId) VALUES
(N'Waterfall: làm tuần tự từng giai đoạn (Req → Design → Code → Test → Deploy), khó quay lại. Agile: chia nhỏ thành Sprint 2 tuần, liên tục deliver và nhận feedback. Project sinh viên nên dùng Agile vì requirement hay thay đổi.',
    DATEADD(DAY,-1,GETDATE()),  (SELECT TOP 1 Id FROM Questions WHERE Content LIKE N'%Agile và Waterfall%'), '101A09FA-D842-4023-A6D8-28CC6C87421C');
GO

-- ============================================================
-- REPORTS
-- ============================================================

-- Đã xử lý → hiện tab "Lịch sử xử lý"
INSERT INTO Reports (ReporterUserId, TargetUserId, Reason, IsResolved, ActionTaken, CreatedAt, QuestionId, AnswerId, DocumentId) VALUES
('12A84878-0A0E-42B2-B1DA-B90F60C13C0B', '5E25BD21-658A-415F-B308-6647AC9FCF7A',
 N'Người dùng đăng câu hỏi spam link quảng cáo không liên quan học tập.',
 1, N'Admin đã xóa bài vi phạm, trừ 10 điểm và cảnh cáo.', DATEADD(DAY,-18,GETDATE()), NULL, NULL, NULL),

('261548BF-27E5-46D2-AF3E-6678AC001850', 'A7AD996F-432C-4F6A-B5DB-5AB32A45217F',
 N'Bình luận chứa từ ngữ thô tục, xúc phạm người dùng khác trong thread.',
 1, N'Admin đã trừ 10 điểm và ghi nhận 1 lần cảnh cáo.', DATEADD(DAY,-14,GETDATE()), NULL, NULL, NULL),

('046F7B16-2038-4C2B-AF07-F63CD9B8CBF1', '9942230F-804A-4331-BB7E-2A8A7F53A125',
 N'Tài liệu đăng có nội dung copy từ nguồn khác, không ghi rõ bản quyền.',
 1, N'Admin đã bỏ qua báo cáo này.', DATEADD(DAY,-10,GETDATE()), NULL, NULL, NULL),

(NULL, '5E25BD21-658A-415F-B308-6647AC9FCF7A',
 N'[AI Phát hiện] Nội dung câu hỏi chứa từ ngữ không phù hợp với cộng đồng.',
 1, N'Hệ thống (AI) tự động phạt trừ 10 điểm và tăng 1 gậy cảnh cáo.', DATEADD(DAY,-8,GETDATE()), NULL, NULL, NULL),

('EC9709AA-DA08-4F54-8A20-0C4EF3261BC4', 'A7AD996F-432C-4F6A-B5DB-5AB32A45217F',
 N'Người dùng liên tục đăng câu hỏi trùng lặp, gây spam diễn đàn.',
 1, N'Admin đã xóa bài vi phạm, trừ 10 điểm và cảnh cáo.', DATEADD(DAY,-5,GETDATE()), NULL, NULL, NULL),

('F90DE63E-ABDC-4045-AB77-E16D51458DC0', '43F5563E-CD98-49B8-8E5D-58B796480B5A',
 N'Câu trả lời cố tình đưa thông tin sai lệch về thuật toán để đánh lừa người học.',
 1, N'Admin đã trừ 10 điểm và ghi nhận 1 lần cảnh cáo.', DATEADD(DAY,-3,GETDATE()), NULL, NULL, NULL),

(NULL, 'D3C97F9B-DDFF-4179-9CEB-2D475BEB7AB5',
 N'[AI Phát hiện] Nội dung bình luận có dấu hiệu quảng cáo dịch vụ bên ngoài.',
 1, N'Hệ thống (AI) tự động phạt trừ 10 điểm và tăng 1 gậy cảnh cáo.', DATEADD(DAY,-2,GETDATE()), NULL, NULL, NULL),

-- Đang chờ → hiện tab "Chờ xử lý"
('12A84878-0A0E-42B2-B1DA-B90F60C13C0B', '9942230F-804A-4331-BB7E-2A8A7F53A125',
 N'Tài liệu có nội dung sai kiến thức cơ bản về cấu trúc dữ liệu, gây hiểu nhầm cho sinh viên.',
 0, NULL, DATEADD(DAY,-2,GETDATE()), NULL, NULL, NULL),

('261548BF-27E5-46D2-AF3E-6678AC001850', '5E25BD21-658A-415F-B308-6647AC9FCF7A',
 N'Người dùng đăng câu hỏi không liên quan đến chủ đề học tập của trang web.',
 0, NULL, DATEADD(DAY,-1,GETDATE()), NULL, NULL, NULL),

('046F7B16-2038-4C2B-AF07-F63CD9B8CBF1', 'A7AD996F-432C-4F6A-B5DB-5AB32A45217F',
 N'Câu trả lời này đưa ra code mẫu bị lỗi nhưng lại khẳng định là đúng.',
 0, NULL, GETDATE(), NULL, NULL, NULL);
GO

-- ============================================================
-- KIỂM TRA KẾT QUẢ
-- ============================================================
SELECT N'Questions'            AS [Table], COUNT(*) AS [Count] FROM Questions
UNION ALL SELECT N'Answers',              COUNT(*) FROM Answers
UNION ALL SELECT N'Reports (Resolved)',   COUNT(*) FROM Reports WHERE IsResolved = 1
UNION ALL SELECT N'Reports (Pending)',    COUNT(*) FROM Reports WHERE IsResolved = 0;
GO
