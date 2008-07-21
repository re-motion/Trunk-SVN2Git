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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Serialization
{
  [TestFixture]
  public class SerializationCallbackTest : ClientTransactionBaseTest
  {
    [Test]
    public void SerializationEvents ()
    {
      ClassWithSerializationCallbacks instance =
          (ClassWithSerializationCallbacks) RepositoryAccessor.NewObject (typeof (ClassWithSerializationCallbacks)).With();

      Assert.AreNotSame (typeof (ClassWithSerializationCallbacks), ((object)instance).GetType ());

      new SerializationCallbackTester<ClassWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter (), instance, ClassWithSerializationCallbacks.SetReceiver)
          .Test_SerializationCallbacks ();
    }

    [Test]
    public void DeserializationEvents ()
    {
      ClassWithSerializationCallbacks instance =
          (ClassWithSerializationCallbacks) RepositoryAccessor.NewObject (typeof (ClassWithSerializationCallbacks)).With();

      Assert.AreNotSame (typeof (ClassWithSerializationCallbacks), ((object) instance).GetType ());

      new SerializationCallbackTester<ClassWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter (), instance, ClassWithSerializationCallbacks.SetReceiver)
          .Test_DeserializationCallbacks ();
    }
  }
}
