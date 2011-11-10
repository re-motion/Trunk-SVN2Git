// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Web;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace OBWTest
{
  [MultiLingualResources ("OBWTest.Globalization.TestBasePage")]
  public class TestBasePage<TFunction> :
      WxePage, 
      Remotion.Web.UI.Controls.IControl,
      IObjectWithResources //  Provides the WebForm's ResourceManager via GetResourceManager() 
      where TFunction : WxeFunction
  {
    public TestBasePage ()
    {
    }

    protected override void OnPreInit (EventArgs e)
    {
      MasterPageFile = (Global.PreferQuirksModeRendering) ? "~/QuirksMode.Master" : "~/StandardMode.Master";
      base.OnPreInit (e);
    }

    protected new TFunction CurrentFunction
    {
      get { return (TFunction) base.CurrentFunction; }
    }

    protected override void OnInit(EventArgs e)
    {
      RegisterEventHandlers();
      base.OnInit (e);
    }

    protected override void OnPreRender(EventArgs e)
    {
      //  A call to the ResourceDispatcher to get have the automatic resources dispatched
      ResourceDispatcher.Dispatch (this, ResourceManagerUtility.GetResourceManager (this));

      string key = GetType().FullName + "_Style";
      if (! HtmlHeadAppender.Current.IsRegistered (key))
      {
        var themedResourceUrlResolver = SafeServiceLocator.Current.GetInstance<IThemedResourceUrlResolverFactory>().CreateResourceUrlResolver();
        string href = themedResourceUrlResolver.GetResourceUrl (this, ResourceType.Html, "Style.css");

        HtmlHeadAppender.Current.RegisterStylesheetLink (key, href);
      }

      key = GetType().FullName + "_Global";
      if (! HtmlHeadAppender.Current.IsRegistered (key))
      {
        string href = ResolveClientUrl ("~/Html/global.css");
        HtmlHeadAppender.Current.RegisterStylesheetLink (key, href);
      }

      base.OnPreRender (e);
    }

    protected virtual void RegisterEventHandlers()
    {
    }

    protected virtual IResourceManager GetResourceManager()
    {
      Type type = GetType();
      if (MultiLingualResources.ExistsResource (type))
        return MultiLingualResources.GetResourceManager (type, true);
      else
        return null;
    }

    IResourceManager IObjectWithResources.GetResourceManager()
    {
      return GetResourceManager();
    }
  }
}