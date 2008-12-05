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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.RdbmsTools.SchemaGeneration.SqlServer
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

    public override void AddViewForConcreteClassToCreateViewScript (ClassDefinition classDefinition, StringBuilder createViewStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("createViewStringBuilder", createViewStringBuilder);

      createViewStringBuilder.AppendFormat (
          "CREATE VIEW [{0}].[{1}] ([ID], [ClassID], [Timestamp]{2})\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp]{2}\r\n"
          + "    FROM [{0}].[{3}]\r\n"
          + "    WHERE [ClassID] IN ({4})\r\n"
          + "  WITH CHECK OPTION\r\n",
          FileBuilder.DefaultSchema,
          GetViewName (classDefinition),
          GetColumnList (GetAllPropertyDefinitions (classDefinition)),
          classDefinition.GetEntityName(),
          GetClassIDList (GetClassDefinitionsForWhereClause (classDefinition)));
    }

    public override void AddViewForAbstractClassToCreateViewScript (
        ClassDefinition classDefinition,
        ClassDefinitionCollection concreteClasses,
        StringBuilder createViewStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("concreteClasses", concreteClasses);
      ArgumentUtility.CheckNotNull ("createViewStringBuilder", createViewStringBuilder);

      List<PropertyDefinition> allPropertyDefinitions = GetAllPropertyDefinitions (classDefinition);
      string classIDListForWhereClause = GetClassIDList (GetClassDefinitionsForWhereClause (classDefinition));

      createViewStringBuilder.AppendFormat (
          "CREATE VIEW [{0}].[{1}] ([ID], [ClassID], [Timestamp]{2})\r\n"
          + "  WITH SCHEMABINDING AS\r\n",
          FileBuilder.DefaultSchema,
          GetViewName (classDefinition),
          GetColumnList (allPropertyDefinitions));

      int numberOfSelects = 0;
      foreach (ClassDefinition tableRootClass in concreteClasses)
      {
        if (numberOfSelects > 0)
          createViewStringBuilder.AppendFormat ("  UNION ALL\r\n");

        createViewStringBuilder.AppendFormat (
            "  SELECT [ID], [ClassID], [Timestamp]{0}\r\n"
            + "    FROM [{1}].[{2}]\r\n"
            + "    WHERE [ClassID] IN ({3})\r\n",
            GetColumnListForUnionSelect (tableRootClass, allPropertyDefinitions),
            FileBuilder.DefaultSchema,
            tableRootClass.MyEntityName,
            classIDListForWhereClause);

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
          GetViewName (classDefinition),
          FileBuilder.DefaultSchema);
    }

    private string GetColumnListForUnionSelect (ClassDefinition classDefinitionForUnionSelect, List<PropertyDefinition> allPropertyDefinitions)
    {
      StringBuilder stringBuilder = new StringBuilder();

      foreach (PropertyDefinition propertyDefinition in allPropertyDefinitions)
      {
        if (IsPartOfInheritanceBranch (classDefinitionForUnionSelect, propertyDefinition.ClassDefinition))
        {
          stringBuilder.AppendFormat (", [{0}]", propertyDefinition.StorageSpecificName);

          if (TableBuilder.HasClassIDColumn (propertyDefinition))
            stringBuilder.AppendFormat (", [{0}]", RdbmsProvider.GetClassIDColumnName (propertyDefinition.StorageSpecificName));
        }
        else
        {
          stringBuilder.Append (", null");

          if (TableBuilder.HasClassIDColumn (propertyDefinition))
            stringBuilder.Append (", null");
        }
      }
      return stringBuilder.ToString();
    }

    private string GetColumnList (List<PropertyDefinition> propertyDefinitions)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (PropertyDefinition propertyDefinition in propertyDefinitions)
      {
        stringBuilder.AppendFormat (", [{0}]", propertyDefinition.StorageSpecificName);

        if (TableBuilder.HasClassIDColumn (propertyDefinition))
          stringBuilder.AppendFormat (", [{0}]", RdbmsProvider.GetClassIDColumnName (propertyDefinition.StorageSpecificName));
      }
      return stringBuilder.ToString();
    }

    private string GetClassIDList (ClassDefinitionCollection classDefinitionCollection)
    {
      StringBuilder classIDListBuilder = new StringBuilder();
      foreach (ClassDefinition classDefinition in classDefinitionCollection)
      {
        if (classIDListBuilder.Length != 0)
          classIDListBuilder.Append (", ");

        classIDListBuilder.AppendFormat ("'{0}'", classDefinition.ID);
      }
      return classIDListBuilder.ToString();
    }

    private string GetViewName (ClassDefinition classDefinition)
    {
      return classDefinition.ID + "View";
    }
  }
}
