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
