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
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Web;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;
using Remotion.Web.Infrastructure;

namespace OBWTest
{

[MultiLingualResources ("OBWTest.Globalization.TestBasePage")]
public class TestWxeBasePage:
    WxePage,
    Remotion.Web.UI.Controls.IControl,
    IObjectWithResources //  Provides the WebForm's ResourceManager via GetResourceManager() 
    // IResourceUrlResolver //  Provides the URLs for this WebForm (e.g. to the FormGridManager)
{  
  private Button _nextButton = new Button();

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

      _nextButton.ID = "NextButton";
      _nextButton.Text = "Next";
      WxeControls.AddAt (0, _nextButton);
    }

    ShowAbortConfirmation = ShowAbortConfirmation.Always;
    EnableAbort = false;
    base.OnInit (e);
    RegisterEventHandlers();
  }

  protected override void OnPreRender(EventArgs e)
  {
    //  A call to the ResourceDispatcher to get have the automatic resources dispatched
    ResourceDispatcher.Dispatch (this, ResourceManagerUtility.GetResourceManager (this));

    base.OnPreRender (e);

    string key = GetType().FullName + "_Style";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      string url = ResourceUrlResolver.GetResourceUrl (
          this,
          new HttpContextWrapper(Context), 
          typeof (ResourceUrlResolver),
          ResourceType.Html,
          Global.UseStandardModeRendering ? ResourceTheme.Standard : ResourceTheme.Legacy,
          "Style.css");
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, url);
    }

    key = GetType().FullName + "_Global";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, "Html/global.css");
    }


    LiteralControl stack = new LiteralControl();

    StringBuilder sb = new StringBuilder();
    sb.Append ("<br><div>");
    sb.Append ("<b>Stack:</b><br>");
    for (WxeStep step = CurrentPageStep; step != null; step = step.ParentStep)
      sb.AppendFormat ("{0}<br>", step.ToString());      
    sb.Append ("</div>");
    stack.Text = sb.ToString();
    
    WxeControls.Add (stack);
  }

  protected virtual void RegisterEventHandlers()
  {
    _nextButton.Click += new EventHandler(NextButton_Click);
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

//  public string GetResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
//  {
//    if (ControlHelper.IsDesignMode (this, this.Context))
//      return resourceType.Name + "/" + relativeUrl;
//    else
//      return Page.ResolveUrl (resourceType.Name + "/" + relativeUrl);
//  }

  private void NextButton_Click(object sender, EventArgs e)
  {
    ExecuteNextStep();
  }

  protected virtual ControlCollection WxeControls
  {
    get { return WxeForm.Controls; }
  }
}

}
