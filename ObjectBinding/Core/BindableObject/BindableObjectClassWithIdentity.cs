using System;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  //TODO: doc
  public class BindableObjectClassWithIdentity : BindableObjectClass, IBusinessObjectClassWithIdentity
  {
    private readonly Type _getObjectServiceType;

    public BindableObjectClassWithIdentity (Type concreteType, BindableObjectProvider businessObjectProvider)
        : base (concreteType, businessObjectProvider)
    {
      _getObjectServiceType = GetGetObjectServiceType();
    }

    public IBusinessObjectWithIdentity GetObject (string uniqueIdentifier)
    {
      IGetObjectService service = GetGetObjectService();
      return service.GetObject (this, uniqueIdentifier);
    }

    private IGetObjectService GetGetObjectService ()
    {
      IGetObjectService service = (IGetObjectService) BusinessObjectProvider.GetService (_getObjectServiceType);
      if (service == null)
      {
        throw new InvalidOperationException (
            string.Format (
                "The '{0}' required for loading objectes of type '{1}' is not registered with the '{2}' associated with this type.",
                _getObjectServiceType.FullName,
                TargetType.FullName,
                typeof (BusinessObjectProvider).FullName));
      }
      return service;
    }

    private Type GetGetObjectServiceType ()
    {
      GetObjectServiceTypeAttribute attribute = AttributeUtility.GetCustomAttribute<GetObjectServiceTypeAttribute> (ConcreteType, true);
      if (attribute == null)
        return typeof (IGetObjectService);
      return attribute.Type;
    }
  }
}