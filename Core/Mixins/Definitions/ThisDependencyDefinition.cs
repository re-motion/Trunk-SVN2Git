using System;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  public class ThisDependencyDefinition : DependencyDefinitionBase
  {
    public ThisDependencyDefinition (RequiredFaceTypeDefinition requiredType, MixinDefinition depender, ThisDependencyDefinition aggregator)
      : base (requiredType, depender, aggregator)
    {
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }

    public new RequiredFaceTypeDefinition RequiredType
    {
      get { return (RequiredFaceTypeDefinition) base.RequiredType; }
    }
  }
}
