/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class DataContainerLoaderHelper : IDataContainerLoaderHelper
  {
    public virtual SelectCommandBuilder GetSelectCommandBuilder (RdbmsProvider provider, string entityName, IEnumerable<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      return SelectCommandBuilder.CreateForIDLookup (provider, entityName, new List<ObjectID> (objectIDs).ToArray ());
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
