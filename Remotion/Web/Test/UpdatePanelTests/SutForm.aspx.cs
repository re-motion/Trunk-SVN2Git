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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;

namespace Remotion.Web.Test.UpdatePanelTests
{
  public partial class SutForm : WxePage
  {
    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      Remotion.Web.UI.HtmlHeadAppender.Current.RegisterStylesheetLink (
          "style",
          Remotion.Web.ResourceUrlResolver.GetResourceUrl (this, typeof (WxePage), Remotion.Web.ResourceType.Html, "Style.css"));
      Remotion.Web.UI.HtmlHeadAppender.Current.RegisterStylesheetLink (
          "fontsize080",
          Remotion.Web.ResourceUrlResolver.GetResourceUrl (this, typeof (WxePage), Remotion.Web.ResourceType.Html, "FontSize080.css"));
    }
  }
}
