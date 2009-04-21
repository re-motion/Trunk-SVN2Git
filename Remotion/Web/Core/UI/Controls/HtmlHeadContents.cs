// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   When added to the webform (inside the head element), the <see cref="HtmlHeadContents"/> 
///   control renderes the controls registered with <see cref="HtmlHeadAppender"/>.
/// </summary>
[ToolboxData ("<{0}:HtmlHeadContents runat=\"server\" id=\"HtmlHeadContents\"></{0}:HtmlHeadContents>")]
public class HtmlHeadContents : Control
{
  private static bool s_isDesignMode;

  protected override void OnInit (EventArgs e)
  {
    base.OnInit (e);
    s_isDesignMode = ControlHelper.IsDesignMode (this);
  }

  protected override void Render(HtmlTextWriter writer)
  {
    HtmlHeadAppender.Current.EnsureAppended (this);

    //  Don't render tags for this control.
    RenderChildren (writer);
  }

  protected override void RenderChildren(HtmlTextWriter writer)
  {
    bool isTextXml = false;

    if (!ControlHelper.IsDesignMode (this))
      isTextXml = ControlHelper.IsXmlConformResponseTextRequired (Context);

    foreach (Control control in Controls)
    {
      HtmlGenericControl genericControl = control as HtmlGenericControl;
      if (genericControl != null)
      {
        //  <link ...> has no closing tags.
        if (string.Compare (genericControl.TagName, "link", true) == 0)
        {
          writer.WriteBeginTag ("link");
          foreach (string attributeKey in genericControl.Attributes.Keys)
            writer.WriteAttribute (attributeKey, genericControl.Attributes[attributeKey]);
          if (isTextXml)
            writer.WriteLineNoTabs (" />");
          else
            writer.WriteLineNoTabs (">");
        }
        else
        {
          control.RenderControl(writer);
          writer.WriteLine();
        }
      }
      else
      {
        control.RenderControl(writer);
        writer.WriteLine();
      }
    }
  }

  protected internal static bool IsDesignMode
  {
    get { return s_isDesignMode; }
  }

}

}
