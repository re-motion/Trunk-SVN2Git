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
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls.DatePickerButtonImplementation.Rendering
{
  public class DatePickerPageRenderer : IDatePickerPageRenderer
  {
    private readonly HttpContextBase _context;
    private readonly DatePickerPage _page;

    public DatePickerPageRenderer (HttpContextBase context, DatePickerPage page)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("page", page);

      _context = context;
      _page = page;
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);
      var page = PageWrapper.CastOrCreate (Page);
      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude (page);

      string key = typeof (DatePickerPageRenderer).FullName + "_Script";
      if (!htmlHeadAppender.IsRegistered (key))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            page,
            Context,
            typeof (DatePickerPageRenderer),
            ResourceType.Html,
            "DatePicker.js");
        htmlHeadAppender.RegisterJavaScriptInclude (key, scriptUrl);
      }

      string styleUrl = ResourceUrlResolver.GetResourceUrl (
          page,
          Context,
          typeof (DatePickerPageRenderer),
          ResourceType.Html,
          ResourceTheme,
          "Style.css");
      htmlHeadAppender.RegisterStylesheetLink (typeof (DatePickerPageRenderer).FullName + "_Style", styleUrl);
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
      get { return SafeServiceLocator.Current.GetInstance<ResourceTheme> (); }
    }
  }
}