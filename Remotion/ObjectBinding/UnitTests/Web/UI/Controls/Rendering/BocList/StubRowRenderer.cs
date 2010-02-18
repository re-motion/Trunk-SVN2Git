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
// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode;
using System.Web;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocList
{
  public class StubRowRenderer : BocListRendererBase, IBocRowRenderer
  {
    public StubRowRenderer (HttpContextBase context, HtmlTextWriter writer, IBocList list)
        : base(context, writer, list, CssClassContainer.Instance)
    {
    }

    public void RenderTitlesRow ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, "titleStub");
      Writer.RenderBeginTag ("tr");
      Writer.RenderEndTag();
    }

    public void RenderEmptyListDataRow ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, "emptyStub");
      Writer.RenderBeginTag ("tr");
      Writer.RenderEndTag ();
    }

    public void RenderDataRow (IBusinessObject businessObject, int rowIndex, int absoluteRowIndex, int originalRowIndex)
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, "dataStub");
      Writer.RenderBeginTag ("tr");
      Writer.RenderEndTag ();
    }
  }
}
