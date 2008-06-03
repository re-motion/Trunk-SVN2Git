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
using Remotion.Security.UnitTests.Core.SampleDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class FunctionalSecurityContextFactoryTest
  {
    [Test]
    public void Initialize ()
    {
      ISecurityContextFactory factory = new FunctionalSecurityContextFactory (typeof (SecurableObject));

      ISecurityContext context = factory.CreateSecurityContext ();
      Assert.IsNotNull (context);
      Assert.AreEqual ("Remotion.Security.UnitTests.Core.SampleDomain.SecurableObject, Remotion.Security.UnitTests", context.Class);
    }

    [Test]
    public void Serialization ()
    {
      FunctionalSecurityContextFactory factory = new FunctionalSecurityContextFactory (typeof (SecurableObject));
      FunctionalSecurityContextFactory deserializedFactory = Serializer.SerializeAndDeserialize (factory);
      Assert.AreNotSame (factory, deserializedFactory);

      ISecurityContext context1 = factory.CreateSecurityContext ();
      ISecurityContext context2 = deserializedFactory.CreateSecurityContext ();
      Assert.AreNotSame (context1, context2);
      Assert.AreEqual (context1, context2);
    }
  }
}
