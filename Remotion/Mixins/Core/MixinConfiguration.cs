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
using System.Linq;
using Remotion.Context;
using Remotion.Logging;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Definitions.Building;
using Remotion.Mixins.Utilities;
using Remotion.Mixins.Validation;
using Remotion.Utilities;

namespace Remotion.Mixins
{
  /// <summary>
  /// Constitutes a mixin configuration (ie. a set of classes associated with mixins) and manages the mixin configuration for the current thread
  /// (actually: <see cref="SafeContext"/>). 
  /// Instances of this class are immutable, i.e., their content is initialized on construction and cannot be changed later on.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Instances of this class represent a single mixin configuration, ie. a set of classes associated with mixins. The class manages a thread-local
  /// (actually <see cref="SafeContext"/>-local) single active configuration instance via its <see cref="ActiveConfiguration"/> property and
  /// related methods; the active configuration can conveniently be replaced via the <see cref="EnterScope"/> method.
  /// </para>
  /// <para>
  /// <see cref="MixinConfiguration"/> also provides entry points for building new mixin configuration objects: <see cref="BuildNew"/>, 
  /// <see cref="BuildFromActive"/>, and <see cref="BuildFrom"/>.
  /// </para>
  /// <para>
  /// While the <see cref="MixinConfiguration.ActiveConfiguration"/> will usually be accessed only indirectly via <see cref="ObjectFactory"/> or 
  /// <see cref="TypeFactory"/>, <see cref="EnterScope"/> and the <see cref="BuildFromActive">BuildFrom...</see> methods can be very useful to adjust 
  /// a thread's mixin configuration at runtime.
  /// </para>
  /// <para>
  /// The master mixin configuration - the default configuration in effect for a thread if not specifically replaced by another configuration - is 
  /// obtained by analyzing the assemblies in the application's bin directory for attributes such as <see cref="UsesAttribute"/>,
  /// <see cref="ExtendsAttribute"/>, and <see cref="CompleteInterfaceAttribute"/>. (For more information about the default configuration, see
  /// <see cref="DeclarativeConfigurationBuilder.BuildDefaultConfiguration"/>.) The master configuration can be accessed via 
  /// <see cref="GetMasterConfiguration"/> and <see cref="SetMasterConfiguration"/>.
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
  /// <threadsafety static="true" instance="true" />
  public partial class MixinConfiguration
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (MixinConfiguration));

    private readonly ClassContextCollection _classContexts;

    /// <summary>
    /// Initializes an empty mixin configuration.
    /// </summary>
    public MixinConfiguration () : this (new ClassContextCollection ())
    {
    }

    /// <summary>
    /// Initializes a non-empty mixin configuration.
    /// </summary>
    /// <param name="classContexts">The class contexts to be held by this <see cref="MixinConfiguration"/>.</param>
    public MixinConfiguration (ClassContextCollection classContexts)
    {
      _classContexts = classContexts;
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

      if (MixinTypeUtility.IsGeneratedConcreteMixedType (targetOrConcreteType))
        return MixinTypeUtility.GetClassContextForConcreteType (targetOrConcreteType);
      
      ClassContext context = ClassContexts.GetWithInheritance (targetOrConcreteType);
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
        return new ClassContext (targetOrConcreteType, Enumerable.Empty<MixinContext>(), Enumerable.Empty<Type>());
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
    /// <returns>An <see cref="ValidationLogData"/> object, which contains information about whether the configuration reresented by this context is valid.</returns>
    /// <remarks>This method retrieves definition items for all the <see cref="ClassContexts"/> known by this configuration and uses the
    /// <see cref="Validator"/> class to validate them. The validation results can be inspected, passed to a <see cref="ValidationException"/>, or
    /// be dumped using the <see cref="ConsoleDumper"/>.</remarks>
    /// <exception cref="NotSupportedException">The <see cref="MixinConfiguration"/> contains a <see cref="ClassContext"/> for a generic type, of
    /// which it cannot make a closed generic type. Because closed types are needed for validation, this <see cref="MixinConfiguration"/>
    /// cannot be validated as a whole. Even in this case, the configuration might still be correct, but validation is deferred to
    /// <see cref="TargetClassDefinitionFactory.CreateTargetClassDefinition(ClassContext)"/>.</exception>
    public ValidationLogData Validate()
    {
      var builder = new TargetClassDefinitionBuilder ();

      var definitions = from classContext in ClassContexts
                        where !classContext.Type.IsGenericTypeDefinition && !classContext.Type.IsInterface
                        select (IVisitableDefinition) builder.Build (classContext);

      return Validator.Validate (definitions);
    }

    [Obsolete ("This feature has been removed. Use an IOC container to resolve interfaces into implementations. (1.13.106)", true)]
    public ClassContext ResolveCompleteInterface (Type interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      throw new NotImplementedException ("This feature has been removed. Use an IOC container to resolve interfaces into implementations.");
    }
  }
}
