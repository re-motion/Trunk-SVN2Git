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
using Remotion.Utilities;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls.Rendering.DatePickerButton.QuirksMode
{
  /// <summary>
  /// Responsible for registering the client script file that the <see cref="DatePickerButton"/> depends on in quirks mode.
  /// </summary>
  public class DatePickerButtonPreRenderer : PreRendererBase<IDatePickerButton>, IDatePickerButtonPreRenderer
  {
    private static readonly string s_datePickerScriptFileKey = typeof (IDatePickerButton).FullName + "_Script";
    private static readonly string s_datePickerStyleFileKey = typeof (IDatePickerButton).FullName + "_Style";

    public DatePickerButtonPreRenderer (IHttpContext context, IDatePickerButton control)
        : base (context, control)
    {
    }

    public override void PreRender ()
    {
      
    }

    /// <summary>
    /// Registers the JavaScript file that contains the necessary functions for showing the pop-up calendar and retrieving the date.
    /// </summary>
    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      if (!htmlHeadAppender.IsRegistered (s_datePickerScriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (IDatePickerButton), ResourceType.Html, "Legacy/DatePicker.js");
        htmlHeadAppender.RegisterJavaScriptInclude (s_datePickerScriptFileKey, scriptUrl);
      }

      if (!htmlHeadAppender.IsRegistered (s_datePickerStyleFileKey))
      {
        string styleUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (IDatePickerButton), ResourceType.Html, "Legacy/DatePicker.css");
        htmlHeadAppender.RegisterStylesheetLink (s_datePickerStyleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
      }
    }
  }
}