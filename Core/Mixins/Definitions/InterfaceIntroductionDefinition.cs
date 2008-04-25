using System;
using Remotion.Utilities;
using System.Reflection;
using System.Diagnostics;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{_type.FullName} introduced via {_implementer.FullName}")]
  public class InterfaceIntroductionDefinition : IVisitableDefinition
  {
    public readonly UniqueDefinitionCollection<MethodInfo, MethodIntroductionDefinition> IntroducedMethods =
        new UniqueDefinitionCollection<MethodInfo, MethodIntroductionDefinition> (delegate (MethodIntroductionDefinition m) { return m.InterfaceMember; });
    public readonly UniqueDefinitionCollection<PropertyInfo, PropertyIntroductionDefinition> IntroducedProperties =
        new UniqueDefinitionCollection<PropertyInfo, PropertyIntroductionDefinition> (delegate (PropertyIntroductionDefinition m) { return m.InterfaceMember; });
    public readonly UniqueDefinitionCollection<EventInfo, EventIntroductionDefinition> IntroducedEvents =
        new UniqueDefinitionCollection<EventInfo, EventIntroductionDefinition> (delegate (EventIntroductionDefinition m) { return m.InterfaceMember; });

    private Type _type;
    private MixinDefinition _implementer;

    public InterfaceIntroductionDefinition (Type type, MixinDefinition implementer)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("implementer", implementer);

      _type = type;
      _implementer = implementer;
    }

    public Type Type
    {
      get { return _type; }
    }

    public MixinDefinition Implementer
    {
      get { return _implementer; }
    }

    public string FullName
    {
      get { return Type.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get { return Implementer; }
    }

    public TargetClassDefinition TargetClass
    {
      get { return Implementer.TargetClass; }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
      IntroducedMethods.Accept (visitor);
      IntroducedProperties.Accept (visitor);
      IntroducedEvents.Accept (visitor);
    }
  }
}
