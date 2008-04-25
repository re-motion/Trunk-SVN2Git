using System;
using System.Collections.ObjectModel;
using System.Reflection;
using Remotion.ObjectBinding.BindableObject.Properties;

namespace Remotion.ObjectBinding.BindableObject
{
  public class PropertyInfoCollection : KeyedCollection<string, IPropertyInformation>
  {
    public PropertyInfoCollection ()
    {
    }

    protected override string GetKeyForItem (IPropertyInformation item)
    {
      return item.Name;
    }
  }
}