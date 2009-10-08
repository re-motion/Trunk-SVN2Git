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
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering
{
  /// <summary>
  /// The <see cref="BocPreRendererBase{TControl}"/> is a base class for all pre-renderers used by the business object controls.
  /// </summary>
  /// <typeparam name="TControl">The type of the control being rendered.</typeparam>
  public abstract class BocPreRendererBase<TControl> : PreRendererBase<TControl>
      where TControl: IBusinessObjectBoundWebControl
  {
    private static readonly string s_browserCompatibilityScriptFileKey = typeof (BocPreRendererBase<>).FullName + "_BrowserCompatibilityScript";

    protected BocPreRendererBase (IHttpContext context, TControl control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude (Control);

      if (!htmlHeadAppender.IsRegistered (s_browserCompatibilityScriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (BocPreRendererBase<>), ResourceType.Html, "BrowserCompatibility.js");
        htmlHeadAppender.RegisterJavaScriptInclude (s_browserCompatibilityScriptFileKey, scriptUrl);
      }
    }
  }
}