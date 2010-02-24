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
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocList
{
  public class StubRowRenderer : IBocRowRenderer
  {
    public StubRowRenderer ()
    {
    }

    public void RenderTitlesRow (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      writer.AddAttribute (HtmlTextWriterAttribute.Class, "titleStub");
      writer.RenderBeginTag ("tr");
      writer.RenderEndTag();
    }

    public void RenderEmptyListDataRow (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      writer.AddAttribute (HtmlTextWriterAttribute.Class, "emptyStub");
      writer.RenderBeginTag ("tr");
      writer.RenderEndTag();
    }

    public void RenderDataRow (HtmlTextWriter writer, IBusinessObject businessObject, int rowIndex, int absoluteRowIndex, int originalRowIndex)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      writer.AddAttribute (HtmlTextWriterAttribute.Class, "dataStub");
      writer.RenderBeginTag ("tr");
      writer.RenderEndTag();
    }
  }
}