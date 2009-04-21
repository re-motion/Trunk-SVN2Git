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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class ClassDefinitionCollectionTest : StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    private ClassDefinitionCollection _collection;
    private ReflectionBasedClassDefinition _classDefinition;

    // construction and disposing

    public ClassDefinitionCollectionTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "Order", c_testDomainProviderID, typeof (Order), false);
      _collection = new ClassDefinitionCollection ();
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
        _collection.Add (ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("OtherID", "OtherTable", c_testDomainProviderID, typeof (Order), false));
        Assert.Fail ("Expected an ArgumentException.");
      }
      catch (ArgumentException e)
      {
        Assert.AreEqual (
            "A ClassDefinition with Type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order' is already part of this collection.\r\nParameter name: value",
            e.Message);

        Assert.IsFalse (_collection.Contains ("OtherID"));
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Class 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order' and "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer' both have the same class ID 'Order'. Use the ClassIDAttribute to define "
        + "unique IDs for these classes. The assemblies involved are 'Remotion.Data.UnitTests, Version=.*, Culture=neutral, "
        + "PublicKeyToken=.*' and 'Remotion.Data.UnitTests, Version=.*, Culture=neutral, "
        + "PublicKeyToken=.*'.", MatchType = MessageMatch.Regex)]
    public void AddTwiceWithSameClassID ()
    {
      _collection.Add (_classDefinition);
      _collection.Add (ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "Order", c_testDomainProviderID, typeof (Customer), false));
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

      ReflectionBasedClassDefinition copy = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (_classDefinition.ID, _classDefinition.MyEntityName, _classDefinition.StorageProviderID, _classDefinition.ClassType, false, _classDefinition.BaseClass);

      Assert.IsFalse (_collection.Contains (copy));
    }

    [Test]
    public void CopyConstructor ()
    {
      _collection.Add (_classDefinition);

      ClassDefinitionCollection copiedCollection = new ClassDefinitionCollection (_collection, false);

      Assert.AreEqual (1, copiedCollection.Count);
      Assert.AreSame (_classDefinition, copiedCollection[0]);
      Assert.AreEqual (_collection.AreResolvedTypesRequired, copiedCollection.AreResolvedTypesRequired);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "Mapping does not contain class 'Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.ClassDefinitionCollectionTest'.")]
    public void GetMandatoryForInvalidClass ()
    {
      TestMappingConfiguration.Current.ClassDefinitions.GetMandatory (this.GetType ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Mapping does not contain class 'Zaphod'.")]
    public void GetMandatoryForInvalidClassID ()
    {
      TestMappingConfiguration.Current.ClassDefinitions.GetMandatory ("Zaphod");
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
  }
}
