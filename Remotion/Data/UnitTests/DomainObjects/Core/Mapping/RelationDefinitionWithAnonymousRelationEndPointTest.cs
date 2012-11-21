// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class RelationDefinitionWithAnonymousRelationEndPointTest : MappingReflectionTestBase
  {
    private RelationDefinition _relation;
    private AnonymousRelationEndPointDefinition _clientEndPoint;
    private RelationEndPointDefinition _locationEndPoint;

    public override void SetUp ()
    {
      base.SetUp ();

      _relation = FakeMappingConfiguration.Current.RelationDefinitions[
        "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Location:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
        +"TestDomain.Integration.Location.Client"];
      _clientEndPoint = (AnonymousRelationEndPointDefinition) _relation.EndPointDefinitions[0];
      _locationEndPoint = (RelationEndPointDefinition) _relation.EndPointDefinitions[1];
    }

    [Test]
    public void GetOppositeEndPointDefinition ()
    {
      Assert.AreSame (_clientEndPoint, _relation.GetOppositeEndPointDefinition ("Location", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Location.Client"));
      Assert.AreSame (_locationEndPoint, _relation.GetOppositeEndPointDefinition ("Client", null));
    }

    [Test]
    public void IsEndPoint ()
    {
      Assert.IsTrue (_relation.IsEndPoint ("Location", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Location.Client"));
      Assert.IsTrue (_relation.IsEndPoint ("Client", null));

      Assert.IsFalse (_relation.IsEndPoint ("Location", null));
      Assert.IsFalse (_relation.IsEndPoint ("Client", "Client"));
    }
  }
}
