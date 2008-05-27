using System;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Apply this attribute to your BindableObjectWithIdentity-type to classify the this type as using the default reflection based object binding implementation.
  /// </summary>
  /// <remarks>
  /// The <see cref="BindableObjectMixin"/> already applies this attribute.
  /// </remarks>
  public class BindableObjectWithIdentityProviderAttribute : BusinessObjectProviderAttribute
  {
    public BindableObjectWithIdentityProviderAttribute ()
        : base (typeof (BindableObjectProvider))
    {
    }
  }
}