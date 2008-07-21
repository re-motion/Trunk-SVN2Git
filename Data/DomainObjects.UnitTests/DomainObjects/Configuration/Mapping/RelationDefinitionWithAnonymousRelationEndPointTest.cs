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
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping
{
  [TestFixture]
  public class RelationDefinitionWithAnonymousRelationEndPointTest : StandardMappingTest
  {
    private RelationDefinition _relation;
    private AnonymousRelationEndPointDefinition _clientEndPoint;
    private RelationEndPointDefinition _locationEndPoint;

    public override void SetUp ()
    {
      base.SetUp ();

      _relation = TestMappingConfiguration.Current.RelationDefinitions.GetMandatory ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client");
      _clientEndPoint = (AnonymousRelationEndPointDefinition) _relation.EndPointDefinitions[0];
      _locationEndPoint = (RelationEndPointDefinition) _relation.EndPointDefinitions[1];
    }

    [Test]
    public void GetOppositeEndPointDefinition ()
    {
      Assert.AreSame (_clientEndPoint, _relation.GetOppositeEndPointDefinition ("Location", "Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client"));
      Assert.AreSame (_locationEndPoint, _relation.GetOppositeEndPointDefinition ("Client", null));
    }

    [Test]
    public void GetOppositeClassDefinition ()
    {
      Assert.AreSame (TestMappingConfiguration.Current.ClassDefinitions[typeof (Client)], _relation.GetOppositeClassDefinition ("Location", "Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client"));
      Assert.AreSame (TestMappingConfiguration.Current.ClassDefinitions[typeof (Location)], _relation.GetOppositeClassDefinition ("Client", null));
    }

    [Test]
    public void IsEndPoint ()
    {
      Assert.IsTrue (_relation.IsEndPoint ("Location", "Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client"));
      Assert.IsTrue (_relation.IsEndPoint ("Client", null));

      Assert.IsFalse (_relation.IsEndPoint ("Location", null));
      Assert.IsFalse (_relation.IsEndPoint ("Client", "Client"));
    }
  }
}
