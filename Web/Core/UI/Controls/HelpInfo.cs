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
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  /// The <see cref="HelpInfo"/> type is used to group all information required for displaying a help-link in a form-grid.
  /// </summary>
  public sealed class HelpInfo
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="HelpInfo"/> type.
    /// </summary>
    /// <param name="navigateUrl">The URL assigned to the hyperlinks <c>href</c>-attribute. Must not be <see langword="null" /> or empty.</param>
    public HelpInfo (string navigateUrl)
        : this (navigateUrl, null, null, null)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HelpInfo"/> type.
    /// </summary>
    /// <param name="navigateUrl">The URL assigned to the hyperlinks <c>href</c>-attribute. Must not be <see langword="null" /> or empty.</param>
    /// <param name="target">The target for the <paramref cref="navigateUrl"/>.</param>
    /// <param name="toolTip">The tool-tip to be displayed when hovering over the help-link.</param>
    /// <param name="onClick">The javascript to be executed for the <c>OnClick</c> event on the client.</param>
    public HelpInfo (string navigateUrl, string target, string toolTip, string onClick)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("navigateUrl", navigateUrl);

      NavigateUrl = navigateUrl;
      Target = target;
      OnClick = onClick;
      ToolTip = toolTip;
    }

    /// <summary>
    /// Gets the URL to be rendered. Use the <c>~/</c> notation to specify an URL relative to the application-root.
    /// </summary>
    public string NavigateUrl { get; private set; }

    /// <summary>
    /// Gets the target for the <see cref="NavigateUrl"/>.
    /// </summary>
    public string Target { get; private set; }

    /// <summary>
    /// Gets the javascript to be executed for the <c>OnClick</c> event on the client.
    /// </summary>
    public string OnClick { get; private set; }

    /// <summary>
    /// Gets the tool-tip to be displayed when hovering over the help-link.
    /// </summary>
    public string ToolTip { get; private set; }
  }
}