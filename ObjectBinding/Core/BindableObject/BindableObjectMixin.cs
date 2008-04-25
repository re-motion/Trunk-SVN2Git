using System;
using Remotion.Mixins.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  //TODO: doc
  [Serializable]
  public class BindableObjectMixin : BindableObjectMixinBase<object>
  {
    public BindableObjectMixin ()
    {
    }

    protected override BindableObjectClass InitializeBindableObjectClass()
    {
      Type targetType = MixinReflector.GetMixinConfiguration (this, This).TargetClass.Type;
      return BindableObjectProvider.Current.GetBindableObjectClass (targetType);
    }
  }
}