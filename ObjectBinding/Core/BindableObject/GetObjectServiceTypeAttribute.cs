using System;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  //TODO: doc
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class GetObjectServiceTypeAttribute : Attribute
  {
    private readonly Type _type;

    public GetObjectServiceTypeAttribute (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (IGetObjectService));
      _type = type;
    }

    public Type Type
    {
      get { return _type; }
    }
  }
}