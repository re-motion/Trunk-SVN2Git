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
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.TypePipe
{
  /// <summary>
  /// Creates new domain object instances via an instance of <see cref="IObjectFactory"/>.
  /// </summary>
  public class TypePipeBasedDomainObjectCreator : IDomainObjectCreator
  {
    private readonly IObjectFactory _objectFactory;

    public TypePipeBasedDomainObjectCreator (IObjectFactory objectFactory)
    {
      ArgumentUtility.CheckNotNull ("objectFactory", objectFactory);

      _objectFactory = objectFactory;
    }

    public DomainObject CreateObjectReference (ObjectID objectID, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("objectID", objectID.ClassDefinition.ClassType, typeof (DomainObject));
      
      CheckDomainTypeAndClassDefinition (objectID.ClassDefinition.ClassType);
      objectID.ClassDefinition.ValidateCurrentMixinConfiguration();

      var mixedType = DomainObjectMixinCodeGenerationBridge.GetConcreteType (objectID.ClassDefinition.ClassType);
      var concreteType = _objectFactory.GetAssembledType (mixedType);
      var instance = (DomainObject) FormatterServices.GetSafeUninitializedObject (concreteType);
      _objectFactory.PrepareExternalUninitializedObject (instance);

      instance.Initialize (objectID, clientTransaction as BindingClientTransaction);

      clientTransaction.EnlistDomainObject (instance);
      clientTransaction.Execute (instance.RaiseReferenceInitializatingEvent);

      return instance;
    }

    public DomainObject CreateNewObject (Type domainObjectType, ParamList constructorParameters, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("domainObjectType", domainObjectType, typeof (DomainObject));
      ArgumentUtility.CheckNotNull ("constructorParameters", constructorParameters);
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      
      CheckDomainTypeAndClassDefinition (domainObjectType);
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (domainObjectType);
      classDefinition.ValidateCurrentMixinConfiguration();

      var mixedType = DomainObjectMixinCodeGenerationBridge.GetConcreteType (domainObjectType);
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        var instance = (DomainObject) _objectFactory.CreateObject (mixedType, constructorParameters, allowNonPublicConstructor: true);
        DomainObjectMixinCodeGenerationBridge.OnDomainObjectCreated (instance);
        return instance;
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