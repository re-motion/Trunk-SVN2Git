// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Mixins.Definitions;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.TypePipe.TypeAssembly;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370
  public class MixinTypeGeneratorFacade : IMixinTypeProvider
  {
    public IMixinInfo GetMixinInfo (IProxyTypeAssemblyContext context, MixinDefinition mixin)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("mixin", mixin);

      if (!mixin.NeedsDerivedMixinType())
        return new RegularMixinInfo (mixin.Type);

      var concreteMixinTypeIdentifier = mixin.GetConcreteMixinTypeIdentifier();
      var concreteMixinType = GetOrGenerateConcreteMixinType (context, concreteMixinTypeIdentifier);

      return new DerivedMixinInfo (concreteMixinType);
    }

    public void AddLoadedConcreteMixinType (IDictionary<string, object> participantState, ConcreteMixinType concreteMixinType)
    {
      ArgumentUtility.CheckNotNull ("participantState", participantState);
      ArgumentUtility.CheckNotNull ("concreteMixinType", concreteMixinType);

      var concreteMixinTypeCache = GetOrCreateConcreteMixinTypeCache (participantState);

      // TODO 5370 Review
      // What if already present? (should we ignore it or raise an exception?)
      if (!concreteMixinTypeCache.ContainsKey (concreteMixinType.Identifier))
        concreteMixinTypeCache.Add (concreteMixinType.Identifier, concreteMixinType);
    }

    public ConcreteMixinType GetOrGenerateConcreteMixinType (ITypeAssemblyContext context, ConcreteMixinTypeIdentifier concreteMixinTypeIdentifier)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("concreteMixinTypeIdentifier", concreteMixinTypeIdentifier);

      var concreteMixinTypeCache = GetOrCreateConcreteMixinTypeCache (context.State);

      ConcreteMixinType concreteMixinType;
      if (!concreteMixinTypeCache.TryGetValue (concreteMixinTypeIdentifier, out concreteMixinType))
      {
        concreteMixinType = GenerateConcreteMixinType (context, concreteMixinTypeIdentifier);

        context.GenerationCompleted += generatedTypeContext =>
        {
          var completedConcreteMixinType = concreteMixinType.SubstituteMutableReflectionObjects (generatedTypeContext);
          concreteMixinTypeCache.Add (concreteMixinTypeIdentifier, completedConcreteMixinType);
        };
      }

      return concreteMixinType;
    }

    private ConcreteMixinType GenerateConcreteMixinType (ITypeAssemblyContext context, ConcreteMixinTypeIdentifier concreteMixinTypeIdentifier)
    {
      var mixinProxyType = context.CreateProxy (concreteMixinTypeIdentifier.MixinType);

      // TODO 5370: Should get this identifier from the context.
      var pipelineIdentifier = SafeServiceLocator.Current.GetInstance<IPipelineRegistry>().DefaultPipeline.ParticipantConfigurationID;
      var generator = new MixinTypeGenerator(concreteMixinTypeIdentifier, mixinProxyType, new AttributeGenerator(), pipelineIdentifier);
      generator.AddInterfaces();
      generator.AddFields();
      generator.AddTypeInitializer();
      generator.ImplementGetObjectData();

      generator.AddMixinTypeAttribute();
      generator.AddDebuggerAttributes();

      var overrideInterface = generator.GenerateOverrides ();
      var methodWrappers = generator.GenerateMethodWrappers();

      return new ConcreteMixinType (
          concreteMixinTypeIdentifier, mixinProxyType, overrideInterface.Type, overrideInterface.InterfaceMethodsByOverriddenMethods, methodWrappers);
    }

    private IDictionary<ConcreteMixinTypeIdentifier, ConcreteMixinType> GetOrCreateConcreteMixinTypeCache (
        IDictionary<string, object> participantState)
    {
      const string key = "ConcreteMixinTypes";
      var concreteMixinTypeCache = (Dictionary<ConcreteMixinTypeIdentifier, ConcreteMixinType>) participantState.GetValueOrDefault (key);
      if (concreteMixinTypeCache == null)
      {
        concreteMixinTypeCache = new Dictionary<ConcreteMixinTypeIdentifier, ConcreteMixinType>();
        participantState.Add (key, concreteMixinTypeCache);
      }

      return concreteMixinTypeCache;
    }
  }
}