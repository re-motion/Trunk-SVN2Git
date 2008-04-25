using System;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  public class AttributeIntroductionDefinition : IVisitableDefinition
  {
    public readonly IAttributeIntroductionTargetDefinition Target;
    public readonly AttributeDefinition Attribute;

    public AttributeIntroductionDefinition (IAttributeIntroductionTargetDefinition target, AttributeDefinition attribute)
    {
      ArgumentUtility.CheckNotNull ("target", target);
      ArgumentUtility.CheckNotNull ("attribute", attribute);

      Target = target;
      Attribute = attribute;
    }

    public Type AttributeType
    {
      get { return Attribute.AttributeType; }
    }

    public string FullName
    {
      get { return Attribute.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get { return Target; }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.Visit (this);
    }
  }
}
