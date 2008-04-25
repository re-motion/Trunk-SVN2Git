using System;

namespace Remotion.Mixins
{
  /// <summary>
  /// Causes the mixin engine to ignore all mixin configuration attributes on the type this attribute is applied to when building the default
  /// mixin configuration.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
  public class IgnoreForMixinConfigurationAttribute : Attribute
  {
  }
}
