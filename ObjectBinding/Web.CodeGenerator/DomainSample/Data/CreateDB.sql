CREATE DATABASE PhoneBook

ON PRIMARY (
	Name = 'PhoneBook_Data',
	Filename = 'C:\Databases\PhoneBook.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'PhoneBook_Log',
	Filename = 'C:\Databases\PhoneBook.ldf',
	Size = 10MB
)
GO

ALTER DATABASE PhoneBook SET RECOVERY SIMPLE
BACKUP LOG PhoneBook WITH TRUNCATE_ONLY
GO
