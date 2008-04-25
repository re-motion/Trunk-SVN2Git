using System;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding
{
  //TODO: doc
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class BindableObjectAttribute : UsesAttribute
  {
    public BindableObjectAttribute ()
        : base (typeof (BindableObjectMixin))
    {
    }
  }
}