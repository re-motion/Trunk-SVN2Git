/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
  }
}
