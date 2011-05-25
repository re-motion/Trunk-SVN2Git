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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Reflection;
using Remotion.Utilities;
using ReflectionUtility = Remotion.Data.DomainObjects.ReflectionUtility;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.PropertyReflectorTests
{
  public class BaseTest : MappingReflectionTestBase
  {
    protected PropertyReflector CreatePropertyReflector<T> (string property)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("property", property);

      Type type = typeof (T);
      var propertyInfo = PropertyInfoAdapter.Create(type.GetProperty (property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
      ClassDefinition classDefinition;
      if (ReflectionUtility.IsDomainObject (type))
        classDefinition = ClassDefinitionFactory.CreateClassDefinition (type.Name, type.Name, TestDomainStorageProviderDefinition, type, true);
      else
        classDefinition = ClassDefinitionFactory.CreateClassDefinition ("Order", "Order", TestDomainStorageProviderDefinition, typeof (Order), false);

      return new PropertyReflector (classDefinition, propertyInfo, MappingConfiguration.Current.NameResolver, new DomainModelConstraintProvider());
    }
  }
}
