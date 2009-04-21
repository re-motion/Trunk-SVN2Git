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
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{

/// <summary> This control can be used to display or edit date/time values. </summary>
/// <include file='doc\include\UI\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/Class/*' />
// TODO: see "Doc\Bugs and ToDos.txt"
[ValidationProperty ("ValidationValue")]
[DefaultEvent ("TextChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocDateTimeValue: BusinessObjectBoundEditableWebControl, IPostBackDataHandler, IFocusableControl
{
  //  constants

  /// <summary> Text displayed when control is displayed in desinger and is read-only has no contents. </summary>
  private const string c_designModeEmptyLabelContents = "##";

  private const string c_defaultControlWidth = "150pt";
  private const int c_defaultDatePickerLengthInPoints = 150;

  private const string c_datePickerPopupForm = "DatePickerForm.aspx";
  private const string c_datePickerScriptFileUrl = "DatePicker.js";
  private const string c_styleFileUrl = "BocDateTimeValue.css";

  // types

  /// <summary> A list of control specific resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
  ///   See the documentation of <b>GetString</b> for further details.
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Remotion.ObjectBinding.Web.Globalization.BocDateTimeValue")]
  protected enum ResourceIdentifier
  {
    /// <summary> The validation error message displayed when no input is provided. </summary>
    RequiredErrorMessage,
    /// <summary> The validation error message displayed when the input is incomplete. </summary>
    IncompleteErrorMessage,
    /// <summary> The validation error message displayed when both the date and the time component invalid. </summary>
    InvalidDateAndTimeErrorMessage,
    /// <summary> The validation error message displayed when the date component is invalid. </summary>
    InvalidDateErrorMessage,
    /// <summary> The validation error message displayed when the time component is invalid. </summary>
    InvalidTimeErrorMessage,
    /// <summary> The alternate text displayed for the date picker button. </summary>
    DataPickerButtonAlternateText
  }

  // static members
	
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { typeof (IBusinessObjectDateTimeProperty) };

  private static readonly string s_datePickerScriptFileKey = typeof (BocDateTimeValue).FullName + "_DatePickerScript";
  private static readonly string s_styleFileKey = typeof (BocDateTimeValue).FullName + "_Style";
  private static readonly object s_dateTimeChangedEvent = new object();

	// member fields

  private TextBox _dateTextBox;
  private TextBox _timeTextBox;
  private Label _label;
  private HyperLink _datePickerButton;

  private Style _commonStyle;
  private SingleRowTextBoxStyle _dateTimeTextBoxStyle;
  private SingleRowTextBoxStyle _dateTextBoxStyle;
  private SingleRowTextBoxStyle _timeTextBoxStyle;
  private Style _labelStyle;
  private Style _datePickerButtonStyle;

  private string _internalDateValue = null;
  private string _internalTimeValue = null;
  /// <summary> A backup of the <see cref="DateTime"/> value. </summary>
  private DateTime? _savedDateTimeValue = null;

  private BocDateTimeValueType _valueType = BocDateTimeValueType.Undefined;
  private BocDateTimeValueType _actualValueType = BocDateTimeValueType.Undefined;

  private Unit _datePickerPopupWidth = Unit.Point (c_defaultDatePickerLengthInPoints);
  private Unit _datePickerPopupHeight = Unit.Point (c_defaultDatePickerLengthInPoints);

  private bool _showSeconds = false;
  private bool _provideMaxLength = true;
  private bool _enableClientScript = true;

  private string _errorMessage;
  private ArrayList _validators;

  /// <summary> Flag that determines whether the client script will be rendered. </summary>
  private bool _hasClientScript = false;

  // construction and disposing

	public BocDateTimeValue()
	{
    _commonStyle = new Style();
    _dateTimeTextBoxStyle = new SingleRowTextBoxStyle();
    _dateTextBoxStyle = new SingleRowTextBoxStyle();
    _timeTextBoxStyle = new SingleRowTextBoxStyle();
    _labelStyle = new Style();
    _datePickerButtonStyle = new Style();
    _dateTextBox = new TextBox();
    _datePickerButton = new HyperLink();
    _timeTextBox = new TextBox();
    _label = new Label();
    _validators = new ArrayList();
	}

	// methods and properties

  protected override void CreateChildControls()
  {
    _dateTextBox.ID = ID + "_Boc_DateTextBox";
    _dateTextBox.EnableViewState = false;
    Controls.Add (_dateTextBox);

    _datePickerButton.ID = ID + "_Boc_DatePickerButton";
    _datePickerButton.EnableViewState = false;
    Controls.Add (_datePickerButton);

    _timeTextBox.ID = ID + "_Boc_TimeTextBox";
    _timeTextBox.EnableViewState = false;
    Controls.Add (_timeTextBox);

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
    base.RegisterHtmlHeadContents (context);

    if (!HtmlHeadAppender.Current.IsRegistered (s_datePickerScriptFileKey))
    {
      string scriptUrl = ResourceUrlResolver.GetResourceUrl (this, context, typeof (DatePickerPage), ResourceType.Html, c_datePickerScriptFileUrl);
      HtmlHeadAppender.Current.RegisterJavaScriptInclude (s_datePickerScriptFileKey, scriptUrl);
    }
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
  /// <include file='doc\include\UI\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/LoadPostData/*' />
  protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
  {
    //  Date input field

    string newDateValue = PageUtility.GetPostBackCollectionItem (Page, _dateTextBox.UniqueID);
    bool isDateChanged =   newDateValue != null 
                        && StringUtility.NullToEmpty (_internalDateValue) != newDateValue;
    if (isDateChanged)
    {
      InternalDateValue = StringUtility.EmptyToNull (newDateValue);
      
      //  Reset the time in if the control is displayed in date mode and the date was changed
      if (   ActualValueType == BocDateTimeValueType.Date
          && _savedDateTimeValue.HasValue)
      {
         _savedDateTimeValue = _savedDateTimeValue.Value.Date;
      }
      IsDirty = true;
    }

    //  Time input field

    string newTimeValue = PageUtility.GetPostBackCollectionItem (Page, _timeTextBox.UniqueID);
    bool isTimeChanged =   newTimeValue != null 
                        && StringUtility.NullToEmpty (_internalTimeValue) != newTimeValue;
    if (isTimeChanged)
    {
      InternalTimeValue = StringUtility.EmptyToNull (newTimeValue);
      
      //  Reset the seconds if the control does not display seconds and the time was changed
      if (   ! ShowSeconds
          && _savedDateTimeValue.HasValue)
      {
          TimeSpan seconds = new TimeSpan (0, 0, _savedDateTimeValue.Value.Second);
         _savedDateTimeValue = _savedDateTimeValue.Value.Subtract (seconds);
      }
      IsDirty = true;
    }

    return isDateChanged || isTimeChanged;
  }

  /// <summary> Called when the state of the control has changed between postbacks. </summary>
  protected virtual void RaisePostDataChangedEvent()
  {
    if (! IsReadOnly && Enabled)
      OnDateTimeChanged();
  }

  /// <summary> Fires the <see cref="DateTimeChanged"/> event. </summary>
  protected virtual void OnDateTimeChanged ()
  {
    EventHandler eventHandler = (EventHandler) Events[s_dateTimeChangedEvent];
    if (eventHandler != null)
      eventHandler (this, EventArgs.Empty);
  }

  /// <summary> Checks whether the control conforms to the required WAI level. </summary>
  /// <exception cref="WcagException"> Thrown if the control does not conform to the required WAI level. </exception>
  protected virtual void EvaluateWaiConformity ()
  {
    if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
    {
      if (DateTextBoxStyle.AutoPostBack == true)
        WcagHelper.Instance.HandleWarning (1, this, "DateTextBoxStyle.AutoPostBack");

      if (TimeTextBoxStyle.AutoPostBack == true)
        WcagHelper.Instance.HandleWarning (1, this, "TimeTextBoxStyle.AutoPostBack");

      if (DateTimeTextBoxStyle.AutoPostBack == true)
        WcagHelper.Instance.HandleWarning (1, this, "DateTimeTextBoxStyle.AutoPostBack");

      if (DateTextBox.AutoPostBack)
        WcagHelper.Instance.HandleWarning (1, this, "DateTextBox.AutoPostBack");

      if (TimeTextBox.AutoPostBack)
        WcagHelper.Instance.HandleWarning (1, this, "TimeTextBox.AutoPostBack");
    }

    if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelDoubleARequired())
    {
      if (ActualValueType == BocDateTimeValueType.DateTime)
        WcagHelper.Instance.HandleError (2, this, "ActualValueType");
    }
  }

  public override void PrepareValidation ()
  {
    base.PrepareValidation();

    if (! IsReadOnly)
    {
      SetEditModeDateValue ();
      SetEditModeTimeValue ();
    }
  }

  private void SetEditModeDateValue ()
  {
    if (ProvideMaxLength)
      _dateTextBox.MaxLength = GetDateMaxLength();
    _dateTextBox.Text = InternalDateValue;
  }

  private void SetEditModeTimeValue ()
  {
    if (ProvideMaxLength)
      _timeTextBox.MaxLength = GetTimeMaxLength();
    _timeTextBox.Text = InternalTimeValue;
  }

  protected override void OnPreRender (EventArgs e)
  {
    EnsureChildControls();
    base.OnPreRender (e);

    LoadResources (GetResourceManager());
    
    bool isReadOnly = IsReadOnly;

    //if (! HtmlHeadAppender.Current.IsRegistered (s_styleFileKey))
    //{
    //  string url = ResourceUrlResolver.GetResourceUrl (
    //      this, Context, typeof (BocDateTimeValue), ResourceType.Html, c_styleFileUrl);
    //  HtmlHeadAppender.Current.RegisterStylesheetLink (s_styleFileKey, url, HtmlHeadAppender.Priority.Library);
    //}

    if (IsReadOnly)
    {
      PreRenderReadOnlyValue();
    }
    else
    {
      PreRenderEditModeValueDate();
      PreRenderEditModeValueTime();
      PreRenderEditModeValueDatePicker();
    }
  }

  /// <summary> Gets a <see cref="HtmlTextWriterTag.Div"/> as the <see cref="WebControl.TagKey"/>. </summary>
  protected override HtmlTextWriterTag TagKey
  {
    get { return HtmlTextWriterTag.Div; }
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

    writer.AddStyleAttribute ("display", "inline");
  }

  protected override void RenderContents (HtmlTextWriter writer)
  {
    EvaluateWaiConformity ();

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
      bool isDateTextBoxHeightEmpty = _dateTextBox.Height.IsEmpty 
                                     && StringUtility.IsNullOrEmpty (_dateTextBox.Style["height"]);
      bool isTimeTextBoxHeightEmpty = _timeTextBox.Height.IsEmpty 
                                     && StringUtility.IsNullOrEmpty (_timeTextBox.Style["height"]);
      if (! isControlHeightEmpty && isDateTextBoxHeightEmpty && isTimeTextBoxHeightEmpty)
        writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      bool isControlWidthEmpty = Width.IsEmpty && StringUtility.IsNullOrEmpty (Style["width"]);
      bool isDateTextBoxWidthEmpty =    _dateTextBox.Width.IsEmpty 
                                     && StringUtility.IsNullOrEmpty (_dateTextBox.Style["width"]);
      bool isTimeTextBoxWidthEmpty =    _timeTextBox.Width.IsEmpty 
                                     && StringUtility.IsNullOrEmpty (_timeTextBox.Style["width"]);
      if (isDateTextBoxWidthEmpty && isTimeTextBoxWidthEmpty)
      {
        if (isControlWidthEmpty)
        {
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
        }
        else
        {
          if (! Width.IsEmpty)
            writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Width.ToString());
          else
            writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Style["width"]);
        }
      }

      writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
      writer.AddStyleAttribute ("display", "inline");
      writer.RenderBeginTag (HtmlTextWriterTag.Table);  // Begin table

      writer.RenderBeginTag (HtmlTextWriterTag.Tr);  // Begin tr

      bool hasDateField =    ActualValueType == BocDateTimeValueType.DateTime
                          || ActualValueType == BocDateTimeValueType.Date
                          || ActualValueType == BocDateTimeValueType.Undefined;
      bool hasTimeField =    ActualValueType == BocDateTimeValueType.DateTime
                          || ActualValueType == BocDateTimeValueType.Undefined;
      bool hasDatePicker =    hasDateField 
                           && (   _enableClientScript && IsDesignMode 
                               || _hasClientScript);

      string dateTextBoxSize = string.Empty;
      string timeTextBoxSize = string.Empty;
      if (hasDateField && hasTimeField && ShowSeconds)
      {
        dateTextBoxSize = "55%";
        timeTextBoxSize = "45%";
      }
      else if (hasDateField && hasTimeField)
      {
        dateTextBoxSize = "60%";
        timeTextBoxSize = "40%";
      }
      else if (hasDateField)
      {
        dateTextBoxSize = "100%";
      }

      if (hasDateField)
      {
        if (_dateTextBoxStyle.Width.IsEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, dateTextBoxSize);
        else
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, _dateTextBoxStyle.Width.ToString());
        writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td

        if (! isControlHeightEmpty && isDateTextBoxHeightEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
        if (! StringUtility.IsNullOrEmpty (dateTextBoxSize))
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        _dateTextBox.RenderControl (writer);  
      
        writer.RenderEndTag(); // End td
      }

      if (hasDatePicker)
      {
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        writer.AddStyleAttribute ("padding-left", "0.3em");
        writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td
        _datePickerButton.RenderControl (writer);  
        writer.RenderEndTag(); // End td
      }

      //HACK: Opera has problems with inline tables and may collapse contents unless a cell with width 0% is present
      if (! hasDatePicker && Context.Request.Browser.Browser == "Opera")
      {
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td
        writer.Write ("&nbsp;");
        writer.RenderEndTag(); // End td
      }

      if (hasTimeField)
      {
        if (_timeTextBoxStyle.Width.IsEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, timeTextBoxSize);
        else
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, _timeTextBoxStyle.Width.ToString());
        if (hasDateField)
          writer.AddStyleAttribute ("padding-left", "0.3em");
        writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td

        if (! isControlHeightEmpty && isTimeTextBoxHeightEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
        if (! StringUtility.IsNullOrEmpty (timeTextBoxSize))
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        _timeTextBox.RenderControl (writer);  
      
        writer.RenderEndTag(); // End td
      }

      writer.RenderEndTag(); // End tr
      writer.RenderEndTag(); // End table
    }
  }

  protected override void LoadControlState (object savedState)
  {
    object[] values = (object[]) savedState;

    base.LoadControlState (values[0]);

    if ( values[1] != null)
      _internalDateValue = (string) values[1];
    if ( values[2] != null)
      _internalTimeValue = (string) values[2];
    _valueType = (BocDateTimeValueType) values[3];
    _actualValueType = (BocDateTimeValueType) values[4];
    _showSeconds = (bool) values[5];
    _provideMaxLength = (bool) values[6];
    _savedDateTimeValue = (DateTime?) values[7];

    _dateTextBox.Text = _internalDateValue;
    _timeTextBox.Text = _internalTimeValue;
  }

  protected override object SaveControlState ()
  {
    object[] values = new object[8];

    values[0] = base.SaveControlState ();
    values[1] = _internalDateValue;
    values[2] = _internalTimeValue;
    values[3] = _valueType;
    values[4] = _actualValueType;
    values[5] = _showSeconds;
    values[6] = _provideMaxLength;
    values[7] = _savedDateTimeValue;

    return values;
  }


  /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
  /// <include file='doc\include\UI\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/LoadValue/*' />
  public override void LoadValue (bool interim)
  {
    if (! interim)
    {
      if (Property != null && DataSource != null && DataSource.BusinessObject != null)
      {
        DateTime? value = (DateTime?) DataSource.BusinessObject.GetProperty (Property);
        LoadValueInternal (value, interim);
      }
    }
  }

  /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
  /// <include file='doc\include\UI\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/LoadUnboundValue/*' />
  public void LoadUnboundValue (DateTime? value, bool interim)
  {
    LoadValueInternal (value, interim);
  }

  /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
  protected virtual void LoadValueInternal (DateTime? value, bool interim)
  {
    if (! interim)
    {
      Value = value;
      IsDirty = false;
    }
  }

  /// <summary> Saves the <see cref="Value"/> into the bound <see cref="IBusinessObject"/>. </summary>
  /// <include file='doc\include\UI\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/SaveValue/*' />
  public override void SaveValue (bool interim)
  {
    if (!interim && IsDirty)
    {
      if (Property != null && DataSource != null && DataSource.BusinessObject != null && ! IsReadOnly)
      {
        DataSource.BusinessObject.SetProperty (Property, Value);
        //  get_Value parses the internal representation of the date/time value
        //  set_Value updates the internal representation of the date/time value
        Value = Value;
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
  /// <include file='doc\include\UI\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/CreateValidators/*' />
  public override BaseValidator[] CreateValidators()
  {
    if (IsReadOnly)
      return new BaseValidator[0];

    BaseValidator[] validators = new BaseValidator[1];

    BocDateTimeValueValidator dateTimeValueValidator = new BocDateTimeValueValidator();
    dateTimeValueValidator.ID = ID + "_ValidatorDateTime";
    dateTimeValueValidator.ControlToValidate = ID;
    if (StringUtility.IsNullOrEmpty (_errorMessage))
    {
      IResourceManager resourceManager = GetResourceManager();
      dateTimeValueValidator.RequiredErrorMessage = 
          resourceManager.GetString (ResourceIdentifier.RequiredErrorMessage);
      dateTimeValueValidator.IncompleteErrorMessage = 
          resourceManager.GetString (ResourceIdentifier.IncompleteErrorMessage);
      dateTimeValueValidator.InvalidDateAndTimeErrorMessage = 
          resourceManager.GetString (ResourceIdentifier.InvalidDateAndTimeErrorMessage);
      dateTimeValueValidator.InvalidDateErrorMessage = 
          resourceManager.GetString (ResourceIdentifier.InvalidDateErrorMessage);
      dateTimeValueValidator.InvalidTimeErrorMessage = 
          resourceManager.GetString (ResourceIdentifier.InvalidTimeErrorMessage);
    }
    else
    {
      dateTimeValueValidator.ErrorMessage = _errorMessage;
    }
    validators[0] = dateTimeValueValidator;

    _validators.AddRange (validators);
    return validators;
  }

  /// <summary> Prerenders the <see cref="Label"/>. </summary>
  private void PreRenderReadOnlyValue()
  {
    if (IsDesignMode && StringUtility.IsNullOrEmpty (_label.Text))
    {
      _label.Text = c_designModeEmptyLabelContents;
      //  Too long, can't resize in designer to less than the content's width
      //  _label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
    }
    else
    {
      object internalValue = Value;

      if (internalValue != null)
      {
        DateTime dateTime = (DateTime) internalValue;

        if (ActualValueType == BocDateTimeValueType.DateTime)
          _label.Text = FormatDateTimeValue (dateTime, true);
        else if (ActualValueType == BocDateTimeValueType.Date)
          _label.Text = FormatDateValue (dateTime, true);
        else
          _label.Text = dateTime.ToString();
      }
      else
      {
        _label.Text = "&nbsp;";
      }
    }

    _label.Height = Unit.Empty;
    _label.Width = Unit.Empty;
    _label.ApplyStyle (_commonStyle);
    _label.ApplyStyle (_labelStyle);
  }

  /// <summary> Prerenders the <see cref="DateTextBox"/>. </summary>
  private void PreRenderEditModeValueDate()
  {
    SetEditModeDateValue ();
    _dateTextBox.ReadOnly = ! Enabled;
    _dateTextBox.Width = Unit.Empty;
    _dateTextBox.Height = Unit.Empty;
    _dateTextBox.ApplyStyle (_commonStyle);
    _dateTimeTextBoxStyle.ApplyStyle (_dateTextBox);
    _dateTextBoxStyle.ApplyStyle (_dateTextBox);
  }

  /// <summary> Prerenders the <see cref="TimeTextBox"/>. </summary>
  private void PreRenderEditModeValueTime()
  {
    SetEditModeTimeValue ();
    _timeTextBox.ReadOnly = ! Enabled;
    _timeTextBox.Height = Unit.Empty;
    _timeTextBox.Width = Unit.Empty;
    _timeTextBox.ApplyStyle (_commonStyle);
    _dateTimeTextBoxStyle.ApplyStyle (_timeTextBox);
    _timeTextBoxStyle.ApplyStyle (_timeTextBox);
  }

  /// <summary> Prerenders the date picker. </summary>
  private void PreRenderEditModeValueDatePicker()
  {
    DetermineClientScriptLevel();

    //  TODO: BocDateTimeValue: When creating a DatePickerButton, move this block into the button
    //  and remove RenderContents.
    if (   _enableClientScript && IsDesignMode 
        || _hasClientScript)
    {
      string imageUrl = ResourceUrlResolver.GetResourceUrl (
        this, Context, typeof (BocDateTimeValue), ResourceType.Image, DatePickerImageUrl);
      if (imageUrl == null)
        _datePickerButton.ImageUrl = DatePickerImageUrl;  
      else
        _datePickerButton.ImageUrl = imageUrl; 
      _datePickerButton.Text = GetResourceManager().GetString (ResourceIdentifier.DataPickerButtonAlternateText);

      string script;
      if (_hasClientScript && Enabled)
      {
        string pickerActionButton = "this";
        string pickerActionContainer = "document.getElementById ('" + ClientID + "')";
        string pickerActionTarget = "document.getElementById ('" + _dateTextBox.ClientID + "')";
        
        string pickerUrl = "'" + ResourceUrlResolver.GetResourceUrl (
            this, Context, typeof (DatePickerPage), ResourceType.UI, c_datePickerPopupForm) + "'";
        
        Unit popUpWidth = _datePickerPopupWidth;
        if (popUpWidth.IsEmpty)
          popUpWidth = Unit.Point (c_defaultDatePickerLengthInPoints);
        string pickerWidth = "'" + popUpWidth.ToString() + "'";
        
        Unit popUpHeight = _datePickerPopupHeight;
        if (popUpHeight.IsEmpty)
          popUpHeight = Unit.Point (c_defaultDatePickerLengthInPoints);
        string pickerHeight = "'" + popUpHeight.ToString() + "'";

        script = "DatePicker_ShowDatePicker("
            + pickerActionButton + ", "
            + pickerActionContainer + ", "
            + pickerActionTarget + ", "
            + pickerUrl + ", "
            + pickerWidth + ", "
            + pickerHeight + ");"
            + "return false;";
       }
      else
      {
        script = "return false;";
      }
      _datePickerButton.Attributes.Add ("href", "#");
      _datePickerButton.Attributes["onclick"] = script;
    }

    _datePickerButton.Style["padding"] = "0px";
    _datePickerButton.Style["border"] = "none";
    _datePickerButton.Style["background-color"] = "transparent";
    _datePickerButton.ApplyStyle (_datePickerButtonStyle);
  }


  /// <summary> Formats the <see cref="DateTime"/> value according to the current culture. </summary>
  /// <param name="dateValue"> The <see cref="DateTime"/> value to be formatted. </param>
  /// <param name="isReadOnly"> <see langword="true"/> if the control is in read-only mode. </param>
  /// <returns> A formatted string representing the <see cref="DateTime"/> value. </returns>
  protected virtual string FormatDateTimeValue (DateTime dateValue, bool isReadOnly)
  {
    isReadOnly = false;

    if (isReadOnly)
    {
      if (ShowSeconds)
      {
        //  F:  dddd, MMMM dd yyyy, hh, mm, ss
        return dateValue.ToString ("F");
      }
      else
      {
        //  f:  dddd, MMMM dd yyyy, hh, mm
        return dateValue.ToString ("f");
      }
    }
    else
    {
      if (ShowSeconds)
      {
        //  G:  yyyy, mm, dd, hh, mm, ss
        return dateValue.ToString ("G");
      }
      else
      {
        //  g:  yyyy, mm, dd, hh, mm
        return dateValue.ToString ("g");
      }
    }
  }

  /// <summary> Formats the <see cref="DateTime"/> value according to the current culture. </summary>
  /// <param name="dateValue"> The <see cref="DateTime"/> value to be formatted. </param>
  /// <returns> A formatted string representing the <see cref="DateTime"/> value. </returns>
  protected virtual string FormatDateTimeValue (DateTime dateValue)
  {
    return FormatDateTimeValue (dateValue, false);
  }

  /// <summary> Formats the <see cref="DateTime"/> value's date component according to the current culture. </summary>
  /// <param name="dateValue"> The <see cref="DateTime"/> value to be formatted. </param>
  /// <param name="isReadOnly"> <see langword="true"/> if the control is in read-only mode. </param>
  /// <returns> A formatted string representing the <see cref="DateTime"/> value's date component. </returns>
  protected virtual string FormatDateValue (DateTime dateValue, bool isReadOnly)
  {
    isReadOnly = false;

    if (isReadOnly)
    {
      //  D:  dddd, MMMM dd yyyy
      return dateValue.ToString ("D");
    }
    else
    {
      //  d:  yyyy, mm, dd
      return dateValue.ToString ("d");
    }
  }

  /// <summary> Formats the <see cref="DateTime"/> value's date component according to the current culture. </summary>
  /// <param name="dateValue"> The <see cref="DateTime"/> value to be formatted. </param>
  /// <returns> A formatted string representing the <see cref="DateTime"/> value's date component. </returns>
  protected virtual string FormatDateValue (DateTime dateValue)
  {
    return FormatDateValue (dateValue, false);
  }

  /// <summary> Formats the <see cref="DateTime"/> value's time component according to the current culture. </summary>
  /// <param name="timeValue"> The <see cref="DateTime"/> value to be formatted. </param>
  /// <param name="isReadOnly"> <see langword="true"/> if the control is in read-only mode. </param>
  /// <returns>  A formatted string representing the <see cref="DateTime"/> value's time component. </returns>
  protected virtual string FormatTimeValue (DateTime timeValue, bool isReadOnly)
  {
    //  ignore Read-Only

    if (ShowSeconds)
    {
      //  T: hh, mm, ss
      return timeValue.ToString ("T");
    }
    else
    {
      //  T: hh, mm
      return timeValue.ToString ("t");
    }
  }

  /// <summary> Formats the <see cref="DateTime"/> value's time component according to the current culture. </summary>
  /// <param name="timeValue"> The <see cref="DateTime"/> value to be formatted. </param>
  /// <returns> A formatted string representing the <see cref="DateTime"/> value's time component.  </returns>
  protected virtual string FormatTimeValue (DateTime timeValue)
  {
    return FormatTimeValue (timeValue, false);
  }

  /// <summary> Calculates the maximum length for required for entering the date component. </summary>
  /// <returns> The length. </returns>
  protected virtual int GetDateMaxLength()
  {
    string maxDate = new DateTime (2000, 12, 31).ToString ("d");
    return maxDate.Length;
  }

  /// <summary> Calculates the maximum length for required for entering the time component. </summary>
  /// <returns> The length. </returns>
  protected virtual int GetTimeMaxLength()
  {
    string maxTime = "";

    if (ShowSeconds)
      maxTime = new DateTime (1, 1, 1, 23, 30, 30).ToString ("T");
    else
      maxTime = new DateTime (1, 1, 1, 23, 30, 30).ToString ("t");

    return maxTime.Length;
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
  ///   <see cref="IBusinessObjectBoundControl.Property"/>.
  /// </summary>
  private void RefreshPropertiesFromObjectModel()
  {
    if (Property != null)
    {
      if (_valueType == BocDateTimeValueType.Undefined)
        _actualValueType = GetBocDateTimeValueType (Property);
    }
  }

  /// <summary>
  ///   Evaluates the type of <paramref name="property"/> and returns the appropriate <see cref="BocDateTimeValueType"/>.
  /// </summary>
  /// <param name="property"> The <see cref="IBusinessObjectProperty"/> to be evaluated. </param>
  /// <returns> The matching <see cref="BocDateTimeValueType"/></returns>
  private BocDateTimeValueType GetBocDateTimeValueType (IBusinessObjectProperty property)
  {
    if (property == null)
      return BocDateTimeValueType.Undefined;

    IBusinessObjectDateTimeProperty dateTimeProperty = property as IBusinessObjectDateTimeProperty;
    if (dateTimeProperty == null)
      throw new NotSupportedException ("BocDateTimeValue does not support property type " + property.GetType());

    if (dateTimeProperty.Type == DateTimeType.Date)
      return BocDateTimeValueType.Date;
    else
      return BocDateTimeValueType.DateTime;
  }

  private void DetermineClientScriptLevel() 
  {
    _hasClientScript = false;

    if (! IsDesignMode)
    {
      if (EnableClientScript) 
      {
        bool isVersionGreaterOrEqual55 = 
               Context.Request.Browser.MajorVersion >= 6
            ||    Context.Request.Browser.MajorVersion == 5 
               && Context.Request.Browser.MinorVersion >= 0.5;
        bool isInternetExplorer55AndHigher = 
            Context.Request.Browser.Browser == "IE" && isVersionGreaterOrEqual55;

        _hasClientScript = isInternetExplorer55AndHigher;

        // // The next set of checks involve looking at the capabilities of the
        // // browser making the request.
        // //
        // // The DatePicker needs to verify whether the browser has EcmaScript (JavaScript)
        // // version 1.2+, and whether the browser supports DHTML, and optionally,
        // // DHTML behaviors.
        //
        // HttpBrowserCapabilities browserCaps = Page.Request.Browser;
        // bool hasEcmaScript = (browserCaps.EcmaScriptVersion.CompareTo(new Version(1, 2)) >= 0);
        // bool hasDOM = (browserCaps.MSDomVersion.Major >= 4);
        // bool hasBehaviors = (browserCaps.MajorVersion > 5) ||
        //                     ((browserCaps.MajorVersion == 5) && (browserCaps.MinorVersion >= .5));
        //
        // _hasClientScript = hasEcmaScript && hasDOM;
      }
    }
  }

  /// <summary> Gets or sets the current value. </summary>
  /// <value> 
  ///   The value has the type specified in the <see cref="ActualValueType"/> property. If the parsing fails,
  ///   <see langword="null"/> is returned.
  /// </value>
  /// <remarks> The dirty state is reset when the value is set. </remarks>
  [Browsable(false)]
  public new DateTime? Value
  {
    get 
    {
      if (InternalDateValue == null && InternalTimeValue == null)
        return null;

      DateTime dateTimeValue = DateTime.MinValue;

      //  Parse Date

      if (   InternalDateValue == null
          && ActualValueType != BocDateTimeValueType.Undefined)
      {
        //throw new FormatException ("The date component of the DateTime value is null.");
        return null;
      }

      try
      {
        if (   ! IsDesignMode
            || ! StringUtility.IsNullOrEmpty (InternalDateValue))
        {
          dateTimeValue = DateTime.Parse (InternalDateValue).Date;
        }
      }
      catch (FormatException)
      {
        //throw new FormatException ("Error while parsing the date component (value: '" + InternalDateValue+ "') of the DateTime value. " + ex.Message);
        return null;
      }
      catch (IndexOutOfRangeException)
      {
        return null;
      }


      //  Parse Time

      if (   (   ActualValueType == BocDateTimeValueType.DateTime
              || ActualValueType == BocDateTimeValueType.Undefined)
          && InternalTimeValue != null)
      {        
        try
        {
          if (   ! IsDesignMode
              || ! StringUtility.IsNullOrEmpty (InternalTimeValue))
          {
            dateTimeValue = dateTimeValue.Add (DateTime.Parse (InternalTimeValue).TimeOfDay);
          }
        }
        catch (FormatException)
        {
          //throw new FormatException ("Error while parsing the time component (value: '" + InternalTimeValue+ "')of the DateTime value. " + ex.Message);
          return null;
        }
        catch (IndexOutOfRangeException)
        {
          return null;
        }

          //  Restore the seconds if the control does not display them.
          if (   ! ShowSeconds
              && _savedDateTimeValue.HasValue)
          {
            dateTimeValue = dateTimeValue.AddSeconds (_savedDateTimeValue.Value.Second);
          }
      }
      else if (    ActualValueType == BocDateTimeValueType.Date
                && _savedDateTimeValue.HasValue)
      {
        //  Restore the time if the control is displayed in date mode.
        dateTimeValue = dateTimeValue.Add (_savedDateTimeValue.Value.TimeOfDay);
      }

      return dateTimeValue;
    }
    set 
    {
      IsDirty = true;
      _savedDateTimeValue = value;

      if (!_savedDateTimeValue.HasValue)
      {
        InternalDateValue = null;
        InternalTimeValue = null;
        return;
      }

      try
      {
        InternalDateValue = FormatDateValue (_savedDateTimeValue.Value);
      }
      catch  (InvalidCastException e)
      {
        throw new ArgumentException ("Expected type '" + _actualValueType.ToString() + "', but was '" + value.GetType().FullName + "'.", "value", e);
      }

      if (   ActualValueType == BocDateTimeValueType.DateTime
          || ActualValueType == BocDateTimeValueType.Undefined)
      {
        try
        {
          InternalTimeValue = FormatTimeValue (_savedDateTimeValue.Value);
        }
        catch  (InvalidCastException e)
        {
          throw new ArgumentException ("Expected type '" + _actualValueType.ToString() + "', but was '" + value.GetType().FullName + "'.", "value", e);
        }
      }
      else
      {
        InternalTimeValue = null;
      }
    }
  }

  /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = ArgumentUtility.CheckType<DateTime?> ("value", value); }
  }

  /// <summary> Gets or sets the string displayed in the <see cref="DateTextBox"/>. </summary>
  protected virtual string InternalDateValue
  {
    get{ return _internalDateValue; }
    set { _internalDateValue = value; }
  }

  /// <summary> Gets or sets the string displayed in the <see cref="TimeTextBox"/>. </summary>
  protected virtual string InternalTimeValue
  {
    get{ return _internalTimeValue; }
    set { _internalTimeValue = value; }
  }

  /// <summary> 
  ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user 
  ///   interface.
  /// </summary>
  /// <returns> 
  ///   A <see cref="String"/> <see cref="Array"/> containing the <see cref="Control.ClientID"/> of the
  ///   <see cref="DateTextBox"/> and the <see cref="TimeTextBox"/> if the control is in edit mode, or an empty array 
  ///   if the control is read-only.
  /// </returns>
  /// <seealso cref="BusinessObjectBoundEditableWebControl.GetTrackedClientIDs">BusinessObjectBoundEditableWebControl.GetTrackedClientIDs</seealso>
  public override string[] GetTrackedClientIDs()
  {
    if (IsReadOnly)
    {
      return new string[0];
    }
    else if (   ActualValueType == BocDateTimeValueType.DateTime
             || ActualValueType == BocDateTimeValueType.Undefined)
    {
      return new string[2] { _dateTextBox.ClientID, _timeTextBox.ClientID };
    }
    else if (ActualValueType == BocDateTimeValueType.Date)
    {
      return new string[1] { _dateTextBox.ClientID} ;
    }
    else
    {
      return new string[0];
    }
  }

  /// <summary> The <see cref="BocDateTimeValue"/> supports only scalar properties. </summary>
  /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="false"/>. </returns>
  /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
  protected override bool SupportsPropertyMultiplicity (bool isList)
  {
    return ! isList;
  }

  /// <summary>
  ///   The <see cref="BocDateTimeValue"/> supports properties of types <see cref="IBusinessObjectDateTimeProperty"/>.
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
  /// <remarks> Returns the <see cref="DateTextBox"/> if the control is in edit mode, otherwise the control itself. </remarks>
  public override Control TargetControl
  {
    get { return IsReadOnly ? (Control) this : _dateTextBox; }
  }

  /// <summary> Gets the ID of the element to receive the focus when the page is loaded. </summary>
  /// <value>
  ///   Returns the <see cref="Control.ClientID"/> of the <see cref="DateTextBox"/> if the control is in edit mode, 
  ///   otherwise <see langword="null"/>. 
  /// </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public string FocusID
  { 
    get { return IsReadOnly ? null : _dateTextBox.ClientID; }
  }

  /// <summary>
  ///   Gets the style that you want to apply to the <see cref="DateTextBox"/> and the <see cref="TimeTextBox"/> 
  ///   (edit mode) and the <see cref="Label"/> (read-only mode).
  /// </summary>
  /// <include file='doc\include\UI\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/CommonStyle/*' />
  [Category("Style")]
  [Description("The style that you want to apply to the date and the time TextBoxes (edit mode) and the Label (read-only mode).")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style CommonStyle
  {
    get { return _commonStyle; }
  }

  /// <summary>
  ///   Gets the style that you want to apply to both the <see cref="DateTextBox"/> and the <see cref="TimeTextBox"/>
  ///   (edit mode) only.
  /// </summary>
  /// <include file='doc\include\UI\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/DateTimeTextBoxStyle/*' />
  [Category("Style")]
  [Description("The style that you want to apply to both the date and the time TextBoxes (edit mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public SingleRowTextBoxStyle DateTimeTextBoxStyle
  {
    get { return _dateTimeTextBoxStyle; }
  }

  /// <summary> Gets the style that you want to apply to the <see cref="DateTextBox"/> (edit mode) only. </summary>
  /// <remarks> These style settings override the styles defined in <see cref="DateTimeTextBoxStyle"/>. </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to only the date TextBox (edit mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public SingleRowTextBoxStyle DateTextBoxStyle
  {
    get { return _dateTextBoxStyle; }
  }

  /// <summary> Gets the style that you want to apply to the <see cref="TimeTextBox"/> (edit mode) only. </summary>
  /// <remarks> These style settings override the styles defined in <see cref="DateTimeTextBoxStyle"/>. </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to only the time TextBox (edit mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public SingleRowTextBoxStyle TimeTextBoxStyle
  {
    get { return _timeTextBoxStyle; }
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

  /// <summary> Gets the style that you want to apply to the <see cref="DatePickerButton"/> (edit mode) only. </summary>
  /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to the Button (edit mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style ButtonStyle
  {
    get { return _datePickerButtonStyle; }
  }

  /// <summary> Gets or sets the width of the IFrame used to display the date picker. </summary>
  /// <value> The <see cref="Unit"/> value used for the width. The default value is <b>150pt</b>. </value>
  [Category ("Appearance")]
  [Description("The width of the IFrame used to display the date picker.")]
  [DefaultValue (typeof (Unit), "150pt")]
  public Unit DatePickerPopupWidth
  {
    get { return _datePickerPopupWidth; }
    set { _datePickerPopupWidth = value; }
  }

  /// <summary> Gets or sets the height of the IFrame used to display the date picker. </summary>
  /// <value> The <see cref="Unit"/> value used for the height. The default value is <b>150pt</b>. </value>
  [Category ("Appearance")]
  [Description("The height of the IFrame used to display the date picker.")]
  [DefaultValue (typeof (Unit), "150pt")]
  public Unit DatePickerPopupHeight
  {
    get { return _datePickerPopupHeight; }
    set { _datePickerPopupHeight = value; }
  }

  /// <summary> Gets or sets a flag that determines whether to display the seconds. </summary>
  /// <value> <see langword="true"/> to enable the seconds. The default value is <see langword="false"/>. </value>
  [Category ("Appearance")]
  [Description ("True to display the seconds. ")]
  [DefaultValue (false)]
  public bool ShowSeconds
  {
    get { return _showSeconds; }
    set { _showSeconds = value; }
  }

  /// <summary> Gets or sets a flag that determines whether to apply an automatic maximum length to the text boxes. </summary>
  /// <value> <see langword="true"/> to enable the maximum length. The default value is <see langword="true"/>. </value>
  [Category ("Behavior")]
  [Description (" True to automatically limit the maxmimum length of the date and time input fields. ")]
  [DefaultValue (true)]
  public bool ProvideMaxLength
  {
    get { return _provideMaxLength; }
    set { _provideMaxLength = value; }
  }

  /// <summary> Gets or sets a flag that determines whether the client script is enabled. </summary>
  /// <value> <see langword="true"/> to enable the client script. The default value is <see langword="true"/>. </value>
  [Category ("Behavior")]
  [Description (" True to enable the client script for the pop-up calendar. ")]
  [DefaultValue (true)]
  public bool EnableClientScript
  {
    get { return _enableClientScript; }
    set { _enableClientScript = value; }
  }

  /// <summary> Gets or sets the <see cref="BocDateTimeValueType"/> assigned from an external source. </summary>
  /// <value> 
  ///   The externally set <see cref="BocDateTimeValueType"/>. The default value is 
  ///   <see cref="BocDateTimeValueType.Undefined"/>. 
  /// </value>
  [Description("Gets or sets a fixed value type.")]
  [Category ("Data")]
  [DefaultValue (BocDateTimeValueType.Undefined)]
  public BocDateTimeValueType ValueType
  {
    get 
    {
      return _valueType; 
    }
    set 
    {
      if (_valueType != value)
      {
        _valueType = value;
        _actualValueType = value;
        if (_valueType != BocDateTimeValueType.Undefined)
        {
          InternalDateValue = string.Empty;
          InternalTimeValue = string.Empty;
        }
      }
    }
  }

  /// <summary> Gets the <see cref="BocDateTimeValueType"/> actually used by the cotnrol. </summary>
  [Browsable (false)]
  public BocDateTimeValueType ActualValueType
  {
    get 
    {
      RefreshPropertiesFromObjectModel();
      return _actualValueType; 
    }
  }

  /// <summary> Gets the <see cref="TextBox"/> used in edit mode for the date component. </summary>
  [Browsable (false)]
  public TextBox DateTextBox
  {
    get { return _dateTextBox; }
  }

  /// <summary> Gets the <see cref="TextBox"/> used in edit mode for the time component. </summary>
  [Browsable (false)]
  public TextBox TimeTextBox
  {
    get { return _timeTextBox; }
  }

  /// <summary> Gets the <see cref="Label"/> used in read-only mode. </summary>
  [Browsable (false)]
  public Label Label
  {
    get { return _label; }
  }

  /// <summary> Gets the <see cref="HyperLink"/> used in edit mode for opening the date picker. </summary>
  [Browsable (false)]
  public HyperLink DatePickerButton
  {
    get { return _datePickerButton; }
  }

  /// <summary> The URL of the image used by the <see cref="DatePickerButton"/>. </summary>
  [Browsable (false)]
  protected virtual string DatePickerImageUrl
  {
    get { return "DatePicker.gif"; }
  }

  /// <summary>
  ///   Gets the contents of the <see cref="DateTextBox"/> and the <see cref="TimeTextBox"/>, 
  ///   seperated by a newline character.
  /// </summary>
  /// <remarks> This property is used for validation. </remarks>
  [Browsable (false)]
  public string ValidationValue
  {
    get
    {
      if (ActualValueType == BocDateTimeValueType.DateTime)
        return InternalDateValue + "\n" + InternalTimeValue;
      else if (ActualValueType == BocDateTimeValueType.Date)
        return InternalDateValue + "\n" + "";
      else
        return "\n";

    }
  }

  /// <summary> This event is fired when the date or time is changed between postbacks. </summary>
  [Category ("Action")]
  [Description ("Fires when the value of the control has changed.")]
  public event EventHandler DateTimeChanged
  {
    add { Events.AddHandler (s_dateTimeChangedEvent, value); }
    remove { Events.RemoveHandler (s_dateTimeChangedEvent, value); }
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
  /// <summary> Gets the CSS-Class applied to the <see cref="BocDateTimeValue"/> itself. </summary>
  /// <remarks> 
  ///   <para> Class: <c>bocDateTimeValue</c>. </para>
  ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassBase
  { get { return "bocDateTimeValue"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocDateTimeValue"/> when it is displayed in read-only mode. </summary>
  /// <remarks> 
  ///   <para> Class: <c>readOnly</c>. </para>
  ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocDateTimeValue.readOnly</c> as a selector. </para>
  /// </remarks>
  protected virtual string CssClassReadOnly
  { get { return "readOnly"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocDateTimeValue"/> when it is displayed disabled. </summary>
  /// <remarks> 
  ///   <para> Class: <c>disabled</c>. </para>
  ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocDateTimeValue.disabled</c> as a selector. </para>
  /// </remarks>
  protected virtual string CssClassDisabled
  { get { return "disabled"; } }
  #endregion
}

/// <summary> A list possible data types for the <see cref="BocDateTimeValue"/> </summary>
public enum BocDateTimeValueType
{
  /// <summary> No formatting applied. </summary>
  Undefined,
  /// <summary> The value is displayed as a date and time value. </summary>
  DateTime,
  /// <summary> Only the date component is displayed. </summary>
  Date
}

}
