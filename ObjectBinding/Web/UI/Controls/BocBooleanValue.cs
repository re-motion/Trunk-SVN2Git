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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocBooleanValue;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{

/// <summary> This control can be used to display or edit a tri-state value (true, false, and undefined). </summary>
/// <include file='doc\include\UI\Controls\BocBooleanValue.xml' path='BocBooleanValue/Class/*' />
[ValidationProperty ("ValidationValue")]
[DefaultEvent ("SelectionChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocBooleanValue: BusinessObjectBoundEditableWebControl, IPostBackDataHandler, IFocusableControl
{
	// constants

  private const string c_scriptFileUrl = "BocBooleanValue.js";

  private const string c_trueIcon = "CheckBoxTrue.gif";
  private const string c_falseIcon = "CheckBoxFalse.gif";
  private const string c_nullIcon = "CheckBoxNull.gif";

  private const string c_defaultControlWidth = "100pt";
  private const string c_nullString = "null";

  private const string c_defaultResourceGroup = "default";

  // types

  /// <summary> A list of control specific resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
  ///   See the documentation of <b>GetString</b> for further details.
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Remotion.ObjectBinding.Web.Globalization.BocBooleanValue")]
  protected enum ResourceIdentifier
  {
    /// <summary> The descripton rendered next the check box when it is checked. </summary>
    TrueDescription,
    /// <summary> The descripton rendered next the check box when it is not checked.  </summary>
    FalseDescription,
    /// <summary> The descripton rendered next the check box when its state is undefined.  </summary>
    NullDescription,
    /// <summary> The validation error message displayed when the null item is selected. </summary>
    NullItemValidationMessage
  }

  // static members
	
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectBooleanProperty) };

  private static readonly object s_checkedChangedEvent = new object();

  private static readonly string s_scriptFileKey = typeof (BocBooleanValue).FullName + "_Script";
  private static readonly string s_startUpScriptKey = typeof (BocBooleanValue).FullName+ "_Startup";

	// member fields
  private bool? _value = null;

  private HyperLink _hyperLink;
  private Image _image;
  private Label _label;
  private HiddenField _hiddenField;
  private Style _labelStyle;

  private bool? _autoPostBack = null;

  private bool _showDescription = true;
  private string _trueDescription = string.Empty;
  private string _falseDescription = string.Empty;
  private string _nullDescription = string.Empty;

  private string _errorMessage;
  private ArrayList _validators;
  /// <summary> Flag that determines whether the client script will be rendered. </summary>
  private bool _hasClientScript = false;

  // construction and disposing

	public BocBooleanValue()
	{
    _labelStyle = new Style();
    _hiddenField = new HiddenField();
    _hyperLink = new HyperLink();
    _image = new Image();
    _label = new Label();
    _validators = new ArrayList();
	}

  // methods and properties

  protected override void CreateChildControls()
  {
    _hiddenField.ID = ID + "_Boc_HiddenField";
    _hiddenField.EnableViewState = false;
    Controls.Add (_hiddenField);

    _hyperLink.ID = ID + "_Boc_HyperLink";
    _hyperLink.EnableViewState = false;
    Controls.Add (_hyperLink);

    _image.ID = ID + "_Boc_Image";
    _image.EnableViewState = false;
    _hyperLink.Controls.Add (_image);

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

    if (!HtmlHeadAppender.Current.IsRegistered (s_scriptFileKey))
    {
      string scriptUrl = ResourceUrlResolver.GetResourceUrl (this, context, typeof (BocBooleanValue), ResourceType.Html, c_scriptFileUrl);
      HtmlHeadAppender.Current.RegisterJavaScriptInclude (s_scriptFileKey, scriptUrl);
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
  /// <include file='doc\include\UI\Controls\BocBooleanValue.xml' path='BocBooleanValue/LoadPostData/*' />
  protected virtual bool LoadPostData (string postDataKey, NameValueCollection postCollection)
  {
    string newValueAsString = PageUtility.GetPostBackCollectionItem (Page, _hiddenField.UniqueID);
    bool? newValue = null;
    bool isDataChanged = false;
    if (newValueAsString != null)
    {
      if (newValueAsString == c_nullString)
        newValue = null;
      else
        newValue = bool.Parse (newValueAsString);
      isDataChanged = _value != newValue;
    }
    if (isDataChanged)
    {
      _value = newValue;
      IsDirty = true;
    }
    return isDataChanged;
  }

  /// <summary> Called when the state of the control has changed between postbacks. </summary>
  protected virtual void RaisePostDataChangedEvent()
  {
    if (! IsReadOnly && Enabled)
      OnCheckedChanged();
  }

  /// <summary> Fires the <see cref="CheckedChanged"/> event. </summary>
  protected virtual void OnCheckedChanged()
  {
    EventHandler eventHandler = (EventHandler) Events[s_checkedChangedEvent];
    if (eventHandler != null)
      eventHandler (this, EventArgs.Empty);
  }

  /// <summary> Checks whether the control conforms to the required WAI level. </summary>
  /// <exception cref="WcagException"> Thrown if the control does not conform to the required WAI level. </exception>
  protected virtual void EvaluateWaiConformity ()
  {
    if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
      WcagHelper.Instance.HandleError (1, this);
  }

  protected override void OnPreRender (EventArgs e)
  {
    EnsureChildControls();
    base.OnPreRender (e);
    
    bool isReadOnly = IsReadOnly;


    BocBooleanValueResourceSet resourceSet = CreateResourceSet();

    string trueDescription = (StringUtility.IsNullOrEmpty (_trueDescription) ? resourceSet.DefaultTrueDescription : _trueDescription);
    string falseDescription = (StringUtility.IsNullOrEmpty (_falseDescription) ? resourceSet.DefaultFalseDescription : _falseDescription);
    string nullDescription = (StringUtility.IsNullOrEmpty (_nullDescription) ? resourceSet.DefaultNullDescription : _nullDescription);

    string imageUrl;
    string description;

    IResourceManager resourceManager = GetResourceManager ();
    LoadResources (resourceManager);

    if (!_value.HasValue)
    {
      imageUrl = resourceSet.NullIconUrl;
      description = nullDescription;
    }
    else if (_value.Value)
    {
      imageUrl = resourceSet.TrueIconUrl;
      description = trueDescription;
    }
    else
    {
      imageUrl = resourceSet.FalseIconUrl;
      description = falseDescription;
    }

    DetermineClientScriptLevel();

    if (_hasClientScript && ! isReadOnly)
    {
      string script;

      if (Enabled)
      {
        if (!Page.ClientScript.IsStartupScriptRegistered (s_startUpScriptKey))
        {
          string trueValue = true.ToString();
          string falseValue = false.ToString();
          string nullValue = c_nullString;

          script = string.Format (
              "BocBooleanValue_InitializeGlobals ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');",
              resourceSet.ResourceKey,
              trueValue, falseValue, nullValue,
              resourceSet.DefaultTrueDescription, resourceSet.DefaultFalseDescription, resourceSet.DefaultNullDescription,
              resourceSet.TrueIconUrl, resourceSet.FalseIconUrl, resourceSet.NullIconUrl);
          ScriptUtility.RegisterStartupScriptBlock (Page, s_startUpScriptKey, script);
        }
      }

      if (Enabled)
      {
        string requiredFlag = IsRequired ? "true" : "false";
        string image = "document.getElementById ('" + _image.ClientID + "')";
        string label = _showDescription ? "document.getElementById ('" + _label.ClientID + "')" : "null";
        string hiddenField = "document.getElementById ('" + _hiddenField.ClientID + "')";
        script = "BocBooleanValue_SelectNextCheckboxValue ("
            + "'" + resourceSet.ResourceKey + "', "
            + image + ", " 
            + label + ", " 
            + hiddenField + ", "
            + requiredFlag + ", "
            + (StringUtility.IsNullOrEmpty (_trueDescription) ? "null" : "'" + _trueDescription + "'") + ", "
            + (StringUtility.IsNullOrEmpty (_falseDescription) ? "null" :"'" +  _falseDescription + "'") + ", "
            + (StringUtility.IsNullOrEmpty (_nullDescription) ? "null" : "'" + _nullDescription + "'") + ");";

        if (_autoPostBack == true)
          script += Page.ClientScript.GetPostBackEventReference (this, "") + ";";
        script += "return false;";
      }
      else
      {
        script = "return false;";
      }
      _label.Attributes.Add ("onclick", script);
      _hyperLink.Attributes.Add ("onclick", script);
      _hyperLink.Attributes.Add ("onkeydown", "BocBooleanValue_OnKeyDown (this);");
      _hyperLink.Style["padding"] = "0px";
      _hyperLink.Style["border"] = "none";
      _hyperLink.Style["background-color"] = "transparent";
      _hyperLink.Attributes.Add ("href", "#");
      _hyperLink.Enabled = Enabled;
    }

    if (!isReadOnly)
      _hiddenField.Value = _value.HasValue ? _value.ToString() : c_nullString;
    _hiddenField.Visible = ! isReadOnly;
    _image.ImageUrl = imageUrl;
    _image.AlternateText = description;
    _image.Style["vertical-align"] = "middle";

    if (_showDescription)
      _label.Text = description;

    _label.Width = Unit.Empty;
    _label.Height = Unit.Empty;
    _label.ApplyStyle (_labelStyle);
  }
  
  /// <summary>
  /// Override this method to change the default look of the <see cref="BocBooleanValue"/>
  /// </summary>
  protected virtual BocBooleanValueResourceSet CreateResourceSet ()
  {
    IResourceManager resourceManager = GetResourceManager();

    BocBooleanValueResourceSet resourceSet = new BocBooleanValueResourceSet (
        c_defaultResourceGroup,
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (BocBooleanValue), ResourceType.Image, c_trueIcon),
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (BocBooleanValue), ResourceType.Image, c_falseIcon),
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (BocBooleanValue), ResourceType.Image, c_nullIcon),
        resourceManager.GetString (ResourceIdentifier.TrueDescription),
        resourceManager.GetString (ResourceIdentifier.FalseDescription),
        resourceManager.GetString (ResourceIdentifier.NullDescription)
        );

    return resourceSet;
  }

  protected override void AddAttributesToRender(HtmlTextWriter writer)
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

    writer.AddStyleAttribute ("white-space", "nowrap");
    if (! IsReadOnly)
    {
      bool isControlWidthEmpty = Width.IsEmpty && StringUtility.IsNullOrEmpty (Style["width"]);
      bool isLabelWidthEmpty = StringUtility.IsNullOrEmpty (_label.Style["width"]);
      if (isLabelWidthEmpty && isControlWidthEmpty)
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
    }
  }

  protected override void Render(HtmlTextWriter writer)
  {
    EvaluateWaiConformity();

    base.Render (writer);
  }

  protected override void LoadControlState (object savedState)
  {
    object[] values = (object[]) savedState;

    base.LoadControlState (values[0]);
    _value = (bool?) values[1];

    _hiddenField.Value = _value.HasValue ? _value.ToString () : c_nullString;
  }

  protected override object SaveControlState ()
  {
    object[] values = new object[2];

    values[0] = base.SaveControlState ();
    values[1] = _value;

    return values;
  }

  /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
  /// <include file='doc\include\UI\Controls\BocBooleanValue.xml' path='BocBooleanValue/LoadValue/*' />
  public override void LoadValue (bool interim)
  {
    if (! interim)
    {
      if (Property != null && DataSource != null && DataSource.BusinessObject != null)
      {
        bool? value = (bool?) DataSource.BusinessObject.GetProperty (Property);
        LoadValueInternal (value, interim);
      }
    }
  }

  /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
  /// <include file='doc\include\UI\Controls\BocBooleanValue.xml' path='BocBooleanValue/LoadUnboundValue/*' />
  public void LoadUnboundValue (bool? value, bool interim)
  {
    LoadValueInternal (value, interim);
  }

  /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
  protected virtual void LoadValueInternal (bool? value, bool interim)
  {
    if (! interim)
    {
      Value = value;
      IsDirty = false;
    }
  }

  /// <summary> Saves the <see cref="Value"/> into the bound <see cref="IBusinessObject"/>. </summary>
  /// <include file='doc\include\UI\Controls\BocBooleanValue.xml' path='BocBooleanValue/SaveValue/*' />
  public override void SaveValue (bool interim)
  {
    if (! interim && IsDirty)
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

    string key;
    key = ResourceManagerUtility.GetGlobalResourceKey (TrueDescription);
    if (! StringUtility.IsNullOrEmpty (key))
      TrueDescription = resourceManager.GetString (key);

    key = ResourceManagerUtility.GetGlobalResourceKey (FalseDescription);
    if (! StringUtility.IsNullOrEmpty (key))
      FalseDescription = resourceManager.GetString (key);

    key = ResourceManagerUtility.GetGlobalResourceKey (NullDescription);
    if (! StringUtility.IsNullOrEmpty (key))
      NullDescription = resourceManager.GetString (key);

    key = ResourceManagerUtility.GetGlobalResourceKey (ErrorMessage);
    if (! StringUtility.IsNullOrEmpty (key))
      ErrorMessage = resourceManager.GetString (key);
  }

  /// <summary> Creates the list of validators required for the current binding and property settings. </summary>
  /// <include file='doc\include\UI\Controls\BocBooleanValue.xml' path='BocBooleanValue/CreateValidators/*' />
  public override BaseValidator[] CreateValidators()
  {
    if (IsReadOnly || ! IsRequired)
      return new BaseValidator[0];

    BaseValidator[] validators = new BaseValidator[1];

    CompareValidator notNullItemValidator = new CompareValidator();
    notNullItemValidator.ID = ID + "_ValidatorNotNullItem";
    notNullItemValidator.ControlToValidate = ID;
    notNullItemValidator.ValueToCompare = c_nullString;
    notNullItemValidator.Operator = ValidationCompareOperator.NotEqual;
    if (StringUtility.IsNullOrEmpty (_errorMessage))
      notNullItemValidator.ErrorMessage = GetResourceManager().GetString (ResourceIdentifier.NullItemValidationMessage);
    else
      notNullItemValidator.ErrorMessage = _errorMessage;
    validators[0] = notNullItemValidator;

    _validators.AddRange (validators);
    return validators;
  }

  private void DetermineClientScriptLevel() 
  {
    _hasClientScript = false;

    if (! ControlHelper.IsDesignMode (this, Context))
    {
      _hasClientScript = true;
      //bool isVersionHigherThan55 = Context.Request.Browser.MajorVersion >= 6
      //                        ||   Context.Request.Browser.MajorVersion == 5 
      //                          && Context.Request.Browser.MinorVersion >= 0.5;
      //bool isInternetExplorer55AndHigher = 
      //    Context.Request.Browser.Browser == "IE" && isVersionHigherThan55;
      //
      //_hasClientScript = isInternetExplorer55AndHigher;
    }
  }

  /// <summary> Gets or sets the <see cref="IBusinessObjectBooleanProperty"/> object this control is bound to. </summary>
  /// <value> An instance of type <see cref="IBusinessObjectBooleanProperty"/>. </value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public new IBusinessObjectBooleanProperty Property
  {
    get { return (IBusinessObjectBooleanProperty) base.Property; }
    set { base.Property = ArgumentUtility.CheckType<IBusinessObjectBooleanProperty> ("value", value); }
  }
  
  /// <summary> Gets or sets the current value. </summary>
  /// <value> The boolean value currently displayed or <see langword="null"/>. </value>
  /// <remarks> The dirty state is reset when the value is set. </remarks>
  [Browsable(false)]
  public new bool? Value
  {
    get
    {
      return _value;
    }
    set
    {
      IsDirty = true;
      _value = value;
    }
  }

  /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = ArgumentUtility.CheckType<bool?> ("value", value); }
  }

  /// <summary> 
  ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user 
  ///   interface.
  /// </summary>
  /// <returns> 
  ///   A <see cref="String"/> <see cref="Array"/> containing the <see cref="Control.ClientID"/> of the
  ///   <see cref="HiddenField"/> if the control is in edit mode, or an empty array if the control is read-only.
  /// </returns>
  /// <seealso cref="BusinessObjectBoundEditableWebControl.GetTrackedClientIDs">BusinessObjectBoundEditableWebControl.GetTrackedClientIDs</seealso>
  public override string[] GetTrackedClientIDs()
  {
    return IsReadOnly ? new string[0] : new string[1] { _hiddenField.ClientID };
  }

  /// <summary> The <see cref="BocCheckBox"/> supports only scalar properties. </summary>
  /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="false"/>. </returns>
  /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
  protected override bool SupportsPropertyMultiplicity (bool isList)
  {
    return ! isList;
  }

  /// <summary>
  ///   The <see cref="BocBooleanValue"/> supports properties of type <see cref="IBusinessObjectBooleanProperty"/>.
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
  /// <value> Always <see langword="true"/>. </value>
  public override bool UseLabel
  {
    get { return true; }
  }

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its 
  ///   <see cref="Control.ClientID"/>.
  /// </summary>
  /// <value> The <see cref="HyperLink"/> if the control is in edit mode, otherwise the control itself. </value>
  public override Control TargetControl 
  {
    get { return IsReadOnly ? (Control) this : _hyperLink; }
  }

  /// <summary> Gets the ID of the element to receive the focus when the page is loaded. </summary>
  /// <value>
  ///   Returns the <see cref="Control.ClientID"/> of the <see cref="HyperLink"/> if the control is in edit mode, 
  ///   otherwise <see langword="null"/>. 
  /// </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public string FocusID
  { 
    get { return IsReadOnly ? null : _hyperLink.ClientID; }
  }

  /// <summary> Gets the string representation of this control's <see cref="Value"/>. </summary>
  /// <remarks> 
  ///   <para>
  ///     Values can be <c>True</c>, <c>False</c>, and <c>null</c>. 
  ///   </para><para>
  ///     This property is used for validation.
  ///   </para>
  /// </remarks>
  [Browsable (false)]
  public string ValidationValue
  {
    get { return _hiddenField.Value; }
  }

  /// <summary> Occurs when the <see cref="Value"/> property changes between postbacks. </summary>
  [Category ("Action")]
  [Description ("Fires when the value of the control has changed.")]
  public event EventHandler CheckedChanged
  {
    add { Events.AddHandler (s_checkedChangedEvent, value); }
    remove { Events.RemoveHandler (s_checkedChangedEvent, value); }
  }

  /// <summary>
  ///   Gets the <see cref="Style"/> that you want to apply to the <see cref="Label"/> used for displaying the 
  ///   description. 
  /// </summary>
  [Category("Style")]
  [Description("The style that you want to apply to the label used for displaying the description.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style LabelStyle
  {
    get { return _labelStyle; }
  }

  /// <summary> Gets the <see cref="System.Web.UI.WebControls.Label"/> used for displaying the description. </summary>
  [Browsable (false)]
  public Label Label
  {
    get { return _label; }
  }

  /// <summary> Gets the <see cref="System.Web.UI.WebControls.HyperLink"/> used for provding click functionality. </summary>
  [Browsable (false)]
  public HyperLink HyperLink
  {
    get { return _hyperLink; }
  }

  /// <summary> Gets the <see cref="System.Web.UI.WebControls.Image"/> used for displaying the checkbox icon. </summary>
  [Browsable (false)]
  public Image Image
  {
    get { return _image; }
  }

  /// <summary> Gets the <see cref="HiddenField"/> used for posting the value back to the server.  </summary>
  [Browsable (false)]
  public HiddenField HiddenField
  {
    get { return _hiddenField; }
  }

  /// <summary> Gets a flag that determines whether changing the checked state causes an automatic postback.</summary>
  /// <value> 
  ///   <see langword="NaBoolean.True"/> to enable automatic postbacks. 
  ///   Defaults to <see langword="null"/>, which is interpreted as <see langword="false"/>.
  /// </value>
  [Description("Automatically postback to the server after the checked state is modified. Undefined is interpreted as false.")]
  [Category("Behavior")]
  [DefaultValue (typeof(bool?), "")]
  [NotifyParentProperty (true)]
  public bool? AutoPostBack
  {
    get { return _autoPostBack; }
    set { _autoPostBack = value; }
  }

  /// <summary> Gets or sets the flag that determines whether to show the description next to the checkbox. </summary>
  /// <value> <see langword="true"/> to enable the description. The default value is <see langword="true"/>. </value>
  [Description("The flag that determines whether to show the description next to the checkbox")]
  [Category ("Appearance")]
  [DefaultValue(true)]
  public bool ShowDescription
  {
    get { return _showDescription; }
    set { _showDescription = value; }
  }

  /// <summary> Gets or sets the description displayed when the checkbox is set to <see langword="true"/>. </summary>
  /// <value> 
  ///   The text displayed for <see langword="true"/>. The default value is an empty <see cref="String"/>.
  ///   In case of the default value, the text is read from the resources for this control.
  /// </value>
  [Description("The description displayed when the checkbox is set to True.")]
  [Category ("Appearance")]
  [DefaultValue("")]
  public string TrueDescription
  {
    get { return _trueDescription; }
    set { _trueDescription = value; }
  }

  /// <summary> Gets or sets the description displayed when the checkbox is set to <see langword="false"/>. </summary>
  /// <value> 
  ///   The text displayed for <see langword="false"/>. The default value is an empty <see cref="String"/>.
  ///   In case of the default value, the text is read from the resources for this control.
  /// </value>
  [Description("The description displayed when the checkbox is set to False.")]
  [Category ("Appearance")]
  [DefaultValue("")]
  public string FalseDescription
  {
    get { return _falseDescription; }
    set { _falseDescription = value; }
  }

  /// <summary> Gets or sets the description displayed when the checkbox is set to <see langword="null"/>. </summary>
  /// <value> 
  ///   The text displayed for <see langword="null"/>. The default value is an empty <see cref="String"/>.
  ///   In case of the default value, the text is read from the resources for this control.
  /// </value>
  [Description("The description displayed when the checkbox is set to null.")]
  [Category ("Appearance")]
  [DefaultValue("")]
  public string NullDescription
  {
    get { return _nullDescription; }
    set { _nullDescription = value; }
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
  /// <summary> Gets the CSS-Class applied to the <see cref="BocBooleanValue"/> itself. </summary>
  /// <remarks> 
  ///   <para> Class: <c>bocBooleanValue</c>. </para>
  ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassBase
  { get { return "bocBooleanValue"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocBooleanValue"/> when it is displayed in read-only mode. </summary>
  /// <remarks> 
  ///   <para> Class: <c>readOnly</c>. </para>
  ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocBooleanValue.readOnly</c> as a selector. </para>
  /// </remarks>
  protected virtual string CssClassReadOnly
  { get { return "readOnly"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocBooleanValue"/> when it is displayed in disabled. </summary>
  /// <remarks> 
  ///   <para> Class: <c>disabled</c>. </para>
  ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocBooleanValue.disabled</c> as a selector. </para>
  /// </remarks>
  protected virtual string CssClassDisabled
  { get { return "disabled"; } }
  #endregion
}
}
