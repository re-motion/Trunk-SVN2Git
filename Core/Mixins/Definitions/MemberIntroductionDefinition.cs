using System;
using System.Diagnostics;
using Remotion.Utilities;
using System.Reflection;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{InterfaceMember}")]
  public abstract class MemberIntroductionDefinition<TMemberInfo, TMemberDefinition>: IVisitableDefinition
      where TMemberInfo : MemberInfo
      where TMemberDefinition : MemberDefinition
  {
    private InterfaceIntroductionDefinition _declaringInterface;
    private TMemberInfo _interfaceMember;
    private TMemberDefinition _implementingMember;

    public MemberIntroductionDefinition (
        InterfaceIntroductionDefinition declaringInterface, TMemberInfo interfaceMember, TMemberDefinition implementingMember)
    {
      ArgumentUtility.CheckNotNull ("interfaceMember", interfaceMember);
      ArgumentUtility.CheckNotNull ("declaringInterface", declaringInterface);
      ArgumentUtility.CheckNotNull ("implementingMember", implementingMember);

      _declaringInterface = declaringInterface;
      _implementingMember = implementingMember;
      _interfaceMember = interfaceMember;
    }

    public InterfaceIntroductionDefinition DeclaringInterface
    {
      get { return _declaringInterface; }
    }

    public TMemberInfo InterfaceMember
    {
      get { return _interfaceMember; }
    }

    public TMemberDefinition ImplementingMember
    {
      get { return _implementingMember; }
    }

    public string FullName
    {
      get { return DeclaringInterface.FullName + "." + InterfaceMember.Name; }
    }

    public IVisitableDefinition Parent
    {
      get { return DeclaringInterface; }
    }

    public abstract void Accept (IDefinitionVisitor visitor);
  }
}