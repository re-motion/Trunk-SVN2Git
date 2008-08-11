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
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// This class contains extension methods for the <see cref="IWxePage"/> interface.
  /// </summary>
  public static class WxePageExtensions
  {
    /// <summary> Executes the <paramref name="function"/> in the current window. </summary>
    /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunction/param[@name="function"]' />
    public static void ExecuteFunction (this IWxePage page, WxeFunction function)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      page.ExecuteFunction (function, false, false, null);
    }

    /// <summary> Executes the <paramref name="function"/> in the current window. </summary>
    /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunction/param[@name="function" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
    public static void ExecuteFunction (this IWxePage page, WxeFunction function, bool createPermaUrl, bool useParentPermaUrl)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      page.ExecuteFunction (function, createPermaUrl, useParentPermaUrl, null);
    }

    /// <summary>
    ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event on returning.
    /// </summary>
    /// <remarks>
    ///   This overload tries to determine automatically whether the current event was caused by the <c>__EVENTTARGET</c> field.
    /// </remarks>
    /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender"]' />
    public static void ExecuteFunctionNoRepost (this IWxePage page, WxeFunction function, Control sender)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      page.ExecuteFunctionNoRepost (function, sender, GetUsesEventTarget (page), false, false, null);
    }

    /// <summary>
    ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event on returning.
    /// </summary>
    /// <remarks>
    ///   This overload allows you to specify whether the current event was caused by the <c>__EVENTTARGET</c> field.
    ///   When in doubt, use <see cref="M:Remotion.Web.ExecutionEngine.WxePageExtensions.ExecuteFunctionNoRepost(Remotion.Web.ExecutionEngine.IWxePage,Remotion.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control)">WxePageExtensions.ExecuteFunctionNoRepost(IWxePage,WxeFunction,Control)</see>.
    /// </remarks>
    /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget"]' />
    public static void ExecuteFunctionNoRepost (this IWxePage page, WxeFunction function, Control sender, bool usesEventTarget)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      page.ExecuteFunctionNoRepost (function, sender, usesEventTarget, false, false, null);
    }

    /// <summary>
    ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event on returning.
    /// </summary>
    /// <remarks>
    ///   This overload tries to determine automatically whether the current event was caused by the <c>__EVENTTARGET</c> field.
    /// </remarks>
    /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
    public static void ExecuteFunctionNoRepost (this IWxePage page, WxeFunction function, Control sender, bool createPermaUrl, bool useParentPermaUrl)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      page.ExecuteFunctionNoRepost (function, sender, GetUsesEventTarget (page), createPermaUrl, useParentPermaUrl, null);
    }

    /// <summary>
    ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event on returning.
    /// </summary>
    /// <remarks>
    ///   This overload tries to determine automatically whether the current event was caused by the <c>__EVENTTARGET</c> field.
    /// </remarks>
    /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
    public static void ExecuteFunctionNoRepost (this IWxePage page, WxeFunction function, Control sender, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      page.ExecuteFunctionNoRepost (function, sender, GetUsesEventTarget (page), createPermaUrl, useParentPermaUrl, permaUrlParameters);
    }

    /// <summary>
    ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event on returning.
    /// </summary>
    /// <remarks>
    ///   This overload allows you to specify whether the current event was caused by the <c>__EVENTTARGET</c> field.
    ///   When in doubt, use <see cref="M:Remotion.Web.ExecutionEngine.WxePageExtensions.ExecuteFunctionNoRepost(Remotion.Web.ExecutionEngine.IWxePage,Remotion.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control,System.Boolean,System.Boolean)">WxePageExtensions.ExecuteFunctionNoRepost(IWxePage,WxeFunction,Control,Boolean,Boolean)</see>.
    /// </remarks>
    /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
    public static void ExecuteFunctionNoRepost (this IWxePage page, WxeFunction function, Control sender, bool usesEventTarget, bool createPermaUrl, bool useParentPermaUrl)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      page.ExecuteFunctionNoRepost (function, sender, usesEventTarget, createPermaUrl, useParentPermaUrl, null);
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the 
    ///   new function.
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters"]' />
    public static void ExecuteFunctionExternal (this IWxePage page, WxeFunction function, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      page.ExecuteFunctionExternal (function, createPermaUrl, useParentPermaUrl, urlParameters, true, null);
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   specified window or frame by through a javascript call.
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback"]' />
    public static void ExecuteFunctionExternal (this IWxePage page, WxeFunction function, string target, Control sender, bool returningPostback)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      page.ExecuteFunctionExternal (function, target, null, sender, returningPostback, false, false, null);
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   specified window or frame through javascript window.open(...).
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback"]' />
    public static void ExecuteFunctionExternal (
        this IWxePage page, WxeFunction function, string target, string features, Control sender, bool returningPostback)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      page.ExecuteFunctionExternal (function, target, features, sender, returningPostback, false, false, null);
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   specified window or frame through javascript window.open(...).
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
    public static void ExecuteFunctionExternal (
        this IWxePage page, WxeFunction function, string target, Control sender, bool returningPostback, bool createPermaUrl, bool useParentPermaUrl)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      page.ExecuteFunctionExternal (function, target, null, sender, returningPostback, createPermaUrl, useParentPermaUrl, null);
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   specified window or frame through javascript window.open(...).
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters"]' />
    public static void ExecuteFunctionExternal (
        this IWxePage page,
        WxeFunction function,
        string target,
        Control sender,
        bool returningPostback,
        bool createPermaUrl,
        bool useParentPermaUrl,
        NameValueCollection urlParameters)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      page.ExecuteFunctionExternal (function, target, null, sender, returningPostback, createPermaUrl, useParentPermaUrl, urlParameters);
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   specified window or frame through javascript window.open(...).
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
    public static void ExecuteFunctionExternal (
        this IWxePage page,
        WxeFunction function,
        string target,
        string features,
        Control sender,
        bool returningPostback,
        bool createPermaUrl,
        bool useParentPermaUrl)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      page.ExecuteFunctionExternal (function, target, features, sender, returningPostback, createPermaUrl, useParentPermaUrl, null);
    }

    /// <summary> 
    ///   Gets a flag describing whether the post back was most likely caused by the ASP.NET post back mechanism.
    /// </summary>
    /// <value> <see langword="true"/> if the post back collection contains the <b>__EVENTTARGET</b> field. </value>
    private static bool GetUsesEventTarget (IWxePage page)
    {
      NameValueCollection postBackCollection = page.GetPostBackCollection ();
      if (postBackCollection == null)
      {
        if (page.IsPostBack)
          throw new InvalidOperationException ("The IWxePage has no PostBackCollection even though this is a post back.");
        return false;
      }
      return !StringUtility.IsNullOrEmpty (postBackCollection[ControlHelper.PostEventSourceID]);
    }
  }
}