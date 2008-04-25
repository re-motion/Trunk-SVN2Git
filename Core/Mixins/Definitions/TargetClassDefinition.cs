using System;
using System.Collections.Generic;
using System.Diagnostics;
using Remotion.Mixins.Utilities;
using Remotion.Utilities;
using Remotion.Mixins.Context;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{Type}")]
  public class TargetClassDefinition : ClassDefinitionBase, IAttributeIntroductionTargetDefinition
  {
    public readonly UniqueDefinitionCollection<Type, MixinDefinition> Mixins =
        new UniqueDefinitionCollection<Type, MixinDefinition> (delegate (MixinDefinition m) { return m.Type; });
    public readonly UniqueDefinitionCollection<Type, RequiredFaceTypeDefinition> RequiredFaceTypes =
        new UniqueDefinitionCollection<Type, RequiredFaceTypeDefinition> (delegate (RequiredFaceTypeDefinition t) { return t.Type; });
    public readonly UniqueDefinitionCollection<Type, RequiredBaseCallTypeDefinition> RequiredBaseCallTypes =
        new UniqueDefinitionCollection<Type, RequiredBaseCallTypeDefinition> (delegate (RequiredBaseCallTypeDefinition t) { return t.Type; });
    public readonly UniqueDefinitionCollection<Type, RequiredMixinTypeDefinition> RequiredMixinTypes =
        new UniqueDefinitionCollection<Type, RequiredMixinTypeDefinition> (delegate (RequiredMixinTypeDefinition t) { return t.Type; });
    public readonly UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> IntroducedInterfaces =
        new UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> (delegate (InterfaceIntroductionDefinition i) { return i.Type; });
    
    private readonly MultiDefinitionCollection<Type, AttributeIntroductionDefinition> _introducedAttributes =
        new MultiDefinitionCollection<Type, AttributeIntroductionDefinition> (delegate (AttributeIntroductionDefinition a) { return a.AttributeType; });

    private readonly MixinTypeInstantiator _mixinTypeInstantiator;
    private readonly ClassContext _configurationContext;

    public TargetClassDefinition (ClassContext configurationContext)
        : base (configurationContext.Type)
    {
      ArgumentUtility.CheckNotNull ("configurationContext", configurationContext);

      _configurationContext = configurationContext;
      _mixinTypeInstantiator = new MixinTypeInstantiator (configurationContext.Type);
    }

    public ClassContext ConfigurationContext
    {
      get { return _configurationContext; }
    }

    public MultiDefinitionCollection<Type, AttributeIntroductionDefinition> IntroducedAttributes
    {
      get { return _introducedAttributes; }
    }

    internal MixinTypeInstantiator MixinTypeInstantiator
    {
      get { return _mixinTypeInstantiator; }
    }

    public bool IsInterface
    {
      get { return Type.IsInterface; }
    }

    public override IVisitableDefinition Parent
    {
      get { return null; }
    }

    protected override void ChildSpecificAccept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.Visit (this);
      
      Mixins.Accept (visitor);
      RequiredFaceTypes.Accept (visitor);
      RequiredBaseCallTypes.Accept (visitor);
      RequiredMixinTypes.Accept (visitor);
      IntroducedAttributes.Accept (visitor);
    }

    public bool HasMixinWithConfiguredType(Type configuredType)
    {
      Type realType = _mixinTypeInstantiator.GetClosedMixinType (configuredType);
      return Mixins.ContainsKey (realType);
    }

    public MixinDefinition GetMixinByConfiguredType(Type configuredType)
    {
      Type realType = _mixinTypeInstantiator.GetClosedMixinType (configuredType);
      return Mixins[realType];
    }

    public IEnumerable<MethodDefinition> GetAllMixinMethods()
    {
      foreach (MixinDefinition mixin in Mixins)
        foreach (MethodDefinition method in mixin.Methods)
          yield return method;
    }

    public IEnumerable<PropertyDefinition> GetAllMixinProperties ()
    {
      foreach (MixinDefinition mixin in Mixins)
        foreach (PropertyDefinition property in mixin.Properties)
          yield return property;
    }

    public IEnumerable<EventDefinition> GetAllMixinEvents ()
    {
      foreach (MixinDefinition mixin in Mixins)
        foreach (EventDefinition eventDefinition in mixin.Events)
          yield return eventDefinition;
    }

    public bool IsAbstract
    {
      get { return Type.IsAbstract; }
    }
  }
}
