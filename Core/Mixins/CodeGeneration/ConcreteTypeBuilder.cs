using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Mixins;
using Remotion.Mixins.Utilities.Singleton;
using Remotion.Mixins.Definitions;
using Remotion.Collections;
using Remotion.Utilities;
using Remotion.Mixins.Context;

namespace Remotion.Mixins.CodeGeneration
{
  /// <summary>
  /// Provides a way to build concrete types for target and mixin classes in mixin configurations and maintains a cache for built types.
  /// </summary>
  /// <remarks>
  /// <para>
  /// You can use different instances of <see cref="ConcreteTypeBuilder"/> by setting them as the
  /// <see cref="ThreadSafeSingletonBase{TSelf,TCreator}.Current"/> property via <see cref="ThreadSafeSingletonBase{TSelf,TCreator}.SetCurrent"/>.
  /// Each of these will have its own instance of <see cref="IModuleManager"/> as its <see cref="Scope"/>, and each scope will have its own
  /// dynamic assembly for code generation. However, each scope's assembly will have the same default name. 
  /// </para>
  /// <para>
  /// Having different assemblies with the same names loaded into one AppDomain can lead to sporadic
  /// <see cref="TypeLoadException">TypeLoadExceptions</see> in reflective scenarios. To avoid running into such errors, set the 
  /// <see cref="IModuleManager.SignedAssemblyName"/> and <see cref="IModuleManager.UnsignedAssemblyName"/> properties of the
  /// <see cref="Scope"/> to unique names for each <see cref="ConcreteTypeBuilder"/> instance you use.
  /// </para>
  /// </remarks>
  public class ConcreteTypeBuilder : ThreadSafeSingletonBase<ConcreteTypeBuilder, DefaultInstanceCreator<ConcreteTypeBuilder>>
  {
    private IModuleManager _scope;
    // Use a pessimistic cache - a ModuleBuilder cannot be used by multiple threads at the same time anyway, so using a lazy cache could actually 
    // cause errors (depending on how it was implemented)
    private readonly InterlockedCache<ClassDefinitionBase, Type> _typeCache = new InterlockedCache<ClassDefinitionBase, Type>();

    private readonly object _scopeLockObject = new object ();
    private INameProvider _typeNameProvider = GuidNameProvider.Instance;
    private INameProvider _mixinTypeNameProvider = GuidNameProvider.Instance;

    /// <summary>
    /// Gets or sets the module scope of this <see cref="ConcreteTypeBuilder"/>. The object returned by this property must not be used by multiple
    /// threads at the same time (or while another thread executes methods on <see cref="ConcreteTypeBuilder"/>). Use the
    /// <see cref="LockAndAccessScope"/> method to access the scope in a thread-safe way.
    /// </summary>
    /// <value>The module scope of this <see cref="ConcreteTypeBuilder"/>.</value>
    public IModuleManager Scope
    {
      get
      {
        lock (_scopeLockObject)
        {
          if (_scope == null)
            _scope = new DynamicProxy.ModuleManager();
          return _scope;
        }
      }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        lock (_scopeLockObject)
        {
          _scope = value;
        }
      }
    }

    /// <summary>
    /// Provides thread-safe access to the module scope of <see cref="ConcreteTypeBuilder"/>, see also <see cref="Scope"/>.
    /// </summary>
    /// <param name="scopeAccessor">A delegate accessing the scope while access to it is locked.</param>
    /// <remarks>This methods locks the scope while executing <paramref name="scopeAccessor"/>. This ensures that no other method of
    /// <see cref="ConcreteTypeBuilder"/> modifies the scope while it is being accessed.</remarks>
    public void LockAndAccessScope (Proc<IModuleManager> scopeAccessor)
    {
      lock (_scopeLockObject)
      {
        scopeAccessor (Scope);
      }
    }

    /// <summary>
    /// Gets or sets the name provider used when generating a concrete mixed type.
    /// </summary>
    /// <value>The type name provider for mixed types.</value>
    public INameProvider TypeNameProvider
    {
      get
      {
        lock (_scopeLockObject)
        {
          return _typeNameProvider;
        }
      }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        lock (_scopeLockObject)
        {
          _typeNameProvider = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets the name provider used when generating a concrete mixin type.
    /// </summary>
    /// <value>The type name provider for mixin types.</value>
    public INameProvider MixinTypeNameProvider
    {
      get
      {
        lock (_scopeLockObject)
        {
          return _mixinTypeNameProvider;
        }
      }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        lock (_scopeLockObject)
        {
          _mixinTypeNameProvider = value;
        }
      }
    }

    /// <summary>
    /// Gets the concrete mixed type for the given target class configuration either from the cache or by generating it.
    /// </summary>
    /// <param name="configuration">The configuration object for the target class.</param>
    /// <returns>A concrete type with all mixins from <paramref name="configuration"/> mixed in.</returns>
    /// <remarks>This is mostly for internal reasons, users should use <see cref="TypeFactory.GetConcreteType(Type)"/> instead.</remarks>
    public Type GetConcreteType (TargetClassDefinition configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      return _typeCache.GetOrCreateValue (
          configuration,
          delegate (ClassDefinitionBase classConfiguration)
          {
            lock (_scopeLockObject)
            {
              ITypeGenerator generator = Scope.CreateTypeGenerator ((TargetClassDefinition) classConfiguration, _typeNameProvider,
                _mixinTypeNameProvider);

              foreach (Tuple<MixinDefinition, Type> finishedMixinTypes in generator.GetBuiltMixinTypes ())
                _typeCache.GetOrCreateValue (finishedMixinTypes.A, delegate { return finishedMixinTypes.B; });

              Type finishedType = generator.GetBuiltType();
              return finishedType;
            }
          });
    }

    /// <summary>
    /// Gets the concrete type for the given mixin class configuration either from the cache or by generating it.
    /// </summary>
    /// <param name="configuration">The configuration object for the mixin class.</param>
    /// <returns>A concrete type for the given mixin <paramref name="configuration"/>.</returns>
    /// <remarks>This is mostly for internal reasons, users will hardly ever need to use this method..</remarks>
    public Type GetConcreteMixinType (MixinDefinition configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      GetConcreteType (configuration.TargetClass); // ensure base type was created
      Type type;
      _typeCache.TryGetValue (configuration, out type);
      if (type == null)
      {
        string message = string.Format ("No concrete mixin type is required for the given configuration (mixin {0} and target class {1}).",
            configuration.FullName, configuration.TargetClass.FullName);
        throw new ArgumentException (message, "configuration");
      }
      else
        return type;
    }

    /// <summary>
    /// Saves the dynamic <see cref="Scope"/> of this builder to disk and resets it, so that the builder can continue to generate types. Use
    /// the <see cref="Scope">Scope's</see> properties via <see cref="LockAndAccessScope"/> to configure the name and paths of the modules being
    /// saved.
    /// </summary>
    /// <returns>An array containing the paths of the assembly files saved.</returns>
    /// <remarks>
    /// <para>
    /// This is similar to directly calling <c>Scope.SaveAssemblies</c>, but in addition resets the <see cref="Scope"/> to a new instance of
    /// <see cref="IModuleManager"/>. That way, the builder can continue to generate types even when the dynamic assemblies have been saved.
    /// Note that each time this method is called, only the types generated since the last save operation are persisted. Also, if the scope isn't
    /// reconfigured to save at different paths, previously saved assemblies might be overwritten.
    /// </para>
    /// <para>
    /// Having different assemblies with the same names loaded into one AppDomain can lead to sporadic
    /// <see cref="TypeLoadException">TypeLoadExceptions</see> in reflective scenarios. To avoid running into such errors, set the 
    /// <see cref="IModuleManager.SignedAssemblyName"/> and <see cref="IModuleManager.UnsignedAssemblyName"/> properties of the
    /// <see cref="Scope"/> to new, unique names after calling this method.
    /// </para>
    /// </remarks>
    public string[] SaveAndResetDynamicScope ()
    {
      lock (_scopeLockObject)
      {
        string[] paths;
        if (Scope.HasSignedAssembly || Scope.HasUnsignedAssembly)
          paths = Scope.SaveAssemblies();
        else
          paths = new string[0];

        _scope = null;
        return paths;
      }
    }

    /// <summary>
    /// Loads an assembly with the given name and adds its mixed types to this builder's cache.
    /// </summary>
    /// <param name="assembly">The assembly whose public types to load into the cache.</param>
    public void LoadAssemblyIntoCache (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      AssemblyName assemblyName = assembly.GetName();
      if (assemblyName.Name == Scope.SignedAssemblyName || assemblyName.Name == Scope.UnsignedAssemblyName)
      {
        string message = string.Format (
            "Cannot load assembly '{0}' into the cache because it has the same name as one of the dynamic assemblies used "
            + "by the mixin engine. Having two assemblies with the same name loaded into one AppDomain can cause strange and sporadic "
            + "TypeLoadExceptions.", assemblyName.Name);
        throw new ArgumentException (message, "assembly");
      }

      ImportTypes(assembly.GetExportedTypes ());
    }

    private void ImportTypes (IEnumerable<Type> types)
    {
      foreach (Type type in types)
      {
        foreach (ConcreteMixedTypeAttribute typeDescriptor in type.GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false))
        {
          TargetClassDefinition targetClassDefinition = typeDescriptor.GetTargetClassDefinition ();
          _typeCache.GetOrCreateValue (targetClassDefinition, delegate { return type; });
        }

        foreach (ConcreteMixinTypeAttribute typeDescriptor in type.GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false))
        {
          MixinDefinition mixinDefinition = typeDescriptor.GetMixinDefinition ();
          _typeCache.GetOrCreateValue (mixinDefinition, delegate { return type; });
        }
      }
    }

    /// <summary>
    /// Initializes a mixin target instance which was created without its constructor having been called.
    /// </summary>
    /// <param name="mixinTarget">The mixin target to initialize.</param>
    /// <exception cref="ArgumentNullException">The mixin target is <see langword="null"/>.</exception>
    /// <remarks>This method is useful when a mixin target instance is created via <see cref="FormatterServices.GetSafeUninitializedObject"/>.</remarks>
    public void InitializeUnconstructedInstance (IMixinTarget mixinTarget)
    {
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);
      Scope.InitializeMixinTarget (mixinTarget);
    }

    /// <summary>
    /// Begins deserialization of a mixed object.
    /// </summary>
    /// <param name="typeTransformer">A transformation object that is given a chance to modify the deserialized type before it is instantiated.</param>
    /// <param name="info">The <see cref="SerializationInfo"/> object provided by the .NET serialization infrastructure.</param>
    /// <param name="context">The <see cref="StreamingContext"/> object provided by the .NET serialization infrastructure.</param>
    /// <returns>An <see cref="IObjectReference"/> object containing a partially deserialized mixed object. Be sure to call
    /// <see cref="FinishDeserialization"/> from an implementation of <see cref="IDeserializationCallback.OnDeserialization"/> to finish the
    /// deserialization process.</returns>
    /// <exception cref="ArgumentNullException">One or more of the parameters passed to this method are <see langword="null"/>.</exception>
    /// <exception cref="SerializationException">The serialization data does not hold the expected values.</exception>
    /// <remarks>
    /// <para>
    /// This method is useful when the mixin engine is combined with other code generation mechanisms. In such a case, the default
    /// <see cref="IObjectReference"/> implementation provided by the mixin code generation can be extended by a custom <see cref="IObjectReference"/>
    /// object by calling this method. This method instantiates the real object to be returned by the deserialization process, but the caller
    /// specifies a <paramref name="typeTransformer"/> delegate that gets the chance to modify the type of object before it is instantiated. The
    /// parameter passed to <paramref name="typeTransformer"/> is the type deducted from the deserialized mixin configuration-
    /// </para>
    /// <para>
    /// This method expects that the deserialized data is from a mixed object, calling it for an unmixed object will yield an exception.
    /// </para>
    /// </remarks>
    public IObjectReference BeginDeserialization (Func<Type, Type> typeTransformer, SerializationInfo info, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("typeTransformer", typeTransformer);
      ArgumentUtility.CheckNotNull ("info", info);

      return Scope.BeginDeserialization (typeTransformer, info, context);
    }

    /// <summary>
    /// Finishes a deserialization process started by <see cref="BeginDeserialization"/>.
    /// </summary>
    /// <param name="objectReference">The object returned from <see cref="BeginDeserialization"/>.</param>
    /// <remarks>
    /// <exception cref="ArgumentNullException">The <paramref name="objectReference"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentTypeException">The <paramref name="objectReference"/> parameter does not hold an object returned by the
    /// <see cref="BeginDeserialization"/> method.</exception>
    /// <para>
    /// Call this method to complete deserialization of a mixed object when the .NET serialization infrastructure has finished its
    /// work, e.g. from an implementation of <see cref="IDeserializationCallback.OnDeserialization"/>. After this method, the real object
    /// contained in <paramref name="objectReference"/> can safely be used.
    /// </para>
    /// <para>
    /// If the given instance is not an instance of a mixed type, this method does nothing.
    /// </para>
    /// </remarks>
    public void FinishDeserialization (IObjectReference objectReference)
    {
      ArgumentUtility.CheckNotNull ("objectReference", objectReference);
      Scope.FinishDeserialization (objectReference);
    }
  }
}
