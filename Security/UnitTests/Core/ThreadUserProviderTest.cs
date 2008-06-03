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
using System.Collections.Specialized;
using System.Threading;
using NUnit.Framework;
using Remotion.Configuration;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class ThreadUserProviderTest
  {
    // types

    // static members

    // member fields

    private IUserProvider _userProvider;

    // construction and disposing

    public ThreadUserProviderTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _userProvider = new ThreadUserProvider ();
    }

    [Test]
    public void Initialize ()
    {
      NameValueCollection config = new NameValueCollection ();
      config.Add ("description", "The Description");

      ExtendedProviderBase provider = new ThreadUserProvider ("Provider", config);

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
    }

    [Test]
    public void GetUser ()
    {
      Assert.AreSame (Thread.CurrentPrincipal, _userProvider.GetUser ());
    }
    
    [Test]
    public void GetIsNull ()
    {
      Assert.IsFalse (_userProvider.IsNull);
    }
  }
}
