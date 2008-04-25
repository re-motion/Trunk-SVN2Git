using System;
using System.Diagnostics;
using Remotion.Mixins;

namespace Remotion.ObjectBinding.BindableObject
{
  //TODO: doc
  [Serializable]
  [CopyCustomAttributes (typeof (BindableObjectWithIdentityMixin.DebuggerDisplay))]
  public abstract class BindableObjectWithIdentityMixin : BindableObjectMixin, IBusinessObjectWithIdentity
  {
    [DebuggerDisplay ("{UniqueIdentifier} ({((Remotion.Mixins.IMixinTarget)this).Configuration.Type.FullName})")]
    public class DebuggerDisplay // the attributes of this class are copied to the target class
    {
    }

    public BindableObjectWithIdentityMixin ()
    {
    }

    public abstract string UniqueIdentifier { get; }
  }
}