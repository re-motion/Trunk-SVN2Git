using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Utilities;
using System.Reflection;

namespace Remotion.Mixins.Definitions
{
  public class RequiredBaseCallTypeDefinition : RequirementDefinitionBase
  {
    public RequiredBaseCallTypeDefinition (TargetClassDefinition targetClass, Type type)
        : base(targetClass, type)
    {
    }

    protected override void ConcreteAccept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}
