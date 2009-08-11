// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Logging;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;
using System.Reflection;
using System.Linq;

namespace Remotion.Mixins.CodeGeneration
{
  /// <summary>
  /// Implements caching of the types generated by <see cref="IModuleManager"/> instances, triggered by <see cref="ConcreteTypeBuilder"/>.
  /// </summary>
  public class CodeGenerationCache
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (CodeGenerationCache));
    
    private readonly object _lockObject = new object();
    private readonly ConcreteTypeBuilder _concreteTypeBuilder;
    private readonly Cache<TargetClassDefinition, Type> _typeCache = new Cache<TargetClassDefinition, Type> ();
    private readonly Cache<ConcreteMixinTypeIdentifier, ConcreteMixinType> _mixinTypeCache = 
        new Cache<ConcreteMixinTypeIdentifier, ConcreteMixinType> ();

    public CodeGenerationCache (ConcreteTypeBuilder concreteTypeBuilder)
    {
      ArgumentUtility.CheckNotNull ("concreteTypeBuilder", concreteTypeBuilder);
      _concreteTypeBuilder = concreteTypeBuilder;
    }

    public Type GetOrCreateConcreteType (IModuleManager moduleManager, TargetClassDefinition targetClassDefinition, INameProvider nameProvider, INameProvider mixinNameProvider)
    {
      ArgumentUtility.CheckNotNull ("moduleManager", moduleManager);
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);
      ArgumentUtility.CheckNotNull ("nameProvider", nameProvider);
      ArgumentUtility.CheckNotNull ("mixinNameProvider", mixinNameProvider);

      lock (_lockObject)
      {
        return _typeCache.GetOrCreateValue (
            targetClassDefinition,
            key => GenerateConcreteType (moduleManager, key, nameProvider, mixinNameProvider));
      }
    }

    private Type GenerateConcreteType (IModuleManager moduleManager, TargetClassDefinition targetClassDefinition, INameProvider nameProvider, INameProvider mixinNameProvider)
    {
      s_log.InfoFormat ("Generating type for {0}.", targetClassDefinition.ConfigurationContext);
      using (new CodeGenerationTimer ())
      {
        ITypeGenerator generator = moduleManager.CreateTypeGenerator (
            this,
            targetClassDefinition,
            nameProvider,
            mixinNameProvider);

        return generator.GetBuiltType();
      }
    }

    public ConcreteMixinType GetOrCreateConcreteMixinType (
        ITypeGenerator mixedTypeGenerator, 
        MixinDefinition mixinDefinition, 
        INameProvider mixinNameProvider)
    {
      ArgumentUtility.CheckNotNull ("mixedTypeGenerator", mixedTypeGenerator);
      ArgumentUtility.CheckNotNull ("mixinDefinition", mixinDefinition);
      ArgumentUtility.CheckNotNull ("mixinNameProvider", mixinNameProvider);

      lock (_lockObject)
      {
        return _mixinTypeCache.GetOrCreateValue (
              mixinDefinition.GetConcreteMixinTypeIdentifier (),
              key => GenerateConcreteMixinType (mixedTypeGenerator, mixinDefinition, mixinNameProvider));
      }
    }

    private ConcreteMixinType GenerateConcreteMixinType(ITypeGenerator mixedTypeGenerator, MixinDefinition mixinDefinition, INameProvider mixinNameProvider)
    {
      return _concreteTypeBuilder.Scope.CreateMixinTypeGenerator (mixedTypeGenerator, mixinDefinition, mixinNameProvider).GetBuiltType ();
    }

    public ConcreteMixinType GetConcreteMixinTypeFromCacheOnly (ConcreteMixinTypeIdentifier concreteMixinTypeIdentifier)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinTypeIdentifier", concreteMixinTypeIdentifier);
      lock (_lockObject)
      {
        ConcreteMixinType type;
        _mixinTypeCache.TryGetValue (concreteMixinTypeIdentifier, out type);
        return type;
      }
    }

    public void ImportTypes (IEnumerable<Type> types, IConcreteTypeMetadataImporter metadataImporter)
    {
      ArgumentUtility.CheckNotNull ("types", types);
      lock (_lockObject)
      {
        foreach (Type type in types)
        {
          ImportConcreteMixedType(metadataImporter, type);
          ImportConcreteMixinType(metadataImporter, type);
        }
      }
    }

    private void ImportConcreteMixedType(IConcreteTypeMetadataImporter metadataImporter, Type type)
    {
      foreach (TargetClassDefinition mixedTypeMetadata in metadataImporter.GetMetadataForMixedType (type, TargetClassDefinitionCache.Current))
        _typeCache.GetOrCreateValue (mixedTypeMetadata, delegate { return type; });
    }


    private void ImportConcreteMixinType (IConcreteTypeMetadataImporter metadataImporter, Type type)
    {
      var mixinDefinitions = metadataImporter.GetMetadataForMixinType (type, TargetClassDefinitionCache.Current).ToArray ();
      if (mixinDefinitions.Length > 0)
      {
        var methodWrappers = metadataImporter.GetMethodWrappersForMixinType (type);
        foreach (MixinDefinition mixinDefinition in mixinDefinitions)
        {
          var concreteMixinType = new ConcreteMixinType (type);
          foreach (Tuple<MethodInfo, MethodInfo> wrapper in methodWrappers)
            concreteMixinType.AddMethodWrapper (wrapper.A, wrapper.B);

          _mixinTypeCache.GetOrCreateValue (mixinDefinition.GetConcreteMixinTypeIdentifier (), delegate { return concreteMixinType; });
        }
      }
    }
  }
}
