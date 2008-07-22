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
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class DomainObjectTest : ClientTransactionBaseTest
  {
    [Test]
    public void Serializable ()
    {
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject ().With ();
      instance.IntProperty = 6;
      Tuple<ClientTransaction, ClassDerivedFromSimpleDomainObject> deserializedData =
          Serializer.SerializeAndDeserialize (Tuple.NewTuple (ClientTransaction.Current, instance));
      ClassDerivedFromSimpleDomainObject deserializedInstance = deserializedData.B;
      using (deserializedData.A.EnterNonDiscardingScope ())
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
          Serializer.SerializeAndDeserialize (Tuple.NewTuple (ClientTransaction.Current, instance));
      ClassWithAllDataTypes deserializedInstance = deserializedData.B;
      using (deserializedData.A.EnterNonDiscardingScope ())
      {
        Assert.AreNotSame (instance, deserializedInstance);
        Assert.AreEqual (5, deserializedInstance.Int32Property);
      }
    }

    [Test]
    public void Serializable_WithISerializable_AndBindingTransaction ()
    {
      ClassWithAllDataTypes instance;
      
      using (ClientTransaction.NewBindingTransaction ().EnterNonDiscardingScope ())
      {
        instance = ClassWithAllDataTypes.NewObject();
      }

      instance.Int32Property = 5;
      Tuple<ClientTransaction, ClassWithAllDataTypes> deserializedData =
          Serializer.SerializeAndDeserialize (Tuple.NewTuple (instance.ClientTransaction, instance));
      
      ClassWithAllDataTypes deserializedInstance = deserializedData.B;
      Assert.IsTrue (deserializedInstance.IsBoundToSpecificTransaction);
      Assert.AreSame (deserializedData.A, deserializedInstance.ClientTransaction);
      Assert.AreEqual (5, deserializedInstance.Int32Property);
    }

    [Test]
    public void Serializable_LoadCount ()
    {
      ClassWithAllDataTypes instance = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.IsTrue (instance.OnLoadedHasBeenCalled);
      Assert.AreEqual (LoadMode.WholeDomainObjectInitialized, instance.OnLoadedLoadMode);

      Tuple<ClientTransaction, ClassWithAllDataTypes> deserializedData =
          Serializer.SerializeAndDeserialize (Tuple.NewTuple (ClientTransaction.Current, instance));

      using (deserializedData.A.EnterNonDiscardingScope ())
      {
        ClassWithAllDataTypes deserializedInstance = deserializedData.B;
        Assert.IsFalse (deserializedInstance.OnLoadedHasBeenCalled);
        using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
        {
          ClientTransaction.Current.EnlistDomainObject (deserializedInstance);
          deserializedInstance.Int32Property = 15;
          Assert.IsTrue (deserializedInstance.OnLoadedHasBeenCalled);
          Assert.AreEqual (LoadMode.DataContainerLoadedOnly, deserializedInstance.OnLoadedLoadMode);
        }
      }
    }
  }
}
