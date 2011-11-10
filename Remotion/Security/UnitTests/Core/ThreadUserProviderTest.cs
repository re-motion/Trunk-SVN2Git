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
using System.Collections.Specialized;
using System.Security.Principal;
using System.Threading;
using NUnit.Framework;
using Remotion.Configuration;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class ThreadUserProviderTest
  {
    private IPrincipalProvider _principalProvider;

    public ThreadUserProviderTest ()
    {
    }

    [SetUp]
    public void SetUp ()
    {
      _principalProvider = new ThreadPrincipalProvider();
    }

    [Test]
    public void Initialize ()
    {
      NameValueCollection config = new NameValueCollection();
      config.Add ("description", "The Description");

      ExtendedProviderBase provider = new ThreadPrincipalProvider ("Provider", config);

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
    }

    [Test]
    public void GetUser ()
    {
      Thread.CurrentPrincipal = new GenericPrincipal (new GenericIdentity ("user"), new string[0]);
      Assert.AreEqual ("user", _principalProvider.GetPrincipal().User);
    }

    [Test]
    public void GetUser_NotAuthenticated ()
    {
      Thread.CurrentPrincipal = new GenericPrincipal (new GenericIdentity (string.Empty), new string[0]);
      Assert.IsFalse (Thread.CurrentPrincipal.Identity.IsAuthenticated);
      Assert.IsTrue (_principalProvider.GetPrincipal ().IsNull);
    }

    [Test]
    public void GetIsNull ()
    {
      Assert.IsFalse (_principalProvider.IsNull);
    }
  }
}
