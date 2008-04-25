using System;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building
{
  public class AttributeIntroductionDefinitionBuilder
  {
    private readonly IAttributeIntroductionTargetDefinition _target;

    public AttributeIntroductionDefinitionBuilder (IAttributeIntroductionTargetDefinition target)
    {
      _target = target;
    }

    public void Apply (IAttributableDefinition attributeSource)
    {
      foreach (AttributeDefinition attribute in attributeSource.CustomAttributes)
      {
        if (ShouldBeIntroduced (attribute))
          _target.IntroducedAttributes.Add (new AttributeIntroductionDefinition (_target, attribute));
      }
    }

    private bool ShouldBeIntroduced (AttributeDefinition attribute)
    {
      return (AttributeUtility.IsAttributeInherited (attribute.AttributeType) || attribute.IsCopyTemplate)
          && (AttributeUtility.IsAttributeAllowMultiple (attribute.AttributeType) || !_target.CustomAttributes.ContainsKey (attribute.AttributeType));
    }
  }
}