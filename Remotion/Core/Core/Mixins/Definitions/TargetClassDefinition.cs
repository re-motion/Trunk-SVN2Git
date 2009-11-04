// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Diagnostics;
using Remotion.Mixins.Context;
using Remotion.Mixins.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{Type}")]
  public class TargetClassDefinition : ClassDefinitionBase, IAttributeIntroductionTarget
  {
    private readonly UniqueDefinitionCollection<Type, MixinDefinition> _mixins =
        new UniqueDefinitionCollection<Type, MixinDefinition> (m => m.Type);
    private readonly UniqueDefinitionCollection<Type, RequiredFaceTypeDefinition> _requiredFaceTypes =
        new UniqueDefinitionCollection<Type, RequiredFaceTypeDefinition> (t => t.Type);
    private readonly UniqueDefinitionCollection<Type, RequiredBaseCallTypeDefinition> _requiredBaseCallTypes =
        new UniqueDefinitionCollection<Type, RequiredBaseCallTypeDefinition> (t => t.Type);
    private readonly UniqueDefinitionCollection<Type, RequiredMixinTypeDefinition> _requiredMixinTypes =
        new UniqueDefinitionCollection<Type, RequiredMixinTypeDefinition> (t => t.Type);
    
    private readonly UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> _receivedInterfaces =
        new UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> (i => i.InterfaceType);
    private readonly MultiDefinitionCollection<Type, AttributeIntroductionDefinition> _receivedAttributes;

    private readonly ClassContext _configurationContext;
    private readonly MixinTypeCloser _mixinTypeCloser;

    public TargetClassDefinition (ClassContext configurationContext)
        : base (configurationContext.Type)
    {
      ArgumentUtility.CheckNotNull ("configurationContext", configurationContext);

      _receivedAttributes = new MultiDefinitionCollection<Type, AttributeIntroductionDefinition> (a => a.AttributeType);

      _configurationContext = configurationContext;
      _mixinTypeCloser = new MixinTypeCloser (configurationContext.Type);
    }

    public MultiDefinitionCollection<Type, AttributeIntroductionDefinition> ReceivedAttributes
    {
      get { return _receivedAttributes; }
    }

    public ClassContext ConfigurationContext
    {
      get { return _configurationContext; }
    }

    public MixinTypeCloser MixinTypeCloser
    {
      get { return _mixinTypeCloser; }
    }

    public bool IsInterface
    {
      get { return Type.IsInterface; }
    }

    public bool IsAbstract
    {
      get { return Type.IsAbstract; }
    }

    public override IVisitableDefinition Parent
    {
      get { return null; }
    }

    public UniqueDefinitionCollection<Type, MixinDefinition> Mixins
    {
      get { return _mixins; }
    }

    public UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> ReceivedInterfaces
    {
      get { return _receivedInterfaces; }
    }

    public UniqueDefinitionCollection<Type, RequiredMixinTypeDefinition> RequiredMixinTypes
    {
      get { return _requiredMixinTypes; }
    }

    public UniqueDefinitionCollection<Type, RequiredBaseCallTypeDefinition> RequiredBaseCallTypes
    {
      get { return _requiredBaseCallTypes; }
    }

    public UniqueDefinitionCollection<Type, RequiredFaceTypeDefinition> RequiredFaceTypes
    {
      get { return _requiredFaceTypes; }
    }

    protected override void ChildSpecificAccept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.Visit (this);
      
      _mixins.Accept (visitor);
      _requiredFaceTypes.Accept (visitor);
      _requiredBaseCallTypes.Accept (visitor);
      _requiredMixinTypes.Accept (visitor);
    }

    public bool HasMixinWithConfiguredType(Type configuredType)
    {
      Type realType = MixinTypeCloser.GetClosedMixinType (configuredType);
      return _mixins.ContainsKey (realType);
    }

    public MixinDefinition GetMixinByConfiguredType(Type configuredType)
    {
      Type realType = MixinTypeCloser.GetClosedMixinType (configuredType);
      return _mixins[realType];
    }
  }
}
