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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class AnonymousRelationEndPointDefinitionTest : MappingReflectionTestBase
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
      Assert.AreSame (_clientDefinition, _definition.ClassDefinition);
      Assert.AreEqual (CardinalityType.Many, _definition.Cardinality);
      Assert.AreEqual (false, _definition.IsMandatory);
      Assert.AreEqual (true, _definition.IsVirtual);
      Assert.IsNull (_definition.PropertyName);
      Assert.IsNull (_definition.PropertyType);
      Assert.IsTrue (_definition.IsAnonymous);
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
      AnonymousRelationEndPointDefinition definition = new AnonymousRelationEndPointDefinition (MappingConfiguration.Current.TypeDefinitions[typeof (Client)]);

      Assert.IsNull (definition.RelationDefinition);
    }

    [Test]
    public void SetRelationDefinition ()
    {
      var propertyDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (Location)]
        ["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Location.Client"];
      var oppositeEndPoint = new RelationEndPointDefinition (propertyDefinition, true);
      var relationDefinition = new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Location.Client", _definition, oppositeEndPoint);

      _definition.SetRelationDefinition (relationDefinition);

      Assert.IsNotNull (_definition.RelationDefinition);
    }
  }
}
