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
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
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

/// <summary> This control can be used to display or edit enumeration values. </summary>
/// <include file='doc\include\UI\Controls\BocEnumValue.xml' path='BocEnumValue/Class/*' />
[ValidationProperty ("Value")]
[DefaultEvent ("SelectionChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocEnumValue: BusinessObjectBoundEditableWebControl, IPostBackDataHandler, IFocusableControl
{
	// constants

  private const string c_nullIdentifier = "==null==";

  /// <summary> The text displayed when control is displayed in desinger, is read-only, and has no contents. </summary>
  private const string c_designModeEmptyLabelContents = "##";
  private const string c_defaultListControlWidth = "150pt";

  // types

  /// <summary> A list of control specific resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
  ///   See the documentation of <b>GetString</b> for further details.
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Remotion.ObjectBinding.Web.Globalization.BocEnumValue")]
  protected enum ResourceIdentifier
  {
    /// <summary> The text rendered for the null item in the list. </summary>
    UndefinedItemText,
    /// <summary> The validation error message displayed when the null item is selected. </summary>
    NullItemValidationMessage
  }

  // static members
	
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { typeof (IBusinessObjectEnumerationProperty) };

  private static readonly object s_selectionChangedEvent = new object();

	// member fields

  private ListControl _listControl;
  private Label _label;

  private object _value = null;
  private string _internalValue = null;
  private string _oldInternalValue = null;
  private IEnumerationValueInfo _enumerationValueInfo = null;
 
  private Style _commonStyle;
  private ListControlStyle _listControlStyle;
  private Style _labelStyle;

  private string _undefinedItemText = string.Empty;

  /// <summary> State field for special behaviour during load control state. </summary>
  /// <remarks> Used by <see cref="RefreshEnumListSelectedValue"/>. </remarks>
  private bool _isExecutingLSaveControlState;
  bool _isEnumListRefreshed = false;

  private string _errorMessage;
  private ArrayList _validators;

  // construction and disposing

	public BocEnumValue()
	{
    _commonStyle = new Style ();
    _listControlStyle = new ListControlStyle ();
    _labelStyle = new Style ();
    _label = new Label();
    _validators = new ArrayList();
	}

	// methods and properties

  /// <remarks>
  ///   If the <see cref="ListControl"/> could not be created from <see cref="ListControlStyle"/>,
  ///   the control is set to read-only.
  /// </remarks>
  protected override void CreateChildControls()
  {
    _listControl = _listControlStyle.Create (false);
    if (_listControl == null)
    {
      _listControl = new DropDownList();
      ReadOnly = true;
    }
    _listControl.ID = ID + "_Boc_ListControl";
    _listControl.EnableViewState = false;
    ((IStateManager) _listControl.Items).TrackViewState();
    Controls.Add (_listControl);

    _label.ID = ID + "_Boc_Label";
    _label.EnableViewState = false;
    Controls.Add (_label);

  }

  protected override void OnInit (EventArgs e)
  {
    base.OnInit (e);
    Binding.BindingChanged += new EventHandler (Binding_BindingChanged);
    if (!IsDesignMode)
      Page.RegisterRequiresPostBack (this);
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
  /// <include file='doc\include\UI\Controls\BocEnumValue.xml' path='BocEnumValue/LoadPostData/*' />
  protected virtual bool LoadPostData (string postDataKey, NameValueCollection postCollection)
  {
    string newValue = PageUtility.GetPostBackCollectionItem (Page, _listControl.UniqueID);
    bool isDataChanged = false;
    if (newValue != null)
    {
      if (_internalValue == null && newValue != c_nullIdentifier)
        isDataChanged = true;
      else if (_internalValue != null && newValue != _internalValue)
        isDataChanged = true;
    }

    if (isDataChanged)
    {
      if (newValue == c_nullIdentifier)
        InternalValue = null;
      else
        InternalValue = newValue;
      IsDirty = true;
    }
    return isDataChanged;
  }

  /// <summary> Called when the state of the control has changed between postbacks. </summary>
  protected virtual void RaisePostDataChangedEvent()
  {
    RefreshEnumListSelectedValue();
    if (! IsReadOnly && Enabled)
      OnSelectionChanged();
  }

  /// <summary> Fires the <see cref="SelectionChanged"/> event. </summary>
  protected virtual void OnSelectionChanged()
  {
    EventHandler eventHandler = (EventHandler) Events[s_selectionChangedEvent];
    if (eventHandler != null)
      eventHandler (this, EventArgs.Empty);
  }

  /// <summary> Checks whether the control conforms to the required WAI level. </summary>
  /// <exception cref="Remotion.Web.UI.WcagException"> Thrown if the control does not conform to the required WAI level. </exception>
  protected virtual void EvaluateWaiConformity ()
  {
    if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
    {
      if (ListControlStyle.AutoPostBack == true)
        WcagHelper.Instance.HandleWarning (1, this, "ListControlStyle.AutoPostBack");

      if (ListControl.AutoPostBack)
        WcagHelper.Instance.HandleWarning (1, this, "ListControl.AutoPostBack");
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
    EnsureEnumListRefreshed (false);
    RefreshEnumListSelectedValue ();
  }

  protected override void OnPreRender (EventArgs e)
  {
    EnsureChildControls();
    base.OnPreRender (e);

    LoadResources (GetResourceManager());

    SetEditModeValue ();

    if (IsReadOnly)
    {
      string text = null;
      if (IsDesignMode && StringUtility.IsNullOrEmpty (_label.Text))
      {
        text = c_designModeEmptyLabelContents;
        //  Too long, can't resize in designer to less than the content's width
        //  _label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
      }
      else if (! IsDesignMode && EnumerationValueInfo != null)
      {
        text = EnumerationValueInfo.DisplayName;
      }

      if (StringUtility.IsNullOrEmpty (text))
        _label.Text = "&nbsp;";
      else
        _label.Text = text;

      _label.Width = Unit.Empty;
      _label.Height = Unit.Empty;
      _label.ApplyStyle (_commonStyle);
      _label.ApplyStyle (_labelStyle);
    }
    else
    {
      _listControl.Enabled = Enabled;

      _listControl.Width = Unit.Empty;
      _listControl.Height = Unit.Empty;
      _listControl.ApplyStyle (_commonStyle);
      _listControlStyle.ApplyStyle (_listControl);
    }
  }

  /// <summary> Gets a <see cref="HtmlTextWriterTag.Div"/> as the <see cref="WebControl.TagKey"/>. </summary>
  protected override HtmlTextWriterTag TagKey
  {
    get { return HtmlTextWriterTag.Div; }
  }

  protected override void AddAttributesToRender (HtmlTextWriter writer)
  {
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

    writer.AddStyleAttribute ("display", "inline");
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
      bool isListControlHeightEmpty = _listControl.Height.IsEmpty 
          && StringUtility.IsNullOrEmpty (_listControl.Style["height"]);
      if (! isControlHeightEmpty && isListControlHeightEmpty)
        writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      bool isControlWidthEmpty = Width.IsEmpty && StringUtility.IsNullOrEmpty (Style["width"]);
      bool isListControlWidthEmpty = _listControl.Width.IsEmpty 
          && StringUtility.IsNullOrEmpty (_listControl.Style["width"]);
      if (isListControlWidthEmpty)
      {
        if (isControlWidthEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultListControlWidth);
        else
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      }

      _listControl.RenderControl (writer);
    }
  }

  protected override void LoadControlState (object savedState)
  {
    _isExecutingLSaveControlState = true;

    object[] values = (object[]) savedState;

    base.LoadControlState (values[0]);
    _value = values[1];
    if (values[2] != null)
      _internalValue = (string) values[2];
    ((IStateManager) _listControl.Items).LoadViewState (values[3]);
    _listControl.SelectedIndex = (int) values[4];

    _isExecutingLSaveControlState = false;
  }

  protected override object SaveControlState ()
  {
    object[] values = new object[5];

    values[0] = base.SaveControlState ();
    values[1] = _value;
    values[2] = _internalValue;
    values[3] = ((IStateManager) _listControl.Items).SaveViewState();
    values[4] = _listControl.SelectedIndex;

    return values;
  }


  /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
  /// <include file='doc\include\UI\Controls\BocEnumValue.xml' path='BocEnumValue/LoadValue/*' />
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
  /// <param name="value"> The enumeration value or <see langword="null"/>. </param>
  /// <include file='doc\include\UI\Controls\BocEnumValue.xml' path='BocEnumValue/LoadUnboundValue/*' />
  public void LoadUnboundValue<TEnum> (TEnum? value, bool interim)
    where TEnum : struct
  {
    ArgumentUtility.CheckType<Enum> ("value", value);
    LoadValueInternal (value, interim);
  }

  /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
  /// <param name="value"> The enumeration value. </param>
  /// <include file='doc\include\UI\Controls\BocEnumValue.xml' path='BocEnumValue/LoadUnboundValue/*' />
  public void LoadUnboundValue<TEnum> (TEnum value, bool interim)
    where TEnum : struct
  {
    ArgumentUtility.CheckType<Enum> ("value", value);
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
  /// <include file='doc\include\UI\Controls\BocEnumValue.xml' path='BocEnumValue/SaveValue/*' />
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
  /// <include file='doc\include\UI\Controls\BocEnumValue.xml' path='BocEnumValue/CreateValidators/*' />
  public override BaseValidator[] CreateValidators()
  {
    if (IsReadOnly || ! IsRequired)
      return new BaseValidator[0];

    BaseValidator[] validators = new BaseValidator[1];
    
    if (IsNullItemVisible)
    {
      CompareValidator notNullItemValidator = new CompareValidator();
      notNullItemValidator.ID = ID + "_ValidatorNotNullItem";
      notNullItemValidator.ControlToValidate = TargetControl.ID;
      notNullItemValidator.ValueToCompare = c_nullIdentifier;
      notNullItemValidator.Operator = ValidationCompareOperator.NotEqual;
      if (StringUtility.IsNullOrEmpty (_errorMessage))
      {
        notNullItemValidator.ErrorMessage = 
            GetResourceManager().GetString (ResourceIdentifier.NullItemValidationMessage);
      }
      else
      {
        notNullItemValidator.ErrorMessage = _errorMessage;
      }      
      validators[0] = notNullItemValidator;
    }
    else
    {
      RequiredFieldValidator requiredValidator = new RequiredFieldValidator();
      requiredValidator.ID = ID + "_ValidatorRequried";
      requiredValidator.ControlToValidate = TargetControl.ID;
      if (StringUtility.IsNullOrEmpty (_errorMessage))
      {
        requiredValidator.ErrorMessage = 
            GetResourceManager().GetString (ResourceIdentifier.NullItemValidationMessage);
      }
      else
      {
        requiredValidator.ErrorMessage = _errorMessage;
      }      
      validators[0] = requiredValidator;
    }

    //  No validation that only enabled enum values get selected and saved.
    //  This behaviour mimics the Fabasoft enum behaviour

    _validators.AddRange (validators);
    return validators;
  }

  /// <summary> Populates the <see cref="ListControl"/> from the <see cref="Property"/>'s list of enabled enum values. </summary>
  protected virtual void RefreshEnumList()
  {
    if (! IsReadOnly)
    {
      if (Property != null)
      {
        EnsureChildControls();
        
        _listControl.Items.Clear();

        if (! IsRequired)
          _listControl.Items.Add (CreateNullItem());

        IEnumerationValueInfo[] valueInfos = GetEnabledValues();

        for (int i = 0; i < valueInfos.Length; i++)
        {
          IEnumerationValueInfo valueInfo = valueInfos[i];
          ListItem item = new ListItem (valueInfo.DisplayName, valueInfo.Identifier);
          _listControl.Items.Add (item);
        }
      }
    }
  }

  private IEnumerationValueInfo[] GetEnabledValues()
  {
    if (Property == null)
      return new IEnumerationValueInfo[0];
    return Property.GetEnabledValues (GetBusinessObject());
  }

  private IBusinessObject GetBusinessObject ()
  {
    return (Property != null && DataSource != null) ? DataSource.BusinessObject : null;
  }

  /// <summary> Ensures that the list of enum values has been populated. </summary>
  /// <param name="forceRefresh"> <see langword="true"/> to force a repopulation of the list. </param>
  protected void EnsureEnumListRefreshed (bool forceRefresh)
  {
    if (forceRefresh)
      _isEnumListRefreshed = false;
    if (IsReadOnly)
      return;
    if (! _isEnumListRefreshed)
    {
      RefreshEnumList();
      _isEnumListRefreshed = true;
    }
  }

  /// <summary> Refreshes the <see cref="ListControl"/> with the new value. </summary>
  protected void RefreshEnumListSelectedValue()
  {
    if (! IsReadOnly)
    {
      EnsureChildControls();

      bool hasPropertyAfterInitializion = ! _isExecutingLSaveControlState && Property != null;

      string itemWithIdentifierToRemove = null;
      if (_oldInternalValue == null && IsRequired)
      {
        itemWithIdentifierToRemove = c_nullIdentifier;
      }
      else if (_oldInternalValue != null && Property != null)
      {
        IEnumerationValueInfo oldEnumerationValueInfo = Property.GetValueInfoByIdentifier (_oldInternalValue, GetBusinessObject());        
        if (oldEnumerationValueInfo != null && ! oldEnumerationValueInfo.IsEnabled)
          itemWithIdentifierToRemove = _oldInternalValue;
        _oldInternalValue = null;
      }

      bool isNullItem =    InternalValue == null
                        || ! hasPropertyAfterInitializion;

      if (   (itemWithIdentifierToRemove == c_nullIdentifier && ! isNullItem)
          || (itemWithIdentifierToRemove != c_nullIdentifier && itemWithIdentifierToRemove != null))
      {
        ListItem itemToRemove = _listControl.Items.FindByValue (itemWithIdentifierToRemove);
        _listControl.Items.Remove (itemToRemove);
      }

      //  Check if null item is to be selected
      if (isNullItem)
      {
        bool isNullItemVisible = IsNullItemVisible;

        ListItem nullItem = _listControl.Items.FindByValue (c_nullIdentifier);
        //  No null item in the list
        if (nullItem == null && isNullItemVisible)
          _listControl.Items.Insert (0, CreateNullItem());
        else if (nullItem != null && ! isNullItemVisible)
          _listControl.Items.Remove (nullItem);

        if (isNullItemVisible)
          _listControl.SelectedValue = c_nullIdentifier;
        else
          _listControl.SelectedValue = null;
      }
      else
      {
          //  Item currently not in the list
        if (_listControl.Items.FindByValue (InternalValue) == null)
        {
          ListItem item = new ListItem (EnumerationValueInfo.DisplayName, EnumerationValueInfo.Identifier);
          _listControl.Items.Add (item);
        }

        _listControl.SelectedValue = InternalValue;
      }
    }
  }

  /// <summary> Evaluates whether to show the null item as an option in the list. </summary>
  private bool IsNullItemVisible
  {
    get
    {
      bool isRadioButtonList = _listControlStyle.ControlType == ListControlType.RadioButtonList;
      if (! isRadioButtonList)
        return true;
      if (IsRequired)
        return false;
      return _listControlStyle.RadioButtonListNullValueVisible;
    }
  }

  /// <summary> Handles refreshing the bound control. </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  private void Binding_BindingChanged (object sender, EventArgs e)
  {
    EnsureEnumListRefreshed (true);
    RefreshEnumListSelectedValue();
  }

  /// <summary> Creates the <see cref="ListItem"/> symbolizing the undefined selection. </summary>
  /// <returns> A <see cref="ListItem"/>. </returns>
  private ListItem CreateNullItem()
  {
    string nullDisplayName = _undefinedItemText;
    if (StringUtility.IsNullOrEmpty (nullDisplayName) && ! (ListControl is DropDownList))
    {
      if (IsDesignMode)
        nullDisplayName = "undefined";
      else
        nullDisplayName = GetResourceManager().GetString (ResourceIdentifier.UndefinedItemText);
    }

    ListItem emptyItem = new ListItem (nullDisplayName, c_nullIdentifier);
    return emptyItem;
  }

  /// <summary> Gets or sets the <see cref="IBusinessObjectEnumerationProperty"/> object this control is bound to. </summary>
  /// <value> An <see cref="IBusinessObjectEnumerationProperty"/> object. </value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public new IBusinessObjectEnumerationProperty Property
  {
    get { return (IBusinessObjectEnumerationProperty) base.Property; }
    set { base.Property = ArgumentUtility.CheckType<IBusinessObjectEnumerationProperty> ("value", value); }
  }
  
  /// <summary> Gets or sets the current value. </summary>
  /// <include file='doc\include\UI\Controls\BocEnumValue.xml' path='BocEnumValue/Value/*' />
  [Browsable(false)]
  public new object Value
  {
    get 
    {
      EnsureValue();
      return _value; 
    }
    set
    {
      IsDirty = true;
      _value = value;

      if (Property != null && _value != null)
        _enumerationValueInfo = Property.GetValueInfoByValue (_value, GetBusinessObject());
      else
        _enumerationValueInfo = null;

      if (_enumerationValueInfo != null)
        InternalValue = _enumerationValueInfo.Identifier;
      else
        InternalValue = null;
    }
  }

  /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = value; }
  }

  /// <summary> Gets the current value. </summary>
  /// <value> 
  ///   The <see cref="EnumerationValueInfo"/> object
  ///   or <see langword="null"/> if no item / the null item is selected 
  ///   or the <see cref="Property"/> is <see langword="null"/>.
  /// </value>
  /// <remarks> Only used to simplify access to the <see cref="IEnumerationValueInfo"/>. </remarks>
  protected IEnumerationValueInfo EnumerationValueInfo
  {
    get 
    {
      if (_enumerationValueInfo == null && Property != null && _value != null)
        _enumerationValueInfo = Property.GetValueInfoByValue (_value, GetBusinessObject());

      return _enumerationValueInfo; 
    }
  }

  /// <summary> Gets or sets the current value. </summary>
  /// <value> 
  ///   The <see cref="IEnumerationValueInfo.Identifier"/> object
  ///   or <see langword="null"/> if no item / the null item is selected.
  /// </value>
  /// <remarks> Used to identify the currently selected item. </remarks>
  protected virtual string InternalValue
  {
    get 
    {
      if (_internalValue == null && EnumerationValueInfo != null)
        _internalValue = EnumerationValueInfo.Identifier;

      return _internalValue; 
    }
    set 
    {
      if (_internalValue == value)
        return;

      _oldInternalValue = _internalValue;
      _internalValue = value;
      
      EnsureValue();
      RefreshEnumListSelectedValue();
    }
  }

  /// <summary> Ensures that the <see cref="Value"/> is set to the enum-value of the <see cref="InternalValue"/>. </summary>
  protected void EnsureValue()
  {
    if (   _enumerationValueInfo != null 
        && _enumerationValueInfo.Identifier == _internalValue)
    {
      //  Still chached in _enumerationValueInfo
      _value = _enumerationValueInfo.Value;
    }
    else if (_internalValue != null && Property != null)
    {
      //  Can get a new EnumerationValueInfo
      _enumerationValueInfo = Property.GetValueInfoByIdentifier (_internalValue, GetBusinessObject());
      _value = _enumerationValueInfo.Value;
    }
    else if (_internalValue == null)
    {
      _value = null;
      _enumerationValueInfo = null;
    }
  }

  /// <summary> 
  ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user 
  ///   interface.
  /// </summary>
  /// <returns> 
  ///   A <see cref="String"/> <see cref="Array"/> containing the <see cref="Control.ClientID"/> of the
  ///   <see cref="ListControl"/> (or the radio buttons that make up the list), if the control is in edit mode, 
  ///   or an empty array if the control is read-only.
  /// </returns>
  /// <seealso cref="BusinessObjectBoundEditableWebControl.GetTrackedClientIDs">BusinessObjectBoundEditableWebControl.GetTrackedClientIDs</seealso>
  public override string[] GetTrackedClientIDs()
  {
    if (IsReadOnly)
    {
      return new string[0];
    }
    else if (   ListControlStyle.ControlType == ListControlType.DropDownList
             || ListControlStyle.ControlType == ListControlType.ListBox)
    {
      return new string[1] { ListControl.ClientID };
    }
    else if (ListControlStyle.ControlType == ListControlType.RadioButtonList)
    {
      RadioButtonList radioButtonList = (RadioButtonList) ListControl;
      string[] clientIDs = new string[radioButtonList.Items.Count];
      for (int i = 0; i < clientIDs.Length; i++)
        clientIDs[i] = radioButtonList.ClientID + "_" + i.ToString(NumberFormatInfo.InvariantInfo);
      return clientIDs;
    }
    else
    {
      return new string[0];
    }
  }

  /// <summary> The <see cref="BocEnumValue"/> supports only scalar properties. </summary>
  /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="false"/>. </returns>
  /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
  protected override bool SupportsPropertyMultiplicity (bool isList)
  {
    return ! isList;
  }

  /// <summary>
  ///   The <see cref="BocEnumValue"/> supports properties of types <see cref="IBusinessObjectEnumerationProperty"/>
  ///   and <see cref="IBusinessObjectBooleanProperty"/>.
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
  /// <value> 
  ///   <see langword="false"/> if the <see cref="ListControlStyle"/>'s 
  ///   <see cref="Remotion.ObjectBinding.Web.UI.Controls.ListControlStyle.ControlType"/> is set to 
  ///   <see cref="ListControlType.DropDownList"/> or <see cref="ListControlType.ListBox"/>. 
  /// </value>
  public override bool UseLabel
  {
    get 
    {
      bool isDropDownList = _listControlStyle.ControlType == ListControlType.DropDownList;
      bool isListBox = _listControlStyle.ControlType == ListControlType.ListBox;
    
      return ! (isDropDownList || isListBox);
    }
  }

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its 
  ///   <see cref="Control.ClientID"/>.
  /// </summary>
  /// <value> The <see cref="ListControl"/> if the control is in edit mode, otherwise the control itself. </value>
  public override Control TargetControl 
  {
    get { return IsReadOnly ? (Control) this : ListControl; }
  }

  /// <summary> Gets the ID of the element to receive the focus when the page is loaded. </summary>
  /// <value>
  ///   Returns the <see cref="Control.ClientID"/> of the <see cref="ListControl"/> if the control is in edit mode, 
  ///   otherwise <see langword="null"/>. 
  /// </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public string FocusID
  { 
    get { return IsReadOnly ? null : ListControl.ClientID; }
  }

  /// <summary> This event is fired when the selection is changed between postbacks. </summary>
  [Category ("Action")]
  [Description ("Fires when the value of the control has changed.")]
  public event EventHandler SelectionChanged
  {
    add { Events.AddHandler (s_selectionChangedEvent, value); }
    remove { Events.RemoveHandler (s_selectionChangedEvent, value); }
  }

  /// <summary>
  ///   Gets the style that you want to apply to the <see cref="ListControl"/> (edit mode) 
  ///   and the <see cref="Label"/> (read-only mode).
  /// </summary>
  /// <remarks>
  ///   Use the <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> to assign individual 
  ///   style settings for the respective modes. Note that if you set one of the <b>Font</b> 
  ///   attributes (Bold, Italic etc.) to <see langword="true"/>, this cannot be overridden using 
  ///   <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/>  properties.
  /// </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to the ListControl (edit mode) and the Label (read-only mode).")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style CommonStyle
  {
    get { return _commonStyle; }
  }

  /// <summary> Gets the style that you want to apply to the <see cref="ListControl"/> (edit mode) only. </summary>
  /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to the ListControl (edit mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public ListControlStyle ListControlStyle
  {
    get { return _listControlStyle; }
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

  /// <summary> Gets the <see cref="ListControl"/> used in edit mode. </summary>
  [Browsable (false)]
  public ListControl ListControl
  {
    get
    {
      EnsureChildControls();
      return _listControl; 
    }
  }

  /// <summary> Gets the <see cref="Label"/> used in read-only mode. </summary>
  [Browsable (false)]
  public Label Label
  {
    get { return _label; }
  }

  /// <summary> Gets or sets the text displayed for the undefined item. </summary>
  /// <value> 
  ///   The text displayed for <see langword="null"/>. The default value is an empty <see cref="String"/>.
  ///   In case of the default value, the text is read from the resources for this control.
  /// </value>
  [Description("The description displayed for the undefined item.")]
  [Category ("Appearance")]
  [DefaultValue("")]
  public string UndefinedItemText
  {
    get { return _undefinedItemText; }
    set { _undefinedItemText = value; }
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
  /// <summary> Gets the CSS-Class applied to the <see cref="BocEnumValue"/> itself. </summary>
  /// <remarks> 
  ///   <para> Class: <c>bocEnumValue</c>. </para>
  ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassBase
  { get { return "bocEnumValue"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocEnumValue"/> when it is displayed in read-only mode. </summary>
  /// <remarks> 
  ///   <para> Class: <c>readOnly</c>. </para>
  ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocEnumValue.readOnly</c> as a selector. </para>
  /// </remarks>
  protected virtual string CssClassReadOnly
  { get { return "readOnly"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocEnumValue"/> when it is displayed disabled. </summary>
  /// <remarks> 
  ///   <para> Class: <c>disabled</c>. </para>
  ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocEnumValue.disabled</c> as a selector. </para>
  /// </remarks>
  protected virtual string CssClassDisabled
  { get { return "disabled"; } }
  #endregion
}

}
