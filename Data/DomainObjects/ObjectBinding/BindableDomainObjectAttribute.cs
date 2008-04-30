using System;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class BindableDomainObjectAttribute : UsesAttribute
  {
    public BindableDomainObjectAttribute ()
        : base (typeof (BindableDomainObjectMixin))
    {
    }
  }
}