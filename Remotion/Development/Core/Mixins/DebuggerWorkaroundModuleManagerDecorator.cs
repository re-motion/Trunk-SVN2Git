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
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Remotion.Diagnostics;
using Remotion.Logging;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Definitions;
using Remotion.Reflection.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.Development.Mixins
{
  /// <summary>
  /// Decorates an <see cref="IModuleManager"/>, counting the generated types and resetting the inner <see cref="IModuleManager"/> when the number 
  /// of types exceeds the given threshold. This can be used as a workaround for the Reflection.Emit bug where calls to 
  /// <see cref="TypeBuilder.CreateType"/> take a very long time to complete  when the debugger is attached and a large number of types is generated 
  /// into the same AssemblyBuilder.
  /// </summary>
  public class DebuggerWorkaroundModuleManagerDecorator : IModuleManager
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (DebuggerWorkaroundModuleManagerDecorator));
    private static readonly TimeSpan s_warningThreshold = TimeSpan.FromSeconds (0.5);

    private readonly int _maximumTypesPerAssembly;
    private readonly IModuleManager _innerModuleManager;
    private readonly IDebuggerInterface _debuggerInterface;

    private int _generatedTypeCountForCurrentScope;
    private int _resetCount;

    public DebuggerWorkaroundModuleManagerDecorator (int maximumTypesPerAssembly, IDebuggerInterface debuggerInterface, IModuleManager innerModuleManager)
    {
      ArgumentUtility.CheckNotNull ("innerModuleManager", innerModuleManager);
      ArgumentUtility.CheckNotNull ("debuggerInterface", debuggerInterface);

      _maximumTypesPerAssembly = maximumTypesPerAssembly;
      _innerModuleManager = innerModuleManager;
      _debuggerInterface = debuggerInterface;
    }

    public int MaximumTypesPerAssembly
    {
      get { return _maximumTypesPerAssembly; }
    }

    public int GeneratedTypeCountForCurrentScope
    {
      get { return _generatedTypeCountForCurrentScope; }
    }

    public int ResetCount
    {
      get { return _resetCount; }
    }

    public IDebuggerInterface DebuggerInterface
    {
      get { return _debuggerInterface; }
    }

    public ITypeGenerator CreateTypeGenerator (
        TargetClassDefinition configuration, IConcreteMixedTypeNameProvider nameProvider, IConcreteMixinTypeProvider concreteMixinTypeProvider)
    {
      CheckTypeThreshold();

      var innerTypeGenerator = _innerModuleManager.CreateTypeGenerator (configuration, nameProvider, concreteMixinTypeProvider);
      return new DebuggerWorkaroundTypeGeneratorDecorator (innerTypeGenerator, this);
    }

    public IMixinTypeGenerator CreateMixinTypeGenerator (
        ConcreteMixinTypeIdentifier concreteMixinTypeIdentifier, IConcreteMixinTypeNameProvider mixinNameProvider)
    {
      CheckTypeThreshold();

      var innerMixinTypeGenerator = _innerModuleManager.CreateMixinTypeGenerator (concreteMixinTypeIdentifier, mixinNameProvider);
      return new DebuggerWorkaroundMixinTypeGeneratorDecorator (innerMixinTypeGenerator, this);
    }

    public void Reset ()
    {
      ++_resetCount;
      s_log.InfoFormat ("Scope is reset. (Count: {0})", _resetCount);
      _innerModuleManager.Reset ();
      _generatedTypeCountForCurrentScope = 0;
    }

    public void TypeGenerated (TimeSpan elapsedTotal)
    {
      ++_generatedTypeCountForCurrentScope;

      s_log.DebugFormat ("A type ({0}) was generated in {1} milliseconds.", _generatedTypeCountForCurrentScope, elapsedTotal.TotalMilliseconds);
      
      if (elapsedTotal > s_warningThreshold)
        s_log.WarnFormat (
            "Type generation needed {0} milliseconds - the threshold ({1}) is probably too low. Current number of types since last reset: {2}.",
            elapsedTotal.TotalMilliseconds,
            _maximumTypesPerAssembly,
            _generatedTypeCountForCurrentScope);
    }

    private void CheckTypeThreshold ()
    {
      if (_debuggerInterface.IsAttached && _generatedTypeCountForCurrentScope >= _maximumTypesPerAssembly)
      {
        s_log.InfoFormat ("Type threshold was exceeded (Types: {0}).", _generatedTypeCountForCurrentScope);
        Reset ();
      }
    }

    void ICodeGenerationModule.OnTypeGenerated (Type generatedType, TypeBuilder typeBuilder)
    {
      _innerModuleManager.OnTypeGenerated (generatedType, typeBuilder);
    }

    string[] IModuleManager.SaveAssemblies ()
    {
      return _innerModuleManager.SaveAssemblies();
    }

    void IModuleManager.InitializeMixinTarget (IMixinTarget target)
    {
      _innerModuleManager.InitializeMixinTarget (target);
    }

    void IModuleManager.InitializeDeserializedMixinTarget (IMixinTarget instance, object[] mixinInstances)
    {
      _innerModuleManager.InitializeDeserializedMixinTarget (instance, mixinInstances);
    }

    IObjectReference IModuleManager.BeginDeserialization (Func<Type, Type> typeTransformer, SerializationInfo info, StreamingContext context)
    {
      return _innerModuleManager.BeginDeserialization (typeTransformer, info, context);
    }

    void IModuleManager.FinishDeserialization (IObjectReference objectReference)
    {
      _innerModuleManager.FinishDeserialization (objectReference);
    }

    IClassEmitter ICodeGenerationModule.CreateClassEmitter (
        string typeName, Type baseType, Type[] interfaces, TypeAttributes typeAttributes, bool forceUnsigned)
    {
      return _innerModuleManager.CreateClassEmitter (typeName, baseType, interfaces, typeAttributes, forceUnsigned);
    }

    string ICodeGenerationModuleInfo.SignedAssemblyName
    {
      get { return _innerModuleManager.SignedAssemblyName; }
      set { _innerModuleManager.SignedAssemblyName = value; }
    }

    string ICodeGenerationModuleInfo.UnsignedAssemblyName
    {
      get { return _innerModuleManager.UnsignedAssemblyName; }
      set { _innerModuleManager.UnsignedAssemblyName = value; }
    }

    string ICodeGenerationModuleInfo.SignedModulePath
    {
      get { return _innerModuleManager.SignedModulePath; }
      set { _innerModuleManager.SignedModulePath = value; }
    }

    string ICodeGenerationModuleInfo.UnsignedModulePath
    {
      get { return _innerModuleManager.UnsignedModulePath; }
      set { _innerModuleManager.UnsignedModulePath = value; }
    }

    bool ICodeGenerationModuleInfo.HasSignedAssembly
    {
      get { return _innerModuleManager.HasSignedAssembly; }
    }

    bool ICodeGenerationModuleInfo.HasUnsignedAssembly
    {
      get { return _innerModuleManager.HasUnsignedAssembly; }
    }

    bool ICodeGenerationModuleInfo.HasAssemblies
    {
      get { return _innerModuleManager.HasAssemblies; }
    }
  }
}