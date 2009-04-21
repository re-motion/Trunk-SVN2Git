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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Factories;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class ClassDefinitionWithNullEndPointTest : StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    private ClassDefinition _clientClass;
    private AnonymousRelationEndPointDefinition _clientEndPoint;
    private ClassDefinition _locationClass;
    private RelationEndPointDefinition _locationEndPoint;

    // construction and disposing

    public ClassDefinitionWithNullEndPointTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _clientClass = TestMappingConfiguration.Current.ClassDefinitions.GetMandatory ("Client");
      _locationClass = TestMappingConfiguration.Current.ClassDefinitions.GetMandatory ("Location");

      RelationDefinition relation = TestMappingConfiguration.Current.RelationDefinitions.GetMandatory ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");
      _clientEndPoint = (AnonymousRelationEndPointDefinition) relation.EndPointDefinitions[0];
      _locationEndPoint = (RelationEndPointDefinition) relation.EndPointDefinitions[1];
    }

    [Test]
    public void GetRelationDefinitions ()
    {
      Assert.IsTrue (_locationClass.GetRelationDefinitions ().Contains ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client"));
      Assert.IsFalse (_clientClass.GetRelationDefinitions ().Contains ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client"));
    }

    [Test]
    public void GetRelationEndPointDefinitions ()
    {
      Assert.IsTrue (Contains (_locationClass.GetRelationEndPointDefinitions (), _locationEndPoint));
      Assert.IsFalse (Contains (_clientClass.GetRelationEndPointDefinitions (), _clientEndPoint));
    }

    [Test]
    public void GetMyRelationEndPointDefinitions ()
    {
      Assert.IsTrue (Contains (_locationClass.GetMyRelationEndPointDefinitions (), _locationEndPoint));
      Assert.IsFalse (Contains (_clientClass.GetMyRelationEndPointDefinitions (), _clientEndPoint));
    }

    private bool Contains (IRelationEndPointDefinition[] endPoints, IRelationEndPointDefinition value)
    {
      foreach (IRelationEndPointDefinition endPoint in endPoints)
      {
        if (ReferenceEquals (endPoint, value))
          return true;
      }

      return false;
    }
  }
}
