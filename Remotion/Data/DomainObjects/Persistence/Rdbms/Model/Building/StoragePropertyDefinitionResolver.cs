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
using System.Collections.Generic;
using System.Linq;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// The <see cref="StoragePropertyDefinitionResolver"/> is responsible to get all <see cref="IRdbmsStoragePropertyDefinition"/> instances for a 
  /// <see cref="ClassDefinition"/> (or for one of its <see cref="PropertyDefinition"/> objects).
  /// </summary>
  public class StoragePropertyDefinitionResolver : IStoragePropertyDefinitionResolver
  {
    public IEnumerable<IRdbmsStoragePropertyDefinition> GetStoragePropertiesForHierarchy (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var allClassesInHierarchy = classDefinition
          .CreateSequence (cd => cd.BaseClass)
          .Reverse ()
          .Concat (classDefinition.GetAllDerivedClasses ());

      var equalityComparer = new DelegateBasedEqualityComparer<Tuple<IPropertyInformation, IRdbmsStoragePropertyDefinition>> (
          (tuple1, tuple2) => tuple1.Item1.Equals (tuple2.Item1),
          tuple => tuple.Item1.GetHashCode ());

      var storageProperties =
          (from cd in allClassesInHierarchy
           from PropertyDefinition pd in cd.MyPropertyDefinitions
           where pd.StorageClass == StorageClass.Persistent
           select Tuple.Create (pd.PropertyInfo, GetStorageProperty (pd)))
              .Distinct (equalityComparer)
              .Select (tuple => tuple.Item2);

      return storageProperties;
    }

    public IRdbmsStoragePropertyDefinition GetStorageProperty (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      if (propertyDefinition.StoragePropertyDefinition == null)
      {
        throw new InvalidOperationException (
            string.Format (
                "Storage property definition has not been set.\r\nDeclaring type: '{0}'\r\nProperty: '{1}'",
                propertyDefinition.PropertyInfo.DeclaringType.FullName,
                propertyDefinition.PropertyInfo.Name));
      }

      var columnDefinition = propertyDefinition.StoragePropertyDefinition as IRdbmsStoragePropertyDefinition;
      if (columnDefinition == null)
      {
        throw new MappingException (
            string.Format (
                "Cannot have non-RDBMS storage properties in an RDBMS mapping.\r\nDeclaring type: '{0}'\r\nProperty: '{1}'",
                propertyDefinition.PropertyInfo.DeclaringType.FullName,
                propertyDefinition.PropertyInfo.Name));
      }

      return columnDefinition;
    }
  }
}