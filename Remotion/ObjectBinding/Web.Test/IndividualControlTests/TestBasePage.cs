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
using System.Globalization;
using System.Threading;
using System.Web;
using Remotion.Globalization;
using Remotion.Web;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;
using Remotion.Web.Infrastructure;

namespace OBWTest.IndividualControlTests
{

[MultiLingualResources ("OBWTest.Globalization.TestBasePage")]
public class TestBasePage :
    WxePage, 
    Remotion.Web.UI.Controls.IControl,
    IObjectWithResources //  Provides the WebForm's ResourceManager via GetResourceManager() 
{  
  protected new TestFunction CurrentFunction
  {
    get { return (TestFunction) base.CurrentFunction; }
  }

  protected override void OnInit(EventArgs e)
  {
    if (! ControlHelper.IsDesignMode (this, Context))
    {
      try
      {
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Request.UserLanguages[0]);
      }
      catch (ArgumentException)
      {}
      try
      {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(Request.UserLanguages[0]);
      }
      catch (ArgumentException)
      {}
    }

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
      string href = ResourceUrlResolver.GetResourceUrl (
          this,
          new HttpContextWrapper(Context),
          typeof (ResourceUrlResolver),
          ResourceType.Html,
          Global.PreferStandardModeRendering ? ResourceTheme.Standard : ResourceTheme.Legacy,
          "Style.css");
      
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, href);
    }

    key = GetType().FullName + "_Global";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      string href = UrlUtility.GetAbsoluteUrl (Page, "~/Html/global.css", false);
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
