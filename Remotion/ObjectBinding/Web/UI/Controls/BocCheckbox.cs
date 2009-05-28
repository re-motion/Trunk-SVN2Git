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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
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

/// <summary> This control can be used to display or edit a boolean value (true or false). </summary>
/// <include file='doc\include\UI\Controls\BocCheckBox.xml' path='BocCheckBox/Class/*' />
[ValidationProperty ("ValidationValue")]
[DefaultEvent ("SelectionChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocCheckBox: BocBooleanValueBase
{
  // constants
  
  private const string c_scriptFileUrl = "BocCheckBox.js";

  private const string c_trueIcon = "CheckBoxTrue.gif";
  private const string c_falseIcon = "CheckBoxFalse.gif";

  // types

  /// <summary> A list of control specific resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
  ///   See the documentation of <b>GetString</b> for further details.
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Remotion.ObjectBinding.Web.Globalization.BocCheckBox")]
  protected enum ResourceIdentifier
  {
    /// <summary> The descripton rendered next the check box when it is checked. </summary>
    TrueDescription,
    /// <summary> The descripton rendered next the check box when it is not checked.  </summary>
    FalseDescription,
  }

  // static members
	
  private static readonly Type[] s_supportedPropertyInterfaces = new[] { 
      typeof (IBusinessObjectBooleanProperty) };

  private static readonly string s_scriptFileKey = typeof (BocCheckBox).FullName + "_Script";
  private static readonly string s_startUpScriptKey = typeof (BocCheckBox).FullName+ "_Startup";

	// member fields

  private bool _value;
  private bool? _defaultValue;
  private bool _isActive = true;

  private readonly HtmlInputCheckBox _checkBox;
  private readonly Image _image;
  private readonly Label _label;
  private readonly Style _labelStyle;

  private bool? _showDescription;

  // construction and disposing

	public BocCheckBox()
	{
    _labelStyle = new Style();
    _checkBox = new HtmlInputCheckBox();
    _image = new Image();
    _label = new Label();
	}

  // methods and properties

  protected override void CreateChildControls()
  {
    _checkBox.ID = ID + "_Boc_CheckBox";
    _checkBox.EnableViewState = false;
    Controls.Add (_checkBox);

    _image.ID = ID + "_Boc_Image";
    _image.EnableViewState = false;
    Controls.Add (_image);

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
      string scriptUrl = ResourceUrlResolver.GetResourceUrl (this, context, typeof (BocCheckBox), ResourceType.Html, c_scriptFileUrl);
      HtmlHeadAppender.Current.RegisterJavaScriptInclude (s_scriptFileKey, scriptUrl);
    }
  }

  /// <summary>
  ///   Uses the <paramref name="postCollection"/> to determine whether the value of this control has been changed 
  ///   between postbacks.
  /// </summary>
  /// <include file='doc\include\UI\Controls\BocCheckBox.xml' path='BocCheckBox/LoadPostData/*' />
  protected override bool LoadPostData (string postDataKey, NameValueCollection postCollection)
  {
    if (! _isActive)
      return false;

    string newValue = PageUtility.GetPostBackCollectionItem ((Page)Page, _checkBox.UniqueID);
    bool newBooleanValue = ! StringUtility.IsNullOrEmpty (newValue);
    bool isDataChanged = _value != newBooleanValue;
    if (isDataChanged)
    {
      _value = newBooleanValue;
      IsDirty = true;
    }
    return isDataChanged;
  }

  /// <summary> Called when the state of the control has changed between postbacks. </summary>
  protected override void RaisePostDataChangedEvent()
  {
    if (! IsReadOnly && Enabled)
      OnCheckedChanged();
  }

  /// <summary> Checks whether the control conforms to the required WAI level. </summary>
  /// <exception cref="WcagException"> Thrown if the control does not conform to the required WAI level. </exception>
  protected virtual void EvaluateWaiConformity ()
  {
    if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
    {
      if (_showDescription == true)
        WcagHelper.Instance.HandleError (1, this, "ShowDescription");

      if (IsAutoPostBackEnabled)
        WcagHelper.Instance.HandleWarning (1, this, "AutoPostBack");
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
    _checkBox.Checked = _value;
  }

  protected override void OnPreRender (EventArgs e)
  {
    EnsureChildControls();
    base.OnPreRender (e);

    LoadResources (GetResourceManager());

    bool isReadOnly = IsReadOnly;
    if (isReadOnly)
      PreRenderReadOnlyMode();
    else
      PreRenderEditMode();

    _isActive = ! isReadOnly && Enabled;
  }

  /// <summary> Pre-renders the child controls for edit mode. </summary>
  private void PreRenderEditMode()
  {
    string trueDescription = null;
    string falseDescription = null;
    string defaultTrueDescription = null;
    string defaultFalseDescription = null;
    if (IsDescriptionEnabled)
    {
      IResourceManager resourceManager = GetResourceManager();
      defaultTrueDescription = resourceManager.GetString (ResourceIdentifier.TrueDescription);
      defaultFalseDescription = resourceManager.GetString (ResourceIdentifier.FalseDescription);

      trueDescription = (StringUtility.IsNullOrEmpty (TrueDescription) ? defaultTrueDescription : TrueDescription);
      falseDescription = (StringUtility.IsNullOrEmpty (FalseDescription) ? defaultFalseDescription : FalseDescription);
    }
    string description = _value ? trueDescription : falseDescription;

    DetermineClientScriptLevel();

    if (HasClientScript)
    {
      string checkBoxScript;
      string labelScript;

      if (Enabled)
      {
        if (!Page.ClientScript.IsStartupScriptRegistered (s_startUpScriptKey))
        {
          string startupScript = string.Format (
              "BocCheckBox_InitializeGlobals ('{0}', '{1}');",
              defaultTrueDescription, defaultFalseDescription);
          Page.ClientScript.RegisterStartupScriptBlock (this, s_startUpScriptKey, startupScript);
        }
      
        string label = IsDescriptionEnabled ? "document.getElementById ('" + Label.ClientID + "')" : "null";
        string checkBox = "document.getElementById ('" + _checkBox.ClientID + "')";
        string script = " ("
                        + checkBox + ", "
                        + label + ", "
                        + (StringUtility.IsNullOrEmpty (TrueDescription) ? "null" : "'" + TrueDescription + "'") + ", "
                        + (StringUtility.IsNullOrEmpty (FalseDescription) ? "null" : "'" + FalseDescription + "'") + ");";

        if (IsAutoPostBackEnabled)
          script += Page.ClientScript.GetPostBackEventReference (this, "") + ";";
        checkBoxScript = "BocCheckBox_OnClick" + script;
        labelScript = "BocCheckBox_ToggleCheckboxValue" + script;
      }
      else
      {
        checkBoxScript = "return false;";
        labelScript = "return false;";
      }
      _checkBox.Attributes.Add ("onclick", checkBoxScript);
      Label.Attributes.Add ("onclick", labelScript);
    }

    SetEditModeValue ();
    _checkBox.Disabled = ! Enabled;
    
    if (IsDescriptionEnabled)
    {
      Label.Text = description;
      Label.Width = Unit.Empty;
      Label.Height = Unit.Empty;
      Label.ApplyStyle (LabelStyle);
    }
  }

  /// <summary> Pre-renders the child controls for read-only mode. </summary>
  private void PreRenderReadOnlyMode()
  {
    string imageUrl = ResourceUrlResolver.GetResourceUrl (
        this,
        Context, 
        typeof (BocCheckBox), 
        ResourceType.Image, 
        _value ? c_trueIcon : c_falseIcon);

    string description;
    if (_value)
    {
      if (StringUtility.IsNullOrEmpty (TrueDescription))
        description = GetResourceManager().GetString (ResourceIdentifier.TrueDescription);
      else
        description = TrueDescription;
    }
    else
    {
      if (StringUtility.IsNullOrEmpty (FalseDescription))
        description = GetResourceManager().GetString (ResourceIdentifier.FalseDescription);
      else
        description = FalseDescription;
    }
    Image.ImageUrl = imageUrl;
    Image.AlternateText = description;
    Image.Style["vertical-align"] = "middle";

    if (IsDescriptionEnabled)
    {
      Label.Text = description;
      Label.Width = Unit.Empty;
      Label.Height = Unit.Empty;
      Label.ApplyStyle (LabelStyle);
    }
  }

  protected override void RenderContents(HtmlTextWriter writer)
  {
    EvaluateWaiConformity ();

    if (IsReadOnly)
    {
      Image.RenderControl (writer);
      Label.RenderControl (writer);
    }
    else
    {
      _checkBox.RenderControl (writer);
      Label.RenderControl (writer);
    }
  }

  protected override void LoadControlState (object savedState)
  {
    object[] values = (object[]) savedState;

    base.LoadControlState (values[0]);
    _value = (bool) values[1];
    _isActive = (bool) values[2];

    _checkBox.Checked = _value;
  }

  protected override object SaveControlState ()
  {
    object[] values = new object[3];

    values[0] = base.SaveControlState ();
    values[1] = _value;
    values[2] = _isActive;

    return values;
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
  }

  private void DetermineClientScriptLevel() 
  {
    HasClientScript = false;

    if (! IsDesignMode )
    {
      HasClientScript = true;
    }
  }

  /// <summary> Gets a flag that determines whether the control is to be treated as a required value. </summary>
  /// <value> Always <see langword="false"/> since the checkbox has no undefined state in the user interface. </value>
  [Browsable(false)]
  public override bool IsRequired 
  {
    get { return false; }
  }

  /// <summary> Gets or sets the current value. </summary>
  /// <value> 
  ///   The boolean value currently displayed. If <see langword="null"/> is assigned, <see cref="GetDefaultValue"/>
  ///   is evaluated to get the value. The <see cref="BusinessObjectBoundEditableWebControl.IsDirty"/> flag is set 
  ///   in this case.
  /// </value>
  /// <remarks> The dirty state is reset when the value is set. </remarks>
  [Browsable(false)]
  public override bool? Value
  {
    get
    {
      return _value;
    }
    set
    {
      IsDirty = true;
      _value = value ?? GetDefaultValue();
    }
  }

  /// <summary> The boolean value to which this control defaults if the assigned value is <see langword="null"/>. </summary>
  /// <value> 
  ///   <see langword="true"/> or <see langword="false"/> to explicitly specify the default value, or <see langword="null"/> to leave the decision 
  ///   to the object model. If the control  is unbound and no default value is specified, <see langword="false"/> is assumed as default value.
  /// </value>
  [Category("Behavior")]
  [Description("The boolean value to which this control defaults if the assigned value is null.")]
  [NotifyParentProperty(true)]
  [DefaultValue (typeof (bool?), "")]
  public bool? DefaultValue
  {
    get { return _defaultValue; }
    set { _defaultValue = value; }
  }

  /// <summary>
  ///   Evaluates the default value settings using the <see cref="DefaultValue"/> and the <see cref="BusinessObjectBoundWebControl.Property"/>'s
  ///   default value.
  /// </summary>
  /// <returns>
  ///   <list type="bullet">
  ///     <item> 
  ///       If <see cref="DefaultValue"/> is set to <see langword="true"/> or 
  ///       <see langword="false"/>, <see langword="true"/> or <see langword="false"/> is returned 
  ///       respectivly.
  ///     </item>
  ///     <item>
  ///       If <see cref="DefaultValue"/> is set to <see langword="null"/>, the <see cref="BusinessObjectBoundWebControl.Property"/>
  ///       is queried for its default value using the <see cref="IBusinessObjectBooleanProperty.GetDefaultValue"/>
  ///       method.
  ///       <list type="bullet">
  ///         <item> 
  ///           If <see cref="IBusinessObjectBooleanProperty.GetDefaultValue"/> returns 
  ///           <see langword="true"/> or <see langword="false"/>, <see langword="true"/> or 
  ///           <see langword="false"/> is returned respectivly.
  ///         </item>
  ///         <item>
  ///           Otherwise <see langword="false"/> is returned.
  ///         </item>
  ///       </list>
  ///     </item>
  ///   </list>
  /// </returns>
  protected bool GetDefaultValue()
  {
    if (_defaultValue == null)
    {
      if (DataSource != null && DataSource.BusinessObjectClass != null && DataSource.BusinessObject != null && Property != null)
        return Property.GetDefaultValue (DataSource.BusinessObjectClass) ?? false;
      else
        return false;
    }
    else
    {
      return _defaultValue == true ? true : false;
    }

  }

  /// <summary> 
  ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user 
  ///   interface.
  /// </summary>
  /// <returns> 
  ///   A <see cref="String"/> <see cref="Array"/> containing the <see cref="Control.ClientID"/> of the
  ///   <see cref="CheckBox"/> if the control is in edit mode, or an empty array if the control is read-only.
  /// </returns>
  /// <seealso cref="BusinessObjectBoundEditableWebControl.GetTrackedClientIDs">BusinessObjectBoundEditableWebControl.GetTrackedClientIDs</seealso>
  public override string[] GetTrackedClientIDs()
  {
    return IsReadOnly ? new string[0] : new[] { _checkBox.ClientID };
  }

  /// <summary>
  ///   The <see cref="BocCheckBox"/> supports properties of type <see cref="IBusinessObjectBooleanProperty"/>.
  /// </summary>
  /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
  protected override Type[] SupportedPropertyInterfaces
  {
    get { return s_supportedPropertyInterfaces; }
  }

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its 
  ///   <see cref="Control.ClientID"/>.
  /// </summary>
  /// <value> The <see cref="HyperLink"/> if the control is in edit mode, otherwise the control itself. </value>
  public override Control TargetControl 
  {
    get { return IsReadOnly ? (Control) this : _checkBox; }
  }

  /// <summary> Gets the ID of the element to receive the focus when the page is loaded. </summary>
  /// <value>
  ///   Returns the <see cref="Control.ClientID"/> of the <see cref="CheckBox"/> if the control is in edit mode, 
  ///   otherwise <see langword="null"/>. 
  /// </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public override string FocusID
  { 
    get { return IsReadOnly ? null : _checkBox.ClientID; }
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
  public bool ValidationValue
  {
    get { return _checkBox.Checked; }
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
  public override Label Label
  {
    get { return _label; }
  }

  /// <summary> Gets the <see cref="System.Web.UI.HtmlControls.HtmlInputCheckBox"/> used for the value in edit mode. </summary>
  [Browsable (false)]
  public HtmlInputCheckBox CheckBox
  {
    get { return _checkBox; }
  }

  /// <summary> Gets the <see cref="System.Web.UI.WebControls.Image"/> used for the value in read-only mode. </summary>
  [Browsable (false)]
  public Image Image
  {
    get { return _image; }
  }

  /// <summary> Gets or sets the flag that determines whether to show the description next to the checkbox. </summary>
  /// <value> 
  ///   <see langword="true"/> to enable the description. 
  ///   Defaults to <see langword="null"/>, which is interpreted as <see langword="false"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="IsDescriptionEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to show the description next to the checkbox. Undefined is interpreted as false.")]
  [Category ("Appearance")]
  [DefaultValue (typeof (bool?), "")]
  public bool? ShowDescription
  {
    get { return _showDescription; }
    set { _showDescription = value; }
  }

  /// <summary> Gets the evaluated value for the <see cref="ShowDescription"/> property. </summary>
  /// <value>
  ///   <see langowrd="true"/> if WAI conformity is not required 
  ///   and <see cref="ShowDescription"/> is <see langword="true"/>. 
  /// </value>
  protected bool IsDescriptionEnabled
  {
    get { return ! WcagHelper.Instance.IsWaiConformanceLevelARequired() && _showDescription == true;}
  }

  #region protected virtual string CssClass...
  /// <summary> Gets the CSS-Class applied to the <see cref="BocCheckBox"/> itself. </summary>
  /// <remarks> 
  ///   <para> Class: <c>bocCheckBox</c>. </para>
  ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
  /// </remarks>
  protected override string CssClassBase
  { get { return "bocCheckBox"; } }

  #endregion
}

}
