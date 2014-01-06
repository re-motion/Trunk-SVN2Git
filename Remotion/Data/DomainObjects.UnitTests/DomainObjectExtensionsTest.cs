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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests
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
    public void GetHandle ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();

      var handle = domainObject.GetHandle ();
      var domainObjectTypedObjectID1 = domainObject.GetHandle<DomainObject> ();
      var domainObjectTypedObjectID2 = ((DomainObject) domainObject).GetHandle ();

      Assert.That (handle, Is.TypeOf<DomainObjectHandle<Order>> ().And.Property ("ObjectID").EqualTo (domainObject.ID));
      Assert.That (domainObjectTypedObjectID1, Is.TypeOf<DomainObjectHandle<Order>> ().And.Property ("ObjectID").EqualTo (domainObject.ID));
      Assert.That (domainObjectTypedObjectID2, Is.TypeOf<DomainObjectHandle<Order>> ().And.Property ("ObjectID").EqualTo (domainObject.ID));

      Assert.That (VariableTypeInferrer.GetVariableType (handle), Is.SameAs (typeof (IDomainObjectHandle<Order>)));
      Assert.That (VariableTypeInferrer.GetVariableType (domainObjectTypedObjectID1), Is.SameAs (typeof (IDomainObjectHandle<DomainObject>)));
      Assert.That (VariableTypeInferrer.GetVariableType (domainObjectTypedObjectID2), Is.SameAs (typeof (IDomainObjectHandle<DomainObject>)));
    }

    [Test]
    public void GetSafeHandle ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();

      var handle = domainObject.GetSafeHandle ();
      var domainObjectTypedObjectID1 = domainObject.GetSafeHandle<DomainObject> ();
      var domainObjectTypedObjectID2 = ((DomainObject) domainObject).GetSafeHandle ();

      Assert.That (handle, Is.TypeOf<DomainObjectHandle<Order>> ().And.Property ("ObjectID").EqualTo (domainObject.ID));
      Assert.That (domainObjectTypedObjectID1, Is.TypeOf<DomainObjectHandle<Order>> ().And.Property ("ObjectID").EqualTo (domainObject.ID));
      Assert.That (domainObjectTypedObjectID2, Is.TypeOf<DomainObjectHandle<Order>> ().And.Property ("ObjectID").EqualTo (domainObject.ID));

      Assert.That (VariableTypeInferrer.GetVariableType (handle), Is.SameAs (typeof (IDomainObjectHandle<Order>)));
      Assert.That (VariableTypeInferrer.GetVariableType (domainObjectTypedObjectID1), Is.SameAs (typeof (IDomainObjectHandle<DomainObject>)));
      Assert.That (VariableTypeInferrer.GetVariableType (domainObjectTypedObjectID2), Is.SameAs (typeof (IDomainObjectHandle<DomainObject>)));
    }

    [Test]
    public void GetSafeHandle_Null ()
    {
      var handle = ((Order) null).GetSafeHandle ();
      Assert.That (handle, Is.Null);
    }
  }
}