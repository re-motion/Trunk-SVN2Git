using System;
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.DomainObjects.UnitTests.Core.DataManagement
{
  public class RelationEndPointBaseTest : ClientTransactionBaseTest
  {
    protected CollectionEndPoint CreateCollectionEndPoint (
        RelationEndPointID endPointID,
        DomainObjectCollection domainObjects)
    {
      CollectionEndPoint newCollectionEndPoint = new CollectionEndPoint (
          ClientTransactionMock, endPointID, domainObjects);

      newCollectionEndPoint.ChangeDelegate = ClientTransactionMock.DataManager.RelationEndPointMap;

      return newCollectionEndPoint;
    }

    protected ObjectEndPoint CreateObjectEndPoint (
        DomainObject domainObject,
        string propertyName,
        ObjectID oppositeObjectID)
    {
      return new ObjectEndPoint (ClientTransaction.Current, domainObject.ID, propertyName, oppositeObjectID);
    }

    protected ObjectEndPoint CreateObjectEndPoint (
        DataContainer dataContainer,
        string propertyName,
        ObjectID oppositeObjectID)
    {
      return new ObjectEndPoint (dataContainer.ClientTransaction, dataContainer.ID, propertyName, oppositeObjectID);
    }

    protected ObjectEndPoint CreateObjectEndPoint (
        RelationEndPointID endPointID,
        ObjectID oppositeObjectID)
    {
      return new ObjectEndPoint (ClientTransactionMock, endPointID, oppositeObjectID);
    }
  }
}
