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
using System.Web.UI;
using Microsoft.Practices.ServiceLocation;
using Remotion.Utilities;
using System.Web;
using Remotion.Web.UI.Controls.Rendering.ListMenu;

namespace Remotion.Web.UI.Controls
{
  public class ListMenu : MenuBase, IListMenu
  {
    private ListMenuLineBreaks _lineBreaks = ListMenuLineBreaks.All;

    public ListMenu (IControl ownerControl, Type[] supportedWebMenuItemTypes)
        : base (ownerControl, supportedWebMenuItemTypes)
    {
      EnableClientScript = true;
    }

    public ListMenu (IControl ownerControl)
        : this (ownerControl, new[] { typeof (WebMenuItem) })
    {
    }

    public ListMenu ()
        : this (null)
    {
    }

    public ListMenuLineBreaks LineBreaks
    {
      get { return _lineBreaks; }
      set { _lineBreaks = value; }
    }

    protected override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
        WcagHelper.Instance.HandleError (1, this);

      var factory = ServiceLocator.Current.GetInstance<IListMenuRendererFactory>();
      var renderer = factory.CreateRenderer (Page.Context, writer, this);
      renderer.Render (writer);
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      if (!IsDesignMode && Page != null)
      {
        RegisterHtmlHeadContents (Page.Context, HtmlHeadAppender.Current);
      }
    }

    public void RegisterHtmlHeadContents (HttpContextBase httpContext, HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("httpContext", httpContext);
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      var factory = ServiceLocator.Current.GetInstance<IListMenuRendererFactory> ();
      var preRenderer = factory.CreatePreRenderer (httpContext, this);
      preRenderer.RegisterHtmlHeadContents (htmlHeadAppender);

    }

    protected override void OnPreRender (EventArgs e)
    {
      var factory = ServiceLocator.Current.GetInstance<IListMenuRendererFactory>();
      var preRenderer = factory.CreatePreRenderer (Page.Context, this);
      preRenderer.PreRender();
    }

    public bool EnableClientScript { get; set; }

    public bool HasClientScript
    {
      get { return !IsDesignMode && EnableClientScript; }
    }
  }
}
