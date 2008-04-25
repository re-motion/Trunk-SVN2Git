using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.ObjectBinding.BindableObject.Properties;

namespace Remotion.ObjectBinding.BindableObject
{
  public interface IPropertyFinder
  {
    IEnumerable<IPropertyInformation> GetPropertyInfos ();
  }
}