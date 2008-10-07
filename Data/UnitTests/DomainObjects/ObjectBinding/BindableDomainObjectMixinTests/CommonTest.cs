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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class CommonTest : ObjectBindingBaseTest
  {
    [Test]
    public void InstantiateMixedType ()
    {
      Assert.That (SampleBindableMixinDomainObject.NewObject(), Is.InstanceOfType (typeof (IBusinessObject)));
    }

    [Test]
    public void SerializeAndDeserialize ()
    {
      SampleBindableMixinDomainObject value = SampleBindableMixinDomainObject.NewObject ();
      Assert.AreNotEqual ("Earl", value.Name);
      value.Name = "Earl";
      Tuple<ClientTransactionMock, SampleBindableMixinDomainObject> deserialized = Serializer.SerializeAndDeserialize (Tuple.NewTuple (ClientTransactionMock, value));

      using (deserialized.A.EnterDiscardingScope ())
      {
        Assert.That (deserialized.B.Name, Is.EqualTo ("Earl"));
        Assert.That (((IBusinessObject) deserialized.B).BusinessObjectClass, Is.SameAs (((IBusinessObject) value).BusinessObjectClass));
      }
    }

    [Test]
    public void SerializeAndDeserialize_WithNewBindableObjectProvider ()
    {
      SampleBindableMixinDomainObject value = SampleBindableMixinDomainObject.NewObject ();
      byte[] serialized = Serializer.Serialize (Tuple.NewTuple (ClientTransactionMock, value));
      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
      Tuple<ClientTransactionMock, SampleBindableMixinDomainObject> deserialized = (Tuple<ClientTransactionMock, SampleBindableMixinDomainObject>) Serializer.Deserialize (serialized);

      using (deserialized.A.EnterDiscardingScope ())
      {
        Assert.That (((IBusinessObject) deserialized.B).BusinessObjectClass, Is.Not.SameAs (((IBusinessObject) value).BusinessObjectClass));
      }
    }
  }
}
