USE PhoneBook
GO

delete from dbo.PhoneNumber
delete from dbo.Person
delete from dbo.Location

-- Location

INSERT INTO [PhoneBook].[dbo].[Location]([ID],[ClassID],[Name],[ZipCode],[City],[Street],[Country])
	VALUES('8d609b4d-8fbb-4ae5-a423-02b1b5099966','Location','Neunkirchen','2732','Neunkirchen','Neunkirchnerstrasse 12','1');
INSERT INTO [PhoneBook].[dbo].[Location]([ID],[ClassID],[Name],[ZipCode],[City],[Street],[Country])
	VALUES('ae6aa494-9433-4317-b2e3-04f28ae69f3f','Location','Hollenthon','2812','Hollenthon','Hollenthon 130','1');
INSERT INTO [PhoneBook].[dbo].[Location]([ID],[ClassID],[Name],[ZipCode],[City],[Street],[Country])
	VALUES('211b5335-4e8e-43ff-90de-cd0e40fd9d6b','Location','St. Pölten','3100','St. Pölten','Hauptstr. 234','1');
INSERT INTO [PhoneBook].[dbo].[Location]([ID],[ClassID],[Name],[ZipCode],[City],[Street],[Country])
	VALUES('285029ff-6b79-4651-98a2-d0246b5e0971','Location','Wien','1010','Wien','Wienerstrasse 121','1');

-- Person

INSERT INTO [PhoneBook].[dbo].[Person]([ID],[ClassID],[FirstName],[LastName],[EMailAddress],[Location])
	VALUES('a0df0c5a-8916-4cdd-a056-04452f5205b9','Person','Markus','Leimhofer',NULL,'285029ff-6b79-4651-98a2-d0246b5e0971');
INSERT INTO [PhoneBook].[dbo].[Person]([ID],[ClassID],[FirstName],[LastName],[EMailAddress],[Location])
	VALUES('50889e75-2ad2-4629-aee0-1c5ddf2db440','Person','David','Birnbauer',NULL,'ae6aa494-9433-4317-b2e3-04f28ae69f3f');
INSERT INTO [PhoneBook].[dbo].[Person]([ID],[ClassID],[FirstName],[LastName],[EMailAddress],[Location])
	VALUES('5540c259-d46c-4eaf-9c33-4439270aa08d','Person','Thomas','Kuhta',NULL,NULL);
INSERT INTO [PhoneBook].[dbo].[Person]([ID],[ClassID],[FirstName],[LastName],[EMailAddress],[Location])
	VALUES('7b7af782-c3f7-43be-9a58-ceb9c20a1330','Person','Werner','Kugler',NULL,'285029ff-6b79-4651-98a2-d0246b5e0971');
INSERT INTO [PhoneBook].[dbo].[Person]([ID],[ClassID],[FirstName],[LastName],[EMailAddress],[Location])
	VALUES('70c39207-ee7b-4638-ad4b-e8cb88688c71','Person','Hans-Peter','Kager',NULL,'285029ff-6b79-4651-98a2-d0246b5e0971');
INSERT INTO [PhoneBook].[dbo].[Person]([ID],[ClassID],[FirstName],[LastName],[EMailAddress],[Location])
	VALUES('7f73e2c5-67c0-4fc3-8c0e-ec7c84e0543b','Person','Stefan','Wenig',NULL,NULL);
INSERT INTO [PhoneBook].[dbo].[Person]([ID],[ClassID],[FirstName],[LastName],[EMailAddress],[Location])
	VALUES('00f4f29e-114d-40a7-b1d1-eccf008c4272','Person','Martin','Mader',NULL,'8d609b4d-8fbb-4ae5-a423-02b1b5099966');
INSERT INTO [PhoneBook].[dbo].[Person]([ID],[ClassID],[FirstName],[LastName],[EMailAddress],[Location])
	VALUES('0cdd32f5-c405-4653-acb6-f3f44ca1faf0','Person','Ernst','Scheithauer',NULL,'211b5335-4e8e-43ff-90de-cd0e40fd9d6b');

-- PhoneNumber

INSERT INTO [PhoneBook].[dbo].[PhoneNumber]([ID],[ClassID],[CountryCode],[AreaCode],[Number],[Extension],[Person])
	VALUES('8d211da6-b867-43f8-9ac7-3c99d4bc4fa3','PhoneNumber','43','02645','1234','0','50889e75-2ad2-4629-aee0-1c5ddf2db440');
INSERT INTO [PhoneBook].[dbo].[PhoneNumber]([ID],[ClassID],[CountryCode],[AreaCode],[Number],[Extension],[Person])
	VALUES('3ffef8bb-ab0c-4004-9033-54711ddd9f61','PhoneNumber','1','2','3','4','00f4f29e-114d-40a7-b1d1-eccf008c4272');
INSERT INTO [PhoneBook].[dbo].[PhoneNumber]([ID],[ClassID],[CountryCode],[AreaCode],[Number],[Extension],[Person])
	VALUES('78444c0d-8684-4e55-8506-63cc5520ca9e','PhoneNumber','12','34','56','78','7b7af782-c3f7-43be-9a58-ceb9c20a1330');
INSERT INTO [PhoneBook].[dbo].[PhoneNumber]([ID],[ClassID],[CountryCode],[AreaCode],[Number],[Extension],[Person])
	VALUES('0c391d5e-caed-4bb5-b524-76e66ffc44fe','PhoneNumber','43','123','456','2','a0df0c5a-8916-4cdd-a056-04452f5205b9');
INSERT INTO [PhoneBook].[dbo].[PhoneNumber]([ID],[ClassID],[CountryCode],[AreaCode],[Number],[Extension],[Person])
	VALUES('b4da8d69-aca7-4b62-8b4c-98e0f4780463','PhoneNumber','43','9876','5432','1','7f73e2c5-67c0-4fc3-8c0e-ec7c84e0543b');
INSERT INTO [PhoneBook].[dbo].[PhoneNumber]([ID],[ClassID],[CountryCode],[AreaCode],[Number],[Extension],[Person])
	VALUES('b5a1eec6-c370-489d-9eef-c4c419aebef9','PhoneNumber','43','0664','1234232',NULL,'50889e75-2ad2-4629-aee0-1c5ddf2db440');
INSERT INTO [PhoneBook].[dbo].[PhoneNumber]([ID],[ClassID],[CountryCode],[AreaCode],[Number],[Extension],[Person])
	VALUES('c124cce1-2320-4977-8e32-d162245fa0d9','PhoneNumber','34','07722','1232131231','8','0cdd32f5-c405-4653-acb6-f3f44ca1faf0');
INSERT INTO [PhoneBook].[dbo].[PhoneNumber]([ID],[ClassID],[CountryCode],[AreaCode],[Number],[Extension],[Person])
	VALUES('0bdb316c-eed3-439c-a3a9-d29ce709f192','PhoneNumber','12','34','56','78','70c39207-ee7b-4638-ad4b-e8cb88688c71');