// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using NUnit.Framework.SyntaxHelpers;
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
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject().With();
      Assert.IsNotNull (instance);
      Assert.AreEqual (0, instance.IntProperty);
      instance.IntProperty = 5;
      Assert.AreEqual (5, instance.IntProperty);
    }

    [Test]
    public void GetObject_WithoutDeleted ()
    {
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject ().With ();
      instance.IntProperty = 7;
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ClassDerivedFromSimpleDomainObject gottenInstance = ClassDerivedFromSimpleDomainObject.GetObject (instance.ID);
        Assert.AreSame (instance, gottenInstance);
        Assert.AreEqual (7, gottenInstance.IntProperty);
      }
    }

    [Test]
    public void GetObject_IncludeDeletedFalse_NonDeleted ()
    {
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject ().With ();
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ClassDerivedFromSimpleDomainObject gottenInstance = ClassDerivedFromSimpleDomainObject.GetObject (instance.ID, false);
        Assert.AreSame (instance, gottenInstance);
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException), ExpectedMessage = "Object '.*' is already deleted.", MatchType = MessageMatch.Regex)]
    public void GetObject_IncludeDeletedFalse_Deleted ()
    {
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject ().With ();
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        instance.Delete();
        ClassDerivedFromSimpleDomainObject.GetObject (instance.ID, false);
      }
    }

    [Test]
    public void GetObject_IncludeDeletedTrue_NonDeleted ()
    {
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject ().With ();
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ClassDerivedFromSimpleDomainObject gottenInstance = ClassDerivedFromSimpleDomainObject.GetObject (instance.ID, true);
        Assert.AreSame (instance, gottenInstance);
      }
    }

    [Test]
    public void GetObject_IncludeDeletedTrue_Deleted ()
    {
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject ().With ();
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        instance.Delete ();
        ClassDerivedFromSimpleDomainObject gottenInstance = ClassDerivedFromSimpleDomainObject.GetObject (instance.ID, true);
        Assert.AreSame (instance, gottenInstance);
        Assert.AreEqual (StateType.Deleted, gottenInstance.State);
      }
    }

    [Test]
    public void DeserializationConstructor_CallsBase ()
    {
      var serializable = ClassDerivedFromSimpleDomainObject_ImplementingISerializable.NewObject ().With();

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
