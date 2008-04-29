using System;
using System.Collections;

namespace Remotion.ObjectBinding
{
  //TODO: doc
  public interface IListInfo
  {
    Type ItemType { get; }
    bool RequiresWriteBack { get; }
    IList CreateList (int count);
    IList InsertItem (IList list, object item, int index);
    IList RemoveItem (IList list, object item);
  }
}