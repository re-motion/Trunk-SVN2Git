using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Mixins.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  public class MethodDefinition : MemberDefinition, IVisitableDefinition
  {
    private static SignatureChecker s_signatureChecker = new SignatureChecker ();

    public readonly new UniqueDefinitionCollection<Type, MethodDefinition> Overrides =
        new UniqueDefinitionCollection<Type, MethodDefinition> (delegate (MethodDefinition m) { return m.DeclaringClass.Type; });

    private MethodDefinition _base;

    public MethodDefinition (MethodInfo memberInfo, ClassDefinitionBase declaringClass)
        : base (memberInfo, declaringClass)
    {
    }

    public MethodInfo MethodInfo
    {
      get { return (MethodInfo) MemberInfo; }
    }

    public override MemberDefinition BaseAsMember
    {
      get { return _base; }
      set
      {
        if (value == null || value is MethodDefinition)
          _base = (MethodDefinition) value;
        else
          throw new ArgumentException ("Base must be MethodDefinition or null.", "value");
      }
    }

    public MethodDefinition Base
    {
      get { return _base; }
      set { BaseAsMember = value; }
    }

    public bool IsAbstract
    {
      get { return MethodInfo.IsAbstract; }
    }

    protected override IDefinitionCollection<Type, MemberDefinition> GetInternalOverridesWrapper()
    {
      return new CovariantDefinitionCollectionWrapper<Type, MethodDefinition, MemberDefinition>(Overrides);
    }

    protected override bool IsSignatureCompatibleWith (MemberDefinition overrider)
    {
      ArgumentUtility.CheckNotNull ("overrider", overrider);

      MethodDefinition overriderMethod = overrider as MethodDefinition;
      if (overriderMethod == null)
      {
        return false;
      }
      else
      {
        return IsSignatureCompatibleWithMethod (overriderMethod);
      }
    }

    private bool IsSignatureCompatibleWithMethod (MethodDefinition overrider)
    {
      ArgumentUtility.CheckNotNull ("overrider", overrider);
      return s_signatureChecker.MethodSignaturesMatch (MethodInfo, overrider.MethodInfo);
    }

    internal override void AddOverride (MemberDefinition member)
    {
      ArgumentUtility.CheckNotNull ("member", member);

      MethodDefinition method = member as MethodDefinition;
      if (method == null)
      {
        string message = string.Format ("Member {0} cannot override method {1} - it is not a method.", member.FullName, FullName);
        throw new ArgumentException (message);
      }

      Overrides.Add (method);
    }

    protected override void ChildSpecificAccept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}