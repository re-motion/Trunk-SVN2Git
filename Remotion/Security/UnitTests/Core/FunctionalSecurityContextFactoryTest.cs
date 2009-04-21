// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
