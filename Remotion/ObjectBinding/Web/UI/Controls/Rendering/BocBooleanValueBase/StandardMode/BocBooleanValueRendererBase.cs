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
using System.Web.UI;
using Remotion.Web.Infrastructure;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocBooleanValueBase.StandardMode
{
  /// <summary>
  /// Base class for the renderers in the same namespace. Contains common constants and methods.
  /// <seealso cref="BocCheckboxRenderer"/>
  /// <seealso cref="BocBooleanValueRenderer"/>
  /// </summary>
  /// <typeparam name="T">The concrete control or corresponding interface that will be rendered.</typeparam>
  public abstract class BocBooleanValueRendererBase<T> : BocRendererBase<T>
      where T: IBocBooleanValueBase
  {
    private const string c_defaultControlWidth = "100pt";

    protected BocBooleanValueRendererBase (IHttpContext context, HtmlTextWriter writer, T control)
        : base (context, writer, control)
    {
    }

    public abstract void Render();

    protected override void AddAdditionalAttributes()
    {
      Writer.AddStyleAttribute (HtmlTextWriterStyle.Display, "inline-block");
      Writer.AddStyleAttribute ("white-space", "nowrap");
      if (!Control.IsReadOnly)
      {
        bool isControlWidthEmpty = Control.Width.IsEmpty && string.IsNullOrEmpty (Control.Style["width"]);
        bool isLabelWidthEmpty = Control.LabelStyle.Width.IsEmpty;
        if (isLabelWidthEmpty && isControlWidthEmpty)
          Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
      }
    }
  }
}