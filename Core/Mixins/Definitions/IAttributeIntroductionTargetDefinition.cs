using System;

namespace Remotion.Mixins.Definitions
{
  public interface IAttributeIntroductionTargetDefinition : IAttributableDefinition, IVisitableDefinition
  {
    MultiDefinitionCollection<Type, AttributeIntroductionDefinition> IntroducedAttributes { get; }
  }
}
