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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class BaseOneSideRelationProperty: StandardMappingTest
  {
    private ReflectionBasedClassDefinition _classDefinition;

    public override void SetUp()
    {
      base.SetUp();

      _classDefinition = CreateReflectionBasedClassDefinition (typeof (ClassWithOneSideRelationProperties));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BaseBidirectionalOneToOne");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (_classDefinition, propertyInfo, Configuration.NameResolver);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata ();

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actual);
      VirtualRelationEndPointDefinition relationEndPointDefinition = (VirtualRelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationPropertiesNotInMapping.BaseBidirectionalOneToOne",
          relationEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ClassWithManySideRelationProperties), relationEndPointDefinition.PropertyType);
      Assert.AreEqual (CardinalityType.One, relationEndPointDefinition.Cardinality);
      Assert.IsNull (relationEndPointDefinition.RelationDefinition);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BaseBidirectionalOneToMany");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (_classDefinition, propertyInfo, Configuration.NameResolver);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata ();

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actual);
      VirtualRelationEndPointDefinition relationEndPointDefinition = (VirtualRelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationPropertiesNotInMapping.BaseBidirectionalOneToMany",
          relationEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ObjectList<ClassWithManySideRelationProperties>), relationEndPointDefinition.PropertyType);
      Assert.AreEqual (CardinalityType.Many, relationEndPointDefinition.Cardinality);
      Assert.IsNull (relationEndPointDefinition.RelationDefinition);
      Assert.AreEqual ("The Sort Expression", relationEndPointDefinition.SortExpression);
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToOne ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BaseBidirectionalOneToOne");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (_classDefinition, propertyInfo, Configuration.NameResolver);

      Assert.IsTrue (relationEndPointReflector.IsVirtualEndRelationEndpoint ());
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToMany ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BaseBidirectionalOneToMany");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (_classDefinition, propertyInfo, Configuration.NameResolver);

      Assert.IsTrue (relationEndPointReflector.IsVirtualEndRelationEndpoint());
    }

    private ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type)
    {
      return ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, "TestDomain", type, false);
    }
  }
}
