using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Design;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{

/// <summary> A BocListView is a named collection of column definitions. </summary>
[ParseChildren (true, "ColumnDefinitionCollection")]
public class BocListView: BusinessObjectControlItem
{
  private string _itemID;
  private object _title;
  /// <summary> 
  ///   The <see cref="BocColumnDefinition"/> objects stored in the <see cref="BocListView"/>. 
  /// </summary>
  private BocColumnDefinitionCollection _columnDefinitions;

  /// <summary> Initialize a new instance. </summary>
  public BocListView (
      IBusinessObjectBoundWebControl ownerControl, 
      object title, 
      BocColumnDefinition[] columnDefinitions)
  {
    _title = title;
    _columnDefinitions = new BocColumnDefinitionCollection (
      ownerControl);
    
    if (columnDefinitions != null)
      _columnDefinitions.AddRange (columnDefinitions);
  }

  /// <summary> Initialize a new instance. </summary>
  public BocListView (object title, BocColumnDefinition[] columnDefinitions)
    : this (null, title, columnDefinitions)
  {
  }

  /// <summary> Initialize a new instance. </summary>
  public BocListView()
    : this (null, string.Empty, null)
  {
  }

  protected override void OnOwnerControlChanged()
  {
    base.OnOwnerControlChanged();
    _columnDefinitions.OwnerControl = OwnerControl;
  }

  public override string ToString()
  {
    string displayName = ItemID;
    if (StringUtility.IsNullOrEmpty (displayName))
      displayName = Title;
    if (StringUtility.IsNullOrEmpty (displayName))
      return DisplayedTypeName;
    else
      return string.Format ("{0}: {1}", displayName, DisplayedTypeName);
  }

  /// <summary> Gets or sets the programmatic name of the <see cref="BocListView"/>. </summary>
  /// <value> A <see cref="string"/> providing an identifier for this <see cref="BocListView"/>. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Description ("The ID of this view.")]
  [Category ("Misc")]
  [DefaultValue("")]
  [NotifyParentProperty (true)]
  [Browsable (true)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Visible)]
  public override string ItemID
  {
    get { return _itemID; }
    set { _itemID = value; }
  }

  /// <summary> Gets or sets the displayed name of the <see cref="BocListView"/>. </summary>
  /// <value> A <see cref="string"/> representing this <see cref="BocListView"/> on the rendered page. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [DefaultValue("")]
  [NotifyParentProperty (true)]
  public string Title
  {
    get { return (_title != null) ? _title.ToString() : string.Empty; }
    set { _title = value; }
  }

  /// <summary> 
  ///   Gets the <see cref="BocColumnDefinition"/> objects stored in the <see cref="BocListView"/>.  
  /// </summary>
  /// <value>
  ///   An array of <see cref="BocColumnDefinition"/> objects that comprise this <see cref="BocListView"/>.
  /// </value>
  [Editor (typeof (BocSimpleColumnDefinitionCollectionEditor), typeof (UITypeEditor))]
  [PersistenceMode (PersistenceMode.InnerDefaultProperty)]
  [Category ("Data")]
  [DefaultValue((string) null)]
  [NotifyParentProperty (true)]
  public BocColumnDefinitionCollection ColumnDefinitions
  {
    get { return _columnDefinitions; }
  }

  /// <summary> Gets the human readable name of this type. </summary>
  protected virtual string DisplayedTypeName
  {
    get { return "ListView"; }
  }
}

  
}
