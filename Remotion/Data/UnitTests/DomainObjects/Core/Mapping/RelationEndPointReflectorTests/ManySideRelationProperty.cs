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
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class ManySideRelationProperty : MappingReflectionTestBase
  {
    private ClassDefinition _classDefinition;
    private Type _classType;

    public override void SetUp ()
    {
      base.SetUp();

      _classType = typeof (ClassWithRealRelationEndPoints);
      _classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "ClassWithManySideRelationProperties",
          "ClassWithManySideRelationProperties",
          UnitTestDomainStorageProviderDefinition,
          _classType,
          false);
    }

    [Test]
    public void GetMetadata_ForOptional ()
    {
      DomainModelConstraintProviderMock
        .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "NoAttribute")))
        .Return (null);
      DomainModelConstraintProviderMock
        .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "NoAttribute")))
        .Return (true);
      DomainModelConstraintProviderMock.Replay();

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("NoAttribute");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (RelationEndPointDefinition), actual);
      Assert.IsFalse (actual.IsMandatory);
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_ForMandatory ()
    {
      DomainModelConstraintProviderMock
        .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "NotNullable")))
        .Return (null);
      DomainModelConstraintProviderMock
        .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "NotNullable")))
        .Return (false);
      DomainModelConstraintProviderMock.Replay ();

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("NotNullable");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (RelationEndPointDefinition), actual);
      Assert.IsTrue (actual.IsMandatory);
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_Unidirectional ()
    {
      DomainModelConstraintProviderMock
        .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "Unidirectional")))
        .Return (null);
      DomainModelConstraintProviderMock
        .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "Unidirectional")))
        .Return (true);
      DomainModelConstraintProviderMock.Replay();

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("Unidirectional");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefinition = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefinition.ClassDefinition);
      Assert.AreSame (GetPropertyDefinition ("Unidirectional"), relationEndPointDefinition.PropertyDefinition);
      Assert.IsNull (relationEndPointDefinition.RelationDefinition);
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne ()
    {
      DomainModelConstraintProviderMock
        .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToOne")))
        .Return (null);
      DomainModelConstraintProviderMock
        .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToOne")))
        .Return (true);
      DomainModelConstraintProviderMock.Replay ();

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BidirectionalOneToOne");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefinition = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefinition.ClassDefinition);
      Assert.AreSame (GetPropertyDefinition ("BidirectionalOneToOne"), relationEndPointDefinition.PropertyDefinition);
      Assert.IsNull (relationEndPointDefinition.RelationDefinition);
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany ()
    {
      DomainModelConstraintProviderMock
        .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToMany")))
        .Return (null);
      DomainModelConstraintProviderMock
        .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToMany")))
        .Return (true);
      DomainModelConstraintProviderMock.Replay();

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BidirectionalOneToMany");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefinition = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefinition.ClassDefinition);
      Assert.AreSame (GetPropertyDefinition ("BidirectionalOneToMany"), relationEndPointDefinition.PropertyDefinition);
      Assert.IsNull (relationEndPointDefinition.RelationDefinition);
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_Unidirectional ()
    {
      DomainModelConstraintProviderMock
        .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "Unidirectional")))
        .Return (null);
      DomainModelConstraintProviderMock.Replay();

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("Unidirectional");

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint());
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToOne ()
    {
      DomainModelConstraintProviderMock
        .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToOne")))
        .Return (null);
      DomainModelConstraintProviderMock.Replay();

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BidirectionalOneToOne");

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint());
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToMany ()
    {
      DomainModelConstraintProviderMock
        .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToMany")))
        .Return (null);
      DomainModelConstraintProviderMock.Replay();

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BidirectionalOneToMany");

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint());
      DomainModelConstraintProviderMock.VerifyAllExpectations();
    }

    private RdbmsRelationEndPointReflector CreateRelationEndPointReflector (string propertyName)
    {
      PropertyReflector propertyReflector = CreatePropertyReflector (propertyName);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));

      return new RdbmsRelationEndPointReflector (
          _classDefinition, propertyReflector.PropertyInfo, Configuration.NameResolver, DomainModelConstraintProviderMock);
    }

    private PropertyReflector CreatePropertyReflector (string property)
    {
      var propertyInfo =
          PropertyInfoAdapter.Create (_classType.GetProperty (property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

      return new PropertyReflector (_classDefinition, propertyInfo, Configuration.NameResolver, DomainModelConstraintProviderMock);
    }

    private PropertyDefinition GetPropertyDefinition (string propertyName)
    {
      return _classDefinition.MyPropertyDefinitions[string.Format ("{0}.{1}", _classType.FullName, propertyName)];
    }
  }
}