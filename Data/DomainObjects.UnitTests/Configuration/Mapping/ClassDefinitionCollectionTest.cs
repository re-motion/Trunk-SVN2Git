using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins.Context;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping
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

      _classDefinition = new ReflectionBasedClassDefinition ("Order", "Order", c_testDomainProviderID, typeof (Order), false, new List<Type> ());
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
        _collection.Add (new ReflectionBasedClassDefinition ("OtherID", "OtherTable", c_testDomainProviderID, typeof (Order), false, new List<Type>()));
        Assert.Fail ("Expected an ArgumentException.");
      }
      catch (ArgumentException e)
      {
        Assert.AreEqual (
            "A ClassDefinition with Type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' is already part of this collection.\r\nParameter name: value",
            e.Message);

        Assert.IsFalse (_collection.Contains ("OtherID"));
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Class 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' and "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer' both have the same class ID 'Order'. Use the ClassIDAttribute to define "
        + "unique IDs for these classes. The assemblies involved are 'Remotion.Data.DomainObjects.UnitTests, Version=.*, Culture=neutral, "
        + "PublicKeyToken=.*' and 'Remotion.Data.DomainObjects.UnitTests, Version=.*, Culture=neutral, "
        + "PublicKeyToken=.*'.", MatchType = MessageMatch.Regex)]
    public void AddTwiceWithSameClassID ()
    {
      _collection.Add (_classDefinition);
      _collection.Add (new ReflectionBasedClassDefinition ("Order", "Order", c_testDomainProviderID, typeof (Customer), false, new List<Type>()));
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

      ReflectionBasedClassDefinition copy = new ReflectionBasedClassDefinition (
          _classDefinition.ID, _classDefinition.MyEntityName, _classDefinition.StorageProviderID, _classDefinition.ClassType, false, _classDefinition.BaseClass, new List<Type>());

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
        ExpectedMessage = "Mapping does not contain class 'Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.ClassDefinitionCollectionTest'.")]
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
