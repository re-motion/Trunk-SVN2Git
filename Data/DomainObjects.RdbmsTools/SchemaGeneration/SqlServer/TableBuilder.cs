using System;
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
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

      string sqlDataType = GetSqlDataType (propertyDefinition.PropertyType);
      if (!string.IsNullOrEmpty (sqlDataType))
        return sqlDataType;

      if (propertyDefinition.PropertyType == typeof (String))
      {
        if (propertyDefinition.MaxLength.HasValue)
          return string.Format ("nvarchar ({0})", propertyDefinition.MaxLength);
        return "ntext";
      }

      if (propertyDefinition.PropertyType == typeof (Byte[]))
        return "image";

      return base.GetSqlDataType (propertyDefinition);
    }

    private static string GetSqlDataType (Type type)
    {
      type = Nullable.GetUnderlyingType (type) ?? type;

      if (type == typeof (Boolean))
        return "bit";
      if (type == typeof (Byte))
        return "tinyint";
      if (type == typeof (DateTime))
        return "datetime";
      if (type == typeof (Decimal))
        return "decimal (38, 3)";
      if (type == typeof (Double))
        return "float";
      if (type == typeof (Guid))
        return "uniqueidentifier";
      if (type == typeof (Int16))
        return "smallint";
      if (type == typeof (Int32))
        return "int";
      if (type == typeof (Int64))
        return "bigint";
      if (type == typeof (Single))
        return "real";
      if (type.IsEnum)
        return GetSqlDataType (Enum.GetUnderlyingType (type));

      return null;
    }

    protected override string SqlDataTypeObjectID
    {
      get { return "uniqueidentifier"; }
    }

    protected override string SqlDataTypeSerializedObjectID
    {
      get { return "varchar (255)"; }
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
          classDefinition.MyEntityName,
          GetColumnList (classDefinition));
    }

    public override void AddToDropTableScript (ClassDefinition classDefinition, StringBuilder dropTableStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("dropTableStringBuilder", dropTableStringBuilder);

      dropTableStringBuilder.AppendFormat (
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = '{0}' AND TABLE_SCHEMA = '{1}')\r\n"
          + "  DROP TABLE [{1}].[{0}]\r\n",
          classDefinition.MyEntityName,
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
          propertyDefinition.StorageSpecificName,
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

      return string.Format ("  [{0}] {1} NULL,\r\n", RdbmsProvider.GetClassIDColumnName (propertyDefinition.StorageSpecificName), SqlDataTypeClassID);
    }
  }
}