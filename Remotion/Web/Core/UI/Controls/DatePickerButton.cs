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
using System.Web.UI.WebControls;
using Microsoft.Practices.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls.Rendering.DatePickerButton;

namespace Remotion.Web.UI.Controls
{
  public class DatePickerButton : WebControl, IDatePickerButton
  {
    private readonly Style _datePickerButtonStyle;

    public DatePickerButton ()
    {
      EnableClientScript = true;
      AlternateText = string.Empty;
      _datePickerButtonStyle = new Style();
    }

    public bool IsDesignMode { get; set; }

    public bool EnableClientScript { get; set; }

    public string AlternateText { get; set; }

    public Style DatePickerButtonStyle
    {
      get { return _datePickerButtonStyle; }
    }

    public string ContainerControlID { get; set; }

    public string TargetControlID { get; set; }
    
    public override void RenderControl (HtmlTextWriter writer)
    {
      var factory = ServiceLocator.Current.GetInstance<IDatePickerButtonRendererFactory> ();
      var renderer = factory.CreateRenderer (new HttpContextWrapper (Context), writer, this);
      renderer.Render();
    }

    protected override void OnPreRender (EventArgs e)
    {
      var factory = ServiceLocator.Current.GetInstance<IDatePickerButtonRendererFactory> ();
      var preRenderer = factory.CreatePreRenderer (new HttpContextWrapper (Context), this);
      preRenderer.PreRender ();
    }

    IControl IDatePickerButton.Parent
    {
      get { return (IControl) Parent; }
    }

    IPage IControl.Page
    {
      get { return PageWrapper.CastOrCreate (base.Page); }
    }

    public void RegisterHtmlHeadContents (IHttpContext httpContext, HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("httpContext", httpContext);
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      var factory = ServiceLocator.Current.GetInstance<IDatePickerButtonRendererFactory> ();
      var preRenderer = factory.CreatePreRenderer (httpContext, this);
      preRenderer.RegisterHtmlHeadContents (htmlHeadAppender);
    }
  }
}