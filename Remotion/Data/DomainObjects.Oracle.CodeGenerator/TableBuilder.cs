// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.CodeGenerator.Sql;
using log4net;

namespace Remotion.Data.DomainObjects.Oracle.CodeGenerator
{
  public class TableBuilder : TableBuilderBase
  {
    // types

    // static members and constants

    private const int c_tableNameMaximumLength = 25;
    private const int c_columnNameMaximumLength = 23;

    // member fields

    // construction and disposing

    public TableBuilder ()
    {
    }

    // methods and properties

    protected override string SqlDataTypeBoolean { get { return "number (1,0)"; } }
    protected override string SqlDataTypeByte { get { return "number (3,0)"; } }
    protected override string SqlDataTypeDate { get { return "timestamp"; } }
    protected override string SqlDataTypeDateTime { get { return "timestamp"; } }
    protected override string SqlDataTypeDecimal { get { return null; } }
    protected override string SqlDataTypeDouble { get { return "binary_double"; } }
    protected override string SqlDataTypeGuid { get { return "raw (16)"; } }
    protected override string SqlDataTypeInt16 { get { return "number (5,0)"; } }
    protected override string SqlDataTypeInt32 { get { return "number (9,0)"; } }
    protected override string SqlDataTypeInt64 { get { return "number (19,0)"; } }
    protected override string SqlDataTypeSingle { get { return "binary_double"; } }
    protected override string SqlDataTypeString { get { return "nvarchar2"; } }
    protected override string SqlDataTypeStringWithoutMaxLength { get { return null; } }
    protected override string SqlDataTypeBinary { get { return "blob"; } }
    protected override string SqlDataTypeObjectID { get { return "raw (16)"; } }
    protected override string SqlDataTypeSerializedObjectID { get { return "varchar2 (255)"; } }
    protected override string SqlDataTypeClassID { get { return "varchar2 (100)"; } }

    public override void AddToCreateTableScript (ClassDefinition classDefinition, StringBuilder createTableStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("createTableStringBuilder", createTableStringBuilder);

      if (classDefinition.MyEntityName.Length > c_tableNameMaximumLength)
      {
        LogUtility.LogWarning (string.Format ("The entity name of class '{0}' is too long ({1} characters). Maximum length: {2}", 
            classDefinition.ID, classDefinition.MyEntityName.Length, c_tableNameMaximumLength));
      }

      createTableStringBuilder.AppendFormat ("CREATE TABLE \"{0}\"\r\n"
          + "(\r\n"
          + "  \"ID\" raw (16) NOT NULL,\r\n"
          + "  \"ClassID\" varchar2 (100) NOT NULL,\r\n"
          + "  \"Timestamp\" number (9,0) DEFAULT 1 NOT NULL,\r\n\r\n"
          + "{1}  CONSTRAINT \"PK_{0}\" PRIMARY KEY (\"ID\")\r\n"
          + ");\r\n"
          + "-- timestamp trigger\r\n"
          + "CREATE TRIGGER \"{0}_ts\" BEFORE UPDATE ON \"{0}\" FOR EACH ROW\r\n"
          + "  BEGIN\r\n"
          + "    :NEW.\"Timestamp\" := :OLD.\"Timestamp\" + 1;\r\n"
          + "  END;\r\n",
          classDefinition.MyEntityName,
          GetColumnList (classDefinition));
    }

    public override void AddToDropTableScript (ClassDefinition classDefinition, StringBuilder dropTableStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("dropTableStringBuilder", dropTableStringBuilder);

      dropTableStringBuilder.AppendFormat ("DROP TABLE \"{0}\";\r\n",
          classDefinition.MyEntityName);
    }

    public override string GetColumn (PropertyDefinition propertyDefinition, bool forceNullable)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      if (propertyDefinition.ColumnName.Length > c_columnNameMaximumLength)
      {
        LogUtility.LogWarning (string.Format ("The column name '{0}' of class '{1}' is too long ({2} characters). Maximum length: {3}", 
            propertyDefinition.ColumnName, propertyDefinition.ClassDefinition.ID, propertyDefinition.ColumnName.Length, c_columnNameMaximumLength));
      }

      string nullable;
      if (propertyDefinition.IsNullable || forceNullable)
        nullable = " NULL";
      else
        nullable = " NOT NULL";

      return string.Format ("  \"{0}\" {1}{2}{3},\r\n{4}",
          propertyDefinition.ColumnName,
          GetSqlDataType (propertyDefinition),
          nullable,
          GetBooleanConstraint (propertyDefinition),
          GetClassIDColumn (propertyDefinition));
    }

    protected override string ColumnListOfParticularClassFormatString
    {
      get { return "  -- {0} columns\r\n{1}\r\n"; }
    }

    private string GetClassIDColumn (PropertyDefinition propertyDefinition)
    {
      if (!HasClassIDColumn (propertyDefinition))
        return string.Empty;

      return string.Format ("  \"{0}\" {1} NULL,\r\n", RdbmsProvider.GetClassIDColumnName (propertyDefinition.ColumnName), SqlDataTypeClassID);
    }

    private string GetBooleanConstraint (PropertyDefinition propertyDefinition)
    {
      if (propertyDefinition.MappingTypeName != "boolean")
        return string.Empty;

      return string.Format (" CONSTRAINT \"C_{0}_{1}\" CHECK (\"{2}\" BETWEEN 0 AND 1)",
          propertyDefinition.ClassDefinition.MyEntityName,
          GetIndex (propertyDefinition),
          propertyDefinition.ColumnName);
    }

    private int GetIndex (PropertyDefinition propertyDefinition)
    {
      PropertyDefinitionCollection propertyDefinitions = propertyDefinition.ClassDefinition.GetPropertyDefinitions ();
      for (int i = 0; i < propertyDefinitions.Count; i++)
      {
        if (propertyDefinitions[i] == propertyDefinition)
          return i;
      }

      return -1;
    }
  }
}
