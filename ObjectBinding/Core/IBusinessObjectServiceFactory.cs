using System;
using Remotion.ObjectBinding;

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// Use the <see cref="IBusinessObjectServiceFactory"/> to create default instances for <see cref="IBusinessObjectService"/> types 
  /// requested via the <see cref="BusinessObjectProvider.GetService"/> method.
  /// </summary>
  public interface IBusinessObjectServiceFactory
  {
    IBusinessObjectService CreateService (IBusinessObjectProviderWithIdentity provider, Type serviceType);
  }
}