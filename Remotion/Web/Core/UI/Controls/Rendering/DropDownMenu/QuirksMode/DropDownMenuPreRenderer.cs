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
using System.Web;

namespace Remotion.Web.UI.Controls.Rendering.DropDownMenu.QuirksMode
{
  /// <summary>
  /// Overrides <see cref="GetBrowserCapableOfScripting"/> to determine if the <see cref="DropDownMenu"/> can be rendered in quirks mode.
  /// <seealso cref="DropDownMenuPreRendererBase"/>
  /// <seealso cref="IDropDownMenu"/>
  /// </summary>
  public class DropDownMenuPreRenderer : DropDownMenuPreRendererBase
  {
    public DropDownMenuPreRenderer (HttpContextBase context, IDropDownMenu control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      throw new NotImplementedException();
    }

    public override bool GetBrowserCapableOfScripting ()
    {
      return IsInternetExplorer55OrHigher();
    }

    private bool IsInternetExplorer55OrHigher ()
    {
      bool isVersionGreaterOrEqual55 =
          Context.Request.Browser.MajorVersion >= 6
          || Context.Request.Browser.MajorVersion == 5
             && Context.Request.Browser.MinorVersion >= 0.5;
      bool isInternetExplorer55AndHigher =
          Context.Request.Browser.Browser == "IE" && isVersionGreaterOrEqual55;

      return isInternetExplorer55AndHigher;
    }
  }
}