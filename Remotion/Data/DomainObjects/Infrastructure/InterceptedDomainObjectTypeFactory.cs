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
using Remotion.Collections;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides functionality for creating instances of DomainObjects which intercept property calls.
  /// </summary>
  public class InterceptedDomainObjectTypeFactory
  {
    private struct CodeGenerationKeys
    {
      public readonly Type PublicDomainObjectType;
      public readonly Type TypeToDeriveFrom;

      public CodeGenerationKeys (Type publicDomainObjectType, Type typeToDeriveFrom)
      {
        ArgumentUtility.CheckNotNull ("publicDomainObjectType", publicDomainObjectType);
        ArgumentUtility.CheckNotNull ("typeToDeriveFrom", typeToDeriveFrom);

        PublicDomainObjectType = publicDomainObjectType;
        TypeToDeriveFrom = typeToDeriveFrom;
      }
    }

    private readonly ModuleManager _scope;
    private readonly InterlockedCache<CodeGenerationKeys, Type> _typeCache = new InterlockedCache<CodeGenerationKeys, Type> ();

    /// <summary>
    /// Initializes a new instance of the <see cref="InterceptedDomainObjectTypeFactory"/> class.
    /// </summary>
    public InterceptedDomainObjectTypeFactory ()
        : this (Environment.CurrentDirectory)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InterceptedDomainObjectTypeFactory"/> class.
    /// </summary>
    /// <param name="assemblyDirectory">The directory to save the generated assemblies to. This directory is only used when
    /// <see cref="SaveGeneratedAssemblies"/> is used.</param>
    public InterceptedDomainObjectTypeFactory (string assemblyDirectory)
    {
      _scope = new ModuleManager (assemblyDirectory);
    }

    /// <summary>
    /// Gets the <see cref="ModuleManager"/> scope used by this factory.
    /// </summary>
    /// <value>The scope used by this factory to generate code.</value>
    public ModuleManager Scope
    {
      get { return _scope; }
    }

    /// <summary>
    /// Saves the assemblies generated by the factory and returns the paths of the saved manifest modules.
    /// </summary>
    /// <returns>The paths of the manifest modules of the saved assemblies.</returns>
    public string[] SaveGeneratedAssemblies ()
    {
      return _scope.SaveAssemblies ();
    }

    /// <summary>
    /// Gets a domain object type assignable to the given base type which intercepts property calls.
    /// </summary>
    /// <param name="baseType">The base domain object type whose properties should be intercepted.</param>
    /// <returns>A domain object type which intercepts property calls.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="baseType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentTypeException"><paramref name="baseType"/> cannot be assigned to <see cref="DomainObject"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="baseType"/> is an abstract, non-instantiable type.</exception>
    /// <exception cref="MappingException">The given <paramref name="baseType"/> is not part of the mapping.</exception>
    public Type GetConcreteDomainObjectType (Type baseType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("baseType", baseType, typeof (DomainObject));

      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (baseType);
      Type concreteBaseType = DomainObjectMixinCodeGenerationBridge.GetConcreteType (baseType);
      
      return GetConcreteDomainObjectType (classDefinition, concreteBaseType, "baseType");
    }

    /// <summary>
    /// Gets a domain object type assignable to the given base type which intercepts property calls.
    /// </summary>
    /// <param name="baseTypeClassDefinition">The base domain object type whose properties should be intercepted.</param>
    /// <param name="concreteBaseType">The base domain object type whose properties should be intercepted.</param>
    /// <returns>A domain object type which intercepts property calls.</returns>
    /// <exception cref="ArgumentNullException">One of the parameters passed to this method is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentTypeException"><paramref name="concreteBaseType"/> cannot be assigned to the type specified by <paramref name="baseTypeClassDefinition"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="baseTypeClassDefinition"/> denotes an abstract, non-instantiable type.</exception>
    public Type GetConcreteDomainObjectType (ClassDefinition baseTypeClassDefinition, Type concreteBaseType)
    {
      ArgumentUtility.CheckNotNull ("baseTypeClassDefinition", baseTypeClassDefinition);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("concreteBaseType", concreteBaseType, baseTypeClassDefinition.ClassType);

      return GetConcreteDomainObjectType(baseTypeClassDefinition, concreteBaseType, "baseTypeClassDefinition");
    }

    /// <summary>
    /// Checkes whether a given domain object type was created by this factory implementation (but not necessarily the same factory instance).
    /// </summary>
    /// <param name="type">The type to be checked.</param>
    /// <returns>True if <paramref name="type"/> was created by an instance of the <see cref="InterceptedDomainObjectTypeFactory"/>; false otherwise.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter was null.</exception>
    public bool WasCreatedByFactory (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return typeof (IInterceptedDomainObject).IsAssignableFrom (type);
    }

    /// <summary>
    /// Prepares an instance which has not been created via an ordinary constructor callfor use.
    /// </summary>
    /// <param name="instance">The instance to be prepared</param>
    /// <exception cref="ArgumentNullException">The <paramref name="instance"/> argument is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="instance"/> is not of a type created by this kind of factory.</exception>
    public void PrepareUnconstructedInstance (DomainObject instance)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      Type type = ((object)instance).GetType ();
      if (!WasCreatedByFactory (type))
        throw new ArgumentException (
            string.Format ("The domain object's type {0} was not created by InterceptedDomainObjectTypeFactory.GetConcreteDomainObjectType.",
              type.FullName), "instance");

      DomainObjectMixinCodeGenerationBridge.PrepareUnconstructedInstance (instance);
    }

    private Type GetConcreteDomainObjectType (ClassDefinition baseTypeClassDefinition, Type concreteBaseType, string argumentNameForExceptions)
    {
      if (baseTypeClassDefinition.IsAbstract)
      {
        string message = string.Format (
          "Cannot instantiate type {0} as it is abstract; for classes with automatic properties, InstantiableAttribute must be used.",
          baseTypeClassDefinition.ClassType.FullName);
        throw new ArgumentException (message, argumentNameForExceptions);
      }

      try
      {
        return _typeCache.GetOrCreateValue (new CodeGenerationKeys (baseTypeClassDefinition.ClassType, concreteBaseType),
            CreateConcreteDomainObjectType);
      }
      catch (NonInterceptableTypeException ex)
      {
        throw new ArgumentException (ex.Message, argumentNameForExceptions, ex);
      }
    }

    private Type CreateConcreteDomainObjectType (CodeGenerationKeys codeGenerationData)
    {
      TypeGenerator generator = _scope.CreateTypeGenerator (codeGenerationData.PublicDomainObjectType, codeGenerationData.TypeToDeriveFrom);
      return generator.BuildType ();
    }
  }
}
