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
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.Utilities;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls
{
  public class BocListQuirksModeClientScriptBehavior : IClientScriptBahavior
  {
    private readonly HttpContextBase _context;
    private readonly IBocList _control;

    public BocListQuirksModeClientScriptBehavior (HttpContextBase context, IBocList control)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("control", control);

      _context = context;
      _control = control;
    }

    public bool IsBrowserCapableOfScripting
    {
      get { return IsInternetExplorer55OrHigher(); }
    }

    protected virtual bool IsInternetExplorer55OrHigher ()
    {
      if (_control.IsDesignMode)
        return true;

      bool isVersionGreaterOrEqual55 =
          _context.Request.Browser.MajorVersion >= 6
          || _context.Request.Browser.MajorVersion == 5
             && _context.Request.Browser.MinorVersion >= 0.5;
      bool isInternetExplorer55AndHigher =
          _context.Request.Browser.Browser == "IE" && isVersionGreaterOrEqual55;

      return isInternetExplorer55AndHigher;
    }
  }
}