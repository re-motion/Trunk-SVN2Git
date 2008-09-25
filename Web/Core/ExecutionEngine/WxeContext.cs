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
using Remotion.Context;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{

  /// <summary>
  ///   The <b>WxeContext</b> contains information about the current WXE execution cycle.
  /// </summary>
  public class WxeContext
  {
    /// <summary> The current <see cref="WxeContext"/>. </summary>
    /// <value> 
    ///   An instance of the <see cref="WxeContext"/> type 
    ///   or <see langword="null"/> if no <see cref="WxeFunction"/> is executing.
    /// </value>
    public static WxeContext Current
    {
      get { return SafeContext.Instance.GetData ("WxeContext") as WxeContext; }
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public static void SetCurrent (WxeContext value)
    {
      SafeContext.Instance.SetData ("WxeContext", value);
    }

    /// <summary> 
    ///   Gets the permanent URL for the <see cref="WxeFunction"/> of the specified <paramref name="functionType"/> 
    ///   and using the <paramref name="urlParameters"/>.
    /// </summary>
    /// <remarks> Call this method only from pages not implementing <see cref="IWxePage"/>. </remarks>
    /// <exception cref="WxeException">
    ///   Thrown if no mapping for the <paramref name="functionType"/> has been defined, and the 
    ///   <see cref="Remotion.Web.Configuration.ExecutionEngineConfiguration.DefaultWxeHandler"/> is not set. 
    /// </exception>
    /// <include file='doc\include\ExecutionEngine\WxeContext.xml' path='WxeContext/GetPermanentUrl/param[@name="httpContext" or @name="functionType" or @name="urlParameters"]' />
    public static string GetPermanentUrl (IHttpContext httpContext, Type functionType, NameValueCollection urlParameters)
    {
      return GetPermanentUrl (httpContext, functionType, urlParameters, false);
    }

    /// <summary> 
    ///   Gets the permanent URL for the <see cref="WxeFunction"/> of the specified <paramref name="functionType"/> 
    ///   and using the <paramref name="urlParameters"/>.
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\WxeContext.xml' path='WxeContext/GetPermanentUrl/param[@name="httpContext" or @name="functionType" or @name="urlParameters" or @name="fallbackOnCurrentUrl"]' />
    protected static string GetPermanentUrl (IHttpContext httpContext, Type functionType, NameValueCollection urlParameters, bool fallbackOnCurrentUrl)
    {
      ArgumentUtility.CheckNotNull ("httpContext", httpContext);
      ArgumentUtility.CheckNotNull ("functionType", functionType);
      if (!typeof (WxeFunction).IsAssignableFrom (functionType))
        throw new ArgumentException (string.Format ("The functionType '{0}' must be derived from WxeFunction.", functionType), "functionType");
      ArgumentUtility.CheckNotNull ("urlParameters", urlParameters);

      NameValueCollection internalUrlParameters = NameValueCollectionUtility.Clone (urlParameters);
      UrlMapping.UrlMappingEntry mappingEntry = UrlMapping.UrlMappingConfiguration.Current.Mappings[functionType];
      if (mappingEntry == null)
      {
        string functionTypeName = WebTypeUtility.GetQualifiedName (functionType);
        internalUrlParameters.Set (WxeHandler.Parameters.WxeFunctionType, functionTypeName);
      }

      string path;
      if (mappingEntry == null)
      {
        string defaultWxeHandler = Configuration.WebConfiguration.Current.ExecutionEngine.DefaultWxeHandler;
        if (StringUtility.IsNullOrEmpty (defaultWxeHandler))
        {
          if (fallbackOnCurrentUrl)
            path = httpContext.Request.Url.AbsolutePath;
          else
            throw new WxeException (
                string.Format (
                    "No URL mapping has been defined for WXE Function '{0}', nor has a default WxeHandler URL been specified in the application configuration (web.config).",
                    functionType.FullName));
        }
        else
        {
          path = httpContext.Response.ApplyAppPathModifier (defaultWxeHandler);
        }
      }
      else
      {
        path = httpContext.Response.ApplyAppPathModifier (mappingEntry.Resource);
      }

      string permanentUrl = UrlUtility.GetAbsoluteUrl (httpContext, path)
                            + UrlUtility.FormatQueryString (internalUrlParameters, httpContext.Response.ContentEncoding);

      int maxLength = Configuration.WebConfiguration.Current.ExecutionEngine.MaximumUrlLength;
      if (permanentUrl.Length > maxLength)
      {
        throw new WxePermanentUrlTooLongException (
            string.Format (
                "Error while creating the permanent URL for WXE function '{0}'. "
                + "The URL exceeds the maximum length of {1} bytes. Generated URL: {2}",
                functionType.Name,
                maxLength,
                permanentUrl));
      }

      return permanentUrl;
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> in the current window from any <see cref="Page"/> by using a redirect.
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\WxeContext.xml' path='WxeContext/ExecuteFunctionExternal/param[@name="page" or @name="function" or @name="urlParameters" or @name="returnToCaller"]' />
    public static void ExecuteFunctionExternal (
        Page page, WxeFunction function, NameValueCollection urlParameters, bool returnToCaller)
    {
      ExecuteFunctionExternal (page, function, false, urlParameters, returnToCaller);
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> in the current window from any <see cref="Page"/> by using a redirect.
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\WxeContext.xml' path='WxeContext/ExecuteFunctionExternal/param[@name="page" or @name="function" or @name="createPermaUrl" or @name="urlParameters" or @name="returnToCaller"]' />
    public static void ExecuteFunctionExternal (
        Page page, WxeFunction function, bool createPermaUrl, NameValueCollection urlParameters, bool returnToCaller)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNull ("function", function);

      string href = GetExternalFunctionUrl (function, createPermaUrl, urlParameters);
      if (returnToCaller)
        function.ReturnUrl = page.Request.RawUrl;
      System.Web.HttpContext.Current.Response.Redirect (href);
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> in the specified window or frame from any <see cref="Page"/> 
    ///   by using java script.
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\WxeContext.xml' path='WxeContext/ExecuteFunctionExternal/param[@name="page" or @name="function" or @name="target" or @name="features" or @name="urlParameters"]' />
    public static void ExecuteFunctionExternal (
        Page page, WxeFunction function, string target, string features, NameValueCollection urlParameters)
    {
      ExecuteFunctionExternal (page, function, target, features, false, urlParameters);
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> in the specified window or frame from any <see cref="Page"/>  
    ///   by using java script.
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\WxeContext.xml' path='WxeContext/ExecuteFunctionExternal/param[@name="page" or @name="function" or @name="target" or @name="features" or @name="createPermaUrl" or @name="urlParameters"]' />
    public static void ExecuteFunctionExternal (
        Page page, WxeFunction function, string target, string features,
        bool createPermaUrl, NameValueCollection urlParameters)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNullOrEmpty ("target", target);

      string href = GetExternalFunctionUrl (function, createPermaUrl, urlParameters);

      string openScript;
      if (features != null)
        openScript = string.Format ("window.open('{0}', '{1}', '{2}');", href, target, features);
      else
        openScript = string.Format ("window.open('{0}', '{1}');", href, target);
      ScriptUtility.RegisterStartupScriptBlock (page, "WxeExecuteFunction", openScript);

      function.ReturnUrl = "javascript:window.close();";
    }

    private static string GetExternalFunctionUrl (WxeFunction function, bool createPermaUrl, NameValueCollection urlParameters)
    {
      string functionToken = WxeContext.Current.GetFunctionTokenForExternalFunction (function, false);

      NameValueCollection internalUrlParameters;
      if (urlParameters == null)
      {
        if (createPermaUrl)
          internalUrlParameters = function.VariablesContainer.SerializeParametersForQueryString ();
        else
          internalUrlParameters = new NameValueCollection ();
      }
      else
      {
        internalUrlParameters = NameValueCollectionUtility.Clone (urlParameters);
      }
      internalUrlParameters.Set (WxeHandler.Parameters.WxeFunctionToken, functionToken);
      return WxeContext.GetPermanentUrl (new HttpContextWrapper (System.Web.HttpContext.Current), function.GetType (), internalUrlParameters);
    }


    private readonly IHttpContext _httpContext;
    private readonly WxeFunctionStateManager _functionStateManager;
    private bool _isPostBack;
    private bool _isReturningPostBack;
    private bool _isOutOfSequencePostBack;
    private NameValueCollection _postBackCollection;
    private WxeFunction _returningFunction;
    private readonly WxeFunctionState _functionState;
    private readonly NameValueCollection _queryString;

    public WxeContext (IHttpContext context, WxeFunctionStateManager functionStateManager, WxeFunctionState functionState, NameValueCollection queryString)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("functionStateManager", functionStateManager);
      ArgumentUtility.CheckNotNull ("functionState", functionState);

      _httpContext = context;
      _functionStateManager = functionStateManager;

      _functionState = functionState;

      if (queryString == null)
      {
        _queryString = new NameValueCollection ();
      }
      else
      {
        _queryString = NameValueCollectionUtility.Clone (queryString);
        _queryString.Remove (WxeHandler.Parameters.WxeFunctionToken);
      }
    }

    public IHttpContext HttpContext
    {
      get { return _httpContext; }
    }

    public WxeFunctionStateManager FunctionStateManager
    {
      get { return _functionStateManager; }
    }

    /// <summary>
    ///   Gets a flag that corresponds to the <see cref="System.Web.UI.Page.IsPostBack">Page.IsPostBack</see> flag, but is 
    ///   available from the beginning of the execution cycle, i.e. even before <b>OnInit</b>.
    /// </summary>
    public bool IsPostBack
    {
      get { return _isPostBack; }
    }

    [EditorBrowsable (EditorBrowsableState.Advanced)]
    public void SetIsPostBack (bool value)
    {
      _isPostBack = value;
    }

    /// <summary>
    ///   During the execution of a page, specifies whether the current postback cycle was caused by returning from a 
    ///   <see cref="WxeFunction"/>.
    /// </summary>
    public bool IsReturningPostBack
    {
      get { return _isReturningPostBack; }
    }

    [EditorBrowsable (EditorBrowsableState.Advanced)]
    public void SetIsReturningPostBack (bool value)
    {
      _isReturningPostBack = value;
    }

    /// <summary>
    ///   Gets a flag that describes whether the current postback cycle was caused by resubmitting a page from the 
    ///   client's cache.
    /// </summary>
    public bool IsOutOfSequencePostBack
    {
      get { return _isOutOfSequencePostBack; }
    }

    [EditorBrowsable (EditorBrowsableState.Advanced)]
    public void SetIsOutOfSequencePostBack (bool value)
    {
      _isOutOfSequencePostBack = value;
    }

    /// <summary> Gets or sets the postback data for the page if it has executed a sub-function. </summary>
    /// <value> The postback data generated during the roundtrip that led to the execution of the sub-function. </value>
    /// <remarks> 
    ///   <para>
    ///     This property is used only for transfering the postback data from the backup location to the page's
    ///     initialization infrastructure.
    ///   </para><para>
    ///     Application developers should only use the 
    ///     <see cref="ISmartPage.GetPostBackCollection">ISmartPage.GetPostBackCollection</see> method to access the
    ///     postback data.
    ///   </para><para>
    ///     Control developers should either implement <see cref="System.Web.UI.IPostBackDataHandler"/> to access 
    ///     postback data relevant to their control or, if they develop a composite control, use the child controls' 
    ///     integrated data handling features to access the data.
    ///   </para>
    /// </remarks>
    [EditorBrowsable (EditorBrowsableState.Advanced)]
    public NameValueCollection PostBackCollection
    {
      get { return _postBackCollection; }
      set { _postBackCollection = value; }
    }

    protected WxeFunctionState FunctionState
    {
      get { return _functionState; }
    }

    public string FunctionToken
    {
      get { return _functionState.FunctionToken; }
    }

    public int PostBackID
    {
      get { return _functionState.PostBackID; }
    }

    public NameValueCollection QueryString
    {
      get { return _queryString; }
    }

    public WxeFunction ReturningFunction
    {
      get { return _returningFunction; }
      set { _returningFunction = value; }
    }

    /// <summary> Gets the URL that resumes the current function. </summary>
    /// <remarks>
    ///   If a WXE application branches to an external web site, the external site can
    ///   link back to this URL to resume the current function at the point where 
    ///   it was interrupted. Note that if the user stays on the external site longer
    ///   that the session or function timeout, resuming will fail with a timeout
    ///   exception.
    /// </remarks>
    public string GetResumeUrl (bool includeServer)
    {
      string pathPart = GetResumePath ();
      pathPart = _httpContext.Response.ApplyAppPathModifier (pathPart);
      if (includeServer)
      {
        string serverPart = _httpContext.Request.Url.GetLeftPart (System.UriPartial.Authority);
        return serverPart + pathPart;
      }
      else
      {
        return pathPart;
      }
    }

    /// <summary> Gets the absolute path that resumes the current function. </summary>
    protected internal string GetResumePath ()
    {
      return GetPath (_httpContext.Request.Url.AbsolutePath, FunctionToken, QueryString);
    }

    /// <summary> Gets the absolute path to the WXE handler used for the current function. </summary>
    /// <param name="queryString"> An optional list of URL parameters to be appended to the path. </param>
    protected internal string GetPath (NameValueCollection queryString)
    {
      if (queryString == null)
        queryString = new NameValueCollection ();

      string path = _httpContext.Response.ApplyAppPathModifier (_httpContext.Request.Url.AbsolutePath);
      return UrlUtility.AddParameters (path, queryString, _httpContext.Response.ContentEncoding);
    }

    /// <summary> Gets the absolute path that resumes the function with specified token. </summary>
    /// <param name="functionToken"> 
    ///   The function token of the function to resume. Must not be <see langword="null"/> or emtpy.
    /// </param>
    /// <param name="queryString"> An optional list of URL parameters to be appended to the path. </param>
    protected internal string GetPath (string functionToken, NameValueCollection queryString)
    {
      return GetPath (_httpContext.Request.Url.AbsolutePath, functionToken, queryString);
    }

    /// <summary> Gets the absolute path that resumes the function with specified token. </summary>
    /// <param name="path"> The path to the <see cref="WxeHandler"/>. Must not be <see langword="null"/> or emtpy. </param>
    /// <param name="functionToken"> 
    ///   The function token of the function to resume. Must not be <see langword="null"/> or emtpy.
    /// </param>
    /// <param name="queryString"> An optional list of URL parameters to be appended to the <paramref name="path"/>. </param>
    protected internal string GetPath (string path, string functionToken, NameValueCollection queryString)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);
      ArgumentUtility.CheckNotNullOrEmpty ("functionToken", functionToken);

      if (path.IndexOf ("?") != -1)
        throw new ArgumentException ("The path must be provided without a query string. Use the query string parameter instead.", "path");

      if (queryString == null)
        queryString = new NameValueCollection ();
      else
        queryString = NameValueCollectionUtility.Clone (queryString);

      queryString.Set (WxeHandler.Parameters.WxeFunctionToken, functionToken);

      path = _httpContext.Response.ApplyAppPathModifier (path);
      return UrlUtility.AddParameters (path, queryString, _httpContext.Response.ContentEncoding);
    }

    /// <summary> 
    ///   Gets the permanent URL for the <see cref="WxeFunction"/> of the specified <paramref name="functionType"/> 
    ///   and using the <paramref name="urlParameters"/>.
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\WxeContext.xml' path='WxeContext/GetPermanentUrl/param[@name="functionType" or @name="urlParameters"]' />
    public string GetPermanentUrl (Type functionType, NameValueCollection urlParameters)
    {
      return GetPermanentUrl (functionType, urlParameters, false);
    }

    /// <summary> 
    ///   Gets the permanent URL for the <see cref="WxeFunction"/> of the specified <paramref name="functionType"/> 
    ///   and using the <paramref name="urlParameters"/>.
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\WxeContext.xml' path='WxeContext/GetPermanentUrl/param[@name="functionType" or @name="urlParameters" or @name="useParentPermanentUrl"]' />
    public string GetPermanentUrl (Type functionType, NameValueCollection urlParameters, bool useParentPermanentUrl)
    {
      ArgumentUtility.CheckNotNull ("urlParameters", urlParameters);

      string permanentUrl = WxeContext.GetPermanentUrl (_httpContext, functionType, urlParameters, true);

      if (useParentPermanentUrl)
      {
        if (urlParameters[WxeHandler.Parameters.ReturnUrl] != null)
          throw new ArgumentException ("The 'urlParameters' collection must not contain a 'ReturnUrl' parameter when creating a parent permanent URL.", "urlParameters");

        int maxLength = Configuration.WebConfiguration.Current.ExecutionEngine.MaximumUrlLength;

        string currentFunctionUrl = UrlUtility.AddParameters (_httpContext.Request.Url.AbsolutePath, _queryString, _httpContext.Response.ContentEncoding);
        StringCollection parentPermanentUrls = ExtractReturnUrls (currentFunctionUrl);

        int count = GetMergeablePermanentUrlCount (permanentUrl, parentPermanentUrls, maxLength);
        string parentPermanentUrl = FormatParentPermanentUrl (parentPermanentUrls, count);

        if (!StringUtility.IsNullOrEmpty (parentPermanentUrl))
          permanentUrl = UrlUtility.AddParameter (permanentUrl, WxeHandler.Parameters.ReturnUrl, parentPermanentUrl, _httpContext.Response.ContentEncoding);
      }
      return permanentUrl;
    }

    /// <summary> Gets the URL to be used for transfering to the external function. </summary>
    internal string GetDestinationUrlForExternalFunction (WxeFunction function, string functionToken, WxePermaUrlOptions permaUrlOptions)
    {
      string href;
      if (permaUrlOptions.UsePermaUrl)
      {
        NameValueCollection internalUrlParameters;
        if (permaUrlOptions.UrlParameters == null)
          internalUrlParameters = function.VariablesContainer.SerializeParametersForQueryString ();
        else
          internalUrlParameters = permaUrlOptions.UrlParameters.Clone ();
        internalUrlParameters.Set (WxeHandler.Parameters.WxeFunctionToken, functionToken);

        href = GetPermanentUrl (function.GetType (), internalUrlParameters, permaUrlOptions.UseParentPermaUrl);
      }
      else
      {
        UrlMappingEntry mappingEntry = UrlMappingConfiguration.Current.Mappings[function.GetType ()];
        string path = (mappingEntry != null) ? mappingEntry.Resource : _httpContext.Request.Url.AbsolutePath;
        href = GetPath (path, functionToken, permaUrlOptions.UrlParameters);
      }

      return href;
    }

    /// <summary>Initalizes a new <see cref="WxeFunctionState"/> with the passed <paramref name="function"/> and returns the associated function token.</summary>
    internal string GetFunctionTokenForExternalFunction (WxeFunction function, bool returnFromExecute)
    {
      bool enableCleanUp = !returnFromExecute;
      WxeFunctionState functionState = new WxeFunctionState (function, enableCleanUp);
      _functionStateManager.Add (functionState);
      return functionState.FunctionToken;
    }

    private StringCollection ExtractReturnUrls (string url)
    {
      StringCollection returnUrls = new StringCollection ();

      while (!StringUtility.IsNullOrEmpty (url))
      {
        string currentUrl = url;
        url = UrlUtility.GetParameter (currentUrl, WxeHandler.Parameters.ReturnUrl, _httpContext.Request.ContentEncoding);

        if (!StringUtility.IsNullOrEmpty (url))
          currentUrl = UrlUtility.DeleteParameter (currentUrl, WxeHandler.Parameters.ReturnUrl);

        returnUrls.Add (currentUrl);
      }
      return returnUrls;
    }

    private string FormatParentPermanentUrl (StringCollection parentPermanentUrls, int count)
    {
      if (count > parentPermanentUrls.Count)
        throw new ArgumentOutOfRangeException ("count");

      string parentPermanentUrl = null;
      for (int i = count - 1; i >= 0; i--)
      {
        string temp = parentPermanentUrls[i];
        if (StringUtility.IsNullOrEmpty (parentPermanentUrl))
        {
          parentPermanentUrl = temp;
        }
        else
        {
          parentPermanentUrl = UrlUtility.AddParameter (temp, WxeHandler.Parameters.ReturnUrl, parentPermanentUrl, _httpContext.Response.ContentEncoding);
        }
      }
      return parentPermanentUrl;
    }

    private int GetMergeablePermanentUrlCount (string baseUrl, StringCollection parentPermanentUrls, int maxLength)
    {
      int i = 0;
      for (; i < parentPermanentUrls.Count; i++)
      {
        string parentPermanentUrl = FormatParentPermanentUrl (parentPermanentUrls, i + 1);
        if (parentPermanentUrl.Length >= maxLength)
          break;
        string url = UrlUtility.AddParameter (baseUrl, WxeHandler.Parameters.ReturnUrl, parentPermanentUrl, _httpContext.Response.ContentEncoding);
        if (url.Length > maxLength)
          break;
      }
      return i;
    }
  }

}
