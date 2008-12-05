// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Utilities
{
  /// <summary>
  /// Utility class for pages.
  /// </summary>
  public class PageUtility
  {
    /// <summary>
    ///   Gets the form's postback data in a fashion that works for WxePages too. 
    ///   Otherwise simialar to <b>Page.Request.Form</b>.
    /// </summary>
    /// <param name="page"> The page to query for the request collection. Must not be <see langword="null"/>. </param>
    /// <returns> 
    ///   The <see cref="NameValueCollection"/> returned by 
    ///   <see cref="Remotion.Web.UI.ISmartPage.GetPostBackCollection">IWxePage.GetPostBackCollection</see> or the 
    ///   <see cref="HttpRequest.Form"/> collection of the <see cref="Page.Request"/>, depending on whether or not the
    ///   <paramref name="page"/> implements <see cref="IWxePage"/>.
    /// </returns>
    public static NameValueCollection GetPostBackCollection (Page page)
    {
      IWxePage wxePage = page as IWxePage;
      if (wxePage != null)
        return wxePage.GetPostBackCollection();
      else
        return page.Request.Form;
    }

    /// <summary>
    ///   Gets a single item from the form's postback data in a fashion that works for WxePages too. 
    ///   Otherwise simialar to <b>Page.Request.Form</b>.
    /// </summary>
    /// <param name="page"> The page to query for the request collection. Must not be <see langword="null"/>. </param>
    /// <param name="name"> The name of the item to be returned. Must not be <see langword="null"/> or empty. </param>
    /// <returns> 
    ///   The item identified by <paramref name="name"/> or <see langword="null"/> if the item could not be found. 
    /// </returns>
    public static string GetPostBackCollectionItem (Page page, string name)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      NameValueCollection collection = PageUtility.GetPostBackCollection (page);
      if (collection == null)
        return null;
      return collection[name];
    }

    private PageUtility ()
    {
    }

    public static Exception GetUnwrappedExceptionFromHttpException (Exception e)
    {
      Exception unwrappedException = e as HttpException;
      while (unwrappedException is HttpException)
      {
        unwrappedException = unwrappedException.InnerException;
      }
      return unwrappedException;
    }
  }
}
