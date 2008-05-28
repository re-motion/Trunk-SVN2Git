using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// The implementation of  <see cref="IBusinessObjectProvider"/> to be used with the <see cref="BindableDomainObjectMixin"/>.
  /// </summary>
  /// <remarks>
  /// This provider is associated with the <see cref="BindableDomainObjectMixin"/> via the <see cref="BindableDomainObjectProviderAttribute"/>.
  /// </remarks>
  public class BindableDomainObjectProvider : BindableObjectProvider
  {
    public BindableDomainObjectProvider (IMetadataFactory metadataFactory, IBusinessObjectServiceFactory serviceFactory)
        : base (metadataFactory, serviceFactory)
    {
    }

    public BindableDomainObjectProvider ()
        : base (BindableDomainObjectMetadataFactory.Instance, new BindableDomainObjectServiceFactory())
    {
    }
  }
}