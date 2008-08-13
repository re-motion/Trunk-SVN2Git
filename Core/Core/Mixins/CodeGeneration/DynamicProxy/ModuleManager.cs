/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Threading;
using Castle.DynamicProxy;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Reflection;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  // This class is not safe for multi-threaded usage. When using it, ensure that its methods and properties are not used from multiple
  // threads at the same time.
  public class ModuleManager : IModuleManager
  {
    public const string DefaultWeakModulePath = "Remotion.Mixins.Generated.Unsigned.{counter}.dll";
    public const string DefaultStrongModulePath = "Remotion.Mixins.Generated.Signed.{counter}.dll";

    private static int s_counter = 0; // we count the instances of this class so that we can generate unique assembly names

    private string _weakAssemblyName;
    private string _weakModulePath;

    private string _strongAssemblyName;
    private string _strongModulePath;

    private ModuleScope _scope;

    public ModuleManager ()
    {
      int currentCount = Interlocked.Increment (ref s_counter);
      _strongModulePath = DefaultStrongModulePath.Replace ("{counter}", currentCount.ToString());
      _strongAssemblyName = Path.GetFileNameWithoutExtension (_strongModulePath);
      _weakModulePath = DefaultWeakModulePath.Replace ("{counter}", currentCount.ToString ());
      _weakAssemblyName = Path.GetFileNameWithoutExtension (_weakModulePath);
    }

    public ITypeGenerator CreateTypeGenerator (TargetClassDefinition configuration, INameProvider nameProvider, INameProvider mixinNameProvider)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      ArgumentUtility.CheckNotNull ("nameProvider", nameProvider);
      ArgumentUtility.CheckNotNull ("mixinNameProvider", mixinNameProvider);

      return new TypeGenerator (this, configuration, nameProvider, mixinNameProvider);
    }

    // should be called when a type was generated with the scope from this module
    public void OnTypeGenerated (Type generatedType, TypeBuilder typeBuilder)
    {
      ArgumentUtility.CheckNotNull ("generatedType", generatedType);
      ArgumentUtility.CheckNotNull ("typeBuilder", typeBuilder);

      EnsureAttributes (typeBuilder.Module);
    }

    private void EnsureAttributes (Module module)
    {
      if (module != Scope.StrongNamedModule && module != Scope.WeakNamedModule)
        throw new ArgumentException ("The module specified is not from this ModuleManager's Scope.");

      if (!module.Assembly.IsDefined (typeof (NonApplicationAssemblyAttribute), false))
      {
        AssemblyBuilder assemblyBuilder = (AssemblyBuilder) module.Assembly;
        assemblyBuilder.SetCustomAttribute (new CustomAttributeBuilder (typeof (NonApplicationAssemblyAttribute).GetConstructor(Type.EmptyTypes),
            new object[0]));
      }
    }

    public string SignedAssemblyName
    {
      get { return _strongAssemblyName;  }
      set
      {
        if (HasSignedAssembly)
          throw new InvalidOperationException ("The name can only be set before the first type is built.");
        _strongAssemblyName = value;
      }
    }

    public string UnsignedAssemblyName
    {
      get { return _weakAssemblyName; }
      set
      {
        if (HasUnsignedAssembly)
          throw new InvalidOperationException ("The name can only be set before the first type is built.");
        _weakAssemblyName = value;
      }
    }

    public string SignedModulePath
    {
      get { return _strongModulePath; }
      set
      {
        if (HasSignedAssembly)
          throw new InvalidOperationException ("The module path can only be set before the first type is built.");
        _strongModulePath = value;
      }
    }

    public string UnsignedModulePath
    {
      get { return _weakModulePath; }
      set
      {
        if (HasUnsignedAssembly)
          throw new InvalidOperationException ("The module path can only be set before the first type is built.");
        _weakModulePath = value;
      }
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

      GeneratedClassInstanceInitializer.InitializeMixinTarget ((IInitializableMixinTarget) target, false);
    }

    public void InitializeDeserializedMixinTarget (IMixinTarget instance, object[] mixinInstances)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      ArgumentUtility.CheckNotNull ("mixinInstances", mixinInstances);

      using (new MixedObjectInstantiationScope (mixinInstances))
      {
        GeneratedClassInstanceInitializer.InitializeMixinTarget ((IInitializableMixinTarget) instance, true);
      }
    }

    public IObjectReference BeginDeserialization (Func<Type, Type> typeTransformer, SerializationInfo info, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("typeTransformer", typeTransformer);
      ArgumentUtility.CheckNotNull ("info", info);

      return new SerializationHelper (typeTransformer, info, context);
    }

    public void FinishDeserialization (IObjectReference objectReference)
    {
      ArgumentUtility.CheckNotNull ("objectReference", objectReference);
      ArgumentUtility.CheckTypeIsAssignableFrom ("objectReference", objectReference.GetType (), typeof (IDeserializationCallback));

      ((IDeserializationCallback) objectReference).OnDeserialization (this);
    }
  }
}
