using System;
using System.Diagnostics;
using Remotion.Mixins;
using Remotion.ObjectBinding;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Apply this mixin to a type in order to add an <see cref="IBusinessObjectWithIdentity"/> implementation.
  /// </summary>
  [Serializable]
  [BindableObjectWithIdentityProvider]
  [CopyCustomAttributes (typeof (BindableObjectWithIdentityMixin.DebuggerDisplay))]
  public abstract class BindableObjectWithIdentityMixin : BindableObjectMixin, IBusinessObjectWithIdentity
  {
    [DebuggerDisplay ("{UniqueIdentifier} ({((Remotion.Mixins.IMixinTarget)this).Configuration.Type.FullName})")]
    internal class DebuggerDisplay // the attributes of this class are copied to the target class
    {
    }

    public BindableObjectWithIdentityMixin ()
    {
    }

    public abstract string UniqueIdentifier { get; }
  }
}