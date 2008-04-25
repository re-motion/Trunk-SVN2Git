-- Drop all views that will be created below
DROP VIEW "RemidaUserView";

DROP VIEW "OrganizationalUnitView";

DROP VIEW "BaseObjectView";

DROP VIEW "SecuredObjectView";

DROP VIEW "ContentObjectView";

DROP VIEW "BaseObjectWithContentView";

DROP VIEW "ExcelReportView";

DROP VIEW "SearchObjectView";

DROP VIEW "SearchDomainObjectView";

DROP VIEW "SearchLegalRemedyView";

DROP VIEW "SettlementTypeView";

DROP VIEW "LegalRemedyView";

DROP VIEW "ClientView";

DROP VIEW "GroupView";

DROP VIEW "GroupTypeView";

DROP VIEW "GroupTypePositionView";

DROP VIEW "PositionView";

DROP VIEW "RoleView";

DROP VIEW "UserView";

DROP VIEW "MetadataObjectView";

DROP VIEW "EnumValueDefinitionView";

DROP VIEW "SecurableClassDefinitionView";

DROP VIEW "StatePropertyReferenceView";

DROP VIEW "StatePropertyDefinitionView";

DROP VIEW "StateDefinitionView";

DROP VIEW "AccessTypeReferenceView";

DROP VIEW "AccessTypeDefinitionView";

DROP VIEW "AbstractRoleDefinitionView";

DROP VIEW "StateCombinationView";

DROP VIEW "StateUsageView";

DROP VIEW "AccessControlListView";

DROP VIEW "AccessControlEntryView";

DROP VIEW "PermissionView";

DROP VIEW "CultureView";

DROP VIEW "LocalizedNameView";

-- Drop foreign keys of all tables that will be created below
DECLARE
  tableName varchar2 (200);
  constraintName varchar2 (200);
  CURSOR dropConstraintsCursor IS SELECT TABLE_NAME, CONSTRAINT_NAME
    FROM USER_CONSTRAINTS
    WHERE CONSTRAINT_TYPE = 'R' AND TABLE_NAME IN ('ContentObject', 'ExcelReport', 'SearchObject', 'SettlementType', 'LegalRemedy', 'Client', 'Group', 'GroupType', 'GroupTypePosition', 'Position', 'Role', 'User', 'EnumValueDefinition', 'SecurableClassDefinition', 'StatePropertyReference', 'StatePropertyDefinition', 'AccessTypeReference', 'StateCombination', 'StateUsage', 'AccessControlList', 'AccessControlEntry', 'Permission', 'Culture', 'LocalizedName')
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
DROP TABLE "ContentObject";

DROP TABLE "ExcelReport";

DROP TABLE "SearchObject";

DROP TABLE "SettlementType";

DROP TABLE "LegalRemedy";

DROP TABLE "Client";

DROP TABLE "Group";

DROP TABLE "GroupType";

DROP TABLE "GroupTypePosition";

DROP TABLE "Position";

DROP TABLE "Role";

DROP TABLE "User";

DROP TABLE "EnumValueDefinition";

DROP TABLE "SecurableClassDefinition";

DROP TABLE "StatePropertyReference";

DROP TABLE "StatePropertyDefinition";

DROP TABLE "AccessTypeReference";

DROP TABLE "StateCombination";

DROP TABLE "StateUsage";

DROP TABLE "AccessControlList";

DROP TABLE "AccessControlEntry";

DROP TABLE "Permission";

DROP TABLE "Culture";

DROP TABLE "LocalizedName";

-- Create all tables
CREATE TABLE "ContentObject"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- BaseObject columns
  "CreatedAt" timestamp NOT NULL,
  "ChangedAt" timestamp NOT NULL,
  "CreatedByID" raw (16) NULL,
  "CreatedByIDClassID" varchar2 (100) NULL,
  "ChangedByID" raw (16) NULL,
  "ChangedByIDClassID" varchar2 (100) NULL,

  -- SecuredObject columns

  -- ContentObject columns
  "ContentObjectIndex" number (9,0) NOT NULL,
  "Content" blob NOT NULL,
  "MimeType" nvarchar2 (254) NULL,
  "FileExtension" nvarchar2 (254) NOT NULL,
  "ProgID" nvarchar2 (254) NULL,
  "BaseObjectWithContentID" raw (16) NULL,
  "BaseObjectWithContentIDClassID" varchar2 (100) NULL,

  CONSTRAINT "PK_ContentObject" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "ContentObject_ts" BEFORE UPDATE ON "ContentObject" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "ExcelReport"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- BaseObject columns
  "CreatedAt" timestamp NOT NULL,
  "ChangedAt" timestamp NOT NULL,
  "CreatedByID" raw (16) NULL,
  "CreatedByIDClassID" varchar2 (100) NULL,
  "ChangedByID" raw (16) NULL,
  "ChangedByIDClassID" varchar2 (100) NULL,

  -- SecuredObject columns

  -- BaseObjectWithContent columns
  "Name" nvarchar2 (254) NOT NULL,

  -- ExcelReport columns
  "SearchObjectID" raw (16) NULL,
  "SearchObjectIDClassID" varchar2 (100) NULL,

  CONSTRAINT "PK_ExcelReport" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "ExcelReport_ts" BEFORE UPDATE ON "ExcelReport" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "SearchObject"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- BaseObject columns
  "CreatedAt" timestamp NOT NULL,
  "ChangedAt" timestamp NOT NULL,
  "CreatedByID" raw (16) NULL,
  "CreatedByIDClassID" varchar2 (100) NULL,
  "ChangedByID" raw (16) NULL,
  "ChangedByIDClassID" varchar2 (100) NULL,

  -- SecuredObject columns

  -- SearchObject columns
  "IsPublic" number (1,0) NOT NULL CONSTRAINT "SearchObject_IsPublic_Range" CHECK ("IsPublic" BETWEEN 0 AND 1),
  "UniqueIdentifier" nvarchar2 (100) NULL,
  "SearchObjectName" nvarchar2 (500) NOT NULL,
  "Parameters" nclob NULL,

  -- SearchDomainObject columns

  -- SearchLegalRemedy columns

  CONSTRAINT "PK_SearchObject" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "SearchObject_ts" BEFORE UPDATE ON "SearchObject" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "SettlementType"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- BaseObject columns
  "CreatedAt" timestamp NOT NULL,
  "ChangedAt" timestamp NOT NULL,
  "CreatedByID" raw (16) NULL,
  "CreatedByIDClassID" varchar2 (100) NULL,
  "ChangedByID" raw (16) NULL,
  "ChangedByIDClassID" varchar2 (100) NULL,

  -- SecuredObject columns

  -- SettlementType columns
  "SettlementTypeName" nvarchar2 (100) NOT NULL,
  "IsActive" number (1,0) NOT NULL CONSTRAINT "SettlementType_IsActive_Range" CHECK ("IsActive" BETWEEN 0 AND 1),

  CONSTRAINT "PK_SettlementType" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "SettlementType_ts" BEFORE UPDATE ON "SettlementType" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "LegalRemedy"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- BaseObject columns
  "CreatedAt" timestamp NOT NULL,
  "ChangedAt" timestamp NOT NULL,
  "CreatedByID" raw (16) NULL,
  "CreatedByIDClassID" varchar2 (100) NULL,
  "ChangedByID" raw (16) NULL,
  "ChangedByIDClassID" varchar2 (100) NULL,

  -- SecuredObject columns

  -- LegalRemedy columns
  "Number" number (9,0) NOT NULL,
  "IsFinished" number (1,0) NOT NULL CONSTRAINT "LR_1" CHECK ("IsFinished" BETWEEN 0 AND 1),
  "TaxableEntity" nvarchar2 (100) NOT NULL,
  "TaxNumber" number (9,0) NULL,
  "IncomingDate" timestamp NULL,
  "IncomingNumber" nvarchar2 (100) NULL,
  "SocialSecurityNumber" number (9,0) NULL,
  "Article" number (9,0) NULL,
  "Law" nvarchar2 (500) NULL,
  "TaxClasses" nvarchar2 (100) NULL,
  "AppealPeriod" nvarchar2 (100) NULL,
  "Subject" nvarchar2 (500) NULL,
  "SettlementDate" timestamp NULL,
  "IsCompanyAuditCase" number (1,0) NOT NULL CONSTRAINT "LR_2" CHECK ("IsCompanyAuditCase" BETWEEN 0 AND 1),
  "IsSuspendedByBAO212a" number (1,0) NOT NULL CONSTRAINT "LR_3" CHECK ("IsSuspendedByBAO212a" BETWEEN 0 AND 1),
  "IsSuspendedByBAO281" number (1,0) NOT NULL CONSTRAINT "LR_4" CHECK ("IsSuspendedByBAO281" BETWEEN 0 AND 1),
  "TransferredToDepartmentDate" timestamp NULL,
  "DepSettlementDate" timestamp NULL,
  "SubmittalRequestDate" timestamp NULL,
  "SubmittalDate" timestamp NULL,
  "FileSubmittalDate" timestamp NULL,
  "IsHearingRequested" number (1,0) NOT NULL CONSTRAINT "LR_5" CHECK ("IsHearingRequested" BETWEEN 0 AND 1),
  "OfficeRepresentative" nvarchar2 (100) NULL,
  "UfsEditor" nvarchar2 (100) NULL,
  "UfsSettlementDate" timestamp NULL,
  "VwGHComplaintDate" timestamp NULL,
  "VwGHSettlementDate" timestamp NULL,
  "Remark" nvarchar2 (1000) NULL,
  "SettlementTypeID" raw (16) NULL,
  "SettlementTypeIDClassID" varchar2 (100) NULL,
  "TeamID" raw (16) NULL,
  "TeamIDClassID" varchar2 (100) NULL,
  "EditorID" raw (16) NULL,
  "EditorIDClassID" varchar2 (100) NULL,
  "DepEditorID" raw (16) NULL,
  "DepEditorIDClassID" varchar2 (100) NULL,
  "DepSettlementTypeID" raw (16) NULL,
  "DepSettlementTypeIDClassID" varchar2 (100) NULL,
  "UfsSettlementTypeID" raw (16) NULL,
  "UfsSettlementTypeIDClassID" varchar2 (100) NULL,

  CONSTRAINT "PK_LegalRemedy" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "LegalRemedy_ts" BEFORE UPDATE ON "LegalRemedy" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "Client"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- Client columns
  "Name" nvarchar2 (100) NOT NULL,

  CONSTRAINT "PK_Client" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "Client_ts" BEFORE UPDATE ON "Client" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "Group"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- Group columns
  "Name" nvarchar2 (100) NOT NULL,
  "ShortName" nvarchar2 (10) NULL,
  "UniqueIdentifier" nvarchar2 (100) NOT NULL,
  "ClientID" raw (16) NULL,
  "ParentID" raw (16) NULL,
  "ParentIDClassID" varchar2 (100) NULL,
  "GroupTypeID" raw (16) NULL,

  -- OrganizationalUnit columns

  CONSTRAINT "PK_Group" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "Group_ts" BEFORE UPDATE ON "Group" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "GroupType"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- GroupType columns
  "Name" nvarchar2 (100) NOT NULL,

  CONSTRAINT "PK_GroupType" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "GroupType_ts" BEFORE UPDATE ON "GroupType" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "GroupTypePosition"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- GroupTypePosition columns
  "Name" nvarchar2 (100) NOT NULL,
  "GroupTypeID" raw (16) NULL,
  "PositionID" raw (16) NULL,

  CONSTRAINT "PK_GroupTypePosition" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "GroupTypePosition_ts" BEFORE UPDATE ON "GroupTypePosition" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "Position"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- Position columns
  "Name" nvarchar2 (100) NOT NULL,
  "Delegation" number (9,0) NOT NULL,

  CONSTRAINT "PK_Position" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "Position_ts" BEFORE UPDATE ON "Position" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "Role"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- Role columns
  "PositionID" raw (16) NULL,
  "GroupID" raw (16) NULL,
  "GroupIDClassID" varchar2 (100) NULL,
  "UserID" raw (16) NULL,
  "UserIDClassID" varchar2 (100) NULL,

  CONSTRAINT "PK_Role" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "Role_ts" BEFORE UPDATE ON "Role" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "User"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- User columns
  "Title" nvarchar2 (100) NULL,
  "FirstName" nvarchar2 (100) NULL,
  "LastName" nvarchar2 (100) NOT NULL,
  "UserName" nvarchar2 (100) NOT NULL,
  "ClientID" raw (16) NULL,
  "GroupID" raw (16) NULL,
  "GroupIDClassID" varchar2 (100) NULL,

  -- RemidaUser columns
  "IsActive" number (1,0) NULL CONSTRAINT "User_1" CHECK ("IsActive" BETWEEN 0 AND 1),
  "IsSecurityAdministrator" number (1,0) NULL CONSTRAINT "User_2" CHECK ("IsSecurityAdministrator" BETWEEN 0 AND 1),

  CONSTRAINT "PK_User" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "User_ts" BEFORE UPDATE ON "User" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "EnumValueDefinition"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- MetadataObject columns
  "Index" number (9,0) NOT NULL,
  "MetadataItemID" raw (16) NOT NULL,
  "Name" nvarchar2 (200) NOT NULL,

  -- EnumValueDefinition columns
  "Value" number (9,0) NOT NULL,

  -- StateDefinition columns
  "StatePropertyID" raw (16) NULL,
  "StatePropertyIDClassID" varchar2 (100) NULL,

  -- AccessTypeDefinition columns

  -- AbstractRoleDefinition columns

  CONSTRAINT "PK_EnumValueDefinition" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "EnumValueDefinition_ts" BEFORE UPDATE ON "EnumValueDefinition" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "SecurableClassDefinition"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- MetadataObject columns
  "Index" number (9,0) NOT NULL,
  "MetadataItemID" raw (16) NOT NULL,
  "Name" nvarchar2 (200) NOT NULL,

  -- SecurableClassDefinition columns
  "ChangedAt" timestamp NOT NULL,
  "BaseSecurableClassID" raw (16) NULL,
  "BaseSecurableClassIDClassID" varchar2 (100) NULL,

  CONSTRAINT "PK_SecurableClassDefinition" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "SecurableClassDefinition_ts" BEFORE UPDATE ON "SecurableClassDefinition" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "StatePropertyReference"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- StatePropertyReference columns
  "SecurableClassID" raw (16) NULL,
  "SecurableClassIDClassID" varchar2 (100) NULL,
  "StatePropertyID" raw (16) NULL,
  "StatePropertyIDClassID" varchar2 (100) NULL,

  CONSTRAINT "PK_StatePropertyReference" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "StatePropertyReference_ts" BEFORE UPDATE ON "StatePropertyReference" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "StatePropertyDefinition"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- MetadataObject columns
  "Index" number (9,0) NOT NULL,
  "MetadataItemID" raw (16) NOT NULL,
  "Name" nvarchar2 (200) NOT NULL,

  -- StatePropertyDefinition columns

  CONSTRAINT "PK_StatePropertyDefinition" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "StatePropertyDefinition_ts" BEFORE UPDATE ON "StatePropertyDefinition" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "AccessTypeReference"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- AccessTypeReference columns
  "Index" number (9,0) NOT NULL,
  "SecurableClassID" raw (16) NULL,
  "SecurableClassIDClassID" varchar2 (100) NULL,
  "AccessTypeID" raw (16) NULL,
  "AccessTypeIDClassID" varchar2 (100) NULL,

  CONSTRAINT "PK_AccessTypeReference" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "AccessTypeReference_ts" BEFORE UPDATE ON "AccessTypeReference" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "StateCombination"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- StateCombination columns
  "Index" number (9,0) NOT NULL,
  "SecurableClassID" raw (16) NULL,
  "SecurableClassIDClassID" varchar2 (100) NULL,
  "AccessControlListID" raw (16) NULL,

  CONSTRAINT "PK_StateCombination" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "StateCombination_ts" BEFORE UPDATE ON "StateCombination" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "StateUsage"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- StateUsage columns
  "StateCombinationID" raw (16) NULL,
  "StateDefinitionID" raw (16) NULL,
  "StateDefinitionIDClassID" varchar2 (100) NULL,

  CONSTRAINT "PK_StateUsage" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "StateUsage_ts" BEFORE UPDATE ON "StateUsage" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "AccessControlList"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- AccessControlList columns
  "ChangedAt" timestamp NOT NULL,
  "Index" number (9,0) NOT NULL,
  "SecurableClassID" raw (16) NULL,
  "SecurableClassIDClassID" varchar2 (100) NULL,

  CONSTRAINT "PK_AccessControlList" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "AccessControlList_ts" BEFORE UPDATE ON "AccessControlList" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "AccessControlEntry"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- AccessControlEntry columns
  "ChangedAt" timestamp NOT NULL,
  "Index" number (9,0) NOT NULL,
  "ClientSelection" number (9,0) NOT NULL,
  "GroupSelection" number (9,0) NOT NULL,
  "UserSelection" number (9,0) NOT NULL,
  "Priority" number (9,0) NULL,
  "AccessControlListID" raw (16) NULL,
  "GroupID" raw (16) NULL,
  "GroupIDClassID" varchar2 (100) NULL,
  "GroupTypeID" raw (16) NULL,
  "PositionID" raw (16) NULL,
  "UserID" raw (16) NULL,
  "UserIDClassID" varchar2 (100) NULL,
  "AbstractRoleID" raw (16) NULL,
  "AbstractRoleIDClassID" varchar2 (100) NULL,

  CONSTRAINT "PK_AccessControlEntry" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "AccessControlEntry_ts" BEFORE UPDATE ON "AccessControlEntry" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "Permission"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- Permission columns
  "Index" number (9,0) NOT NULL,
  "Allowed" number (1,0) NULL CONSTRAINT "Permission_Allowed_Range" CHECK ("Allowed" BETWEEN 0 AND 1),
  "AccessControlEntryID" raw (16) NULL,
  "AccessTypeDefinitionID" raw (16) NULL,
  "AccessTypeDefinitionIDClassID" varchar2 (100) NULL,

  CONSTRAINT "PK_Permission" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "Permission_ts" BEFORE UPDATE ON "Permission" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "Culture"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- Culture columns
  "CultureName" nvarchar2 (10) NOT NULL,

  CONSTRAINT "PK_Culture" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "Culture_ts" BEFORE UPDATE ON "Culture" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

CREATE TABLE "LocalizedName"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- LocalizedName columns
  "Text" nclob NOT NULL,
  "CultureID" raw (16) NULL,
  "MetadataObjectID" raw (16) NULL,
  "MetadataObjectIDClassID" varchar2 (100) NULL,

  CONSTRAINT "PK_LocalizedName" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "LocalizedName_ts" BEFORE UPDATE ON "LocalizedName" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

-- Create constraints for tables that were created above
ALTER TABLE "ContentObject" ADD CONSTRAINT "FK_BaseObjectToCreator" FOREIGN KEY ("CreatedByID") REFERENCES "User" ("ID");
ALTER TABLE "ContentObject" ADD CONSTRAINT "FK_BaseObjectToEditor" FOREIGN KEY ("ChangedByID") REFERENCES "User" ("ID");

ALTER TABLE "ExcelReport" ADD CONSTRAINT "FK_BaseObjectToCreator_1" FOREIGN KEY ("CreatedByID") REFERENCES "User" ("ID");
ALTER TABLE "ExcelReport" ADD CONSTRAINT "FK_BaseObjectToEditor_1" FOREIGN KEY ("ChangedByID") REFERENCES "User" ("ID");
ALTER TABLE "ExcelReport" ADD CONSTRAINT "FK_SearchObjectToExcelReport" FOREIGN KEY ("SearchObjectID") REFERENCES "SearchObject" ("ID");


-- Create a view for every class
CREATE VIEW "RemidaUserView" ("ID", "ClassID", "Timestamp", "Title", "FirstName", "LastName", "UserName", "ClientID", "GroupID", "GroupIDClassID", "IsActive", "IsSecurityAdministrator")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Title", "FirstName", "LastName", "UserName", "ClientID", "GroupID", "GroupIDClassID", "IsActive", "IsSecurityAdministrator"
    FROM "User"
    WHERE "ClassID" IN ('RemidaUser')
  WITH CHECK OPTION;

CREATE VIEW "OrganizationalUnitView" ("ID", "ClassID", "Timestamp", "Name", "ShortName", "UniqueIdentifier", "ClientID", "ParentID", "ParentIDClassID", "GroupTypeID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Name", "ShortName", "UniqueIdentifier", "ClientID", "ParentID", "ParentIDClassID", "GroupTypeID"
    FROM "Group"
    WHERE "ClassID" IN ('OrganizationalUnit')
  WITH CHECK OPTION;

CREATE VIEW "BaseObjectView" ("ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", "ContentObjectIndex", "Content", "MimeType", "FileExtension", "ProgID", "BaseObjectWithContentID", "BaseObjectWithContentIDClassID", "Name", "SearchObjectID", "SearchObjectIDClassID", "IsPublic", "UniqueIdentifier", "SearchObjectName", "Parameters", "SettlementTypeName", "IsActive", "Number", "IsFinished", "TaxableEntity", "TaxNumber", "IncomingDate", "IncomingNumber", "SocialSecurityNumber", "Article", "Law", "TaxClasses", "AppealPeriod", "Subject", "SettlementDate", "IsCompanyAuditCase", "IsSuspendedByBAO212a", "IsSuspendedByBAO281", "TransferredToDepartmentDate", "DepSettlementDate", "SubmittalRequestDate", "SubmittalDate", "FileSubmittalDate", "IsHearingRequested", "OfficeRepresentative", "UfsEditor", "UfsSettlementDate", "VwGHComplaintDate", "VwGHSettlementDate", "Remark", "SettlementTypeID", "SettlementTypeIDClassID", "TeamID", "TeamIDClassID", "EditorID", "EditorIDClassID", "DepEditorID", "DepEditorIDClassID", "DepSettlementTypeID", "DepSettlementTypeIDClassID", "UfsSettlementTypeID", "UfsSettlementTypeIDClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", "ContentObjectIndex", "Content", "MimeType", "FileExtension", "ProgID", "BaseObjectWithContentID", "BaseObjectWithContentIDClassID", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null
    FROM "ContentObject"
    WHERE "ClassID" IN ('ContentObject', 'ExcelReport', 'SearchObject', 'SearchDomainObject', 'SearchLegalRemedy', 'SettlementType', 'LegalRemedy')
  UNION ALL
  SELECT "ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", null, null, null, null, null, null, null, "Name", "SearchObjectID", "SearchObjectIDClassID", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null
    FROM "ExcelReport"
    WHERE "ClassID" IN ('ContentObject', 'ExcelReport', 'SearchObject', 'SearchDomainObject', 'SearchLegalRemedy', 'SettlementType', 'LegalRemedy')
  UNION ALL
  SELECT "ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", null, null, null, null, null, null, null, null, null, null, "IsPublic", "UniqueIdentifier", "SearchObjectName", "Parameters", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null
    FROM "SearchObject"
    WHERE "ClassID" IN ('ContentObject', 'ExcelReport', 'SearchObject', 'SearchDomainObject', 'SearchLegalRemedy', 'SettlementType', 'LegalRemedy')
  UNION ALL
  SELECT "ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", null, null, null, null, null, null, null, null, null, null, null, null, null, null, "SettlementTypeName", "IsActive", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null
    FROM "SettlementType"
    WHERE "ClassID" IN ('ContentObject', 'ExcelReport', 'SearchObject', 'SearchDomainObject', 'SearchLegalRemedy', 'SettlementType', 'LegalRemedy')
  UNION ALL
  SELECT "ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, "Number", "IsFinished", "TaxableEntity", "TaxNumber", "IncomingDate", "IncomingNumber", "SocialSecurityNumber", "Article", "Law", "TaxClasses", "AppealPeriod", "Subject", "SettlementDate", "IsCompanyAuditCase", "IsSuspendedByBAO212a", "IsSuspendedByBAO281", "TransferredToDepartmentDate", "DepSettlementDate", "SubmittalRequestDate", "SubmittalDate", "FileSubmittalDate", "IsHearingRequested", "OfficeRepresentative", "UfsEditor", "UfsSettlementDate", "VwGHComplaintDate", "VwGHSettlementDate", "Remark", "SettlementTypeID", "SettlementTypeIDClassID", "TeamID", "TeamIDClassID", "EditorID", "EditorIDClassID", "DepEditorID", "DepEditorIDClassID", "DepSettlementTypeID", "DepSettlementTypeIDClassID", "UfsSettlementTypeID", "UfsSettlementTypeIDClassID"
    FROM "LegalRemedy"
    WHERE "ClassID" IN ('ContentObject', 'ExcelReport', 'SearchObject', 'SearchDomainObject', 'SearchLegalRemedy', 'SettlementType', 'LegalRemedy');

CREATE VIEW "SecuredObjectView" ("ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", "ContentObjectIndex", "Content", "MimeType", "FileExtension", "ProgID", "BaseObjectWithContentID", "BaseObjectWithContentIDClassID", "Name", "SearchObjectID", "SearchObjectIDClassID", "IsPublic", "UniqueIdentifier", "SearchObjectName", "Parameters", "SettlementTypeName", "IsActive", "Number", "IsFinished", "TaxableEntity", "TaxNumber", "IncomingDate", "IncomingNumber", "SocialSecurityNumber", "Article", "Law", "TaxClasses", "AppealPeriod", "Subject", "SettlementDate", "IsCompanyAuditCase", "IsSuspendedByBAO212a", "IsSuspendedByBAO281", "TransferredToDepartmentDate", "DepSettlementDate", "SubmittalRequestDate", "SubmittalDate", "FileSubmittalDate", "IsHearingRequested", "OfficeRepresentative", "UfsEditor", "UfsSettlementDate", "VwGHComplaintDate", "VwGHSettlementDate", "Remark", "SettlementTypeID", "SettlementTypeIDClassID", "TeamID", "TeamIDClassID", "EditorID", "EditorIDClassID", "DepEditorID", "DepEditorIDClassID", "DepSettlementTypeID", "DepSettlementTypeIDClassID", "UfsSettlementTypeID", "UfsSettlementTypeIDClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", "ContentObjectIndex", "Content", "MimeType", "FileExtension", "ProgID", "BaseObjectWithContentID", "BaseObjectWithContentIDClassID", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null
    FROM "ContentObject"
    WHERE "ClassID" IN ('ContentObject', 'ExcelReport', 'SearchObject', 'SearchDomainObject', 'SearchLegalRemedy', 'SettlementType', 'LegalRemedy')
  UNION ALL
  SELECT "ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", null, null, null, null, null, null, null, "Name", "SearchObjectID", "SearchObjectIDClassID", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null
    FROM "ExcelReport"
    WHERE "ClassID" IN ('ContentObject', 'ExcelReport', 'SearchObject', 'SearchDomainObject', 'SearchLegalRemedy', 'SettlementType', 'LegalRemedy')
  UNION ALL
  SELECT "ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", null, null, null, null, null, null, null, null, null, null, "IsPublic", "UniqueIdentifier", "SearchObjectName", "Parameters", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null
    FROM "SearchObject"
    WHERE "ClassID" IN ('ContentObject', 'ExcelReport', 'SearchObject', 'SearchDomainObject', 'SearchLegalRemedy', 'SettlementType', 'LegalRemedy')
  UNION ALL
  SELECT "ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", null, null, null, null, null, null, null, null, null, null, null, null, null, null, "SettlementTypeName", "IsActive", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null
    FROM "SettlementType"
    WHERE "ClassID" IN ('ContentObject', 'ExcelReport', 'SearchObject', 'SearchDomainObject', 'SearchLegalRemedy', 'SettlementType', 'LegalRemedy')
  UNION ALL
  SELECT "ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, "Number", "IsFinished", "TaxableEntity", "TaxNumber", "IncomingDate", "IncomingNumber", "SocialSecurityNumber", "Article", "Law", "TaxClasses", "AppealPeriod", "Subject", "SettlementDate", "IsCompanyAuditCase", "IsSuspendedByBAO212a", "IsSuspendedByBAO281", "TransferredToDepartmentDate", "DepSettlementDate", "SubmittalRequestDate", "SubmittalDate", "FileSubmittalDate", "IsHearingRequested", "OfficeRepresentative", "UfsEditor", "UfsSettlementDate", "VwGHComplaintDate", "VwGHSettlementDate", "Remark", "SettlementTypeID", "SettlementTypeIDClassID", "TeamID", "TeamIDClassID", "EditorID", "EditorIDClassID", "DepEditorID", "DepEditorIDClassID", "DepSettlementTypeID", "DepSettlementTypeIDClassID", "UfsSettlementTypeID", "UfsSettlementTypeIDClassID"
    FROM "LegalRemedy"
    WHERE "ClassID" IN ('ContentObject', 'ExcelReport', 'SearchObject', 'SearchDomainObject', 'SearchLegalRemedy', 'SettlementType', 'LegalRemedy');

CREATE VIEW "ContentObjectView" ("ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", "ContentObjectIndex", "Content", "MimeType", "FileExtension", "ProgID", "BaseObjectWithContentID", "BaseObjectWithContentIDClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", "ContentObjectIndex", "Content", "MimeType", "FileExtension", "ProgID", "BaseObjectWithContentID", "BaseObjectWithContentIDClassID"
    FROM "ContentObject"
    WHERE "ClassID" IN ('ContentObject')
  WITH CHECK OPTION;

CREATE VIEW "BaseObjectWithContentView" ("ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", "Name", "SearchObjectID", "SearchObjectIDClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", "Name", "SearchObjectID", "SearchObjectIDClassID"
    FROM "ExcelReport"
    WHERE "ClassID" IN ('ExcelReport')
  WITH CHECK OPTION;

CREATE VIEW "ExcelReportView" ("ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", "Name", "SearchObjectID", "SearchObjectIDClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", "Name", "SearchObjectID", "SearchObjectIDClassID"
    FROM "ExcelReport"
    WHERE "ClassID" IN ('ExcelReport')
  WITH CHECK OPTION;

CREATE VIEW "SearchObjectView" ("ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", "IsPublic", "UniqueIdentifier", "SearchObjectName", "Parameters")
  AS
  SELECT "ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", "IsPublic", "UniqueIdentifier", "SearchObjectName", "Parameters"
    FROM "SearchObject"
    WHERE "ClassID" IN ('SearchObject', 'SearchDomainObject', 'SearchLegalRemedy')
  WITH CHECK OPTION;

CREATE VIEW "SearchDomainObjectView" ("ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", "IsPublic", "UniqueIdentifier", "SearchObjectName", "Parameters")
  AS
  SELECT "ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", "IsPublic", "UniqueIdentifier", "SearchObjectName", "Parameters"
    FROM "SearchObject"
    WHERE "ClassID" IN ('SearchDomainObject', 'SearchLegalRemedy')
  WITH CHECK OPTION;

CREATE VIEW "SearchLegalRemedyView" ("ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", "IsPublic", "UniqueIdentifier", "SearchObjectName", "Parameters")
  AS
  SELECT "ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", "IsPublic", "UniqueIdentifier", "SearchObjectName", "Parameters"
    FROM "SearchObject"
    WHERE "ClassID" IN ('SearchLegalRemedy')
  WITH CHECK OPTION;

CREATE VIEW "SettlementTypeView" ("ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", "SettlementTypeName", "IsActive")
  AS
  SELECT "ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", "SettlementTypeName", "IsActive"
    FROM "SettlementType"
    WHERE "ClassID" IN ('SettlementType')
  WITH CHECK OPTION;

CREATE VIEW "LegalRemedyView" ("ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", "Number", "IsFinished", "TaxableEntity", "TaxNumber", "IncomingDate", "IncomingNumber", "SocialSecurityNumber", "Article", "Law", "TaxClasses", "AppealPeriod", "Subject", "SettlementDate", "IsCompanyAuditCase", "IsSuspendedByBAO212a", "IsSuspendedByBAO281", "TransferredToDepartmentDate", "DepSettlementDate", "SubmittalRequestDate", "SubmittalDate", "FileSubmittalDate", "IsHearingRequested", "OfficeRepresentative", "UfsEditor", "UfsSettlementDate", "VwGHComplaintDate", "VwGHSettlementDate", "Remark", "SettlementTypeID", "SettlementTypeIDClassID", "TeamID", "TeamIDClassID", "EditorID", "EditorIDClassID", "DepEditorID", "DepEditorIDClassID", "DepSettlementTypeID", "DepSettlementTypeIDClassID", "UfsSettlementTypeID", "UfsSettlementTypeIDClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "CreatedAt", "ChangedAt", "CreatedByID", "CreatedByIDClassID", "ChangedByID", "ChangedByIDClassID", "Number", "IsFinished", "TaxableEntity", "TaxNumber", "IncomingDate", "IncomingNumber", "SocialSecurityNumber", "Article", "Law", "TaxClasses", "AppealPeriod", "Subject", "SettlementDate", "IsCompanyAuditCase", "IsSuspendedByBAO212a", "IsSuspendedByBAO281", "TransferredToDepartmentDate", "DepSettlementDate", "SubmittalRequestDate", "SubmittalDate", "FileSubmittalDate", "IsHearingRequested", "OfficeRepresentative", "UfsEditor", "UfsSettlementDate", "VwGHComplaintDate", "VwGHSettlementDate", "Remark", "SettlementTypeID", "SettlementTypeIDClassID", "TeamID", "TeamIDClassID", "EditorID", "EditorIDClassID", "DepEditorID", "DepEditorIDClassID", "DepSettlementTypeID", "DepSettlementTypeIDClassID", "UfsSettlementTypeID", "UfsSettlementTypeIDClassID"
    FROM "LegalRemedy"
    WHERE "ClassID" IN ('LegalRemedy')
  WITH CHECK OPTION;

CREATE VIEW "ClientView" ("ID", "ClassID", "Timestamp", "Name")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Name"
    FROM "Client"
    WHERE "ClassID" IN ('Client')
  WITH CHECK OPTION;

CREATE VIEW "GroupView" ("ID", "ClassID", "Timestamp", "Name", "ShortName", "UniqueIdentifier", "ClientID", "ParentID", "ParentIDClassID", "GroupTypeID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Name", "ShortName", "UniqueIdentifier", "ClientID", "ParentID", "ParentIDClassID", "GroupTypeID"
    FROM "Group"
    WHERE "ClassID" IN ('Group', 'OrganizationalUnit')
  WITH CHECK OPTION;

CREATE VIEW "GroupTypeView" ("ID", "ClassID", "Timestamp", "Name")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Name"
    FROM "GroupType"
    WHERE "ClassID" IN ('GroupType')
  WITH CHECK OPTION;

CREATE VIEW "GroupTypePositionView" ("ID", "ClassID", "Timestamp", "Name", "GroupTypeID", "PositionID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Name", "GroupTypeID", "PositionID"
    FROM "GroupTypePosition"
    WHERE "ClassID" IN ('GroupTypePosition')
  WITH CHECK OPTION;

CREATE VIEW "PositionView" ("ID", "ClassID", "Timestamp", "Name", "Delegation")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Name", "Delegation"
    FROM "Position"
    WHERE "ClassID" IN ('Position')
  WITH CHECK OPTION;

CREATE VIEW "RoleView" ("ID", "ClassID", "Timestamp", "PositionID", "GroupID", "GroupIDClassID", "UserID", "UserIDClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "PositionID", "GroupID", "GroupIDClassID", "UserID", "UserIDClassID"
    FROM "Role"
    WHERE "ClassID" IN ('Role')
  WITH CHECK OPTION;

CREATE VIEW "UserView" ("ID", "ClassID", "Timestamp", "Title", "FirstName", "LastName", "UserName", "ClientID", "GroupID", "GroupIDClassID", "IsActive", "IsSecurityAdministrator")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Title", "FirstName", "LastName", "UserName", "ClientID", "GroupID", "GroupIDClassID", "IsActive", "IsSecurityAdministrator"
    FROM "User"
    WHERE "ClassID" IN ('User', 'RemidaUser')
  WITH CHECK OPTION;

CREATE VIEW "MetadataObjectView" ("ID", "ClassID", "Timestamp", "Index", "MetadataItemID", "Name", "Value", "StatePropertyID", "StatePropertyIDClassID", "ChangedAt", "BaseSecurableClassID", "BaseSecurableClassIDClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Index", "MetadataItemID", "Name", "Value", "StatePropertyID", "StatePropertyIDClassID", null, null, null
    FROM "EnumValueDefinition"
    WHERE "ClassID" IN ('EnumValueDefinition', 'StateDefinition', 'AccessTypeDefinition', 'AbstractRoleDefinition', 'SecurableClassDefinition', 'StatePropertyDefinition')
  UNION ALL
  SELECT "ID", "ClassID", "Timestamp", "Index", "MetadataItemID", "Name", null, null, null, "ChangedAt", "BaseSecurableClassID", "BaseSecurableClassIDClassID"
    FROM "SecurableClassDefinition"
    WHERE "ClassID" IN ('EnumValueDefinition', 'StateDefinition', 'AccessTypeDefinition', 'AbstractRoleDefinition', 'SecurableClassDefinition', 'StatePropertyDefinition')
  UNION ALL
  SELECT "ID", "ClassID", "Timestamp", "Index", "MetadataItemID", "Name", null, null, null, null, null, null
    FROM "StatePropertyDefinition"
    WHERE "ClassID" IN ('EnumValueDefinition', 'StateDefinition', 'AccessTypeDefinition', 'AbstractRoleDefinition', 'SecurableClassDefinition', 'StatePropertyDefinition');

CREATE VIEW "EnumValueDefinitionView" ("ID", "ClassID", "Timestamp", "Index", "MetadataItemID", "Name", "Value", "StatePropertyID", "StatePropertyIDClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Index", "MetadataItemID", "Name", "Value", "StatePropertyID", "StatePropertyIDClassID"
    FROM "EnumValueDefinition"
    WHERE "ClassID" IN ('EnumValueDefinition', 'StateDefinition', 'AccessTypeDefinition', 'AbstractRoleDefinition')
  WITH CHECK OPTION;

CREATE VIEW "SecurableClassDefinitionView" ("ID", "ClassID", "Timestamp", "Index", "MetadataItemID", "Name", "ChangedAt", "BaseSecurableClassID", "BaseSecurableClassIDClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Index", "MetadataItemID", "Name", "ChangedAt", "BaseSecurableClassID", "BaseSecurableClassIDClassID"
    FROM "SecurableClassDefinition"
    WHERE "ClassID" IN ('SecurableClassDefinition')
  WITH CHECK OPTION;

CREATE VIEW "StatePropertyReferenceView" ("ID", "ClassID", "Timestamp", "SecurableClassID", "SecurableClassIDClassID", "StatePropertyID", "StatePropertyIDClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "SecurableClassID", "SecurableClassIDClassID", "StatePropertyID", "StatePropertyIDClassID"
    FROM "StatePropertyReference"
    WHERE "ClassID" IN ('StatePropertyReference')
  WITH CHECK OPTION;

CREATE VIEW "StatePropertyDefinitionView" ("ID", "ClassID", "Timestamp", "Index", "MetadataItemID", "Name")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Index", "MetadataItemID", "Name"
    FROM "StatePropertyDefinition"
    WHERE "ClassID" IN ('StatePropertyDefinition')
  WITH CHECK OPTION;

CREATE VIEW "StateDefinitionView" ("ID", "ClassID", "Timestamp", "Index", "MetadataItemID", "Name", "Value", "StatePropertyID", "StatePropertyIDClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Index", "MetadataItemID", "Name", "Value", "StatePropertyID", "StatePropertyIDClassID"
    FROM "EnumValueDefinition"
    WHERE "ClassID" IN ('StateDefinition')
  WITH CHECK OPTION;

CREATE VIEW "AccessTypeReferenceView" ("ID", "ClassID", "Timestamp", "Index", "SecurableClassID", "SecurableClassIDClassID", "AccessTypeID", "AccessTypeIDClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Index", "SecurableClassID", "SecurableClassIDClassID", "AccessTypeID", "AccessTypeIDClassID"
    FROM "AccessTypeReference"
    WHERE "ClassID" IN ('AccessTypeReference')
  WITH CHECK OPTION;

CREATE VIEW "AccessTypeDefinitionView" ("ID", "ClassID", "Timestamp", "Index", "MetadataItemID", "Name", "Value")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Index", "MetadataItemID", "Name", "Value"
    FROM "EnumValueDefinition"
    WHERE "ClassID" IN ('AccessTypeDefinition')
  WITH CHECK OPTION;

CREATE VIEW "AbstractRoleDefinitionView" ("ID", "ClassID", "Timestamp", "Index", "MetadataItemID", "Name", "Value")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Index", "MetadataItemID", "Name", "Value"
    FROM "EnumValueDefinition"
    WHERE "ClassID" IN ('AbstractRoleDefinition')
  WITH CHECK OPTION;

CREATE VIEW "StateCombinationView" ("ID", "ClassID", "Timestamp", "Index", "SecurableClassID", "SecurableClassIDClassID", "AccessControlListID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Index", "SecurableClassID", "SecurableClassIDClassID", "AccessControlListID"
    FROM "StateCombination"
    WHERE "ClassID" IN ('StateCombination')
  WITH CHECK OPTION;

CREATE VIEW "StateUsageView" ("ID", "ClassID", "Timestamp", "StateCombinationID", "StateDefinitionID", "StateDefinitionIDClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "StateCombinationID", "StateDefinitionID", "StateDefinitionIDClassID"
    FROM "StateUsage"
    WHERE "ClassID" IN ('StateUsage')
  WITH CHECK OPTION;

CREATE VIEW "AccessControlListView" ("ID", "ClassID", "Timestamp", "ChangedAt", "Index", "SecurableClassID", "SecurableClassIDClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "ChangedAt", "Index", "SecurableClassID", "SecurableClassIDClassID"
    FROM "AccessControlList"
    WHERE "ClassID" IN ('AccessControlList')
  WITH CHECK OPTION;

CREATE VIEW "AccessControlEntryView" ("ID", "ClassID", "Timestamp", "ChangedAt", "Index", "ClientSelection", "GroupSelection", "UserSelection", "Priority", "AccessControlListID", "GroupID", "GroupIDClassID", "GroupTypeID", "PositionID", "UserID", "UserIDClassID", "AbstractRoleID", "AbstractRoleIDClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "ChangedAt", "Index", "ClientSelection", "GroupSelection", "UserSelection", "Priority", "AccessControlListID", "GroupID", "GroupIDClassID", "GroupTypeID", "PositionID", "UserID", "UserIDClassID", "AbstractRoleID", "AbstractRoleIDClassID"
    FROM "AccessControlEntry"
    WHERE "ClassID" IN ('AccessControlEntry')
  WITH CHECK OPTION;

CREATE VIEW "PermissionView" ("ID", "ClassID", "Timestamp", "Index", "Allowed", "AccessControlEntryID", "AccessTypeDefinitionID", "AccessTypeDefinitionIDClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Index", "Allowed", "AccessControlEntryID", "AccessTypeDefinitionID", "AccessTypeDefinitionIDClassID"
    FROM "Permission"
    WHERE "ClassID" IN ('Permission')
  WITH CHECK OPTION;

CREATE VIEW "CultureView" ("ID", "ClassID", "Timestamp", "CultureName")
  AS
  SELECT "ID", "ClassID", "Timestamp", "CultureName"
    FROM "Culture"
    WHERE "ClassID" IN ('Culture')
  WITH CHECK OPTION;

CREATE VIEW "LocalizedNameView" ("ID", "ClassID", "Timestamp", "Text", "CultureID", "MetadataObjectID", "MetadataObjectIDClassID")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Text", "CultureID", "MetadataObjectID", "MetadataObjectIDClassID"
    FROM "LocalizedName"
    WHERE "ClassID" IN ('LocalizedName')
  WITH CHECK OPTION;
