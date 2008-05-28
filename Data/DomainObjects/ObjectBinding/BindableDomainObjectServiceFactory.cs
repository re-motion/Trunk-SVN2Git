using System;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// The <see cref="BindableDomainObjectServiceFactory"/> is the implementation of the <see cref="IBusinessObjectServiceFactory"/> interface for
  /// the <see cref="BindableDomainObjectProvider"/> and provides default service instances bindable domain object implementations.
  /// </summary>
  /// <remarks>
  /// The following <see cref="IBusinessObjectService"/> interfaces are supported.
  /// <list type="bullet">
  ///   <listheader>
  ///     <term>Service Interface</term>
  ///     <description>Service creates instance of type</description>
  ///   </listheader>
  ///   <item>
  ///     <term><see cref="IBindableObjectGlobalizationService"/></term>
  ///     <description><see cref="BindableObjectGlobalizationService"/></description>
  ///   </item>
  ///   <item>
  ///     <term><see cref="IBusinessObjectStringFormatterService"/></term>
  ///     <description><see cref="BusinessObjectStringFormatterService"/></description>
  ///   </item>
  ///   <item>
  ///     <term><see cref="IGetObjectService"/></term>
  ///     <description><see cref="BindableDomainObjectGetObjectService"/></description>
  ///   </item>
  ///   <item>
  ///     <term><see cref="ISearchAvailableObjectsService"/></term>
  ///     <description><see cref="BindableDomainObjectSearchService"/></description>
  ///   </item>
  /// </list>
  /// </remarks>
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