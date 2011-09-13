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
using System.Web;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.Web.Services
{
  public sealed class IconProxy
  {
    private readonly string _url;
    private readonly string _alternateText;
    private readonly string _toolTip;
    private readonly int? _heightInPixels;
    private readonly int? _widthInPixels;

    public static IconProxy Create (HttpContextBase httpContext, IconInfo iconInfo)
    {
      ArgumentUtility.CheckNotNull ("httpContext", httpContext);
      ArgumentUtility.CheckNotNull ("iconInfo", iconInfo);

      int? heightInPixels = null;
      if (!iconInfo.Height.IsEmpty && iconInfo.Height.Type == UnitType.Pixel)
        heightInPixels = (int) iconInfo.Height.Value;

      int? widthInPixels = null;
      if (!iconInfo.Width.IsEmpty && iconInfo.Width.Type == UnitType.Pixel)
        widthInPixels = (int) iconInfo.Width.Value;

      var absoluteUrl = UrlUtility.GetAbsoluteUrl (httpContext, iconInfo.Url);
      return new IconProxy (absoluteUrl, iconInfo.AlternateText, iconInfo.ToolTip, heightInPixels, widthInPixels);
    }

    private IconProxy (string url, string alternateText, string toolTip, int? heightInPixels, int? widthInPixels)
    {
      _url = url;
      _alternateText = alternateText;
      _toolTip = toolTip;
      _heightInPixels = heightInPixels;
      _widthInPixels = widthInPixels;
    }

    public string Url
    {
      get { return _url; }
    }

    public string AlternateText
    {
      get { return _alternateText; }
    }

    public string ToolTip
    {
      get { return _toolTip; }
    }

    public int? HeightInPixels
    {
      get { return _heightInPixels; }
    }

    public int? WidthInPixels
    {
      get { return _widthInPixels; }
    }
  }
}