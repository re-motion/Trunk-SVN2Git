// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class PendingLoadedObjectDataRegistrationContextTest : StandardMappingTest
  {
    private ILoadedObjectData _loadedObject1;
    private ILoadedObjectData _loadedObject2;

    public override void SetUp ()
    {
      base.SetUp ();

      _loadedObject1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (DomainObjectIDs.Order1);
      _loadedObject2 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (DomainObjectIDs.Order2);
    }

    [Test]
    public void AddObjectsPendingRegistration_AddsObjectsToList ()
    {
      var context = new PendingLoadedObjectDataRegistrationContext();

      context.AddObjectsPendingRegistration (new[] { _loadedObject1, _loadedObject2 });

      Assert.That (context.ObjectsPendingRegistration, Is.EquivalentTo (new[] { _loadedObject1, _loadedObject2 }));
    }

    [Test]
    public void AddObjectsPendingRegistration_MultipleTimes_ObjectsAreAddedToList ()
    {
      var context = new PendingLoadedObjectDataRegistrationContext ();

      context.AddObjectsPendingRegistration (new[] { _loadedObject1 });
      context.AddObjectsPendingRegistration (new[] { _loadedObject2 });

      Assert.That (context.ObjectsPendingRegistration, Is.EquivalentTo (new[] { _loadedObject1, _loadedObject2 }));
    }

    [Test]
    public void AddObjectsPendingRegistration_MultipleTimes_SameObject_IsOnlyAddedOnce ()
    {
      var context = new PendingLoadedObjectDataRegistrationContext ();

      context.AddObjectsPendingRegistration (new[] { _loadedObject1 });
      context.AddObjectsPendingRegistration (new[] { _loadedObject1, _loadedObject2 });

      Assert.That (context.ObjectsPendingRegistration, Is.EquivalentTo (new[] { _loadedObject1, _loadedObject2 }));
    }

    [Test]
    public void AddObjectsPendingRegistration_MultipleTimes_DifferentObjectWithSameObjectID_FirstObjectWins ()
    {
      var context = new PendingLoadedObjectDataRegistrationContext ();

      var alternativeObject1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (_loadedObject1.ObjectID);

      context.AddObjectsPendingRegistration (new[] { _loadedObject1 });
      context.AddObjectsPendingRegistration (new[] { alternativeObject1 });

      Assert.That (context.ObjectsPendingRegistration, Is.EquivalentTo (new[] { _loadedObject1 }));
    }
  }
}