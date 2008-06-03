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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   A <see cref="HyperLink"/> that provides integration into the <see cref="ISmartNavigablePage"/> framework by
///   automatically appending the navigation URL parameters to the rendered <see cref="HyperLink.NavigateUrl"/>.
/// </summary>
public class SmartHyperLink : HyperLink
{
	public SmartHyperLink()
	{
	}

  /// <summary> 
  ///   Uses <see cref="ISmartNavigablePage.AppendNavigationUrlParameters"/> to include the navigation URL parameters
  ///   with the rendered <see cref="HyperLink.NavigateUrl"/>.
  /// </summary>
  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    string navigateUrlBackup = NavigateUrl;
    bool hasNavigateUrl = ! StringUtility.IsNullOrEmpty (NavigateUrl);
    bool isDesignMode = ControlHelper.IsDesignMode (this);

    if (! isDesignMode && Page is ISmartNavigablePage && hasNavigateUrl)
      NavigateUrl = ((ISmartNavigablePage) Page).AppendNavigationUrlParameters (NavigateUrl);

    base.AddAttributesToRender (writer);
    
    if (hasNavigateUrl)
      NavigateUrl = navigateUrlBackup;
  }
}

}
