using System;

namespace Remotion.Mixins
{
  public interface IOverrideAttribute
  {
    Type OverriddenType { get; }
  }
}