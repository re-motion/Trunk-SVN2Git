using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Security.UnitTests.Data.DomainObjects.TestDomain
{
  public class StubStorageProvider : StorageProvider
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public StubStorageProvider (StorageProviderDefinition definition)
      : base (definition)
    {
    }

    // methods and properties

    public override DataContainer LoadDataContainer (ObjectID id)
    {
      throw new NotImplementedException ();
    }

    public override DataContainerCollection LoadDataContainers (IEnumerable<ObjectID> ids)
    {
      throw new NotImplementedException ();
    }

    public override DataContainerCollection ExecuteCollectionQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      DataContainerCollection collection = new DataContainerCollection ();
      if (query.ID == "GetSecurableObjects")
        collection.Add (DataContainer.CreateNew (CreateNewObjectID  (MappingConfiguration.Current.ClassDefinitions[typeof (SecurableObject)])));

      return collection;
    }

    public override object ExecuteScalarQuery (IQuery query)
    {
      throw new NotImplementedException ();
    }

    public override void Save (DataContainerCollection dataContainers)
    {
    }

    public override void SetTimestamp (DataContainerCollection dataContainers)
    {
    }

    public override DataContainerCollection LoadDataContainersByRelatedID (ClassDefinition classDefinition, string propertyName, ObjectID relatedID)
    {
      throw new NotImplementedException ();
    }

    public override void BeginTransaction ()
    {
    }

    public override void Commit ()
    {
    }

    public override void Rollback ()
    {
    }

    public override ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      CheckDisposed ();
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      CheckClassDefinition (classDefinition, "classDefinition");

      return new ObjectID (classDefinition.ID, Guid.NewGuid ());
    }

    private void CheckClassDefinition (ClassDefinition classDefinition, string argumentName)
    {
      if (classDefinition.StorageProviderID != ID)
      {
        throw CreateArgumentException (
            argumentName,
            "The StorageProviderID '{0}' of the provided ClassDefinition does not match with this StorageProvider's ID '{1}'.",
            classDefinition.StorageProviderID,
            ID);
      }
    }
  }
}