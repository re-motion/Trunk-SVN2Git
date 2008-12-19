// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
