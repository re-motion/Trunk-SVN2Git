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
using System.Web;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// Encapsulates execute logic for WXE functions.
  /// </summary>
  /// <remarks>
  /// Dispose the <see cref="WxeExecutor"/> at the end of the page life cycle, i.e. in the <see cref="Control.Dispose"/> method.
  /// </remarks>
  public class WxeExecutor : IDisposable, IWxeExecutor
  {
    private readonly HttpContext _httpContext;
    private readonly IWxePage _page;
    private readonly WxePageInfo _wxePageInfo;

    public WxeExecutor (HttpContext context, IWxePage page, WxePageInfo wxePageInfo)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNull ("wxePageInfo", wxePageInfo);

      _wxePageInfo = wxePageInfo;
      _page = page;
      _httpContext = context;
      _httpContext.Handler = page;
    }

    /// <summary>
    /// Invoke <see cref="IDisposable.Dispose"/> at the end of the page life cycle, i.e. in the <see cref="Control.Dispose"/> method.
    /// </summary>
    void IDisposable.Dispose ()
    {
      _httpContext.Handler = _wxePageInfo.WxeHandler;
    }

    public HttpContext HttpContext
    {
      get { return _httpContext; }
    }

    public void ExecuteFunction (WxeFunction function, WxePermaUrlOptions permaUrlOptions)
    {
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNull ("permaUrlOptions", permaUrlOptions);

      try
      {
        _httpContext.Handler = _wxePageInfo.WxeHandler;
        _wxePageInfo.CurrentStep.ExecuteFunction (_page, function, permaUrlOptions);
      }
      finally
      {
        _httpContext.Handler = _page;
      }
    }

    public void ExecuteFunctionNoRepost (WxeFunction function, Control sender, WxeCallOptionsNoRepost options)
    {
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNull ("sender", sender);
      ArgumentUtility.CheckNotNull ("options", options);

      try
      {
        _httpContext.Handler = _wxePageInfo.WxeHandler;
        _wxePageInfo.CurrentStep.ExecuteFunctionNoRepost (
            _page, function, sender, options.UsesEventTarget ?? UsesEventTarget, options.PermaUrlOptions);
      }
      finally
      {
        _httpContext.Handler = _page;
      }
    }

    public void ExecuteFunctionExternalByRedirect (WxeFunction function, WxeCallOptionsExternalByRedirect options)
    {
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNull ("options", options);

      try
      {
        _httpContext.Handler = _wxePageInfo.WxeHandler;
        _wxePageInfo.CurrentStep.ExecuteFunctionExternalByRedirect (
            _page, function, options.PermaUrlOptions, options.ReturnToCaller, options.CallerUrlParameters);
      }
      finally
      {
        _httpContext.Handler = _page;
      }
    }

    public void ExecuteFunctionExternal (WxeFunction function, Control sender, WxeCallOptionsExternal options)
    {
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNull ("sender", sender);
      ArgumentUtility.CheckNotNull ("options", options);

      string functionToken = _wxePageInfo.CurrentStep.GetFunctionTokenForExternalFunction (function, options.ReturningPostback);

      string href = _wxePageInfo.CurrentStep.GetDestinationUrlForExternalFunction (
          function,
          functionToken,
          options.PermaUrlOptions.UsePermaUrl,
          options.PermaUrlOptions.UseParentPermaUrl,
          options.PermaUrlOptions.UrlParameters);

      string openScript = string.Format ("window.open('{0}', '{1}', '{2}');", href, options.Target, StringUtility.NullToEmpty (options.Features));
      ScriptUtility.RegisterStartupScriptBlock ((Page) _page, "WxeExecuteFunction", openScript);

      function.ReturnUrl = "javascript:" + GetClosingScriptForExternalFunction (functionToken, sender, options.ReturningPostback);
    }

    /// <summary> 
    ///   Gets a flag describing whether the post back was most likely caused by the ASP.NET post back mechanism.
    /// </summary>
    /// <value> <see langword="true"/> if the post back collection contains the <b>__EVENTTARGET</b> field. </value>
    private bool UsesEventTarget
    {
      get
      {
        NameValueCollection postBackCollection = _page.GetPostBackCollection();
        if (postBackCollection == null)
        {
          if (_page.IsPostBack)
            throw new InvalidOperationException ("The IWxePage has no PostBackCollection even though this is a post back.");
          return false;
        }
        return !StringUtility.IsNullOrEmpty (postBackCollection[ControlHelper.PostEventSourceID]);
      }
    }

    /// <summary> Gets the client script to be used as the return URL for the window of the external function. </summary>
    private string GetClosingScriptForExternalFunction (string functionToken, Control sender, bool returningPostback)
    {
      if (!returningPostback)
        return "window.close();";
      else if (UsesEventTarget)
      {
        NameValueCollection postBackCollection = _page.GetPostBackCollection();
        if (postBackCollection == null)
          throw new InvalidOperationException ("The IWxePage has no PostBackCollection even though this is a post back.");

        string eventTarget = postBackCollection[ControlHelper.PostEventSourceID];
        string eventArgument = postBackCollection[ControlHelper.PostEventArgumentID];
        return FormatDoPostBackClientScript (functionToken, _page.CurrentStep.PageToken, sender.ClientID, eventTarget, eventArgument);
      }
      else
      {
        ArgumentUtility.CheckNotNull ("sender", sender);
        if (!(sender is IPostBackEventHandler || sender is IPostBackDataHandler))
        {
          throw new ArgumentException (
              "The sender must implement either IPostBackEventHandler or IPostBackDataHandler. Provide the control that raised the post back event.");
        }
        return FormatDoSubmitClientScript (functionToken, _page.CurrentStep.PageToken, sender.ClientID);
      }
    }

    /// <summary> 
    ///   Gets the client script used to execute <c>__dopostback</c> in the parent form before closing the window of the 
    ///   external function.
    /// </summary>
    private string FormatDoPostBackClientScript (string functionToken, string pageToken, string senderID, string eventTarget, string eventArgument)
    {
      return string.Format (
@"
if (   window.opener != null
    && ! window.opener.closed
    && window.opener.wxeDoPostBack != null
    && window.opener.document.getElementById('{0}') != null
    && window.opener.document.getElementById('{0}').value == '{1}')
{{
  window.opener.wxeDoPostBack('{2}', '{3}', '{4}'); 
}}
window.close();
",
          WxePageInfo.PageTokenID,
          pageToken,
          eventTarget,
          eventArgument,
          functionToken);
    }

    /// <summary> 
    ///   Gets the client script used to submit the parent form before closing the window of the external function. 
    /// </summary>
    private string FormatDoSubmitClientScript (string functionToken, string pageToken, string senderID)
    {
      return string.Format (
@"
if (   window.opener != null
    && ! window.opener.closed
    && window.opener.wxeDoSubmit != null
    && window.opener.document.getElementById('{0}') != null
    && window.opener.document.getElementById('{0}').value == '{1}')
{{
  window.opener.wxeDoSubmit('{2}', '{3}');
}}
window.close();
",
          WxePageInfo.PageTokenID,
          pageToken,
          senderID,
          functionToken);
    }
  }
}