// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.DataManagement;
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
    public static readonly InterceptedDomainObjectCreator Instance = new InterceptedDomainObjectCreator ();

    private InterceptedDomainObjectCreator ()
    {
      Factory = new InterceptedDomainObjectTypeFactory ();
    }

    public InterceptedDomainObjectTypeFactory Factory { get; set; }

    public DomainObject CreateWithDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      dataContainer.ClassDefinition.GetValidator ().ValidateCurrentMixinConfiguration ();

      Type concreteType = Factory.GetConcreteDomainObjectType(dataContainer.DomainObjectType);

      var instance = (DomainObject) FormatterServices.GetSafeUninitializedObject (concreteType);
      Factory.PrepareUnconstructedInstance (instance);
      dataContainer.SetDomainObject (instance);
      instance.Initialize (dataContainer.ID, dataContainer.ClientTransaction);
      return instance;
    }

    public ConstructorLookupInfo GetConstructorLookupInfo (Type domainObjectType)
    {
      ArgumentUtility.CheckNotNull ("domainObjectType", domainObjectType);

      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (domainObjectType);
      classDefinition.GetValidator ().ValidateCurrentMixinConfiguration ();

      Type concreteType = Factory.GetConcreteDomainObjectType (domainObjectType);
      return new DomainObjectConstructorLookupInfo (domainObjectType, concreteType, 
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }
  }
}
