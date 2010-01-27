// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Design;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;
using StringArrayConverter=Remotion.Web.UI.Design.StringArrayConverter;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public abstract class BocReferenceValueBase : BusinessObjectBoundEditableWebControl, IPostBackDataHandler, IPostBackEventHandler, IBocMenuItemContainer
  {
    protected const string c_nullIdentifier = "==null==";
    private static readonly Type[] s_supportedPropertyInterfaces = new[] { typeof (IBusinessObjectReferenceProperty) };

    protected static readonly object SelectionChangedEvent = new object();
    protected static readonly object MenuItemClickEvent = new object();
    protected static readonly object CommandClickEvent = new object();
    private readonly DropDownMenu _optionsMenu;

    /// <summary> The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> of the current object. </summary>
    private string _internalValue;

    /// <summary> The command rendered for this reference value. </summary>
    private readonly SingleControlItemCollection _command;

    private string _errorMessage;
    private readonly ArrayList _validators;
    private string _optionsTitle;
    private bool _showOptionsMenu = true;
    private Unit _optionsMenuWidth = Unit.Empty;
    private bool? _hasValueEmbeddedInsideOptionsMenu;
    private string[] _hiddenMenuItems;

    protected BocReferenceValueBase ()
    {
      _optionsMenu = new DropDownMenu (this);
      _command = new SingleControlItemCollection (new BocCommand (), new[] { typeof (BocCommand) });
      _validators = new ArrayList ();

    }

    /// <summary>
    ///   The <see cref="BocReferenceValue"/> supports properties of types <see cref="IBusinessObjectReferenceProperty"/>.
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
    /// <value> Always <see langword="false"/>. </value>
    public override bool UseLabel
    {
      get { return false; }
    }

    protected abstract string ValueContainingControlID { get; }

    /// <summary> Gets or sets the current value. </summary>
    /// <value> 
    ///   The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> for the current 
    ///   <see cref="IBusinessObjectWithIdentity"/> object 
    ///   or <see langword="null"/> if no item / the null item is selected.
    /// </value>
    protected string InternalValue
    {
      get { return _internalValue; }
      set
      {
        _internalValue = StringUtility.EmptyToNull (value);
      }
    }

    bool IBocMenuItemContainer.IsReadOnly
    {
      get { return IsReadOnly; }
    }

    bool IBocMenuItemContainer.IsSelectionEnabled
    {
      get { return true; }
    }

    /// <summary> Gets or sets the <see cref="BocCommand"/> for this control's <see cref="Value"/>. </summary>
    /// <value> A <see cref="BocCommand"/>. </value>
    /// <remarks> This property is used for accessing the <see cref="BocCommand"/> at run time and for Designer support. </remarks>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Category ("Menu")]
    [Description ("The command rendered for this control's Value.")]
    [NotifyParentProperty (true)]
    public BocCommand Command
    {
      get { return (BocCommand) _command.ControlItem; }
      set
      {
        _command.ControlItem = value;
        if (value != null)
          _command.ControlItem.OwnerControl = this;
      }
    }

    /// <summary> Gets or sets the current value. </summary>
    /// <include file='doc\include\UI\Controls\BocReferenceValueBase.xml' path='BocReferenceValueBase/Value/*' />
    [Browsable (false)]
    public new abstract IBusinessObjectWithIdentity Value { get; set; }

    /// <summary> Gets or sets the encapsulated <see cref="BocCommand"/> for this control's <see cref="Value"/>. </summary>
    /// <value> 
    ///   A <see cref="SingleControlItemCollection"/> containing a <see cref="BocCommand"/> in its 
    ///   <see cref="SingleControlItemCollection.ControlItem"/> property.
    /// </value>
    /// <remarks> This property is used for persisting the <see cref="BocReferenceValueBase.Command"/> into the <b>ASPX</b> source code. </remarks>
    [PersistenceMode (PersistenceMode.InnerProperty)]
    [Browsable (false)]
    [NotifyParentProperty (true)]
    public SingleControlItemCollection PersistedCommand
    {
      get { return _command; }
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
        for (int i = 0; i < Validators.Count; i++)
        {
          BaseValidator validator = (BaseValidator) Validators[i];
          validator.ErrorMessage = _errorMessage;
        }
      }
    }

    /// <summary> Gets a flag describing whether the <see cref="OptionsMenu"/> is visible. </summary>
    protected bool HasOptionsMenu
    {
      get
      {
        return ! WcagHelper.Instance.IsWaiConformanceLevelARequired()
               && _showOptionsMenu && (OptionsMenuItems.Count > 0 || IsDesignMode)
               && OptionsMenu.IsBrowserCapableOfScripting;
      }
    }

    /// <summary> Gets the <see cref="DropDownMenu"/> offering additional commands for the current <see cref="Value"/>. </summary>
    protected DropDownMenu OptionsMenu
    {
      get { return _optionsMenu; }
    }

    /// <summary> Gets the <see cref="BocMenuItem"/> objects displayed in the <see cref="OptionsMenu"/>. </summary>
    [PersistenceMode (PersistenceMode.InnerProperty)]
    [ListBindable (false)]
    [Category ("Menu")]
    [Description ("The menu items displayed by options menu.")]
    [DefaultValue ((string) null)]
    [Editor (typeof (BocMenuItemCollectionEditor), typeof (UITypeEditor))]
    public WebMenuItemCollection OptionsMenuItems
    {
      get { return _optionsMenu.MenuItems; }
    }

    /// <summary> Gets or sets the text that is rendered as a label for the <see cref="OptionsMenu"/>. </summary>
    /// <value> 
    ///   The text rendered as the <see cref="OptionsMenu"/>'s label. The default value is an empty <see cref="String"/>. 
    /// </value>
    [Category ("Menu")]
    [Description ("The text that is rendered as a label for the options menu.")]
    [DefaultValue ("")]
    public string OptionsTitle
    {
      get { return _optionsTitle; }
      set { _optionsTitle = value; }
    }

    /// <summary> Gets or sets a flag that determines whether to display the <see cref="OptionsMenu"/>. </summary>
    /// <value> <see langword="true"/> to show the <see cref="OptionsMenu"/>. The default value is <see langword="true"/>. </value>
    [Category ("Menu")]
    [Description ("Enables the options menu.")]
    [DefaultValue (true)]
    public bool ShowOptionsMenu
    {
      get { return _showOptionsMenu; }
      set { _showOptionsMenu = value; }
    }

    /// <summary> Gets or sets the width of the options menu. </summary>
    /// <value> The <see cref="Unit"/> value used for the option menu's width. The default value is <b>undefined</b>. </value>
    [Category ("Menu")]
    [Description ("The width of the options menu.")]
    [DefaultValue (typeof (Unit), "")]
    public Unit OptionsMenuWidth
    {
      get { return _optionsMenuWidth; }
      set { _optionsMenuWidth = value; }
    }

    /// <summary> Gets or sets flag that determines whether to use the value as the <see cref="OptionsMenu"/>'s head. </summary>
    /// <value> 
    ///   <see langword="true"/> to embed the value inside the options menu's head. 
    ///   The default value is <see langword="true"/>. 
    /// </value>
    [Category ("Menu")]
    [Description ("Determines whether to use the value as the options menu's head.")]
    [DefaultValue (typeof (bool?), "")]
    public bool? HasValueEmbeddedInsideOptionsMenu
    {
      get { return _hasValueEmbeddedInsideOptionsMenu; }
      set { _hasValueEmbeddedInsideOptionsMenu = value; }
    }

    /// <summary> Gets or sets the list of menu items to be hidden. </summary>
    /// <value> The <see cref="WebMenuItem.ItemID"/> values of the menu items to hide. </value>
    [Category ("Menu")]
    [Description ("The list of menu items to be hidden, identified by their ItemIDs.")]
    [DefaultValue ((string) null)]
    [PersistenceMode (PersistenceMode.Attribute)]
    [TypeConverter (typeof (StringArrayConverter))]
    public string[] HiddenMenuItems
    {
      get
      {
        if (_hiddenMenuItems == null)
          return new string[0];
        return _hiddenMenuItems;
      }
      set { _hiddenMenuItems = value; }
    }

    protected ArrayList Validators
    {
      get { return _validators; }
    }

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> of the selected 
    ///   <see cref="IBusinessObjectWithIdentity"/>.
    /// </summary>
    /// <value> A string or <see langword="null"/> if no  <see cref="IBusinessObjectWithIdentity"/> is selected. </value>
    [Browsable (false)]
    public string BusinessObjectUniqueIdentifier
    {
      get
      {
        if (IsNullValue(InternalValue))
          return null;
        return InternalValue;
      }
    }

    /// <summary> Gets or sets the <see cref="IBusinessObjectReferenceProperty"/> object this control is bound to. </summary>
    /// <value> An <see cref="IBusinessObjectReferenceProperty"/> object. </value>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public new IBusinessObjectReferenceProperty Property
    {
      get { return (IBusinessObjectReferenceProperty) base.Property; }
      set { base.Property = value; }
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

    /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
    /// <value> The value must be of type <see cref="IBusinessObjectWithIdentity"/>. </value>
    protected override object ValueImplementation
    {
      get { return Value; }
      set { Value = (IBusinessObjectWithIdentity) value; }
    }

    private bool ShowIcon
    {
      get
      {
        if (!EnableIcon)
          return false;
        if (Property == null)
          return false;
        if (GetIcon (Value, Property.ReferenceClass.BusinessObjectProvider) == null)
          return false;

        return true;
      }
    }

    /// <summary> 
    ///   Gets or sets a flag that determines whether the icon is shown in front of the <see cref="BocReferenceValue.Value"/>.
    /// </summary>
    /// <value> <see langword="true"/> to show the icon. The default value is <see langword="true"/>. </value>
    /// <remarks> 
    ///   An icon is only shown if the <see cref="BocReferenceValueBase.Property"/>'s 
    ///   <see cref="IBusinessObjectClass.BusinessObjectProvider">ReferenceClass.BusinessObjectProvider</see>
    ///   provides an instance of type <see cref="IBusinessObjectWebUIService"/> and 
    ///   <see cref="IBusinessObjectWebUIService.GetIcon"/> returns not <see langword="null"/>.
    /// </remarks>
    [PersistenceMode (PersistenceMode.Attribute), Category ("Appearance"), Description ("Flag that determines whether to show the icon in front of the value."), DefaultValue (true)]
    public bool EnableIcon { get; set; }

    /// <summary> The <see cref="BocReferenceValue"/> supports only scalar properties. </summary>
    /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="false"/>. </returns>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
    protected override bool SupportsPropertyMultiplicity (bool isList)
    {
      return ! isList;
    }

    public override bool SupportsProperty (IBusinessObjectProperty property)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      if (!base.SupportsProperty (property))
        return false;
      return ((IBusinessObjectReferenceProperty) property).ReferenceClass is IBusinessObjectClassWithIdentity;
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      if (!IsDesignMode)
      {
        Page.RegisterRequiresPostBack (this);
        InitializeMenusItems ();
      }
    }

    /// <include file='doc\include\UI\Controls\BocReferenceValueBase.xml' path='BocReferenceValue/InitializeMenusItems/*' />
    protected virtual void InitializeMenusItems ()
    {
    }

    /// <include file='doc\include\UI\Controls\BocReferenceValueBase.xml' path='BocReferenceValue/PreRenderMenuItems/*' />
    protected virtual void PreRenderMenuItems ()
    {
      if (_hiddenMenuItems == null)
        return;

      BocDropDownMenu.HideMenuItems (OptionsMenuItems, _hiddenMenuItems);
    }

    /// <remarks>
    ///   If the <see cref="DropDownList"/> could not be created from <see cref="DropDownListStyle"/>,
    ///   the control is set to read-only.
    /// </remarks>
    protected override void CreateChildControls ()
    {
      _optionsMenu.ID = ID + "_Boc_OptionsMenu";
      Controls.Add (_optionsMenu);
      _optionsMenu.EventCommandClick += OptionsMenu_EventCommandClick;
      _optionsMenu.WxeFunctionCommandClick += OptionsMenu_WxeFunctionCommandClick;
    }

    /// <summary> This event is fired when the value's command is clicked. </summary>
    [Category ("Action")]
    [Description ("Fires when the value's command is clicked.")]
    public event BocCommandClickEventHandler CommandClick
    {
      add { Events.AddHandler (CommandClickEvent, value); }
      remove { Events.RemoveHandler (CommandClickEvent, value); }
    }

    /// <summary> Is raised when a menu item with a command of type <see cref="CommandType.Event"/> is clicked. </summary>
    [Category ("Action")]
    [Description ("Is raised when a menu item with a command of type Event is clicked.")]
    public event WebMenuItemClickEventHandler MenuItemClick
    {
      add { Events.AddHandler (MenuItemClickEvent, value); }
      remove { Events.RemoveHandler (MenuItemClickEvent, value); }
    }

    /// <summary> 
    ///   Handles the <see cref="MenuBase.EventCommandClick"/> event of the <see cref="OptionsMenu"/>.
    /// </summary>
    /// <param name="sender"> The source of the event. </param>
    /// <param name="e"> An <see cref="WebMenuItemClickEventArgs"/> object that contains the event data. </param>
    private void OptionsMenu_EventCommandClick (object sender, WebMenuItemClickEventArgs e)
    {
      OnMenuItemEventCommandClick (e.Item);
    }

    /// <summary> 
    ///   Calls the <see cref="BocMenuItemCommand.OnClick"/> method of the <paramref name="menuItem"/>'s 
    ///   <see cref="BocMenuItem.Command"/> and raises <see cref="MenuItemClick"/> event. 
    /// </summary>
    /// <param name="menuItem"> The <see cref="BocMenuItem"/> that has been clicked. </param>
    /// <remarks> Only called for commands of type <see cref="CommandType.Event"/>. </remarks>
    protected virtual void OnMenuItemEventCommandClick (WebMenuItem menuItem)
    {
      WebMenuItemClickEventHandler menuItemClickHandler = (WebMenuItemClickEventHandler) Events[MenuItemClickEvent];
      if (menuItem != null && menuItem.Command != null)
      {
        if (menuItem is BocMenuItem)
          ((BocMenuItemCommand) menuItem.Command).OnClick ((BocMenuItem) menuItem);
        else
          menuItem.Command.OnClick ();
      }
      if (menuItemClickHandler != null)
      {
        WebMenuItemClickEventArgs e = new WebMenuItemClickEventArgs (menuItem);
        menuItemClickHandler (this, e);
      }
    }

    /// <summary> Handles the <see cref="MenuBase.WxeFunctionCommandClick"/> event of the <see cref="OptionsMenu"/>. </summary>
    /// <param name="sender"> The source of the event. </param>
    /// <param name="e"> An <see cref="WebMenuItemClickEventArgs"/> object that contains the event data. </param>
    /// <remarks> Only called for commands of type <see cref="CommandType.Event"/>. </remarks>
    private void OptionsMenu_WxeFunctionCommandClick (object sender, WebMenuItemClickEventArgs e)
    {
      OnMenuItemWxeFunctionCommandClick (e.Item);
    }

    /// <summary> 
    ///   Calls the <see cref="BocMenuItemCommand.ExecuteWxeFunction"/> method of the <paramref name="menuItem"/>'s 
    ///   <see cref="BocMenuItem.Command"/>.
    /// </summary>
    /// <param name="menuItem"> The <see cref="BocMenuItem"/> that has been clicked. </param>
    /// <remarks> Only called for commands of type <see cref="CommandType.WxeFunction"/>. </remarks>
    protected virtual void OnMenuItemWxeFunctionCommandClick (WebMenuItem menuItem)
    {
      if (menuItem != null && menuItem.Command != null)
      {
        if (menuItem is BocMenuItem)
        {
          int[] indices = new int[0];
          IBusinessObject[] businessObjects;
          if (Value != null)
            businessObjects = new[] { (IBusinessObject)Value };
          else
            businessObjects = new IBusinessObject[0];

          BocMenuItemCommand command = (BocMenuItemCommand) menuItem.Command;
          if (Page is IWxePage)
            command.ExecuteWxeFunction ((IWxePage) Page, indices, businessObjects);
          //else
          //  command.ExecuteWxeFunction (Page, indices, businessObjects);
        }
        else
        {
          Command command = menuItem.Command;
          if (Page is IWxePage)
            command.ExecuteWxeFunction ((IWxePage) Page, null);
          //else
          //  command.ExecuteWxeFunction (Page, null, new NameValueCollection (0));
        }
      }
    }

    /// <summary> Invokes the <see cref="LoadPostData(string,System.Collections.Specialized.NameValueCollection)"/> method. </summary>
    bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      if (RequiresLoadPostData)
        return LoadPostData (postDataKey, postCollection);
      else
        return false;
    }

    /// <summary> Invokes the <see cref="RaisePostDataChangedEvent()"/> method. </summary>
    void IPostBackDataHandler.RaisePostDataChangedEvent ()
    {
      RaisePostDataChangedEvent();
    }

    /// <summary> Called when the state of the control has changed between postbacks. </summary>
    protected abstract void RaisePostDataChangedEvent ();

    /// <summary>
    ///   Uses the <paramref name="postCollection"/> to determine whether the value of this control has been changed
    ///   between postbacks.
    /// </summary>
    /// <include file='doc\include\UI\Controls\BocReferenceValueBase.xml' path='BocReferenceValueBase/LoadPostData/*' />
    protected virtual bool LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      string newValue = PageUtility.GetPostBackCollectionItem (Page, ValueContainingControlID);
      bool isDataChanged = false;
      if (newValue != null)
      {
        if (InternalValue == null && !IsNullValue(newValue))
          isDataChanged = true;
        else if (InternalValue != null && newValue != InternalValue)
          isDataChanged = true;
      }

      if (isDataChanged)
      {
        if (IsNullValue(newValue))
          InternalValue = null;
        else
          InternalValue = newValue;
        
        IsDirty = true;
        OnDataChanged ();
      }
      return isDataChanged;
    }

    protected abstract void OnDataChanged ();

    /// <summary> Fires the <see cref="SelectionChanged"/> event. </summary>
    protected virtual void OnSelectionChanged ()
    {
      EventHandler eventHandler = (EventHandler) Events[SelectionChangedEvent];
      if (eventHandler != null)
        eventHandler (this, EventArgs.Empty);
    }

    /// <summary> Calls the <see cref="RaisePostBackEvent"/> method. </summary>
    void IPostBackEventHandler.RaisePostBackEvent (string eventArgument)
    {
      RaisePostBackEvent (eventArgument);
    }

    /// <summary> Called when the control caused a post back. </summary>
    /// <param name="eventArgument"> an empty <see cref="String"/>. </param>
    protected virtual void RaisePostBackEvent (string eventArgument)
    {
      HandleCommand();
    }

    /// <summary> Handles post back events raised by the value's <see cref="Command"/>. </summary>
    private void HandleCommand ()
    {
      switch (Command.Type)
      {
        case CommandType.Event:
        {
          OnCommandClick (Value);
          break;
        }
        case CommandType.WxeFunction:
        {
          if (Page is IWxePage)
            Command.ExecuteWxeFunction ((IWxePage) Page, Value);
          break;
        }
        default:
        {
          break;
        }
      }
    }

    /// <summary> Fires the <see cref="CommandClick"/> event. </summary>
    /// <param name="businessObject"> 
    ///   The current <see cref="Value"/>, which corresponds to the clicked <see cref="IBusinessObjectWithIdentity"/>,
    ///   unless somebody changed the <see cref="Value"/> in the code behind before the event fired.
    /// </param>
    protected virtual void OnCommandClick (IBusinessObjectWithIdentity businessObject)
    {
      if (Command != null)
      {
        Command.OnClick (businessObject);
        BocCommandClickEventHandler commandClickHandler = (BocCommandClickEventHandler) Events[CommandClickEvent];
        if (commandClickHandler != null)
        {
          BocCommandClickEventArgs e = new BocCommandClickEventArgs (Command, businessObject);
          commandClickHandler (this, e);
        }
      }
    }

    IBusinessObject[] IBocMenuItemContainer.GetSelectedBusinessObjects ()
    {
      return (Value == null) ? new IBusinessObject[0] : new IBusinessObject[] { Value };
    }

    void IBocMenuItemContainer.RemoveBusinessObjects (IBusinessObject[] businessObjects)
    {
      RemoveBusinessObjects (businessObjects);
    }

    void IBocMenuItemContainer.InsertBusinessObjects (IBusinessObject[] businessObjects)
    {
      InsertBusinessObjects (businessObjects);
    }

    /// <summary> Removes the <paramref name="businessObjects"/> from the <see cref="Value"/> collection. </summary>
    /// <remarks> Sets the dirty state. </remarks>
    protected virtual void RemoveBusinessObjects (IBusinessObject[] businessObjects)
    {
      if (Value == null)
        return;

      if (businessObjects.Length > 0 && businessObjects[0] is IBusinessObjectWithIdentity)
      {
        if (((IBusinessObjectWithIdentity) businessObjects[0]).UniqueIdentifier == Value.UniqueIdentifier)
        {
          Value = null;
          IsDirty = true;
        }
      }
    }

    /// <summary> Adds the <paramref name="businessObjects"/> to the <see cref="Value"/> collection. </summary>
    /// <remarks> Sets the dirty state. </remarks>
    protected virtual void InsertBusinessObjects (IBusinessObject[] businessObjects)
    {
      if (businessObjects.Length > 0)
      {
        Value = (IBusinessObjectWithIdentity) businessObjects[0];
        IsDirty = true;
      }
    }

    /// <summary> This event is fired when the selection is changed between postbacks. </summary>
    [Category ("Action")]
    [Description ("Fires when the value of the control has changed.")]
    public event EventHandler SelectionChanged
    {
      add { Events.AddHandler(SelectionChangedEvent, value); }
      remove { Events.RemoveHandler(SelectionChangedEvent, value); }
    }

    /// <summary> Loads the resources into the control's properties. </summary>
    protected override void LoadResources (IResourceManager resourceManager)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);

      if (IsDesignMode)
        return;
      base.LoadResources (resourceManager);

      string key = ResourceManagerUtility.GetGlobalResourceKey (ErrorMessage);
      if (!string.IsNullOrEmpty (key))
        ErrorMessage = resourceManager.GetString (key);
    }

    protected override void OnPreRender (EventArgs e)
    {
      EnsureChildControls();
      base.OnPreRender (e);

      LoadResources (GetResourceManager());

      if (!IsDesignMode)
        PreRenderMenuItems();

      if (HasOptionsMenu)
        PreRenderOptionsMenu();
      else
        OptionsMenu.Visible = false;

      if (Command != null)
        Command.RegisterForSynchronousPostBack (this, null, string.Format ("BocReferenceValue '{0}', Object Command", ID));
    }

    /// <summary> Returns the <see cref="IResourceManager"/> used to access the resources for this control. </summary>
    protected abstract IResourceManager GetResourceManager ();

    protected virtual void PreRenderOptionsMenu ()
    {
      OptionsMenu.Enabled = Enabled;
      OptionsMenu.IsReadOnly = IsReadOnly;
      if (string.IsNullOrEmpty (OptionsTitle))
        OptionsMenu.TitleText = GetOptionsMenuTitle();
      else
        OptionsMenu.TitleText = OptionsTitle;
      OptionsMenu.Style["vertical-align"] = "middle";

      if (!IsDesignMode)
      {
        string getSelectionCount;
        if (IsReadOnly)
        {
          if (InternalValue != null)
            getSelectionCount = "function() { return 1; }";
          else
            getSelectionCount = "function() { return 0; }";
        }
        else
          getSelectionCount = GetSelectionCountFunction();
        OptionsMenu.GetSelectionCount = getSelectionCount;
      }
    }

    protected abstract string GetSelectionCountFunction ();

    protected abstract string GetOptionsMenuTitle ();

    /// <summary>
    ///   Returns the string to be used in the drop down list for the specified <see cref="IBusinessObjectWithIdentity"/>.
    /// </summary>
    /// <param name="businessObject"> The <see cref="IBusinessObjectWithIdentity"/> to get the display name for. </param>
    /// <returns> The display name for the specified <see cref="IBusinessObjectWithIdentity"/>. </returns>
    /// <remarks> 
    ///   <para>
    ///     Override this method to change the way the display name is composed. 
    ///   </para><para>
    ///     The default implementation used the <see cref="IBusinessObject.DisplayName"/> property to get the display name.
    ///   </para>
    /// </remarks>
    protected virtual string GetDisplayName (IBusinessObjectWithIdentity businessObject)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);
      return businessObject.DisplayNameSafe;
    }

    protected bool IsCommandEnabled (bool isReadOnly)
    {
      if (WcagHelper.Instance.IsWaiConformanceLevelARequired())
        return false;

      bool isCommandEnabled = false;
      if (Command != null)
      {
        bool isActive = Command.Show == CommandShow.Always
                        || isReadOnly && Command.Show == CommandShow.ReadOnly
                        || ! isReadOnly && Command.Show == CommandShow.EditMode;
        bool isCommandLinkPossible = (IsReadOnly || ShowIcon) && InternalValue != null;
        if (isActive
            && Command.Type != CommandType.None
            && isCommandLinkPossible)
          isCommandEnabled = Enabled;
      }
      return isCommandEnabled;
    }

    private bool IsNullValue (string newValue)
    {
      return newValue == c_nullIdentifier;
    }
  }
}
