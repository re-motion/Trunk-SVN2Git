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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  /// <summary>
  /// The <see cref="RdbmsPersistenceModelProvider"/> implements methods that retrieve rdbms-specific persistence model definitions from mapping objects.
  /// </summary>
  public class RdbmsPersistenceModelProvider : IRdbmsPersistenceModelProvider
  {
    public IEntityDefinition GetEntityDefinition (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var storageEntityDefinitionAsIEntityDefinition = classDefinition.StorageEntityDefinition as IEntityDefinition;
      if (storageEntityDefinitionAsIEntityDefinition == null)
      {
        throw new InvalidOperationException (
            string.Format (
                "The RdbmsProvider expected a storage definition object of type '{0}' for class-definition '{1}', "
                + "but found a storage definition object of type '{2}'.",
                typeof (IEntityDefinition).Name,
                classDefinition.ID,
                classDefinition.StorageEntityDefinition.GetType().Name));
      }

      return storageEntityDefinitionAsIEntityDefinition;
    }

    public IColumnDefinition GetColumnDefinition (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      var storagePropertyDefinitionAsIColumnDefinition = propertyDefinition.StoragePropertyDefinition as IColumnDefinition;
      if (storagePropertyDefinitionAsIColumnDefinition == null)
      {
        throw new InvalidOperationException(
          string.Format("The RdbmsProvider expected a storage definition object of type '{0}' for property '{1}' of class-definition '{2}', "
                + "but found a storage definition object of type '{3}'.",
                typeof (IColumnDefinition).Name,
                propertyDefinition.PropertyName,
                propertyDefinition.ClassDefinition.ID,
                propertyDefinition.StoragePropertyDefinition.GetType().Name));
      }

      return storagePropertyDefinitionAsIColumnDefinition;
    }

    public IDColumnDefinition GetIDColumnDefinition (RelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      var storagePropertyDefinitionAsIDColumnDefinition = relationEndPointDefinition.PropertyDefinition.StoragePropertyDefinition as IDColumnDefinition;
      if (storagePropertyDefinitionAsIDColumnDefinition == null)
      {
        throw new InvalidOperationException (
          string.Format ("The RdbmsProvider expected a storage definition object of type '{0}' for property '{1}' of class-definition '{2}', "
                + "but found a storage definition object of type '{3}'.",
                typeof (IDColumnDefinition).Name,
                relationEndPointDefinition.PropertyDefinition.PropertyName,
                relationEndPointDefinition.PropertyDefinition.ClassDefinition.ID,
                relationEndPointDefinition.PropertyDefinition.StoragePropertyDefinition.GetType ().Name));
      }

      return storagePropertyDefinitionAsIDColumnDefinition;
    }
  }
}