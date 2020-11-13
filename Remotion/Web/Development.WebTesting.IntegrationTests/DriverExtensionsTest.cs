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
using System.Text.RegularExpressions;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class DriverExtensionsTest : IntegrationTest
  {
    [Test]
    public void GetBrowserName_ReturnsCorrectName ()
    {
      Assert.That (Driver.GetBrowserName(), Is.EqualTo (Helper.BrowserConfiguration.BrowserName));
    }

    [Test]
    public void GetBrowserName_ReturnsBrowserVersion ()
    {
      Assert.That (ContainsVersionPattern (Driver.GetBrowserVersion()), Is.True);
    }

    [Test]
    public void GetBrowserName_ReturnsWebDriverVersion ()
    {
      Assert.That (ContainsVersionPattern (Driver.GetWebDriverVersion()), Is.True);
    }

    private static bool ContainsVersionPattern (string text)
    {
      return Regex.IsMatch (text, @"(\d+\.){1,3}(\d+)");
    }
  }
}