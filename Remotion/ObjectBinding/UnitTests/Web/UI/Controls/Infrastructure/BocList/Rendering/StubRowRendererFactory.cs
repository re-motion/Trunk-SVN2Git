// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using System.Web.UI;
using Microsoft.Practices.ServiceLocation;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering;
using Remotion.Web.Infrastructure;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList.Rendering
{
  public class StubRowRendererFactory : IBocRowRendererFactory
  {
    public IBocRowRenderer CreateRenderer (IHttpContext context, HtmlTextWriter writer, IBocList list, IServiceLocator serviceLocator)
    {
      return new StubRowRenderer (context, writer, list);
    }
  }
}