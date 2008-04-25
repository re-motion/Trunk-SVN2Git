using System;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  public class MixinDependencyDefinition : DependencyDefinitionBase
  {
    public MixinDependencyDefinition (RequiredMixinTypeDefinition requiredType, MixinDefinition depender, MixinDependencyDefinition aggregator)
      : base (requiredType, depender, aggregator)
    {
    }

    public override ClassDefinitionBase GetImplementer ()
    {
      if (RequiredType.Type.IsInterface)
        return Depender.TargetClass.IntroducedInterfaces.ContainsKey (RequiredType.Type)
            ? Depender.TargetClass.IntroducedInterfaces[RequiredType.Type].Implementer : null;
      else
        return Depender.TargetClass.Mixins[RequiredType.Type];
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }

    public new RequiredMixinTypeDefinition RequiredType
    {
      get { return (RequiredMixinTypeDefinition) base.RequiredType; }
    }
  }
}
