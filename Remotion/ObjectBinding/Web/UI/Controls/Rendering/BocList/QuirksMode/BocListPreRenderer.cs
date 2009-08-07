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
using Remotion.Web;
using Remotion.Web.Infrastructure;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode
{
  public class BocListPreRenderer : BocListPreRendererBase
  {
    public BocListPreRenderer (IHttpContext context, IBocList control, CssClassContainer cssClassContainer)
        : base(context, control, cssClassContainer)
    {
    }

    public override bool IsBrowserCapableOfScripting
    {
      get { return IsInternetExplorer55OrHigher(); }
    }
    
    protected override ResourceTheme ResourceTheme
    {
      get { return ResourceTheme.Legacy; }
    }

    protected virtual bool IsInternetExplorer55OrHigher ()
    {
      if (Control.IsDesignMode)
        return true;

      bool isVersionGreaterOrEqual55 =
          Context.Request.Browser.MajorVersion >= 6
          || Context.Request.Browser.MajorVersion == 5
             && Context.Request.Browser.MinorVersion >= 0.5;
      bool isInternetExplorer55AndHigher =
          Context.Request.Browser.Browser == "IE" && isVersionGreaterOrEqual55;

      return isInternetExplorer55AndHigher;
    }
  }
}