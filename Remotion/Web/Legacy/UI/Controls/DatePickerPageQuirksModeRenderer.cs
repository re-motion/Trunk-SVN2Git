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
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation.Rendering;

namespace Remotion.Web.Legacy.UI.Controls
{
  public class DatePickerPageQuirksModeRenderer : IDatePickerPageRenderer
  {
    public DatePickerPageQuirksModeRenderer ()
    {
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, DatePickerPage page, HttpContextBase context)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);
      var newPage = PageWrapper.CastOrCreate (page);
      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude ();

      string key = typeof (DatePickerPageQuirksModeRenderer).FullName + "_Script";
      if (!htmlHeadAppender.IsRegistered (key))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            newPage,
            context,
            typeof (DatePickerPageQuirksModeRenderer),
            ResourceType.Html,
            "DatePicker.js");
        htmlHeadAppender.RegisterJavaScriptInclude (key, scriptUrl);
      }
    }
    
  }
}