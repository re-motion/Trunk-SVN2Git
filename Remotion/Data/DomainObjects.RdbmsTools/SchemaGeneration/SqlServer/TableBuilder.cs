// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Text;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.RdbmsTools.SchemaGeneration.SqlServer
{
  public class TableBuilder : TableBuilderBase
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public TableBuilder ()
    {
    }

    // methods and properties

    public override string GetSqlDataType (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      var storageProviderFinder = new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);
      return new SqlStorageTypeCalculator ().GetStorageType (propertyDefinition, storageProviderFinder);
    }

    protected override string SqlDataTypeClassID
    {
      get { return "varchar (100)"; }
    }

    public override void AddToCreateTableScript (ClassDefinition classDefinition, StringBuilder createTableStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("createTableStringBuilder", createTableStringBuilder);

      createTableStringBuilder.AppendFormat (
          "CREATE TABLE [{0}].[{1}]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar (100) NOT NULL,\r\n"
          + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
          + "{2}  CONSTRAINT [PK_{1}] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n",
          FileBuilder.DefaultSchema,
          classDefinition.StorageEntityDefinition.LegacyEntityName,
          GetColumnList (classDefinition));
    }

    public override void AddToDropTableScript (ClassDefinition classDefinition, StringBuilder dropTableStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("dropTableStringBuilder", dropTableStringBuilder);

      dropTableStringBuilder.AppendFormat (
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = '{0}' AND TABLE_SCHEMA = '{1}')\r\n"
          + "  DROP TABLE [{1}].[{0}]\r\n",
          classDefinition.StorageEntityDefinition.LegacyEntityName,
          FileBuilder.DefaultSchema);
    }

    public override string GetColumn (PropertyDefinition propertyDefinition, bool forceNullable)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      string nullable;
      if (propertyDefinition.IsNullable || forceNullable)
        nullable = " NULL";
      else
        nullable = " NOT NULL";

      return string.Format (
          "  [{0}] {1}{2},\r\n{3}",
          propertyDefinition.StoragePropertyDefinition.Name,
          GetSqlDataType (propertyDefinition),
          nullable,
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

      return string.Format ("  [{0}] {1} NULL,\r\n", RdbmsProvider.GetClassIDColumnName (propertyDefinition.StoragePropertyDefinition.Name), SqlDataTypeClassID);
    }
  }
}
