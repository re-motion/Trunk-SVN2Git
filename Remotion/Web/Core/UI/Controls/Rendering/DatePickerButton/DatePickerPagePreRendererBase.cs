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
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls.Rendering.DatePickerButton
{
  public abstract class DatePickerPagePreRendererBase : IDatePickerPagePreRenderer
  {
    private readonly IHttpContext _context;
    private readonly DatePickerPage _page;

    protected DatePickerPagePreRendererBase (IHttpContext context, DatePickerPage page)
    {
      _context = context;
      _page = page;
    }

    public IHttpContext Context
    {
      get { return _context; }
    }

    public DatePickerPage Page
    {
      get { return _page; }
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      var page = PageWrapper.CastOrCreate (Page);
      HtmlHeadAppender.Current.RegisterJQueryJavaScriptInclude (page);

      string key = typeof (DatePickerPage).FullName + "_Script";
      if (!HtmlHeadAppender.Current.IsRegistered (key))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            page,
            Context,
            typeof (DatePickerPage),
            ResourceType.Html,
            ScriptFileName);
        HtmlHeadAppender.Current.RegisterJavaScriptInclude (key, scriptUrl);
      }
    }

    protected abstract string ScriptFileName { get; }
  }
}