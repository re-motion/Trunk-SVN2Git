using System;
using Remotion.ObjectBinding;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Use the <see cref="BusinessObjectServiceFactory"/> is the default implementation of the <see cref="IBusinessObjectServiceFactory"/>
  /// and provides service instances common for all bindable object implementations.
  /// </summary>
  public class BusinessObjectServiceFactory : IBusinessObjectServiceFactory
  {
    public virtual IBusinessObjectService CreateService (Type serviceType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("serviceType", serviceType, typeof (IBusinessObjectService));

      if (serviceType == typeof (IBindableObjectGlobalizationService))
        return new BindableObjectGlobalizationService();

      if (serviceType == typeof (IBusinessObjectStringFormatterService))
        return new BusinessObjectStringFormatterService();

      return null;
    }
  }
}