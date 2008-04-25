using System;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  public class MethodIntroductionDefinition : MemberIntroductionDefinition<MethodInfo, MethodDefinition>
  {
    public MethodIntroductionDefinition (InterfaceIntroductionDefinition declaringInterface, MethodInfo interfaceMember, MethodDefinition implementingMember)
        : base (declaringInterface, interfaceMember, implementingMember)
    {
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}
