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

namespace Remotion.Web.UI.Controls.Rendering.DatePickerButton
{
  public abstract class DatePickerButtonPreRendererBase : PreRendererBase<IDatePickerButton>, IDatePickerButtonPreRenderer
  {
    private static readonly string s_datePickerScriptFileKey = typeof (IDatePickerButton).FullName + "_Script";
    private static readonly string s_datePickerStyleFileKey = typeof (IDatePickerButton).FullName + "_Style";
    private const string c_datePickerScriptFileName = "DatePicker.js";
    private const string c_datePickerStyleFileName = "DatePicker.css";

    protected DatePickerButtonPreRendererBase (IHttpContext context, IDatePickerButton control)
        : base(context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      if (!htmlHeadAppender.IsRegistered (s_datePickerScriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (IDatePickerButton), ResourceType.Html, ResourceTheme.Standard, c_datePickerScriptFileName);
        htmlHeadAppender.RegisterJavaScriptInclude (s_datePickerScriptFileKey, scriptUrl);
      }

      if (!htmlHeadAppender.IsRegistered (s_datePickerStyleFileKey))
      {
        string styleUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (IDatePickerButton), ResourceType.Html, ResourceTheme.Standard, c_datePickerStyleFileName);
        htmlHeadAppender.RegisterStylesheetLink (s_datePickerStyleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
      }
    }
  }
}