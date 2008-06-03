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

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping
{
  [TestFixture]
  public class RelationDefinitionCollectionTest : StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    private RelationDefinitionCollection _collection;
    private RelationDefinition _relationDefinition;

    // construction and disposing

    public RelationDefinitionCollectionTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _relationDefinition = TestMappingConfiguration.Current.RelationDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"];
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
      Assert.AreSame (_relationDefinition, _collection["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"]);
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
      Assert.IsTrue (_collection.Contains ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));
    }

    [Test]
    public void ContainsRelationDefinitionIDFalse ()
    {
      Assert.IsFalse (_collection.Contains ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));
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

      RelationDefinition copy = new RelationDefinition (
          _relationDefinition.ID, _relationDefinition.EndPointDefinitions[0], _relationDefinition.EndPointDefinitions[1]);

      Assert.IsFalse (_collection.Contains (copy));
    }

    [Test]
    public void CopyConstructor ()
    {
      _collection.Add (_relationDefinition);

      RelationDefinitionCollection copiedCollection = new RelationDefinitionCollection (_collection, false);

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
  }
}
