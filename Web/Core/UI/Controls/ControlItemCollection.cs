using System;
using System.Collections;
using System.ComponentModel;
using System.Web.UI;
using Remotion.Globalization;
using Remotion.Logging;
using Remotion.Utilities;
using Remotion.Web.UI.Globalization;

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   Provides <see cref="IComponent"/>-like functionality for non-UI items of controls.
/// </summary>
/// <remarks>
///   <b>IComponent</b> is not used because it involves CodeDOM designer serialization.
/// </remarks>
public interface IControlItem
{
  Control OwnerControl { get; set; }
  string ItemID { get; }
  void LoadResources (IResourceManager resourceManager);
}

/// <summary>
///   Colletion of <see cref="IControlItem"/>.
/// </summary>
public class ControlItemCollection: CollectionBase
{
  private static readonly ILog s_log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

  private Control _ownerControl;
  private Type[] _supportedTypes;
  /// <summary> true if BeginEdit was called. </summary>
  private bool _isEditing;
  /// <summary> true if _isEditing is true and the collection's values got changed. </summary>
  private bool _isChanged;

  /// <summary> The event raised after the items contained in the collection has been changed. </summary>
  public event CollectionChangeEventHandler CollectionChanged;

  /// <summary> Creates a new instance. </summary>
  /// <param name="ownerControl"> Owner control. </param>
  /// <param name="supportedTypes">
  ///   Supported types must implement <see cref="IControlItem"/>. 
  ///   Must not be <see langword="null"/> or contain items that are <see langword="null"/>.
  /// </param>
  public ControlItemCollection (Control ownerControl, Type[] supportedTypes)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("supportedTypes", supportedTypes);
    for (int i = 0; i < supportedTypes.Length; i++)
    {
      Type type = supportedTypes[i];
      if (! typeof (IControlItem).IsAssignableFrom (type)) 
        throw new ArgumentException (string.Format ("Type '{0}' at index {1} does not implement interface 'IControlItem'.", type.FullName, i), "type");
    }

    _ownerControl = ownerControl;
    _supportedTypes = supportedTypes;
  }

  /// <summary> Creates a new instance. </summary>
  /// <param name="ownerControl"> Owner control. </param>
  /// <param name="supportedTypes"> 
  ///   Supported types must implement <see cref="IControlItem"/>. 
  ///   Must not be <see langword="null"/> or contain items that are <see langword="null"/>.
  /// </param>
  public ControlItemCollection (IControl ownerControl, Type[] supportedTypes)
    : this ((Control) ownerControl, supportedTypes)
  {
  }

  /// <summary> Places the collection into edit mode. </summary>
  /// <remarks> No individual <see cref="CollectionChanged"/> events are raised during edit mode. </remarks>
  public void BeginEdit()
  {
    _isEditing = true;
  }

  /// <summary> Returns the collection to normal mode. </summary>
  /// <remarks> A common <see cref="CollectionChanged"/> event is raised if changes occured during edit-mode. </remarks>
  public void EndEdit()
  {
    if (_isEditing)
    {
      _isEditing = false;
      if (_isChanged)
      {
        _isChanged = false;
        OnCollectionChanged (new CollectionChangeEventArgs (CollectionChangeAction.Refresh, null));
      }
    }
  }

  /// <summary> Raises the <see cref="CollectionChanged"/> event. </summary>
  private void OnCollectionChanged (CollectionChangeEventArgs e)
  {
    if (CollectionChanged != null && !_isEditing)
      CollectionChanged(this, e);
  }

  protected virtual void ValidateNewValue (object value)
  {
    IControlItem controlItem = ArgumentUtility.CheckNotNullAndType<IControlItem> ("value", value);
    
    if (! IsSupportedType (controlItem)) 
      throw new ArgumentTypeException ("value", controlItem.GetType());
    if (Find (controlItem.ItemID) != null)
      throw new ArgumentException (string.Format ("The collection already contains an item with ItemID '{0}'.", controlItem.ItemID), "value");
  }

  protected override void OnInsert (int index, object value)
  {
    ValidateNewValue (value);

    base.OnInsert (index, value);
    IControlItem controlItem = (IControlItem) value;
    controlItem.OwnerControl = _ownerControl;
  }

  protected override void OnInsertComplete (int index, object value)
  {
    ArgumentUtility.CheckNotNull ("value", value);

    base.OnInsertComplete (index, value);
    _isChanged |= _isEditing;
    OnCollectionChanged (new CollectionChangeEventArgs (CollectionChangeAction.Add, value));
  }

  protected override void OnSet (int index, object oldValue, object newValue)
  {
    ValidateNewValue (newValue);

    base.OnSet (index, oldValue, newValue);
    IControlItem controlItem = (IControlItem) newValue;
    controlItem.OwnerControl = _ownerControl;
  }

  protected override void OnSetComplete(int index, object oldValue, object newValue)
  {
    ArgumentUtility.CheckNotNull ("oldValue", oldValue);
    ArgumentUtility.CheckNotNull ("newValue", newValue);

    base.OnSetComplete (index, oldValue, newValue);
    _isChanged |= _isEditing;
    OnCollectionChanged (new CollectionChangeEventArgs (CollectionChangeAction.Remove, oldValue));
    OnCollectionChanged (new CollectionChangeEventArgs (CollectionChangeAction.Add, newValue));
  }

  protected override void OnRemoveComplete(int index, object value)
  {
    ArgumentUtility.CheckNotNull ("value", value);

    base.OnRemoveComplete (index, value);
    _isChanged |= _isEditing;
    OnCollectionChanged (new CollectionChangeEventArgs (CollectionChangeAction.Remove, value));
  }

  public int Add (IControlItem value)
  {
    int count = List.Add (value);
    return count;
  }

  public void AddRange (params IControlItem[] values)
  {
    AddRange ((IList) values);
  }

  protected void AddRange (IList values)
  {
    ArgumentUtility.CheckNotNull ("values", values);
    ArgumentUtility.CheckItemsNotNullAndType ("values", values, typeof (IControlItem));

    BeginEdit();
    for (int i = 0; i < values.Count; i++)
      Add ((IControlItem) values[i]);
    EndEdit();
  }

  public void Insert (int index, IControlItem value)
  {
    List.Insert (index, value);
  }

  /// <remarks> Redefine this member in a derived class if you wish to return a more specific array. </remarks>
  public IControlItem[] ToArray()
  {
    return (IControlItem[]) InnerList.ToArray (typeof (IControlItem));
  }

  public virtual void Sort()
  {
    Sort (Comparer.Default);
  }

  public virtual void Sort (IComparer comparer)
  {
    Sort (0, Count, comparer);
  }

  public virtual void Sort (int index, int count, IComparer comparer)
  {
    InnerList.Sort (index, count, comparer);
  }

  public int IndexOf (IControlItem item)
  {
    return InnerList.IndexOf (item);
  }

  /// <summary> Finds the <see cref="IControlItem"/> with an <see cref="IControlItem.ItemID"/> of <paramref name="id"/>. </summary>
  /// <param name="id"> The ID to look for. </param>
  /// <returns> An <see cref="IControlItem"/> or <see langword="null"/> if no matching item was found. </returns>
  public IControlItem Find (string id)
  {
    if (StringUtility.IsNullOrEmpty (id))
      return null;

    for (int i = 0; i < InnerList.Count; i++)
    {
      IControlItem item = (IControlItem) InnerList[i];
      if (item.ItemID == id)
        return item;
    }
    return null;
  }

  /// <remarks> 
  ///   Do not redefine the indexer as a public member in any derived class if you intend to use it in a peristed
  ///   property. Otherwise ASP.net will not know which property to use, this one or the new one.
  ///   It is possible to redefine it as a non-public member.
  /// </remarks>
  public IControlItem this[int index]
  {
    get { return (IControlItem) List[index]; }
    set { List[index] = value; }
  }

  /// <summary> Tests whether the specified control item's type is supported by the collection. </summary>
  private bool IsSupportedType (IControlItem controlItem)
  {
    Type controlItemType = controlItem.GetType();

    for (int i = 0; i < _supportedTypes.Length; i++)
    {
      Type type = _supportedTypes[i];
      if (type.IsAssignableFrom (controlItemType))
        return true;
    }
    
    return false;
  }

  /// <summary> Gets or sets the control to which this collection belongs. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public Control OwnerControl
  {
    get { return _ownerControl; }
    set 
    {
      _ownerControl = value; 
      for (int i = 0; i < InnerList.Count; i++)
      {
        IControlItem controlItem = (IControlItem) InnerList[i];
        controlItem.OwnerControl = _ownerControl;
      }
    }
  }

  public void Dispatch (IDictionary values, Control parent, string collectionName)
  {
    string parentID = string.Empty;
    string page = string.Empty;
    if (parent != null)
    {
      parentID = parent.UniqueID;
      page = parent.Page.ToString();
    }

    foreach (DictionaryEntry entry in values)
    {
      string id = (string) entry.Key;
      
      IControlItem item = Find (id);
      if (item != null)
        ResourceDispatcher.DispatchGeneric (item, (IDictionary) entry.Value);
      else  //  Invalid collection element
        s_log.Debug ("'" + parentID + "' on page '" + page + "' does not contain an item with an ID of '" + id + "' inside the collection '" + collectionName + "'.");
    }
  }

  public void LoadResources (IResourceManager resourceManager)
  {
    if (resourceManager == null)
      return;

    for (int i = 0; i < InnerList.Count; i++)
    {
      IControlItem controlItem = (IControlItem) InnerList[i];
      controlItem.LoadResources (resourceManager);
    }
  }
}

}
