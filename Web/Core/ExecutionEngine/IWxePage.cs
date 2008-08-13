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

    /// <summary>Executes the <paramref name="function"/> using the specified <paramref name="callArguments"/>.</summary>
    /// <param name="function">The <see cref="WxeFunction"/> to be executed.</param>
    /// <param name="callArguments">The <see cref="IWxeCallArguments"/> used to control the function invocation.</param>
    void ExecuteFunction (WxeFunction function, IWxeCallArguments callArguments);

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
