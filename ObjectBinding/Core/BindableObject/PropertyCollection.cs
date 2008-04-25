using System;
using System.Collections.ObjectModel;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  public class PropertyCollection : KeyedCollection<string, PropertyBase>
  {
    public PropertyCollection ()
    {
    }

    protected override string GetKeyForItem (PropertyBase item)
    {
      return item.Identifier;
    }

    public PropertyBase[] ToArray ()
    {
      return ArrayUtility.Convert (Items);
    }
  }
}