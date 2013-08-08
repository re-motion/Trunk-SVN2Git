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
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.TypePipe.Implementation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.TypePipe
{
  /// <summary>
  /// Creates new domain object instances via an instance of <see cref="IPipeline"/>.
  /// </summary>
  public class DomainObjectCreator : IDomainObjectCreator
  {
    // TODO 5375: Remove if possible.
    public static readonly DomainObjectCreator Instance = CreateInstance();

    // TODO 5375: Refactor away and move to callers?
    private static DomainObjectCreator CreateInstance ()
    {
      var pipelineRegistry = SafeServiceLocator.Current.GetInstance<IPipelineRegistry>();
      var defaultPipeline = pipelineRegistry.DefaultPipeline;

      return new DomainObjectCreator (defaultPipeline);
    }

    private readonly IPipeline _pipeline;

    public DomainObjectCreator (IPipeline pipeline)
    {
      ArgumentUtility.CheckNotNull ("pipeline", pipeline);

      _pipeline = pipeline;
    }

    public IPipeline Pipeline
    {
      get { return _pipeline; }
    }

    public DomainObject CreateObjectReference (IObjectInitializationContext objectInitializationContext, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("objectInitializationContext", objectInitializationContext);
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      var objectID = objectInitializationContext.ObjectID;
      CheckDomainTypeAndClassDefinition (objectID.ClassDefinition.ClassType);
      objectID.ClassDefinition.ValidateCurrentMixinConfiguration();

      var concreteType = _pipeline.ReflectionService.GetAssembledType (objectID.ClassDefinition.ClassType);
      var instance = (DomainObject) FormatterServices.GetSafeUninitializedObject (concreteType);
      _pipeline.PrepareExternalUninitializedObject (instance, InitializationSemantics.Construction);

      // These calls are normally performed by DomainObject's ctor
      instance.Initialize (objectID, objectInitializationContext.RootTransaction);
      objectInitializationContext.RegisterObject (instance);

      using (clientTransaction.EnterNonDiscardingScope())
      {
        instance.RaiseReferenceInitializatingEvent();
      }

      return instance;
    }

    public DomainObject CreateNewObject (IObjectInitializationContext objectInitializationContext, ParamList constructorParameters, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("objectInitializationContext", objectInitializationContext);
      ArgumentUtility.CheckNotNull ("constructorParameters", constructorParameters);
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      var domainObjectType = objectInitializationContext.ObjectID.ClassDefinition.ClassType;
      CheckDomainTypeAndClassDefinition (domainObjectType);
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (domainObjectType);
      classDefinition.ValidateCurrentMixinConfiguration();

      using (clientTransaction.EnterNonDiscardingScope ())
      {
        using (new ObjectInititalizationContextScope (objectInitializationContext))
        {
          var instance = (DomainObject) _pipeline.Create (domainObjectType, constructorParameters, allowNonPublicConstructor: true);
          DomainObjectMixinCodeGenerationBridge.OnDomainObjectCreated (instance);
          return instance;
        }
      }
    }

    private void CheckDomainTypeAndClassDefinition (Type domainObjectType)
    {
      if (domainObjectType.IsSealed)
      {
        var message = string.Format ("Cannot instantiate type '{0}' as it is sealed.", domainObjectType.FullName);
        throw new NonInterceptableTypeException (message, domainObjectType);
      }

      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (domainObjectType);
      if (classDefinition.IsAbstract)
      {
        var message1 = string.Format (
            "Cannot instantiate type '{0}' as it is abstract; for classes with automatic properties, InstantiableAttribute must be used.",
            classDefinition.ClassType.FullName);
        throw new NonInterceptableTypeException (message1, classDefinition.ClassType);
      }
    }
  }
}