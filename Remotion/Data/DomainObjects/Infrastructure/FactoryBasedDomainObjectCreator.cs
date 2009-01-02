// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  // Creates new domain object instances via the DPInterceptedDomainObjectFactory.
  // Needed constructors:
  // (any constructor) -- for new objects
  // no constructor for loading required
  internal class FactoryBasedDomainObjectCreator : IDomainObjectCreator
  {
    public static readonly FactoryBasedDomainObjectCreator Instance = new FactoryBasedDomainObjectCreator ();

    public DomainObject CreateWithDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      Assertion.IsTrue (dataContainer.ClassDefinition is ReflectionBasedClassDefinition);

      dataContainer.ClassDefinition.GetValidator ().ValidateCurrentMixinConfiguration ();

      IDomainObjectFactory factory = DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory;
      Type concreteType = factory.GetConcreteDomainObjectType(dataContainer.DomainObjectType);
      DomainObject instance = (DomainObject) FormatterServices.GetSafeUninitializedObject (concreteType);
      factory.PrepareUnconstructedInstance (instance);
      instance.InitializeFromDataContainer (dataContainer);
      return instance;
    }

    public IFuncInvoker<T> GetTypesafeConstructorInvoker<T> ()
       where T : DomainObject
    {
      return GetTypesafeConstructorInvoker<T> (typeof (T));
    }

    public IFuncInvoker<DomainObject> GetTypesafeConstructorInvoker (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (DomainObject));
      return GetTypesafeConstructorInvoker<DomainObject> (type);
    }

    private IFuncInvoker<TStaticType> GetTypesafeConstructorInvoker<TStaticType> (Type domainObjectType)
        where TStaticType : DomainObject
    {
      ArgumentUtility.CheckNotNull ("domainObjectType", domainObjectType);
      
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (domainObjectType);
      classDefinition.GetValidator ().ValidateCurrentMixinConfiguration ();

      IDomainObjectFactory factory = DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory;
      Type concreteType = factory.GetConcreteDomainObjectType(domainObjectType);
      return factory.GetTypesafeConstructorInvoker<TStaticType> (concreteType);
    }
  }
}
