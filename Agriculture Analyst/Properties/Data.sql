INSERT INTO [Users]
(Username, DateOfBirth, Gender, Address, Name, Email, Phone, Password, AddedAt, IsActive)
VALUES
-- Admin
(N'admin01', '1990-05-12', N'Male', N'123 A Street, Hanoi', N'Nguyen Van Admin',
 N'admin@gmail.com', '0901234567',
 N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92',
 GETDATE(), 1),

-- Employee
(N'emp01', '1995-03-22', N'Female', N'45 B Street, HCMC', N'Tran Thi Employee',
 N'employee@gmail.com', '0912345678',
 N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92',
 GETDATE(), 1),

-- Customer
(N'cus01', '2000-11-01', N'Male', N'78 C Street, Da Nang', N'Le Van Customer',
 N'customer@gmail.com', '0923456789',
 N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92',
 GETDATE(), 1);
