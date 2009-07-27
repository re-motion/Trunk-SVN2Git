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
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.ServiceLocation;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocEnumValue;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Infrastructure;
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
  [ToolboxItemFilter ("System.Web.UI")]
  public class BocEnumValue : BusinessObjectBoundEditableWebControl, IBocEnumValue, IPostBackDataHandler, IFocusableControl
  {
    // constants

    private const string c_nullIdentifier = "==null==";
    private const string c_labelIDPostfix = "_Boc_Label";
    private const string c_listControlIDPostfix = "_Boc_ListControl";
    private const string c_styleFileName = "BocEnumValue.css";

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

    private static readonly Type[] s_supportedPropertyInterfaces = new[] { typeof (IBusinessObjectEnumerationProperty) };

    private static readonly object s_selectionChangedEvent = new object();
    private static readonly string s_styleKey = typeof (BocEnumValue).FullName + "_Style";

    // member fields

    private object _value;
    private string _internalValue;
    private IEnumerationValueInfo _enumerationValueInfo;

    private readonly Style _commonStyle;
    private readonly ListControlStyle _listControlStyle;
    private readonly Style _labelStyle;

    private string _undefinedItemText = string.Empty;

    private string _errorMessage;
    private readonly ArrayList _validators;

    // construction and disposing

    public BocEnumValue ()
    {
      _commonStyle = new Style();
      _listControlStyle = new ListControlStyle();
      _labelStyle = new Style();
      _validators = new ArrayList();
    }

    // methods and properties

    public override void RegisterHtmlHeadContents (IHttpContext httpContext, HtmlHeadAppender htmlHeadAppender)
    {
      if (!htmlHeadAppender.IsRegistered (s_styleKey))
      {
        string url = ResourceUrlResolver.GetResourceUrl (this, httpContext, typeof (BocEnumValue), ResourceType.Html, c_styleFileName);
        htmlHeadAppender.RegisterStylesheetLink (s_styleKey, url, HtmlHeadAppender.Priority.Library);
      }
    }

    public override void RenderControl (HtmlTextWriter writer)
    {
      var factory = ServiceLocator.Current.GetInstance<IBocEnumValueRendererFactory>();
      var renderer = factory.CreateRenderer (new HttpContextWrapper (Context), writer, this);
      renderer.Render();
    }

    /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='doc\include\UI\Controls\BocEnumValue.xml' path='BocEnumValue/LoadValue/*' />
    public override void LoadValue (bool interim)
    {
      if (interim)
        return;
      
      if (Property != null && DataSource != null && DataSource.BusinessObject != null)
      {
        object value = DataSource.BusinessObject.GetProperty (Property);
        LoadValueInternal (value, false);
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

    /// <summary> Saves the <see cref="Value"/> into the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='doc\include\UI\Controls\BocEnumValue.xml' path='BocEnumValue/SaveValue/*' />
    public override void SaveValue (bool interim)
    {
      if (!interim && IsDirty)
      {
        if (Property != null && DataSource != null && DataSource.BusinessObject != null && !IsReadOnly)
        {
          DataSource.BusinessObject.SetProperty (Property, Value);
          IsDirty = false;
        }
      }
    }

    /// <summary> Creates the list of validators required for the current binding and property settings. </summary>
    /// <include file='doc\include\UI\Controls\BocEnumValue.xml' path='BocEnumValue/CreateValidators/*' />
    public override BaseValidator[] CreateValidators ()
    {
      if (IsReadOnly || !IsRequired)
        return new BaseValidator[0];

      BaseValidator[] validators = new BaseValidator[1];

      if (IsNullItemVisible)
      {
        CompareValidator notNullItemValidator = new CompareValidator ();
        notNullItemValidator.ID = ID + "_ValidatorNotNullItem";
        notNullItemValidator.ControlToValidate = TargetControl.ID;
        notNullItemValidator.ValueToCompare = c_nullIdentifier;
        notNullItemValidator.Operator = ValidationCompareOperator.NotEqual;
        if (StringUtility.IsNullOrEmpty (_errorMessage))
        {
          notNullItemValidator.ErrorMessage =
              GetResourceManager ().GetString (ResourceIdentifier.NullItemValidationMessage);
        }
        else
          notNullItemValidator.ErrorMessage = _errorMessage;
        validators[0] = notNullItemValidator;
      }
      else
      {
        RequiredFieldValidator requiredValidator = new RequiredFieldValidator ();
        requiredValidator.ID = ID + "_ValidatorRequried";
        requiredValidator.ControlToValidate = TargetControl.ID;
        if (StringUtility.IsNullOrEmpty (_errorMessage))
        {
          requiredValidator.ErrorMessage =
              GetResourceManager ().GetString (ResourceIdentifier.NullItemValidationMessage);
        }
        else
          requiredValidator.ErrorMessage = _errorMessage;
        validators[0] = requiredValidator;
      }

      //  No validation that only enabled enum values get selected and saved.
      //  This behaviour mimics the Fabasoft enum behaviour

      _validators.AddRange (validators);
      return validators;
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
    public override string[] GetTrackedClientIDs ()
    {
      if (IsReadOnly)
        return new string[0];
      else if (ListControlStyle.ControlType == ListControlType.DropDownList
               || ListControlStyle.ControlType == ListControlType.ListBox)
        return new[] { GetListControlClientID () };
      else if (ListControlStyle.ControlType == ListControlType.RadioButtonList)
      {
        string[] clientIDs = new string[GetEnabledValues ().Length + (IsRequired ? 0 : 1)];
        for (int i = 0; i < clientIDs.Length; i++)
          clientIDs[i] = GetListControlClientID () + "_" + i.ToString (NumberFormatInfo.InvariantInfo);
        return clientIDs;
      }
      else
        return new string[0];
    }

    private string GetListControlUniqueID ()
    {
      return UniqueID + c_listControlIDPostfix;
    }

    public string GetListControlClientID ()
    {
      return ClientID + c_listControlIDPostfix;
    }

    public string GetLabelClientID ()
    {
      return ClientID + c_labelIDPostfix;
    }

    string IBocEnumValue.LabelID
    {
      get { return UniqueID + c_labelIDPostfix; }
    }

    string IBocEnumValue.ListControlID
    {
      get { return UniqueID + c_listControlIDPostfix; }
    }

    /// <summary> This event is fired when the selection is changed between postbacks. </summary>
    [Category ("Action")]
    [Description ("Fires when the value of the control has changed.")]
    public event EventHandler SelectionChanged
    {
      add { Events.AddHandler (s_selectionChangedEvent, value); }
      remove { Events.RemoveHandler (s_selectionChangedEvent, value); }
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
    [Browsable (false)]
    public new object Value
    {
      get
      {
        EnsureValue ();
        return _value;
      }
      set
      {
        IsDirty = true;
        _value = value;

        if (Property != null && _value != null)
          _enumerationValueInfo = Property.GetValueInfoByValue (_value, GetBusinessObject ());
        else
          _enumerationValueInfo = null;

        if (_enumerationValueInfo != null)
          InternalValue = _enumerationValueInfo.Identifier;
        else
          InternalValue = null;
      }
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

        return !(isDropDownList || isListBox);
      }
    }

    /// <summary>
    ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its 
    ///   <see cref="Control.ClientID"/>.
    /// </summary>
    /// <value> The  control itself. </value>
    public override Control TargetControl
    {
      get { return this; }
    }

    /// <summary> Gets the ID of the element to receive the focus when the page is loaded. </summary>
    /// <value>
    ///   Returns the <see cref="Control.ClientID"/> of the list control if the control is in edit mode, 
    ///   otherwise <see langword="null"/>. 
    /// </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public string FocusID
    {
      get { return IsReadOnly ? null : GetListControlClientID (); }
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
    [Category ("Style")]
    [Description ("The style that you want to apply to the ListControl (edit mode) and the Label (read-only mode).")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public Style CommonStyle
    {
      get { return _commonStyle; }
    }

    /// <summary> Gets the style that you want to apply to the <see cref="ListControl"/> (edit mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
    [Category ("Style")]
    [Description ("The style that you want to apply to the ListControl (edit mode) only.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public ListControlStyle ListControlStyle
    {
      get { return _listControlStyle; }
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

    /// <summary> Gets or sets the text displayed for the undefined item. </summary>
    /// <value> 
    ///   The text displayed for <see langword="null"/>. The default value is an empty <see cref="String"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description ("The description displayed for the undefined item.")]
    [Category ("Appearance")]
    [DefaultValue ("")]
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
          BaseValidator validator = (BaseValidator) _validators[i];
          validator.ErrorMessage = _errorMessage;
        }
      }
    }

    [Browsable(false)]
    public string NullIdentifier
    {
      get { return c_nullIdentifier; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      if (!IsDesignMode)
        Page.RegisterRequiresPostBack (this);
    }

    protected override void LoadControlState (object savedState)
    {
      object[] values = (object[]) savedState;

      base.LoadControlState (values[0]);
      _value = values[1];
      if (values[2] != null)
        _internalValue = (string) values[2];
    }

    protected override object SaveControlState ()
    {
      object[] values = new object[5];

      values[0] = base.SaveControlState();
      values[1] = _value;
      values[2] = _internalValue;

      return values;
    }

    /// <summary>
    ///   Uses the <paramref name="postCollection"/> to determine whether the value of this control has been changed
    ///   between postbacks.
    /// </summary>
    /// <include file='doc\include\UI\Controls\BocEnumValue.xml' path='BocEnumValue/LoadPostData/*' />
    protected virtual bool LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      string newValue = PageUtility.GetPostBackCollectionItem (Page, GetListControlUniqueID ());
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
    protected virtual void RaisePostDataChangedEvent ()
    {
      if (!IsReadOnly && Enabled)
        OnSelectionChanged ();
    }

    /// <summary> Fires the <see cref="SelectionChanged"/> event. </summary>
    protected virtual void OnSelectionChanged ()
    {
      EventHandler eventHandler = (EventHandler) Events[s_selectionChangedEvent];
      if (eventHandler != null)
        eventHandler (this, EventArgs.Empty);
    }

    /// <summary> Checks whether the control conforms to the required WAI level. </summary>
    /// <exception cref="Remotion.Web.UI.WcagException"> Thrown if the control does not conform to the required WAI level. </exception>
    protected virtual void EvaluateWaiConformity ()
    {
      if (WcagHelper.Instance.IsWcagDebuggingEnabled () && WcagHelper.Instance.IsWaiConformanceLevelARequired ())
      {
        if (ListControlStyle.AutoPostBack == true)
          WcagHelper.Instance.HandleWarning (1, this, "ListControlStyle.AutoPostBack");
      }
    }

    protected override void OnPreRender (EventArgs e)
    {
      EnsureChildControls ();
      base.OnPreRender (e);

      LoadResources (GetResourceManager ());

      EvaluateWaiConformity ();
    }

    /// <summary> Gets a <see cref="HtmlTextWriterTag.Div"/> as the <see cref="WebControl.TagKey"/>. </summary>
    protected override HtmlTextWriterTag TagKey
    {
      get { return HtmlTextWriterTag.Div; }
    }
    
    /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
    protected virtual void LoadValueInternal (object value, bool interim)
    {
      if (interim)
        return;

      Value = value;
      IsDirty = false;
    }

    /// <summary> Returns the <see cref="IResourceManager"/> used to access the resources for this control. </summary>
    protected virtual IResourceManager GetResourceManager ()
    {
      return GetResourceManager (typeof (ResourceIdentifier));
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
          _enumerationValueInfo = Property.GetValueInfoByValue (_value, GetBusinessObject ());

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

        _internalValue = value;

        EnsureValue ();
      }
    }

    /// <summary> Ensures that the <see cref="Value"/> is set to the enum-value of the <see cref="InternalValue"/>. </summary>
    protected void EnsureValue ()
    {
      if (_enumerationValueInfo != null
          && _enumerationValueInfo.Identifier == _internalValue)
      {
        //  Still chached in _enumerationValueInfo
        _value = _enumerationValueInfo.Value;
      }
      else if (_internalValue != null && Property != null)
      {
        //  Can get a new EnumerationValueInfo
        _enumerationValueInfo = Property.GetValueInfoByIdentifier (_internalValue, GetBusinessObject ());
        _value = _enumerationValueInfo.Value;
      }
      else if (_internalValue == null)
      {
        _value = null;
        _enumerationValueInfo = null;
      }
    }

    /// <summary> The <see cref="BocEnumValue"/> supports only scalar properties. </summary>
    /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="false"/>. </returns>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
    protected override bool SupportsPropertyMultiplicity (bool isList)
    {
      return !isList;
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

    private IEnumerationValueInfo[] GetEnabledValues ()
    {
      if (Property == null)
        return new IEnumerationValueInfo[0];
      return Property.GetEnabledValues (GetBusinessObject ());
    }

    private IBusinessObject GetBusinessObject ()
    {
      return (Property != null && DataSource != null) ? DataSource.BusinessObject : null;
    }

    /// <summary> Evaluates whether to show the null item as an option in the list. </summary>
    private bool IsNullItemVisible
    {
      get
      {
        bool isRadioButtonList = _listControlStyle.ControlType == ListControlType.RadioButtonList;
        if (!isRadioButtonList)
          return true;
        if (IsRequired)
          return false;
        return _listControlStyle.RadioButtonListNullValueVisible;
      }
    }

    private string GetNullItemText ()
    {
      string nullDisplayName = _undefinedItemText;
      if (string.IsNullOrEmpty (nullDisplayName) && (ListControlStyle.ControlType != ListControlType.DropDownList))
      {
        if (IsDesignMode)
          nullDisplayName = "undefined";
        else
          nullDisplayName = GetResourceManager ().GetString (ResourceIdentifier.UndefinedItemText);
      }
      return nullDisplayName;
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
      RaisePostDataChangedEvent ();
    }

    bool IBocRenderableControl.IsDesignMode
    {
      get { return IsDesignMode; }
    }

    IEnumerationValueInfo[] IBocEnumValue.GetEnabledValues ()
    {
      return GetEnabledValues ();
    }

    IEnumerationValueInfo IBocEnumValue.EnumerationValueInfo
    {
      get { return EnumerationValueInfo; }
    }

    string IBocEnumValue.GetNullItemText ()
    {
      return GetNullItemText ();
    }
  }
}