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
using NUnit.Framework;
using Remotion.Security;
using Remotion.Web.Security;

namespace Remotion.Web.UnitTests.Security
{
  [TestFixture]
  public class HttpContextUserProviderTest
  {
    [Test]
    public void Initialize ()
    {
      NameValueCollection config = new NameValueCollection ();
      config.Add ("description", "The Description");

      HttpContextUserProvider provider = new HttpContextUserProvider ("Provider", config);

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
    }
    
    [Test]
    public void GetIsNull()
    {
      IUserProvider _userProvider = new HttpContextUserProvider();
      Assert.IsFalse (_userProvider.IsNull);
    }
  }
}
