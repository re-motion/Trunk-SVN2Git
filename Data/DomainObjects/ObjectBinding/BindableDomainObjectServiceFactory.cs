using System;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  public class BindableDomainObjectServiceFactory : BusinessObjectServiceFactory
  {
    public override Remotion.ObjectBinding.IBusinessObjectService CreateService (Type serviceType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("serviceType", serviceType, typeof (IBusinessObjectService));

      if (serviceType == typeof (IGetObjectService))
        return new BindableDomainObjectGetObjectService();

      if (serviceType == typeof (ISearchAvailableObjectsService))
        return new BindableDomainObjectSearchService();

      return base.CreateService (serviceType);
    }

  }
}