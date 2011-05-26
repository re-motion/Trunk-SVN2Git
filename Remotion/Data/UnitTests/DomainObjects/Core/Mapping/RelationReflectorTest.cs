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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.RelationReflector.OppositePropertyIsAlsoDeclaredInBaseClass;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.RelationReflector.RelatedTypeDoesNotMatchOppositeProperty_AboveInheritanceRoot;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.RelationReflector.RelatedTypeDoesNotMatchOppositeProperty_BelowInheritanceRoot;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;
using Rhino.Mocks;
using Class1 =
    Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.RelationReflector.RelatedTypeDoesNotMatchOppositeProperty_BelowInheritanceRoot.
        Class1;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class RelationReflectorTest : MappingReflectionTestBase
  {
    private ClassDefinition _classWithRealRelationEndPoints;
    private ClassDefinition _classWithVirtualRelationEndPoints;
    private ClassDefinition _classWithBothEndPointsOnSameClassClassDefinition;
    private Dictionary<Type, ClassDefinition> _classDefinitions;
    private ReflectionBasedNameResolver _nameResolver;

    public override void SetUp ()
    {
      base.SetUp();
      _nameResolver = new ReflectionBasedNameResolver();
      _classWithRealRelationEndPoints = ClassDefinitionFactory.CreateClassDefinition (typeof (ClassWithRealRelationEndPoints));
      _classWithRealRelationEndPoints.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _classWithRealRelationEndPoints.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());

      _classWithVirtualRelationEndPoints = ClassDefinitionFactory.CreateClassDefinition (typeof (ClassWithVirtualRelationEndPoints));
      _classWithVirtualRelationEndPoints.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _classWithVirtualRelationEndPoints.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());

      _classWithBothEndPointsOnSameClassClassDefinition =
          ClassDefinitionFactory.CreateClassDefinition (typeof (ClassWithBothEndPointsOnSameClass));
      _classWithBothEndPointsOnSameClassClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _classWithBothEndPointsOnSameClassClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());

      _classDefinitions = new[]
                          {
                              _classWithRealRelationEndPoints,
                              _classWithVirtualRelationEndPoints,
                              _classWithBothEndPointsOnSameClassClassDefinition
                          }.ToDictionary (cd => cd.ClassType);
    }

    [Test]
    public void GetMetadata_RealSide_ID ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "Unidirectional")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "Unidirectional")))
          .Return (null);
      DomainModelConstraintProviderMock.Replay();

      var relationReflector = CreateRelationReflector (
          _classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints),
          "Unidirectional");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (
          actualRelationDefinition.ID,
          Is.EqualTo (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints:"
              +
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints.Unidirectional"));
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_Unidirectional_EndPoint0 ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "Unidirectional")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "Unidirectional")))
          .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      var relationReflector = CreateRelationReflector (
          _classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints),
          "Unidirectional");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOf (typeof (RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_classWithRealRelationEndPoints.MyPropertyDefinitions[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_classWithRealRelationEndPoints));
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_Unidirectional_EndPoint1 ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "Unidirectional")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "Unidirectional")))
          .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      var relationReflector = CreateRelationReflector (
          _classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints),
          "Unidirectional");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOf (typeof (AnonymousRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (AnonymousRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_classWithVirtualRelationEndPoints));

      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_WithRealRelationEndPoint_BidirectionalOneToOne_CheckEndPoint0 ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToOne")))
          .Repeat.Twice ()
          .Return (true);
      DomainModelConstraintProviderMock
         .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToOne")))
         .Repeat.Times (3)
         .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithRealRelationEndPoints, typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToOne");
      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithVirtualRelationEndPoints, typeof (ClassWithVirtualRelationEndPoints), "BidirectionalOneToOne");

      var relationReflector = CreateRelationReflector (
          _classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints),
          "BidirectionalOneToOne");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOf (typeof (RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_classWithRealRelationEndPoints.MyPropertyDefinitions[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_classWithRealRelationEndPoints));
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_WithRealRelationEndPoint_BidirectionalOneToOne_CheckEndPoint1 ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToOne")))
          .Repeat.Twice()
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToOne")))
          .Repeat.Times (3)
          .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithRealRelationEndPoints, typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToOne");
      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithVirtualRelationEndPoints, typeof (ClassWithVirtualRelationEndPoints), "BidirectionalOneToOne");

      var relationReflector = CreateRelationReflector (
          _classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints),
          "BidirectionalOneToOne");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOf (typeof (VirtualRelationEndPointDefinition)));
      var oppositeEndPointDefinition =
          (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_classWithVirtualRelationEndPoints));
      Assert.That (
          oppositeEndPointDefinition.PropertyName,
          Is.EqualTo (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToOne"));
      Assert.That (oppositeEndPointDefinition.PropertyType, Is.SameAs (typeof (ClassWithRealRelationEndPoints)));
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_WithVirtualRelationEndPoint_BidirectionalOneToOne_CheckEndPoint0 ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToOne")))
          .Repeat.Twice ()
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToOne")))
          .Repeat.Twice()
          .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithRealRelationEndPoints, typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToOne");
      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithVirtualRelationEndPoints, typeof (ClassWithVirtualRelationEndPoints), "BidirectionalOneToOne");
      var propertyInfo = PropertyInfoAdapter.Create (typeof (ClassWithVirtualRelationEndPoints).GetProperty ("BidirectionalOneToOne"));
      var relationReflector = CreateRelationReflector (_classWithVirtualRelationEndPoints, propertyInfo);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOf (typeof (VirtualRelationEndPointDefinition)));
      var oppositeEndPointDefinition =
          (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_classWithVirtualRelationEndPoints));
      Assert.That (
          oppositeEndPointDefinition.PropertyName,
          Is.EqualTo (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToOne"));
      Assert.That (oppositeEndPointDefinition.PropertyType, Is.SameAs (typeof (ClassWithRealRelationEndPoints)));
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_WithVirtualRelationEndPoint_BidirectionalOneToOne_CheckEndPoint1 ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToOne")))
          .Repeat.Twice()
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToOne")))
          .Repeat.Twice ()
          .Return (null);
      DomainModelConstraintProviderMock.Replay();

      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithRealRelationEndPoints, typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToOne");
      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithVirtualRelationEndPoints, typeof (ClassWithVirtualRelationEndPoints), "BidirectionalOneToOne");
      var propertyInfo = PropertyInfoAdapter.Create (typeof (ClassWithVirtualRelationEndPoints).GetProperty ("BidirectionalOneToOne"));
      var relationReflector = CreateRelationReflector (_classWithVirtualRelationEndPoints, propertyInfo);

      var actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOf (typeof (RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_classWithRealRelationEndPoints.MyPropertyDefinitions[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_classWithRealRelationEndPoints));
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_WithRealRelationEndPoint_BidirectionalOneToMany_CheckEndPoint0 ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToMany")))
          .Repeat.Times (3)
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToMany")))
          .Repeat.Times(3)
          .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithRealRelationEndPoints, typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToMany");
      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithVirtualRelationEndPoints, typeof (ClassWithVirtualRelationEndPoints), "BidirectionalOneToMany");

      var relationReflector = CreateRelationReflector (
          _classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints),
          "BidirectionalOneToMany");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOf (typeof (RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_classWithRealRelationEndPoints.MyPropertyDefinitions[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_classWithRealRelationEndPoints));
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_WithRealRelationEndPoint_BidirectionalOneToMany_CheckEndPoint1 ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToMany")))
          .Repeat.Times (3)
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToMany")))
          .Repeat.Times (3)
          .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithRealRelationEndPoints, typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToMany");
      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithVirtualRelationEndPoints, typeof (ClassWithVirtualRelationEndPoints), "BidirectionalOneToMany");

      var relationReflector = CreateRelationReflector (
          _classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints),
          "BidirectionalOneToMany");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOf (typeof (VirtualRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_classWithVirtualRelationEndPoints));
      Assert.That (
          oppositeEndPointDefinition.PropertyName,
          Is.EqualTo (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToMany"));
      Assert.That (oppositeEndPointDefinition.PropertyType, Is.SameAs (typeof (ObjectList<ClassWithRealRelationEndPoints>)));
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_WithVirtualRelationEndPoint_BidirectionalOneToMany_CheckEndPoint0 ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToMany")))
          .Repeat.Times (3)
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToMany")))
          .Repeat.Twice ()
          .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithRealRelationEndPoints, typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToMany");
      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithVirtualRelationEndPoints, typeof (ClassWithVirtualRelationEndPoints), "BidirectionalOneToMany");
      var propertyInfo = PropertyInfoAdapter.Create (typeof (ClassWithVirtualRelationEndPoints).GetProperty ("BidirectionalOneToMany"));
      var relationReflector = CreateRelationReflector (_classWithVirtualRelationEndPoints, propertyInfo);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOf (typeof (VirtualRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_classWithVirtualRelationEndPoints));
      Assert.That (
          oppositeEndPointDefinition.PropertyName,
          Is.EqualTo (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToMany"));
      Assert.That (oppositeEndPointDefinition.PropertyType, Is.SameAs (typeof (ObjectList<ClassWithRealRelationEndPoints>)));
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_WithVirtualRelationEndPoint_BidirectionalOneToMany_CheckEndPoint1 ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToMany")))
          .Repeat.Times (3)
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToMany")))
          .Repeat.Twice ()
          .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithRealRelationEndPoints, typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToMany");
      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithVirtualRelationEndPoints, typeof (ClassWithVirtualRelationEndPoints), "BidirectionalOneToMany");
      var propertyInfo = PropertyInfoAdapter.Create (typeof (ClassWithVirtualRelationEndPoints).GetProperty ("BidirectionalOneToMany"));
      var relationReflector = CreateRelationReflector (_classWithVirtualRelationEndPoints, propertyInfo);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOf (typeof (RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_classWithRealRelationEndPoints.MyPropertyDefinitions[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_classWithRealRelationEndPoints));
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany_WithBothEndPointsOnSameClass_EndPoint0 ()
    {
       DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "Parent")))
          .Return (true);
       DomainModelConstraintProviderMock
           .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "Parent")))
           .Repeat.Twice()
           .Return (null);
       DomainModelConstraintProviderMock
           .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "Children")))
           .Repeat.Twice()
           .Return (true);
       DomainModelConstraintProviderMock
            .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "Children")))
            .Return (null);
      DomainModelConstraintProviderMock.Replay();

      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithBothEndPointsOnSameClassClassDefinition, typeof (ClassWithBothEndPointsOnSameClass), "Parent");
      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithBothEndPointsOnSameClassClassDefinition, typeof (ClassWithBothEndPointsOnSameClass), "Children");
      var relationReflector = CreateRelationReflector (
          _classWithBothEndPointsOnSameClassClassDefinition,
          typeof (ClassWithBothEndPointsOnSameClass),
          "Parent");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOf (typeof (RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_classWithBothEndPointsOnSameClassClassDefinition.MyPropertyDefinitions[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_classWithBothEndPointsOnSameClassClassDefinition));
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany_WithBothEndPointsOnSameClass_EndPoint1 ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "Parent")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "Children")))
          .Repeat.Twice()
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "Parent")))
          .Repeat.Twice()
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "Children")))
          .Return (null);
      DomainModelConstraintProviderMock.Replay();
      
      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithBothEndPointsOnSameClassClassDefinition, typeof (ClassWithBothEndPointsOnSameClass), "Parent");
      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithBothEndPointsOnSameClassClassDefinition, typeof (ClassWithBothEndPointsOnSameClass), "Children");
      var relationReflector = CreateRelationReflector (
          _classWithBothEndPointsOnSameClassClassDefinition,
          typeof (ClassWithBothEndPointsOnSameClass),
          "Parent");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOf (typeof (VirtualRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_classWithBothEndPointsOnSameClassClassDefinition));
      Assert.That (
          oppositeEndPointDefinition.PropertyName,
          Is.EqualTo (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithBothEndPointsOnSameClass.Children"));
      Assert.That (oppositeEndPointDefinition.PropertyType, Is.SameAs (typeof (ObjectList<ClassWithBothEndPointsOnSameClass>)));
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_WithInvalidOppositePropertyName ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "InvalidOppositePropertyNameLeftSide")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "InvalidOppositePropertyNameLeftSide")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "InvalidPropertyNameInBidirectionalRelationAttributeOnOppositePropertyRightSide")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "InvalidPropertyNameInBidirectionalRelationAttributeOnOppositePropertyRightSide")))
          .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      var type1 = GetClassWithInvalidBidirectionalRelationLeftSide();
      var type2 = GetClassWithInvalidBidirectionalRelationRightSide();

      var classDefinition1 = ClassDefinitionFactory.CreateClassDefinition (type1);
      classDefinition1.SetPropertyDefinitions (new PropertyDefinitionCollection());
      classDefinition1.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      var propertyInfo = EnsurePropertyDefinitionExisitsOnClassDefinition (classDefinition1, type1, "InvalidOppositePropertyNameLeftSide");
      var classDefinition2 = ClassDefinitionFactory.CreateClassDefinition (type2);
      classDefinition2.SetPropertyDefinitions (new PropertyDefinitionCollection());
      classDefinition2.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      EnsurePropertyDefinitionExisitsOnClassDefinition (
          classDefinition2, type2, "InvalidPropertyNameInBidirectionalRelationAttributeOnOppositePropertyRightSide");

      var relationReflector = CreateRelationReflector (classDefinition1, propertyInfo);
      _classDefinitions.Add (classDefinition1.ClassType, classDefinition1);
      _classDefinitions.Add (classDefinition2.ClassType, classDefinition2);

      var result = relationReflector.GetMetadata (_classDefinitions);

      Assert.That (
          result.ID,
          Is.EqualTo (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide:Remotion.Data.UnitTests."
              + "DomainObjects.Core.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide.InvalidOppositePropertyNameLeftSide"));
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_OppositeClassDefinition_IsDeclaringTypeOfOppositeProperty_NotReturnTypeOfThisProperty ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "RelationProperty")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "RelationProperty")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "RelationPropertyOnBaseClass")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "RelationPropertyOnBaseClass")))
          .Return (null);
      DomainModelConstraintProviderMock.Replay();

      var originatingClass = ClassDefinitionFactory.CreateClassDefinition (typeof (Class1));
      originatingClass.SetPropertyDefinitions (new PropertyDefinitionCollection());
      originatingClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      var originatingProperty =
          EnsurePropertyDefinitionExisitsOnClassDefinition (originatingClass, typeof (Class1), "RelationProperty");

      var classDeclaringOppositeProperty = ClassDefinitionFactory.CreateClassDefinition (typeof (BaseClass2));
      classDeclaringOppositeProperty.SetPropertyDefinitions (new PropertyDefinitionCollection());
      classDeclaringOppositeProperty.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      var oppositeProperty =
          EnsurePropertyDefinitionExisitsOnClassDefinition (classDeclaringOppositeProperty, typeof (BaseClass2), "RelationPropertyOnBaseClass");
      var derivedOfClassDeclaringOppositeProperty = ClassDefinitionFactory.CreateClassDefinition (
          typeof (DerivedClass2), classDeclaringOppositeProperty);
      derivedOfClassDeclaringOppositeProperty.SetPropertyDefinitions (new PropertyDefinitionCollection());
      derivedOfClassDeclaringOppositeProperty.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());

      // This tests the scenario that the relation property's return type is a subclass of the opposite property's declaring type
      Assert.That (originatingProperty.PropertyType, Is.Not.SameAs (classDeclaringOppositeProperty.ClassType));
      Assert.That (originatingProperty.PropertyType.BaseType, Is.SameAs (classDeclaringOppositeProperty.ClassType));

      var classDefinitions = new[] { originatingClass, classDeclaringOppositeProperty, derivedOfClassDeclaringOppositeProperty }
          .ToDictionary (cd => cd.ClassType);

      var relationReflector = new RelationReflector (originatingClass, originatingProperty, _nameResolver);
      var result = relationReflector.GetMetadata (classDefinitions);

      Assert.That (result.EndPointDefinitions[0].PropertyInfo, Is.SameAs (originatingProperty));
      Assert.That (result.EndPointDefinitions[0].ClassDefinition, Is.SameAs (originatingClass));
      Assert.That (result.EndPointDefinitions[1].PropertyInfo, Is.SameAs (oppositeProperty));
      Assert.That (result.EndPointDefinitions[1].ClassDefinition, Is.SameAs (classDeclaringOppositeProperty));
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_OppositeClassDefinition_IsDeclaringTypeOfProperty_WithOverriddenProperty ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "RelationProperty")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "RelationProperty")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "OverriddenProperty")))
          .Repeat.Twice ()
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "OverriddenProperty")))
          .Repeat.Twice ()
          .Return (null);
      DomainModelConstraintProviderMock.Replay();

      var originatingClass =
          ClassDefinitionFactory.CreateClassDefinition (
              typeof (TestDomain.RelationReflector.RelatedTypeDoesNotMatchOverriddenOppositeProperty_BelowInheritanceRoot.Class1));
      originatingClass.SetPropertyDefinitions (new PropertyDefinitionCollection());
      originatingClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      var originatingProperty = EnsurePropertyDefinitionExisitsOnClassDefinition (
          originatingClass,
          typeof (TestDomain.RelationReflector.RelatedTypeDoesNotMatchOverriddenOppositeProperty_BelowInheritanceRoot.Class1),
          "RelationProperty");
      var classDeclaringOppositeProperty =
          ClassDefinitionFactory.CreateClassDefinition (
              typeof (TestDomain.RelationReflector.RelatedTypeDoesNotMatchOverriddenOppositeProperty_BelowInheritanceRoot.BaseClass2));
      classDeclaringOppositeProperty.SetPropertyDefinitions (new PropertyDefinitionCollection());
      classDeclaringOppositeProperty.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      var oppositeProperty = EnsurePropertyDefinitionExisitsOnClassDefinition (
          classDeclaringOppositeProperty,
          typeof (TestDomain.RelationReflector.RelatedTypeDoesNotMatchOverriddenOppositeProperty_BelowInheritanceRoot.BaseClass2),
          "OverriddenProperty");
      var derivedOfClassDeclaringOppositeProperty =
          ClassDefinitionFactory.CreateClassDefinition (
              typeof (TestDomain.RelationReflector.RelatedTypeDoesNotMatchOverriddenOppositeProperty_BelowInheritanceRoot.DerivedClass2),
              classDeclaringOppositeProperty);
      derivedOfClassDeclaringOppositeProperty.SetPropertyDefinitions (new PropertyDefinitionCollection());
      derivedOfClassDeclaringOppositeProperty.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      EnsurePropertyDefinitionExisitsOnClassDefinition (
          derivedOfClassDeclaringOppositeProperty,
          typeof (TestDomain.RelationReflector.RelatedTypeDoesNotMatchOverriddenOppositeProperty_BelowInheritanceRoot.DerivedClass2),
          "OverriddenProperty");

      // This tests the scenario that the relation property's return type is a subclass of the opposite property's declaring type
      Assert.That (originatingProperty.PropertyType, Is.Not.SameAs (classDeclaringOppositeProperty.ClassType));
      Assert.That (originatingProperty.PropertyType.BaseType, Is.SameAs (classDeclaringOppositeProperty.ClassType));

      var classDefinitions = new[] { originatingClass, classDeclaringOppositeProperty, derivedOfClassDeclaringOppositeProperty }
          .ToDictionary (cd => cd.ClassType);

      var relationReflector = new RelationReflector (originatingClass, originatingProperty, _nameResolver);
      var result = relationReflector.GetMetadata (classDefinitions);

      Assert.That (result.EndPointDefinitions[0].PropertyInfo, Is.SameAs (originatingProperty));
      Assert.That (result.EndPointDefinitions[0].ClassDefinition, Is.SameAs (originatingClass));
      Assert.That (result.EndPointDefinitions[1].PropertyInfo, Is.SameAs (oppositeProperty));
      Assert.That (result.EndPointDefinitions[1].ClassDefinition, Is.SameAs (classDeclaringOppositeProperty));
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_OppositeClassDefinition_IsInheritanceRoot_IfDeclaringTypeOfPropertyIsNotInMapping ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "RelationProperty")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "RelationProperty")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "RelationPropertyOnClassAboveInheritanceRoot")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "RelationPropertyOnClassAboveInheritanceRoot")))
          .Return (null);
      DomainModelConstraintProviderMock.Replay();

      var originatingClass = ClassDefinitionFactory.CreateClassDefinition (
          typeof (TestDomain.RelationReflector.RelatedTypeDoesNotMatchOppositeProperty_AboveInheritanceRoot.Class1));
      originatingClass.SetPropertyDefinitions (new PropertyDefinitionCollection());
      originatingClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      var originatingProperty = EnsurePropertyDefinitionExisitsOnClassDefinition (
          originatingClass,
          typeof (TestDomain.RelationReflector.RelatedTypeDoesNotMatchOppositeProperty_AboveInheritanceRoot.Class1),
          "RelationProperty");

      var classNotInMapping = ClassDefinitionFactory.CreateClassDefinition (typeof (ClassAboveInheritanceRoot));
      classNotInMapping.SetPropertyDefinitions (new PropertyDefinitionCollection());
      classNotInMapping.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      var derivedOfClassDeclaringOppositeProperty = ClassDefinitionFactory.CreateClassDefinition (typeof (Class2));
      derivedOfClassDeclaringOppositeProperty.SetPropertyDefinitions (new PropertyDefinitionCollection());
      derivedOfClassDeclaringOppositeProperty.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      EnsurePropertyDefinitionExisitsOnClassDefinition (
          derivedOfClassDeclaringOppositeProperty, typeof (Class2), "RelationPropertyOnClassAboveInheritanceRoot");

      Assert.That (originatingProperty.PropertyType, Is.Not.SameAs (classNotInMapping.ClassType));
      Assert.That (originatingProperty.PropertyType.BaseType, Is.SameAs (classNotInMapping.ClassType));

      var classDefinitions = new[] { originatingClass, derivedOfClassDeclaringOppositeProperty }.ToDictionary (cd => cd.ClassType);

      var relationReflector = new RelationReflector (originatingClass, originatingProperty, _nameResolver);
      var result = relationReflector.GetMetadata (classDefinitions);

      Assert.That (result.EndPointDefinitions[0].PropertyInfo, Is.SameAs (originatingProperty));
      Assert.That (result.EndPointDefinitions[0].ClassDefinition, Is.SameAs (originatingClass));
      Assert.That (result.EndPointDefinitions[1].PropertyInfo.Name, Is.EqualTo ("RelationPropertyOnClassAboveInheritanceRoot"));
      Assert.That (result.EndPointDefinitions[1].ClassDefinition, Is.SameAs (derivedOfClassDeclaringOppositeProperty));
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_RelatedPropertyTypeIsNotInMapping ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalRelationProperty")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalRelationProperty")))
          .Return (null);
      DomainModelConstraintProviderMock.Replay();

      var originatingClass =
          ClassDefinitionFactory.CreateClassDefinition (typeof (TestDomain.RelationReflector.RelatedPropertyTypeIsNotInMapping.Class1));
      originatingClass.SetPropertyDefinitions (new PropertyDefinitionCollection());
      originatingClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      var originatingProperty = EnsurePropertyDefinitionExisitsOnClassDefinition (
          originatingClass, typeof (TestDomain.RelationReflector.RelatedPropertyTypeIsNotInMapping.Class1), "BidirectionalRelationProperty");
      var classDefinitions = new[] { originatingClass }.ToDictionary (cd => cd.ClassType);

      var relationReflector = new RelationReflector (originatingClass, originatingProperty, _nameResolver);
      var result = relationReflector.GetMetadata (classDefinitions);

      Assert.That (result.EndPointDefinitions[0].ClassDefinition, Is.EqualTo (originatingClass));
      Assert.That (result.EndPointDefinitions[1], Is.TypeOf (typeof (PropertyNotFoundRelationEndPointDefinition)));
      Assert.That (result.EndPointDefinitions[1].PropertyName, Is.EqualTo ("RelationProperty"));
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_OppositePropertyIsAlsoDeclaredInBaseClass ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "OppositeProperty")))
          .Repeat.Times(3)
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "OppositeProperty")))
          .Repeat.Times (3)
          .Return (null);
      DomainModelConstraintProviderMock.Replay();

      var originatingClass = ClassDefinitionFactory.CreateClassDefinition (typeof (ClassWithOppositeProperty));
      originatingClass.SetPropertyDefinitions (new PropertyDefinitionCollection());
      originatingClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      var originatingProperty =
          EnsurePropertyDefinitionExisitsOnClassDefinition (originatingClass, typeof (ClassWithOppositeProperty), "OppositeProperty");

      var oppositeBaseClass = ClassDefinitionFactory.CreateClassDefinition (typeof (OppositeBaseClass));
      oppositeBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection());
      oppositeBaseClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      var oppositePropertyFromBaseClass =
          EnsurePropertyDefinitionExisitsOnClassDefinition (oppositeBaseClass, typeof (OppositeBaseClass), "OppositeProperty");

      var oppositeClass = ClassDefinitionFactory.CreateClassDefinition (typeof (OppositeClass));
      oppositeClass.SetPropertyDefinitions (new PropertyDefinitionCollection());
      oppositeClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      var oppositeProperty =
          EnsurePropertyDefinitionExisitsOnClassDefinition (oppositeClass, typeof (OppositeClass), "OppositeProperty");

      var classDefinitions = new[] { originatingClass, oppositeClass, oppositeBaseClass }.ToDictionary (cd => cd.ClassType);
      var relationReflector = new RelationReflector (originatingClass, originatingProperty, _nameResolver);
      var result = relationReflector.GetMetadata (classDefinitions);

      Assert.That (result.EndPointDefinitions[0].PropertyInfo, Is.SameAs (originatingProperty));
      Assert.That (result.EndPointDefinitions[0].ClassDefinition, Is.SameAs (originatingClass));
      Assert.That (result.EndPointDefinitions[1].PropertyInfo, Is.Not.SameAs (oppositePropertyFromBaseClass));
      Assert.That (result.EndPointDefinitions[1].PropertyInfo, Is.SameAs (oppositeProperty));
      Assert.That (result.EndPointDefinitions[1].ClassDefinition, Is.SameAs (oppositeClass));
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetOppositePropertyInfo_WithStorageClassNoneOppositeProperty ()
    {
      var classType = typeof (TestDomain.RelationReflector.OppositePropertyHasStorageClassNoneAttribute.ClassWithOppositeProperty);
      var originatingClass = ClassDefinitionFactory.CreateClassDefinition (classType);
      var originatingProperty = PropertyInfoAdapter.Create (classType.GetProperty ("OppositeProperty"));

      var relationReflector = new RelationReflector (originatingClass, originatingProperty, _nameResolver);

      var oppositePropertyInfo = (IPropertyInformation) PrivateInvoke.InvokeNonPublicMethod (relationReflector, "GetOppositePropertyInfo");
      Assert.That (oppositePropertyInfo, Is.Null);
    }

    private Type GetClassWithInvalidBidirectionalRelationLeftSide ()
    {
      return typeof (ClassWithInvalidBidirectionalRelationLeftSide);
    }

    private Type GetClassWithInvalidBidirectionalRelationRightSide ()
    {
      return typeof (ClassWithInvalidBidirectionalRelationRightSide);
    }

    private RelationReflector CreateRelationReflector (ClassDefinition classDefinition, IPropertyInformation propertyInfo)
    {
      return new RelationReflector (classDefinition, propertyInfo, _nameResolver);
    }

    private RelationReflector CreateRelationReflector (ClassDefinition classDefinition, Type declaringType, string propertyName)
    {
      var propertyInfo = EnsurePropertyDefinitionExisitsOnClassDefinition (classDefinition, declaringType, propertyName);
      return CreateRelationReflector (classDefinition, propertyInfo);
    }

    private IPropertyInformation EnsurePropertyDefinitionExisitsOnClassDefinition (
        ClassDefinition classDefinition,
        Type declaringType,
        string propertyName)
    {
      var propertyInfo =
          PropertyInfoAdapter.Create (declaringType.GetProperty (propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
      var propertyReflector = new PropertyReflector (
          classDefinition, propertyInfo, new ReflectionBasedNameResolver(), DomainModelConstraintProviderMock);
      var propertyDefinition = propertyReflector.GetMetadata();

      if (!classDefinition.MyPropertyDefinitions.Contains (propertyDefinition.PropertyName))
      {
        var propertyDefinitions = new PropertyDefinitionCollection (classDefinition.MyPropertyDefinitions, false);
        propertyDefinitions.Add (propertyDefinition);
        PrivateInvoke.SetNonPublicField (classDefinition, "_propertyDefinitions", propertyDefinitions);
        var endPoints = new RelationEndPointDefinitionCollection (classDefinition.MyRelationEndPointDefinitions, false);
        endPoints.Add (MappingObjectFactory.CreateRelationEndPointDefinition (classDefinition, propertyInfo));
        PrivateInvoke.SetNonPublicField (classDefinition, "_relationEndPoints", endPoints);
      }

      return propertyInfo;
    }
  }
}