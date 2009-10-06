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
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Threading;
using Castle.DynamicProxy;
using Remotion.Mixins.Definitions;
using Remotion.Reflection;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;
using Castle.DynamicProxy.Generators.Emitters;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  // This class is not safe for multi-threaded usage. When using it, ensure that its methods and properties are not used from multiple
  // threads at the same time.
  public class ModuleManager : IModuleManager
  {
    public const string DefaultWeakModulePath = "Remotion.Mixins.Generated.Unsigned.{counter}.dll";
    public const string DefaultStrongModulePath = "Remotion.Mixins.Generated.Signed.{counter}.dll";

    private static readonly HashSet<Assembly> s_createdAssemblies = new HashSet<Assembly> ();
    private static readonly object s_lockObject = new object();

    public static HashSet<Assembly> CreatedAssemblies
    {
      get { return s_createdAssemblies; }
    }

    private static void EnsureAssemblyRegistered (Assembly assembly)
    {
      lock (s_lockObject)
      {
        s_createdAssemblies.Add (assembly);
      }
    }

    private static int s_counter = 0; // we count the instances of this class so that we can generate unique assembly names

    private readonly int _currentCount;

    private string _weakAssemblyName;
    private string _weakModulePath;

    private string _strongAssemblyName;
    private string _strongModulePath;

    private ModuleScope _scope;

    public ModuleManager ()
    {
      _currentCount = Interlocked.Increment (ref s_counter);
      _strongModulePath = FormatModuleOrAssemblyName (DefaultStrongModulePath);
      _strongAssemblyName = Path.GetFileNameWithoutExtension (_strongModulePath);
      _weakModulePath = FormatModuleOrAssemblyName (DefaultWeakModulePath);
      _weakAssemblyName = Path.GetFileNameWithoutExtension (_weakModulePath);
    }

    private string FormatModuleOrAssemblyName (string formatSring)
    {
      return formatSring.Replace ("{counter}", _currentCount.ToString ());
    }

    public ITypeGenerator CreateTypeGenerator (CodeGenerationCache cache, TargetClassDefinition configuration, INameProvider nameProvider, INameProvider mixinNameProvider)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      ArgumentUtility.CheckNotNull ("nameProvider", nameProvider);
      ArgumentUtility.CheckNotNull ("mixinNameProvider", mixinNameProvider);

      return new TypeGenerator (cache, this, configuration, nameProvider, mixinNameProvider);
    }

    public IMixinTypeGenerator CreateMixinTypeGenerator (MixinDefinition mixinDefinition, INameProvider mixinNameProvider)
    {
      ArgumentUtility.CheckNotNull ("mixinDefinition", mixinDefinition);
      ArgumentUtility.CheckNotNull ("mixinNameProvider", mixinNameProvider);

      return new MixinTypeGenerator (this, mixinDefinition, mixinNameProvider);
    }

    // should be called when a type was generated with the scope from this module
    public void OnTypeGenerated (Type generatedType, TypeBuilder typeBuilder)
    {
      ArgumentUtility.CheckNotNull ("generatedType", generatedType);
      ArgumentUtility.CheckNotNull ("typeBuilder", typeBuilder);

      EnsureAttributes (typeBuilder.Module);
      EnsureAssemblyRegistered (generatedType.Assembly);
    }

    private void EnsureAttributes (Module module)
    {
      if (module != Scope.StrongNamedModule && module != Scope.WeakNamedModule)
        throw new ArgumentException ("The module specified is not from this ModuleManager's Scope.");

      var assemblyBuilder = (AssemblyBuilder) module.Assembly;
      if (!assemblyBuilder.IsDefined (typeof (NonApplicationAssemblyAttribute), false))
      {
        assemblyBuilder.SetCustomAttribute (new CustomAttributeBuilder (typeof (NonApplicationAssemblyAttribute).GetConstructor(Type.EmptyTypes),
            new object[0]));
      }
    }

    public string SignedAssemblyName
    {
      get { return _strongAssemblyName;  }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        if (HasSignedAssembly)
          throw new InvalidOperationException ("The name can only be set before the first type is built.");
        _strongAssemblyName = FormatModuleOrAssemblyName (value);
      }
    }

    public string UnsignedAssemblyName
    {
      get { return _weakAssemblyName; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        if (HasUnsignedAssembly)
          throw new InvalidOperationException ("The name can only be set before the first type is built.");
        _weakAssemblyName = FormatModuleOrAssemblyName (value);
      }
    }

    public string SignedModulePath
    {
      get { return _strongModulePath; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        if (HasSignedAssembly)
          throw new InvalidOperationException ("The module path can only be set before the first type is built.");
        _strongModulePath = FormatModuleOrAssemblyName (value);
      }
    }

    public string UnsignedModulePath
    {
      get { return _weakModulePath; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        if (HasUnsignedAssembly)
          throw new InvalidOperationException ("The module path can only be set before the first type is built.");
        _weakModulePath = FormatModuleOrAssemblyName (value);
      }
    }

    public ModuleBuilder SignedModule
    {
      get { return Scope.StrongNamedModule; }
    }

    public ModuleBuilder UnsignedModule
    {
      get { return Scope.WeakNamedModule; }
    }

    public ModuleScope Scope
    {
      get
      {
        if (_scope == null)
          _scope = new ModuleScope (true, _strongAssemblyName, _strongModulePath, _weakAssemblyName, _weakModulePath);
        return _scope;
      }
      set
      {
        _scope = value;
      }
    }

    public bool HasSignedAssembly
    {
      get { return _scope != null && _scope.StrongNamedModule != null; }
    }

    public bool HasUnsignedAssembly
    {
      get { return _scope != null && _scope.WeakNamedModule != null; }
    }

    public bool HasAssemblies
    {
      get { return HasSignedAssembly || HasUnsignedAssembly; }
    }

    public string[] SaveAssemblies ()
    {
      if (!HasSignedAssembly && !HasUnsignedAssembly)
        throw new InvalidOperationException ("No types have been built, so no assembly has been generated.");
      else
      {
        if (HasSignedAssembly)
          EnsureAttributes (Scope.StrongNamedModule);
        if (HasUnsignedAssembly)
          EnsureAttributes (Scope.WeakNamedModule);

        return AssemblySaver.SaveAssemblies (_scope);
      }
    }

    public void InitializeMixinTarget (IMixinTarget target)
    {
      ArgumentUtility.CheckNotNull ("target", target);
      ((IInitializableMixinTarget) target).Initialize ();
    }

    public void InitializeDeserializedMixinTarget (IMixinTarget instance, object[] mixinInstances)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      ArgumentUtility.CheckNotNull ("mixinInstances", mixinInstances);

      ((IInitializableMixinTarget) instance).InitializeAfterDeserialization (mixinInstances);
    }

    public IObjectReference BeginDeserialization (Func<Type, Type> typeTransformer, SerializationInfo info, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("typeTransformer", typeTransformer);
      ArgumentUtility.CheckNotNull ("info", info);

      return new SerializationHelper (info, context, typeTransformer);
    }

    public void FinishDeserialization (IObjectReference objectReference)
    {
      ArgumentUtility.CheckNotNull ("objectReference", objectReference);
      ArgumentUtility.CheckTypeIsAssignableFrom ("objectReference", objectReference.GetType (), typeof (IDeserializationCallback));

      ((IDeserializationCallback) objectReference).OnDeserialization (this);
    }

    public IClassEmitter CreateClassEmitter (string typeName, Type baseType, Type[] interfaces, TypeAttributes typeAttributes, bool forceUnsigned)
    {
      var dynamicProxyEmitter = new ClassEmitter (Scope, typeName, baseType, interfaces, typeAttributes, forceUnsigned);
      return new CustomClassEmitter (dynamicProxyEmitter);
    }
  }
}
