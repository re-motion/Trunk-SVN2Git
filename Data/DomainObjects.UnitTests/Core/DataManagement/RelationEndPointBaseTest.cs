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
