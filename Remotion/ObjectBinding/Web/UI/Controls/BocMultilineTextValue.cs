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
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{

/// <summary> This control can be used to display or edit a list of strings. </summary>
/// <include file='doc\include\UI\Controls\BocMultilineTextValue.xml' path='BocMultilineTextValue/Class/*' />
[ValidationProperty ("Text")]
[DefaultEvent ("TextChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocMultilineTextValue: BusinessObjectBoundEditableWebControl, IPostBackDataHandler, IFocusableControl
{
	// constants

  /// <summary> Text displayed when control is displayed in desinger, is read-only, and has no contents. </summary>
  private const string c_designModeEmptyLabelContents = "##";
  private const string c_defaultTextBoxWidth = "150pt";
  private const int c_defaultColumns = 60;

  // types

  /// <summary> A list of control specific resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
  ///   See the documentation of <b>GetString</b> for further details.
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Remotion.ObjectBinding.Web.Globalization.BocMultilineTextValue")]
  protected enum ResourceIdentifier
  {
    /// <summary> The validation error message displayed when no text is entered but input is required. </summary>
    RequiredValidationMessage,
    /// <summary> The validation error message displayed when entered text exceeds the maximum length. </summary>
    MaxLengthValidationMessage
  }

  // static members
	
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectStringProperty) };

  private static readonly object s_textChangedEvent = new object();

	// member fields

  private TextBox _textBox;
  private Label _label;

  private Style _commonStyle;
  private TextBoxStyle _textBoxStyle;
  private Style _labelStyle;

  /// <summary> The concatenated string built from the string array. Uses <c>\r\n</c> as delimiter. </summary>
  private string _internalValue = null;

  private string _errorMessage;
  private ArrayList _validators;

  // construction and disposing

	public BocMultilineTextValue()
	{
    _commonStyle = new Style();
    _textBoxStyle = new TextBoxStyle (TextBoxMode.MultiLine);
    _labelStyle = new Style();
    _textBox = new TextBox();
    _label = new Label();
    _validators = new ArrayList();
	}

  // methods and properties

  protected override void CreateChildControls()
  {
    _textBox.ID = ID + "_Boc_TextBox";
    _textBox.EnableViewState = false;
    Controls.Add (_textBox);

    _label.ID = ID + "_Boc_Label";
    _label.EnableViewState = false;
    Controls.Add (_label);
  }


  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
    if (!IsDesignMode)
      Page.RegisterRequiresPostBack (this);
  }

  public override void RegisterHtmlHeadContents (HttpContext context)
  {
    base.RegisterHtmlHeadContents (context);

    _textBoxStyle.RegisterJavaScriptInclude (_textBox, context);
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
  void IPostBackDataHandler.RaisePostDataChangedEvent()
  {
    RaisePostDataChangedEvent();
  }

  /// <summary>
  ///   Uses the <paramref name="postCollection"/> to determine whether the value of this control has been changed 
  ///   between postbacks.
  /// </summary>
  /// <include file='doc\include\UI\Controls\BocMultilineTextValue.xml' path='BocMultilineTextValue/LoadPostData/*' />
  protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
  {
    string newValue = PageUtility.GetPostBackCollectionItem (Page, _textBox.UniqueID);
    bool isDataChanged = newValue != null && StringUtility.NullToEmpty (_internalValue) != newValue;
    if (isDataChanged)
    {
      _internalValue = newValue;
      IsDirty = true;
    }
    return isDataChanged;
  }

  /// <summary> Called when the state of the control has changed between postbacks. </summary>
  protected virtual void RaisePostDataChangedEvent()
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

  /// <summary> Checks whether the control conforms to the required WAI level. </summary>
  /// <exception cref="Remotion.Web.UI.WcagException"> Thrown if the control does not conform to the required WAI level. </exception>
  protected virtual void EvaluateWaiConformity ()
  {
    if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
    {
      if (TextBoxStyle.AutoPostBack == true)
        WcagHelper.Instance.HandleWarning (1, this, "TextBoxStyle.AutoPostBack");

      if (TextBox.AutoPostBack)
        WcagHelper.Instance.HandleWarning (1, this, "TextBox.AutoPostBack");
    }
  }

  public override void PrepareValidation ()
  {
    base.PrepareValidation();

    if (! IsReadOnly)
      SetEditModeValue ();
  }

  private void SetEditModeValue ()
  {
    _textBox.Text = Text;
  }

  protected override void OnPreRender (EventArgs e)
  {
    EnsureChildControls();
    base.OnPreRender (e);
    
    LoadResources (GetResourceManager());
    
    if (IsReadOnly)
    {
      string[] lines = Value;
      string text = null;
      if (lines != null)
      {
        for (int i = 0; i < lines.Length; i++)
          lines[i] = HttpUtility.HtmlEncode (lines[i]);
        text = StringUtility.ConcatWithSeparator (lines, "<br />");
      }
      if (StringUtility.IsNullOrEmpty (text))
      {
        if (IsDesignMode)
        {
          text = c_designModeEmptyLabelContents;
          //  Too long, can't resize in designer to less than the content's width
          //  _label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
        }
        else
        {
          text = "&nbsp;";
        }
      }
      _label.Text = text;

      _label.Width = Unit.Empty;
      _label.Height = Unit.Empty;
      _label.ApplyStyle (_commonStyle);
      _label.ApplyStyle (_labelStyle);
    }
    else
    {
      SetEditModeValue ();

      _textBox.ReadOnly = ! Enabled;
      _textBox.Width = Unit.Empty;
      _textBox.Height = Unit.Empty;
      _textBox.ApplyStyle (_commonStyle);
      _textBoxStyle.ApplyStyle (_textBox);
      if (_textBox.Columns < 1)
        _textBox.Columns = c_defaultColumns;
    }
  }

  protected override void AddAttributesToRender (HtmlTextWriter writer)
  {
    string backUpStyleWidth = Style["width"];
    if (! StringUtility.IsNullOrEmpty (Style["width"]))
      Style["width"] = null;
    Unit backUpWidth = Width; // base.Width and base.ControlStyle.Width
    if (! Width.IsEmpty)
      Width = Unit.Empty;

    bool isReadOnly = IsReadOnly;
    bool isDisabled = ! Enabled;

    string backUpCssClass = CssClass; // base.CssClass and base.ControlStyle.CssClass
    if ((isReadOnly || isDisabled) && ! StringUtility.IsNullOrEmpty (CssClass))
    {
      if (isReadOnly)
        CssClass += " " + CssClassReadOnly;
      else if (isDisabled)
        CssClass += " " + CssClassDisabled;
    }
    string backUpAttributeCssClass = Attributes["class"];
    if ((isReadOnly || isDisabled) && ! StringUtility.IsNullOrEmpty (Attributes["class"]))
    {
      if (isReadOnly)
        Attributes["class"] += " " + CssClassReadOnly;
      else if (isDisabled)
        Attributes["class"] += " " + CssClassDisabled;
    }
    
    base.AddAttributesToRender (writer);

    if ((isReadOnly || isDisabled) && ! StringUtility.IsNullOrEmpty (CssClass))
      CssClass = backUpCssClass;
    if ((isReadOnly || isDisabled) && ! StringUtility.IsNullOrEmpty (Attributes["class"]))
      Attributes["class"] = backUpAttributeCssClass;
    
    if (StringUtility.IsNullOrEmpty (CssClass) && StringUtility.IsNullOrEmpty (Attributes["class"]))
    {
      string cssClass = CssClassBase;
      if (isReadOnly)
        cssClass += " " + CssClassReadOnly;
      else if (isDisabled)
        cssClass += " " + CssClassDisabled;
      writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
    }

    if (! StringUtility.IsNullOrEmpty (backUpStyleWidth))
      Style["width"] = backUpStyleWidth;
    if (! backUpWidth.IsEmpty)
      Width = backUpWidth;

    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "auto");
  }

  protected override void RenderContents (HtmlTextWriter writer)
  {
    EvaluateWaiConformity();

    if (IsReadOnly)
    {
      bool isControlHeightEmpty = Height.IsEmpty && StringUtility.IsNullOrEmpty (Style["height"]);
      bool isLabelHeightEmpty = _label.Height.IsEmpty && StringUtility.IsNullOrEmpty (_label.Style["height"]);
      if (! isControlHeightEmpty && isLabelHeightEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      bool isControlWidthEmpty = Width.IsEmpty && StringUtility.IsNullOrEmpty (Style["width"]);
      bool isLabelWidthEmpty = _label.Width.IsEmpty &&StringUtility.IsNullOrEmpty (_label.Style["width"]);
      if (! isControlWidthEmpty && isLabelWidthEmpty)
      {
        if (! Width.IsEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Width.ToString());
        else
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Style["width"]);
      }

      _label.RenderControl (writer);
    }
    else
    {
      bool isControlHeightEmpty = Height.IsEmpty && StringUtility.IsNullOrEmpty (Style["height"]);
      bool isTextBoxHeightEmpty = StringUtility.IsNullOrEmpty (_textBox.Style["height"]);
      if (! isControlHeightEmpty && isTextBoxHeightEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      bool isControlWidthEmpty = Width.IsEmpty && StringUtility.IsNullOrEmpty (Style["width"]);
      bool isTextBoxWidthEmpty = _textBox.Width.IsEmpty && StringUtility.IsNullOrEmpty (_textBox.Style["width"]);
      if (isTextBoxWidthEmpty)
      {
        if (isControlWidthEmpty)
        {
          if (_textBoxStyle.Columns == null)
            writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultTextBoxWidth);
        }
        else
        {
          if (! Width.IsEmpty)
            writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Width.ToString());
          else
            writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Style["width"]);
        }
      }

      _textBox.RenderControl (writer);
    }
  }

  protected override void LoadControlState (object savedState)
  {
    object[] values = (object[]) savedState;

    base.LoadControlState (values[0]);
    _internalValue = (string) values[1];

    _textBox.Text = Text;
  }

  protected override object SaveControlState ()
  {
    object[] values = new object[2];

    values[0] = base.SaveControlState ();
    values[1] = _internalValue;

    return values;
  }


  /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
  /// <include file='doc\include\UI\Controls\BocMultilineTextValue.xml' path='BocMultilineTextValue/LoadValue/*' />
  public override void LoadValue (bool interim)
  {
    if (! interim)
    {
      if (Property != null && DataSource != null && DataSource.BusinessObject != null)
      {
        string[] value = (string[]) DataSource.BusinessObject.GetProperty (Property);
        LoadValueInternal (value, interim);
      }
    }
  }

  /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
  /// <param name="value"> The <see cref="String"/> <see cref="Array"/> to load or <see langword="null"/>. </param>
  /// <include file='doc\include\UI\Controls\BocMultilineTextValue.xml' path='BocMultilineTextValue/LoadUnboundValue/*' />
  public void LoadUnboundValue (string[] value, bool interim)
  {
    LoadValueInternal (value, interim);
  }

  /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
  protected virtual void LoadValueInternal (string[] value, bool interim)
  {
    if (! interim)
    {
      Value = value;
      IsDirty = false;
    }
  }

  /// <summary> Saves the <see cref="Value"/> into the bound <see cref="IBusinessObject"/>. </summary>
  /// <include file='doc\include\UI\Controls\BocMultilineTextValue.xml' path='BocMultilineTextValue/SaveValue/*' />
  public override void SaveValue (bool interim)
  {
    if (!interim && IsDirty)
    {
      if (Property != null && DataSource != null && DataSource.BusinessObject != null && ! IsReadOnly)
      {
        DataSource.BusinessObject.SetProperty (Property, Value);
        IsDirty = false;
      }
    }
  }

  /// <summary> Returns the <see cref="IResourceManager"/> used to access the resources for this control. </summary>
  protected virtual IResourceManager GetResourceManager()
  {
    return GetResourceManager (typeof (ResourceIdentifier));
  }

  /// <summary> Loads the resources into the control's properties. </summary>
  protected override void LoadResources (IResourceManager resourceManager)
  {
    if (resourceManager == null)
      return;
    if (IsDesignMode)
      return;
    base.LoadResources (resourceManager);

    string key = ResourceManagerUtility.GetGlobalResourceKey (ErrorMessage);
    if (! StringUtility.IsNullOrEmpty (key))
      ErrorMessage = resourceManager.GetString (key);
  }

  /// <summary> Creates the list of validators required for the current binding and property settings. </summary>
  /// <include file='doc\include\UI\Controls\BocMultilineTextValue.xml' path='BocMultilineTextValue/CreateValidators/*' />
  public override BaseValidator[] CreateValidators()
  {
    if (IsReadOnly || ! IsRequired)
      return new BaseValidator[0];

    ArrayList validators = new ArrayList (2);

    RequiredFieldValidator requiredValidator = new RequiredFieldValidator();
    requiredValidator.ID = ID + "_ValidatorRequired";
    requiredValidator.ControlToValidate = TargetControl.ID;
    if (StringUtility.IsNullOrEmpty (_errorMessage))
    {
      requiredValidator.ErrorMessage = 
          GetResourceManager().GetString (ResourceIdentifier.RequiredValidationMessage);
    }
    else
    {
      requiredValidator.ErrorMessage = _errorMessage;
    }      
    validators.Add (requiredValidator);

    if ( _textBoxStyle.MaxLength != null)
    {
      LengthValidator lengthValidator = new LengthValidator();
      lengthValidator.ID = ID + "_ValidatorMaxLength";
      lengthValidator.ControlToValidate = TargetControl.ID;
      lengthValidator.MaximumLength = _textBoxStyle.MaxLength.Value;
      if (StringUtility.IsNullOrEmpty (_errorMessage))
      {
        string maxLengthMessage = GetResourceManager().GetString (ResourceIdentifier.MaxLengthValidationMessage);
        lengthValidator.ErrorMessage = string.Format (maxLengthMessage, _textBoxStyle.MaxLength.Value);            
      }
      else
      {
        lengthValidator.ErrorMessage = _errorMessage;
      }      
      validators.Add (lengthValidator);
    }

    _validators.AddRange (validators);
    return (BaseValidator[]) validators.ToArray (typeof (BaseValidator));
  }

  /// <summary> Gets or sets the <see cref="IBusinessObjectStringProperty"/> object this control is bound to. </summary>
  /// <value> An <see cref="IBusinessObjectStringProperty"/> object. </value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public new IBusinessObjectStringProperty Property
  {
    get { return (IBusinessObjectStringProperty) base.Property; }
    set { base.Property = ArgumentUtility.CheckType<IBusinessObjectStringProperty> ("value", value); }
  }
  
  /// <summary> Gets or sets the current value. </summary>
  /// <value> The <see cref="String"/> array currently displayed or <see langword="null"/> if no text is entered. </value>
  /// <remarks> The dirty state is reset when the value is set. </remarks>
  [Browsable(false)]
  public new string[] Value
  {
    get 
    {
      string text = _internalValue;
      if (text != null)
        text = text.Trim();

      if (StringUtility.IsNullOrEmpty (text))
      {
        return null;
      }
      else
      {
        //  Allows for an optional \r
        string temp = _internalValue.Replace ("\r", "");
        return temp.Split ('\n');
      }
    }
    set
    {
      IsDirty = true;

      if (value == null)
        _internalValue = null;
      else
        _internalValue = StringUtility.ConcatWithSeparator (value, "\r\n");
    }
  }

  /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
  /// <value> The value must be of type <b>string[]</b>. </value>
  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = (string[]) value; }
  }

  /// <summary> Gets or sets the string representation of the current value. </summary>
  /// <remarks> Uses <c>\r\n</c> or <c>\n</c> as separation characters. The default value is an empty <see cref="String"/>. </remarks>
  [Description("The string representation of the current value.")]
  [Category("Data")]
  [DefaultValue ("")]
  public string Text
  {
    get { return StringUtility.NullToEmpty (_internalValue); }
    set { _internalValue = value; }
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
  public override string[] GetTrackedClientIDs()
  {
    return IsReadOnly ? new string[0] : new string[1] { _textBox.ClientID };
  }

  /// <summary>
  ///   The <see cref="BocMultilineTextValue"/> supports properties of types <see cref="IBusinessObjectStringProperty"/>.
  /// </summary>
  /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
  protected override Type[] SupportedPropertyInterfaces
  {
    get { return s_supportedPropertyInterfaces; }
  }

  /// <summary> The <see cref="BocMultilineTextValue"/> supports only list properties. </summary>
  /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="true"/>. </returns>
  /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
  protected override bool SupportsPropertyMultiplicity (bool isList)
  {
    return isList;
  }

  /// <summary>
  ///   Gets a flag that determines whether it is valid to generate HTML &lt;label&gt; tags referencing the
  ///   <see cref="TargetControl"/>.
  /// </summary>
  /// <value> Always <see langword="true"/>. </value>
  public override bool UseLabel
  {
    get { return true; }
  }

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its 
  ///   <see cref="Control.ClientID"/>.
  /// </summary>
  /// <value> The <see cref="TextBox"/> if the control is in edit mode, otherwise the control itself. </value>
  public override Control TargetControl 
  {
    get { return IsReadOnly ? (Control) this : _textBox; }
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
    get { return IsReadOnly ? null : _textBox.ClientID; }
  }

  /// <summary> Occurs when the <see cref="Value"/> property changes between postbacks. </summary>
  [Category ("Action")]
  [Description ("Fires when the value of the control has changed.")]
  public event EventHandler TextChanged
  {
    add { Events.AddHandler (s_textChangedEvent, value); }
    remove { Events.RemoveHandler (s_textChangedEvent, value); }
  }

  /// <summary>
  ///   Gets the style that you want to apply to the <see cref="TextBox"/> (edit mode) 
  ///   and the <see cref="Label"/> (read-only mode).
  /// </summary>
  /// <remarks>
  ///   Use the <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> to assign individual 
  ///   style settings for the respective modes. Note that if you set one of the <b>Font</b> 
  ///   attributes (Bold, Italic etc.) to <see langword="true"/>, this cannot be overridden using 
  ///   <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> properties.
  /// </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to the TextBox (edit mode) and the Label (read-only mode).")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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

  /// <summary> Gets the <see cref="TextBox"/> used in edit mode. </summary>
  [Browsable (false)]
  public TextBox TextBox
  {
    get { return _textBox; }
  }

  /// <summary> Gets the <see cref="Label"/> used in read-only mode. </summary>
  [Browsable (false)]
  public Label Label
  {
    get { return _label; }
  }

  /// <summary> Gets or sets the validation error message. </summary>
  /// <value> 
  ///   The error message displayed when validation fails. The default value is an empty <see cref="String"/>.
  ///   In case of the default value, the text is read from the resources for this control.
  /// </value>
  [Description("Validation message displayed if there is an error.")]
  [Category ("Validator")]
  [DefaultValue("")]
  public string ErrorMessage
  {
    get
    { 
      return _errorMessage; 
    }
    set 
    {
      _errorMessage = value; 
      for (int i = 0; i < _validators.Count; i++)
      {
        BaseValidator validator = (BaseValidator) _validators[i];
        validator.ErrorMessage = _errorMessage;
      }
    }
  }

  #region protected virtual string CssClass...
  /// <summary> Gets the CSS-Class applied to the <see cref="BocMultilineTextValue"/> itself. </summary>
  /// <remarks> 
  ///   <para> Class: <c>bocMultilineTextValue</c>. </para>
  ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassBase
  { get { return "bocMultilineTextValue"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocMultilineTextValue"/> when it is displayed in read-only mode. </summary>
  /// <remarks> 
  ///   <para> Class: <c>readOnly</c>. </para>
  ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocMultilineTextValue.readOnly</c> as a selector. </para>
  /// </remarks>
  protected virtual string CssClassReadOnly
  { get { return "readOnly"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocMultilineTextValue"/> when it is displayed disabled. </summary>
  /// <remarks> 
  ///   <para> Class: <c>disabled</c>. </para>
  ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocMultilineTextValue.disabled</c> as a selector. </para>
  /// </remarks>
  protected virtual string CssClassDisabled
  { get { return "disabled"; } }
  #endregion
}
}
