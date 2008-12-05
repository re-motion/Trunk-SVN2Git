// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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

/// <summary> This control can be used to display or edit values that can be edited in a text box. </summary>
/// <include file='doc\include\UI\Controls\BocTextValue.xml' path='BocTextValue/Class/*' />
[ValidationProperty ("Text")]
[DefaultEvent ("TextChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocTextValue: BusinessObjectBoundEditableWebControl, IPostBackDataHandler, IFocusableControl
{
  //  constants

  /// <summary> Text displayed when control is displayed in desinger, is read-only, and has no contents. </summary>
  private const string c_designModeEmptyLabelContents = "##";
  private const string c_defaultTextBoxWidth = "150pt";
  private const int c_defaultColumns = 60;

  //  statics

  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectNumericProperty), typeof (IBusinessObjectStringProperty), typeof (IBusinessObjectDateTimeProperty) };

  private static readonly object s_textChangedEvent = new object();

  // types

  /// <summary> A list of control specific resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
  ///   See the documentation of <b>GetString</b> for further details.
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Remotion.ObjectBinding.Web.Globalization.BocTextValue")]
  protected enum ResourceIdentifier
  {
    /// <summary> The validation error message displayed when no text is entered but input is required. </summary>
    RequiredErrorMessage,
    /// <summary> The validation error message displayed when the text exceeds the maximum length. </summary>
    MaxLengthValidationMessage,
    /// <summary> The validation error message displayed when the text is no valid date/time value. </summary>
    InvalidDateAndTimeErrorMessage,
    /// <summary> The validation error message displayed when the text is no valid date value. </summary>
    InvalidDateErrorMessage,
    /// <summary> The validation error message displayed when the text is no valid integer. </summary>
    InvalidIntegerErrorMessage,
    /// <summary> The validation error message displayed when the text is no valid integer. </summary>
    InvalidDoubleErrorMessage
  }

  // fields

  private BocTextValueType _valueType = BocTextValueType.Undefined;
  private BocTextValueType _actualValueType = BocTextValueType.Undefined;

  private string _text = string.Empty;
  private TextBox _textBox;
  private Label _label;

  private Style _commonStyle = new Style ();
  private TextBoxStyle _textBoxStyle = new TextBoxStyle ();
  private Style _labelStyle = new Style ();
  private string _format = null;

  private string _errorMessage;
  private ArrayList _validators;

	public BocTextValue()
	{
    _textBox = new TextBox();
    _label = new Label ();
    _validators = new ArrayList();
	}

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
    Binding.BindingChanged += new EventHandler (Binding_BindingChanged);

    if (!IsDesignMode)
      Page.RegisterRequiresPostBack (this);
  }

  public override void RegisterHtmlHeadContents (HttpContext context)
  {
 	  base.RegisterHtmlHeadContents(context);
  
    EnsureChildControls();
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
  /// <include file='doc\include\UI\Controls\BocTextValue.xml' path='BocTextValue/LoadPostData/*' />
  protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
  {
    string newValue = PageUtility.GetPostBackCollectionItem (Page, _textBox.UniqueID);
    bool isDataChanged = newValue != null && StringUtility.NullToEmpty (_text) != newValue;
    if (isDataChanged)
    {
      _text = newValue;
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
    _textBox.Text = _text;
  }

  protected override void OnPreRender (EventArgs e)
  {
    EnsureChildControls();
    base.OnPreRender (e);

    LoadResources (GetResourceManager());

    if (IsReadOnly)
    {
      string text = null;
      if (   TextBoxStyle.TextMode == TextBoxMode.MultiLine 
          && ! StringUtility.IsNullOrEmpty (_text))
      {
        //  Allows for an optional \r
        string temp = _text.Replace ("\r", "");
        string[] lines = temp.Split ('\n');
        for (int i = 0; i < lines.Length; i++)
          lines[i] = HttpUtility.HtmlEncode (lines[i]);
        text = StringUtility.ConcatWithSeparator (lines, "<br />");
      }
      else
      {
        text = HttpUtility.HtmlEncode (_text);
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
      SetEditModeValue();

      _textBox.ReadOnly = ! Enabled;
      _textBox.Width = Unit.Empty;
      _textBox.Height = Unit.Empty;
      _textBox.ApplyStyle (_commonStyle);
      _textBoxStyle.ApplyStyle (_textBox);
      if (_textBox.TextMode == TextBoxMode.MultiLine && _textBox.Columns < 1)
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
      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
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
      bool isTextBoxHeightEmpty = _textBox.Height.IsEmpty && StringUtility.IsNullOrEmpty (_textBox.Style["height"]);
      if (! isControlHeightEmpty && isTextBoxHeightEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      bool isControlWidthEmpty = Width.IsEmpty && StringUtility.IsNullOrEmpty (Style["width"]);
      bool isTextBoxWidthEmpty = _textBox.Width.IsEmpty && StringUtility.IsNullOrEmpty (_textBox.Style["width"]);
      if (isTextBoxWidthEmpty)
      {
        if (isControlWidthEmpty)
        {
          if (_textBoxStyle.TextMode != TextBoxMode.MultiLine || _textBoxStyle.Columns == null)
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
    _text = (string) values[1];
    _valueType = (BocTextValueType) values[2];
    _actualValueType = (BocTextValueType) values[3];

    _textBox.Text = _text;
  }

  protected override object SaveControlState ()
  {
    object[] values = new object[4];
    values[0] = base.SaveControlState ();
    values[1] = _text;
    values[2] = _valueType;
    values[3] = _actualValueType;
    return values;
  }


  /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
  /// <include file='doc\include\UI\Controls\BocTextValue.xml' path='BocTextValue/LoadValue/*' />
  public override void LoadValue (bool interim)
  {
    if (! interim)
    {
      if (Property != null && DataSource != null && DataSource.BusinessObject != null)
      {
        object value = DataSource.BusinessObject.GetProperty (Property);
        LoadValueInternal (value, interim);
      }
    }
  }


  /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
  /// <param name="value"> A <see cref="String"/> to load or <see langword="null"/>. </param>
  /// <include file='doc\include\UI\Controls\BocTextValue.xml' path='BocTextValue/LoadUnboundValue/*' />
  public void LoadUnboundValue (string value, bool interim)
  {
    LoadValueInternal (value, interim);
  }

  /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
  /// <param name="value"> The <see cref="Int32"/> value to load or <see langword="null"/>. </param>
  /// <include file='doc\include\UI\Controls\BocTextValue.xml' path='BocTextValue/LoadUnboundValue/*' />
  public void LoadUnboundValue (int? value, bool interim)
  {
    LoadValueInternal (value, interim);
  }

  /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
  /// <param name="value"> The <see cref="Double"/> value to load or <see langword="null"/>. </param>
  /// <include file='doc\include\UI\Controls\BocTextValue.xml' path='BocTextValue/LoadUnboundValue/*' />
  public void LoadUnboundValue (double? value, bool interim)
  {
    LoadValueInternal (value, interim);
  }

  /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
  /// <param name="value"> The <see cref="DateTime"/> value to load or <see langword="null"/>. </param>
  /// <include file='doc\include\UI\Controls\BocTextValue.xml' path='BocTextValue/LoadUnboundValue/*' />
  public void LoadUnboundValue (DateTime? value, bool interim)
  {
    LoadValueInternal (value, interim);
  }

  /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
  protected virtual void LoadValueInternal (object value, bool interim)
  {
    if (! interim)
    {
      Value = value;
      IsDirty = false;
    }
  }

  /// <summary> Saves the <see cref="Value"/> into the bound <see cref="IBusinessObject"/>. </summary>
  /// <include file='doc\include\UI\Controls\BocTextValue.xml' path='BocTextValue/SaveValue/*' />
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

    //  Dispatch simple properties
    string key = ResourceManagerUtility.GetGlobalResourceKey (ErrorMessage);
    if (! StringUtility.IsNullOrEmpty (key))
      ErrorMessage = resourceManager.GetString (key);
  }

  /// <summary> Creates the list of validators required for the current binding and property settings. </summary>
  /// <include file='doc\include\UI\Controls\BocTextValue.xml' path='BocTextValue/CreateValidators/*' />
  public override BaseValidator[] CreateValidators()
  {
    if (IsReadOnly)
      return new BaseValidator[0];

    string baseID = ID + "_Validator";
    ArrayList validators = new ArrayList(3);

    IResourceManager resourceManager = GetResourceManager();

    if (IsRequired)
    {
      RequiredFieldValidator requiredValidator = new RequiredFieldValidator();
      requiredValidator.ID = baseID + "Required";
      requiredValidator.ControlToValidate = TargetControl.ID;
      if (StringUtility.IsNullOrEmpty (_errorMessage))
        requiredValidator.ErrorMessage = resourceManager.GetString (ResourceIdentifier.RequiredErrorMessage);
      else
        requiredValidator.ErrorMessage = _errorMessage;

      validators.Add (requiredValidator);
    }

    if (_textBoxStyle.MaxLength != null)
    {
      LengthValidator lengthValidator = new LengthValidator();
      lengthValidator.ID = baseID + "MaxLength";
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

    BocTextValueType valueType = ActualValueType;
    switch (valueType)
    {
      case BocTextValueType.Undefined:
      {
        break;
      }
      case BocTextValueType.String:
      {
        break;
      }
      case BocTextValueType.DateTime:
      {
        DateTimeValidator typeValidator = new DateTimeValidator();
        typeValidator.ID = baseID + "Type";
        typeValidator.ControlToValidate = TargetControl.ID;
        if (StringUtility.IsNullOrEmpty (_errorMessage))
          typeValidator.ErrorMessage = resourceManager.GetString (ResourceIdentifier.InvalidDateAndTimeErrorMessage);
        else
          typeValidator.ErrorMessage = _errorMessage;
        validators.Add (typeValidator);
        break;
      }
      case BocTextValueType.Date:
      {
        CompareValidator typeValidator = new CompareValidator();
        typeValidator.ID = baseID + "Type";
        typeValidator.ControlToValidate = TargetControl.ID;
        typeValidator.Operator = ValidationCompareOperator.DataTypeCheck;
        typeValidator.Type = ValidationDataType.Date;
        if (StringUtility.IsNullOrEmpty (_errorMessage))
          typeValidator.ErrorMessage = resourceManager.GetString (ResourceIdentifier.InvalidDateErrorMessage);
        else
          typeValidator.ErrorMessage = _errorMessage;
        validators.Add (typeValidator);
        break;
      }
      case BocTextValueType.Byte:
      case BocTextValueType.Int16:
      case BocTextValueType.Int32:
      case BocTextValueType.Int64:
      case BocTextValueType.Decimal:
      case BocTextValueType.Double:
      case BocTextValueType.Single:
      {
        NumericValidator typeValidator = new NumericValidator();
        typeValidator.ID = baseID + "Type";
        typeValidator.ControlToValidate = TargetControl.ID;
        if (Property != null)
          typeValidator.AllowNegative = ((IBusinessObjectNumericProperty) Property).AllowNegative;
        typeValidator.DataType = GetNumericValidatorDataType (valueType);
        if (StringUtility.IsNullOrEmpty (_errorMessage))
          typeValidator.ErrorMessage = resourceManager.GetString (GetNumericValidatorErrorMessage (GetNumericValidatorDataType (valueType)));
        else
          typeValidator.ErrorMessage = _errorMessage;
        validators.Add (typeValidator);
        break;
      }
      default:
      {
        throw new ArgumentException ("BocTextValue '" + ID + "': Cannot convert " + valueType.ToString() + " to type " + typeof (ValidationDataType).FullName + ".");
      }
    }
    _validators.AddRange (validators);
    return (BaseValidator[]) validators.ToArray (typeof (BaseValidator));
  }

  private NumericValidationDataType GetNumericValidatorDataType (BocTextValueType valueType)
  {
    switch (valueType)
    {
      case BocTextValueType.Byte:
        return NumericValidationDataType.Byte;

      case BocTextValueType.Int16:
        return NumericValidationDataType.Int16;

      case BocTextValueType.Int32:
        return NumericValidationDataType.Int32;

      case BocTextValueType.Int64:
        return NumericValidationDataType.Int64;

      case BocTextValueType.Decimal:
        return NumericValidationDataType.Decimal;

      case BocTextValueType.Double:
        return NumericValidationDataType.Double;

      case BocTextValueType.Single:
        return NumericValidationDataType.Single;
    }
    throw new ArgumentOutOfRangeException ("valueType", valueType, "Only numeric value types are supported.");
  }

  private ResourceIdentifier GetNumericValidatorErrorMessage (NumericValidationDataType dataType)
  {
    switch (dataType)
    {
      case NumericValidationDataType.Byte:
        return ResourceIdentifier.InvalidIntegerErrorMessage;

      case NumericValidationDataType.Decimal:
        return ResourceIdentifier.InvalidDoubleErrorMessage;

      case NumericValidationDataType.Double:
        return ResourceIdentifier.InvalidDoubleErrorMessage;

      case NumericValidationDataType.Int16:
        return ResourceIdentifier.InvalidIntegerErrorMessage;

      case NumericValidationDataType.Int32:
        return ResourceIdentifier.InvalidIntegerErrorMessage;

      case NumericValidationDataType.Int64:
        return ResourceIdentifier.InvalidIntegerErrorMessage;

      case NumericValidationDataType.Single:
        return ResourceIdentifier.InvalidDoubleErrorMessage;
    }
    throw new ArgumentOutOfRangeException ("dataType", dataType, "Only numeric value types are supported.");
  }

  /// <summary> Handles refreshing the bound control. </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  private void Binding_BindingChanged (object sender, EventArgs e)
  {
    RefreshPropertiesFromObjectModel();
  }

  /// <summary>
  ///   Refreshes all properties of <see cref="BocTextValue"/> that depend on the current value of 
  ///   <see cref="BusinessObjectBoundWebControl.Property"/>.
  /// </summary>
  private void RefreshPropertiesFromObjectModel()
  {
    if (Property != null)
    {
      if (_valueType == BocTextValueType.Undefined)
        _actualValueType = GetBocTextValueType (Property);
      
      IBusinessObjectStringProperty stringProperty = Property as IBusinessObjectStringProperty;
      if (stringProperty != null)
      {
        if (_textBoxStyle.MaxLength == null)
        {
          int? length = stringProperty.MaxLength;
          if (length.HasValue)
            _textBoxStyle.MaxLength = length.Value;
        }
      }
    }
  }

  /// <summary> Returns the proper <see cref="BocTextValueType"/> for the passed <see cref="IBusinessObjectProperty"/>. </summary>
  /// <param name="property"> The <see cref="IBusinessObjectProperty"/> to analyze. </param>
  /// <exception cref="NotSupportedException"> The specialized type of the <paremref name="property"/> is not supported. </exception>
  private BocTextValueType GetBocTextValueType (IBusinessObjectProperty property)
  {
    if (property is IBusinessObjectStringProperty)
    {
      return BocTextValueType.String;
    }
    else if (property is IBusinessObjectNumericProperty)
    {
      IBusinessObjectNumericProperty numericProperty = (IBusinessObjectNumericProperty) property;
      if (numericProperty.Type == typeof (byte))
        return BocTextValueType.Byte;
      else if (numericProperty.Type == typeof (decimal))
        return BocTextValueType.Decimal;
      else if (numericProperty.Type == typeof (double))
        return BocTextValueType.Double;
      else if (numericProperty.Type == typeof (short))
        return BocTextValueType.Int16;
      else if (numericProperty.Type == typeof (int))
        return BocTextValueType.Int32;
      else if (numericProperty.Type == typeof (long))
        return BocTextValueType.Int64;
      else if (numericProperty.Type == typeof (float))
        return BocTextValueType.Single;
      else
        throw new NotSupportedException ("BocTextValue does not support property type " + property.GetType ());
    }
    else if (property is IBusinessObjectDateTimeProperty)
    {
      IBusinessObjectDateTimeProperty dateTimeProperty = (IBusinessObjectDateTimeProperty) property;
      if (dateTimeProperty.Type == DateTimeType.Date)
        return BocTextValueType.Date;
      else
        return BocTextValueType.DateTime;
    }
    else
    {
      throw new NotSupportedException ("BocTextValue does not support property type " + property.GetType ());
    }
  }

  /// <summary> Gets or sets the current value. </summary>
  /// <value> 
  ///   <para>
  ///     The value has the type specified in the <see cref="ValueType"/> property (<see cref="String"/>, 
  ///     <see cref="Int32"/>, <see cref="Double"/> or <see cref="DateTime"/>). If <see cref="ValueType"/> is not
  ///     set, the type is determined by the bound <see cref="BusinessObjectBoundWebControl.Property"/>.
  ///   </para><para>
  ///     Returns <see langword="null"/> if <see cref="Text"/> is an empty <see cref="String"/>.
  ///   </para>
  /// </value>
  /// <remarks> The dirty state is reset when the value is set. </remarks>
  /// <exception cref="FormatException"> 
  ///   The value of the <see cref="Text"/> property cannot be converted to the specified <see cref="ValueType"/>.
  /// </exception>
  [Description("Gets or sets the current value.")]
  [Browsable (false)]
  public new object Value
  {
    get 
    {
      string text = _text;
      if (text != null)
        text = text.Trim();

      if (StringUtility.IsNullOrEmpty (text))
        return null;

      switch (ActualValueType)
      {
        case BocTextValueType.String:
          return text;

        case BocTextValueType.Byte:
          return byte.Parse (text);

        case BocTextValueType.Int16:
          return short.Parse (text);

        case BocTextValueType.Int32:
          return int.Parse (text);

        case BocTextValueType.Int64:
          return long.Parse (text);

        case BocTextValueType.Date:
          return DateTime.Parse (text).Date;

        case BocTextValueType.DateTime:
          return DateTime.Parse (text);

        case BocTextValueType.Decimal:
          return decimal.Parse (text);

        case BocTextValueType.Double:
          return double.Parse (text);

        case BocTextValueType.Single:
          return float.Parse (text);
      }
      return text;
    }

    set 
    { 
      IsDirty = true;

      if (value == null)
      {
        _text = null;
        return;
      }

      IFormattable formattable = value as IFormattable;
      if (formattable != null)
      {
        string format = Format;
        if (format == null)
        {
          if (ActualValueType == BocTextValueType.Date)
            format = "d";
          else if (ActualValueType == BocTextValueType.DateTime)
            format = "g";
        }
        _text = formattable.ToString (format, null);
      }
      else
      {
        _text = value.ToString();
      }
    }
  }

  /// <summary>
  ///   Gets a flag describing whether it is save (i.e. accessing <see cref="Value"/> does not throw a 
  ///   <see cref="FormatException"/> or <see cref="OverflowException"/>) to read the contents of <see cref="Value"/>.
  /// </summary>
  /// <remarks> Valid values include <see langword="null"/>. </remarks>
  [Browsable(false)]
  public bool IsValidValue
  {
    get
    {
      try
      {
        //  Force the evaluation of Value
        if (Value != null)
          return true;
      }
      catch (FormatException)
      {
        return false;
      }
      catch (OverflowException)
      {
        return false;
      }

      return true;
    }
  }

  /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
 protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = value; }
  }

  /// <summary> Gets or sets the string representation of the current value. </summary>
  /// <value> 
  ///   An empty <see cref="String"/> if the control's value is <see langword="null"/> or empty. 
  ///   The default value is an empty <see cref="String"/>. 
  /// </value>
  [Description("Gets or sets the string representation of the current value.")]
  [Category("Data")]
  [DefaultValue ("")]
  public string Text
  {
    get { return StringUtility.NullToEmpty (_text); }
    set
    {
      IsDirty = true;
      _text = value;
    }
  }

  /// <summary> Gets or sets the <see cref="BocTextValueType"/> assigned from an external source. </summary>
  /// <value> 
  ///   The externally set <see cref="BocTextValueType"/>. The default value is 
  ///   <see cref="BocTextValueType.Undefined"/>. 
  /// </value>
  [Description("Gets or sets a fixed value type.")]
  [Category("Data")]
  [DefaultValue (BocTextValueType.Undefined)]
  public BocTextValueType ValueType
  {
    get { return _valueType; }
    set 
    {
      if (_valueType != value)
      {
        _valueType = value;
        _actualValueType = value;
        if (_valueType != BocTextValueType.Undefined)
          _text = string.Empty;
      }
    }
  }

  /// <summary>
  ///   Gets the controls fixed <see cref="ValueType"/> or, if <see cref="BocTextValueType.Undefined"/>, 
  ///   the <see cref="BusinessObjectBoundWebControl.Property"/>'s value type.
  /// </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public BocTextValueType ActualValueType
  {
    get 
    {
      if (_valueType == BocTextValueType.Undefined && Property != null)
        _actualValueType = GetBocTextValueType (Property);
      return _actualValueType;
    }
  }

  /// <summary> Gets or sets the format string used to create the string value.  </summary>
  /// <value> 
  ///   A string passed to the <b>ToString</b> method of the object returned by <see cref="Value"/>.
  ///   The default value is an empty <see cref="String"/>. 
  /// </value>
  /// <remarks>
  ///   <see cref="IFormattable"/> is used to format the value using this string. The default is "d" for date-only
  ///   values and "g" for date/time values (use "G" to display seconds too). 
  /// </remarks>
  [Description ("Gets or sets the format string used to create the string value. Format must be parsable by the value's type if the control is in edit mode.")]
  [Category ("Style")]
  [DefaultValue ("")]
  public string Format
  {
    get { return StringUtility.EmptyToNull (_format); }
    set { _format = value; }
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

  /// <summary> The <see cref="BocTextValue"/> supports only scalar properties. </summary>
  /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="false"/>. </returns>
  /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
  protected override bool SupportsPropertyMultiplicity (bool isList)
  {
    return ! isList;
  }

  /// <summary>
  ///   The <see cref="BocTextValue"/> supports properties of types <see cref="IBusinessObjectStringProperty"/>,
  ///   <see cref="IBusinessObjectDateTimeProperty"/>, and <see cref="IBusinessObjectNumericProperty"/>.
  /// </summary>
  /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
  protected override Type[] SupportedPropertyInterfaces
  {
    get { return s_supportedPropertyInterfaces; }
  }

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

  /// <summary> This event is fired when the text is changed between postbacks. </summary>
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
  ///   Use the <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> to assign individual style settings for
  ///   the respective modes. Note that if you set one of the <b>Font</b> attributes (Bold, Italic etc.) to 
  ///   <see langword="true"/>, this cannot be overridden using <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> 
  ///   properties.
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
  [Category("Style")]
  [Description("The style that you want to apply to the TextBox (edit mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public TextBoxStyle TextBoxStyle
  {
    get { return _textBoxStyle; }
  }

  /// <summary> Gets the style that you want to apply to the <see cref="Label"/> (read-only mode) only. </summary>
  /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to the Label (read-only mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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
  /// <summary> Gets the CSS-Class applied to the <see cref="BocTextValue"/> itself. </summary>
  /// <remarks> 
  ///   <para> Class: <c>bocTextValue</c>. </para>
  ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassBase
  { get { return "bocTextValue"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocTextValue"/> when it is displayed in read-only mode. </summary>
  /// <remarks> 
  ///   <para> Class: <c>readOnly</c>. </para>
  ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocTextValue.readOnly</c> as a selector. </para>
  /// </remarks>
  protected virtual string CssClassReadOnly
  { get { return "readOnly"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocTextValue"/> when it is displayed disabled. </summary>
  /// <remarks> 
  ///   <para> Class: <c>disabled</c>. </para>
  ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocTextValue.disabled</c> as a selector.</para>
  /// </remarks>
  protected virtual string CssClassDisabled
  { get { return "disabled"; } }
  #endregion
}

/// <summary> A list possible data types for the <see cref="BocTextValue"/> </summary>
public enum BocTextValueType
{
  /// <summary> 
  ///   Format the value as its default string representation. 
  ///   No parsing is possible, <see cref="P:BoxTextValue.Value"/> will return a string. 
  /// </summary>
  Undefined,
  /// <summary> Interpret the value as a <see cref="String"/>. </summary>
  String,
  /// <summary> Interpret the value as an <see cref="Byte"/>. </summary>
  Byte,
  /// <summary> Interpret the value as a <see cref="Int16"/>. </summary>
  Int16,
  /// <summary> Interpret the value as a <see cref="Int32"/>. </summary>
  Int32,
  /// <summary> Interpret the value as a <see cref="Int64"/>. </summary>
  Int64,
  /// <summary> Interpret the value as a <see cref="DateTime"/> with the time component set to zero. </summary>
  Date,
  /// <summary> Interpret the value as a <see cref="DateTime"/>. </summary>
  DateTime,
  /// <summary> Interpret the value as a <see cref="Decimal"/>. </summary>
  Decimal,
  /// <summary> Interpret the value as a <see cref="Double"/>. </summary>
  Double,
  /// <summary> Interpret the value as a <see cref="Single"/>. </summary>
  Single,
}

}
