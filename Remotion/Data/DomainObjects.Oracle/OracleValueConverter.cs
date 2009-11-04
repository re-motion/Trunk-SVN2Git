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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using Remotion.NullableValueTypes;

namespace Remotion.Data.DomainObjects.Oracle
{
  public class OracleValueConverter: ValueConverter
  {
    public override object GetDBValue (ObjectID id, string storageProviderID)
    {
      return ConvertDBValue (base.GetDBValue (id, storageProviderID));
    }

    public override object GetDBValue (object value)
    {
      return ConvertDBValue (base.GetDBValue (value));
    }

    private object ConvertDBValue (object value)
    {
      if (value is Guid)
        return ((Guid) value).ToByteArray ();
      else if (value is bool)
        return (bool) value ? 1 : 0;
      return value;
    }

    public override ObjectID GetObjectID (ClassDefinition classDefinition, object dataValue)
    {
      if (dataValue is byte[])
        return base.GetObjectID (classDefinition, new Guid ((byte[]) dataValue));
      else
        return base.GetObjectID (classDefinition, dataValue);
    }

    public override object GetValue (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, object dataValue)
    {
      if (dataValue != null)
      {
        if (propertyDefinition.PropertyType == typeof (Guid))
        {
          byte[] binaryGuid = ArgumentUtility.CheckType<byte[]> ("dataValue", dataValue);
          dataValue = new Guid (binaryGuid);
        }
        else if (propertyDefinition.PropertyType == typeof (bool))
        {
          // dataValue could be of type short OR decimal, because ORACLE interpretes NUMBER(1,0) columns in views with union, 
          //    where not all participating tables return this columns,
          //    as NUMBER without precision, so dataValue would be decimal instead of short in this case
          //
          // Example (BooleanColumn2 would be NUMBER (9) in View):
          // CREATE VIEW "TestView" ("ID", "ClassID", "Timestamp", "StringColumn1", "BooleanColumn2")
          // AS
          // SELECT "ID", "ClassID", "Timestamp", "StringColumn1", null
          //   FROM "TestTable1"
          // UNION ALL
          // SELECT "ID", "ClassID", "Timestamp", null, "BooleanColumn2"
          //   FROM "TestTable2"

          Int16 boolAsInt = Convert.ToInt16(dataValue);
          dataValue = (boolAsInt != 0);
        }
        else if (propertyDefinition.PropertyType == typeof (Int32) && dataValue.GetType () == typeof (decimal))
        {
          dataValue = Convert.ToInt32 (dataValue);
        }
        else if (propertyDefinition.PropertyType == typeof (NaBoolean) && dataValue.GetType () == typeof (short))
        {
          dataValue = Convert.ToBoolean (dataValue);
        }
      }
      return base.GetValue (classDefinition, propertyDefinition, dataValue);
    }
  }
}
