using System;
using Remotion.Mixins.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Apply this mixin to a type in order to add an <see cref="IBusinessObject"/> implementation.
  /// </summary>
  [Serializable]
  [BindableObjectProvider]
  public class BindableObjectMixin : BindableObjectMixinBase<object>
  {
    public BindableObjectMixin ()
    {
    }

    protected override BindableObjectClass InitializeBindableObjectClass()
    {
      Type targetType = MixinReflector.GetMixinConfiguration (this, This).TargetClass.Type;
      return BindableObjectProvider.GetBindableObjectClassFromProvider (targetType);
    }
  }
}