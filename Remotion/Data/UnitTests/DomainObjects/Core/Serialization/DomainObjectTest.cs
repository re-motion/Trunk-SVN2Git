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
      
      using (ClientTransaction.CreateBindingTransaction ().EnterNonDiscardingScope ())
      {
        instance = ClassWithAllDataTypes.NewObject();
      }

      instance.Int32Property = 5;
      Tuple<ClientTransaction, ClassWithAllDataTypes> deserializedData =
          Serializer.SerializeAndDeserialize (Tuple.NewTuple (instance.BindingTransaction, instance));
      
      ClassWithAllDataTypes deserializedInstance = deserializedData.B;
      Assert.IsTrue (deserializedInstance.IsBoundToSpecificTransaction);
      Assert.AreSame (deserializedData.A, deserializedInstance.BindingTransaction);
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
        using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
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
