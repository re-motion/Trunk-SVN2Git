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
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints
{
  public static class CollectionEndPointTestHelper
  {
    public static void SetCollection (CollectionEndPoint collectionEndPoint, DomainObjectCollection newCollection)
    {
      PrivateInvoke.SetPublicProperty (collectionEndPoint, "Collection", newCollection);
    }

    public static IRealObjectEndPoint GetFakeOppositeEndPoint (DomainObject item)
    {
      var fakeEndPoint = MockRepository.GenerateStub<IRealObjectEndPoint>();
      fakeEndPoint.Stub (stub => stub.ObjectID).Return (item.ID);
      fakeEndPoint.Stub (stub => stub.GetDomainObject()).Return (item);
      fakeEndPoint.Stub (stub => stub.GetDomainObjectReference()).Return (item);
      return fakeEndPoint;
    }

    public static void FillCollectionEndPointWithInitialContents (CollectionEndPoint endPoint, IEnumerable<DomainObject> initialContents)
    {
      var dataManager = ClientTransactionTestHelper.GetDataManager (ClientTransaction.Current);
      var domainObjects = initialContents.ToArray ();
      foreach (var domainObject in domainObjects)
      {
        var oppositeEndPointID = RelationEndPointID.Create (domainObject.ID, endPoint.Definition.GetOppositeEndPointDefinition());
        var oppositeEndPoint = (IRealObjectEndPoint) dataManager.GetRelationEndPointWithLazyLoad (oppositeEndPointID);
        endPoint.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
      }
      endPoint.MarkDataComplete (domainObjects);
    }

    public static ICollectionEndPointLoadState GetLoadState (CollectionEndPoint collectionEndPoint)
    {
      return (ICollectionEndPointLoadState) PrivateInvoke.GetNonPublicField (collectionEndPoint, "_loadState");
    }
  }
}