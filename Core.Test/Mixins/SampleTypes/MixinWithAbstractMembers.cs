using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IMixinWithAbstractMembers
  {
    string ImplementedMethod ();
    string ImplementedProperty ();
    string ImplementedEvent ();
  }

  [Serializable]
  public abstract class MixinWithAbstractMembers : Mixin<object, object>, IMixinWithAbstractMembers
  {
    public int I;

    public string ImplementedMethod ()
    {
      return "MixinWithAbstractMembers.ImplementedMethod-" + AbstractMethod (25);
    }

    public string ImplementedProperty ()
    {
      return "MixinWithAbstractMembers.ImplementedProperty-" + AbstractProperty;
    }

    public string ImplementedEvent ()
    {
      Func<string> func = delegate { return "MixinWithAbstractMembers.ImplementedEvent"; };
      AbstractEvent += func;
      string result = RaiseEvent ();
      AbstractEvent -= func;
      return result;
    }

    protected abstract string AbstractMethod (int i);
    protected abstract string AbstractProperty { get; }
    protected abstract event Func<string> AbstractEvent;
    protected abstract string RaiseEvent ();
  }
}
