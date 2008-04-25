using System;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  public class SuppressedInterfaceIntroductionDefinition : IVisitableDefinition
  {
    public readonly Type Type;
    public readonly MixinDefinition Implementer;

    private readonly bool _explicitSuppression;

    public SuppressedInterfaceIntroductionDefinition (Type type, MixinDefinition implementer, bool explicitSuppression)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("implementer", implementer);

      Type = type;
      Implementer = implementer;
      _explicitSuppression = explicitSuppression;
    }

    public bool IsExplicitlySuppressed
    {
      get { return _explicitSuppression; }
    }

    public bool IsShadowed
    {
      get { return !IsExplicitlySuppressed; }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }

    public string FullName
    {
      get { return Type.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get { return Implementer; }
    }
  }
}
