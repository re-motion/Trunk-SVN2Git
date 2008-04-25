using System;
using System.Diagnostics;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{InterfaceMethod}")]
  public class RequiredMethodDefinition : IVisitableDefinition
  {
    private RequirementDefinitionBase _declaringRequirement;
    private readonly MethodInfo _interfaceMethod;
    private readonly MethodDefinition _implementingMethod;

    public RequiredMethodDefinition (RequirementDefinitionBase declaringRequirement, MethodInfo interfaceMethod, MethodDefinition implementingMethod)
    {
      ArgumentUtility.CheckNotNull ("declaringRequirement", declaringRequirement);
      ArgumentUtility.CheckNotNull ("implementingMethod", implementingMethod);
      ArgumentUtility.CheckNotNull ("interfaceMethod", interfaceMethod);

      _declaringRequirement = declaringRequirement;
      _interfaceMethod = interfaceMethod;
      _implementingMethod = implementingMethod;
    }

    public RequirementDefinitionBase DeclaringRequirement
    {
      get { return _declaringRequirement; }
    }

    public MethodInfo InterfaceMethod
    {
      get { return _interfaceMethod; }
    }

    public MethodDefinition ImplementingMethod
    {
      get { return _implementingMethod; }
    }

    public string FullName
    {
      get { return _declaringRequirement.FullName + "." + _interfaceMethod.Name; }
    }

    public IVisitableDefinition Parent
    {
      get { return _declaringRequirement; }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}