using System;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  public class PropertyIntroductionDefinition : MemberIntroductionDefinition<PropertyInfo, PropertyDefinition>
  {
    private bool _introducesGetMethod;
    private bool _introducesSetMethod;

    public PropertyIntroductionDefinition (InterfaceIntroductionDefinition declaringInterface, PropertyInfo interfaceMember, PropertyDefinition implementingMember)
        : base (declaringInterface, interfaceMember, implementingMember)
    {
      _introducesGetMethod = interfaceMember.GetGetMethod() != null;
      _introducesSetMethod = interfaceMember.GetSetMethod () != null;
    }

    public bool IntroducesGetMethod
    {
      get { return _introducesGetMethod; }
    }

    public bool IntroducesSetMethod
    {
      get { return _introducesSetMethod; }
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}
