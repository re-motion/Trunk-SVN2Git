using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using System.Collections;
using Remotion.Data.DomainObjects.CodeGenerator.Sql;

namespace Remotion.Data.DomainObjects.Oracle.CodeGenerator
{
  public class ConstraintBuilder : ConstraintBuilderBase
  {
    // types

    // static members and constants

    private const int c_relationNameMaximumLength = 25;

    // member fields

    private Hashtable _constraintNamesUsed;

    // construction and disposing

    public ConstraintBuilder ()
    {
      _constraintNamesUsed = new Hashtable ();
    }

    // methods and properties

    public override void AddToDropConstraintScript (List<string> entityNamesForDropConstraintScript, StringBuilder dropConstraintStringBuilder)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("entityNamesForDropConstraintScript", entityNamesForDropConstraintScript);
      ArgumentUtility.CheckNotNull ("dropConstraintStringBuilder", dropConstraintStringBuilder);

      dropConstraintStringBuilder.AppendFormat ("DECLARE\r\n"
          + "  tableName varchar2 (200);\r\n"
          + "  constraintName varchar2 (200);\r\n"
          + "  CURSOR dropConstraintsCursor IS SELECT TABLE_NAME, CONSTRAINT_NAME\r\n"
          + "    FROM USER_CONSTRAINTS\r\n"
          + "    WHERE CONSTRAINT_TYPE = 'R' AND TABLE_NAME IN ('{0}')\r\n"
          + "    ORDER BY TABLE_NAME, CONSTRAINT_NAME;\r\n"
          + "BEGIN\r\n"
          + "  OPEN dropConstraintsCursor;\r\n"
          + "  LOOP\r\n"
          + "    FETCH dropConstraintsCursor INTO tableName, constraintName;\r\n"
          + "    EXIT WHEN dropConstraintsCursor%NOTFOUND;\r\n"
          + "    EXECUTE IMMEDIATE 'ALTER TABLE \"' || tableName || '\" DROP CONSTRAINT \"' || constraintName || '\"';\r\n"
          + "  END LOOP;\r\n"
          + "  CLOSE dropConstraintsCursor;\r\n"
          + "END;\r\n",
         string.Join ("', '", entityNamesForDropConstraintScript.ToArray ()));
    }

    public override void AddToCreateConstraintScript (ClassDefinition classDefinition, StringBuilder createConstraintStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("createConstraintStringBuilder", createConstraintStringBuilder);

      foreach (string constraint in GetConstraints (classDefinition))
      {
        createConstraintStringBuilder.AppendFormat ("ALTER TABLE \"{0}\" ADD {1};\r\n",
            classDefinition.MyEntityName,
            constraint);
      }
    }

    public override string GetConstraint (IRelationEndPointDefinition relationEndPoint, PropertyDefinition propertyDefinition, ClassDefinition oppositeClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("oppositeClassDefinition", oppositeClassDefinition);

      if (relationEndPoint.RelationDefinition.ID.Length > c_relationNameMaximumLength)
      {
        LogUtility.LogWarning (string.Format ("The relation name '{0}' in class '{1}' is too long ({2} characters). Maximum length: {3}",
            relationEndPoint.RelationDefinition.ID, relationEndPoint.ClassDefinition.ID, relationEndPoint.RelationDefinition.ID.Length, c_relationNameMaximumLength));
      }

      return string.Format ("CONSTRAINT \"FK_{0}\" FOREIGN KEY (\"{1}\") REFERENCES \"{2}\" (\"ID\")",
          GetUniqueConstraintName (relationEndPoint),
          propertyDefinition.ColumnName,
          oppositeClassDefinition.GetEntityName ());
    }


    protected override string ConstraintSeparator
    {
      get { throw new NotSupportedException ("Oracle does not support adding multiple constraints within a single ALTER TABLE statement."); }
    }

    private string GetUniqueConstraintName (IRelationEndPointDefinition relationEndPoint)
    {
      int i = 1;

      string constraintName = relationEndPoint.RelationDefinition.ID;
      while (_constraintNamesUsed.ContainsKey (constraintName))
      {
        constraintName = string.Format ("{0}_{1}", relationEndPoint.RelationDefinition.ID, i);
        i++;
      }

      _constraintNamesUsed.Add (constraintName, constraintName);
      return constraintName;
    }
  }
}
