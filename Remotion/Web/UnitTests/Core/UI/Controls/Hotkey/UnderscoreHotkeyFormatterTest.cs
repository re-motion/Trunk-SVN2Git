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
using NUnit.Framework;
using Remotion.Web.UI.Controls.Hotkey;

namespace Remotion.Web.UnitTests.Core.UI.Controls.Hotkey
{
  [TestFixture]
  public class UnderscoreHotkeyFormatterTest
  {
    [Test]
    public void Format_NoEncoding_NoHotkey ()
    {
      var formatter = new UnderscoreHotkeyFormatter();
      var textWithHotkey = new TextWithHotkey ("f<b>o</b>o b&ar", null);

      Assert.That (formatter.Format (textWithHotkey, false), Is.EqualTo ("f<b>o</b>o b&ar"));
    }

    [Test]
    public void Format_WithHtmlEncoding_NoHotkey ()
    {
      var formatter = new UnderscoreHotkeyFormatter();
      var textWithHotkey = new TextWithHotkey ("f<b>o</b>o b&ar", null);

      Assert.That (formatter.Format (textWithHotkey, true), Is.EqualTo ("f&lt;b&gt;o&lt;/b&gt;o b&amp;ar"));
    }

    [Test]
    public void Format_NoEncoding_WithHotkey ()
    {
      var formatter = new UnderscoreHotkeyFormatter();
      var textWithHotkey = new TextWithHotkey ("foo b&ar", 4);

      Assert.That (formatter.Format (textWithHotkey, false), Is.EqualTo ("foo <u>b</u>&ar"));
    }

    [Test]
    public void Format_NoEncoding_WithHotkeyAtStart ()
    {
      var formatter = new UnderscoreHotkeyFormatter();
      var textWithHotkey = new TextWithHotkey ("foo bar", 0);

      Assert.That (formatter.Format (textWithHotkey, false), Is.EqualTo ("<u>f</u>oo bar"));
    }

    [Test]
    public void Format_NoEncoding_WithHotkeyAtEnd ()
    {
      var formatter = new UnderscoreHotkeyFormatter();
      var textWithHotkey = new TextWithHotkey ("foo bar", 6);

      Assert.That (formatter.Format (textWithHotkey, false), Is.EqualTo ("foo ba<u>r</u>"));
    }

    [Test]
    public void Format_WithHtmlEncoding_WithHotkey ()
    {
      var formatter = new UnderscoreHotkeyFormatter();
      var textWithHotkey = new TextWithHotkey ("f<b>o</b>o b&ar", 11);

      Assert.That (formatter.Format (textWithHotkey, true), Is.EqualTo ("f&lt;b&gt;o&lt;/b&gt;o <u>b</u>&amp;ar"));
    }

    [Test]
    public void Format_WithHtmlEncoding_WithEncodedHotkey ()
    {
      var formatter = new UnderscoreHotkeyFormatter();
      var textWithHotkey = new TextWithHotkey ("foo öar", 4);

      Assert.That (formatter.Format (textWithHotkey, true), Is.EqualTo ("foo <u>&#246;</u>ar"));
    }
  }
}