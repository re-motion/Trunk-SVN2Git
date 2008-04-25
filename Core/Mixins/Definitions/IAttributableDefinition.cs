using System;

namespace Remotion.Mixins.Definitions
{
  public interface IAttributableDefinition
  {
    MultiDefinitionCollection<Type, AttributeDefinition> CustomAttributes { get; }
  }
}
