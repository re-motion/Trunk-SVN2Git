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