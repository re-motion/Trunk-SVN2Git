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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using System.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode
{
  /// <summary>
  /// Abstract base class for BocList renderers. Defines common constants, properties and utility methods.
  /// </summary>
  public abstract class BocListRendererBase : BocRendererBase<IBocList>
  {
    private readonly CssClassContainer _cssClasses;

    /// <summary>
    /// Constructor initializing the renderer with the <see cref="BocList"/> rendering object and the
    /// <see cref="HtmlTextWriter"/> rendering target.
    /// </summary>
    /// <remarks>Each <see cref="BocList"/> renderer has to be bound to the list to render and 
    /// the <see cref="HtmlTextWriter"/> target to render it to. Therefore, these properties are <code>readonly</code>
    /// and must be set in the constructor.</remarks>
    /// <param name="context">The <see cref="HttpContextBase"/> that contains the response for which to render the list.</param>
    /// <param name="list">The <see cref="BocList"/> to render.</param>
    /// <param name="cssClasses">The <see cref="CssClassContainer"/> containing the CSS classes to apply to the rendered elements.</param>
    protected BocListRendererBase (HttpContextBase context, IBocList list, CssClassContainer cssClasses)
        : base(context, list)
    {
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);
      _cssClasses = cssClasses;
    }

    public CssClassContainer CssClasses
    {
      get { return _cssClasses; }
    }

    /// <summary>Gets the <see cref="BocList"/> object that will be rendered.</summary>
    public IBocList List
    {
      get { return Control; }
    }

    public override sealed string CssClassBase
    {
      get { return CssClasses.Base; }
    }

    public override sealed string CssClassDisabled
    {
      get { return CssClasses.Disabled; }
    }

    public override sealed string CssClassReadOnly
    {
      get { return CssClasses.ReadOnly; }
    }
  }
}
