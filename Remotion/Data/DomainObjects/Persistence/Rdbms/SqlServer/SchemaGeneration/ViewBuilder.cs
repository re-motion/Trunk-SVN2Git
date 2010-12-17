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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  public class ViewBuilder : ViewBuilderBase
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public ViewBuilder ()
    {
    }

    // methods and properties

    public override string CreateViewSeparator
    {
      get { return "GO\r\n\r\n"; }
    }

    public override void AddFilterViewToCreateViewScript (FilterViewDefinition filterViewDefinition, StringBuilder createViewStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("filterViewDefinition", filterViewDefinition);
      ArgumentUtility.CheckNotNull ("createViewStringBuilder", createViewStringBuilder);

      createViewStringBuilder.AppendFormat (
          "CREATE VIEW [{0}].[{1}] ({2})\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT {2}\r\n"
          + "    FROM [{0}].[{3}]\r\n"
          + "    WHERE [ClassID] IN ({4})\r\n"
          + "  WITH CHECK OPTION\r\n",
          FileBuilder.DefaultSchema,
          filterViewDefinition.ViewName,
          GetColumnList (filterViewDefinition.GetColumns()),
          filterViewDefinition.GetBaseTable().TableName,
          GetClassIDList (filterViewDefinition.ClassIDs));
    }

    public override void AddTableViewToCreateViewScript (
        TableDefinition tableDefinition,
        StringBuilder createViewStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);
      ArgumentUtility.CheckNotNull ("createViewStringBuilder", createViewStringBuilder);

      createViewStringBuilder.AppendFormat (
          "CREATE VIEW [{0}].[{1}] ({2})\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT {2}\r\n"
          + "    FROM [{0}].[{3}]\r\n"
          + "  WITH CHECK OPTION\r\n",
          FileBuilder.DefaultSchema,
          tableDefinition.ViewName,
          GetColumnList (tableDefinition.GetColumns()),
          tableDefinition.TableName);
    }

    public override void AddUnionViewToCreateViewScript (
        ClassDefinition classDefinition,
        ClassDefinitionCollection concreteClasses,
        StringBuilder createViewStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("concreteClasses", concreteClasses);
      ArgumentUtility.CheckNotNull ("createViewStringBuilder", createViewStringBuilder);

      var groupedPropertyDefinitions = GetGroupedPropertyDefinitions (classDefinition);

      createViewStringBuilder.AppendFormat (
          "CREATE VIEW [{0}].[{1}] ([ID], [ClassID], [Timestamp]{2})\r\n"
          + "  WITH SCHEMABINDING AS\r\n",
          FileBuilder.DefaultSchema,
          classDefinition.StorageEntityDefinition.LegacyViewName,
          GetColumnList (groupedPropertyDefinitions));

      int numberOfSelects = 0;
      foreach (ClassDefinition tableRootClass in concreteClasses)
      {
        if (numberOfSelects > 0)
          createViewStringBuilder.AppendFormat ("  UNION ALL\r\n");

        createViewStringBuilder.AppendFormat (
            "  SELECT [ID], [ClassID], [Timestamp]{0}\r\n"
            + "    FROM [{1}].[{2}]\r\n",
            GetColumnListForUnionSelect (tableRootClass, groupedPropertyDefinitions),
            FileBuilder.DefaultSchema,
            tableRootClass.StorageEntityDefinition.LegacyEntityName);

        numberOfSelects++;
      }
      if (numberOfSelects == 1)
        createViewStringBuilder.Append ("  WITH CHECK OPTION\r\n");
    }

    public override void AddToDropViewScript (ClassDefinition classDefinition, StringBuilder dropViewStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("dropViewStringBuilder", dropViewStringBuilder);

      dropViewStringBuilder.AppendFormat (
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = '{0}' AND TABLE_SCHEMA = '{1}')\r\n"
          + "  DROP VIEW [{1}].[{0}]\r\n",
          classDefinition.StorageEntityDefinition.LegacyViewName,
          FileBuilder.DefaultSchema);
    }

    private string GetColumnListForUnionSelect (ClassDefinition classDefinitionForUnionSelect, IEnumerable<IGrouping<string , PropertyDefinition>> groupedPropertyDefinitions)
    {
      StringBuilder stringBuilder = new StringBuilder();

      foreach (var propertyDefinitionGroup in groupedPropertyDefinitions)
      {
        var storagePropertyName = propertyDefinitionGroup.Key;

        if (propertyDefinitionGroup.Any (
                propertyDefinition => IsPartOfInheritanceBranch (classDefinitionForUnionSelect, propertyDefinition.ClassDefinition)))
        {
          stringBuilder.AppendFormat (", [{0}]", storagePropertyName);

          if (TableBuilder.HasClassIDColumn (propertyDefinitionGroup.First ()))
            stringBuilder.AppendFormat (", [{0}]", RdbmsProvider.GetClassIDColumnName (storagePropertyName));
        }
        else
        {
          stringBuilder.Append (", null");

          if (TableBuilder.HasClassIDColumn (propertyDefinitionGroup.First ()))
            stringBuilder.Append (", null");
        }
      }
      return stringBuilder.ToString();
    }

    private string GetColumnList (IEnumerable<IGrouping<string , PropertyDefinition>> groupedPropertyDefinitions)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (var propertyDefinitionGroup in groupedPropertyDefinitions)
      {
        var storagePropertyName = propertyDefinitionGroup.Key;
        var propertyDefinition = propertyDefinitionGroup.First ();

        stringBuilder.AppendFormat (", [{0}]", storagePropertyName);

        if (TableBuilder.HasClassIDColumn (propertyDefinition))
          stringBuilder.AppendFormat (", [{0}]", RdbmsProvider.GetClassIDColumnName (storagePropertyName));
      }
      return stringBuilder.ToString();
    }

    private string GetColumnList (IEnumerable<IColumnDefinition> columnDefinitions)
    {
      var visitor = new SqlNameListColumnDefinitionVisitor ();

      foreach (var columnDefinition in columnDefinitions)
        columnDefinition.Accept (visitor);

      return visitor.GetNameList ();
    }

    private string GetClassIDList (IEnumerable<string> classIDs)
    {
      return SeparatedStringBuilder.Build (", ", classIDs, id => "'" + id + "'");
    }
  }
}
