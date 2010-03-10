// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.Utilities;

namespace Remotion.Web.UnitTests.Utilities
{
  [TestFixture]
  public class HtmlUtilityTest
  {
    [Test]
    public void HtmlEncode_DoubleQuote ()
    {
      Assert.That (HtmlUtility.HtmlEncode ("a\"b"), Is.EqualTo ("a&quot;b"));
    }

    [Test]
    public void HtmlEncode_SingleQuote ()
    {
      Assert.That (HtmlUtility.HtmlEncode ("a'b"), Is.EqualTo ("a&#039;b"));
    }

    [Test]
    public void HtmlEncode_NewLine ()
    {
      Assert.That (HtmlUtility.HtmlEncode ("a\nb"), Is.EqualTo ("a<br />b"));
    }

    [Test]
    public void HtmlEncode_LineFeed ()
    {
      Assert.That (HtmlUtility.HtmlEncode ("a\rb"), Is.EqualTo ("a<br />b"));
    }

    [Test]
    public void HtmlEncode_LineFeedNewLine ()
    {
      Assert.That (HtmlUtility.HtmlEncode ("a\r\nb"), Is.EqualTo ("a<br />b"));
    }
  }
}