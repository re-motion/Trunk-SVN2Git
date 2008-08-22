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
  public abstract class WxeUIStep : WxeStep
  {
    private WxePageStep _pageStep;
    private WxeFunction _function;
    private NameValueCollection _postBackCollection;

    protected void ExecutePage (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      Assertion.IsNotNull (PageStep, "Executing a WxeUIStep without an associated WxePageStep is not supported.");

      try
      {
        string url = PageStep.Page;
        string queryString = context.HttpContext.Request.Url.Query;
        if (! string.IsNullOrEmpty (queryString))
        {
          queryString = queryString.Replace (":", HttpUtility.UrlEncode (":"));
          if (url.Contains ("?"))
            url = url + "&" + queryString.TrimStart ('?');
          else
            url = url + queryString;
        }
        context.HttpContext.Server.Transfer (url, context.IsPostBack);
      }
      catch (HttpException e)
      {
        if (GetUnwrappedExceptionFromHttpException (e) is WxeExecuteNextStepException)
          return;
        throw;
      }
    }

    internal void ExecuteFunction (WxeUserControl2 userControl, WxeFunction function)
    {
      ArgumentUtility.CheckNotNull ("userControl", userControl);
      ArgumentUtility.CheckNotNull ("function", function);

      BackupPostBackCollection (userControl.WxePage);
      
      PrepareExecuteFunction (function, true);

      try
      {
        Execute ();
      }
      catch (WxeExecuteUserControlStepException)
      {
      }
    }

    protected void PrepareExecuteFunction (WxeFunction function, bool isSubFunction)
    {
      ArgumentUtility.CheckNotNull ("function", function);

      if (_function != null)
        throw new InvalidOperationException ("Cannot execute function while another function executes.");

      _function = function;
      if (isSubFunction)
        _function.SetParentStep (this);
    }

    protected void ClearExecutingFunction ()
    {
      _function = null;
    }

    public WxePageStep PageStep
    {
      get
      {
        if (_pageStep == null)
          _pageStep = WxeStep.GetStepByType<WxePageStep> (this);
        return _pageStep;
      }
    }

    protected WxeFunction Function
    {
      get { return _function; }
    }

    protected NameValueCollection PostBackCollection
    {
      get { return _postBackCollection; }
    }

    protected void BackupPostBackCollection (IWxePage page)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      _postBackCollection = new NameValueCollection (page.GetPostBackCollection ());
    }

    protected void RemoveEventSource (Control sender, bool usesEventTarget)
    {
      Assertion.IsNotNull (_postBackCollection, "BackupPostBackCollection must be invoked before calling RemoveEventSource");

      if (usesEventTarget)
      {
        _postBackCollection.Remove (ControlHelper.PostEventSourceID);
        _postBackCollection.Remove (ControlHelper.PostEventArgumentID);
      }
      else
      {
        ArgumentUtility.CheckNotNull ("sender", sender);
        if (!(sender is IPostBackEventHandler || sender is IPostBackDataHandler))
        {
          throw new ArgumentException (
              "The sender must implement either IPostBackEventHandler or IPostBackDataHandler. Provide the control that raised the post back event.");
        }
        _postBackCollection.Remove (sender.UniqueID);
      }
    }

    protected void ProcessCurrentFunction (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      //  Use the Page's postback data
      context.PostBackCollection = null;
      context.SetIsReturningPostBack (false);
    }

    protected void ProcessExecutedFunction (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      //  Provide the executed sub-function to the executing page
      context.ReturningFunction = Function;
      ClearExecutingFunction ();

      context.SetIsPostBack (true);

      // Correct the PostBack-Sequence number
      _postBackCollection[WxePageInfo<WxePage>.PostBackSequenceNumberID] = context.PostBackID.ToString();

      //  Provide the backed up postback data to the executing page
      context.PostBackCollection = _postBackCollection;
      _postBackCollection = null;
      context.SetIsReturningPostBack (true);
    }

    protected void ProcessExecutedExternalFunction (WxeContext context)
    {
      //  Provide the executed sub-function to the executing page
      context.ReturningFunction = Function;
      ClearExecutingFunction ();

      context.SetIsPostBack (true);

      bool isPostRequest = string.Compare (context.HttpContext.Request.HttpMethod, "POST", true) == 0;
      if (isPostRequest)
      {
        // Use original postback data
        context.PostBackCollection = null;
      }
      else
      {
        // Correct the PostBack-Sequence number
        PostBackCollection[WxePageInfo<WxePage>.PostBackSequenceNumberID] = context.PostBackID.ToString ();
        //  Provide the backed up postback data to the executing page
        context.PostBackCollection = PostBackCollection;
        context.SetIsReturningPostBack (true);
      }

      _postBackCollection = null;
    }
  }
}