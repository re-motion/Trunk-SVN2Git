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
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;

namespace Remotion.Development.UnitTests.Core.UnitTesting
{
  [TestFixture]
  public class CultureScopeTest
  {
    [Test]
    public void CultureScopeByNameTest ()
    {
      Thread currentThread = Thread.CurrentThread;
      using (new CultureScope ("de-AT", "uz-Cyrl-UZ"))
      {
        Assert.That (currentThread.CurrentCulture.Name, Is.EqualTo ("de-AT"));
        Assert.That (currentThread.CurrentUICulture.Name, Is.EqualTo ("uz-Cyrl-UZ"));
        using (new CultureScope ("en-GB", "fr-MC"))
        {
          Assert.That (currentThread.CurrentCulture.Name, Is.EqualTo ("en-GB"));
          Assert.That (currentThread.CurrentUICulture.Name, Is.EqualTo ("fr-MC"));
        }
        Assert.That (currentThread.CurrentCulture.Name, Is.EqualTo ("de-AT"));
        Assert.That (currentThread.CurrentUICulture.Name, Is.EqualTo ("uz-Cyrl-UZ"));
      }
    }

    [Test]
    public void CultureScopeByCultureInfoTest ()
    {
      Thread currentThread = Thread.CurrentThread;
      using (new CultureScope ("de-AT", "uz-Cyrl-UZ"))
      {
        Assert.That (currentThread.CurrentCulture.Name, Is.EqualTo ("de-AT"));
        Assert.That (currentThread.CurrentUICulture.Name, Is.EqualTo ("uz-Cyrl-UZ"));
        using (new CultureScope (new CultureInfo ("en-GB", false), new CultureInfo ("fr-MC", false)))
        {
          Assert.That (currentThread.CurrentCulture.Name, Is.EqualTo ("en-GB"));
          Assert.That (currentThread.CurrentUICulture.Name, Is.EqualTo ("fr-MC"));
        }
        Assert.That (currentThread.CurrentCulture.Name, Is.EqualTo ("de-AT"));
        Assert.That (currentThread.CurrentUICulture.Name, Is.EqualTo ("uz-Cyrl-UZ"));
      }
    }
  }
}