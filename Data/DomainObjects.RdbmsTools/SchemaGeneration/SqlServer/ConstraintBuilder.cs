using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.RdbmsTools.SchemaGeneration.SqlServer
{
  public class ConstraintBuilder : ConstraintBuilderBase
  {
    public ConstraintBuilder ()
    {
    }

    public override void AddToDropConstraintScript (List<string> entityNamesForDropConstraintScript, StringBuilder dropConstraintStringBuilder)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("entityNamesForDropConstraintScript", entityNamesForDropConstraintScript);
      ArgumentUtility.CheckNotNull ("dropConstraintStringBuilder", dropConstraintStringBuilder);

      dropConstraintStringBuilder.AppendFormat (
          "DECLARE @statement nvarchar (4000)\r\n"
          + "SET @statement = ''\r\n"
          + "SELECT @statement = @statement + 'ALTER TABLE [{0}].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' \r\n"
          + "    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id \r\n"
          + "    WHERE fk.xtype = 'F' AND t.name IN ('{1}')\r\n"
          + "    ORDER BY t.name, fk.name\r\n"
          + "exec sp_executesql @statement\r\n",
          FileBuilder.DefaultSchema,
          string.Join ("', '", entityNamesForDropConstraintScript.ToArray()));
    }

    public override void AddToCreateConstraintScript (ClassDefinition classDefinition, StringBuilder createConstraintStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("createConstraintStringBuilder", createConstraintStringBuilder);

      string constraints = ConcatenateConstraints (GetConstraints (classDefinition));
      if (constraints.Length != 0)
      {
        createConstraintStringBuilder.AppendFormat (
            "ALTER TABLE [{0}].[{1}] ADD\r\n{2}\r\n",
            FileBuilder.DefaultSchema,
            classDefinition.MyEntityName,
            constraints);
      }
    }

    private string ConcatenateConstraints (List<string> constraints)
    {
      StringBuilder constraintBuilder = new StringBuilder();
      foreach (string constraint in constraints)
      {
        if (constraintBuilder.Length != 0)
          constraintBuilder.Append (ConstraintSeparator);

        constraintBuilder.Append (constraint);
      }

      return constraintBuilder.ToString();
    }

    public override string GetConstraint (
        ClassDefinition classDefinition, 
        IRelationEndPointDefinition relationEndPoint, 
        PropertyDefinition propertyDefinition, 
        ClassDefinition oppositeClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("oppositeClassDefinition", oppositeClassDefinition);

      return string.Format (
          "  CONSTRAINT [FK_{0}] FOREIGN KEY ([{1}]) REFERENCES [{2}].[{3}] ([ID])",
          classDefinition.GetEntityName () + "_" + propertyDefinition.StorageSpecificName,
          propertyDefinition.StorageSpecificName,
          FileBuilder.DefaultSchema,
          oppositeClassDefinition.GetEntityName());
    }

    protected override string ConstraintSeparator
    {
      get { return ",\r\n"; }
    }
  }
}