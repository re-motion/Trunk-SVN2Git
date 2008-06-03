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
using System.Web.UI.HtmlControls;
using Remotion.Web.Configuration;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI
{

public interface IWindowStateManager
{
  object GetData (string key);
  void SetData (string key, object value);
}

/// <summary> Specifies the client side events supported for registration by the <see cref="ISmartPage"/>. </summary>
public enum SmartPageEvents
{
  /// <summary> Rasied when the document has finished loading. Signature: <c>void Function (hasSubmitted, isCached)</c> </summary>
  OnLoad,
  /// <summary> Raised when the user posts back to the server. Signature: <c>void Function (eventTargetID, eventArgs)</c> </summary>
  OnPostBack,
  /// <summary> Raised when the user leaves the page. Signature: <c>void Function (hasSubmitted, isCached)</c> </summary>
  OnAbort,
  /// <summary> Raised when the user scrolls the page. Signature: <c>void Function ()</c> </summary>
  OnScroll,
  /// <summary> Raised when the user resizes the page. Signature: <c>void Function ()</c> </summary>
  OnResize,
  /// <summary> 
  ///   Raised before the request to load a new page (or reload the current page) is executed. Not supported in Opera.
  ///   Signature: <c>void Function ()</c>
  /// </summary>
  OnBeforeUnload,
  /// <summary> Raised before the page is removed from the window. Signature: <c>void Function ()</c> </summary>
  OnUnload
}

/// <summary>
///   This interface represents a page that has a dirty-state and can prevent multiple postbacks.
/// </summary>
/// <include file='doc\include\UI\ISmartPage.xml' path='ISmartPage/Class/*' />
public interface ISmartPage: IPage
{
  /// <summary> Gets the post back data for the page. </summary>
  NameValueCollection GetPostBackCollection ();

  /// <summary>
  ///   Registers a control implementing <see cref="IEditableControl"/> for tracking of it's server- and client-side
  ///   dirty state.
  /// </summary>
  /// <param name="control"> A control implementing <see cref="IEditableControl"/> that will be tracked. </param>
  void RegisterControlForDirtyStateTracking (IEditableControl control);

  /// <summary>
  ///   Resgisters a <see cref="Control.ClientID"/> for the tracking of the controls client-side dirty state.
  /// </summary>
  /// <param name="clientID"> The ID of an HTML input/textarea/select element. </param>
  void RegisterControlForClientSideDirtyStateTracking (string clientID);

  /// <summary> 
  ///   Evaluates whether any control regsitered using <see cref="RegisterControlForDirtyStateTracking"/>
  ///   has values that must be persisted before the user leaves the page. 
  /// </summary>
  /// <returns> <see langword="true"/> if the page is dirty (i.e. has unpersisted changes). </returns>
  bool EvaluateDirtyState();

  /// <summary>
  ///   Gets a flag that determines whether the dirty state will be taken into account when displaying the abort 
  ///   confirmation dialog.
  /// </summary>
  /// <value> 
  ///   <see langword="true"/> to invoke <see cref="EvaluateDirtyState"/> and track changes on the client. 
  /// </value>
  bool IsDirtyStateTrackingEnabled { get; }

  /// <summary>
  ///   Gets or sets a flag that determines whether to display a confirmation dialog before leaving the page. 
  ///  </summary>
  /// <value> <see langword="true"/> to display the confirmation dialog. </value>
  /// <remarks> 
  ///   If <see cref="IsDirtyStateTrackingEnabled"/> evaluates <see langword="true"/>, a confirmation will only be 
  ///   displayed if the page is dirty.
  /// </remarks>
  bool IsAbortConfirmationEnabled { get; }

  /// <summary> Gets the message displayed when the user attempts to abort the WXE Function. </summary>
  /// <remarks> 
  ///   In case of an empty <see cref="String"/>, the text is read from the resources for <see cref="SmartPageInfo"/>. 
  /// </remarks>
  string AbortMessage { get; }

  /// <summary> Gets the message displayed when the user attempts to submit while the page is already submitting. </summary>
  /// <remarks> 
  ///   In case of an empty <see cref="String"/>, the text is read from the resources for <see cref="SmartPageInfo"/>. 
  /// </remarks>
  string StatusIsSubmittingMessage { get; }

  /// <summary> 
  ///   Gets a flag whether the is submitting status messages will be displayed when the user tries to postback while 
  ///   a request is being processed.
  /// </summary>
  bool IsStatusIsSubmittingMessageEnabled { get; }

  /// <summary> 
  ///   Registers Java Script functions to be executed when the respective <paramref name="pageEvent"/> is raised.
  /// </summary>
  /// <include file='doc\include\UI\ISmartPage.xml' path='ISmartPage/RegisterClientSidePageEventHandler/*' />
  void RegisterClientSidePageEventHandler (SmartPageEvents pageEvent, string key, string function);

  /// <summary>
  ///   Regisiters a Java Script function used to evaluate whether to continue with the submit.
  ///   Signature: <c>bool Function (isAborting, hasSubmitted, hasUnloaded, isCached)</c>
  /// </summary>
  string CheckFormStateFunction { get; set; }

  void RegisterCommandForSynchronousPostBack (Control control, string eventArguments);

  /// <summary> Gets or sets the <see cref="HtmlForm"/> of the ASP.NET page. </summary>
  [EditorBrowsable (EditorBrowsableState.Never)]
  HtmlForm HtmlForm { get; set; }
}

/// <summary> Defines the options for showing the abort confirmation dialog upon leaving the page. </summary>
public enum ShowAbortConfirmation
{
  /// <summary> Disables the abort confirmation dialog. </summary>
  Never,
  /// <summary> Always displays an abort confirmation dialog before leaving the page. </summary>
  Always,
  /// <summary> Only displays an abort confirmation dialog when the page is dirty. </summary>
  OnlyIfDirty
}

/// <summary>
///   <b>SmartPage</b> is the default implementation of the <see cref="ISmartPage"/> interface. Use this type
///   a base class for pages that should supress multiple postbacks, require smart navigation, or have a dirty-state.
/// </summary>
/// <include file='doc\include\UI\SmartPage.xml' path='SmartPage/Class/*' />
public class SmartPage: Page, ISmartPage, ISmartNavigablePage
{
  #region ISmartPage Implementation

  /// <summary> 
  ///   Registers Java Script functions to be executed when the respective <paramref name="pageEvent"/> is raised.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/RegisterClientSidePageEventHandler/*' />
  public void RegisterClientSidePageEventHandler (SmartPageEvents pageEvent, string key, string function)
  {
    _smartPageInfo.RegisterClientSidePageEventHandler (pageEvent, key, function);
  }


  string ISmartPage.CheckFormStateFunction
  {
    get { return _smartPageInfo.CheckFormStateFunction; }
    set { _smartPageInfo.CheckFormStateFunction = value; }
  }

  /// <summary> Gets or sets the message displayed when the user attempts to leave the page. </summary>
  /// <remarks> 
  ///   In case of an empty <see cref="String"/>, the text is read from the resources for <see cref="SmartPageInfo"/>. 
  /// </remarks>
  [Description("The message displayed when the user attempts to leave the page.")]
  [Category ("Appearance")]
  [DefaultValue ("")]
  public virtual string AbortMessage 
  {
    get { return _smartPageInfo.AbortMessage; }
    set { _smartPageInfo.AbortMessage = value; }
  }

  /// <summary> 
  ///   Gets or sets the message displayed when the user attempts to submit while the page is already submitting. 
  /// </summary>
  /// <remarks> 
  ///   In case of an empty <see cref="String"/>, the text is read from the resources for <see cref="SmartPageInfo"/>. 
  /// </remarks>
  [Description("The message displayed when the user attempts to submit while the page is already submitting.")]
  [Category ("Appearance")]
  [DefaultValue ("")]
  public virtual string StatusIsSubmittingMessage
  {
    get { return _smartPageInfo.StatusIsSubmittingMessage; }
    set { _smartPageInfo.StatusIsSubmittingMessage = value; }
  }

  /// <summary>
  ///   Registers a control implementing <see cref="IEditableControl"/> for tracking of it's server- and client-side
  ///   dirty state.
  /// </summary>
  /// <param name="control"> A control implementing <see cref="IEditableControl"/> that will be tracked.  </param>
  public void RegisterControlForDirtyStateTracking (IEditableControl control)
  {
    _smartPageInfo.RegisterControlForDirtyStateTracking (control);
  }

  /// <summary>
  ///   Resiters a <see cref="Control.ClientID"/> for the tracking of the controls client-side dirty state.
  /// </summary>
  /// <param name="clientID"> The ID of an HTML input/textarea/select element. </param>
  public void RegisterControlForClientSideDirtyStateTracking (string clientID)
  {
    _smartPageInfo.RegisterControlForDirtyStateTracking (clientID);
  }

  public void RegisterCommandForSynchronousPostBack (Control control, string eventArguments)
  {
    _smartPageInfo.RegisterCommandForSynchronousPostBack (control, eventArguments);
  }

  #endregion

  #region ISmartNavigablePage Implementation

  /// <summary> Clears scrolling and focus information on the page. </summary>
  public void DiscardSmartNavigationData ()
  {
    _smartPageInfo.DiscardSmartNavigationData();
  }

  /// <summary> Sets the focus to the passed control. </summary>
  /// <param name="control"> 
  ///   The <see cref="IFocusableControl"/> to assign the focus to. Must no be <see langword="null"/>.
  /// </param>
  public void SetFocus (IFocusableControl control)
  {
    _smartPageInfo.SetFocus (control);
  }

  /// <summary> Sets the focus to the passed control ID. </summary>
  /// <param name="id"> 
  ///   The client side ID of the control to assign the focus to. Must no be <see langword="null"/> or empty. 
  /// </param>
  public new void SetFocus (string id)
  {
    _smartPageInfo.SetFocus (id);
  }

  /// <summary> Registers a <see cref="INavigationControl"/> with the <see cref="ISmartNavigablePage"/>. </summary>
  /// <param name="control"> The <see cref="INavigationControl"/> to register. Must not be <see langword="null"/>. </param>
  public void RegisterNavigationControl (INavigationControl control)
  {
    _smartPageInfo.RegisterNavigationControl (control);
  }

  /// <summary> 
  ///   Appends the URL parameters returned by <see cref="GetNavigationUrlParameters"/> to the <paramref name="url"/>.
  /// </summary>
  /// <param name="url"> A URL or a query string. Must not be <see langword="null"/>. </param>
  /// <returns> 
  ///   The <paramref name="url"/> appended with the URL parameters returned by 
  ///   <see cref="GetNavigationUrlParameters"/>. 
  /// </returns>
  public string AppendNavigationUrlParameters (string url)
  {
    return _smartPageInfo.AppendNavigationUrlParameters (url);
  }

  /// <summary> 
  ///   Evaluates the <see cref="INavigationControl.GetNavigationUrlParameters"/> methods of all controls registered
  ///   using <see cref="RegisterNavigationControl"/>.
  /// </summary>
  /// <returns>
  ///   A <see cref="NameValueCollection"/> containing the URL parameters required by this 
  ///   <see cref="ISmartNavigablePage"/> to restore its navigation state when using hyperlinks.
  /// </returns>
  public NameValueCollection GetNavigationUrlParameters()
  {
    return _smartPageInfo.GetNavigationUrlParameters();
  }

  #endregion

  private SmartPageInfo _smartPageInfo;
  private ValidatableControlInitializer _validatableControlInitializer;
  private PostLoadInvoker _postLoadInvoker;
  private bool _isDirty = false;
  private ShowAbortConfirmation _showAbortConfirmation = ShowAbortConfirmation.OnlyIfDirty;
  private bool? _enableStatusIsSubmittingMessage;
  private bool? _enableSmartScrolling;
  private bool? _enableSmartFocusing;

  public SmartPage()
  {
    _smartPageInfo = new SmartPageInfo (this);
    _validatableControlInitializer = new ValidatableControlInitializer (this);
    _postLoadInvoker = new PostLoadInvoker (this);
  }


  protected override NameValueCollection DeterminePostBackMode()
  {
    NameValueCollection result = base.DeterminePostBackMode();
    return result;
  }

  /// <summary> Gets the post back data for the page. </summary>
  NameValueCollection ISmartPage.GetPostBackCollection ()
  {
    if (string.Compare (Request.HttpMethod, "POST", true) == 0)
      return Request.Form;
    else
      return Request.QueryString;
  }


  /// <summary> Gets or sets the <b>HtmlForm</b> of this page. </summary>
  /// <remarks> Redirects the call to the <see cref="HtmlForm"/> property. </remarks>
  HtmlForm ISmartPage.HtmlForm
  {
    get { return HtmlForm; }
    set { HtmlForm = value; }
  }

  /// <summary> Gets or sets the <b>HtmlForm</b> of this page. </summary>
  /// <remarks>
  ///   <note type="inheritinfo"> 
  ///     Override this property you do not wish to rely on automatic detection of the <see cref="HtmlForm"/>
  ///     using reflection.
  ///   </note>
  /// </remarks>
  [EditorBrowsable (EditorBrowsableState.Never)]
  protected virtual HtmlForm HtmlForm
  {
    get { return _smartPageInfo.HtmlForm; }
    set { _smartPageInfo.HtmlForm = value; }
  }


  /// <summary>
  ///   Call this method before validating when using <see cref="Remotion.Web.UI.Controls.FormGridManager"/> 
  ///   and <see cref="M:Remotion.ObjectBinding.Web.UI.Controls.IBusinessObjectDataSourceControl.Validate()"/>.
  /// </summary>
  public void PrepareValidation()
  {
    EnsurePostLoadInvoked();
    EnsureValidatableControlsInitialized();
  }

  /// <summary> Ensures that PostLoad is called on all controls that support <see cref="ISupportsPostLoadControl"/>. </summary>
  public void EnsurePostLoadInvoked ()
  {
    _postLoadInvoker.EnsurePostLoadInvoked();
  }

  /// <summary> Ensures that all validators are registered with their <see cref="IValidatableControl"/> controls. </summary>
  public void EnsureValidatableControlsInitialized ()
  {
    _validatableControlInitializer.EnsureValidatableControlsInitialized ();
  }



  /// <summary> Gets or sets a flag describing whether the page is dirty. </summary>
  /// <value> <see langword="true"/> if the page requires saving. Defaults to <see langword="false"/>.  </value>
  public bool IsDirty
  {
    get { return _isDirty; }
    set { _isDirty = value; }
  }

  /// <summary> 
  ///   Evaluates whether any control regsitered using <see cref="RegisterControlForDirtyStateTracking"/>
  ///   has values that must be persisted before the user leaves the page. 
  /// </summary>
  /// <value> The value returned by <see cref="IsDirty"/>. </value>
  public virtual bool EvaluateDirtyState()
  {
    if (_isDirty)
      return true;
    return _smartPageInfo.EvaluateDirtyState();
  }

  /// <summary> Gets a flag whether to only show the abort confirmation if the page is dirty. </summary>
  /// <value> 
  ///   <see langword="true"/> if <see cref="ShowAbortConfirmation"/> is set to 
  ///   <see cref="F:ShowAbortConfirmation.OnlyIfDirty"/>. 
  /// </value>
  protected virtual bool IsDirtyStateTrackingEnabled
  {
    get { return ShowAbortConfirmation == ShowAbortConfirmation.OnlyIfDirty; }
  }

  /// <summary> Gets the value returned by <see cref="IsDirtyStateTrackingEnabled"/>. </summary>
  bool ISmartPage.IsDirtyStateTrackingEnabled
  {
    get { return IsDirtyStateTrackingEnabled; }
  }

  /// <summary> 
  ///   Gets or sets a value that determines whether to display a confirmation dialog before leaving the page. 
  /// </summary>
  /// <value> 
  ///   <see cref="F:ShowAbortConfirmation.Always"/> to always display a confirmation dialog before leaving the page. 
  ///   <see cref="F:ShowAbortConfirmation.OnlyIfDirty"/> to display a confirmation dialog only when the page is dirty. 
  ///   <see cref="F:ShowAbortConfirmation.Never"/> to disable the confirmation dialog. 
  ///   Defaults to <see cref="F:ShowAbortConfirmation.OnlyIfDirty"/>.
  /// </value>
  [Description("Determines whether to display a confirmation dialog before leaving the page.")]
  [Category ("Behavior")]
  [DefaultValue (ShowAbortConfirmation.OnlyIfDirty)]
  public virtual ShowAbortConfirmation ShowAbortConfirmation
  {
    get { return _showAbortConfirmation; }
    set { _showAbortConfirmation = value; }
  }

  /// <summary> Gets the evaluated value for the <see cref="ShowAbortConfirmation"/> property. </summary>
  /// <value> 
  ///   <see langword="true"/> if <see cref="ShowAbortConfirmation"/> is set to
  ///   <see cref="F:ShowAbortConfirmation.Always"/> or <see cref="F:ShowAbortConfirmation.OnlyIfDirty"/>. 
  /// </value>
  /// <remarks> 
  ///   If <see cref="IsDirtyStateTrackingEnabled"/> evaluates <see langword="true"/>, a confirmation will only be 
  ///   displayed if the page is dirty.
  /// </remarks>
  protected virtual bool IsAbortConfirmationEnabled
  {
    get
    {
      return   ShowAbortConfirmation == ShowAbortConfirmation.Always
            || ShowAbortConfirmation == ShowAbortConfirmation.OnlyIfDirty;
    }
  }

  /// <summary> Gets the value returned by <see cref="IsAbortConfirmationEnabled"/>. </summary>
  bool ISmartPage.IsAbortConfirmationEnabled
  {
    get { return IsAbortConfirmationEnabled; }
  }


  /// <summary> 
  ///   Gets or sets the flag that determines whether to display a message when the user tries to start a second
  ///   request.
  /// </summary>
  /// <value> 
  ///   <see langword="true"/> to enable the status messages. Defaults to <see langword="null"/>, which is interpreted as <see langword="true"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="IsStatusIsSubmittingMessageEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to display a status message when the user attempts to start a "
             + "second request. Undefined is interpreted as true.")]
  [Category ("Behavior")]
  [DefaultValue (null)]
  public virtual bool? EnableStatusIsSubmittingMessage
  {
    get { return _enableStatusIsSubmittingMessage; }
    set { _enableStatusIsSubmittingMessage = value; }
  }

  /// <summary> 
  ///   Gets a flag whether a status message  will be displayed when the user tries to postback while a request is 
  ///   being processed.
  /// </summary>
  protected virtual bool IsStatusIsSubmittingMessageEnabled
  {
    get { return _enableStatusIsSubmittingMessage != false; }
  }

  /// <summary> Gets the value returned by <see cref="IsStatusIsSubmittingMessageEnabled"/>. </summary>
  bool ISmartPage.IsStatusIsSubmittingMessageEnabled
  {
    get { return IsStatusIsSubmittingMessageEnabled; }
  }


  /// <summary> Gets or sets the flag that determines whether to use smart scrolling. </summary>
  /// <value> 
  ///   <see langword="true"/> to use smart scrolling. Defaults to <see langword="null"/>, which is interpreted as <see langword="true"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="IsSmartScrollingEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to use smart scrolling. Undefined is interpreted as true.")]
  [Category ("Behavior")]
  [DefaultValue (null)]
  public virtual bool? EnableSmartScrolling
  {
    get { return _enableSmartScrolling; }
    set { _enableSmartScrolling = value; }
  }

  /// <summary> Gets the evaluated value for the <see cref="EnableSmartScrolling"/> property. </summary>
  /// <value> 
  ///   <see langword="false"/> if <see cref="EnableSmartScrolling"/> is <see langword="false"/>
  ///   or the <see cref="SmartNavigationConfiguration.EnableScrolling"/> configuration setting is 
  ///   <see langword="false"/>.
  /// </value>
  protected virtual bool IsSmartScrollingEnabled
  {
    get
    {
      if (! WebConfiguration.Current.SmartNavigation.EnableScrolling)
        return false;
      return _enableSmartScrolling != false; 
    }
  }

  /// <summary> Gets the value returned by <see cref="IsSmartScrollingEnabled"/>. </summary>
  bool ISmartNavigablePage.IsSmartScrollingEnabled
  {
    get { return IsSmartScrollingEnabled; }
  }


  /// <summary> Gets or sets the flag that determines whether to use smart navigation. </summary>
  /// <value> 
  ///   <see langword="true"/> to use smart navigation. 
  ///   Defaults to <see langword="null"/>, which is interpreted as <see langword="true"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="IsSmartFocusingEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to use smart navigation. Undefined is interpreted as true.")]
  [Category ("Behavior")]
  [DefaultValue (null)]
  public virtual bool? EnableSmartFocusing
  {
    get { return _enableSmartFocusing; }
    set { _enableSmartFocusing = value; }
  }

  /// <summary> Gets the evaluated value for the <see cref="EnableSmartFocusing"/> property. </summary>
  /// <value> 
  ///   <see langword="false"/> if <see cref="EnableSmartFocusing"/> is <see langword="false"/>
  ///   or the <see cref="SmartNavigationConfiguration.EnableFocusing"/> configuration setting is 
  ///   <see langword="false"/>.
  /// </value>
  protected virtual bool IsSmartFocusingEnabled
  {
    get
    {
      if (! WebConfiguration.Current.SmartNavigation.EnableFocusing)
        return false;
      return _enableSmartFocusing != false; 
    }
  }

  /// <summary> Gets the value returned by <see cref="IsSmartFocusingEnabled"/>. </summary>
  bool ISmartNavigablePage.IsSmartFocusingEnabled
  {
    get { return IsSmartFocusingEnabled; }
  }

  protected override void OnInit (EventArgs e)
  {
    base.OnInit (e);
    if (!ControlHelper.IsDesignMode (this, Context))
    {
      RegisterRequiresControlState (this);
    }
  }

  protected override void LoadControlState(object savedState)
  {
    object[] values = (object[]) savedState;
    base.LoadControlState (values[0]);
    _isDirty = (bool)  values[1];
  }

  protected override object SaveControlState()
  {
    object[] values = new object[2];
    values[0] = base.SaveControlState();
    values[1] = _isDirty;
    return values;
  }

  protected override void OnPreRenderComplete (EventArgs e)
  {
    _smartPageInfo.OnPreRenderComplete ();
    base.OnPreRenderComplete (e);
  }
}

}
