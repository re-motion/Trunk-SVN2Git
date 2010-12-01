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
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class Common : MappingReflectionTestBase
  {
    [Test]
    public void CreateRelationEndPointReflector()
    {
      PropertyInfo propertyInfo = typeof (ClassWithVirtualRelationEndPoints).GetProperty ("NoAttribute");
      Assert.IsInstanceOfType (
          typeof (RdbmsRelationEndPointReflector), 
          RelationEndPointReflector.CreateRelationEndPointReflector (
              CreateReflectionBasedClassDefinition (typeof (ClassWithVirtualRelationEndPoints)), propertyInfo, Configuration.NameResolver));
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_WithoutAttribute ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithRealRelationEndPoints).GetProperty ("NoAttribute");
      RelationEndPointReflector relationEndPointReflector =
          new RelationEndPointReflector (CreateReflectionBasedClassDefinition (typeof (ClassWithRealRelationEndPoints)), propertyInfo, Configuration.NameResolver);

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint ());
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_WithCollectionPropertyAndWithoutAttribute ()
    {
      Type type = typeof(ClassWithInvalidUnidirectionalRelation);

      PropertyInfo propertyInfo = type.GetProperty ("LeftSide");
      RelationEndPointReflector relationEndPointReflector =
          new RelationEndPointReflector (CreateReflectionBasedClassDefinition (type), propertyInfo, Configuration.NameResolver);

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint ());
    }

    [Test]
    public void IsVirtualRelationEndPoint_UnidirectionalRelation()
    {
      Type type = typeof (ClassWithRealRelationEndPoints);

      PropertyInfo propertyInfo = type.GetProperty ("Unidirectional");
      var relationEndPointReflector =
          new RdbmsRelationEndPointReflector (CreateReflectionBasedClassDefinition (type), propertyInfo, Configuration.NameResolver);

      Assert.IsFalse(relationEndPointReflector.IsVirtualEndRelationEndpoint ());
    }

    private ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type)
    {
      return ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, UnitTestDomainStorageProviderDefinition, type, false);
    }
  }
}
