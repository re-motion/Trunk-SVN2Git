-- Drop all views that will be created below
DROP VIEW "OfficialView";

DROP VIEW "SpecialOfficialView";

-- Drop foreign keys of all tables that will be created below
DECLARE
  tableName varchar2 (200);
  constraintName varchar2 (200);
  CURSOR dropConstraintsCursor IS SELECT TABLE_NAME, CONSTRAINT_NAME
    FROM USER_CONSTRAINTS
    WHERE CONSTRAINT_TYPE = 'R' AND TABLE_NAME IN ('Official')
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
DROP TABLE "Official";

-- Create all tables
CREATE TABLE "Official"
(
  "ID" raw (16) NOT NULL,
  "ClassID" varchar2 (100) NOT NULL,
  "Timestamp" number (9,0) DEFAULT 1 NOT NULL,

  -- Official columns
  "Name" nvarchar2 (100) NOT NULL,
  "ResponsibleForOrderPriority" number (9,0) NOT NULL,
  "ResponsibleForCustomerType" number (9,0) NOT NULL,

  -- SpecialOfficial columns
  "Speciality" nvarchar2 (255) NULL,

  CONSTRAINT "PK_Official" PRIMARY KEY ("ID")
);
-- timestamp trigger
CREATE TRIGGER "Official_ts" BEFORE UPDATE ON "Official" FOR EACH ROW
  BEGIN
    :NEW."Timestamp" := :OLD."Timestamp" + 1;
  END;

-- Create constraints for tables that were created above

-- Create a view for every class
CREATE VIEW "OfficialView" ("ID", "ClassID", "Timestamp", "Name", "ResponsibleForOrderPriority", "ResponsibleForCustomerType", "Speciality")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Name", "ResponsibleForOrderPriority", "ResponsibleForCustomerType", "Speciality"
    FROM "Official"
    WHERE "ClassID" IN ('Official', 'SpecialOfficial')
  WITH CHECK OPTION;

CREATE VIEW "SpecialOfficialView" ("ID", "ClassID", "Timestamp", "Name", "ResponsibleForOrderPriority", "ResponsibleForCustomerType", "Speciality")
  AS
  SELECT "ID", "ClassID", "Timestamp", "Name", "ResponsibleForOrderPriority", "ResponsibleForCustomerType", "Speciality"
    FROM "Official"
    WHERE "ClassID" IN ('SpecialOfficial')
  WITH CHECK OPTION;
