using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance
{
  [TestFixture]
  public class ReflectionBasedClassDefinitionTest : TableInheritanceMappingTest
  {
    private ReflectionBasedClassDefinition _domainBaseClass;
    private ReflectionBasedClassDefinition _personClass;
    private ReflectionBasedClassDefinition _customerClass;
    private ReflectionBasedClassDefinition _addressClass;
    private ReflectionBasedClassDefinition _organizationalUnitClass;

    public override void SetUp ()
    {
      base.SetUp ();

      _domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), false, new List<Type>());
      _personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, _domainBaseClass, new List<Type>());
      _customerClass = new ReflectionBasedClassDefinition ("Customer", null, TableInheritanceTestDomainProviderID, typeof (Customer), false, _personClass, new List<Type>());
      _addressClass = new ReflectionBasedClassDefinition ("Address", "TableInheritance_Address", TableInheritanceTestDomainProviderID, typeof (Address), false, new List<Type>());

      _organizationalUnitClass = new ReflectionBasedClassDefinition (
          "OrganizationalUnit", "TableInheritance_OrganizationalUnit", TableInheritanceTestDomainProviderID, typeof (OrganizationalUnit), false, _domainBaseClass, new List<Type>());
    }

    [Test]
    public void InitializeWithNullEntityName ()
    {
      Assert.IsNull (_domainBaseClass.MyEntityName);

      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), false, new List<Type>());
      Assert.IsNull (classDefinition.MyEntityName);
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void EntityNameMustNotBeEmptyWithClassType ()
    {
      new ReflectionBasedClassDefinition ("DomainBase", string.Empty, TableInheritanceTestDomainProviderID, typeof (DomainBase), false, new List<Type>());
    }

    [Test]
    public void NullEntityNameWithDerivedClass ()
    {
      Assert.IsNull (_domainBaseClass.MyEntityName);
      Assert.IsNotNull (_personClass.MyEntityName);
      Assert.IsNull (_customerClass.MyEntityName);
    }

    [Test]
    public void GetEntityName ()
    {
      Assert.IsNull (_domainBaseClass.GetEntityName ());
      Assert.AreEqual ("TableInheritance_Person", _personClass.GetEntityName ());
      Assert.AreEqual ("TableInheritance_Person", _customerClass.GetEntityName ());
    }

    [Test]
    public void GetAllConcreteEntityNamesForConreteSingle ()
    {
      string[] entityNames = _customerClass.GetAllConcreteEntityNames ();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }

    [Test]
    public void GetAllConcreteEntityNamesForConcrete ()
    {
      string[] entityNames = _personClass.GetAllConcreteEntityNames ();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }

    [Test]
    public void GetAllConcreteEntityNamesForConreteSingleWithEntityName ()
    {
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, new List<Type>());
      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition ("Customer", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass, new List<Type>());

      string[] entityNames = customerClass.GetAllConcreteEntityNames ();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }

    [Test]
    public void GetAllConcreteEntityNamesForAbstractClass ()
    {
      string[] entityNames = _domainBaseClass.GetAllConcreteEntityNames ();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (2, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
      Assert.AreEqual ("TableInheritance_OrganizationalUnit", entityNames[1]);
    }

    [Test]
    public void GetAllConcreteEntityNamesForAbstractClassWithSameEntityNameInInheritanceHierarchy ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), false, new List<Type>());
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass, new List<Type>());
      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition ("Customer", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass, new List<Type>());

      string[] entityNames = domainBaseClass.GetAllConcreteEntityNames ();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'OtherName' of class 'OrganizationalUnit' must not define storage specific name 'NameColumn', because class 'Person' "
        + "in same inheritance hierarchy already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithDerivationInDifferentEntitiesAndMatchingColumnNames ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true, new List<Type>());
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass, new List<Type>());
      ReflectionBasedClassDefinition organizationalUnit = new ReflectionBasedClassDefinition ("OrganizationalUnit", "TableInheritance_OrganizationalUnit", TableInheritanceTestDomainProviderID, typeof (OrganizationalUnit), false, domainBaseClass, new List<Type>());

      personClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(personClass, "Name", "NameColumn", typeof (string), 100));
      organizationalUnit.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(organizationalUnit, "OtherName", "NameColumn", typeof (string), 100));

      domainBaseClass.ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'OtherName' of class 'OrganizationalUnit' must not define storage specific name 'NameColumn', because class 'Person' "
        + "in same inheritance hierarchy already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithParallelDerivationInSameEntitiesAndMatchingColumnNames ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", "TableInheritance_DomainBase", TableInheritanceTestDomainProviderID, typeof (DomainBase), true, new List<Type>());
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", null, TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass, new List<Type>());
      ReflectionBasedClassDefinition organizationalUnit = new ReflectionBasedClassDefinition ("OrganizationalUnit", null, TableInheritanceTestDomainProviderID, typeof (OrganizationalUnit), false, domainBaseClass, new List<Type>());

      personClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(personClass, "Name", "NameColumn", typeof (string), 100));
      organizationalUnit.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(organizationalUnit, "OtherName", "NameColumn", typeof (string), 100));

      domainBaseClass.ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
       "Property 'OtherName' of class 'Person' must not define storage specific name 'NameColumn', because class 'DomainBase' "
       + "in same inheritance hierarchy already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithDerivationInSameEntityAndDuplicateColumnName ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true, new List<Type>());
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass, new List<Type>());

      domainBaseClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(domainBaseClass, "Name", "NameColumn", typeof (string), 100));
      personClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(personClass, "OtherName", "NameColumn", typeof (string), 100));

      domainBaseClass.ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
       "Property 'OtherName' of class 'Customer' must not define storage specific name 'NameColumn', because class 'DomainBase' "
       + "in same inheritance hierarchy already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithDerivationInSameEntityAndDuplicateColumnNameInBaseOfBaseClass ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true, new List<Type>());
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass, new List<Type>());
      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition ("Customer", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass, new List<Type>());

      domainBaseClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(domainBaseClass, "Name", "NameColumn", typeof (string), 100));
      customerClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(customerClass, "OtherName", "NameColumn", typeof (string), 100));

      domainBaseClass.ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
       "Property 'OtherName' of class 'Person' must not define storage specific name 'NameColumn', because class 'DomainBase' "
       + "in same inheritance hierarchy already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithDerivationInUndefinedEntityAndDuplicateColumnName ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true, new List<Type>());
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", null, TableInheritanceTestDomainProviderID, typeof (Person), true, domainBaseClass, new List<Type>());

      domainBaseClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(domainBaseClass, "Name", "NameColumn", typeof (string), 100));
      personClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(personClass, "OtherName", "NameColumn", typeof (string), 100));

      domainBaseClass.ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }
  }
}
