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
using JetBrains.Annotations;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  [TestFixture]
  public class DomainObjectExtensionsTest : StandardMappingTest
  {
    [Test]
    public void GetIDOrNull ()
    {
      Assert.That (((DomainObject) null).GetSafeID(), Is.Null);

      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      Assert.That (domainObject.GetSafeID(), Is.EqualTo (domainObject.ID));
    }

    [Test]
    public void GetTypedID ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();

      var objectID = domainObject.GetTypedID ();
      var domainObjectTypedObjectID1 = domainObject.GetTypedID<DomainObject> ();
      var domainObjectTypedObjectID2 = ((DomainObject) domainObject).GetTypedID ();

      Assert.That (objectID, Is.TypeOf<ObjectID<Order>> ().And.EqualTo (domainObject.ID));
      Assert.That (domainObjectTypedObjectID1, Is.TypeOf<ObjectID<Order>> ().And.EqualTo (domainObject.ID));
      Assert.That (domainObjectTypedObjectID2, Is.TypeOf<ObjectID<Order>> ().And.EqualTo (domainObject.ID));

      Assert.That (VariableTypeInferrer.GetVariableType (objectID), Is.SameAs (typeof (IObjectID<Order>)));
      Assert.That (VariableTypeInferrer.GetVariableType (domainObjectTypedObjectID1), Is.SameAs (typeof (IObjectID<DomainObject>)));
      Assert.That (VariableTypeInferrer.GetVariableType (domainObjectTypedObjectID2), Is.SameAs (typeof (IObjectID<DomainObject>)));
    }

    [Test]
    public void GetSafeTypedID ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();

      var objectID = domainObject.GetSafeTypedID ();
      var nullID = ((Order) null).GetSafeTypedID ();

      Assert.That (objectID, Is.TypeOf<ObjectID<Order>> ().And.EqualTo (domainObject.ID));
      Assert.That (VariableTypeInferrer.GetVariableType (objectID), Is.SameAs (typeof (IObjectID<Order>)));

      Assert.That (nullID, Is.Null);
    }
  }
}