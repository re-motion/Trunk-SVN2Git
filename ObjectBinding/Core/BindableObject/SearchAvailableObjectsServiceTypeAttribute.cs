using System;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  //TODO: doc
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class SearchAvailableObjectsServiceTypeAttribute : Attribute
  {
    private readonly Type _type;

    public SearchAvailableObjectsServiceTypeAttribute (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (ISearchAvailableObjectsService));
      _type = type;
    }

    public Type Type
    {
      get { return _type; }
    }
  }
}