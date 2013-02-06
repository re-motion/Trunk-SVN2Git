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
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Creates new domain object instances via the <see cref="InterceptedDomainObjectTypeFactory"/>.
  /// </summary>
  public class InterceptedDomainObjectCreator : IDomainObjectCreator
  {
    public static readonly InterceptedDomainObjectCreator Instance = new InterceptedDomainObjectCreator();

    public InterceptedDomainObjectCreator ()
    {
      Factory = new InterceptedDomainObjectTypeFactory (Environment.CurrentDirectory, TypeConversionProvider.Create());
    }

    public InterceptedDomainObjectTypeFactory Factory { get; set; }

    public DomainObject CreateObjectReference (ObjectID objectID, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      objectID.ClassDefinition.ValidateCurrentMixinConfiguration ();

      var concreteType = Factory.GetConcreteDomainObjectType (objectID.ClassDefinition.ClassType);

      var instance = (DomainObject) FormatterServices.GetSafeUninitializedObject (concreteType);
      Factory.PrepareUnconstructedInstance (instance);

      instance.Initialize (objectID, clientTransaction as BindingClientTransaction);

      clientTransaction.EnlistDomainObject (instance);
      clientTransaction.Execute (instance.RaiseReferenceInitializatingEvent);

      return instance;
    }

    public DomainObject CreateNewObject (Type domainObjectType, ParamList constructorParameters, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("domainObjectType", domainObjectType);
      ArgumentUtility.CheckNotNull ("constructorParameters", constructorParameters);

      var constructorLookupInfo = GetConstructorLookupInfo (domainObjectType);
      using (clientTransaction.EnterNonDiscardingScope())
      {
        var instance = (DomainObject) constructorParameters.InvokeConstructor (constructorLookupInfo);
        DomainObjectMixinCodeGenerationBridge.OnDomainObjectCreated (instance);
        return instance;
      }
    }

    // Public solely for TypePipe.PerformanceTests.
    public IConstructorLookupInfo GetConstructorLookupInfo (Type domainObjectType)
    {
      ArgumentUtility.CheckNotNull ("domainObjectType", domainObjectType);

      ClassDefinition classDefinition = MappingConfiguration.Current.GetTypeDefinition (domainObjectType);
      classDefinition.ValidateCurrentMixinConfiguration ();

      Type concreteType = Factory.GetConcreteDomainObjectType (domainObjectType);
      return new DomainObjectConstructorLookupInfo (domainObjectType, concreteType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }
  }
}
