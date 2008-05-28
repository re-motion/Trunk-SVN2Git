using System;
using Remotion.ObjectBinding;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// Apply this attribute to your BindableDomainObject-type to classify the this type as using the DomainObject-specific implementation of object binding.
  /// </summary>
  /// <remarks>
  /// The <see cref="BindableDomainObjectMixin"/> already applies this attribute.
  /// </remarks>
  public class BindableDomainObjectProviderAttribute : BusinessObjectProviderAttribute
  {
    public BindableDomainObjectProviderAttribute ()
        : base (typeof (BindableDomainObjectProvider))
    {
    }
  }
}