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
    private IUserProvider _userProvider;

    public ThreadUserProviderTest ()
    {
    }

    [SetUp]
    public void SetUp ()
    {
      _userProvider = new ThreadUserProvider();
    }

    [Test]
    public void Initialize ()
    {
      NameValueCollection config = new NameValueCollection();
      config.Add ("description", "The Description");

      ExtendedProviderBase provider = new ThreadUserProvider ("Provider", config);

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
    }

    [Test]
    public void GetUser ()
    {
      Thread.CurrentPrincipal = new GenericPrincipal (new GenericIdentity ("user"), new string[0]);
      Assert.AreEqual ("user", _userProvider.GetUser().User);
    }

    [Test]
    public void GetUser_WithEmptyIdentity ()
    {
      Thread.CurrentPrincipal = new GenericPrincipal (new GenericIdentity (string.Empty), new string[0]);
      Assert.IsTrue (_userProvider.GetUser().IsNull);
    }

    [Test]
    public void GetIsNull ()
    {
      Assert.IsFalse (_userProvider.IsNull);
    }
  }
}