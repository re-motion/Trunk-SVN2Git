/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping
{
  [TestFixture]
  public class AnonymousRelationEndPointDefinitionTest : StandardMappingTest
  {
    private ClassDefinition _clientDefinition;
    private AnonymousRelationEndPointDefinition _definition;

    public override void SetUp ()
    {
      base.SetUp ();

      _clientDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Client));
      _definition = new AnonymousRelationEndPointDefinition (_clientDefinition);
    }

    [Test]
    public void Initialize ()
    {

      Assert.IsNotNull (_definition as IRelationEndPointDefinition);
      Assert.IsNotNull (_definition as INullObject);
      Assert.AreSame (_clientDefinition, _definition.ClassDefinition);
      Assert.AreEqual (CardinalityType.Many, _definition.Cardinality);
      Assert.AreEqual (false, _definition.IsMandatory);
      Assert.AreEqual (true, _definition.IsVirtual);
      Assert.IsNull (_definition.PropertyName);
      Assert.IsNull (_definition.PropertyType);
      Assert.AreEqual (_clientDefinition.IsClassTypeResolved, _definition.IsPropertyTypeResolved);
      Assert.IsNull (_definition.PropertyTypeName);
      Assert.IsTrue (_definition.IsNull);
    }

    [Test]
    public void CorrespondsToTrue ()
    {
      Assert.IsTrue (_definition.CorrespondsTo (_clientDefinition.ID, null));
    }

    [Test]
    public void CorrespondsToFalse ()
    {
      Assert.IsFalse (_definition.CorrespondsTo (_clientDefinition.ID, "PropertyName"));
    }

    [Test]
    public void RelationDefinitionNull ()
    {
      AnonymousRelationEndPointDefinition definition = new AnonymousRelationEndPointDefinition (MappingConfiguration.Current.ClassDefinitions[typeof (Client)]);

      Assert.IsNull (definition.RelationDefinition);
    }

    [Test]
    public void RelationDefinitionNotNull ()
    {
      RelationEndPointDefinition oppositeEndPoint = new RelationEndPointDefinition (
          MappingConfiguration.Current.ClassDefinitions[typeof (Location)], "Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client", true);

      RelationDefinition relationDefinition = new RelationDefinition ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client", _definition, oppositeEndPoint);

      Assert.IsNotNull (_definition.RelationDefinition);
    }
  }
}
