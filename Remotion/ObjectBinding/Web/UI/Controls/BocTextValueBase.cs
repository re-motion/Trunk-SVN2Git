// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  /// Abstract base class for <see cref="BocTextValue"/> and <see cref="BocMultilineTextValue"/>, both of which handle text input.
  /// </summary>
  [ControlValueProperty ("Text")]
  [DefaultProperty ("Text")]
  [ValidationProperty ("Text")]
  [ParseChildren (true, "Text")]
  [DefaultEvent ("TextChanged")]
  [ToolboxItemFilter ("System.Web.UI")]
  public abstract class BocTextValueBase : BusinessObjectBoundEditableWebControl, IBocTextValueBase, IPostBackDataHandler, IFocusableControl
  {
    private const string c_textboxIDPostfix = "_Boc_TextBox";
    private readonly Style _commonStyle = new Style();
    private readonly TextBoxStyle _textBoxStyle;
    private readonly Style _labelStyle = new Style();
    private string _errorMessage;
    private readonly List<BaseValidator> _validators = new List<BaseValidator>();
    private static readonly object s_textChangedEvent = new object();

    protected BocTextValueBase ()
        : this (TextBoxMode.SingleLine)
    {
    }

    protected BocTextValueBase (TextBoxMode mode)
    {
      _textBoxStyle = new TextBoxStyle (mode);
    }

    /// <summary>
    /// Contains the types of properties supported by the control. A property assigned to <see cref="BusinessObjectBoundWebControl.Property"/>
    /// but be assignable to one of the contained interfaces.
    /// </summary>
    /// <value>An array of types, containing interfaces derived from <see cref="IBusinessObjectProperty"/>.</value>
    protected abstract override Type[] SupportedPropertyInterfaces { get; }

    /// <summary>
    ///   Gets a flag that determines whether it is valid to generate HTML &lt;label&gt; tags referencing the
    ///   <see cref="TargetControl"/>.
    /// </summary>
    /// <value> Returns always <see langword="true"/>. </value>
    public override bool UseLabel
    {
      get { return true; }
    }

    /// <summary>
    ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its 
    ///   <see cref="Control.ClientID"/>.
    /// </summary>
    /// <value> The control itself. </value>
    public override Control TargetControl
    {
      get { return this; }
    }

    /// <summary> Gets the ID of the element to receive the focus when the page is loaded. </summary>
    /// <value>
    ///   Returns the <see cref="Control.ClientID"/> of the <see cref="TextBox"/> if the control is in edit mode, 
    ///   otherwise <see langword="null"/>. 
    /// </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public string FocusID
    {
      get { return IsReadOnly ? null : GetTextBoxClientID(); }
    }

    /// <summary>
    ///   Gets the style that you want to apply to the <see cref="TextBox"/> (edit mode) 
    ///   and the <see cref="Label"/> (read-only mode).
    /// </summary>
    /// <value>The <see cref="Style"/> object applied to both edit and read-only mode UI.</value>
    /// <remarks>
    ///   Use the <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> to assign individual style settings for
    ///   the respective modes. Note that if you set one of the <b>Font</b> attributes (Bold, Italic etc.) to 
    ///   <see langword="true"/>, this cannot be overridden using <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> 
    ///   properties.
    /// </remarks>
    [Category ("Style")]
    [Description ("The style that you want to apply to the TextBox (edit mode) and the Label (read-only mode).")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public Style CommonStyle
    {
      get { return _commonStyle; }
    }

    /// <summary> Gets the style that you want to apply to the <see cref="TextBox"/> (edit mode) only. </summary>
    /// <value>The <see cref="Style"/> object applied to edit mode UI.</value>
    /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
    [Category ("Style")]
    [Description ("The style that you want to apply to the TextBox (edit mode) only.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public TextBoxStyle TextBoxStyle
    {
      get { return _textBoxStyle; }
    }

    /// <summary> Gets the style that you want to apply to the <see cref="Label"/> (read-only mode) only. </summary>
    /// <value>The <see cref="Style"/> object applied read-only mode UI.</value>
    /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
    [Category ("Style")]
    [Description ("The style that you want to apply to the Label (read-only mode) only.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public Style LabelStyle
    {
      get { return _labelStyle; }
    }

    /// <summary> Gets or sets the validation error message. </summary>
    /// <value> 
    ///   The error message displayed when validation fails. The default value is an empty <see cref="String"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description ("Validation message displayed if there is an error.")]
    [Category ("Validator")]
    [DefaultValue ("")]
    public string ErrorMessage
    {
      get { return _errorMessage; }
      set
      {
        _errorMessage = value;
        for (int i = 0; i < _validators.Count; i++)
        {
          BaseValidator validator = _validators[i];
          validator.ErrorMessage = _errorMessage;
        }
      }
    }

    /// <summary> Gets or sets the string representation of the current value. </summary>
    /// <value>The text contents of the input field in edit mode and the displayed text in read-only mode.</value>
    /// <remarks> Uses <c>\r\n</c> or <c>\n</c> as separation characters. The default value is an empty <see cref="String"/>. </remarks>
    [Description ("The string representation of the current value.")]
    [Category ("Data")]
    [DefaultValue ("")]
    public abstract string Text { get; set; }

    /// <summary>
    /// Registers the control as requiring a postback.
    /// </summary>
    /// <param name="e">ignored</param>
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      if (!IsDesignMode)
        Page.RegisterRequiresPostBack (this);
    }

    public override void RegisterHtmlHeadContents (IHttpContext httpContext, HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("httpContext", httpContext);
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      base.RegisterHtmlHeadContents (httpContext, htmlHeadAppender);

      EnsureChildControls();
    }

    /// <summary> Invokes the <see cref="LoadPostData"/> method. </summary>
    bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      if (RequiresLoadPostData)
        return LoadPostData (postDataKey, postCollection);
      else
        return false;
    }

    /// <summary> Invokes the <see cref="RaisePostDataChangedEvent"/> method. </summary>
    void IPostBackDataHandler.RaisePostDataChangedEvent ()
    {
      RaisePostDataChangedEvent();
    }

    /// <summary>
    ///   Uses the <paramref name="postCollection"/> to determine whether the value of this control has been changed 
    ///   between postbacks.
    /// </summary>
    /// <include file='doc\include\UI\Controls\BocTextValue.xml' path='BocTextValue/LoadPostData/*' />
    protected virtual bool LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      string newValue = PageUtility.GetPostBackCollectionItem (Page, GetTextBoxUniqueID());
      bool isDataChanged = newValue != null && StringUtility.NullToEmpty (Text) != newValue;
      if (isDataChanged)
      {
        Text = newValue;
        IsDirty = true;
      }
      return isDataChanged;
    }

    /// <summary> Called when the state of the control has changed between postbacks. </summary>
    protected virtual void RaisePostDataChangedEvent ()
    {
      if (! IsReadOnly && Enabled)
        OnTextChanged();
    }

    /// <summary> Fires the <see cref="TextChanged"/> event. </summary>
    protected virtual void OnTextChanged ()
    {
      EventHandler eventHandler = (EventHandler) Events[s_textChangedEvent];
      if (eventHandler != null)
        eventHandler (this, EventArgs.Empty);
    }

    public abstract override void RenderControl (HtmlTextWriter writer);

    /// <summary> Checks whether the control conforms to the required WAI level. </summary>
    /// <exception cref="Remotion.Web.UI.WcagException"> Thrown if the control does not conform to the required WAI level. </exception>
    protected virtual void EvaluateWaiConformity ()
    {
      if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
      {
        if (TextBoxStyle.AutoPostBack == true)
          WcagHelper.Instance.HandleWarning (1, this, "TextBoxStyle.AutoPostBack");
      }
    }

    /// <summary> Loads the resources into the control's properties. </summary>
    protected override void LoadResources (IResourceManager resourceManager)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);

      if (IsDesignMode)
        return;
      base.LoadResources (resourceManager);

      //  Dispatch simple properties
      string key = ResourceManagerUtility.GetGlobalResourceKey (ErrorMessage);
      if (! StringUtility.IsNullOrEmpty (key))
        ErrorMessage = resourceManager.GetString (key);
    }

    /// <summary> 
    ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user 
    ///   interface.
    /// </summary>
    /// <returns> 
    ///   A <see cref="String"/> <see cref="Array"/> containing the <see cref="Control.ClientID"/> of the
    ///   <see cref="TextBox"/> if the control is in edit mode, or an empty array if the control is read-only.
    /// </returns>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.GetTrackedClientIDs">BusinessObjectBoundEditableWebControl.GetTrackedClientIDs</seealso>
    public override string[] GetTrackedClientIDs ()
    {
      return IsReadOnly ? new string[0] : new[] { GetTextBoxClientID() };
    }

    /// <summary> This event is fired when the text is changed between postbacks. </summary>
    [Category ("Action")]
    [Description ("Fires when the value of the control has changed.")]
    public event EventHandler TextChanged
    {
      add { Events.AddHandler (s_textChangedEvent, value); }
      remove { Events.RemoveHandler (s_textChangedEvent, value); }
    }

    /// <summary> Creates the list of validators required for the current binding and property settings. </summary>
    public override BaseValidator[] CreateValidators ()
    {
      if (IsReadOnly)
        return new BaseValidator[0];

      _validators.AddRange (GetValidators());
      return _validators.ToArray();
    }

    bool IBocRenderableControl.IsDesignMode
    {
      get { return IsDesignMode; }
    }

    /// <summary>
    /// Creates and configures the validators that validate the control's text value.
    /// </summary>
    /// <returns>An enumeration of validators to use for checking whether the control's input is valid.</returns>
    protected abstract IEnumerable<BaseValidator> GetValidators ();

    /// <summary>
    /// Loads the resources used by the control. <seealso cref="GetResourceManager"/>
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreRender (EventArgs e)
    {
      EnsureChildControls();
      base.OnPreRender (e);

      LoadResources (GetResourceManager());

      EvaluateWaiConformity();
    }

    /// <summary> Returns the <see cref="IResourceManager"/> used to access the resources for this control. </summary>
    protected abstract IResourceManager GetResourceManager ();

    /// <summary>
    /// Returns the ID to use for the input field in the rendered HTML.
    /// </summary>
    /// <returns>The control's <see cref="Control.ClientID"/> postfixed by a constant textbox id.</returns>
    public string GetTextBoxClientID ()
    {
      return ClientID + c_textboxIDPostfix;
    }

    private string GetTextBoxUniqueID ()
    {
      return UniqueID + c_textboxIDPostfix;
    }

    string IBocTextValueBase.TextBoxID
    {
      get { return GetTextBoxUniqueID(); }
    }
  }
}