-- Drop all views that will be created below
DROP VIEW "CompanyView";

DROP VIEW "CustomerView";

DROP VIEW "PartnerView";

DROP VIEW "DevelopmentPartnerView";

DROP VIEW "AddressView";

DROP VIEW "OrderView";

DROP VIEW "OrderItemView";

DROP VIEW "CeoView";

DROP VIEW "ClassWithBooleanView";

DROP VIEW "ClassWithAllDataTypesView";

DROP VIEW "EmployeeView";

DROP VIEW "ClassWithRelationsView";

DROP VIEW "ConcreteClassView";

DROP VIEW "DerivedClassView";

DROP VIEW "SecondDerivedClassView";

DROP VIEW "DerivedOfDerivedClassView";

-- Drop foreign keys of all tables that will be created below
DECLARE
  tableName varchar2 (200);
  constraintName varchar2 (200);
  CURSOR dropConstraintsCursor IS SELECT TABLE_NAME, CONSTRAINT_NAME
    FROM USER_CONSTRAINTS
    WHERE CONSTRAINT_TYPE = 'R' AND TABLE_NAME IN ('Customer', 'DevelopmentPartner', 'Address', 'Order', 'OrderItem', 'Ceo', 'ClassWithBoolean', 'TableWithAllDataTypes', 'Employee', 'ClassWithRelations', 'ConcreteClass')
    ORDER BY TABLE_NAME, CONSTRAINT_NAME;
BEGIN
  OPEN dropConstraintsCursor;
  LOOP
    FETCH dropConstraintsCursor INTO tableName, constraintName;
    EXIT WHEN dropConstraintsCursor%NOTFOUND;
    EXECUTE IMMEDIATE 'ALTER TABLE "' || tableName || '" DROP CONSTRAINT "' || constraintName || '"';
  END LOOP;
  CLOSE dropConstraintsCursor;
END;

-- Drop all tables that will be created below
DROP TABLE "Customer";

DROP TABLE "DevelopmentPartner";

DROP TABLE "Address";

DROP TABLE "Order";

DROP TABLE "OrderItem";

DROP TABLE "Ceo";

DROP TABLE "ClassWithBoolean";

DROP TABLE "TableWithAllDataTypes";

DROP TABLE "Employee";

DROP TABLE "ClassWithRelations";

DROP TABLE "ConcreteClass";

-- Create all tables
CREATE TABLE "Customer"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- Company columns
  "Name" nvarchar2 (100) NOT NULL,
  "PhoneNumber" nvarchar2 (100) NULL,
  "AddressID" raw (16) NULL,

  -- Customer columns
  "CustomerType" number (9,0) NOT NULL,
  "CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches" nvarchar2 (100) NOT NULL,
  "PrimaryOfficialID" varchar2 (255) NULL,

  CONSTRAINT "PK_Customer" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "Customer_ts" BEFORE UPDATE ON "Customer" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "DevelopmentPartner"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- Company columns
  "Name" nvarchar2 (100) NOT NULL,
  "PhoneNumber" nvarchar2 (100) NULL,
  "AddressID" raw (16) NULL,

  -- Partner columns
  "Description" nvarchar2 (255) NOT NULL,
  "PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches" nvarchar2 (100) NOT NULL,

  -- DevelopmentPartner columns
  "Competences" nvarchar2 (255) NOT NULL,

  CONSTRAINT "PK_DevelopmentPartner" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "DevelopmentPartner_ts" BEFORE UPDATE ON "DevelopmentPartner" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "Address"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- Address columns
  "Street" nvarchar2 (100) NOT NULL,
  "Zip" nvarchar2 (100) NOT NULL,
  "City" nvarchar2 (100) NOT NULL,

  CONSTRAINT "PK_Address" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "Address_ts" BEFORE UPDATE ON "Address" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "Order"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- Order columns
  "Number" number (9,0) NOT NULL,
  "Priority" number (9,0) NOT NULL,
  "CustomerID" raw (16) NULL,
  "CustomerIDClassID" varchar2 (100) NULL,
  "OfficialID" varchar2 (255) NULL,

  CONSTRAINT "PK_Order" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "Order_ts" BEFORE UPDATE ON "Order" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "OrderItem"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- OrderItem columns
  "Position" number (9,0) NOT NULL,
  "Product" nvarchar2 (100) NOT NULL,
  "OrderID" raw (16) NULL,

  CONSTRAINT "PK_OrderItem" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "OrderItem_ts" BEFORE UPDATE ON "OrderItem" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "Ceo"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- Ceo columns
  "Name" nvarchar2 (100) NOT NULL,
  "CompanyID" raw (16) NULL,
  "CompanyIDClassID" varchar2 (100) NULL,

  CONSTRAINT "PK_Ceo" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "Ceo_ts" BEFORE UPDATE ON "Ceo" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "ClassWithBoolean"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- ClassWithBoolean columns
  "Boolean" number (1,0) NOT NULL CONSTRAINT "C_ClassWithBoolean_0" CHECK ("Boolean" BETWEEN 0 AND 1),

  CONSTRAINT "PK_ClassWithBoolean" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "ClassWithBoolean_ts" BEFORE UPDATE ON "ClassWithBoolean" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "TableWithAllDataTypes"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- ClassWithAllDataTypes columns
  "Boolean" number (1,0) NOT NULL CONSTRAINT "C_TableWithAllDataTypes_0" CHECK ("Boolean" BETWEEN 0 AND 1),
  "Byte" number (3,0) NOT NULL,
  "Date" timestamp NOT NULL,
  "DateTime" timestamp NOT NULL,
  "Double" binary_float NOT NULL,
  "Enum" number (9,0) NOT NULL,
  "Guid" raw (16) NOT NULL,
  "Int16" number (5,0) NOT NULL,
  "Int32" number (9,0) NOT NULL,
  "Int64" number (19,0) NOT NULL,
  "Single" binary_double NOT NULL,
  "String" nvarchar2 (100) NOT NULL,
  "NaBoolean" number (1,0) NULL CONSTRAINT "C_TableWithAllDataTypes_12" CHECK ("NaBoolean" BETWEEN 0 AND 1),
  "NaByte" number (3,0) NULL,
  "NaDate" timestamp NULL,
  "NaDateTime" timestamp NULL,
  "NaDouble" binary_float NULL,
  "NaGuid" raw (16) NULL,
  "NaInt16" number (5,0) NULL,
  "NaInt32" number (9,0) NULL,
  "NaInt64" number (19,0) NULL,
  "NaSingle" binary_double NULL,
  "StringWithNullValue" nvarchar2 (100) NULL,
  "NaBooleanWithNullValue" number (1,0) NULL CONSTRAINT "C_TableWithAllDataTypes_23" CHECK ("NaBooleanWithNullValue" BETWEEN 0 AND 1),
  "NaByteWithNullValue" number (3,0) NULL,
  "NaDateWithNullValue" timestamp NULL,
  "NaDateTimeWithNullValue" timestamp NULL,
  "NaDoubleWithNullValue" binary_float NULL,
  "NaGuidWithNullValue" raw (16) NULL,
  "NaInt16WithNullValue" number (5,0) NULL,
  "NaInt32WithNullValue" number (9,0) NULL,
  "NaInt64WithNullValue" number (19,0) NULL,
  "NaSingleWithNullValue" binary_double NULL,

  CONSTRAINT "PK_TableWithAllDataTypes" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "TableWithAllDataTypes_ts" BEFORE UPDATE ON "TableWithAllDataTypes" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "Employee"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- Employee columns
  "Name" nvarchar2 (100) NOT NULL,
  "SupervisorID" raw (16) NULL,

  CONSTRAINT "PK_Employee" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "Employee_ts" BEFORE UPDATE ON "Employee" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "ClassWithRelations"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- ClassWithRelations columns
  "DerivedClassID" raw (16) NULL,
  "DerivedClassIDClassID" varchar2 (100) NULL,

  CONSTRAINT "PK_ClassWithRelations" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "ClassWithRelations_ts" BEFORE UPDATE ON "ClassWithRelations" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "ConcreteClass"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- ConcreteClass columns
  "PropertyInConcreteClass" nvarchar2 (100) NOT NULL,

  -- DerivedClass columns
  "PropertyInDerivedClass" nvarchar2 (100) NULL,

  -- DerivedOfDerivedClass columns
  "PropertyInDerivedOfDerivedClass" nvarchar2 (100) NULL,
  "ClassWithRelationsInDerivedOfDerivedClassID" raw (16) NULL,

  -- SecondDerivedClass columns
  "PropertyInSecondDerivedClass" nvarchar2 (100) NULL,
  "ClassWithRelationsInSecondDerivedClassID" raw (16) NULL,

  CONSTRAINT "PK_ConcreteClass" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "ConcreteClass_ts" BEFORE UPDATE ON "ConcreteClass" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

-- Create constraints for tables that were created above
ALTER TABLE "Customer" ADD CONSTRAINT "FK_AddressToCompany" FOREIGN KEY ("AddressID") REFERENCES "Address" ("ID");

ALTER TABLE "DevelopmentPartner" ADD CONSTRAINT "FK_AddressToCompany_1" FOREIGN KEY ("AddressID") REFERENCES "Address" ("ID");

ALTER TABLE "Order" ADD CONSTRAINT "FK_CustomerToOrder" FOREIGN KEY ("CustomerID") REFERENCES "Customer" ("ID");

ALTER TABLE "OrderItem" ADD CONSTRAINT "FK_OrderToOrderItem" FOREIGN KEY ("OrderID") REFERENCES "Order" ("ID");

ALTER TABLE "Employee" ADD CONSTRAINT "FK_EmployeeToEmployee" FOREIGN KEY ("SupervisorID") REFERENCES "Employee" ("ID");

ALTER TABLE "ClassWithRelations" ADD CONSTRAINT "FK_DerivedClassToClassWithRelations" FOREIGN KEY ("DerivedClassID") REFERENCES "ConcreteClass" ("ID");

ALTER TABLE "ConcreteClass" ADD CONSTRAINT "FK_ClassWithRelationsToDerivedOfDerivedClass" FOREIGN KEY ("ClassWithRelationsInDerivedOfDerivedClassID") REFERENCES "ClassWithRelations" ("ID");
ALTER TABLE "ConcreteClass" ADD CONSTRAINT "FK_ClassWithRelationsToSecondDerivedClass" FOREIGN KEY ("ClassWithRelationsInSecondDerivedClassID") REFERENCES "ClassWithRelations" ("ID");

-- Create a view for every class
CREATE VIEW "CompanyView" ("ID", "ClassID", "Timestamp", "Name", "PhoneNumber", "AddressID", "CustomerType", "CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches", "PrimaryOfficialID", "Description", "PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches", "Competences")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Name", "PhoneNumber", "AddressID", "CustomerType", "CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches", "PrimaryOfficialID", null, null, null
    FROM "Customer"
    WHERE "ClassID" IN ('Customer', 'DevelopmentPartner')
  UNION ALL
  SELECT "ID", "ClassID", "Timestamp", "Name", "PhoneNumber", "AddressID", null, null, null, "Description", "PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches", "Competences"
    FROM "DevelopmentPartner"
    WHERE "ClassID" IN ('Customer', 'DevelopmentPartner');

CREATE VIEW "CustomerView" ("ID", "ClassID", "Timestamp", "Name", "PhoneNumber", "AddressID", "CustomerType", "CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches", "PrimaryOfficialID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Name", "PhoneNumber", "AddressID", "CustomerType", "CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches", "PrimaryOfficialID"
    FROM "Customer"
    WHERE "ClassID" IN ('Customer')
  WITH CHECK OPTION;

CREATE VIEW "PartnerView" ("ID", "ClassID", "Timestamp", "Name", "PhoneNumber", "AddressID", "Description", "PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches", "Competences")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Name", "PhoneNumber", "AddressID", "Description", "PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches", "Competences"
    FROM "DevelopmentPartner"
    WHERE "ClassID" IN ('DevelopmentPartner')
  WITH CHECK OPTION;

CREATE VIEW "DevelopmentPartnerView" ("ID", "ClassID", "Timestamp", "Name", "PhoneNumber", "AddressID", "Description", "PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches", "Competences")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Name", "PhoneNumber", "AddressID", "Description", "PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches", "Competences"
    FROM "DevelopmentPartner"
    WHERE "ClassID" IN ('DevelopmentPartner')
  WITH CHECK OPTION;

CREATE VIEW "AddressView" ("ID", "ClassID", "Timestamp", "Street", "Zip", "City")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Street", "Zip", "City"
    FROM "Address"
    WHERE "ClassID" IN ('Address')
  WITH CHECK OPTION;

CREATE VIEW "OrderView" ("ID", "ClassID", "Timestamp", "Number", "Priority", "CustomerID", "CustomerIDClassID", "OfficialID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Number", "Priority", "CustomerID", "CustomerIDClassID", "OfficialID"
    FROM "Order"
    WHERE "ClassID" IN ('Order')
  WITH CHECK OPTION;

CREATE VIEW "OrderItemView" ("ID", "ClassID", "Timestamp", "Position", "Product", "OrderID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Position", "Product", "OrderID"
    FROM "OrderItem"
    WHERE "ClassID" IN ('OrderItem')
  WITH CHECK OPTION;

CREATE VIEW "CeoView" ("ID", "ClassID", "Timestamp", "Name", "CompanyID", "CompanyIDClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Name", "CompanyID", "CompanyIDClassID"
    FROM "Ceo"
    WHERE "ClassID" IN ('Ceo')
  WITH CHECK OPTION;

CREATE VIEW "ClassWithBooleanView" ("ID", "ClassID", "Timestamp", "Boolean")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Boolean"
    FROM "ClassWithBoolean"
    WHERE "ClassID" IN ('ClassWithBoolean')
  WITH CHECK OPTION;

CREATE VIEW "ClassWithAllDataTypesView" ("ID", "ClassID", "Timestamp", "Boolean", "Byte", "Date", "DateTime", "Double", "Enum", "Guid", "Int16", "Int32", "Int64", "Single", "String", "NaBoolean", "NaByte", "NaDate", "NaDateTime", "NaDouble", "NaGuid", "NaInt16", "NaInt32", "NaInt64", "NaSingle", "StringWithNullValue", "NaBooleanWithNullValue", "NaByteWithNullValue", "NaDateWithNullValue", "NaDateTimeWithNullValue", "NaDoubleWithNullValue", "NaGuidWithNullValue", "NaInt16WithNullValue", "NaInt32WithNullValue", "NaInt64WithNullValue", "NaSingleWithNullValue")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Boolean", "Byte", "Date", "DateTime", "Double", "Enum", "Guid", "Int16", "Int32", "Int64", "Single", "String", "NaBoolean", "NaByte", "NaDate", "NaDateTime", "NaDouble", "NaGuid", "NaInt16", "NaInt32", "NaInt64", "NaSingle", "StringWithNullValue", "NaBooleanWithNullValue", "NaByteWithNullValue", "NaDateWithNullValue", "NaDateTimeWithNullValue", "NaDoubleWithNullValue", "NaGuidWithNullValue", "NaInt16WithNullValue", "NaInt32WithNullValue", "NaInt64WithNullValue", "NaSingleWithNullValue"
    FROM "TableWithAllDataTypes"
    WHERE "ClassID" IN ('ClassWithAllDataTypes')
  WITH CHECK OPTION;

CREATE VIEW "EmployeeView" ("ID", "ClassID", "Timestamp", "Name", "SupervisorID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Name", "SupervisorID"
    FROM "Employee"
    WHERE "ClassID" IN ('Employee')
  WITH CHECK OPTION;

CREATE VIEW "ClassWithRelationsView" ("ID", "ClassID", "Timestamp", "DerivedClassID", "DerivedClassIDClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "DerivedClassID", "DerivedClassIDClassID"
    FROM "ClassWithRelations"
    WHERE "ClassID" IN ('ClassWithRelations')
  WITH CHECK OPTION;

CREATE VIEW "ConcreteClassView" ("ID", "ClassID", "Timestamp", "PropertyInConcreteClass", "PropertyInDerivedClass", "PropertyInDerivedOfDerivedClass", "ClassWithRelationsInDerivedOfDerivedClassID", "PropertyInSecondDerivedClass", "ClassWithRelationsInSecondDerivedClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "PropertyInConcreteClass", "PropertyInDerivedClass", "PropertyInDerivedOfDerivedClass", "ClassWithRelationsInDerivedOfDerivedClassID", "PropertyInSecondDerivedClass", "ClassWithRelationsInSecondDerivedClassID"
    FROM "ConcreteClass"
    WHERE "ClassID" IN ('ConcreteClass', 'DerivedClass', 'DerivedOfDerivedClass', 'SecondDerivedClass')
  WITH CHECK OPTION;

CREATE VIEW "DerivedClassView" ("ID", "ClassID", "Timestamp", "PropertyInConcreteClass", "PropertyInDerivedClass", "PropertyInDerivedOfDerivedClass", "ClassWithRelationsInDerivedOfDerivedClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "PropertyInConcreteClass", "PropertyInDerivedClass", "PropertyInDerivedOfDerivedClass", "ClassWithRelationsInDerivedOfDerivedClassID"
    FROM "ConcreteClass"
    WHERE "ClassID" IN ('DerivedClass', 'DerivedOfDerivedClass')
  WITH CHECK OPTION;

CREATE VIEW "SecondDerivedClassView" ("ID", "ClassID", "Timestamp", "PropertyInConcreteClass", "PropertyInSecondDerivedClass", "ClassWithRelationsInSecondDerivedClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "PropertyInConcreteClass", "PropertyInSecondDerivedClass", "ClassWithRelationsInSecondDerivedClassID"
    FROM "ConcreteClass"
    WHERE "ClassID" IN ('SecondDerivedClass')
  WITH CHECK OPTION;

CREATE VIEW "DerivedOfDerivedClassView" ("ID", "ClassID", "Timestamp", "PropertyInConcreteClass", "PropertyInDerivedClass", "PropertyInDerivedOfDerivedClass", "ClassWithRelationsInDerivedOfDerivedClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "PropertyInConcreteClass", "PropertyInDerivedClass", "PropertyInDerivedOfDerivedClass", "ClassWithRelationsInDerivedOfDerivedClassID"
    FROM "ConcreteClass"
    WHERE "ClassID" IN ('DerivedOfDerivedClass')
  WITH CHECK OPTION;
