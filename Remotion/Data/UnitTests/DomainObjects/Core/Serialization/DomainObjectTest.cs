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
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using System.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class DomainObjectTest : ClientTransactionBaseTest
  {
    [Test]
    public void Serializable ()
    {
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject ();
      instance.IntProperty = 6;
      Tuple<ClientTransaction, ClassDerivedFromSimpleDomainObject> deserializedData =
          Serializer.SerializeAndDeserialize (Tuple.Create (ClientTransaction.Current, instance));
      ClassDerivedFromSimpleDomainObject deserializedInstance = deserializedData.Item2;
      using (deserializedData.Item1.EnterNonDiscardingScope ())
      {
        Assert.AreNotSame (instance, deserializedInstance);
        Assert.AreEqual (6, deserializedInstance.IntProperty);
      }
    }

    [Test]
    public void Serializable_WithISerializable ()
    {
      ClassWithAllDataTypes instance = ClassWithAllDataTypes.NewObject ();
      instance.Int32Property = 5;
      Tuple<ClientTransaction, ClassWithAllDataTypes> deserializedData =
          Serializer.SerializeAndDeserialize (Tuple.Create (ClientTransaction.Current, instance));
      ClassWithAllDataTypes deserializedInstance = deserializedData.Item2;
      using (deserializedData.Item1.EnterNonDiscardingScope ())
      {
        Assert.AreNotSame (instance, deserializedInstance);
        Assert.AreEqual (5, deserializedInstance.Int32Property);
      }
    }

    [Test]
    public void Serializable_WithISerializable_AndBindingTransaction ()
    {
      ClassWithAllDataTypes instance;
      
      using (ClientTransaction.CreateBindingTransaction ().EnterNonDiscardingScope ())
      {
        instance = ClassWithAllDataTypes.NewObject();
      }

      instance.Int32Property = 5;
      Tuple<ClientTransaction, ClassWithAllDataTypes> deserializedData =
          Serializer.SerializeAndDeserialize (Tuple.Create (instance.GetBindingTransaction(), instance));
      
      ClassWithAllDataTypes deserializedInstance = deserializedData.Item2;
      Assert.IsTrue (deserializedInstance.HasBindingTransaction);
      Assert.AreSame (deserializedData.Item1, deserializedInstance.GetBindingTransaction());
      Assert.AreEqual (5, deserializedInstance.Int32Property);
    }

    [Test]
    public void Serializable_LoadCount ()
    {
      ClassWithAllDataTypes instance = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.IsTrue (instance.OnLoadedCalled);
      Assert.AreEqual (LoadMode.WholeDomainObjectInitialized, instance.OnLoadedLoadMode);

      Tuple<ClientTransaction, ClassWithAllDataTypes> deserializedData =
          Serializer.SerializeAndDeserialize (Tuple.Create (ClientTransaction.Current, instance));

      using (deserializedData.Item1.EnterNonDiscardingScope ())
      {
        ClassWithAllDataTypes deserializedInstance = deserializedData.Item2;
        Assert.IsFalse (deserializedInstance.OnLoadedCalled);
        using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
        {
          ClientTransaction.Current.EnlistDomainObject (deserializedInstance);
          deserializedInstance.Int32Property = 15;
          Assert.IsTrue (deserializedInstance.OnLoadedCalled);
          Assert.AreEqual (LoadMode.DataContainerLoadedOnly, deserializedInstance.OnLoadedLoadMode);
        }
      }
    }

    [Test]
    public void Serialization_WithISerializable_IncludesEventHandlers ()
    {
      ClassWithAllDataTypes instance = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      var eventReceiver = new DomainObjectEventReceiver (instance);

      var deserializedData = Serializer.SerializeAndDeserialize (Tuple.Create (instance, eventReceiver));
      AssertEventRegistered (deserializedData.Item1, "RelationChanging", deserializedData.Item2, GetEventHandlerMethod (instance, "RelationChanging"));
      AssertEventRegistered (deserializedData.Item1, "RelationChanged", deserializedData.Item2, GetEventHandlerMethod (instance, "RelationChanged"));
      AssertEventRegistered (deserializedData.Item1, "PropertyChanging", deserializedData.Item2, GetEventHandlerMethod (instance, "PropertyChanging"));
      AssertEventRegistered (deserializedData.Item1, "PropertyChanged", deserializedData.Item2, GetEventHandlerMethod (instance, "PropertyChanged"));
      AssertEventRegistered (deserializedData.Item1, "Deleting", deserializedData.Item2, GetEventHandlerMethod (instance, "Deleting"));
      AssertEventRegistered (deserializedData.Item1, "Deleted", deserializedData.Item2, GetEventHandlerMethod (instance, "Deleted"));
      AssertEventRegistered (deserializedData.Item1, "Committing", deserializedData.Item2, GetEventHandlerMethod (instance, "Committing"));
      AssertEventRegistered (deserializedData.Item1, "Committed", deserializedData.Item2, GetEventHandlerMethod (instance, "Committed"));
      AssertEventRegistered (deserializedData.Item1, "RollingBack", deserializedData.Item2, GetEventHandlerMethod (instance, "RollingBack"));
      AssertEventRegistered (deserializedData.Item1, "RolledBack", deserializedData.Item2, GetEventHandlerMethod (instance, "RolledBack"));
    }

    private void AssertEventRegistered (DomainObject domainObject, string eventName, object receiver, MethodInfo receiverMethod)
    {
      var eventDelegate = (Delegate) PrivateInvoke.GetNonPublicField (domainObject, typeof (DomainObject), eventName);
      Assert.That (eventDelegate, Is.Not.Null);
      Assert.That (eventDelegate.Target, Is.SameAs (receiver));
      Assert.That (eventDelegate.Method, Is.EqualTo (receiverMethod));
    }

    private MethodInfo GetEventHandlerMethod (DomainObject instance, string eventName)
    {
      var eventDelegate = (Delegate) PrivateInvoke.GetNonPublicField (instance, typeof (DomainObject), eventName);
      return eventDelegate.Method;
    }
  }
}
