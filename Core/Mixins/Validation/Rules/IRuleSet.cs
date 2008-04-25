using System;

namespace Remotion.Mixins.Validation.Rules
{
  public interface IRuleSet
  {
    void Install (ValidatingVisitor visitor);
  }
}
