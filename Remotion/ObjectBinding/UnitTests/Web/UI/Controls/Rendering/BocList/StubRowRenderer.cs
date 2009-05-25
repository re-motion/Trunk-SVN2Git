// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode;
using Remotion.Web.Infrastructure;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocList
{
  public class StubRowRenderer : BocListRendererBase, IBocRowRenderer
  {
    public StubRowRenderer (IHttpContext context, HtmlTextWriter writer, IBocList list)
        : base(context, writer, list)
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