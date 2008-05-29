using System;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  /// <summary>
  /// The <see cref="SecurityManagerObjectServiceFactoryMixin"/> is an extension of the <see cref="BindableObjectServiceFactory"/> used by
  /// the <see cref="BindableObjectProvider"/> and provides default service instances for bindable domain object implementations.
  /// </summary>
  /// <remarks>
  /// The following <see cref="IBusinessObjectService"/> interfaces are supported.
  /// <list type="bullet">
  ///   <listheader>
  ///     <term>Service Interface</term>
  ///     <description>Service creates instance of type</description>
  ///   </listheader>
  ///   <item>
  ///     <term><see cref="UserPropertiesSearchService"/></term>
  ///     <description><see cref="UserPropertiesSearchService"/></description>
  ///   </item>
  ///   <item>
  ///     <term><see cref="GroupPropertiesSearchService"/></term>
  ///     <description><see cref="GroupPropertiesSearchService"/></description>
  ///   </item>
  ///   <item>
  ///     <term><see cref="RolePropertiesSearchService"/></term>
  ///     <description><see cref="RolePropertiesSearchService"/></description>
  ///   </item>
  /// </list>
  /// </remarks>
  [CLSCompliant (false)]
  [Extends (typeof (BindableObjectServiceFactory), AdditionalDependencies = new Type[] {typeof (BindableDomainObjectServiceFactoryMixin)})]
  public class SecurityManagerObjectServiceFactoryMixin
      : Mixin<BindableObjectServiceFactory, IBusinessObjectServiceFactory>, IBusinessObjectServiceFactory
  {
    public SecurityManagerObjectServiceFactoryMixin ()
    {
    }

    [OverrideTarget]
    public virtual IBusinessObjectService CreateService (Type serviceType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("serviceType", serviceType, typeof (IBusinessObjectService));

      if (serviceType == typeof (UserPropertiesSearchService))
        return new UserPropertiesSearchService();

      if (serviceType == typeof (GroupPropertiesSearchService))
        return new GroupPropertiesSearchService();

      if (serviceType == typeof (RolePropertiesSearchService))
        return new RolePropertiesSearchService();

      return Base.CreateService (serviceType);
    }
  }
}