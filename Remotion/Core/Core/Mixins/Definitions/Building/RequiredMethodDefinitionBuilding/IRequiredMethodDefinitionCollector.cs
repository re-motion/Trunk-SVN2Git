using System;
using System.Collections.Generic;

namespace Remotion.Mixins.Definitions.Building.RequiredMethodDefinitionBuilding
{
  public interface IRequiredMethodDefinitionCollector
  {
    IEnumerable<RequiredMethodDefinition> CreateRequiredMethodDefinitions (RequirementDefinitionBase requirement);
  }
}