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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class RelationDefinitionCollectionTest : MappingReflectionTestBase
  {
    // types

    // static members and constants

    // member fields

    private RelationDefinitionCollection _collection;
    private RelationDefinition _relationDefinition;

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _relationDefinition = FakeMappingConfiguration.Current.RelationDefinitions["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket.Order"];
      _collection = new RelationDefinitionCollection ();
    }

    [Test]
    public void Add ()
    {
      _collection.Add (_relationDefinition);
      Assert.AreEqual (1, _collection.Count);
    }

    [Test]
    public void RelationDefinitionIndexer ()
    {
      _collection.Add (_relationDefinition);
      Assert.AreSame (_relationDefinition, _collection["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket.Order"]);
    }

    [Test]
    public void NumericIndexer ()
    {
      _collection.Add (_relationDefinition);
      Assert.AreSame (_relationDefinition, _collection[0]);
    }

    [Test]
    public void ContainsRelationDefinitionIDTrue ()
    {
      _collection.Add (_relationDefinition);
      Assert.IsTrue (_collection.Contains ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket.Order"));
    }

    [Test]
    public void ContainsRelationDefinitionIDFalse ()
    {
      Assert.IsFalse (_collection.Contains ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket.Order"));
    }

    [Test]
    public void ContainsRelationDefinitionTrue ()
    {
      _collection.Add (_relationDefinition);

      Assert.IsTrue (_collection.Contains (_relationDefinition));
    }

    [Test]
    public void ContainsRelationDefinitionFalse ()
    {
      _collection.Add (_relationDefinition);

      var copy = new RelationDefinition (
          _relationDefinition.ID, _relationDefinition.EndPointDefinitions[0], _relationDefinition.EndPointDefinitions[1]);

      Assert.IsFalse (_collection.Contains (copy));
    }

    [Test]
    public void CopyConstructor ()
    {
      _collection.Add (_relationDefinition);

      var copiedCollection = new RelationDefinitionCollection (_collection, false);

      Assert.AreEqual (1, copiedCollection.Count);
      Assert.AreSame (_relationDefinition, copiedCollection[0]);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "Relation 'NonExistingRelationDefinitionID' does not exist.")]
    public void GetMandatoryWithNonExistingRelationDefinitionID ()
    {
      _collection.GetMandatory ("NonExistingRelationDefinitionID");
    }

    [Test]
    public void ContainsRelationDefinition ()
    {
      _collection.Add (_relationDefinition);

      Assert.IsTrue (_collection.Contains (_relationDefinition));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsNullRelationDefinition ()
    {
      _collection.Contains ((RelationDefinition) null);
    }

    [Test]
    public void SetReadOnly ()
    {
      Assert.That (_collection.IsReadOnly, Is.False);

      _collection.SetReadOnly ();

      Assert.That (_collection.IsReadOnly, Is.True);
    }
  }
}
