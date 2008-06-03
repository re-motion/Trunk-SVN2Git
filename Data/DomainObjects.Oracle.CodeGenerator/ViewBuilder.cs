/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.CodeGenerator.Sql;

namespace Remotion.Data.DomainObjects.Oracle.CodeGenerator
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
      get { return "\r\n"; }
    }

    public override void AddViewForConcreteClassToCreateViewScript (ClassDefinition classDefinition, StringBuilder createViewStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("createViewStringBuilder", createViewStringBuilder); 

      createViewStringBuilder.AppendFormat (
          "CREATE VIEW \"{0}\" (\"ID\", \"ClassID\", \"Timestamp\", {1})\r\n"
          + "  AS\r\n"
          + "  SELECT \"ID\", \"ClassID\", \"Timestamp\", {1}\r\n"
          + "    FROM \"{2}\"\r\n"
          + "    WHERE \"ClassID\" IN ({3})\r\n"
          + "  WITH CHECK OPTION;\r\n",
          GetViewName (classDefinition),
          GetColumnList (GetAllPropertyDefinitions (classDefinition)),
          classDefinition.GetEntityName (),
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
          "CREATE VIEW \"{0}\" (\"ID\", \"ClassID\", \"Timestamp\", {1})\r\n"
          + "  AS\r\n",
          GetViewName (classDefinition),
          GetColumnList (allPropertyDefinitions));

      int numberOfSelects = 0;
      foreach (ClassDefinition tableRootClass in concreteClasses)
      {
        if (numberOfSelects > 0)
          createViewStringBuilder.AppendFormat ("  UNION ALL\r\n");

        createViewStringBuilder.AppendFormat (
            "  SELECT \"ID\", \"ClassID\", \"Timestamp\", {0}\r\n"
            + "    FROM \"{1}\"\r\n"
            + "    WHERE \"ClassID\" IN ({2})\r\n",
            GetColumnListForUnionSelect (tableRootClass, allPropertyDefinitions),
            tableRootClass.MyEntityName,
            classIDListForWhereClause);

        numberOfSelects++;
      }
      if (numberOfSelects == 1)
        createViewStringBuilder.Append ("  WITH CHECK OPTION;\r\n");
      else
        createViewStringBuilder.Insert (createViewStringBuilder.Length - 2, ';');
    }

    public override void AddToDropViewScript (ClassDefinition classDefinition, StringBuilder dropViewStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("dropViewStringBuilder", dropViewStringBuilder);

      dropViewStringBuilder.AppendFormat ("DROP VIEW \"{0}\";\r\n",
          GetViewName (classDefinition));
    }

    private string GetColumnListForUnionSelect (ClassDefinition classDefinitionForUnionSelect, List<PropertyDefinition> allPropertyDefinitions)
    {
      StringBuilder stringBuilder = new StringBuilder ();

      foreach (PropertyDefinition propertyDefinition in allPropertyDefinitions)
      {
        if (stringBuilder.Length != 0)
          stringBuilder.Append (", ");

        if (IsPartOfInheritanceBranch (classDefinitionForUnionSelect, propertyDefinition.ClassDefinition))
        {
          stringBuilder.AppendFormat ("\"{0}\"", propertyDefinition.ColumnName);

          if (TableBuilder.HasClassIDColumn (propertyDefinition))
            stringBuilder.AppendFormat (", \"{0}\"", RdbmsProvider.GetClassIDColumnName (propertyDefinition.ColumnName));
        }
        else
        {
          stringBuilder.Append ("null");

          if (TableBuilder.HasClassIDColumn (propertyDefinition))
            stringBuilder.Append (", null");
        }
      }
      return stringBuilder.ToString ();
    }

    private string GetColumnList (List<PropertyDefinition> propertyDefinitions)
    {
      StringBuilder stringBuilder = new StringBuilder ();
      foreach (PropertyDefinition propertyDefinition in propertyDefinitions)
      {
        if (stringBuilder.Length != 0)
          stringBuilder.Append (", ");

        stringBuilder.AppendFormat ("\"{0}\"", propertyDefinition.ColumnName);

        if (TableBuilder.HasClassIDColumn (propertyDefinition))
          stringBuilder.AppendFormat (", \"{0}\"", RdbmsProvider.GetClassIDColumnName (propertyDefinition.ColumnName));
      }
      return stringBuilder.ToString ();
    }

    private string GetClassIDList (ClassDefinitionCollection classDefinitionCollection)
    {
      StringBuilder classIDListBuilder = new StringBuilder ();
      foreach (ClassDefinition classDefinition in classDefinitionCollection)
      {
        if (classIDListBuilder.Length != 0)
          classIDListBuilder.Append (", ");

        classIDListBuilder.AppendFormat ("'{0}'", classDefinition.ID);
      }
      return classIDListBuilder.ToString ();
    }

    private string GetViewName (ClassDefinition classDefinition)
    {
      return classDefinition.ID + "View";
    }
  }
}
