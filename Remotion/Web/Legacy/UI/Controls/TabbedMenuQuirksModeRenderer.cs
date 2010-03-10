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
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.TabbedMenuImplementation;

namespace Remotion.Web.Legacy.UI.Controls
{
  /// <summary>
  /// Implements <see cref="IRenderer"/> for quirks mode rendering of <see cref="TabbedMenu"/> controls.
  /// <seealso cref="ITabbedMenu"/>
  /// </summary>
  public class TabbedMenuQuirksModeRenderer : Web.UI.Controls.TabbedMenuImplementation.Rendering.TabbedMenuRenderer
  {
    public TabbedMenuQuirksModeRenderer (HttpContextBase context, ITabbedMenu control)
        : base(context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      // Do not call base implementation
      //base.RegisterHtmlHeadContents

      string key = typeof (TabbedMenuQuirksModeRenderer).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (key))
      {
        string url = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (TabbedMenuQuirksModeRenderer), ResourceType.Html, "TabbedMenu.css");
        htmlHeadAppender.RegisterStylesheetLink (key, url, HtmlHeadAppender.Priority.Library);
      }
    }
  }
}