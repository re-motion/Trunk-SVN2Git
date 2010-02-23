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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode;
using System.Web;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocList
{
  public class StubColumnRenderer : BocColumnRendererBase<StubColumnDefinition>
  {
    public StubColumnRenderer (HttpContextBase context, IBocList list, StubColumnDefinition columnDefinition)
        : base (context, list, columnDefinition, CssClassContainer.Instance)
    {
    }

    public override void RenderTitleCell (HtmlTextWriter writer, SortingDirection sortingDirection, int orderIndex)
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Th);
      writer.RenderEndTag();
    }

    public override void RenderDataCell (HtmlTextWriter writer, int rowIndex, bool showIcon, BocListDataRowRenderEventArgs dataRowRenderEventArgs)
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      writer.RenderEndTag();
    }

    protected override void RenderCellContents (HtmlTextWriter writer, BocListDataRowRenderEventArgs dataRowRenderEventArgs, int rowIndex, bool showIcon)
    {
      throw new NotImplementedException();
    }
  }
}
