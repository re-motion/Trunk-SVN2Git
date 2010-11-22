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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// <see cref="StorageTypeCalculator"/> is the base class for type-calculator implementations which determine the storage-specific type for a 
  /// storable column definition.
  /// </summary>
  public abstract class StorageTypeCalculator
  {
    protected abstract string SqlDataTypeObjectID { get; }
    protected abstract string SqlDataTypeSerializedObjectID { get; }

    public virtual string GetStorageType (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      if (propertyDefinition.IsObjectID)
      {
        var oppositeClass = propertyDefinition.ClassDefinition.GetOppositeClassDefinition (propertyDefinition.PropertyName);
        if(oppositeClass==null)
          Console.WriteLine (propertyDefinition.ClassDefinition.ID+"->"+propertyDefinition.PropertyName);

        if (oppositeClass.StorageProviderID == propertyDefinition.ClassDefinition.StorageProviderID)
          return SqlDataTypeObjectID;
        else
          return SqlDataTypeSerializedObjectID;
      }

      throw new InvalidOperationException (string.Format (
              "Data type '{0}' is not supported.\r\nDeclaring type: '{1}'\r\nProperty: '{2}'",
              propertyDefinition.PropertyType,
              propertyDefinition.ClassDefinition.ClassType.FullName,
              propertyDefinition.PropertyName));
    }
  }
}