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
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls.Rendering.DatePickerButton.QuirksMode
{
  /// <summary>
  /// Responsible for registering the client script file that the <see cref="DatePickerButton"/> depends on in quirks mode.
  /// </summary>
  public class DatePickerButtonPreRenderer : PreRendererBase<IDatePickerButton>, IDatePickerButtonPreRenderer
  {
    private const string c_datePickerScriptFileName = "DatePicker.js";

    private static readonly string s_datePickerScriptFileKey = typeof (Controls.DatePickerButton).FullName + "_Script";

    public DatePickerButtonPreRenderer (IHttpContext context, IDatePickerButton control)
        : base (context, control)
    {
    }

    /// <summary>
    /// Registers the JavaScript file that contains the necessary functions for showing the pop-up calendar and retrieving the date.
    /// </summary>
    public override void PreRender ()
    {
      if (HtmlHeadAppender.Current.IsRegistered (ScriptFileKey))
        return;

      string scriptUrl = ResourceUrlResolver.GetResourceUrl (Control, Context, typeof (DatePickerPage), ResourceType.Html, ScriptFileName);
      HtmlHeadAppender.Current.RegisterJavaScriptInclude (ScriptFileKey, scriptUrl);
    }

    /// <summary>
    /// Gets the key with which to register the script file.
    /// </summary>
    public string ScriptFileKey
    {
      get { return s_datePickerScriptFileKey; }
    }

    /// <summary>
    /// Gets the name of the script file.
    /// </summary>
    public string ScriptFileName
    {
      get { return c_datePickerScriptFileName; }
    }
  }
}