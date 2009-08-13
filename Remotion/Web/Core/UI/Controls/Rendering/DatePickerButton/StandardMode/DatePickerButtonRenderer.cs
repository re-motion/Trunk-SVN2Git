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
using System.Web.UI.WebControls;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls.Rendering.DatePickerButton.StandardMode
{
  /// <summary>
  /// Responsible for rendering a <see cref="DatePickerButton"/> control in quirks mode.
  /// <seealso cref="IDatePickerButton"/>
  /// </summary>
  public class DatePickerButtonRenderer: DatePickerButtonRendererBase
  {
    public DatePickerButtonRenderer (IHttpContext context, HtmlTextWriter writer, IDatePickerButton control)
        : base (context, writer, control)
    {
    }

    protected override bool DetermineClientScriptLevel ()
    {
      return true;
    }

    protected override Unit PopUpWidth
    {
      get { return new Unit (14, UnitType.Em); }
    }

    protected override Unit PopUpHeight
    {
      get { return new Unit (16, UnitType.Em); }
    }
  }
}