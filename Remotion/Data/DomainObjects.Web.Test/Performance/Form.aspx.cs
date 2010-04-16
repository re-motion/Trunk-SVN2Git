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
using Remotion.Data.DomainObjects.Web.Test.WxeFunctions;
using Remotion.ServiceLocation;
using Remotion.Web;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;

namespace Remotion.Data.DomainObjects.Web.Test.Performance
{
  public partial class Form : WxePage
  {
    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);
      var items = ((PerformanceFunction) CurrentFunction).Items;
      ItemList.LoadUnboundValue (items, !IsPostBack);
    }

    protected override void OnPreRender (EventArgs e)
    {
      string key = GetType ().FullName + "_Style";
      if (!HtmlHeadAppender.Current.IsRegistered (key))
      {
        var themedResourceUrlResolver = SafeServiceLocator.Current.GetInstance<IThemedResourceUrlResolverFactory> ().CreateResourceUrlResolver ();
        string href = themedResourceUrlResolver.GetResourceUrl (this, ResourceType.Html, "Style.css");

        HtmlHeadAppender.Current.RegisterStylesheetLink (key, href);
      }

      base.OnPreRender (e);
    }
  }
}