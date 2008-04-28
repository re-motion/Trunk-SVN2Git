using System;
using Remotion.Implementation;

namespace Remotion.Mixins
{
  /// <summary>
  /// When applied to a mixin, specifies that this mixin does not introduce a specific interface to the target class.
  /// </summary>
  /// <remarks>Use this attribute if a mixin should implement an interface "just for itself" and the interface should not be
  /// forwarded to the target class.</remarks>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public class NonIntroducedAttribute : Attribute
  {
    private readonly Type _suppressedInterface;

    public NonIntroducedAttribute (Type suppressedInterface)
    {
      _suppressedInterface = ArgumentUtility.CheckNotNull ("suppressedInterface", suppressedInterface);
    }

    public Type SuppressedInterface
    {
      get { return _suppressedInterface; }
    }
  }
}
