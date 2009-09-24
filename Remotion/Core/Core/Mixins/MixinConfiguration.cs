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
using System.Linq;
using Remotion.Context;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Definitions.Building;
using Remotion.Mixins.Utilities;
using Remotion.Mixins.Validation;
using Remotion.Utilities;

namespace Remotion.Mixins
{
  /// <summary>
  /// Constitutes a mixin configuration (ie. a set of classes associated with mixins) and manages the mixin configuration for the
  /// current thread.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Instances of this class represent a single mixin configuration, ie. a set of classes associated with mixins. The class manages a thread-local
  /// (actually <see cref="SafeContext"/>-local) single active configuration instance via its <see cref="ActiveConfiguration"/> property and
  /// related methods; the active configuration can conveniently be replaced via the <see cref="EnterScope"/> method. The also provides entry points
  /// for building new mixin configuration objects: <see cref="BuildNew"/>, <see cref="BuildFromActive"/>, and <see cref="BuildFrom"/>.
  /// </para>
  /// <para>
  /// While the <see cref="MixinConfiguration.ActiveConfiguration"/> will usually be accessed only indirectly via <see cref="ObjectFactory"/> or <see cref="TypeFactory"/>,
  /// <see cref="EnterScope"/> and the <see cref="BuildFromActive">BuildFrom...</see> methods can be very useful to adjust a thread's mixin
  /// configuration at runtime.
  /// </para>
  /// <para>
  /// The master mixin configuration - the configuration in effect for a thread if not specifically replaced by another configuration - is obtained
  /// by analyzing the assemblies in the application's bin directory  for attributes such as <see cref="UsesAttribute"/>,
  /// <see cref="ExtendsAttribute"/>, and <see cref="CompleteInterfaceAttribute"/>. (For more information about the default configuration, see
  /// <see cref="DeclarativeConfigurationBuilder.BuildDefaultConfiguration"/>.) The master configuration can also be manipulated via
  /// <see cref="EditMasterConfiguration"/>.
  /// </para>
  /// <example>
  /// The following shows an exemplary application of the <see cref="MixinConfiguration"/> class that manually builds mixin configuration instances
  /// and activates them for the current thread for a given scope.
  /// <code>
  /// class Program
  /// {
  ///   public static void Main()
  ///   {
  ///     // myType1 is an instantiation of MyType with the default mixin configuration
  ///     MyType myType1 = ObjectFactory.Create&lt;MyType&gt; ().With();
  /// 
  ///     using (MixinConfiguration.BuildNew().ForClass&lt;MyType&gt;.AddMixin&lt;SpecialMixin&gt;().EnterScope())
  ///     {
  ///       // myType2 is an instantiation of MyType with a specific configuration, which contains only SpecialMixin
  ///       MyType myType2 = ObjectFactory.Create&lt;MyType&gt; ().With();
  /// 
  ///       using (MixinConfiguration.BuildNew().EnterScope())
  ///       {
  ///         // myType3 is an instantiation of MyType without any mixins
  ///         MyType myType3 = ObjectFactory.Create&lt;MyType&gt; ().With();
  ///       }
  ///     }
  /// 
  ///     // myType4 again is an instantiation of MyType with the default mixin configuration
  ///     MyType myType4 = ObjectFactory.Create&lt;MyType&gt; ().With();
  ///   }
  /// }
  /// </code>
  /// </example>
  /// </remarks>
  /// <threadsafety static="true" instance="false">
  ///    <para>Instances of this class are meant to be used one-per-thread, see <see cref="ActiveConfiguration"/>.</para>
  /// </threadsafety>
  public partial class MixinConfiguration
  {
    private readonly ClassContextCollection _classContexts;
    private readonly Dictionary<Type, ClassContext> _registeredInterfaces = new Dictionary<Type,ClassContext> ();

    /// <summary>
    /// Initializes a new empty mixin configuarion that does not inherit anything from another configuration.
    /// </summary>
    public MixinConfiguration ()
        : this ((MixinConfiguration) null)
    {
    }

    /// <summary>
    /// Initializes a new configuration that inherits from another configuration.
    /// </summary>
    /// <param name="parentConfiguration">The parent configuration. The new configuration will inherit all class contexts from its parent configuration. Can be
    /// <see langword="null"/>.</param>
    public MixinConfiguration (MixinConfiguration parentConfiguration)
    {
      _classContexts = new ClassContextCollection();
      _classContexts.ClassContextAdded += ClassContextAdded;
      _classContexts.ClassContextRemoved += ClassContextRemoved;
 
      if (parentConfiguration != null)
        parentConfiguration.CopyTo (this);
    }

    /// <summary>
    /// Initializes a new non-empty configuration.
    /// </summary>
    /// <param name="classContexts">The class contexts to be held by this <see cref="MixinConfiguration"/>.</param>
    public MixinConfiguration (ClassContextCollection classContexts)
    {
      _classContexts = classContexts;
      _classContexts.ClassContextAdded += ClassContextAdded;
      _classContexts.ClassContextRemoved += ClassContextRemoved;

      foreach (var classContext in classContexts)
        ClassContextAdded (this, new ClassContextEventArgs (classContext)); // register interfaces
    }

    /// <summary>
    /// Gets the class contexts currently stored in this <see cref="MixinConfiguration"/>. Only contexts that have been explicitly added for classes
    /// are returned.
    /// </summary>
    /// <value>The class contexts currently sotred in this configuration.</value>
    /// <remarks>
    /// <para>
    /// Note that the collection returned cannot be used to enumerate all mixed classes, only
    /// those which are explicitly configured for mixins. If, for example, a base class is configured to have a mixin, its subclasses will not be
    /// enumerated by the collection even though they inherit the mixin from the base class.
    /// </para>
    /// <para>
    /// Use <see cref="GetContext(System.Type)"/> to retrieve a <see cref="ClassContext"/> for a specific type.
    /// </para>
    /// </remarks>
    public ClassContextCollection ClassContexts
    {
      get { return _classContexts; }
    }

    /// <summary>
    /// Returns a <see cref="ClassContext"/> for the given target type, or <see langword="null" /> if the type is not configured in this 
    /// <see cref="MixinConfiguration"/>.
    /// </summary>
    /// <param name="targetOrConcreteType">Base type for which a context should be returned or a concrete mixed type.</param>
    /// <returns>A <see cref="ClassContext"/> for the a given target type, or <see langword="null"/> if the type is not configured.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetOrConcreteType"/> parameter is <see langword="null"/>.</exception>
    /// <remarks>
    /// <para>
    /// Use this to extract a class context for a given target type from an <see cref="MixinConfiguration"/> as it would be used for mixed type code 
    /// generation. Besides looking up the target type in the <see cref="ClassContexts"/> collection, this method also checks whether the class
    /// context is empty, and returns <see langword="null" /> if so.
    /// </para>
    /// <para>
    /// Use the <see cref="GetContextForce(System.Type)"/> method to reveive an empty but valid <see cref="ClassContext"/> for types that do not have 
    /// a mixin configuration instead of <see langword="null" />.
    /// </para>
    /// <para>
    /// If <paramref name="targetOrConcreteType"/> is already a generated type, the <see cref="ClassContext"/> used for its generation is returned.
    /// </para>
    /// </remarks>
    public ClassContext GetContext (Type targetOrConcreteType)
    {
      ArgumentUtility.CheckNotNull ("targetOrConcreteType", targetOrConcreteType);

      ClassContext context = 
          MixinTypeUtility.IsGeneratedConcreteMixedType (targetOrConcreteType) 
              ? MixinReflector.GetClassContextFromConcreteType (targetOrConcreteType) 
              : ClassContexts.GetWithInheritance (targetOrConcreteType);

      if (context == null || context.IsEmpty())
        return null;
      else
        return context;
    }

    /// <summary>
    /// Returns a <see cref="ClassContext"/> for the given target type, generating a trivial default <see cref="ClassContext"/> for types that are
    /// not configured in this <see cref="MixinConfiguration"/>.
    /// </summary>
    /// <param name="targetOrConcreteType">Base type for which a context should be returned or a concrete mixed type.</param>
    /// <returns>A <see cref="ClassContext"/> for the a given target type.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetOrConcreteType"/> parameter is <see langword="null"/>.</exception>
    /// <remarks>
    /// <para>
    /// Use this to extract a class context for a given target type from an <see cref="MixinConfiguration"/> as it would be used to create the
    /// <see cref="TargetClassDefinition"/> object for the target type. Besides looking up the target type in the given mixin configuration, this
    /// includes generating a default context if necessary.
    /// </para>
    /// <para>
    /// Use the <see cref="GetContext(System.Type)"/> method to avoid generating an empty default <see cref="ClassContext"/> for non-configured
    /// instances.
    /// </para>
    /// <para>
    /// If <paramref name="targetOrConcreteType"/> is already a generated type, a new <see cref="ClassContext"/> is nevertheless generated.
    /// </para>
    /// </remarks>
    public ClassContext GetContextForce (Type targetOrConcreteType)
    {
      ArgumentUtility.CheckNotNull ("targetOrConcreteType", targetOrConcreteType);

      ClassContext context = ClassContexts.GetWithInheritance (targetOrConcreteType);

      if (context == null)
        return new ClassContext (targetOrConcreteType);
      else
        return context;
    }

    /// <summary>
    /// Temporarily replaces the mixin configuration associated with the current thread (actually <see cref="SafeContext"/>) with this 
    /// <see cref="MixinConfiguration"/>. The original configuration will be restored when the returned object's <see cref="IDisposable.Dispose"/> 
    /// method is called.
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> object for restoring the original configuration.</returns>
    public IDisposable EnterScope ()
    {
      var scope = new MixinConfigurationScope (PeekActiveConfiguration);
      SetActiveConfiguration (this);
      return scope;
    }

    /// <summary>
    /// Validates the whole configuration.
    /// </summary>
    /// <returns>An <see cref="IValidationLog"/>, which contains information about whether the configuration reresented by this context is valid.</returns>
    /// <remarks>This method retrieves definition items for all the <see cref="ClassContexts"/> known by this configuration and uses the
    /// <see cref="Validator"/> class to validate them. The validation results can be inspected, passed to a <see cref="ValidationException"/>, or
    /// be dumped using the <see cref="ConsoleDumper"/>.</remarks>
    /// <exception cref="NotSupportedException">The <see cref="MixinConfiguration"/> contains a <see cref="ClassContext"/> for a generic type, of
    /// which it cannot make a closed generic type. Because closed types are needed for validation, this <see cref="MixinConfiguration"/>
    /// cannot be validated as a whole. Even in this case, the configuration might still be correct, but validation is deferred to
    /// <see cref="TargetClassDefinitionFactory.CreateTargetClassDefinition(ClassContext)"/>.</exception>
    public IValidationLog Validate()
    {
      var builder = new TargetClassDefinitionBuilder ();

      var definitions = from classContext in ActiveConfiguration.ClassContexts
                        where !classContext.Type.IsGenericTypeDefinition && !classContext.Type.IsInterface
                        select (IVisitableDefinition) builder.Build (classContext);

      return Validator.Validate (definitions);
    }

    /// <summary>
    /// Registers an interface to be associated with the given <see cref="ClassContext"/>. Later calls to <see cref="ResolveInterface"/>
    /// with the given interface type will result in the registered context being returned.
    /// </summary>
    /// <param name="interfaceType">Type of the interface to be registered.</param>
    /// <param name="associatedClassContext">The class context to be associated with the interface type.</param>
    /// <exception cref="InvalidOperationException">The interface has already been registered.</exception>
    /// <exception cref="ArgumentNullException">One of the parameters is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="interfaceType"/> argument is not an interface or
    /// <paramref name="associatedClassContext"/> has not been added to this configuration.</exception>
    public void RegisterInterface (Type interfaceType, ClassContext associatedClassContext)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      ArgumentUtility.CheckNotNull ("associatedClassContext", associatedClassContext);

      if (!interfaceType.IsInterface)
        throw new ArgumentException ("The argument is not an interface.", "interfaceType");

      if (!_classContexts.ContainsExact (associatedClassContext.Type)
          || !ReferenceEquals (_classContexts.GetExact (associatedClassContext.Type), associatedClassContext))
        throw new ArgumentException ("The class context hasn't been added to this configuration.", "associatedClassContext");

      if (_registeredInterfaces.ContainsKey (interfaceType))
      {
        string message = string.Format ("The interface {0} has already been associated with a class context.", interfaceType.FullName);
        throw new InvalidOperationException (message);
      }

      _registeredInterfaces.Add (interfaceType, associatedClassContext);
    }

    /// <summary>
    /// Registers an interface to be associated with the <see cref="ClassContext"/> for the given type. Later calls to <see cref="ResolveInterface"/>
    /// with the given interface type will result in the registered context being returned.
    /// </summary>
    /// <param name="interfaceType">Type of the interface to be registered.</param>
    /// <param name="associatedClassType">The type whose class context is to be associated with the interface type.</param>
    /// <exception cref="InvalidOperationException">The interface has already been registered.</exception>
    /// <exception cref="ArgumentNullException">One of the parameters is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="interfaceType"/> argument is not an interface or no <see cref="ClassContext"/> for
    /// <paramref name="associatedClassType"/> has been added to this configuration.</exception>
    public void RegisterInterface (Type interfaceType, Type associatedClassType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      ArgumentUtility.CheckNotNull ("associatedClassType", associatedClassType);

      ClassContext context = ClassContexts.GetExact (associatedClassType);
      if (context == null)
      {
        string message = string.Format ("There is no class context for the given type {0}.", associatedClassType.FullName);
        throw new ArgumentException (message, "associatedClassType");
      }
      else
        RegisterInterface (interfaceType, context);
    }


    /// <summary>
    /// Resolves the given interface into a class context.
    /// </summary>
    /// <param name="interfaceType">The interface type to be resolved.</param>
    /// <returns>The <see cref="ClassContext"/> previously registered for the given type, or <see langword="null"/> if the no context was registered.
    /// </returns>
    /// <exception cref="ArgumentNullException">The <paramref name="interfaceType"/> argument is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="interfaceType"/> argument is not an interface.</exception>
    public ClassContext ResolveInterface (Type interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);

      if (!interfaceType.IsInterface)
        throw new ArgumentException ("The argument is not an interface.", "interfaceType");

      if (_registeredInterfaces.ContainsKey (interfaceType))
        return _registeredInterfaces[interfaceType];
      else
        return null;
    }

    /// <summary>
    /// Copies all configuration data of this configuration to a destination object, replacing class contexts and registered interfaces
    /// for types that are configured in both objects.
    /// </summary>
    /// <param name="destination">The destination to copy all configuration data to..</param>
    public void CopyTo (MixinConfiguration destination)
    {
      ArgumentUtility.CheckNotNull ("destination", destination);

      foreach (ClassContext classContext in ClassContexts)
      {
        try
        {
          destination.ClassContexts.AddOrReplace (classContext);
        }
        catch (InvalidOperationException ex)
        {
          throw new ArgumentException (
              "The given destination configuration object conflicts with the source configuration: " + ex.Message,
              "destination",
              ex);
        }
      }

      foreach (KeyValuePair<Type, ClassContext> interfaceRegistration in _registeredInterfaces)
      {
        if (destination._registeredInterfaces.ContainsKey (interfaceRegistration.Key))
          destination._registeredInterfaces.Remove (interfaceRegistration.Key);
        destination.RegisterInterface (interfaceRegistration.Key, interfaceRegistration.Value.Type);
      }
    }

    private void ClassContextAdded (object sender, ClassContextEventArgs e)
    {
      foreach (Type completeInterface in e.ClassContext.CompleteInterfaces)
        RegisterInterface (completeInterface, e.ClassContext);
    }

    private void ClassContextRemoved (object sender, ClassContextEventArgs e)
    {
      var interfacesToBeRemoved = new List<Type> ();
      foreach (KeyValuePair<Type, ClassContext> item in _registeredInterfaces)
      {
        if (ReferenceEquals (item.Value, e.ClassContext))
          interfacesToBeRemoved.Add (item.Key);
      }
      foreach (Type type in interfacesToBeRemoved)
        _registeredInterfaces.Remove (type);
    }
  }
}
