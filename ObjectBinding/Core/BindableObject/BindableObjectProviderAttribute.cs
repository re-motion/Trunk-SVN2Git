using System;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Apply this attribute to your BindableObject-type to classify the this type as using the default reflection based object binding implementation.
  /// </summary>
  /// <remarks>
  /// The <see cref="BindableObjectMixin"/> already applies this attribute.
  /// </remarks>
  public class BindableObjectProviderAttribute : BusinessObjectProviderAttribute
  {
    public BindableObjectProviderAttribute ()
        : base (typeof (BindableObjectProvider))
    {
    }
  }
}