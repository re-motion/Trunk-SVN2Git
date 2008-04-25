using System;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  public class BaseDependencyDefinition : DependencyDefinitionBase
  {
    public BaseDependencyDefinition (RequiredBaseCallTypeDefinition requiredType, MixinDefinition depender, BaseDependencyDefinition aggregator)
      : base (requiredType, depender, aggregator)
    {
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }

    public override ClassDefinitionBase GetImplementer ()
    {
      ClassDefinitionBase implementer = base.GetImplementer ();
      // check for duck interface
      if (implementer == null && !RequiredType.IsEmptyInterface)
      {
        implementer = Depender.TargetClass; // duck interface
      }
      return implementer;
    }

    public new RequiredBaseCallTypeDefinition RequiredType
    {
      get { return (RequiredBaseCallTypeDefinition) base.RequiredType; }
    }
  }
}
