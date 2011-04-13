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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Model;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class ClassDefinitionCollectionTest : MappingReflectionTestBase
  {
    // types

    // static members and constants

    // member fields

    private ClassDefinitionCollection _collection;
    private ClassDefinition _classDefinition;

    // construction and disposing

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "Order", "Order", UnitTestDomainStorageProviderDefinition, typeof (Order), false);
      _classDefinition.SetStorageEntity (
          new TableDefinition (
              UnitTestDomainStorageProviderDefinition,
              new EntityNameDefinition (null, "Order"),
              new EntityNameDefinition (null, "OrderView"),
              new SimpleColumnDefinition[0],
              new ITableConstraintDefinition[0],
              new IIndexDefinition[0]));
      _collection = new ClassDefinitionCollection();
    }

    [Test]
    public void Initialize ()
    {
      Assert.IsTrue (_collection.AreResolvedTypesRequired);
    }

    [Test]
    public void AddWithResolvedType ()
    {
      Assert.AreEqual (0, _collection.Count);

      _collection.Add (_classDefinition);

      Assert.AreEqual (1, _collection.Count);
    }

    [Test]
    public void AddTwiceWithSameType ()
    {
      _collection.Add (_classDefinition);

      try
      {
        _collection.Add (
            ClassDefinitionFactory.CreateClassDefinition (
                "OtherID", "OtherTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false));
        Assert.Fail ("Expected an ArgumentException.");
      }
      catch (ArgumentException e)
      {
        Assert.AreEqual (
            "A ClassDefinition with Type 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order' is already part of this collection.\r\nParameter name: value",
            e.Message);

        Assert.IsFalse (_collection.Contains ("OtherID"));
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "Class 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order' and "
                          +
                          "'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer' both have the same class ID 'Order'. Use the ClassIDAttribute to define "
                          + "unique IDs for these classes. The assemblies involved are 'Remotion.Data.UnitTests, Version=.*, Culture=neutral, "
                          + "PublicKeyToken=.*' and 'Remotion.Data.UnitTests, Version=.*, Culture=neutral, "
                          + "PublicKeyToken=.*'.", MatchType = MessageMatch.Regex)]
    public void AddTwiceWithSameClassID ()
    {
      _collection.Add (_classDefinition);
      _collection.Add (
          ClassDefinitionFactory.CreateClassDefinition (
              "Order", "Order", UnitTestDomainStorageProviderDefinition, typeof (Customer), false));
    }

    [Test]
    public void TypeIndexer ()
    {
      _collection.Add (_classDefinition);
      Assert.AreSame (_classDefinition, _collection[typeof (Order)]);
    }

    [Test]
    public void NumericIndexer ()
    {
      _collection.Add (_classDefinition);
      Assert.AreSame (_classDefinition, _collection[0]);
    }

    [Test]
    public void ContainsClassTypeTrue ()
    {
      _collection.Add (_classDefinition);
      Assert.IsTrue (_collection.Contains (typeof (Order)));
    }

    [Test]
    public void ContainsClassTypeFalse ()
    {
      Assert.IsFalse (_collection.Contains (typeof (Order)));
    }

    [Test]
    public void ContainsClassDefinitionTrue ()
    {
      _collection.Add (_classDefinition);
      Assert.IsTrue (_collection.Contains (_classDefinition));
    }

    [Test]
    public void ContainsClassDefinitionFalse ()
    {
      _collection.Add (_classDefinition);

      ClassDefinition copy = ClassDefinitionFactory.CreateClassDefinition (
          _classDefinition.ID,
          StorageModelTestHelper.GetEntityName (_classDefinition),
          UnitTestDomainStorageProviderDefinition,
          _classDefinition.ClassType,
          false,
          _classDefinition.BaseClass);

      Assert.IsFalse (_collection.Contains (copy));
    }

    [Test]
    public void CopyConstructor_IEnumerableCollection ()
    {
      var copiedCollection = new ClassDefinitionCollection (new[] { _classDefinition }, false, true);

      Assert.AreEqual (1, copiedCollection.Count);
      Assert.AreSame (_classDefinition, copiedCollection[0]);
      Assert.AreEqual (true, copiedCollection.AreResolvedTypesRequired);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "Mapping does not contain class 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.ClassDefinitionCollectionTest'.")]
    public void GetMandatoryForInvalidClass ()
    {
      FakeMappingConfiguration.Current.ClassDefinitions.GetMandatory (GetType());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Mapping does not contain class 'Zaphod'.")]
    public void GetMandatoryForInvalidClassID ()
    {
      FakeMappingConfiguration.Current.ClassDefinitions.GetMandatory ("Zaphod");
    }

    [Test]
    public void ContainsClassDefinition ()
    {
      _collection.Add (_classDefinition);

      Assert.IsTrue (_collection.Contains (_classDefinition));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsNullClassDefinition ()
    {
      _collection.Contains ((ClassDefinition) null);
    }

    [Test]
    public void ContainsClassID ()
    {
      _collection.Add (_classDefinition);
      Assert.IsTrue (_collection.Contains (_classDefinition.ID));
    }

    [Test]
    public void SetReadOnly ()
    {
      Assert.That (_collection.IsReadOnly, Is.False);

      _collection.SetReadOnly();

      Assert.That (_collection.IsReadOnly, Is.True);
    }
  }
}