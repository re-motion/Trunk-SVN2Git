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
using System.IO;
using System.Web.UI;
using NUnit.Framework;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.Core.UI.Controls
{
  [TestFixture]
  public class CommandInfoTest
  {
    [Test]
    public void CreateForPostBack ()
    {
      var commandInfo = CommandInfo.CreateForPostBack ("TheTitle", "ClickHandler");

      Assert.That (commandInfo.Title, Is.EqualTo ("TheTitle"));
      Assert.That (commandInfo.OnClick, Is.EqualTo ("ClickHandler"));
      Assert.That (commandInfo.Href, Is.EqualTo ("#"));
      Assert.That (commandInfo.Target, Is.Null);
    }

    [Test]
    public void CreateForPostBack_OnClickOnly ()
    {
      var commandInfo = CommandInfo.CreateForPostBack (null, "ClickHandler");

      Assert.That (commandInfo.Title, Is.Null);
      Assert.That (commandInfo.OnClick, Is.EqualTo ("ClickHandler"));
      Assert.That (commandInfo.Href, Is.EqualTo ("#"));
      Assert.That (commandInfo.Target, Is.Null);
    }

    [Test]
    public void CreateForLink ()
    {
      var commandInfo = CommandInfo.CreateForLink ("TheTitle", "Url", "TheTarget", "ClickHandler");

      Assert.That (commandInfo.Title, Is.EqualTo ("TheTitle"));
      Assert.That (commandInfo.OnClick, Is.EqualTo ("ClickHandler"));
      Assert.That (commandInfo.Href, Is.EqualTo ("Url"));
      Assert.That (commandInfo.Target, Is.EqualTo ("TheTarget"));
    }

    [Test]
    public void CreateForLink_HrefOnly ()
    {
      var commandInfo = CommandInfo.CreateForLink (null, "Url", null, null);

      Assert.That (commandInfo.Title, Is.Null);
      Assert.That (commandInfo.OnClick, Is.Null);
      Assert.That (commandInfo.Href, Is.EqualTo ("Url"));
      Assert.That (commandInfo.Target, Is.Null);
    }

    [Test]
    public void AddAttributesToRender ()
    {
      var commandInfo = CommandInfo.CreateForLink ("TheTitle", "Url", "TheTarget", "ClickHandler");

      var stringWriter = new StringWriter();
      var htmlTextWriter = new HtmlTextWriter (stringWriter);

      commandInfo.AddAttributesToRender (htmlTextWriter);

      htmlTextWriter.RenderBeginTag (HtmlTextWriterTag.A);
      htmlTextWriter.RenderEndTag();

      var result = stringWriter.ToString();

      Assert.That (result, Is.StringContaining ("title=\"TheTitle\""));
      Assert.That (result, Is.StringContaining ("onclick=\"ClickHandler\""));
      Assert.That (result, Is.StringContaining ("href=\"Url\""));
      Assert.That (result, Is.StringContaining ("target=\"TheTarget\""));
    }

    [Test]
    public void AddAttributesToRender_HrefOnly ()
    {
      var commandInfo = CommandInfo.CreateForLink (null, "Url", null, null);

      var stringWriter = new StringWriter();
      var htmlTextWriter = new HtmlTextWriter (stringWriter);

      commandInfo.AddAttributesToRender (htmlTextWriter);

      htmlTextWriter.RenderBeginTag (HtmlTextWriterTag.A);
      htmlTextWriter.RenderEndTag();

      var result = stringWriter.ToString();

      Assert.That (result, Is.Not.StringContaining ("title="));
      Assert.That (result, Is.Not.StringContaining ("onclick="));
      Assert.That (result, Is.StringContaining ("href=\"Url\""));
      Assert.That (result, Is.Not.StringContaining ("target="));
    }

    [Test]
    public void AddAttributesToRender_OnClickOnly ()
    {
      var commandInfo = CommandInfo.CreateForPostBack (null, "ClickHandler");

      var stringWriter = new StringWriter();
      var htmlTextWriter = new HtmlTextWriter (stringWriter);

      commandInfo.AddAttributesToRender (htmlTextWriter);

      htmlTextWriter.RenderBeginTag (HtmlTextWriterTag.A);
      htmlTextWriter.RenderEndTag();

      var result = stringWriter.ToString();

      Assert.That (result, Is.Not.StringContaining ("title="));
      Assert.That (result, Is.StringContaining ("onclick=\"ClickHandler\""));
      Assert.That (result, Is.StringContaining ("href=\"#\""));
      Assert.That (result, Is.Not.StringContaining ("target="));
    }

    [Test]
    public void AddAttributesToRender_EncodesTitle ()
    {
      var commandInfo = CommandInfo.CreateForLink ("TheTitle\"Space'", "Url", "TheTarget", "ClickHandler");

      var stringWriter = new StringWriter ();
      var htmlTextWriter = new HtmlTextWriter (stringWriter);

      commandInfo.AddAttributesToRender (htmlTextWriter);

      htmlTextWriter.RenderBeginTag (HtmlTextWriterTag.A);
      htmlTextWriter.RenderEndTag ();

      var result = stringWriter.ToString ();

      Assert.That (result, Is.StringContaining ("title=\"TheTitle&quot;Space&#39;\""));
    }

    [Test]
    public void AddAttributesToRender_EncodesOnClick ()
    {
      var commandInfo = CommandInfo.CreateForLink ("TheTitle", "Url", "TheTarget", "ClickHandler\"Space'");

      var stringWriter = new StringWriter ();
      var htmlTextWriter = new HtmlTextWriter (stringWriter);

      commandInfo.AddAttributesToRender (htmlTextWriter);

      htmlTextWriter.RenderBeginTag (HtmlTextWriterTag.A);
      htmlTextWriter.RenderEndTag ();

      var result = stringWriter.ToString ();

      Assert.That (result, Is.StringContaining ("onclick=\"ClickHandler&quot;Space&#39;\""));
    }

    [Test]
    public void AddAttributesToRender_EncodesHref ()
    {
      var commandInfo = CommandInfo.CreateForLink ("TheTitle", "Url\"Space'", "TheTarget", "ClickHandler");

      var stringWriter = new StringWriter ();
      var htmlTextWriter = new HtmlTextWriter (stringWriter);

      commandInfo.AddAttributesToRender (htmlTextWriter);

      htmlTextWriter.RenderBeginTag (HtmlTextWriterTag.A);
      htmlTextWriter.RenderEndTag ();

      var result = stringWriter.ToString ();

      Assert.That (result, Is.StringContaining ("href=\"Url&quot;Space&#39;\""));
    }

    [Test]
    public void AddAttributesToRender_DoesNotEncodeTarget ()
    {
      var commandInfo = CommandInfo.CreateForLink ("TheTitle", "Url", "TheTarget\"Space'", "ClickHandler");

      var stringWriter = new StringWriter ();
      var htmlTextWriter = new HtmlTextWriter (stringWriter);

      commandInfo.AddAttributesToRender (htmlTextWriter);

      htmlTextWriter.RenderBeginTag (HtmlTextWriterTag.A);
      htmlTextWriter.RenderEndTag ();

      var result = stringWriter.ToString ();

      Assert.That (result, Is.StringContaining ("target=\"TheTarget\"Space'\""));
    }
  }
}