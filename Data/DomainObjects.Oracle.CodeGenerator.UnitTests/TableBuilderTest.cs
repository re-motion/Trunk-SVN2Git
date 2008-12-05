// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.NullableValueTypes;
using log4net.Appender;
using log4net.Config;
using log4net.Core;

namespace Remotion.Data.DomainObjects.Oracle.CodeGenerator.UnitTests
{
  [TestFixture]
  public class TableBuilderTest : MappingBaseTest
  {
    // types

    // static members and constants

    // member fields

    private TableBuilder _tableBuilder;

    // construction and disposing

    public TableBuilderTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _tableBuilder = new TableBuilder ();
    }

    [Test]
    public void GetSqlDataType ()
    {
      Assert.AreEqual ("number (1,0)", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "boolean", false, false, NaInt32.Null)));
      Assert.AreEqual ("number (3,0)", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "byte", false, false, NaInt32.Null)));
      Assert.AreEqual ("timestamp", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "date", false, false, NaInt32.Null)));
      Assert.AreEqual ("timestamp", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "dateTime", false, false, NaInt32.Null)));
      Assert.AreEqual ("binary_float", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "double", false, false, NaInt32.Null)));
      Assert.AreEqual ("raw (16)", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "guid", false, false, NaInt32.Null)));
      Assert.AreEqual ("number (5,0)", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "int16", false, false, NaInt32.Null)));
      Assert.AreEqual ("number (9,0)", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "int32", false, false, NaInt32.Null)));
      Assert.AreEqual ("number (19,0)", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "int64", false, false, NaInt32.Null)));
      Assert.AreEqual ("binary_double", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "single", false, false, NaInt32.Null)));
      Assert.AreEqual ("nvarchar2 (100)", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "string", false, false, 100)));
      Assert.AreEqual ("blob", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "binary", false, false, NaInt32.Null)));
      Assert.AreEqual ("raw (16)", _tableBuilder.GetSqlDataType (OrderItemClass.GetMandatoryPropertyDefinition ("Order")));
      Assert.AreEqual ("varchar2 (255)", _tableBuilder.GetSqlDataType (CustomerClass.GetMandatoryPropertyDefinition ("PrimaryOfficial")));
    }

    [Test]
    public void GetSqlDataType_WithDotNetType ()
    {
      string mappingTypeName = "Namespace.TypeName, AssemblyName";
      Assert.AreEqual ("number (9,0)", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", mappingTypeName, false, false, NaInt32.Null)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The data type 'decimal' cannot be mapped to a SQL data type.")]
    public void GetSqlDataType_WithDecimal_ThrowArgumentException ()
    {
      _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "decimal", false, false, NaInt32.Null));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The data type 'stringWithoutMaxLength' cannot be mapped to a SQL data type.")]
    public void GetSqlDataType_WithStringWithoutMaxLength_ThrowArgumentException ()
    {
      _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "string"));
    }

    [Test]
    public void AddToCreateTableScript ()
    {
      string expectedStatement = "CREATE TABLE \"Ceo\"\r\n"
          + "(\r\n"
          + "  \"ID\" raw (16) NOT NULL,\r\n"
          + "  \"ClassID\" varchar2 (100) NOT NULL,\r\n"
          + "  \"Timestamp\" number (9,0) DEFAULT 1 NOT NULL,\r\n\r\n"
          + "  -- Ceo columns\r\n"
          + "  \"Name\" nvarchar2 (100) NOT NULL,\r\n"
          + "  \"CompanyID\" raw (16) NULL,\r\n"
          + "  \"CompanyIDClassID\" varchar2 (100) NULL,\r\n\r\n"
          + "  CONSTRAINT \"PK_Ceo\" PRIMARY KEY (\"ID\")\r\n"
          + ");\r\n"
          + "-- timestamp trigger\r\n"
          + "CREATE TRIGGER \"Ceo_ts\" BEFORE UPDATE ON \"Ceo\" FOR EACH ROW\r\n"
          + "  BEGIN\r\n"
          + "    :NEW.\"Timestamp\" := :OLD.\"Timestamp\" + 1;\r\n"
          + "  END;\r\n";
      StringBuilder stringBuilder = new StringBuilder ();

      _tableBuilder.AddToCreateTableScript (CeoClass, stringBuilder);

      Assert.AreEqual (expectedStatement, stringBuilder.ToString ());
    }

    [Test]
    public void AddToCreateTableScriptForClassWithBoolean ()
    {
      string expectedStatement = "CREATE TABLE \"ClassWithBoolean\"\r\n"
          + "(\r\n"
          + "  \"ID\" raw (16) NOT NULL,\r\n"
          + "  \"ClassID\" varchar2 (100) NOT NULL,\r\n"
          + "  \"Timestamp\" number (9,0) DEFAULT 1 NOT NULL,\r\n\r\n"
          + "  -- ClassWithBoolean columns\r\n"
          + "  \"Boolean\" number (1,0) NOT NULL CONSTRAINT \"C_ClassWithBoolean_0\" CHECK (\"Boolean\" BETWEEN 0 AND 1),\r\n\r\n"
          + "  CONSTRAINT \"PK_ClassWithBoolean\" PRIMARY KEY (\"ID\")\r\n"
          + ");\r\n"
          + "-- timestamp trigger\r\n"
          + "CREATE TRIGGER \"ClassWithBoolean_ts\" BEFORE UPDATE ON \"ClassWithBoolean\" FOR EACH ROW\r\n"
          + "  BEGIN\r\n"
          + "    :NEW.\"Timestamp\" := :OLD.\"Timestamp\" + 1;\r\n"
          + "  END;\r\n";
      StringBuilder stringBuilder = new StringBuilder ();

      _tableBuilder.AddToCreateTableScript (MappingConfiguration.ClassDefinitions.GetMandatory ("ClassWithBoolean"), stringBuilder);

      Assert.AreEqual (expectedStatement, stringBuilder.ToString ());
    }

    [Test]
    public void AddToDropTableScript ()
    {
      string expectedScript = "DROP TABLE \"Customer\";\r\n";
      StringBuilder stringBuilder = new StringBuilder ();

      _tableBuilder.AddToDropTableScript (CustomerClass, stringBuilder);

      Assert.AreEqual (expectedScript, stringBuilder.ToString ());
    }

    [Test]
    public void AddToCreateTableScriptForClassWithEntityNameExceedsMaximumLength ()
    {
      MemoryAppender memoryAppender = new MemoryAppender ();
      BasicConfigurator.Configure (memoryAppender);

      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      ClassDefinition classDefinition = new ClassDefinition ("ClassWithVeryLongEntityName", "Entity_ClassWithVeryLongEntityName", "FirstStorageProvider", "ClassWithVeryLongEntityName", false);
      classes.Add (classDefinition);

      _tableBuilder.AddTables (classes);

      string expectedCreateTableScript = "CREATE TABLE \"Entity_ClassWithVeryLongEntityName\"\r\n"
          + "(\r\n"
          + "  \"ID\" raw (16) NOT NULL,\r\n"
          + "  \"ClassID\" varchar2 (100) NOT NULL,\r\n"
          + "  \"Timestamp\" number (9,0) DEFAULT 1 NOT NULL,\r\n\r\n"
          + "  -- ClassWithVeryLongEntityName columns\r\n\r\n"
          + "  CONSTRAINT \"PK_Entity_ClassWithVeryLongEntityName\" PRIMARY KEY (\"ID\")\r\n"
          + ");\r\n"
          + "-- timestamp trigger\r\n"
          + "CREATE TRIGGER \"Entity_ClassWithVeryLongEntityName_ts\" BEFORE UPDATE ON \"Entity_ClassWithVeryLongEntityName\" FOR EACH ROW\r\n"
          + "  BEGIN\r\n"
          + "    :NEW.\"Timestamp\" := :OLD.\"Timestamp\" + 1;\r\n"
          + "  END;\r\n";

      Assert.AreEqual (expectedCreateTableScript, _tableBuilder.GetCreateTableScript ());

      LoggingEvent[] events = memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      Assert.AreEqual (Level.Warn, events[0].Level);
      Assert.AreEqual ("The entity name of class 'ClassWithVeryLongEntityName' is too long (34 characters). Maximum length: 25", events[0].RenderedMessage);
    }

    [Test]
    public void AddToCreateTableScriptForClassWithColumnNameExceedsMaximumLength ()
    {
      MemoryAppender memoryAppender = new MemoryAppender ();
      BasicConfigurator.Configure (memoryAppender);

      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      classes.Add (CustomerClass);

      _tableBuilder.AddTables (classes);

      string expectedCreateTableScript = "CREATE TABLE \"Customer\"\r\n"
          + "(\r\n"
          + "  \"ID\" raw (16) NOT NULL,\r\n"
          + "  \"ClassID\" varchar2 (100) NOT NULL,\r\n"
          + "  \"Timestamp\" number (9,0) DEFAULT 1 NOT NULL,\r\n\r\n"
          + "  -- Company columns\r\n"
          + "  \"Name\" nvarchar2 (100) NOT NULL,\r\n"
          + "  \"PhoneNumber\" nvarchar2 (100) NULL,\r\n"
          + "  \"AddressID\" raw (16) NULL,\r\n\r\n"
          + "  -- Customer columns\r\n"
          + "  \"CustomerType\" number (9,0) NOT NULL,\r\n"
          + "  \"CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches\" nvarchar2 (100) NOT NULL,\r\n"
          + "  \"PrimaryOfficialID\" varchar2 (255) NULL,\r\n\r\n"
          + "  CONSTRAINT \"PK_Customer\" PRIMARY KEY (\"ID\")\r\n"
          + ");\r\n"
          + "-- timestamp trigger\r\n"
          + "CREATE TRIGGER \"Customer_ts\" BEFORE UPDATE ON \"Customer\" FOR EACH ROW\r\n"
          + "  BEGIN\r\n"
          + "    :NEW.\"Timestamp\" := :OLD.\"Timestamp\" + 1;\r\n"
          + "  END;\r\n";

      Assert.AreEqual (expectedCreateTableScript, _tableBuilder.GetCreateTableScript ());

      LoggingEvent[] events = memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      Assert.AreEqual (Level.Warn, events[0].Level);
      Assert.AreEqual ("The column name 'CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches' of class 'Customer' is too long (63 characters). Maximum length: 23", events[0].RenderedMessage);
    }

    [Test]
    public void IntegrationTest ()
    {
      MemoryAppender memoryAppender = new MemoryAppender ();
      BasicConfigurator.Configure (memoryAppender);

      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      classes.Add (CustomerClass);
      classes.Add (OrderClass);

      _tableBuilder.AddTables (classes);

      string expectedCreateTableScript = "CREATE TABLE \"Customer\"\r\n"
          + "(\r\n"
          + "  \"ID\" raw (16) NOT NULL,\r\n"
          + "  \"ClassID\" varchar2 (100) NOT NULL,\r\n"
          + "  \"Timestamp\" number (9,0) DEFAULT 1 NOT NULL,\r\n\r\n"
          + "  -- Company columns\r\n"
          + "  \"Name\" nvarchar2 (100) NOT NULL,\r\n"
          + "  \"PhoneNumber\" nvarchar2 (100) NULL,\r\n"
          + "  \"AddressID\" raw (16) NULL,\r\n\r\n"
          + "  -- Customer columns\r\n"
          + "  \"CustomerType\" number (9,0) NOT NULL,\r\n"
          + "  \"CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches\" nvarchar2 (100) NOT NULL,\r\n"
          + "  \"PrimaryOfficialID\" varchar2 (255) NULL,\r\n\r\n"
          + "  CONSTRAINT \"PK_Customer\" PRIMARY KEY (\"ID\")\r\n"
          + ");\r\n"
          + "-- timestamp trigger\r\n"
          + "CREATE TRIGGER \"Customer_ts\" BEFORE UPDATE ON \"Customer\" FOR EACH ROW\r\n"
          + "  BEGIN\r\n"
          + "    :NEW.\"Timestamp\" := :OLD.\"Timestamp\" + 1;\r\n"
          + "  END;\r\n\r\n"
          + "CREATE TABLE \"Order\"\r\n"
          + "(\r\n"
          + "  \"ID\" raw (16) NOT NULL,\r\n"
          + "  \"ClassID\" varchar2 (100) NOT NULL,\r\n"
          + "  \"Timestamp\" number (9,0) DEFAULT 1 NOT NULL,\r\n\r\n"
          + "  -- Order columns\r\n"
          + "  \"Number\" number (9,0) NOT NULL,\r\n"
          + "  \"Priority\" number (9,0) NOT NULL,\r\n"
          + "  \"CustomerID\" raw (16) NULL,\r\n"
          + "  \"CustomerIDClassID\" varchar2 (100) NULL,\r\n"
          + "  \"OfficialID\" varchar2 (255) NULL,\r\n\r\n"
          + "  CONSTRAINT \"PK_Order\" PRIMARY KEY (\"ID\")\r\n"
          + ");\r\n"
          + "-- timestamp trigger\r\n"
          + "CREATE TRIGGER \"Order_ts\" BEFORE UPDATE ON \"Order\" FOR EACH ROW\r\n"
          + "  BEGIN\r\n"
          + "    :NEW.\"Timestamp\" := :OLD.\"Timestamp\" + 1;\r\n"
          + "  END;\r\n";

      Assert.AreEqual (expectedCreateTableScript, _tableBuilder.GetCreateTableScript ());

      string expectedDropTableScript = "DROP TABLE \"Customer\";\r\n\r\n"
          + "DROP TABLE \"Order\";\r\n";

      Assert.AreEqual (expectedDropTableScript, _tableBuilder.GetDropTableScript ());

      LoggingEvent[] events = memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      Assert.AreEqual (Level.Warn, events[0].Level);
      Assert.AreEqual ("The column name 'CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches' of class 'Customer' is too long (63 characters). Maximum length: 23", events[0].RenderedMessage);
    }
  }
}
