// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Remotion.Context;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI
{
  /// <summary>
  ///   Provides a mechanism to register HTML header elements (e.g., stylesheet or script links).
  /// </summary>
  /// <example>
  ///   Insert the following line into the head element of the webform you want to add 
  ///   the registered controls to.
  ///   <code>
  ///     &lt;rwc:htmlheadcontents runat="server" id="HtmlHeadContents"&gt;&lt;/rwc:htmlheadcontents&gt;
  ///   </code>
  ///   Register a new <c>HTML head element</c> using the following syntax.
  ///   <code>
  ///     HtmlHeadAppender.Current.Register...(key, ...);
  ///   </code>
  /// </example>
  public sealed class HtmlHeadAppender
  {
    private const string c_contextKey = "Remotion.Web.UI.HtmlHeadAppender.Current";

    public enum Priority
    {
      Script = 0, // Absolute values to emphasize sorted nature of enum values
      Library = 1,
      UserControl = 2,
      Page = 3
    }

    /// <summary>
    ///   Gets the <see cref="HtmlHeadAppender"/> instance.
    /// </summary>
    public static HtmlHeadAppender Current
    {
      get
      {
        var current = (HtmlHeadAppender) SafeContext.Instance.GetData (c_contextKey);

        if (current == null)
        {
          current = new HtmlHeadAppender ();
          SafeContext.Instance.SetData (c_contextKey, current);
        }

        return current;
      }
    }

    /// <summary> ListDictionary&lt;string key, Control headElement&gt; </summary>
    private readonly ListDictionary _registeredHeadElements = new ListDictionary();

    /// <summary> SortedList&lt;Priority (int) priority, ArrayList headElements &gt; </summary>
    private readonly SortedList _sortedHeadElements = new SortedList();

    /// <summary> <see langword="true"/> if <see cref="EnsureAppended"/> has already executed. </summary>
    private bool _hasAppendExecuted;

    private WeakReference _handler = new WeakReference (null);

    /// <remarks>
    ///   Factory pattern. No public construction.
    /// </remarks>
    /// <exclude/>
    private HtmlHeadAppender ()
    {
    }

    /// <summary>
    ///   Appends the <c>HTML head elements</c> registered with the <see cref="Current"/>
    ///   <see cref="HtmlHeadAppender"/> to the <paramref name="htmlHeadContents"/>' <b>Controls</b> collection.
    /// </summary>
    /// <remarks>
    ///   Call this method during the rendering of the web form's <c>head element</c>.
    /// </remarks>
    /// <param name="htmlHeadContents">
    ///   <see cref="HtmlHeadContents"/> to whose <b>Controls</b> collection the headers will be appended.
    ///   Must not be <see langword="null"/>.
    /// </param>
    public void EnsureAppended (HtmlHeadContents htmlHeadContents)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadContents", htmlHeadContents);

      EnsureStateIsClearedAfterServerTransfer();

      if (_hasAppendExecuted)
        return;

      if (ControlHelper.IsDesignMode (htmlHeadContents))
        return;
      if (ControlHelper.IsDesignMode (htmlHeadContents))
        htmlHeadContents.Controls.Clear();

      for (int idxPriority = 0; idxPriority < _sortedHeadElements.Count; idxPriority++)
      {
        ArrayList headElements = (ArrayList) _sortedHeadElements.GetByIndex (idxPriority);
        for (int idxElements = 0; idxElements < headElements.Count; idxElements++)
        {
          Control headElement = (Control) headElements[idxElements];
          if (! htmlHeadContents.Controls.Contains (headElement))
            htmlHeadContents.Controls.Add (headElement);
        }
      }

      if (ControlHelper.IsDesignMode (htmlHeadContents))
      {
        _sortedHeadElements.Clear();
        _registeredHeadElements.Clear();
      }
      else
        _hasAppendExecuted = true;
    }

    /// <summary> Gets a flag indicating wheter <see cref="EnsureAppended"/> has been executed. </summary>
    /// <value> <see langword="true"/> if  <see cref="EnsureAppended"/> has been executed. </value>
    /// <remarks> Use this property to ensure that an <see cref="HtmlHeadContents"/> is present on the page. </remarks>
    public bool HasAppended
    {
      get { return _hasAppendExecuted; }
    }

    /// <summary>
    ///   Sets the <c>title</c> of the page.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     All calls to <see cref="SetTitle"/> must be completed before
    ///     <see cref="EnsureAppended"/> is called. (Typically during the <c>Render</c> phase.)
    ///   </para><para>
    ///     Remove the title tag from the aspx-source.
    ///   </para><para>
    ///     Registeres the title with a default priority of Page.
    ///   </para>
    /// </remarks>
    /// <param name="title"> The stirng to be isnerted as the title. </param>
    public void SetTitle (string title)
    {
      const string key = "title";

      if (IsRegistered (key))
        ((HtmlGenericControl) _registeredHeadElements[key]).InnerText = title;
      else
      {
        HtmlGenericControl headElement = new HtmlGenericControl ("title");
        headElement.EnableViewState = false;
        headElement.InnerText = title;
        RegisterHeadElement ("title", headElement, Priority.Page);
      }
    }

    /// <summary> Registers a stylesheet file. </summary>
    /// <remarks>
    ///   All calls to <see cref="RegisterStylesheetLink(string, string, Priority)"/> must be completed before
    ///   <see cref="EnsureAppended"/> is called. (Typically during the <c>Render</c> phase.)
    /// </remarks>
    /// <param name="key"> 
    ///   The unique key identifying the stylesheet file in the headers collection. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="url"> The url of the stylesheet file. Must not be <see langword="null"/>. </param>
    /// <param name="priority"> 
    ///   The priority level of the head element. Elements are rendered in the following order:
    ///   Library, UserControl, Page.
    /// </param>
    /// <exception cref="HttpException"> 
    ///   Thrown if method is called after <see cref="EnsureAppended"/> has executed.
    /// </exception>
    public void RegisterStylesheetLink (string key, IResourceUrl url, Priority priority)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);
      ArgumentUtility.CheckNotNull ("url", url);

      RegisterStylesheetLink (key, url.GetUrl(), priority);
    }

    /// <summary> Registers a stylesheet file. </summary>
    /// <remarks>
    ///   All calls to <see cref="RegisterStylesheetLink(string, string, Priority)"/> must be completed before
    ///   <see cref="EnsureAppended"/> is called. (Typically during the <c>Render</c> phase.)
    /// </remarks>
    /// <param name="key"> 
    ///   The unique key identifying the stylesheet file in the headers collection. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="href"> The url of the stylesheet file. Must not be <see langword="null"/> or empty. </param>
    /// <param name="priority"> 
    ///   The priority level of the head element. Elements are rendered in the following order:
    ///   Library, UserControl, Page.
    /// </param>
    /// <exception cref="HttpException"> 
    ///   Thrown if method is called after <see cref="EnsureAppended"/> has executed.
    /// </exception>
    public void RegisterStylesheetLink (string key, string href, Priority priority)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);
      ArgumentUtility.CheckNotNullOrEmpty ("href", href);

      HtmlGenericControl headElement = new HtmlGenericControl ("link");
      headElement.EnableViewState = false;
      headElement.Attributes.Add ("type", "text/css");
      headElement.Attributes.Add ("rel", "stylesheet");
      headElement.Attributes.Add ("href", href);
      RegisterHeadElement (key, headElement, priority);
    }

    /// <summary> Registers a stylesheet file. </summary>
    /// <remarks>
    ///   <para>
    ///     All calls to <see cref="RegisterStylesheetLink(string, string)"/> must be completed before
    ///     <see cref="EnsureAppended"/> is called. (Typically during the <c>Render</c> phase.)
    ///   </para><para>
    ///     Registeres the javascript file with a default priority of Page.
    ///   </para>
    /// </remarks>
    /// <param name="key"> 
    ///   The unique key identifying the stylesheet file in the headers collection. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="url"> 
    /// The url of the stylesheet file. Must not be <see langword="null"/>. 
    /// </param>
    /// <exception cref="HttpException"> 
    ///   Thrown if method is called after <see cref="EnsureAppended"/> has executed.
    /// </exception>
    public void RegisterStylesheetLink (string key, IResourceUrl url)
    {
      RegisterStylesheetLink (key, url, Priority.Page);
    }

    /// <summary> Registers a stylesheet file. </summary>
    /// <remarks>
    ///   <para>
    ///     All calls to <see cref="RegisterStylesheetLink(string, string)"/> must be completed before
    ///     <see cref="EnsureAppended"/> is called. (Typically during the <c>Render</c> phase.)
    ///   </para><para>
    ///     Registeres the javascript file with a default priority of Page.
    ///   </para>
    /// </remarks>
    /// <param name="key"> 
    ///   The unique key identifying the stylesheet file in the headers collection. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="href"> 
    ///   The url of the stylesheet file. Must not be <see langword="null"/> or empty. 
    /// </param>
    /// <exception cref="HttpException"> 
    ///   Thrown if method is called after <see cref="EnsureAppended"/> has executed.
    /// </exception>
    public void RegisterStylesheetLink (string key, string href)
    {
      RegisterStylesheetLink (key, href, Priority.Page);
    }

    /// <summary> Registers a javascript file. </summary>
    /// <remarks>
    ///   <para>
    ///     All calls to <see cref="RegisterJavaScriptInclude"/> must be completed before
    ///     <see cref="EnsureAppended"/> is called. (Typically during the <c>Render</c> phase.)
    ///   </para><para>
    ///     Registeres the javascript file with a default priority of Page.
    ///   </para>
    /// </remarks>
    /// <param name="key">
    ///   The unique key identifying the javascript file in the headers collection. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="url"> 
    ///   The url of the javascript file. Must not be <see langword="null"/>. 
    /// </param>
    /// <exception cref="HttpException"> 
    ///   Thrown if method is called after <see cref="EnsureAppended"/> has executed.
    /// </exception>
    public void RegisterJavaScriptInclude (string key, IResourceUrl url)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);
      ArgumentUtility.CheckNotNull ("url", url);

      RegisterJavaScriptInclude (key, url.GetUrl());
    }

    /// <summary> Registers a javascript file. </summary>
    /// <remarks>
    ///   <para>
    ///     All calls to <see cref="RegisterJavaScriptInclude"/> must be completed before
    ///     <see cref="EnsureAppended"/> is called. (Typically during the <c>Render</c> phase.)
    ///   </para><para>
    ///     Registeres the javascript file with a default priority of Page.
    ///   </para>
    /// </remarks>
    /// <param name="key">
    ///   The unique key identifying the javascript file in the headers collection. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="src"> 
    ///   The url of the javascript file. Must not be <see langword="null"/> or empty. 
    /// </param>
    /// <exception cref="HttpException"> 
    ///   Thrown if method is called after <see cref="EnsureAppended"/> has executed.
    /// </exception>
    public void RegisterJavaScriptInclude (string key, string src)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);
      ArgumentUtility.CheckNotNullOrEmpty ("src", src);

      HtmlGenericControl headElement = new HtmlGenericControl ("script");
      headElement.EnableViewState = false;
      headElement.Attributes.Add ("type", "text/javascript");
      headElement.Attributes.Add ("src", src);
      RegisterHeadElement (key, headElement, Priority.Script);
    }

    public void RegisterUtilitiesJavaScriptInclude ()
    {
      string jqueryKey = typeof (HtmlHeadContents).FullName + "_JQuery";
      var jqueryFileUrl = ResourceUrlFactory.CreateResourceUrl (typeof (HtmlHeadContents), ResourceType.Html, "jquery.js");
      RegisterJavaScriptInclude (jqueryKey, jqueryFileUrl);

      string utilitiesKey = typeof (HtmlHeadContents).FullName + "_Utilities";
      var utilitiesScripFileUrl = ResourceUrlFactory.CreateResourceUrl (typeof (HtmlHeadContents), ResourceType.Html, "Utilities.js");
      RegisterJavaScriptInclude (utilitiesKey, utilitiesScripFileUrl);
    }

    public void RegisterJQueryBgiFramesJavaScriptInclude ()
    {
      string key = typeof (HtmlHeadContents).FullName + "_JQueryBgiFrames";
      var href = ResourceUrlFactory.CreateResourceUrl (typeof (HtmlHeadContents), ResourceType.Html, "jquery.bgiframe.min.js");
      RegisterJavaScriptInclude (key, href);
    }

    /// <summary> Registers a <see cref="Control"/> containing an HTML head element. </summary>
    /// <remarks>
    ///   All calls to <see cref="RegisterHeadElement"/> must be completed before
    ///   <see cref="EnsureAppended"/> is called. (Typically during the <c>Render</c> phase.)
    /// </remarks>
    /// <param name="key"> 
    ///   The unique key identifying the header element in the collection. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="headElement"> 
    ///   The <see cref="Control"/> representing the head element. Must not be <see langword="null"/>. 
    /// </param>
    /// <param name="priority"> 
    ///   The priority level of the head element. Elements are rendered in the following order:
    ///   Library, UserControl, Page.
    /// </param>
    /// <exception cref="HttpException"> 
    ///   Thrown if method is called after <see cref="EnsureAppended"/> has executed.
    /// </exception>
    public void RegisterHeadElement (string key, Control headElement, Priority priority)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);
      ArgumentUtility.CheckNotNull ("headElement", headElement);

      EnsureStateIsClearedAfterServerTransfer();

      if (_hasAppendExecuted)
        throw new HttpException ("RegisterHeadElement must not be called after EnsureAppended has executed.");
      if (! IsRegistered (key))
      {
        _registeredHeadElements.Add (key, headElement);
        GetHeadElements (priority).Add (headElement);
      }
    }

    /// <summary>
    ///   Test's whether an element with this <paramref name="key"/> has already been registered.
    /// </summary>
    /// <param name="key"> The string to test. Must not be <see langword="null"/> or empty. </param>
    /// <returns>
    ///   <see langword="true"/> if an element with this <paramref name="key"/> has already been 
    ///   registered.
    /// </returns>
    public bool IsRegistered (string key)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);

      EnsureStateIsClearedAfterServerTransfer();

      return _registeredHeadElements.Contains (key);
    }

    /// <summary> Gets the list of head elements for the specified <see cref="Priority"/>. </summary>
    /// <param name="priority"> The <see cref="Priority"/> level to get the head elements for. </param>
    /// <returns> An <see cref="ArrayList"/> of the head elements for the specified <see cref="Priority"/>. </returns>
    private ArrayList GetHeadElements (Priority priority)
    {
      ArrayList headElements = (ArrayList) _sortedHeadElements[priority];
      if (headElements == null)
      {
        headElements = new ArrayList();
        _sortedHeadElements[priority] = headElements;
      }
      return headElements;
    }

    //  public void RegisterStylesheetLingForInternetExplorerOnly (string key, string href, Priority priority)
    //  {
    //    ArgumentUtility.CheckNotNullOrEmpty ("key", key);
    //    ArgumentUtility.CheckNotNullOrEmpty ("href", href);
    //
    //    LiteralControl headElement = new LiteralControl();
    //    headElement.EnableViewState = false;
    //    StringBuilder innerHtml = new StringBuilder();
    //    innerHtml.AppendFormat (
    //        "<!--[if IE]><style type=\"text/css\" rel=\"stylesheet\">@import url({0});</style><![endif]-->", href);
    //    headElement.Text = innerHtml.ToString();
    //    RegisterHeadElement (key, headElement, priority);
    //  }

    private void EnsureStateIsClearedAfterServerTransfer ()
    {
      HttpContext context = HttpContext.Current;
      if (context != null)
      {
        var handler = context.Handler;
        if (!ReferenceEquals (handler, _handler.Target))
        {
          _registeredHeadElements.Clear();
          _sortedHeadElements.Clear();
          _hasAppendExecuted = false;
          _handler = new WeakReference (handler);
        }
      }
    }

    private IResourceUrlFactory ResourceUrlFactory
    {
      get { return SafeServiceLocator.Current.GetInstance<IResourceUrlFactory>(); }
    }
  }
}