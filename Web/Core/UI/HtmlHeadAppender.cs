using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
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

  private static HtmlHeadAppender s_designModeCurrent;

  /// <summary> ListDictionary&lt;string key, Control headElement&gt; </summary>
  private ListDictionary _registeredHeadElements = new ListDictionary();
  /// <summary> SortedList&lt;Priority (int) priority, ArrayList headElements &gt; </summary>
  private SortedList _sortedHeadElements = new SortedList();
  /// <summary> <see langword="true"/> if <see cref="EnsureAppended"/> has already executed. </summary>
  private bool _hasAppendExecuted = false;
  
  /// <remarks>
  ///   Factory pattern. No public construction.
  /// </remarks>
  /// <exclude/>
  private HtmlHeadAppender()
  {
  }

  /// <summary>
  ///   Gets the <see cref="HtmlHeadAppender"/> instance.
  /// </summary>
  public static HtmlHeadAppender Current
  {
    get 
    { 
      if (HtmlHeadContents.IsDesignMode)
        return GetDesignModeCurrent();
      else
        return GetCurrent();
    }
  }

  private static HtmlHeadAppender GetCurrent()
  {
    HtmlHeadAppender current = (HtmlHeadAppender) CallContext.GetData (c_contextKey);
    
    if (current == null)
    {
      lock (typeof (HtmlHeadAppender))
      {
        current = (HtmlHeadAppender) CallContext.GetData (c_contextKey);
        if (current == null)
        {
          current = new HtmlHeadAppender();
          CallContext.SetData (c_contextKey, current);
        }
      }
    }
    return current;
  }

  private static HtmlHeadAppender GetDesignModeCurrent()
  {
    if (s_designModeCurrent == null)
      s_designModeCurrent = new HtmlHeadAppender();
    return s_designModeCurrent;
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
    {
      _hasAppendExecuted = true;
    }
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
  ///     All calls to <see cref="RegisterStylesheetLink"/> must be completed before
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
    string key = "title";

    if (IsRegistered (key))
    {
      ((HtmlGenericControl) _registeredHeadElements[key]).InnerText = title;
    }
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
  ///   All calls to <see cref="RegisterStylesheetLink"/> must be completed before
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
  ///     All calls to <see cref="RegisterStylesheetLink"/> must be completed before
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

  public void RegisterUtilitiesJavaScriptInclude (Control control)
  {
    ArgumentUtility.CheckNotNull ("control", control);
    string key = typeof (HtmlHeadContents).FullName + "_Utilities";
    if (! IsRegistered (key))
    {
      string href = ResourceUrlResolver.GetResourceUrl (control, typeof (HtmlHeadContents), ResourceType.Html, "Utilities.js");
      RegisterJavaScriptInclude (key, href);
    }
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
}

}
