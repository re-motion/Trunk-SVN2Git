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
using System.Reflection;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="ColumnDefinitionResolver"/> is responsible to get all <see cref="IColumnDefinition"/>s for a <see cref="ClassDefinition"/>
  /// </summary>
  public class ColumnDefinitionResolver : IColumnDefinitionResolver
  {
    public IEnumerable<IColumnDefinition> GetColumnDefinitionsForHierarchy (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var allClassesInHierarchy = classDefinition
          .CreateSequence (cd => cd.BaseClass)
          .Reverse ()
          .Concat (classDefinition.GetAllDerivedClasses ().Cast<ClassDefinition> ());

      var equalityComparer = new DelegateBasedEqualityComparer<Tuple<PropertyInfo, IColumnDefinition>> (
          (tuple1, tuple2) => tuple1.Item1 == tuple2.Item1,
          tuple => tuple.Item1.GetHashCode ());

      var columnDefinitions =
          (from cd in allClassesInHierarchy
           from PropertyDefinition pd in cd.MyPropertyDefinitions
           where pd.StorageClass == StorageClass.Persistent
           select Tuple.Create (pd.PropertyInfo, GetColumnDefinition (pd)))
              .Distinct (equalityComparer)
              .Select (tuple => tuple.Item2);

      return columnDefinitions;
    }

    public IColumnDefinition GetColumnDefinition (PropertyDefinition propertyDefinition)
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

      var columnDefinition = propertyDefinition.StoragePropertyDefinition as IColumnDefinition;
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