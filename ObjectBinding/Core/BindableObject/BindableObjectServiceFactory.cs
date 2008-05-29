using System;
using Remotion.Mixins;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// The <see cref="BindableObjectServiceFactory"/> is the default implementation of the <see cref="IBusinessObjectServiceFactory"/>
  /// and provides service instances common for all bindable object implementations.
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
  /// </list>
  /// </remarks>
  public class BindableObjectServiceFactory : IBusinessObjectServiceFactory
  {
    public static BindableObjectServiceFactory Create ()
    {
      return ObjectFactory.Create<BindableObjectServiceFactory> (true).With();
    }

    protected BindableObjectServiceFactory ()
    {
    }

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