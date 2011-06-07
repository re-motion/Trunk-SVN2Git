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
  public class DataContainerLoaderHelper : IDataContainerLoaderHelper
  {
    private readonly IStorageNameProvider _storageNameProvider;

    public DataContainerLoaderHelper (IStorageNameProvider storageNameProvider)
    {
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);

      _storageNameProvider = storageNameProvider;
    }

    public virtual IDbCommandBuilder GetCommandBuilderForIDLookup (RdbmsProvider provider, string entityName, ObjectID[] objectIDs)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      if (objectIDs.Length == 1)
        return new SingleIDLookupDbCommandBuilder (provider, _storageNameProvider, "*", entityName, _storageNameProvider.IDColumnName, objectIDs[0], null);
      else
        return new MultiIDLookupDbCommandBuilder (provider, _storageNameProvider, "*", entityName, _storageNameProvider.IDColumnName, provider.GetIDColumnTypeName(), objectIDs);
    }

    public virtual IDbCommandBuilder GetCommandBuilderForRelatedIDLookup (RdbmsProvider provider, string entityName, PropertyDefinition relationProperty, ObjectID relatedID)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);
      ArgumentUtility.CheckNotNull ("relationProperty", relationProperty);
      ArgumentUtility.CheckNotNull ("relatedID", relatedID);

      var oppositeRelationEndPointDefinition = 
          (VirtualRelationEndPointDefinition) relationProperty.ClassDefinition.GetMandatoryOppositeEndPointDefinition (relationProperty.PropertyName);

      return new SingleIDLookupDbCommandBuilder (
          provider,
          _storageNameProvider,
          "*",
          entityName,
          relationProperty.StoragePropertyDefinition.Name,
          relatedID,
          oppositeRelationEndPointDefinition.GetSortExpression());
    }

    public virtual ConcreteTableInheritanceRelationLoader GetConcreteTableInheritanceRelationLoader (RdbmsProvider provider, ClassDefinition classDefinition,
        PropertyDefinition propertyDefinition, ObjectID relatedID)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("relatedID", relatedID);

      return new ConcreteTableInheritanceRelationLoader (provider, _storageNameProvider, classDefinition, propertyDefinition, relatedID);
    }
  }
}
