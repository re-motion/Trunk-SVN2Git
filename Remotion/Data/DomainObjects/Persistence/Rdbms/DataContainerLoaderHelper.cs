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

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class DataContainerLoaderHelper : IDataContainerLoaderHelper
  {
    public virtual SelectCommandBuilder GetSelectCommandBuilder (RdbmsProvider provider, string entityName, ObjectID[] objectIDs)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      return SelectCommandBuilder.CreateForIDLookup (provider, "*", entityName, objectIDs);
    }

    public virtual SelectCommandBuilder GetSelectCommandBuilderForRelatedIDLookup (RdbmsProvider provider, string entityName, PropertyDefinition relationProperty, ObjectID relatedID)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);
      ArgumentUtility.CheckNotNull ("relationProperty", relationProperty);
      ArgumentUtility.CheckNotNull ("relatedID", relatedID);

      return SelectCommandBuilder.CreateForRelatedIDLookup (provider, entityName, relationProperty, relatedID);
    }

    public virtual ConcreteTableInheritanceRelationLoader GetConcreteTableInheritanceRelationLoader (RdbmsProvider provider, ClassDefinition classDefinition,
        PropertyDefinition propertyDefinition, ObjectID relatedID)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("relatedID", relatedID);

      return new ConcreteTableInheritanceRelationLoader (provider, classDefinition, propertyDefinition, relatedID);
    }
  }
}
