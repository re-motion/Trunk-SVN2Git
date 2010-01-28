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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq.IntegrationTests
{
  [TestFixture]
  public abstract class IntegrationTestBase : ClientTransactionBaseTest
  {
    protected void CheckQueryResult<T> (IEnumerable<T> query, params ObjectID[] expectedObjectIDs)
        where T : DomainObject
    {
      T[] results = query.ToArray ();
      T[] expected = GetExpectedObjects<T> (expectedObjectIDs);
      Assert.That (results, Is.EquivalentTo (expected));
    }

    protected T[] GetExpectedObjects<T> (params ObjectID[] expectedObjectIDs)
        where T: DomainObject
    {
      return (from id in expectedObjectIDs 
              select (id == null ? null : (T) LifetimeService.GetObject (ClientTransactionMock, id, false))).ToArray ();
    }

    protected void CheckDataContainersRegistered (params ObjectID[] objectIDs)
    {
      // check that related objects have been loaded
      foreach (var id in objectIDs)
        Assert.That (ClientTransactionMock.DataManager.DataContainerMap[id], Is.Not.Null);
    }

    protected void CheckCollectionRelationRegistered (ObjectID originatingObjectID, string shortPropertyName, bool checkOrdering, params ObjectID[] expectedRelatedObjectIDs)
    {
      var relationEndPointDefinition =
          originatingObjectID.ClassDefinition.GetMandatoryRelationEndPointDefinition (
              originatingObjectID.ClassDefinition.ClassType.FullName + "." + shortPropertyName);

      var collectionEndPoint = (CollectionEndPoint)
                               ClientTransactionMock.DataManager.RelationEndPointMap[
                                   new RelationEndPointID (originatingObjectID, relationEndPointDefinition)];
      Assert.That (collectionEndPoint, Is.Not.Null);

      var expectedRelatedObjects = expectedRelatedObjectIDs.Select (id => LifetimeService.GetObject (ClientTransactionMock, id, false)).ToArray ();
      if (checkOrdering)
        Assert.That (collectionEndPoint.OppositeDomainObjects, Is.EqualTo (expectedRelatedObjects));
      else
        Assert.That (collectionEndPoint.OppositeDomainObjects, Is.EquivalentTo (expectedRelatedObjects));
    }

    protected void CheckObjectRelationRegistered (ObjectID originatingObjectID, string shortPropertyName, ObjectID expectedRelatedObjectID)
    {
      var declaringType = originatingObjectID.ClassDefinition.ClassType;
      CheckObjectRelationRegistered(originatingObjectID, declaringType, shortPropertyName, expectedRelatedObjectID);
    }

    protected void CheckObjectRelationRegistered (ObjectID originatingObjectID, Type declaringType, string shortPropertyName, ObjectID expectedRelatedObjectID)
    {
      var longPropertyName = declaringType.FullName + "." + shortPropertyName;
      var relationEndPointDefinition =
          originatingObjectID.ClassDefinition.GetMandatoryRelationEndPointDefinition (
              longPropertyName);

      var objectEndPoint = (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[
                                                new RelationEndPointID (originatingObjectID, relationEndPointDefinition)];
      Assert.That (objectEndPoint, Is.Not.Null);
      Assert.That (objectEndPoint.OppositeObjectID, Is.EqualTo (expectedRelatedObjectID));
    }
  }
}
