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
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Logging;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocReferenceValue;
using Remotion.ObjectBinding.Web.UI.Design;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> This control can be used to display or edit reference values. </summary>
  /// <include file='doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/Class/*' />
  // TODO: see "Doc\Bugs and ToDos.txt"
  [ValidationProperty ("BusinessObjectUniqueIdentifier")]
  [DefaultEvent ("SelectionChanged")]
  [ToolboxItemFilter ("System.Web.UI")]
  [Designer (typeof (BocReferenceValueDesigner))]
  public class BocReferenceValue
      :
          BocReferenceValueBase,
          IBocReferenceValue,
          IResourceDispatchTarget,
          IFocusableControl
  {
    // constants

    /// <summary> The text displayed when control is displayed in desinger, is read-only, and has no contents. </summary>
    private const string c_designModeEmptyLabelContents = "##";

    /// <summary> The key identifying a options menu item resource entry. </summary>
    private const string c_resourceKeyOptionsMenuItems = "OptionsMenuItems";

    /// <summary> The key identifying the command resource entry. </summary>
    private const string c_resourceKeyCommand = "Command";

    private const string c_dropDownListIDPostfix = "Boc_DropDownList";

    // types

    /// <summary> A list of control specific resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources ("Remotion.ObjectBinding.Web.Globalization.BocReferenceValue")]
    protected enum ResourceIdentifier
    {
      /// <summary> Label displayed in the OptionsMenu. </summary>
      OptionsTitle,
      /// <summary> The validation error message displayed when the null item is selected. </summary>
      NullItemValidationMessage
    }

    // static members

    private static readonly ILog s_log = LogManager.GetLogger (MethodBase.GetCurrentMethod().DeclaringType);

    // member fields

    private bool _isBusinessObejectListPopulated;

    private readonly Style _commonStyle;
    private readonly DropDownListStyle _dropDownListStyle;
    private readonly Style _labelStyle;

    /// <summary> 
    ///   The object returned by <see cref="BocReferenceValue"/>. 
    ///   Does not require <see cref="System.Runtime.Serialization.ISerializable"/>. 
    /// </summary>
    private IBusinessObjectWithIdentity _value;

    private string _displayName;
    private readonly ListItemCollection _listItems;

    private string _select = String.Empty;
    private bool? _enableSelectStatement;

    // construction and disposing

    public BocReferenceValue ()
    {
      EnableIcon = true;
      _listItems = new ListItemCollection();
      _commonStyle = new Style();
      _dropDownListStyle = new DropDownListStyle();
      _labelStyle = new Style();
    }

    // methods and properties

    public override void RegisterHtmlHeadContents (IHttpContext httpContext, HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("httpContext", httpContext);
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      base.RegisterHtmlHeadContents (httpContext, htmlHeadAppender);

      var factory = ServiceLocator.GetInstance<IBocReferenceValueRendererFactory>();
      var preRenderer = factory.CreatePreRenderer (httpContext, this);
      preRenderer.RegisterHtmlHeadContents (htmlHeadAppender);
    }

    /// <remarks>
    ///   If the <see cref="DropDownList"/> could not be created from <see cref="DropDownListStyle"/>,
    ///   the control is set to read-only.
    /// </remarks>
    protected override void CreateChildControls ()
    {
      base.CreateChildControls();
      ((IStateManager) _listItems).TrackViewState();
    }

    /// <remarks> Populates the list. </remarks>
    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (! ControlExistedInPreviousRequest)
        EnsureBusinessObjectListPopulated();
    }

    protected override string ValueContainingControlID
    {
      get { return DropDownListUniqueID; }
    }

    protected override void OnDataChanged ()
    {
    }

    public string DropDownListUniqueID
    {
      get { return UniqueID + IdSeparator + c_dropDownListIDPostfix; }
    }

    /// <summary> Called when the state of the control has changed between postbacks. </summary>
    protected override void RaisePostDataChangedEvent ()
    {
      if (InternalValue == null)
        _displayName = null;
      else
      {
        ListItem selectedItem = _listItems.FindByValue (InternalValue);
        if (selectedItem == null)
          throw new InvalidOperationException (string.Format ("The key '{0}' does not correspond to a known element.", InternalValue));
        _displayName = selectedItem.Text;
      }

      if (! IsReadOnly && Enabled)
        OnSelectionChanged();
    }

    /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
    /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
    void IResourceDispatchTarget.Dispatch (IDictionary values)
    {
      ArgumentUtility.CheckNotNull ("values", values);
      Dispatch (values);
    }

    /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
    /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
    protected virtual void Dispatch (IDictionary values)
    {
      HybridDictionary optionsMenuItemValues = new HybridDictionary();
      HybridDictionary propertyValues = new HybridDictionary();
      HybridDictionary commandValues = new HybridDictionary();

      //  Parse the values

      foreach (DictionaryEntry entry in values)
      {
        string key = (string) entry.Key;
        string[] keyParts = key.Split (new[] { ':' }, 3);

        //  Is a property/value entry?
        if (keyParts.Length == 1)
        {
          string property = keyParts[0];
          propertyValues.Add (property, entry.Value);
        }
            //  Is compound element entry
        else if (keyParts.Length == 2)
        {
          //  Compound key: "elementID:property"
          string elementID = keyParts[0];
          string property = keyParts[1];

          //  Switch to the right collection
          switch (elementID)
          {
            case c_resourceKeyCommand:
            {
              commandValues.Add (property, entry.Value);
              break;
            }
            default:
            {
              //  Invalid collection property
              s_log.Debug (
                  "BocReferenceValue '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page
                  + "' does not contain an element named '" + elementID + "'.");
              break;
            }
          }
        }
            //  Is collection entry?
        else if (keyParts.Length == 3)
        {
          //  Compound key: "collectionID:elementID:property"
          string collectionID = keyParts[0];
          string elementID = keyParts[1];
          string property = keyParts[2];

          IDictionary currentCollection = null;

          //  Switch to the right collection
          switch (collectionID)
          {
            case c_resourceKeyOptionsMenuItems:
            {
              currentCollection = optionsMenuItemValues;
              break;
            }
            default:
            {
              //  Invalid collection property
              s_log.Debug (
                  "BocReferenceValue '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page
                  + "' does not contain a collection property named '" + collectionID + "'.");
              break;
            }
          }

          //  Add the property/value pair to the collection
          if (currentCollection != null)
          {
            //  Get the dictonary for the current element
            IDictionary elementValues = (IDictionary) currentCollection[elementID];

            //  If no dictonary exists, create it and insert it into the elements hashtable.
            if (elementValues == null)
            {
              elementValues = new HybridDictionary();
              currentCollection[elementID] = elementValues;
            }

            //  Insert the argument and resource's value into the dictonary for the specified element.
            elementValues.Add (property, entry.Value);
          }
        }
        else
        {
          //  Not supported format or invalid property
          s_log.Debug (
              "BocReferenceValue '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page
              + "' received a resource with an invalid or unknown key '" + key
              + "'. Required format: 'property' or 'collectionID:elementID:property'.");
        }
      }

      //  Dispatch simple properties
      ResourceDispatcher.DispatchGeneric (this, propertyValues);

      //  Dispatch compound element properties
      if (Command != null)
        ResourceDispatcher.DispatchGeneric (Command, commandValues);

      //  Dispatch to collections
      OptionsMenuItems.Dispatch (optionsMenuItemValues, this, "OptionsMenuItems");
    }

    /// <summary> Loads the resources into the control's properties. </summary>
    protected override void LoadResources (IResourceManager resourceManager)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);

      base.LoadResources (resourceManager);

      string key;
      key = ResourceManagerUtility.GetGlobalResourceKey (Select);
      if (! StringUtility.IsNullOrEmpty (key))
        Select = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (OptionsTitle);
      if (! StringUtility.IsNullOrEmpty (key))
        OptionsTitle = resourceManager.GetString (key);

      if (Command != null)
        Command.LoadResources (resourceManager);

      OptionsMenuItems.LoadResources (resourceManager);
    }

    /// <summary> Checks whether the control conforms to the required WAI level. </summary>
    /// <exception cref="WcagException"> Thrown if the control does not conform to the required WAI level. </exception>
    protected virtual void EvaluateWaiConformity ()
    {
      if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
      {
        if (ShowOptionsMenu)
          WcagHelper.Instance.HandleError (1, this, "ShowOptionsMenu");
        bool hasPostBackCommand = Command != null && (Command.Type == CommandType.Event || Command.Type == CommandType.WxeFunction);
        if (hasPostBackCommand)
          WcagHelper.Instance.HandleError (1, this, "Command");

        if (DropDownListStyle.AutoPostBack == true)
          WcagHelper.Instance.HandleWarning (1, this, "DropDownListStyle.AutoPostBack");
      }
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      var factory = ServiceLocator.GetInstance<IBocReferenceValueRendererFactory>();
      var preRenderer = factory.CreatePreRenderer (Context, this);
      preRenderer.PreRender();
    }

    protected override void Render (HtmlTextWriter writer)
    {
      EvaluateWaiConformity();

      var factory = ServiceLocator.GetInstance<IBocReferenceValueRendererFactory>();
      var renderer = factory.CreateRenderer (Context, writer, this);
      renderer.Render();
    }

    protected override void LoadControlState (object savedState)
    {
      object[] values = (object[]) savedState;

      base.LoadControlState (values[0]);
      if (values[1] != null)
        InternalValue = (string) values[1];
      _displayName = (string) values[2];
      ((IStateManager) _listItems).LoadViewState (values[3]);
    }

    protected override object SaveControlState ()
    {
      object[] values = new object[4];

      values[0] = base.SaveControlState();
      values[1] = InternalValue;
      values[2] = _displayName;
      values[3] = ((IStateManager) _listItems).SaveViewState();

      return values;
    }


    /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/LoadValue/*' />
    public override void LoadValue (bool interim)
    {
      if (! interim)
      {
        if (Property != null && DataSource != null && DataSource.BusinessObject != null)
        {
          IBusinessObjectWithIdentity value = (IBusinessObjectWithIdentity) DataSource.BusinessObject.GetProperty (Property);
          LoadValueInternal (value, interim);
        }
      }
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value"> 
    ///   The object implementing <see cref="IBusinessObjectWithIdentity"/> to load, or <see langword="null"/>. 
    /// </param>
    /// <include file='doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/LoadUnboundValue/*' />
    public void LoadUnboundValue (IBusinessObjectWithIdentity value, bool interim)
    {
      LoadValueInternal (value, interim);
    }

    /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
    protected virtual void LoadValueInternal (IBusinessObjectWithIdentity value, bool interim)
    {
      if (! interim)
      {
        Value = value;
        IsDirty = false;
      }
    }

    /// <summary> Saves the <see cref="Value"/> into the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/SaveValue/*' />
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
    protected override IResourceManager GetResourceManager ()
    {
      return GetResourceManager (typeof (ResourceIdentifier));
    }

    protected override string GetSelectionCountFunction ()
    {
      return "function() { return BocReferenceValue_GetSelectionCount ('" + DropDownListClientID + "'); }";
    }

    protected override string GetOptionsMenuTitle ()
    {
      return GetResourceManager().GetString (ResourceIdentifier.OptionsTitle);
    }

    /// <summary> Creates the list of validators required for the current binding and property settings. </summary>
    /// <include file='doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/CreateValidators/*' />
    public override BaseValidator[] CreateValidators ()
    {
      if (IsReadOnly || ! IsRequired)
        return new BaseValidator[0];

      BaseValidator[] validators = new BaseValidator[1];

      CompareValidator notNullItemValidator = new CompareValidator();
      notNullItemValidator.ID = ID + "_ValidatorNotNullItem";
      notNullItemValidator.ControlToValidate = TargetControl.ID;
      notNullItemValidator.ValueToCompare = c_nullIdentifier;
      notNullItemValidator.Operator = ValidationCompareOperator.NotEqual;
      if (string.IsNullOrEmpty (ErrorMessage))
      {
        notNullItemValidator.ErrorMessage =
            GetResourceManager().GetString (ResourceIdentifier.NullItemValidationMessage);
      }
      else
        notNullItemValidator.ErrorMessage = ErrorMessage;
      validators[0] = notNullItemValidator;

      Validators.AddRange (validators);
      return validators;
    }

    /// <summary> Sets the <see cref="IBusinessObjectWithIdentity"/> objects to be displayed in edit mode. </summary>
    /// <remarks>
    ///   Use this method to set the listed items, e.g. from the parent control, if no <see cref="Select"/>
    ///   statement was provided.
    /// </remarks>
    /// <param name="businessObjects">
    ///   An array of <see cref="IBusinessObjectWithIdentity"/> objects to be used as list of possible values.
    ///   Must not be <see langword="null"/>.
    /// </param>
    public void SetBusinessObjectList (IBusinessObjectWithIdentity[] businessObjects)
    {
      ArgumentUtility.CheckNotNull ("businessObjects", businessObjects);
      RefreshBusinessObjectList (businessObjects);
    }

    /// <summary> Sets the <see cref="IBusinessObjectWithIdentity"/> objects to be displayed in edit mode. </summary>
    /// <remarks>
    ///   Use this method to set the listed items, e.g. from the parent control, if no <see cref="Select"/>
    ///   statement was provided.
    /// </remarks>
    /// <param name="businessObjects">
    ///   An <see cref="IList"/> of <see cref="IBusinessObjectWithIdentity"/> objects to be used as list of possible 
    ///   values. Must not be <see langword="null"/>.
    /// </param>
    public void SetBusinessObjectList (IList businessObjects)
    {
      ArgumentUtility.CheckNotNull ("businessObjects", businessObjects);
      ArgumentUtility.CheckItemsNotNullAndType ("businessObjects", businessObjects, typeof (IBusinessObjectWithIdentity));
      RefreshBusinessObjectList (businessObjects);
    }

    /// <summary> Clears the list of <see cref="IBusinessObjectWithIdentity"/> objects to be displayed in edit mode. </summary>
    /// <remarks> If the value is not required, the null item will displayed anyway. </remarks>
    public void ClearBusinessObjectList ()
    {
      RefreshBusinessObjectList (null);
    }

    /// <summary> Calls <see cref="PopulateBusinessObjectList"/> if the list has not yet been populated. </summary>
    protected void EnsureBusinessObjectListPopulated ()
    {
      if (_isBusinessObejectListPopulated)
        return;
      PopulateBusinessObjectList();
    }


    /// <summary>
    ///   Queries <see cref="IBusinessObjectReferenceProperty.SearchAvailableObjects"/> for the
    ///   <see cref="IBusinessObjectWithIdentity"/> objects to be displayed in edit mode and sets the list with the
    ///   objects returned by the query.
    /// </summary>
    /// <remarks> 
    ///   <para>
    ///     Uses the <see cref="Select"/> statement to query the <see cref="BocReferenceValueBase.Property"/>'s 
    ///     <see cref="IBusinessObjectReferenceProperty.SearchAvailableObjects"/> method for the list contents.
    ///   </para><para>
    ///     Only populates the list if <see cref="EnableSelectStatement"/> is not <see langword="false"/>.
    ///     Otherwise the list will be left empty.
    ///   </para>  
    /// </remarks>
    protected void PopulateBusinessObjectList ()
    {
      if (! IsSelectStatementEnabled)
        return;

      if (Property == null)
        return;

      IBusinessObject[] businessObjects = null;

      //  Get all matching business objects
      if (DataSource != null)
        businessObjects = Property.SearchAvailableObjects (DataSource.BusinessObject, new DefaultSearchArguments (_select));

      RefreshBusinessObjectList (ArrayUtility.Convert<IBusinessObject, IBusinessObjectWithIdentity> (businessObjects));
    }

    /// <summary> Populates the <see cref="DropDownList"/> with the items passed in <paramref name="businessObjects"/>. </summary>
    /// <param name="businessObjects">
    ///   The <see cref="IList"/> of <see cref="IBusinessObjectWithIdentity"/> objects to populate the 
    ///   <see cref="DropDownList"/>. Use <see langword="null"/> to clear the list.
    /// </param>
    /// <remarks> This method controls the actual refilling of the <see cref="DropDownList"/>. </remarks>
    protected virtual void RefreshBusinessObjectList (IList businessObjects)
    {
      _isBusinessObejectListPopulated = true;
      _listItems.Clear();

      if (businessObjects != null)
      {
        foreach (IBusinessObjectWithIdentity businessObject in businessObjects)
        {
          ListItem item = new ListItem (GetDisplayName (businessObject), businessObject.UniqueIdentifier);
          _listItems.Add (item);
        }
      }
    }

    string IBocReferenceValue.GetLabelText ()
    {
      string text;
      if (InternalValue != null)
        text = HttpUtility.HtmlEncode (_displayName);
      else
        text = String.Empty;
      if (StringUtility.IsNullOrEmpty (text))
      {
        if (IsDesignMode)
        {
          text = c_designModeEmptyLabelContents;
          //  Too long, can't resize in designer to less than the content's width
          //  _label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
        }
        else
          text = "&nbsp;";
      }
      return text;
    }

    /// <summary> Creates the <see cref="ListItem"/> symbolizing the undefined selection. </summary>
    /// <returns> A <see cref="ListItem"/>. </returns>
    private ListItem CreateNullItem ()
    {
      ListItem emptyItem = new ListItem (string.Empty, c_nullIdentifier);
      return emptyItem;
    }

    /// <summary> Gets or sets the current value. </summary>
    /// <include file='doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/Value/*' />
    [Browsable (false)]
    public override IBusinessObjectWithIdentity Value
    {
      get
      {
        if (InternalValue == null)
          _value = null;
        else if (Property != null && (_value == null || _value.UniqueIdentifier != InternalValue))
        {
          //  Only reload if value is outdated
          _value = ((IBusinessObjectClassWithIdentity) Property.ReferenceClass).GetObject (InternalValue);
        }

        return _value;
      }
      set
      {
        IsDirty = true;

        _value = value;

        if (value != null)
        {
          InternalValue = value.UniqueIdentifier;
          _displayName = GetDisplayName (value);
        }
        else
        {
          InternalValue = null;
          _displayName = null;
        }
      }
    }

    /// <summary> 
    ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user interface.
    /// </summary>
    /// <returns> 
    ///   A <see cref="String"/> <see cref="Array"/> containing the <see cref="Control.ClientID"/> of the
    ///   <see cref="DropDownList"/> if the control is in edit mode, or an empty array if the control is read-only.
    /// </returns>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.GetTrackedClientIDs">BusinessObjectBoundEditableWebControl.GetTrackedClientIDs</seealso>
    public override string[] GetTrackedClientIDs ()
    {
      return IsReadOnly ? new string[0] : new[] { DropDownListClientID };
    }

    /// <summary> Gets the ID of the element to receive the focus when the page is loaded. </summary>
    /// <value>
    ///   Returns the <see cref="Control.ClientID"/> of the <see cref="DropDownList"/> if the control is in edit mode, 
    ///   otherwise <see langword="null"/>. 
    /// </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public string FocusID
    {
      get { return IsReadOnly ? null : DropDownListClientID; }
    }

    /// <summary>
    ///   Gets the style that you want to apply to the <see cref="DropDownList"/> (edit mode) 
    ///   and the <see cref="Label"/> (read-only mode).
    /// </summary>
    /// <remarks>
    ///   Use the <see cref="DropDownListStyle"/> and <see cref="LabelStyle"/> to assign individual 
    ///   style settings for the respective modes. Note that if you set one of the <b>Font</b> 
    ///   attributes (Bold, Italic etc.) to <see langword="true"/>, this cannot be overridden using 
    ///   <see cref="DropDownListStyle"/> and <see cref="LabelStyle"/>  properties.
    /// </remarks>
    [Category ("Style")]
    [Description ("The style that you want to apply to the DropDownList (edit mode) and the Label (read-only mode).")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public Style CommonStyle
    {
      get { return _commonStyle; }
    }

    /// <summary> Gets the style that you want to apply to the <see cref="DropDownList"/> (edit mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
    [Category ("Style")]
    [Description ("The style that you want to apply to the DropDownList (edit mode) only.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public DropDownListStyle DropDownListStyle
    {
      get { return _dropDownListStyle; }
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

    /// <summary> The search expression used to populate the selection list in edit mode. </summary>
    /// <value> A <see cref="String"/> with a valid search expression. The default value is an empty <see cref="String"/>. </value>
    /// <remarks> A valid <see cref="BocReferenceValueBase.Property"/> is required in order to populate the list using the search statement. </remarks>
    [Category ("Data")]
    [Description ("Set the search expression for populating the selection list.")]
    [DefaultValue ("")]
    public string Select
    {
      get { return _select; }
      set
      {
        if (value == null)
          _select = null;
        else
          _select = value.Trim();
      }
    }

    /// <summary> Gets or sets the flag that determines whether to evaluate the <see cref="Select"/> statement. </summary>
    /// <value> 
    ///   <see langword="true"/> to evaluate the select statement. 
    ///   Defaults to <see langword="null"/>, which is interpreted as <see langword="true"/>.
    /// </value>
    /// <remarks>
    ///   Use <see cref="IsSelectStatementEnabled"/> to evaluate this property.
    /// </remarks>
    [Description ("The flag that determines whether to evaluate the Select statement. Undefined is interpreted as true.")]
    [Category ("Behavior")]
    [DefaultValue (typeof (bool?), "")]
    public bool? EnableSelectStatement
    {
      get { return _enableSelectStatement; }
      set { _enableSelectStatement = value; }
    }

    /// <summary> Gets the evaluated value for the <see cref="EnableSelectStatement"/> property. </summary>
    /// <value>
    ///   <see langowrd="false"/> if <see cref="EnableSelectStatement"/> is <see langword="false"/>. 
    /// </value>
    protected bool IsSelectStatementEnabled
    {
      get { return _enableSelectStatement != false; }
    }

    void IBocReferenceValue.PopulateDropDownList (DropDownList dropDownList)
    {
      ArgumentUtility.CheckNotNull ("dropDownList", dropDownList);
      dropDownList.Items.Clear();

      bool isNullItem = (InternalValue == null);

      if (isNullItem || !IsRequired)
        dropDownList.Items.Add (CreateNullItem());

      foreach (ListItem listItem in _listItems)
        dropDownList.Items.Add (new ListItem (listItem.Text, listItem.Value));

      //  Check if null item is to be selected
      if (isNullItem)
        dropDownList.SelectedValue = c_nullIdentifier;
      else
      {
        if (dropDownList.Items.FindByValue (InternalValue) != null)
          dropDownList.SelectedValue = InternalValue;
        else if (Value != null)
        {
          //  Item not yet in the list but is a valid item.
          var businessObject = Value;

          var item = new ListItem (GetDisplayName (businessObject), businessObject.UniqueIdentifier);
          dropDownList.Items.Add (item);

          dropDownList.SelectedValue = InternalValue;
        }
      }
    }

    bool IBocReferenceValue.EmbedInOptionsMenu
    {
      get
      {
        return HasValueEmbeddedInsideOptionsMenu == true && HasOptionsMenu
               || HasValueEmbeddedInsideOptionsMenu == null && IsReadOnly && HasOptionsMenu;
      }
    }

    bool IBocRenderableControl.IsDesignMode
    {
      get { return IsDesignMode; }
    }

    bool IBocReferenceValue.HasOptionsMenu
    {
      get { return HasOptionsMenu; }
    }

    string IBocReferenceValue.InternalValue
    {
      get { return InternalValue; }
    }

    bool IBocReferenceValue.IsCommandEnabled (bool readOnly)
    {
      return IsCommandEnabled (readOnly);
    }

    DropDownMenu IBocReferenceValue.OptionsMenu
    {
      get { return OptionsMenu; }
    }

    IconInfo IBocReferenceValue.GetIcon (IBusinessObject value, IBusinessObjectProvider provider)
    {
      return GetIcon (value, provider);
    }

    public string DropDownListClientID
    {
      get { return ClientID + ClientIDSeparator + c_dropDownListIDPostfix; }
    }

    string IBocReferenceValue.LabelClientID
    {
      get { return ClientID + "_Boc_Label"; }
    }

    string IBocReferenceValue.IconClientID
    {
      get { return ClientID + "_Boc_Icon"; }
    }

    string IBocReferenceValue.NullIdentifier
    {
      get { return c_nullIdentifier; }
    }
  }
}