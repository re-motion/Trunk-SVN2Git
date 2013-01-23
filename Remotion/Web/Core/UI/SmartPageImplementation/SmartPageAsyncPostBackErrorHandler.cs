// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using Remotion.Utilities;

namespace Remotion.Web.UI.SmartPageImplementation
{
  public class SmartPageAsyncPostBackErrorHandler
  {
    private readonly HttpContextBase _context;
    //PageRequestManager
    internal const string AsyncPostBackErrorKey = "System.Web.UI.PageRequestManager:AsyncPostBackError";
    internal const string AsyncPostBackErrorMessageKey = "System.Web.UI.PageRequestManager:AsyncPostBackErrorMessage";
    internal const string AsyncPostBackErrorHttpCodeKey = "System.Web.UI.PageRequestManager:AsyncPostBackErrorHttpCode";



    public SmartPageAsyncPostBackErrorHandler (HttpContextBase context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      
      _context = context;
    }

    public void HandleError (Exception error)
    {
      ArgumentUtility.CheckNotNull ("error", error);

      string errorHtml = GetErrorHtml (_context, error);

      _context.Items[AsyncPostBackErrorKey] = true;
      _context.Items[AsyncPostBackErrorMessageKey] = errorHtml;
      _context.Items[AsyncPostBackErrorHttpCodeKey] = 500;

      throw new AsyncUnhandledException (error);
    }

    private static string GetErrorHtml (HttpContextBase context, Exception exception)
    {
      string errorHtml;
      if (context.IsCustomErrorEnabled)
      {
        string aspNetErrorPage;
        using (var reader = new StreamReader (
            Assembly.GetExecutingAssembly().GetManifestResourceStream (
                typeof (SmartPageInfo),
                "Generic_Error_Async_Remote.htm")))
        {
          aspNetErrorPage = reader.ReadToEnd();
        }
        errorHtml = ExtractBodyContent (aspNetErrorPage);
        errorHtml = errorHtml.Replace ("{applicationPath}", context.Request.ApplicationPath);
      }
      else
      {
        var aspNetErrorPage = new HttpUnhandledException ("Async Postback Error", exception).GetHtmlErrorMessage();
        errorHtml = ExtractBodyContent (aspNetErrorPage);
      }
      return errorHtml;
    }

    private static string ExtractBodyContent (string aspNetErrorPage)
    {
      var bodyBegin = @"<body bgcolor=""white"">";
      var bodyEnd = @"</body>";
      return aspNetErrorPage.Split (new[] { bodyBegin, bodyEnd }, StringSplitOptions.None).Skip (1).Take (1).Single();
    }
  }
}