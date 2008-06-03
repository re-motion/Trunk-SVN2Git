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
using System.Text;
using System.Web;
using System.Web.UI;
using Remotion.Collections;
using Remotion.Globalization;
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{

public class WxePageInfo: WxeTemplateControlInfo, IDisposable
{
  /// <summary> A list of resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
  ///   See the documentation of <b>GetString</b> for further details.
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Remotion.Web.Globalization.WxePageInfo")]
  protected enum ResourceIdentifier
  {
    /// <summary> Displayed when the user attempts to submit while the page is already aborting. </summary>
    StatusIsAbortingMessage,
    /// <summary> Displayed when the user returnes to a cached page that has already been submitted or aborted. </summary>
    StatusIsCachedMessage
  }

  public static readonly string ReturningTokenID = "wxeReturningTokenField";
  public static readonly string PageTokenID = "wxePageTokenField";
  public static readonly string PostBackSequenceNumberID = "wxePostBackSequenceNumberField";
  
  private const string c_scriptFileUrl = "ExecutionEngine.js";
  private const string c_styleFileUrl = "ExecutionEngine.css";
  private const string c_styleFileUrlForIE = "ExecutionEngineIE.css";

  private static readonly string s_scriptFileKey = typeof (WxePageInfo).FullName + "_Script";
  private static readonly string s_styleFileKey = typeof (WxePageInfo).FullName + "_Style";
  private static readonly string s_styleFileKeyForIE = typeof (WxePageInfo).FullName + "_StyleIE";

  private IWxePage _page;
  private WxeForm _wxeForm;
  private bool _postbackCollectionInitialized = false;
  private bool _isPostDataHandled = false;
  private NameValueCollection _postbackCollection = null;
  /// <summary> The <see cref="WxeFunctionState"/> designated by <b>WxeForm.ReturningToken</b>. </summary>
  private WxeFunctionState _returningFunctionState = null;

  private bool _executeNextStep = false;
  private HttpContext _httpContext;

  private string _statusIsAbortingMessage = string.Empty;
  private string _statusIsCachedMessage = string.Empty;

  /// <summary> Initializes a new instance of the <b>WxePageInfo</b> type. </summary>
  /// <param name="page"> 
  ///   The <see cref="IWxePage"/> containing this <b>WxePageInfo</b> object. 
  ///   The page must be derived from <see cref="System.Web.UI.Page">System.Web.UI.Page</see>.
  /// </param>
  public WxePageInfo (IWxePage page)
    : base (page)
  {
    ArgumentUtility.CheckNotNullAndType<Page> ("page", page);
    _page = page;
  }

  /// <summary>
  /// Finds the control if it is the page's form. Otherwise, call the find method of the page's base class.
  /// </summary>
  public Control FindControl (string id, out bool callBaseMethod)
  {
    if (_wxeForm != null && !StringUtility.IsNullOrEmpty(_wxeForm.ID) && id == _wxeForm.UniqueID)
    {
      callBaseMethod = false;
      return _wxeForm;
    }
    else
    {
      callBaseMethod = true;
      return null;
    }
  }

  public override void Initialize (HttpContext context)
  {
    ArgumentUtility.CheckNotNull ("context", context);
    base.Initialize (context);

    _httpContext = context;
    _httpContext.Handler = _page;

    if (! ControlHelper.IsDesignMode (_page, context))
    {
      _wxeForm = WxeForm.Replace (_page.HtmlForm);
      _page.HtmlForm = _wxeForm;
    }

    if (_page.CurrentStep != null)
      ScriptManager.RegisterHiddenField ((Page)_page, WxePageInfo.PageTokenID, CurrentStep.PageToken);

    _wxeForm.LoadPostData += new EventHandler(Form_LoadPostData);

    if (!ControlHelper.IsDesignMode (_page))
    {
      string url = ResourceUrlResolver.GetResourceUrl ((Page) _page, typeof (WxePageInfo), ResourceType.Html, c_scriptFileUrl);
      HtmlHeadAppender.Current.RegisterJavaScriptInclude (s_scriptFileKey, url);

      url = ResourceUrlResolver.GetResourceUrl ((Page) _page, typeof (WxePageInfo), ResourceType.Html, c_styleFileUrl);
      HtmlHeadAppender.Current.RegisterStylesheetLink (s_styleFileKey, url, HtmlHeadAppender.Priority.Library);

      //      url = ResourceUrlResolver.GetResourceUrl (page, typeof (WxePageInfo), ResourceType.Html, c_styleFileUrlForIE);
      //      HtmlHeadAppender.Current.RegisterStylesheetLingForInternetExplorerOnly
      //          (s_styleFileKeyForIE, url, HtmlHeadAppender.Priority.Library);
    }
  }

  
  public NameValueCollection EnsurePostBackModeDetermined (HttpContext context)
  {
    ArgumentUtility.CheckNotNull ("context", context);

    if (! _postbackCollectionInitialized)
    {
      _postbackCollection = DeterminePostBackMode (context);
      _postbackCollectionInitialized = true;
      if (_postbackCollection != null)
        EnsurePostDataHandled (_postbackCollection);
    }
    return _postbackCollection;
  }  

  private NameValueCollection DeterminePostBackMode (HttpContext httpContext)
  {
    WxeContext wxeContext = WxeContext.Current;
    if (wxeContext == null)
      return null;
    if (! wxeContext.IsPostBack)
      return null;
    if (wxeContext.PostBackCollection != null)
      return wxeContext.PostBackCollection;
    if (httpContext.Request == null)
      return null;

    NameValueCollection collection;
    if (StringUtility.AreEqual (httpContext.Request.HttpMethod, "POST", false))
      collection = httpContext.Request.Form;
    else
      collection = httpContext.Request.QueryString;

    if ((collection[ControlHelper.ViewStateID] == null) && (collection[ControlHelper.PostEventSourceID] == null))
      return null;
    else
      return collection;
  }


  private void Form_LoadPostData (object sender, EventArgs e)
  {
    NameValueCollection postBackCollection = _page.GetPostBackCollection();
    if (postBackCollection == null)
      throw new InvalidOperationException ("The IWxePage has no PostBackCollection even though this is a post back.");
    EnsurePostDataHandled (postBackCollection);
    
    if (WxeContext.Current.IsOutOfSequencePostBack && ! _page.AreOutOfSequencePostBacksEnabled)
      throw new WxePostbackOutOfSequenceException();
  }

  protected void EnsurePostDataHandled (NameValueCollection postBackCollection)
  {
    if (_isPostDataHandled)
      return;

    _isPostDataHandled = true;
    HandleLoadPostData (postBackCollection);
  }

  /// <exception cref="WxePostbackOutOfSequenceException"> 
  ///   Thrown if a postback with an incorrect sequence number is handled. 
  /// </exception>
  protected virtual void HandleLoadPostData (NameValueCollection postBackCollection)
  {
    ArgumentUtility.CheckNotNull ("postBackCollection", postBackCollection);

    WxeContext wxeContext = WxeContext.Current;

    int postBackID = int.Parse (postBackCollection[WxePageInfo.PostBackSequenceNumberID]);
    if (postBackID != wxeContext.PostBackID)
      wxeContext.SetIsOutOfSequencePostBack (true);

    string returningToken = postBackCollection[WxePageInfo.ReturningTokenID];
    if (! StringUtility.IsNullOrEmpty (returningToken))
    {
      WxeFunctionStateManager functionStates = WxeFunctionStateManager.Current;
      WxeFunctionState functionState = functionStates.GetItem (returningToken);
      if (functionState != null)
      {
        wxeContext.ReturningFunction = functionState.Function;
        wxeContext.SetIsReturningPostBack (true);
        _returningFunctionState = functionState;
      }
    }
  }

  public void OnPreRenderComplete ()
  {
    WxeContext wxeContext = WxeContext.Current;
    Page page = (Page) _page;

    ScriptManager.RegisterHiddenField (page, WxeHandler.Parameters.WxeFunctionToken, wxeContext.FunctionToken);
    ScriptManager.RegisterHiddenField (page, WxePageInfo.ReturningTokenID, null);
    int nextPostBackID = wxeContext.PostBackID + 1;
    ScriptManager.RegisterHiddenField (page, WxePageInfo.PostBackSequenceNumberID, nextPostBackID.ToString ());

    string key = "wxeDoSubmit";
    ScriptUtility.RegisterClientScriptBlock (page, key,
          "function wxeDoSubmit (button, pageToken) { \r\n"
        + "  var theForm = document." + _wxeForm.ClientID + "; \r\n"
        + "  theForm." + WxePageInfo.ReturningTokenID + ".value = pageToken; \r\n"
        + "  document.getElementById(button).click(); \r\n"
        + "}");

    key = "wxeDoPostBack";
    ScriptUtility.RegisterClientScriptBlock (page, key,
          "function wxeDoPostBack (control, argument, returningToken) { \r\n"
        + "  var theForm = document." + _wxeForm.ClientID + "; \r\n"
        + "  theForm." + WxePageInfo.ReturningTokenID + ".value = returningToken; \r\n"
        + "  __doPostBack (control, argument); \r\n"
        + "}");

    HtmlHeadAppender.Current.RegisterUtilitiesJavaScriptInclude ((Page) _page);
  
    RegisterWxeInitializationScript(); 
    SetCacheSettings();
  }

  private void SetCacheSettings()
  {
    HttpContext context = WxeContext.Current.HttpContext;
    context.Response.Cache.SetCacheability (HttpCacheability.Private);
  }

  private void RegisterWxeInitializationScript()
  {
    IResourceManager resourceManager = GetResourceManager();
    
    string temp;
    WxeContext wxeContext = WxeContext.Current;
    
    int refreshIntervall = 0;
    string refreshPath = "null";
    string abortPath = "null";
    if (WxeHandler.IsSessionManagementEnabled)
    {
      //  Ensure the registration of "__doPostBack" on the page.
      temp = _page.GetPostBackEventReference ((Page)_page);

      bool isAbortEnabled = _page.IsAbortEnabled;

      string resumePath = wxeContext.GetPath (wxeContext.FunctionToken, null);

      if (WxeHandler.IsSessionRefreshEnabled)
      {
        refreshIntervall = WxeHandler.RefreshInterval * 60000;
        refreshPath = "'" + resumePath + "&" + WxeHandler.Parameters.WxeAction + "=" + WxeHandler.Actions.Refresh + "'";
      }
      
      if (isAbortEnabled)
        abortPath = "'" + resumePath + "&" + WxeHandler.Parameters.WxeAction + "=" + WxeHandler.Actions.Abort + "'";
    }

    string statusIsAbortingMessage = "null";        
    string statusIsCachedMessage = "null";
    if (_page.AreStatusMessagesEnabled)
    {
      if (StringUtility.IsNullOrEmpty (_page.StatusIsAbortingMessage))
        temp = resourceManager.GetString (ResourceIdentifier.StatusIsAbortingMessage);
      else
        temp = _page.StatusIsAbortingMessage;
      statusIsAbortingMessage = "'" + ScriptUtility.EscapeClientScript (temp) + "'";        

      if (StringUtility.IsNullOrEmpty (_page.StatusIsCachedMessage))
        temp = resourceManager.GetString (ResourceIdentifier.StatusIsCachedMessage);
      else
        temp = _page.StatusIsCachedMessage;
      statusIsCachedMessage = "'" + ScriptUtility.EscapeClientScript (temp) + "'";
    }
 
    _page.RegisterClientSidePageEventHandler (SmartPageEvents.OnLoad, "WxePage_OnLoad", "WxePage_OnLoad");
    _page.RegisterClientSidePageEventHandler (SmartPageEvents.OnAbort, "WxePage_OnAbort", "WxePage_OnAbort");
    _page.RegisterClientSidePageEventHandler (SmartPageEvents.OnUnload, "WxePage_OnUnload", "WxePage_OnUnload");
    _page.CheckFormStateFunction = "WxePage_CheckFormState";

    string isCacheDetectionEnabled = _page.AreOutOfSequencePostBacksEnabled ? "false" : "true";
  
    StringBuilder initScript = new StringBuilder (500);

    initScript.AppendLine ("WxePage_Context.Instance = new WxePage_Context (");
    initScript.AppendLine ("    ").Append (isCacheDetectionEnabled).AppendLine (",");
    initScript.AppendLine ("    ").Append (refreshIntervall).AppendLine (",");
    initScript.AppendLine ("    ").Append (refreshPath).AppendLine (",");
    initScript.AppendLine ("    ").Append (abortPath).AppendLine (",");
    initScript.AppendLine ("    ").Append (statusIsAbortingMessage).AppendLine (",");
    initScript.AppendLine ("    ").Append (statusIsCachedMessage).AppendLine (");");

    ScriptUtility.RegisterClientScriptBlock ((Page)_page, "wxeInitialize", initScript.ToString());
  }

  
  /// <summary> Implements <see cref="IWxePage.StatusIsCachedMessage">IWxePage.StatusIsCachedMessage</see>. </summary>
  public string StatusIsCachedMessage
  {
    get { return _statusIsCachedMessage; }
    set { _statusIsCachedMessage = StringUtility.NullToEmpty (value); }
  }

  /// <summary> Implements <see cref="IWxePage.StatusIsAbortingMessage">IWxePage.StatusIsAbortingMessage</see>. </summary>
  public string StatusIsAbortingMessage
  {
    get { return _statusIsAbortingMessage; }
    set { _statusIsAbortingMessage = StringUtility.NullToEmpty (value); }
  }

  /// <summary> Implements <see cref="IWxePage.ExecuteNextStep">IWxePage.ExecuteNextStep</see>. </summary>
  public void ExecuteNextStep ()
  {
    _executeNextStep = true;
    _page.Visible = false; // suppress prerender and render events
  }


  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunction(Remotion.Web.ExecutionEngine.WxeFunction)">IWxePage.ExecuteFunction(WxeFunction)</see>.
  /// </summary>
  public void ExecuteFunction (WxeFunction function)
  {
    ExecuteFunction (function, false, false, null);
  }

  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunction(Remotion.Web.ExecutionEngine.WxeFunction,System.Boolean,System.Boolean)">IWxePage.ExecuteFunction(WxeFunction,Boolean,Boolean)</see>.
  /// </summary>
  public void ExecuteFunction (WxeFunction function, bool createPermaUrl, bool useParentPermaUrl)
  {
    ExecuteFunction (function, createPermaUrl, useParentPermaUrl, null);
  }

  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunction(Remotion.Web.ExecutionEngine.WxeFunction,System.Boolean,System.Boolean,System.Collections.Specialized.NameValueCollection)">IWxePage.ExecuteFunction(WxeFunction,Boolean,Boolean,NameValueCollection)</see>.
  /// </summary>
  public void ExecuteFunction (
      WxeFunction function, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters)
  {
    _httpContext.Handler = WxeHandler;
    try
    {
      CurrentStep.ExecuteFunction (_page, function, createPermaUrl, useParentPermaUrl, permaUrlParameters);
    }
    finally
    {
      _httpContext.Handler = _page;
    }
  }


  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunctionNoRepost(Remotion.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control)">IWxePage.ExecuteFunctionNoRepost (WxeFunction,Control)</see>.
  /// </summary>
  public void ExecuteFunctionNoRepost (WxeFunction function, Control sender)
  {
    ExecuteFunctionNoRepost (function, sender, UsesEventTarget, false, false, null);
  }

  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunctionNoRepost(Remotion.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control,System.Boolean)">IWxePage.ExecuteFunctionNoRepost(WxeFunction,Control,Boolean)</see>.
  /// </summary>
  public void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool usesEventTarget)
  {
    ExecuteFunctionNoRepost (function, sender, usesEventTarget, false, false, null);
  }

  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunctionNoRepost(Remotion.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control,System.Boolean,System.Boolean)">IWxePage.ExecuteFunctionNoRepost (WxeFunction,Control,Boolean,Boolean)</see>.
  /// </summary>
  public void ExecuteFunctionNoRepost (
      WxeFunction function, Control sender, bool createPermaUrl, bool useParentPermaUrl)
  {
    ExecuteFunctionNoRepost (function, sender, UsesEventTarget, createPermaUrl, useParentPermaUrl, null);
  }

  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunctionNoRepost(Remotion.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control,System.Boolean,System.Boolean,System.Collections.Specialized.NameValueCollection)">IWxePage.ExecuteFunctionNoRepost (WxeFunction,Control,Boolean,Boolean,NameValueCollection)</see>.
  /// </summary>
  public void ExecuteFunctionNoRepost (
      WxeFunction function, Control sender, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters)
  {
    ExecuteFunctionNoRepost (function, sender, UsesEventTarget, createPermaUrl, useParentPermaUrl, permaUrlParameters);
  }

  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunctionNoRepost(Remotion.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control,System.Boolean,System.Boolean,System.Boolean)">IWxePage.ExecuteFunctionNoRepost(WxeFunction,Control,Boolean,Boolean,Boolean)</see>.
  /// </summary>
  public void ExecuteFunctionNoRepost (
      WxeFunction function, Control sender, bool usesEventTarget, bool createPermaUrl, bool useParentPermaUrl)
  {
    ExecuteFunctionNoRepost (function, sender, usesEventTarget, createPermaUrl, useParentPermaUrl, null);
  }

  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunctionNoRepost(Remotion.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control,System.Boolean,System.Boolean,System.Boolean,System.Collections.Specialized.NameValueCollection)">IWxePage.ExecuteFunctionNoRepost(WxeFunction,Control,Boolean,Boolean,Boolean,NameValueCollection)</see>.
  /// </summary>
  public void ExecuteFunctionNoRepost (
      WxeFunction function, Control sender, bool usesEventTarget, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters)
  {
    _httpContext.Handler = WxeHandler;
    try
    {
      CurrentStep.ExecuteFunctionNoRepost (
          _page, function, sender, usesEventTarget, createPermaUrl, useParentPermaUrl, permaUrlParameters);
    }
    finally
    {
      _httpContext.Handler = _page;
    }
  }

  /// <summary> 
  ///   Gets a flag describing whether the post back was most likely caused by the ASP.NET post back mechanism.
  /// </summary>
  /// <value> <see langword="true"/> if the post back collection contains the <b>__EVENTTARGET</b> field. </value>
  protected bool UsesEventTarget
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
      return ! StringUtility.IsNullOrEmpty (postBackCollection[ControlHelper.PostEventSourceID]); 
    }
  }

  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunctionExternal(Remotion.Web.ExecutionEngine.WxeFunction,System.Boolean,System.Boolean,System.Collections.Specialized.NameValueCollection)">IWxePage.ExecuteFunctionExternal(WxeFunction,Boolean,Boolean,NameValueCollection)</see>.
  /// </summary>
  public void ExecuteFunctionExternal (
      WxeFunction function, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters)
  {
    ExecuteFunctionExternal (function, createPermaUrl, useParentPermaUrl, urlParameters, true, null);
  }

  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunctionExternal(Remotion.Web.ExecutionEngine.WxeFunction,System.Boolean,System.Boolean,System.Collections.Specialized.NameValueCollection,System.Boolean,System.Collections.Specialized.NameValueCollection)">IWxePage.ExecuteFunctionExternal(WxeFunction,Boolean,Boolean,NameValueCollection,Boolean,NameValueCollection)</see>.
  /// </summary>
  public void ExecuteFunctionExternal (
      WxeFunction function, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters, 
      bool returnToCaller, NameValueCollection callerUrlParameters)
  {
    _httpContext.Handler = WxeHandler;
    try
    {
      CurrentStep.ExecuteFunctionExternal (
          _page, function, createPermaUrl, useParentPermaUrl, urlParameters, returnToCaller, callerUrlParameters);
    }
    finally
    {
      _httpContext.Handler = _page;
    }
  }

  #region ExecuteFunctionExternalNoRepost
//  /// <summary>
//  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunctionExternalNoRepost(Remotion.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control,System.Boolean,System.Boolean,System.Collections.Specialized.NameValueCollection)">IWxePage.ExecuteFunctionExternalNoRepost(WxeFunction,Control,Boolean,Boolean,NameValueCollection)</see>.
//  /// </summary>
//  public void ExecuteFunctionExternalNoRepost (
//      WxeFunction function, Control sender,
//      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters)
//  {
//    ExecuteFunctionExternalNoRepost (function, sender, UsesEventTarget, createPermaUrl, useParentPermaUrl, urlParameters, true, null);
//  }
//
//  /// <summary>
//  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunctionExternalNoRepost(Remotion.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control,System.Boolean,System.Boolean,System.Boolean,System.Collections.Specialized.NameValueCollection)">IWxePage.ExecuteFunctionExternalNoRepost(WxeFunctionControl,Boolean,Boolean,Boolean,NameValueCollection)</see>.
//  /// </summary>
//  public void ExecuteFunctionExternalNoRepost (
//      WxeFunction function, Control sender, bool usesEventTarget,
//      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters)
//  {
//    ExecuteFunctionExternalNoRepost (function, sender, usesEventTarget, createPermaUrl, useParentPermaUrl, urlParameters, true, null);
//  }
//
//  /// <summary>
//  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunctionExternalNoRepost(Remotion.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control,System.Boolean,System.Boolean,System.Collections.Specialized.NameValueCollection,System.Boolean,System.Collections.Specialized.NameValueCollection)">IWxePage.ExecuteFunctionExternalNoRepost(WxeFunction,Control,Boolean,Boolean,NameValueCollection,Boolean,NameValueCollection)</see>.
//  /// </summary>
//  public void ExecuteFunctionExternalNoRepost (
//      WxeFunction function, Control sender,
//      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters, 
//      bool returnToCaller, NameValueCollection callerUrlParameters)
//  {
//    ExecuteFunctionExternalNoRepost (
//        function, sender, UsesEventTarget, createPermaUrl, useParentPermaUrl, urlParameters, returnToCaller, callerUrlParameters);
//  }
//
//  /// <summary>
//  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunctionExternalNoRepost(Remotion.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control,System.Boolean,System.Boolean,System.Boolean,System.Collections.Specialized.NameValueCollection,System.Boolean,System.Collections.Specialized.NameValueCollection)">IWxePage.ExecuteFunctionExternalNoRepost(WxeFunction,Control,Boolean,Boolean,Boolean,NameValueCollection,Boolean,NameValueCollection)</see>.
//  /// </summary>
//  public void ExecuteFunctionExternalNoRepost (
//      WxeFunction function, Control sender, bool usesEventTarget,
//      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters, 
//      bool returnToCaller, NameValueCollection callerUrlParameters)
//  {
//    _httpContext.Handler = WxeHandler;
//    try
//    {
//      CurrentStep.ExecuteFunctionExternalNoRepost (
//          _page, function, sender, usesEventTarget, createPermaUrl, useParentPermaUrl, urlParameters, returnToCaller, callerUrlParameters);
//    }
//    finally
//    {
//      _httpContext.Handler = _page;
//    }
//  }
  #endregion

  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunctionExternal(Remotion.Web.ExecutionEngine.WxeFunction,System.String,System.Web.UI.Control,System.Boolean)">IWxePage.ExecuteFunctionExternal(WxeFunction,String,Control,Boolean)</see>.
  /// </summary>
  public void ExecuteFunctionExternal (WxeFunction function, string target, Control sender, bool returningPostback)
  {
    ExecuteFunctionExternal (function, target, null, sender, returningPostback, false, false, null);
  }

  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunctionExternal(Remotion.Web.ExecutionEngine.WxeFunction,System.String,System.String,System.Web.UI.Control,System.Boolean)">IWxePage.ExecuteFunctionExternal(WxeFunction,String,String,Control,Boolean)</see>.
  /// </summary>
  public void ExecuteFunctionExternal (
      WxeFunction function, string target, string features, Control sender, bool returningPostback)
  {
    ExecuteFunctionExternal (function, target, features, sender, returningPostback, false, false, null);
  }

  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunctionExternal(Remotion.Web.ExecutionEngine.WxeFunction,System.String,System.Web.UI.Control,System.Boolean,System.Boolean,System.Boolean)">IWxePage.ExecuteFunctionExternal(WxeFunction,String,Control,Boolean,Boolean,Boolean)</see>.
  /// </summary>
  public void ExecuteFunctionExternal (
      WxeFunction function, string target, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl)
  {
    ExecuteFunctionExternal (function, target, null, sender, returningPostback, createPermaUrl, useParentPermaUrl, null);
  }

  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunctionExternal(Remotion.Web.ExecutionEngine.WxeFunction,System.String,System.Web.UI.Control,System.Boolean,System.Boolean,System.Boolean,System.Collections.Specialized.NameValueCollection)">IWxePage.ExecuteFunctionExternal(WxeFunction,String,Control,Boolean,Boolean,Boolean,NameValueCollection)</see>.
  /// </summary>
  public void ExecuteFunctionExternal (
      WxeFunction function, string target, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters)
  {
    ExecuteFunctionExternal (function, target, null, sender, returningPostback, createPermaUrl, useParentPermaUrl, urlParameters);
  }

  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunctionExternal(Remotion.Web.ExecutionEngine.WxeFunction,System.String,System.String,System.Web.UI.Control,System.Boolean,System.Boolean,System.Boolean)">IWxePage.ExecuteFunctionExternal(WxeFunction,String,String,Control,Boolean,Boolean,Boolean)</see>.
  /// </summary>
  public void ExecuteFunctionExternal (
      WxeFunction function, string target, string features, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl)
  {
    ExecuteFunctionExternal (function, target, features, sender, returningPostback, createPermaUrl, useParentPermaUrl, null);
  }

  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.ExecuteFunctionExternal(Remotion.Web.ExecutionEngine.WxeFunction,System.String,System.String,System.Web.UI.Control,System.Boolean,System.Boolean,System.Boolean,System.Collections.Specialized.NameValueCollection)">IWxePage.ExecuteFunctionExternal(WxeFunction,String,String,Control,Boolean,Boolean,Boolean,NameValueCollection)</see>.
  /// </summary>
  public void ExecuteFunctionExternal (
      WxeFunction function, string target, string features, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters)
  {
    ArgumentUtility.CheckNotNull ("function", function);
    ArgumentUtility.CheckNotNullOrEmpty ("target", target);

    WxeContext wxeContext = WxeContext.Current;

    string functionToken = CurrentStep.GetFunctionTokenForExternalFunction (function, returningPostback);

    string href = CurrentStep.GetDestinationUrlForExternalFunction (
        function, functionToken, createPermaUrl, useParentPermaUrl, urlParameters);

    string openScript;
    if (features != null)
      openScript = string.Format ("window.open('{0}', '{1}', '{2}');", href, target, features);
    else
      openScript = string.Format ("window.open('{0}', '{1}');", href, target);
    ScriptUtility.RegisterStartupScriptBlock ((Page) _page, "WxeExecuteFunction", openScript);

    function.ReturnUrl = 
        "javascript:" + GetClosingScriptForExternalFunction (functionToken, sender, returningPostback);
  }

  /// <summary> Gets the client script to be used as the return URL for the window of the external function. </summary>
  private string GetClosingScriptForExternalFunction (string functionToken, Control sender, bool returningPostback)
  {
    if (! returningPostback)
    {
      return "window.close();";
    }
    else if (UsesEventTarget)
    {
      NameValueCollection postBackCollection = _page.GetPostBackCollection();
      if (postBackCollection == null)
          throw new InvalidOperationException ("The IWxePage has no PostBackCollection even though this is a post back.");
      
      string eventTarget = postBackCollection[ControlHelper.PostEventSourceID];
      string eventArgument = postBackCollection[ControlHelper.PostEventArgumentID];
      return FormatDoPostBackClientScript (
          functionToken, _page.CurrentStep.PageToken, sender.ClientID, eventTarget, eventArgument);
    }
    else
    {
      ArgumentUtility.CheckNotNull ("sender", sender);
      if (! (sender is IPostBackEventHandler || sender is IPostBackDataHandler))
        throw new ArgumentException ("The sender must implement either IPostBackEventHandler or IPostBackDataHandler. Provide the control that raised the post back event.");
      return FormatDoSubmitClientScript (functionToken, _page.CurrentStep.PageToken, sender.ClientID);
    }
  }

  /// <summary> 
  ///   Gets the client script used to execute <c>__dopostback</c> in the parent form before closing the window of the 
  ///   external function.
  /// </summary>
  private string FormatDoPostBackClientScript (
      string functionToken, string pageToken, string senderID, string eventTarget, string eventArgument)
  {
    return string.Format (
          "\r\n"
        + "if (   window.opener != null \r\n"
        + "    && ! window.opener.closed \r\n"
        + "    && window.opener.wxeDoPostBack != null \r\n"
        + "    && window.opener.document.getElementById('{0}') != null \r\n"
        + "    && window.opener.document.getElementById('{0}').value == '{1}') \r\n"
        + "{{ \r\n"
        + "  window.opener.wxeDoPostBack('{2}', '{3}', '{4}'); \r\n"
        + "}} \r\n"
        + "window.close(); \r\n",
        WxePageInfo.PageTokenID, pageToken, eventTarget, eventArgument, functionToken);
  }

  /// <summary> 
  ///   Gets the client script used to submit the parent form before closing the window of the external function. 
  /// </summary>
  private string FormatDoSubmitClientScript (string functionToken, string pageToken, string senderID)
  {
    return string.Format (
          "\r\n"
        + "if (   window.opener != null \r\n"
        + "    && ! window.opener.closed \r\n"
        + "    && window.opener.wxeDoSubmit != null \r\n"
        + "    && window.opener.document.getElementById('{0}') != null \r\n"
        + "    && window.opener.document.getElementById('{0}').value == '{1}') \r\n"
        + "{{ \r\n"
        + "  window.opener.wxeDoSubmit('{2}', '{3}'); \r\n"
        + "}} \r\n"
        + "window.close(); \r\n",
        WxePageInfo.PageTokenID, pageToken, senderID, functionToken);
  }

  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.GetPermanentUrlParameters()">IWxePage.GetPermanentUrlParameters()</see>.
  /// </summary>
  public NameValueCollection GetPermanentUrlParameters()
  {
    NameValueCollection urlParameters = CurrentFunction.SerializeParametersForQueryString();
    
    ISmartNavigablePage smartNavigablePage = _page as ISmartNavigablePage;
    if (smartNavigablePage != null)
      NameValueCollectionUtility.Append (urlParameters, smartNavigablePage.GetNavigationUrlParameters());

    return urlParameters;
  }

  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.GetPermanentUrl()">IWxePage.GetPermanentUrl()</see>.
  /// </summary>
  public string GetPermanentUrl ()
  {
    return GetPermanentUrl (CurrentFunction.GetType(), GetPermanentUrlParameters());
  }
  
  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.GetPermanentUrl(System.Collections.Specialized.NameValueCollection)">IWxePage.GetPermanentUrl(NameValueCollection)</see>.
  /// </summary>
  public string GetPermanentUrl (NameValueCollection urlParameters)
  {
    return GetPermanentUrl (CurrentFunction.GetType(), urlParameters);
  }
  
  /// <summary>
  ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.GetPermanentUrl(System.Type,System.Collections.Specialized.NameValueCollection)">IWxePage.GetPermanentUrl(Type,NameValueCollection)</see>.
  /// </summary>
  public string GetPermanentUrl (Type functionType, NameValueCollection urlParameters)
  {
    return WxeContext.Current.GetPermanentUrl (functionType, urlParameters);
  }

  /// <summary> Implements <see cref="IWxePage.IsOutOfSequencePostBack">IWxePage.IsOutOfSequencePostBack</see>. </summary>
  public bool IsOutOfSequencePostBack
  {
    get 
    { 
      WxeContext wxeContext = WxeContext.Current;
      return ((wxeContext == null) ? false : wxeContext.IsOutOfSequencePostBack); 
    }
  }

  /// <summary> Implements <see cref="IWxePage.IsReturningPostBack">IWxePage.IsReturningPostBack</see>. </summary>
  public bool IsReturningPostBack
  {
    get 
    { 
      WxeContext wxeContext = WxeContext.Current;
      return ((wxeContext == null) ? false : wxeContext.IsReturningPostBack); 
    }
  }

  /// <summary> Implements <see cref="IWxePage.ReturningFunction">IWxePage.ReturningFunction</see>. </summary>
  public WxeFunction ReturningFunction
  {
    get 
    { 
      WxeContext wxeContext = WxeContext.Current;
      return ((wxeContext == null) ? null : wxeContext.ReturningFunction); 
    }
  }

  /// <summary> Saves the viewstate into the executing <see cref="WxePageStep"/>. </summary>
  /// <param name="viewState"> An <b>ASP.NET</b> viewstate object. </param>
  public void SavePageStateToPersistenceMedium (object viewState)
  {
    CurrentStep.SavePageStateToPersistenceMedium (viewState);
  }

  /// <summary> Returns the viewstate previously saved into the executing <see cref="WxePageStep"/>. </summary>
  /// <returns> An <b>ASP.NET</b> viewstate object. </returns>
  public object LoadPageStateFromPersistenceMedium()
  {
    return CurrentStep.LoadPageStateFromPersistenceMedium ();
  }


  /// <summary> 
  ///   If <see cref="ExecuteNextStep"/> has been called prior to disposing the page, <b>Dispose</b> will
  ///   break execution of this page life cycle and allow the Execution Engine to continue with the next step.
  /// </summary>
  /// <remarks> 
  ///   <para>
  ///     If <see cref="ExecuteNextStep"/> has been called, <b>Dispose</b> clears the <see cref="HttpResponse"/>'s
  ///     output and ends the execution of the current step by throwing a <see cref="WxeExecuteNextStepException"/>. 
  ///     This exception is handled by the Execution Engine framework.
  ///   </para>
  ///   <note>
  ///     See the remarks section of <see cref="IWxePage"/> for details on calling <b>Dispose</b>.
  ///   </note>
  /// </remarks>
  public void Dispose ()
  {
    if (ControlHelper.IsDesignMode (_page, _httpContext))
      return;

    _httpContext.Handler = WxeHandler;

    if (_returningFunctionState != null)
    {
      bool isRootFunction = _returningFunctionState.Function == _returningFunctionState.Function.RootFunction;
      if (isRootFunction)
        WxeFunctionStateManager.Current.Abort (_returningFunctionState);
    }

    if (_executeNextStep)
    {
      _httpContext.Response.Clear(); // throw away page trace output
      throw new WxeExecuteNextStepException();
    }
  }

  public WxeForm WxeForm
  {
    get { return _wxeForm; }
  }

  
  /// <summary> Find the <see cref="IResourceManager"/> for this WxePageInfo. </summary>
  protected virtual IResourceManager GetResourceManager()
  {
    return GetResourceManager (typeof (ResourceIdentifier));
  }


  private NameObjectCollection WindowState
  {
    get
    {
      NameObjectCollection windowState = 
          (NameObjectCollection) CurrentFunction.RootFunction.Variables["WxeWindowState"];
      if (windowState == null)
      {
        windowState = new NameObjectCollection();
        CurrentFunction.RootFunction.Variables["WxeWindowState"] = windowState;
      }
      return windowState;
    }
  }

  /// <summary>
  ///   Implements <see cref="Remotion.Web.UI.IWindowStateManager.GetData">Remotion.Web.UI.IWindowStateManager.GetData</see>.
  /// </summary>
  public object GetData (string key)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("key", key);
    return WindowState[key]; 
  }

  /// <summary>
  ///   Implements <see cref="Remotion.Web.UI.IWindowStateManager.SetData">Remotion.Web.UI.IWindowStateManager.SetData</see>.
  /// </summary>
  public void SetData (string key, object value)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("key", key);
    WindowState[key] = value;
  }

}

}
