using System;
using System.Collections;
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  public class ListInfo : IListInfo
  {
    private readonly Type _propertyType;
    private readonly Type _itemType;

    public ListInfo (Type propertyType, Type itemType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("propertyType", propertyType, typeof (IList));
      ArgumentUtility.CheckNotNull ("itemType", itemType);
    
      _propertyType = propertyType;
      _itemType = itemType;
    }

    public Type PropertyType
    {
      get { return _propertyType; }
    }

    public Type ItemType
    {
      get { return _itemType; }
    }

    public bool RequiresWriteBack
    {
      get { return _propertyType.IsArray; }
    }

    public IList CreateList (int count)
    {
      return Array.CreateInstance (_itemType, count);
    }

    public IList InsertItem (IList list, object item, int index)
    {
      throw new NotImplementedException();
    }

    public IList RemoveItem (IList list, object item)
    {
      throw new NotImplementedException();
    }
  }
}