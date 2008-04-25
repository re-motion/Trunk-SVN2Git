using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;
using Remotion.Web.UI;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary> This interface represents a page that can be used in a <see cref="WxePageStep"/>. </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/Class/*' />
  public interface IWxePage: ISmartPage, IWxeTemplateControl
  {
    /// <summary> End this page step and continue with the WXE function. </summary>
    void ExecuteNextStep ();

    /// <summary> Executes the <paramref name="function"/> in the current. </summary>
    /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunction/param[@name="function"]' />
    void ExecuteFunction (WxeFunction function);

    /// <summary> Executes the <paramref name="function"/> in the current window. </summary>
    /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunction/param[@name="function" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
    void ExecuteFunction (WxeFunction function, bool createPermaUrl, bool useParentPermaUrl);

    /// <summary> Executes the <paramref name="function"/> in the current window. </summary>
    /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunction/param[@name="function" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
    void ExecuteFunction (WxeFunction function, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters);

    /// <summary>
    ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event 
    ///   on returning.
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender"]' />
    void ExecuteFunctionNoRepost (WxeFunction function, Control sender);
  
    /// <summary>
    ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event 
    ///   on returning.
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget"]' />
    void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool usesEventTarget);

    /// <summary>
    ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event 
    ///   on returning.
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
    void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool createPermaUrl, bool useParentPermaUrl);

    /// <summary>
    ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event 
    ///   on returning.
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
    void ExecuteFunctionNoRepost (
        WxeFunction function, Control sender, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters);
  
    /// <summary>
    ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event 
    ///   on returning.
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
    void ExecuteFunctionNoRepost (
        WxeFunction function, Control sender, bool usesEventTarget, bool createPermaUrl, bool useParentPermaUrl);
  
    /// <summary>
    ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event 
    ///   on returning.
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
    void ExecuteFunctionNoRepost (
        WxeFunction function, Control sender, bool usesEventTarget, 
        bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters);

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the 
    ///   new function.
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters"]' />
    void ExecuteFunctionExternal (
        WxeFunction function, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters);

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the 
    ///   new function.
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters" or @name="returnToCaller" or @name="callerUrlParameters"]' />
    void ExecuteFunctionExternal (
        WxeFunction function, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters,
        bool returnToCaller, NameValueCollection callerUrlParameters);

    #region ExecuteFunctionExternalNoRepost
    //  /// <summary> 
    //  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    //  ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the 
    //  ///   new function.
    //  /// </summary>
    //  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternalNoRepost/param[@name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
    //  void ExecuteFunctionExternalNoRepost (
    //      WxeFunction function, Control sender, 
    //      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters);
    //
    //  /// <summary> 
    //  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    //  ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the 
    //  ///   new function.
    //  /// </summary>
    //  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternalNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
    //  void ExecuteFunctionExternalNoRepost (
    //      WxeFunction function, Control sender, bool usesEventTarget, 
    //      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters);
    //
    //  /// <summary> 
    //  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    //  ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the 
    //  ///   new function.
    //  /// </summary>
    //  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternalNoRepost/param[@name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters" or @name="returnToCaller" or @name="callerUrlParameters"]' />
    //  void ExecuteFunctionExternalNoRepost (
    //      WxeFunction function, Control sender, 
    //      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters,
    //      bool returnToCaller, NameValueCollection callerUrlParameters);
    //
    //  /// <summary> 
    //  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    //  ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the 
    //  ///   new function.
    //  /// </summary>
    //  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternalNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters" or @name="returnToCaller" or @name="callerUrlParameters"]' />
    //  void ExecuteFunctionExternalNoRepost (
    //      WxeFunction function, Control sender, bool usesEventTarget, 
    //      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters,
    //      bool returnToCaller, NameValueCollection callerUrlParameters);
    #endregion

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   specified window or frame through javascript window.open(...).
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback"]' />
    void ExecuteFunctionExternal (WxeFunction function, string target, Control sender, bool returningPostback);
  
    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   specified window or frame through javascript window.open(...).
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback"]' />
    void ExecuteFunctionExternal (WxeFunction function, string target, string features, Control sender, bool returningPostback);

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   specified window or frame through javascript window.open(...).
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
    void ExecuteFunctionExternal (
        WxeFunction function, string target, Control sender, bool returningPostback, 
        bool createPermaUrl, bool useParentPermaUrl);

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   specified window or frame through javascript window.open(...).
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters"]' />
    void ExecuteFunctionExternal (
        WxeFunction function, string target, Control sender, bool returningPostback, 
        bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters);
  
    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   specified window or frame through javascript window.open(...).
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
    void ExecuteFunctionExternal (
        WxeFunction function, string target, string features, Control sender, bool returningPostback, bool createPermaUrl, bool useParentPermaUrl);
  
    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   specified window or frame through javascript window.open(...).
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters"]' />
    void ExecuteFunctionExternal (
        WxeFunction function, string target, string features, Control sender, bool returningPostback, 
        bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters);

    /// <summary> Gets a flag describing whether this post-back has been triggered by returning from a WXE function. </summary>
    bool IsReturningPostBack { get; }

    /// <summary> Gets the WXE function that has been executed in the current page. </summary>
    WxeFunction ReturningFunction { get; }

    /// <summary>
    ///   Gets a flag that determines whether to abort the session upon closing the window. 
    ///  </summary>
    /// <value> <see langword="true"/> to abort the session upon navigtion away from the page. </value>
    bool IsAbortEnabled { get; }

    /// <summary>
    ///   Gets a flag that determines whether to allow out-of-sequence postbacks (i.e. post-backs from an already 
    ///   submitted page because of the cache). 
    ///  </summary>
    /// <value> <see langword="true"/> to enable out of sequence post-backs. </value>
    /// <remarks> 
    ///   <see cref="AreOutOfSequencePostBacksEnabled"/> should only return <see langword="true"/> if 
    ///   <see cref="IsAbortEnabled"/> evaluates <see langword="false"/>.
    /// </remarks>
    bool AreOutOfSequencePostBacksEnabled { get; }

    /// <summary>
    ///   Gets a flag that describes whether the current postback cycle was caused by resubmitting a page from the 
    ///   client's cache.
    /// </summary>
    /// <value> <see langword="true"/> if the page has been re-submitted. </value>
    bool IsOutOfSequencePostBack { get; }

    /// <summary> 
    ///   Gets a flag whether the status messages (i.e. is submitting, is aborting) will be displayed when the user
    ///   tries to e.g. postback while a request is being processed.
    /// </summary>
    bool AreStatusMessagesEnabled { get; }

    /// <summary> Gets the message displayed when the user attempts to submit while the page is already aborting. </summary>
    /// <remarks> 
    ///   In case of an empty <see cref="String"/>, the text is read from the resources for <see cref="WxePageInfo"/>. 
    /// </remarks>
    string StatusIsAbortingMessage { get; }

    /// <summary> 
    ///   Gets the message displayed when the user returnes to a cached page that has already been submited or aborted. 
    /// </summary>
    /// <remarks> 
    ///   In case of an empty <see cref="String"/>, the text is read from the resources for <see cref="WxePageInfo"/>. 
    /// </remarks>
    string StatusIsCachedMessage { get; }

    /// <summary> Gets the permanent URL parameters the current page. </summary>
    NameValueCollection GetPermanentUrlParameters();

    /// <summary> Gets the permanent URL for the current page. </summary>
    string GetPermanentUrl();
  
    /// <summary> Gets the permanent URL for the current page using the specified <paramref name="queryString"/>. </summary>
    /// <include file='doc\include\ExecutionEngine\IWxePage.xml' 
    ///     path='IWxePage/GetPermanentUrl/param[@name="queryString"]' />
    string GetPermanentUrl (NameValueCollection queryString);
  
    /// <summary> 
    ///   Gets the permanent URL for the <see cref="WxeFunction"/> of the specified <paramref name="functionType"/> 
    ///   and using the <paramref name="queryString"/>.
    /// </summary>
    /// <include file='doc\include\ExecutionEngine\IWxePage.xml' 
    ///     path='IWxePage/GetPermanentUrl/param[@name="functionType" or @name="queryString"]' />
    string GetPermanentUrl (Type functionType, NameValueCollection queryString);

    /// <summary> Gets or sets the <see cref="WxeHandler"/> of the current request. </summary>
    [EditorBrowsable (EditorBrowsableState.Never)]
    WxeHandler WxeHandler { get; }
  }
}