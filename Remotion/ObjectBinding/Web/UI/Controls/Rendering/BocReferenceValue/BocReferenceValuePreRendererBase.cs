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

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocReferenceValue
{
  public abstract class BocReferenceValuePreRendererBase : BocPreRendererBase<IBocReferenceValue>, IBocReferenceValuePreRenderer
  {
    protected const string c_styleFileUrl = "BocReferenceValue.css";

    private static readonly string s_startUpScriptKey = typeof (IBocReferenceValue).FullName + "_Startup";
    
    protected BocReferenceValuePreRendererBase (IHttpContext context, IBocReferenceValue control)
        : base (context, control)
    {
    }

    public override void PreRender ()
    {
      if (!Control.IsDesignMode && !Control.Page.ClientScript.IsStartupScriptRegistered (typeof (BocReferenceValuePreRendererBase), s_startUpScriptKey))
      {
        string script = "BocReferenceValue_InitializeGlobals ('" + Control.NullIdentifier + "');";
        Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (BocReferenceValuePreRendererBase), s_startUpScriptKey, script);
      }
    }
  }
}