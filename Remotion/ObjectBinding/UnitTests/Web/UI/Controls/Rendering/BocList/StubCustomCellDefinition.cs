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
using System.Web.UI.HtmlControls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocList
{
  public class StubCustomCellDefinition : BocCustomColumnDefinitionCell
  {
    protected override Control CreateControl (BocCustomCellArguments arguments)
    {
      return new StubCustomCellControl();
    }

    protected override void Render (HtmlTextWriter writer, BocCustomCellRenderArguments arguments)
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Div);
      writer.RenderEndTag();
    }
  }

  public class StubCustomCellControl : HtmlGenericControl, IControl
  {
    public StubCustomCellControl () : base ("div")
    {
      Attributes.Add ("class", "mockedCustomCellControl");
    }
  }
}