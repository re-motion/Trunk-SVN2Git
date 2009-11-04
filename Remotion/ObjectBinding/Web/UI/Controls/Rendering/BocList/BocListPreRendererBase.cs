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
using Remotion.Web.Infrastructure;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList
{
  public abstract class BocListPreRendererBase : BocPreRendererBase<IBocList>, IBocListPreRenderer
  {
    private static readonly string s_startUpScriptKey = typeof (IBocList).FullName + "_Startup";

    private readonly CssClassContainer _cssClasses;

    protected BocListPreRendererBase (IHttpContext context, IBocList control, CssClassContainer cssClassContainer)
        : base (context, control)
    {
      _cssClasses = cssClassContainer;
    }

    public override void PreRender ()
    {
      if (!Control.HasClientScript)
        return;

      // Startup script initalizing the global values of the script.
      if (!Control.Page.ClientScript.IsStartupScriptRegistered (typeof (BocListPreRendererBase), s_startUpScriptKey))
      {
        string script = string.Format (
            "BocList_InitializeGlobals ('{0}', '{1}');",
            CssClasses.DataRow,
            CssClasses.DataRowSelected);
        Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (BocListPreRendererBase), s_startUpScriptKey, script);
      }
    }

    public abstract bool IsBrowserCapableOfScripting { get; }

    protected CssClassContainer CssClasses
    {
      get { return _cssClasses; }
    }
  }
}
