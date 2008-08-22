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
using System.IO;
using System.Reflection;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary> This step interrupts the server side execution to display a page to the user. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Class/*' />
  [Serializable]
  public class WxePageStep : WxeUIStep
  {
    private string _page = null;
    private string _pageref = null;
    private string _pageRoot;
    private string _pageToken;
    private string _state;
    private bool _isRedirectToPermanentUrlRequired = false;
    private bool _useParentPermaUrl = false;
    private NameValueCollection _permaUrlParameters;
    private bool _createPermaUrl = false;
    private bool _returnToCaller = false;
    private NameValueCollection _callerUrlParameters = null;
    private bool _isRedirectedToPermanentUrl = false;
    private bool _hasReturnedFromRedirectToPermanentUrl = false;
    private string _resumeUrl;
    private bool _isExecuteFunctionExternalRequired = false;
    private bool _isExternalFunctionInvoked = false;

    /// <summary> Initializes a new instance of the <b>WxePageStep</b> type. </summary>
    /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Ctor/param[@name="page"]' />
    public WxePageStep (string page)
        : this (null, page)
    {
    }

    /// <summary> Initializes a new instance of the <b>WxePageStep</b> type. </summary>
    /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Ctor/param[@name="page" or @name="resourceAssembly"]' />
    protected WxePageStep (Assembly resourceAssembly, string page)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("page", page);

      _page = page;
      Initialize (resourceAssembly);
    }

    /// <summary> Initializes a new instance of the <b>WxePageStep</b> type. </summary>
    /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Ctor/param[@name="pageref"]' />
    public WxePageStep (WxeVariableReference pageref)
        : this (null, pageref)
    {
    }

    /// <summary> Initializes a new instance of the <b>WxePageStep</b> type. </summary>
    /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Ctor/param[@name="pageref" or @name="resourceAssembly"]' />
    protected WxePageStep (Assembly resourceAssembly, WxeVariableReference pageref)
    {
      ArgumentUtility.CheckNotNull ("pageref", pageref);

      _pageref = pageref.Name;
      Initialize (resourceAssembly);
    }

    /// <summary> Common initalization code for the <see cref="WxePageStep"/>. </summary>
    /// <param name="resourceAssembly"> The (optional) <see cref="Assembly"/> containing the page. </param>
    private void Initialize (Assembly resourceAssembly)
    {
      _pageToken = Guid.NewGuid().ToString();

      if (resourceAssembly == null)
        _pageRoot = string.Empty;
      else
        _pageRoot = ResourceUrlResolver.GetAssemblyRoot (false, resourceAssembly);
    }

    /// <summary> The URL of the page to be displayed by this <see cref="WxePageStep"/>. </summary>
    public string Page
    {
      get
      {
        string name;
        if (_page != null)
          name = _page;
        else if (_pageref != null && Variables[_pageref] != null)
          name = (string) Variables[_pageref];
        else
          throw new WxeException ("No Page specified for " + this.GetType().FullName + ".");
        return _pageRoot + name;
      }
    }

    /// <summary> 
    ///   Displays the <see cref="WxePageStep"/>'s page or the sub-function that has been invoked by the 
    ///   <see cref="ExecuteFunction"/> method.
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Execute/*' />
    public override void Execute (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      if (Function == null)
      {
        //  This is the PageStep if it isn't executing a sub-function

        ProcessCurrentFunction (context);
      }
      else
      {
        if (! _isExecuteFunctionExternalRequired)
        {
          //  This is the PageStep currently executing a sub-function

          EnsureHasRedirectedToPermanentUrl (context);
          Function.Execute (context);
          //  This point is only reached after the sub-function has completed execution.

          //  This is the PageStep after the sub-function has completed execution

          EnsureHasReturnedFromRedirectToPermanentUrl (context);

          ProcessExecutedFunction (context);
          CleanupAfterHavingReturnedFromRedirectToPermanentUrl();
        }
        else
        {
          //  This is the PageStep currently executing an external function
          EnsureExternalFunctionInvoked (context);
          //  This point is only reached after the external function has been started.

          //  This is the PageStep after the external function has completed execution 
          //  or a postback to the executing page has been received
          ProcessExecutedExternalFunction (context);
          CleanupAfterHavingInvokedExternalFunction();
        }
      }

      ExecutePage (context);
    }

    private void EnsureHasRedirectedToPermanentUrl (WxeContext context)
    {
      if (_isRedirectToPermanentUrlRequired && ! _isRedirectedToPermanentUrl)
      {
        string destinationUrl;
        try
        {
          destinationUrl = GetDestinationPermanentUrl (context);
        }
        catch (WxePermanentUrlTooLongException)
        {
          ClearExecutingFunction ();
          throw;
        }

        _permaUrlParameters = null;
        _resumeUrl = context.GetResumePath();
        _isRedirectedToPermanentUrl = true;
        PageUtility.Redirect (context.HttpContext.Response, destinationUrl);
      }
    }

    private void EnsureHasReturnedFromRedirectToPermanentUrl (WxeContext context)
    {
      if (_isRedirectToPermanentUrlRequired && _isRedirectedToPermanentUrl && ! _hasReturnedFromRedirectToPermanentUrl)
      {
        _hasReturnedFromRedirectToPermanentUrl = true;
        PageUtility.Redirect (context.HttpContext.Response, _resumeUrl);
      }
    }

    private void CleanupAfterHavingReturnedFromRedirectToPermanentUrl ()
    {
      if (_isRedirectToPermanentUrlRequired && _hasReturnedFromRedirectToPermanentUrl)
      {
        _isRedirectToPermanentUrlRequired = false;
        _isRedirectedToPermanentUrl = false;
        _hasReturnedFromRedirectToPermanentUrl = false;
      }
    }

    private string GetDestinationPermanentUrl (WxeContext context)
    {
      NameValueCollection internalUrlParameters;
      if (_permaUrlParameters == null)
        internalUrlParameters = Function.SerializeParametersForQueryString();
      else
        internalUrlParameters = NameValueCollectionUtility.Clone (_permaUrlParameters);

      internalUrlParameters.Set (WxeHandler.Parameters.WxeFunctionToken, context.FunctionToken);

      return context.GetPermanentUrl (Function.GetType(), internalUrlParameters, _useParentPermaUrl);
    }

    private void EnsureExternalFunctionInvoked (WxeContext context)
    {
      if (_isExecuteFunctionExternalRequired && ! _isExternalFunctionInvoked)
      {
        string functionToken = GetFunctionTokenForExternalFunction (Function, _returnToCaller);

        string destinationUrl;
        try
        {
          destinationUrl = GetDestinationUrlForExternalFunction (Function, functionToken, _createPermaUrl, _useParentPermaUrl, _permaUrlParameters);
        }
        catch (WxePermanentUrlTooLongException)
        {
          ClearExecutingFunction ();
          throw;
        }

        if (_returnToCaller)
        {
          _callerUrlParameters.Set (WxeHandler.Parameters.WxeFunctionToken, context.FunctionToken);
          Function.ReturnUrl = context.GetPermanentUrl (ParentFunction.GetType(), _callerUrlParameters);
        }

        _permaUrlParameters = null;
        _callerUrlParameters = null;
        _isExternalFunctionInvoked = true;
        PageUtility.Redirect (context.HttpContext.Response, destinationUrl);
      }
    }

    private void CleanupAfterHavingInvokedExternalFunction ()
    {
      if (_isExecuteFunctionExternalRequired && _isExternalFunctionInvoked)
      {
        _isExecuteFunctionExternalRequired = false;
        _isExternalFunctionInvoked = false;
      }
    }

    /// <summary> 
    ///   Initalizes a new <see cref="WxeFunctionState"/> with the passed <paramref name="function"/> and returns
    ///   the associated function token.
    /// </summary>
    internal string GetFunctionTokenForExternalFunction (WxeFunction function, bool returnFromExecute)
    {
      bool enableCleanUp = ! returnFromExecute;
      WxeFunctionState functionState = new WxeFunctionState (function, enableCleanUp);
      WxeFunctionStateManager functionStates = WxeFunctionStateManager.Current;
      functionStates.Add (functionState);
      return functionState.FunctionToken;
    }


    /// <summary> Gets the URL to be used for transfering to the external function. </summary>
    internal string GetDestinationUrlForExternalFunction (
        WxeFunction function,
        string functionToken,
        bool createPermaUrl,
        bool useParentPermaUrl,
        NameValueCollection urlParameters)
    {
      WxeContext wxeContext = WxeContext.Current;

      string href;
      if (createPermaUrl)
      {
        NameValueCollection internalUrlParameters;
        if (urlParameters == null)
          internalUrlParameters = function.SerializeParametersForQueryString();
        else
          internalUrlParameters = NameValueCollectionUtility.Clone (urlParameters);
        internalUrlParameters.Set (WxeHandler.Parameters.WxeFunctionToken, functionToken);

        href = wxeContext.GetPermanentUrl (function.GetType(), internalUrlParameters, useParentPermaUrl);
      }
      else
      {
        UrlMappingEntry mappingEntry = UrlMappingConfiguration.Current.Mappings[function.GetType()];
        string path = (mappingEntry != null) ? mappingEntry.Resource : wxeContext.HttpContext.Request.Url.AbsolutePath;
        href = wxeContext.GetPath (path, functionToken, urlParameters);
      }

      return href;
    }

    /// <summary> Gets the currently executing <see cref="WxeStep"/>. </summary>
    /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/ExecutingStep/*' />
    public override WxeStep ExecutingStep
    {
      get
      {
        if (Function != null)
          return Function.ExecutingStep;
        else
          return this;
      }
    }

    /// <summary> Executes the specified <see cref="WxeFunction"/>, then returns to this page. </summary>
    /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/ExecuteFunction/*' />
    internal void ExecuteFunction<TWxePage> (TWxePage page, WxeFunction function, WxePermaUrlOptions permaUrlOptions)
        where TWxePage: Page, IWxePage
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNull ("permaUrlOptions", permaUrlOptions);

      BackupPostBackCollection (page);

      InternalExecuteFunction (page, function, permaUrlOptions);
    }

    /// <summary>
    ///   Executes the specified <see cref="WxeFunction"/>, then returns to this page without raising the 
    ///   postback event after the user returns.
    /// </summary>
    /// <remarks> Invoke this method by calling <see cref="WxeExecutor{TWxePage}.ExecuteFunctionNoRepost"/>. </remarks>
    internal void ExecuteFunctionNoRepost<TWxePage> (
        TWxePage page, WxeFunction function, Control sender, bool usesEventTarget, WxePermaUrlOptions permaUrlOptions)
        where TWxePage: Page, IWxePage
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNull ("permaUrlOptions", permaUrlOptions);

      BackupPostBackCollection (page);
      RemoveEventSource (sender, usesEventTarget);

      InternalExecuteFunction (page, function, permaUrlOptions);
    }

    /// <summary> Executes the specified <see cref="WxeFunction"/>, then returns to this page. </summary>
    /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/InternalExecuteFunction/*' />
    private void InternalExecuteFunction<TWxePage> (TWxePage page, WxeFunction function, WxePermaUrlOptions permaUrlOptions)
        where TWxePage: Page, IWxePage
    {
      PrepareExecuteFunction (function, true);
      _isRedirectToPermanentUrlRequired = permaUrlOptions.UsePermaUrl;
      _useParentPermaUrl = permaUrlOptions.UseParentPermaUrl;
      _permaUrlParameters = permaUrlOptions.UrlParameters;

      ControlHelper.SaveAllState (page);

      Execute();
    }

    internal void ExecuteFunctionExternalByRedirect<TWxePage> (
        TWxePage page, WxeFunction function, WxePermaUrlOptions permaUrlOptions, bool returnToCaller, NameValueCollection callerUrlParameters)
        where TWxePage: Page, IWxePage
    {
      BackupPostBackCollection (page);

      InternalExecuteFunctionExternalByRedirect (page, function, permaUrlOptions, returnToCaller, callerUrlParameters);
    }

    private void InternalExecuteFunctionExternalByRedirect<TWxePage> (
        TWxePage page, WxeFunction function, WxePermaUrlOptions permaUrlOptions, bool returnToCaller, NameValueCollection callerUrlParameters)
        where TWxePage : Page, IWxePage
    {
      _isExecuteFunctionExternalRequired = true;
      _isExternalFunctionInvoked = false;
      PrepareExecuteFunction (function, false);
      _createPermaUrl = permaUrlOptions.UsePermaUrl;
      _useParentPermaUrl = permaUrlOptions.UseParentPermaUrl;
      _permaUrlParameters = permaUrlOptions.UrlParameters;
      _returnToCaller = returnToCaller;
      _callerUrlParameters = null;
      if (_returnToCaller)
      {
        if (callerUrlParameters == null)
          _callerUrlParameters = page.GetPermanentUrlParameters();
        else
          _callerUrlParameters = NameValueCollectionUtility.Clone (callerUrlParameters);
      }

      ControlHelper.SaveAllState (page);

      Execute();
    }

    /// <summary> Gets the token for this page step. </summary>
    /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/PageToken/*' />
    public string PageToken
    {
      get { return _pageToken; }
    }

    public override string ToString ()
    {
      return "WxePageStep: " + Page;
    }

    /// <summary> Saves the passed <paramref name="state"/> object into the <see cref="WxePageStep"/>. </summary>
    /// <param name="state"> An <b>ASP.NET</b> viewstate object. </param>
    public void SavePageStateToPersistenceMedium (object state)
    {
      LosFormatter formatter = new LosFormatter();
      StringWriter writer = new StringWriter();
      formatter.Serialize (writer, state);
      _state = writer.ToString();
    }

    /// <summary> 
    ///   Returns the viewstate previsously saved through the <see cref="SavePageStateToPersistenceMedium"/> method. 
    /// </summary>
    /// <returns> An <b>ASP.NET</b> viewstate object. </returns>
    public object LoadPageStateFromPersistenceMedium ()
    {
      LosFormatter formatter = new LosFormatter();
      return formatter.Deserialize (_state);
    }

    /// <summary> 
    ///   Aborts the <see cref="WxePageStep"/>. Aborting will cascade to any <see cref="WxeFunction"/> executed
    ///   in the scope of this step if they are part of the same hierarchy, i.e. share a common <see cref="WxeStep.RootFunction"/>.
    /// </summary>
    protected override void AbortRecursive ()
    {
      base.AbortRecursive();
      if (Function != null && Function.RootFunction == this.RootFunction)
        Function.Abort();
    }
  }
}