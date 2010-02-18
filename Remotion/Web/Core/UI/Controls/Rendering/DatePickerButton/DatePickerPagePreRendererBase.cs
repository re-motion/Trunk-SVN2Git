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
using Microsoft.Practices.ServiceLocation;
using System.Web;

namespace Remotion.Web.UI.Controls.Rendering.DatePickerButton
{
  public abstract class DatePickerPagePreRendererBase : IDatePickerPagePreRenderer
  {
    private readonly HttpContextBase _context;
    private readonly DatePickerPage _page;

    protected DatePickerPagePreRendererBase (HttpContextBase context, DatePickerPage page)
    {
      _context = context;
      _page = page;
    }

    public HttpContextBase Context
    {
      get { return _context; }
    }

    public DatePickerPage Page
    {
      get { return _page; }
    }

    protected ResourceTheme ResourceTheme
    {
      get { return ServiceLocator.Current.GetInstance<ResourceTheme>(); }
    }

    public abstract void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender);
  }
}
