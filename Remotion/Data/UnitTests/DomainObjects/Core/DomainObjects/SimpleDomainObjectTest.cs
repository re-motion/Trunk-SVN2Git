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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class SimpleDomainObjectTest : ClientTransactionBaseTest
  {
    [Test]
    public void NewObject ()
    {
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject();
      Assert.That (instance, Is.Not.Null);
      Assert.That (instance.IntProperty, Is.EqualTo (0));
      instance.IntProperty = 5;
      Assert.That (instance.IntProperty, Is.EqualTo (5));
    }

    [Test]
    public void GetObject ()
    {
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject ();
      instance.IntProperty = 7;
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ClassDerivedFromSimpleDomainObject gottenInstance = ClassDerivedFromSimpleDomainObject.GetObject (instance.ID);
        Assert.That (gottenInstance, Is.SameAs (instance));
        Assert.That (gottenInstance.IntProperty, Is.EqualTo (7));
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException), ExpectedMessage = "Object '.*' is already deleted.", MatchType = MessageMatch.Regex)]
    public void GetObject_IncludeDeletedFalse_Deleted ()
    {
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject ();
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        instance.Delete();
        ClassDerivedFromSimpleDomainObject.GetObject (instance.ID, false);
      }
    }

    [Test]
    public void GetObject_IncludeDeletedTrue_Deleted ()
    {
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject ();
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        instance.Delete ();
        ClassDerivedFromSimpleDomainObject gottenInstance = ClassDerivedFromSimpleDomainObject.GetObject (instance.ID, true);
        Assert.That (gottenInstance, Is.SameAs (instance));
        Assert.That (gottenInstance.State, Is.EqualTo (StateType.Deleted));
      }
    }

    [Test]
    public void TryGetObject ()
    {
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject ();
      instance.IntProperty = 7;
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var gottenInstance = ClassDerivedFromSimpleDomainObject.TryGetObject (instance.ID);
        Assert.That (gottenInstance, Is.SameAs (instance));
        Assert.That (gottenInstance.IntProperty, Is.EqualTo (7));
      }
    }

    [Test]
    public void TryGetObject_NotFound ()
    {
      var id = new ObjectID (typeof (Order), Guid.NewGuid());
      var gottenInstance = ClassDerivedFromSimpleDomainObject.TryGetObject (id);
      Assert.That (gottenInstance, Is.Null);
    }

    [Test]
    public void DeserializationConstructor_CallsBase ()
    {
      var serializable = ClassDerivedFromSimpleDomainObject_ImplementingISerializable.NewObject ();

      var info = new SerializationInfo (typeof (ClassDerivedFromSimpleDomainObject_ImplementingISerializable), new FormatterConverter ());
      var context = new StreamingContext ();

      serializable.GetObjectData (info, context);
      Assert.That (info.MemberCount, Is.GreaterThan (0));

      var deserialized = 
          (ClassDerivedFromSimpleDomainObject_ImplementingISerializable) Activator.CreateInstance (((object)serializable).GetType (), info, context);
      Assert.That (deserialized.ID, Is.EqualTo (serializable.ID));
    }
  }
}
