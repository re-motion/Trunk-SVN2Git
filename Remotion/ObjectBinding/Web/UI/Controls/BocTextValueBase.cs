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
  [ControlValueProperty("Text")]
  [DefaultProperty("Text")]
  [ValidationProperty ("Text")]
  [ParseChildren(true, "Text")]
  [DefaultEvent ("TextChanged")]
  [ToolboxItemFilter ("System.Web.UI")]
  public abstract class BocTextValueBase : BusinessObjectBoundEditableWebControl, IBocTextValueBase, IPostBackDataHandler, IFocusableControl
  {
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
    ///   The <see cref="BocTextValue"/> supports properties of types <see cref="IBusinessObjectStringProperty"/>,
    ///   <see cref="IBusinessObjectDateTimeProperty"/>, and <see cref="IBusinessObjectNumericProperty"/>.
    /// </summary>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
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
    /// <value> Returns the <see cref="TextBox"/> if the control is in edit mode, otherwise the control itself. </value>
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
      get { return IsReadOnly ? null : ClientID; }
    }

    [Browsable(false)]
    public bool AutoPostBack { get; set; }

    /// <summary>
    ///   Gets the style that you want to apply to the <see cref="TextBox"/> (edit mode) 
    ///   and the <see cref="Label"/> (read-only mode).
    /// </summary>
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

    /// <summary> Gets the CSS-Class applied to the <see cref="BocTextValue"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>bocTextValue</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    protected abstract string CssClassBase { get; }

    string IBocRenderableControl.CssClassBase { get { return CssClassBase; } }

      /// <summary> Gets the CSS-Class applied to the <see cref="BocTextValue"/> when it is displayed in read-only mode. </summary>
    /// <remarks> 
    ///   <para> Class: <c>readOnly</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocTextValue.readOnly</c> as a selector. </para>
    /// </remarks>
    protected virtual string CssClassReadOnly
    {
      get { return "readOnly"; }
    }

    string IBocRenderableControl.CssClassReadOnly { get { return CssClassReadOnly; } }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocTextValue"/> when it is displayed disabled. </summary>
    /// <remarks> 
    ///   <para> Class: <c>disabled</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocTextValue.disabled</c> as a selector.</para>
    /// </remarks>
    protected virtual string CssClassDisabled
    {
      get { return "disabled"; }
    }

    string IBocRenderableControl.CssClassDisabled { get { return CssClassDisabled; } }

    /// <summary> Gets or sets the string representation of the current value. </summary>
    /// <remarks> Uses <c>\r\n</c> or <c>\n</c> as separation characters. The default value is an empty <see cref="String"/>. </remarks>
    [Description ("The string representation of the current value.")]
    [Category ("Data")]
    [DefaultValue ("")]
    public abstract string Text { get; set; }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      if (!IsDesignMode)
        Page.RegisterRequiresPostBack (this);
    }

    public override void RegisterHtmlHeadContents (HttpContext context)
    {
      base.RegisterHtmlHeadContents (context);

      EnsureChildControls();
      _textBoxStyle.RegisterJavaScriptInclude (this, context);
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

    protected override void Render (HtmlTextWriter writer)
    {
      EvaluateWaiConformity();
      var renderer = GetRenderer(new HttpContextWrapper(Context), writer);
      renderer.Render();
    }

    /// <summary> Checks whether the control conforms to the required WAI level. </summary>
    /// <exception cref="Remotion.Web.UI.WcagException"> Thrown if the control does not conform to the required WAI level. </exception>
    protected virtual void EvaluateWaiConformity ()
    {
      if (WcagHelper.Instance.IsWcagDebuggingEnabled () && WcagHelper.Instance.IsWaiConformanceLevelARequired ())
      {
        if (TextBoxStyle.AutoPostBack == true)
          WcagHelper.Instance.HandleWarning (1, this, "TextBoxStyle.AutoPostBack");

        if (!IsReadOnly && AutoPostBack)
          WcagHelper.Instance.HandleWarning (1, this, "TextBox.AutoPostBack");
      }
    }

    protected abstract IRenderer GetRenderer (IHttpContext context, HtmlTextWriter writer);

    /// <summary> Loads the resources into the control's properties. </summary>
    protected override void LoadResources (IResourceManager resourceManager)
    {
      if (resourceManager == null)
        return;
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
      if (IsReadOnly || ! IsRequired)
        return new BaseValidator[0];

      _validators.AddRange (GetValidators());
      return _validators.ToArray();
    }

    bool IBocRenderableControl.IsDesignMode { get { return IsDesignMode; } }
    public void BaseAddAttributesToRender (HtmlTextWriter writer)
    {
      base.AddAttributesToRender (writer);
    }

    protected abstract IEnumerable<BaseValidator> GetValidators ();

    Style IBocRenderableControl.ControlStyle { get { return ControlStyle; } }

    protected override void OnPreRender (EventArgs e)
    {
      EnsureChildControls();
      base.OnPreRender (e);

      if (!IsDesignMode)
        Page.RegisterRequiresPostBack (this);

      LoadResources (GetResourceManager());
    }

    /// <summary> Returns the <see cref="IResourceManager"/> used to access the resources for this control. </summary>
    protected abstract IResourceManager GetResourceManager ();

    public string GetTextBoxUniqueID ()
    {
      return UniqueID + IdSeparator + "Boc_TextBox";
    }

    public string GetTextBoxClientID ()
    {
      return ClientID + ClientIDSeparator + "Boc_TextBox";
    }
  }
}