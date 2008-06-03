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
using Remotion.Web.UI;

namespace Remotion.Web.Test
{
  public class SutBasePage : SmartPage
  {
    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      Remotion.Web.UI.HtmlHeadAppender.Current.RegisterStylesheetLink (
          "style",
          Remotion.Web.ResourceUrlResolver.GetResourceUrl (this, typeof (SmartPage), Remotion.Web.ResourceType.Html, "Style.css"));
      Remotion.Web.UI.HtmlHeadAppender.Current.RegisterStylesheetLink (
          "fontsize080",
          Remotion.Web.ResourceUrlResolver.GetResourceUrl (this, typeof (SmartPage), Remotion.Web.ResourceType.Html, "FontSize080.css"));
    }
  }
}
