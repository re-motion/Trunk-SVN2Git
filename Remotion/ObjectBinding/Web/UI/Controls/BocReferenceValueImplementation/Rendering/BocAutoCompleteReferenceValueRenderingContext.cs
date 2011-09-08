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
using System.Web;
using System.Web.UI;
using Remotion.ObjectBinding.Web.Services;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering
{
  /// <summary>
  /// Groups all arguments required for rendering a <see cref="BocAutoCompleteReferenceValue"/>.
  /// </summary>
  public class BocAutoCompleteReferenceValueRenderingContext : BocRenderingContext<IBocAutoCompleteReferenceValue>
  {
    private readonly SearchAvailableObjectWebServiceContext _searchAvailableObjectWebServiceContext;

    public BocAutoCompleteReferenceValueRenderingContext (
        HttpContextBase httpContext,
        HtmlTextWriter writer,
        IBocAutoCompleteReferenceValue control,
        SearchAvailableObjectWebServiceContext searchAvailableObjectWebServiceContext)
        : base (httpContext, writer, control)
    {
      _searchAvailableObjectWebServiceContext = searchAvailableObjectWebServiceContext;
    }

    public SearchAvailableObjectWebServiceContext SearchAvailableObjectWebServiceContext
    {
      get { return _searchAvailableObjectWebServiceContext; }
    }
  }
}