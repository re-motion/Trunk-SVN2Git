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
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Design;
using Remotion.Security;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public interface IBocSortableColumnDefinition: IControlItem
  {
    bool IsSortable { get; }
  }

  /// <summary> A BocColumnDefinition defines how to display a column of a list. </summary>
  [Editor (typeof (ExpandableObjectConverter), typeof (UITypeEditor))]
  public abstract class BocColumnDefinition : BusinessObjectControlItem, IControlItem
  {
    private string _itemID = string.Empty;
    private string _columnTitle = string.Empty;
    private Unit _width = Unit.Empty;
    private string _cssClass = string.Empty;

    public BocColumnDefinition ()
    {
    }

    public override string ToString ()
    {
      string displayName = ItemID;
      if (StringUtility.IsNullOrEmpty (displayName))
        displayName = ColumnTitle;
      if (StringUtility.IsNullOrEmpty (displayName))
        return DisplayedTypeName;
      else
        return string.Format ("{0}: {1}", displayName, DisplayedTypeName);
    }

    /// <summary> Gets the programmatic name of the <see cref="BocColumnDefinition"/>. </summary>
    /// <value> A <see cref="string"/> providing an identifier for the <see cref="BocColumnDefinition"/>. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Misc")]
    [Description ("The programmatic name of the column definition.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    [ParenthesizePropertyName (true)]
    [Browsable (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Visible)]
    public override string ItemID
    {
      get { return _itemID; }
      set { _itemID = StringUtility.NullToEmpty (value); }
    }

    /// <summary> Gets the displayed value of the column title. </summary>
    /// <remarks> Override this property to change the way the column title text is generated. </remarks>
    /// <value> A <see cref="string"/> representing this column's title row contents. </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public virtual string ColumnTitleDisplayValue
    {
      get { return ColumnTitle; }
    }

    /// <summary> Gets or sets the text displayed in the column title. </summary>
    /// <remarks>
    ///   Override this property to add validity checks to the set accessor.
    ///   The get accessor should return the value verbatim. The value will be HTML encoded.
    /// </remarks>
    /// <value> A <see cref="string"/> representing the manually set title of this column. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("The manually assigned value of the column title, can be empty. The value will be HTML encoded.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public virtual string ColumnTitle
    {
      get { return _columnTitle; }
      set { _columnTitle = StringUtility.NullToEmpty (value); }
    }

    /// <summary> Gets or sets the width of the column definition. </summary>
    /// <value> A <see cref="Unit"/> providing the width of this column when it is rendered. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Layout")]
    [Description ("The width of the rendered column.")]
    [DefaultValue (typeof (Unit), "")]
    [NotifyParentProperty (true)]
    public Unit Width
    {
      get { return _width; }
      set { _width = value; }
    }

    /// <summary> Gets or sets the CSS-class of the column definition. </summary>
    /// <value> A <see cref="string"/> providing the CSS-class added to the class attribute when this column is rendered. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Layout")]
    [Description ("The CSS-class of the rendered column's cells.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public string CssClass
    {
      get { return _cssClass; }
      set { _cssClass = StringUtility.NullToEmpty (value); }
    }


    /// <summary> Gets the human readable name of this type. </summary>
    protected virtual string DisplayedTypeName
    {
      get { return "ColumnDefinition"; }
    }

    public override void LoadResources (IResourceManager resourceManager)
    {
      if (resourceManager == null)
        return;
      base.LoadResources (resourceManager);

      string key;
      key = ResourceManagerUtility.GetGlobalResourceKey (ColumnTitle);
      if (!StringUtility.IsNullOrEmpty (key))
        ColumnTitle = resourceManager.GetString (key);
    }
  }

  /// <summary> A column definition with the possibility of rendering a command in the cell. </summary>
  public abstract class BocCommandEnabledColumnDefinition : BocColumnDefinition
  {
    /// <summary> The <see cref="BocListItemCommand"/> rendered in this column. </summary>
    private SingleControlItemCollection _command;

    public BocCommandEnabledColumnDefinition ()
    {
      _command = new SingleControlItemCollection (new BocListItemCommand (), new Type[] { typeof (BocListItemCommand) });
    }

    protected override void OnOwnerControlChanged ()
    {
      base.OnOwnerControlChanged ();
      if (Command != null)
        Command.OwnerControl = OwnerControl;
    }

    /// <summary> Gets or sets the <see cref="BocListItemCommand"/> rendered in this column. </summary>
    /// <value> A <see cref="BocListItemCommand"/>. </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Category ("Behavior")]
    [Description ("The command rendered in this column.")]
    [NotifyParentProperty (true)]
    public BocListItemCommand Command
    {
      get { return (BocListItemCommand) _command.ControlItem; }
      set
      {
        _command.ControlItem = value;
        if (OwnerControl != null)
          _command.ControlItem.OwnerControl = (Control) OwnerControl;
      }
    }

    private bool ShouldSerializeCommand ()
    {
      if (Command == null)
        return false;

      if (Command.Type == CommandType.None)
        return false;
      else
        return true;
    }

    /// <summary> Sets the <see cref="Command"/> to its default value. </summary>
    /// <remarks> 
    ///   The default value is a <see cref="BocListItemCommand"/> object with a <c>Command.Type</c> set to 
    ///   <see cref="CommandType.None"/>.
    /// </remarks>
    private void ResetCommand ()
    {
      if (Command != null)
      {
        Command = (BocListItemCommand) Activator.CreateInstance (Command.GetType ());
        Command.Type = CommandType.None;
      }
    }

    [PersistenceMode (PersistenceMode.InnerProperty)]
    [Browsable (false)]
    public SingleControlItemCollection PersistedCommand
    {
      get { return _command; }
    }

    /// <summary> Controls the persisting of the <see cref="Command"/>. </summary>
    /// <remarks> 
    ///   Does not persist <see cref="BocListItemCommand"/> objects with a <c>Command.Type</c> set to 
    ///   <see cref="CommandType.None"/>.
    /// </remarks>
    private bool ShouldSerializePersistedCommand ()
    {
      return ShouldSerializeCommand ();
    }
  }

  /// <summary> A column definition containing no data, only the <see cref="BocListItemCommand"/>. </summary>
  public class BocCommandColumnDefinition : BocCommandEnabledColumnDefinition
  {
    private string _text = string.Empty;
    private IconInfo _icon = new IconInfo ();

    public BocCommandColumnDefinition ()
    {
    }

    /// <summary> Returns a <see cref="string"/> that represents this <see cref="BocColumnDefinition"/>. </summary>
    /// <returns> Returns <see cref="Text"/>, followed by the the class name of the instance.  </returns>
    public override string ToString ()
    {
      string displayName = ItemID;
      if (StringUtility.IsNullOrEmpty (displayName))
        displayName = ColumnTitle;
      if (StringUtility.IsNullOrEmpty (displayName))
        displayName = Text;
      if (StringUtility.IsNullOrEmpty (displayName))
        return DisplayedTypeName;
      else
        return string.Format ("{0}: {1}", displayName, DisplayedTypeName);
    }

    /// <summary> Gets or sets the text representing the command in the rendered page. </summary>
    /// <value> A <see cref="string"/> representing the command. </value>
    /// <remarks> The value will not be HTML encoded. </remarks>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("The text representing the command in the rendered page. The value will not be HTML encoded.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public string Text
    {
      get { return _text; }
      set { _text = StringUtility.NullToEmpty (value); }
    }

    /// <summary> 
    ///   Gets or sets the image representing the  command in the rendered page. Must not be <see langword="null"/>. 
    /// </summary>
    /// <value> An <see cref="IconInfo"/> representing the command. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [Category ("Appearance")]
    [Description ("The image representing the command in the rendered page.")]
    [NotifyParentProperty (true)]
    public IconInfo Icon
    {
      get
      {
        return _icon;
      }
      set
      {
        ArgumentUtility.CheckNotNull ("Icon", value);
        _icon = value;
      }
    }

    private bool ShouldSerializeIcon ()
    {
      return IconInfo.ShouldSerialize (_icon);
    }

    private void ResetIcon ()
    {
      _icon.Reset ();
    }

    /// <summary> Gets the human readable name of this type. </summary>
    protected override string DisplayedTypeName
    {
      get { return "CommandColumnDefinition"; }
    }

    public override void LoadResources (IResourceManager resourceManager)
    {
      if (resourceManager == null)
        return;
      base.LoadResources (resourceManager);

      string key;
      key = ResourceManagerUtility.GetGlobalResourceKey (Text);
      if (!StringUtility.IsNullOrEmpty (key))
        Text = resourceManager.GetString (key);

      Icon.LoadResources (resourceManager);
    }
  }

  /// <summary> A column definition for displaying data and an optional command. </summary>
  public abstract class BocValueColumnDefinition : BocCommandEnabledColumnDefinition, IBocSortableColumnDefinition
  {
    private bool _enforceWidth;
    private bool _isSortable = true;

    public BocValueColumnDefinition ()
    {
    }

    /// <summary> Creates a string representation of the data displayed in this column. </summary>
    /// <param name="obj"> The <see cref="IBusinessObject"/> to be displayed in this column. </param>
    /// <returns> A <see cref="string"/> representing the contents of <paramref name="obj"/>. </returns>
    public abstract string GetStringValue (IBusinessObject obj);

    /// <summary> 
    ///   Gets or sets a flag that determines whether to hide overflowing contents in the data rows instead of 
    ///   breaking into a new line. 
    /// </summary>
    /// <value> <see langword="true"/> to enforce the width. </value>
    /// <remarks> 
    ///     <see cref="BocColumnDefinition.Width"/> must not be of type <see cref="UnitType.Percentage"/>, 
    ///     if the width is to be enforced.
    /// </remarks>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Layout")]
    [Description ("Hides overflowing contents in the data rows instead of breaking into a new line.")]
    [DefaultValue (false)]
    [NotifyParentProperty (true)]
    public bool EnforceWidth
    {
      get { return _enforceWidth; }
      set { _enforceWidth = value; }
    }

    /// <summary> Gets or sets a flag that determines whether to enable sorting for this columns. </summary>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("A flag determining whether to enable sorting for this columns.")]
    [DefaultValue (true)]
    [NotifyParentProperty (true)]
    public bool IsSortable
    {
      get { return _isSortable; }
      set { _isSortable = value; }
    }

    /// <summary> Gets the human readable name of this type. </summary>
    protected override string DisplayedTypeName
    {
      get { return "ValueColumnDefinition"; }
    }
  }

  /// <summary> A column definition for displaying a single property path. </summary>
  /// <remarks>
  ///   Note that using the methods of <see cref="BusinessObjectPropertyPath"/>, 
  ///   the original value of this property can be retreived or changed.
  /// </remarks>
  public class BocSimpleColumnDefinition : BocValueColumnDefinition, IBusinessObjectClassSource
  {
    private const string c_notAccessible = "×";

    private string _formatString = string.Empty;
    private PropertyPathBinding _propertyPathBinding;
    private string _editModeControlType = string.Empty;
    private bool _isReadOnly;
    private bool _enableIcon = false;

    public BocSimpleColumnDefinition ()
    {
      _formatString = string.Empty;
      _propertyPathBinding = new PropertyPathBinding ();
    }

    /// <summary> Passes the new OwnerControl to the <see cref="PropertyPathBindingCollection"/>. </summary>
    protected override void OnOwnerControlChanged ()
    {
      _propertyPathBinding.OwnerControl = OwnerControl;
      base.OnOwnerControlChanged ();
    }

    /// <summary> Creates a string representation of the data displayed in this column. </summary>
    /// <param name="obj"> The <see cref="IBusinessObject"/> to be displayed in this column. </param>
    /// <returns> A <see cref="string"/> representing the contents of <paramref name="obj"/>. </returns>
    public override string GetStringValue (IBusinessObject obj)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);

      IBusinessObjectPropertyPath propertyPath = null;
      if (!_propertyPathBinding.IsDynamic)
        propertyPath = _propertyPathBinding.GetPropertyPath ();
      else
        propertyPath = _propertyPathBinding.GetDynamicPropertyPath (obj.BusinessObjectClass);

      if (propertyPath == null)
        return string.Empty;

      string formatString = _formatString;
      if (StringUtility.IsNullOrEmpty (formatString))
      {
        if (propertyPath.LastProperty is IBusinessObjectDateTimeProperty)
        {
          if (((IBusinessObjectDateTimeProperty) propertyPath.LastProperty).Type == DateTimeType.Date)
            formatString = "d";
          else
            formatString = "g";
        }
      }

      try
      {
        return propertyPath.GetString (obj, StringUtility.EmptyToNull (formatString));
      }
      //TODO: Move to BusinessObjectPropertyPath.GetString
      catch (PermissionDeniedException)
      {
        return c_notAccessible;
      }
    }

    /// <summary>
    ///   Gets or sets the format string describing how the value accessed through the 
    ///   <see cref="BusinessObjectPropertyPath"/> object is formatted.
    /// </summary>
    /// <value> 
    ///   A <see cref="string"/> representing a valid format string. 
    /// </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Format")]
    [Description ("A format string describing how the value accessed through the Property Path is formatted.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public string FormatString
    {
      get { return _formatString; }
      set { _formatString = StringUtility.NullToEmpty (value); }
    }

    public IBusinessObjectPropertyPath GetPropertyPath ()
    {
      return _propertyPathBinding.GetPropertyPath ();
    }

    public IBusinessObjectPropertyPath GetDynamicPropertyPath (IBusinessObjectClass businessObjectClass)
    {
      return _propertyPathBinding.GetDynamicPropertyPath (businessObjectClass);
    }

    public void SetPropertyPath (IBusinessObjectPropertyPath propertyPath)
    {
      _propertyPathBinding.SetPropertyPath (propertyPath);
    }

    [DefaultValue (false)]
    [Category ("Data")]
    public bool IsDynamic
    {
      get { return _propertyPathBinding.IsDynamic; }
      set { _propertyPathBinding.IsDynamic = value; }
    }

    /// <summary>
    ///   Gets or sets the string representation of the <see cref="GetPropertyPath"/>. 
    ///   Must not be <see langword="null"/> or emtpy.
    /// </summary>
    /// <value> A <see cref="string"/> representing the <see cref="GetPropertyPath"/>. </value>
    [Editor (typeof (PropertyPathPickerEditor), typeof (UITypeEditor))]
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Data")]
    [Description ("The string representation of the Property Path. Must not be emtpy.")]
    //  No default value
    [NotifyParentProperty (true)]
    public string PropertyPathIdentifier
    {
      get { return _propertyPathBinding.PropertyPathIdentifier; }
      set { _propertyPathBinding.PropertyPathIdentifier = value; }
    }

    /// <summary> 
    ///   Returns an instance the class specified by the <see cref="EditModeControlType"/> property, which will then be used for editing this 
    ///   column during edit mode.
    /// </summary>
    public IBusinessObjectBoundEditableWebControl CreateEditModeControl ()
    {
      if (StringUtility.IsNullOrEmpty (_editModeControlType))
        return null;

      Type type = WebTypeUtility.GetType (_editModeControlType, true, false);
      return (IBusinessObjectBoundEditableWebControl) Activator.CreateInstance (type);
    }

    /// <summary>
    ///   Gets or sets the type of the <see cref="IBusinessObjectBoundEditableWebControl"/> to be instantiated 
    ///   for editing the value of this column during edit mode.
    /// </summary>
    /// <remarks>
    ///    Optionally uses the abbreviated type name as defined in <see cref="TypeUtility.ParseAbbreviatedTypeName"/>. 
    /// </remarks>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("The IBusinessObjectBoundEditableWebControl to be used for editing the value of this column during edit mode.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public string EditModeControlType
    {
      get { return _editModeControlType; }
      set { _editModeControlType = StringUtility.NullToEmpty (value); }
    }

    /// <summary>
    ///   Gets or sets a flag that determines whether the displayed value can be edited if the row is in edit mode.
    /// </summary>
    /// <remarks> It is only possible to explicitly disable the editing of the value. </remarks>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("A flag that determines whether the displayed value can be edited if the row is in edit mode.")]
    [DefaultValue (false)]
    [NotifyParentProperty (true)]
    public bool IsReadOnly
    {
      get { return _isReadOnly; }
      set { _isReadOnly = value; }
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("Flag that determines whether to show the icon in front of the value. Only allowed for reference properties")]
    [DefaultValue (true)]
    public bool EnableIcon
    {
      get { return _enableIcon; }
      set { _enableIcon = value; }
    }

    /// <summary> Gets the displayed value of the column title. </summary>
    /// <remarks> 
    ///   If <see cref="BocColumnDefinition.ColumnTitle"/> is empty or <see langowrd="null"/>, 
    ///   the <c>DisplayName</c> of the <see cref="IBusinessObjectProperty"/> is returned.
    /// </remarks>
    /// <value> A <see cref="string"/> representing this column's title row contents. </value>
    public override string ColumnTitleDisplayValue
    {
      get
      {
        bool isTitleEmpty = StringUtility.IsNullOrEmpty (ColumnTitle);

        if (!isTitleEmpty)
          return ColumnTitle;

        IBusinessObjectPropertyPath propertyPath = null;
        if (!_propertyPathBinding.IsDynamic)
        {
          try
          {
            propertyPath = _propertyPathBinding.GetPropertyPath ();
          }
            // TODO: Why is this catch block required?
          catch (ArgumentException)
          {
          }
        }

        if (propertyPath != null)
          return propertyPath.LastProperty.DisplayName;

        return string.Empty;
      }
    }

    /// <summary> The human readable name of this type. </summary>
    protected override string DisplayedTypeName
    {
      get { return "SimpleColumnDefinition"; }
    }

    IBusinessObjectClass IBusinessObjectClassSource.BusinessObjectClass
    {
      get { return _propertyPathBinding.BusinessObjectClass; }
    }
  }

  /// <summary> A column definition for displaying a string made up from different property paths. </summary>
  /// <remarks> Note that values in these columnDefinitions can usually not be modified directly. </remarks>
  public class BocCompoundColumnDefinition : BocValueColumnDefinition
  {
    private const string c_notAccessible = "×";
    
    /// <summary>
    ///   A format string describing how the values accessed through the 
    ///   <see cref="BusinessObjectPropertyPath"/> objects are merged by <see cref="GetStringValue"/>.
    /// </summary>
    private string _formatString = string.Empty;

    /// <summary>
    ///   The collection of <see cref="PropertyPathBinding"/> objects used by
    ///   <see cref="GetStringValue"/> to access the values of an <see cref="IBusinessObject"/>.
    /// </summary>
    private PropertyPathBindingCollection _propertyPathBindings;

    public BocCompoundColumnDefinition ()
    {
      _propertyPathBindings = new PropertyPathBindingCollection (null);
    }

    /// <summary> Creates a string representation of the data displayed in this column. </summary>
    /// <param name="obj"> The <see cref="IBusinessObject"/> to be displayed in this column. </param>
    /// <returns> A <see cref="string"/> representing the contents of <paramref name="obj"/>. </returns>
    public override string GetStringValue (IBusinessObject obj)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);

      BusinessObjectPropertyPath.Formatter[] formatters = new BusinessObjectPropertyPath.Formatter[_propertyPathBindings.Count];
      for (int i = 0; i < _propertyPathBindings.Count; ++i)
      {
        if (_propertyPathBindings[i].IsDynamic)
          formatters[i] = new BusinessObjectPropertyPath.Formatter (obj, _propertyPathBindings[i].GetDynamicPropertyPath (obj.BusinessObjectClass));
        else
          formatters[i] = new BusinessObjectPropertyPath.Formatter (obj, _propertyPathBindings[i].GetPropertyPath ());
      }      

      try
      {
        return string.Format (_formatString, formatters);
      }
      //TODO: Move to BusinessObjectPropertyPath.GetString
      catch (PermissionDeniedException)
      {
        return c_notAccessible;
      }
    }

    /// <summary> Passes the new OwnerControl to the <see cref="PropertyPathBindingCollection"/>. </summary>
    protected override void OnOwnerControlChanged ()
    {
      _propertyPathBindings.OwnerControl = OwnerControl;
      base.OnOwnerControlChanged ();
    }

    /// <summary>
    ///   Gets or sets the format string describing how the values accessed through the 
    ///   <see cref="BusinessObjectPropertyPath"/> objects are merged by <see cref="GetStringValue"/>.
    /// </summary>
    /// <value> 
    ///   A <see cref="string"/> containing a format item for each 
    ///   <see cref="BusinessObjectPropertyPath"/> to be displayed. The indices must match the 
    ///   order of the <see cref="BusinessObjectPropertyPath"/> objects to be formatted.
    /// </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Format")]
    [Description ("A format string describing how the values accessed through the Property Path are merged.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public string FormatString
    {
      get { return _formatString; }
      set { _formatString = StringUtility.NullToEmpty (value); }
    }

    /// <summary>
    ///   Gets the collection of <see cref="PropertyPathBinding"/> objects used by
    ///   <see cref="GetStringValue"/> to access the values of an <see cref="IBusinessObject"/>.
    /// </summary>
    /// <value> A collection of <see cref="PropertyPathBinding"/> objects. </value>
    [PersistenceMode (PersistenceMode.InnerProperty)]
    [Category ("Data")]
    [Description ("The Property Paths used to access the values of Business Object.")]
    [NotifyParentProperty (true)]
    public PropertyPathBindingCollection PropertyPathBindings
    {
      get { return _propertyPathBindings; }
    }

    /// <summary> Gets or sets the text displayed in the column title. Must not be empty or <see langword="null"/>. </summary>
    /// <value> A <see cref="string"/> representing the title of this column. </value>
    [Description ("The assigned value of the column title, must not be empty.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public override string ColumnTitle
    {
      get { return base.ColumnTitle; }
      set
      {
        ArgumentUtility.CheckNotNullOrEmpty ("ColumnTitle", value);
        base.ColumnTitle = value;
      }
    }

    /// <summary> Gets the human readable name of this type. </summary>
    protected override string DisplayedTypeName
    {
      get { return "CompoundColumnDefinition"; }
    }
  }

  /// <summary> A column definition used for switching between edit mode and returning from it via save and cancel. </summary>
  public class BocRowEditModeColumnDefinition : BocColumnDefinition
  {
    private string _editText = string.Empty;
    private IconInfo _editIcon = new IconInfo ();
    private string _saveText = string.Empty;
    private IconInfo _saveIcon = new IconInfo ();
    private string _cancelText = string.Empty;
    private IconInfo _cancelIcon = new IconInfo ();
    private BocRowEditColumnDefinitionShow _show = BocRowEditColumnDefinitionShow.EditMode;

    public BocRowEditModeColumnDefinition ()
    {
    }

    /// <summary>
    ///   Determines when the column is shown to the user in regard of the <see cref="BocList"/>'s read-only setting.
    /// </summary>
    /// <value> 
    ///   One of the <see cref="BocRowEditColumnDefinitionShow"/> enumeration values. 
    ///   The default is <see cref="BocRowEditColumnDefinitionShow.EditMode"/>.
    /// </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("Determines when to show the column to the user in regard to the BocList's read-only setting.")]
    [DefaultValue (BocRowEditColumnDefinitionShow.EditMode)]
    [NotifyParentProperty (true)]
    public BocRowEditColumnDefinitionShow Show
    {
      get { return _show; }
      set { _show = value; }
    }

    /// <summary> Gets or sets the text representing the edit command in the rendered page. </summary>
    /// <value> A <see cref="string"/> representing the edit command. </value>
    /// <remarks> The value will not be HTML encoded. </remarks>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("The text representing the edit command in the rendered page. The value will not be HTML encoded.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public string EditText
    {
      get { return _editText; }
      set { _editText = StringUtility.NullToEmpty (value); }
    }

    /// <summary>
    ///  Gets or sets the image representing the edit command in the rendered page. Must not be <see langword="null"/>. 
    /// </summary>
    /// <value> An <see cref="IconInfo"/> representing the edit command. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [Category ("Appearance")]
    [Description ("The image representing the edit command in the rendered page.")]
    [NotifyParentProperty (true)]
    public IconInfo EditIcon
    {
      get
      {
        return _editIcon;
      }
      set
      {
        ArgumentUtility.CheckNotNull ("EditIcon", value);
        _editIcon = value;
      }
    }

    private bool ShouldSerializeEditIcon ()
    {
      return IconInfo.ShouldSerialize (_editIcon);
    }

    private void ResetEditIcon ()
    {
      _editIcon.Reset ();
    }


    /// <summary> Gets or sets the text representing the save command in the rendered page. </summary>
    /// <value> A <see cref="string"/> representing the save command. </value>
    /// <remarks> The value will not be HTML encoded. </remarks>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("The text representing the save command in the rendered page. The value will not be HTML encoded.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public string SaveText
    {
      get { return _saveText; }
      set { _saveText = StringUtility.NullToEmpty (value); }
    }

    /// <summary> 
    ///   Gets or sets the image representing the save command in the rendered page. Must not be <see langword="null"/>.
    /// </summary>
    /// <value> An <see cref="IconInfo"/> representing the save command. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [Category ("Appearance")]
    [Description ("The relative url to image representing the save command in the rendered page.")]
    [NotifyParentProperty (true)]
    public IconInfo SaveIcon
    {
      get
      {
        return _saveIcon;
      }
      set
      {
        ArgumentUtility.CheckNotNull ("SaveIcon", value);
        _saveIcon = value;
      }
    }

    private bool ShouldSerializeSaveIcon ()
    {
      return IconInfo.ShouldSerialize (_saveIcon);
    }

    private void ResetSaveIcon ()
    {
      _saveIcon.Reset ();
    }

    /// <summary> Gets or sets the text representing the cancel command in the rendered page. </summary>
    /// <value> A <see cref="string"/> representing the cancel command. </value>
    /// <remarks> The value will not be HTML encoded. </remarks>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("The text representing the cancel command in the rendered page. The value will not be HTML encoded.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public string CancelText
    {
      get { return _cancelText; }
      set { _cancelText = StringUtility.NullToEmpty (value); }
    }

    /// <summary> 
    ///   Gets or sets the image representing the cancel command in the rendered page. Must not be <see langword="null"/>. 
    /// </summary>
    /// <value> An <see cref="IconInfo"/> representing the cancel command. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [Category ("Appearance")]
    [Description ("The image representing the cancel command in the rendered page.")]
    [NotifyParentProperty (true)]
    public IconInfo CancelIcon
    {
      get
      {
        return _cancelIcon;
      }
      set
      {
        ArgumentUtility.CheckNotNull ("CancelIcon", value);
        _cancelIcon = value;
      }
    }

    private bool ShouldSerializeCancelIcon ()
    {
      return IconInfo.ShouldSerialize (_cancelIcon);
    }

    private void ResetCancelIcon ()
    {
      _cancelIcon.Reset ();
    }

    /// <summary> Gets the human readable name of this type. </summary>
    protected override string DisplayedTypeName
    {
      get { return "RowEditModeColumnDefinition"; }
    }
    public override void LoadResources (IResourceManager resourceManager)
    {
      if (resourceManager == null)
        return;
      base.LoadResources (resourceManager);

      string key;
      key = ResourceManagerUtility.GetGlobalResourceKey (EditText);
      if (!StringUtility.IsNullOrEmpty (key))
        EditText = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (SaveText);
      if (!StringUtility.IsNullOrEmpty (key))
        SaveText = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (CancelText);
      if (!StringUtility.IsNullOrEmpty (key))
        CancelText = resourceManager.GetString (key);

      if (EditIcon != null)
        EditIcon.LoadResources (resourceManager);
      if (SaveIcon != null)
        SaveIcon.LoadResources (resourceManager);
      if (CancelIcon != null)
        CancelIcon.LoadResources (resourceManager);
    }
  }

  /// <summary> Defines when the <see cref="BocRowEditModeColumnDefinition"/> will be shown in the <see cref="BocList"/>. </summary>
  public enum BocRowEditColumnDefinitionShow
  {
    /// <summary> The column is always shown, but inactive if the <see cref="BocList"/> is read-only. </summary>
    Always,
    /// <summary> The column is only shown if the <see cref="BocList"/> is in edit-mode. </summary>
    EditMode
  }

  /// <summary> A column definition that renders a <see cref="DropDownMenu"/> in the cell. </summary>
  public class BocDropDownMenuColumnDefinition : BocColumnDefinition
  {
    private string _menuTitleText = string.Empty;
    private IconInfo _menuTitleIcon;

    public BocDropDownMenuColumnDefinition ()
    {
      _menuTitleIcon = new IconInfo ();
    }

    /// <summary> Gets the human readable name of this type. </summary>
    protected override string DisplayedTypeName
    {
      get { return "DropDownMenuColumnDefinition"; }
    }

    /// <summary> Gets or sets the text displayed as the menu title. </summary>
    /// <value> A <see cref="string"/> displayed as the menu's title. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("The menu title, can be empty.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public string MenuTitleText
    {
      get { return _menuTitleText; }
      set { _menuTitleText = StringUtility.NullToEmpty (value); }
    }

    /// <summary> Gets or sets the icon displayed in the menu's title field. </summary>
    /// <value> An <see cref="IconInfo"/> displayed in the menu's title field. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [Category ("Appearance")]
    [Description ("An icon displayed in the menu's title field, can be empty.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public IconInfo MenuTitleIcon
    {
      get { return _menuTitleIcon; }
      set { _menuTitleIcon = value; }
    }


    public override void LoadResources (IResourceManager resourceManager)
    {
      if (resourceManager == null)
        return;
      base.LoadResources (resourceManager);

      string key;
      key = ResourceManagerUtility.GetGlobalResourceKey (MenuTitleText);
      if (!StringUtility.IsNullOrEmpty (key))
        MenuTitleText = resourceManager.GetString (key);

      if (MenuTitleIcon != null)
        MenuTitleIcon.LoadResources (resourceManager);
    }
  }

  /// <summary> A column definition that acts as a placeholder for inserting a column for each property. </summary>
  public class BocAllPropertiesPlaceholderColumnDefinition : BocColumnDefinition
  {
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public override string ItemID
    {
      get { return base.ItemID; }
      set { base.ItemID = value; }
    }

    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public override string ColumnTitle
    {
      get { return base.ColumnTitle; }
      set { base.ColumnTitle = value; }
    }

    /// <summary> Gets or sets the combined width of the generated column definitions. </summary>
    /// <value> A <see cref="Unit"/> providing the combined width of the generated columns when they are rendered. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Layout")]
    [Description ("The width used for all generated property columns combined.")]
    [DefaultValue (typeof (Unit), "")]
    [NotifyParentProperty (true)]
    public new Unit Width
    {
      get { return base.Width; }
      set { base.Width = value; }
    }

    /// <summary> Gets or sets the CSS-class of the generated column definitions. </summary>
    /// <value> A <see cref="string"/> providing the CSS-class added to the class attribute when the columns are rendered. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Layout")]
    [Description ("The CSS-class of the generated columns' cells.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public new string CssClass
    {
      get { return base.CssClass; }
      set { base.CssClass = value; }
    }
    /// <summary> Gets the human readable name of this type. </summary>
    protected override string DisplayedTypeName
    {
      get { return "AllPropertiesPlaceholderColumnDefinition"; }
    }
  }

}
