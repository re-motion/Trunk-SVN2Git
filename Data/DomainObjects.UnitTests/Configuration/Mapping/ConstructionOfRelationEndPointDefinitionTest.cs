using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class ConstructionOfRelationEndPointDefinitionTest: StandardMappingTest
  {
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "Relation definition error: Property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Name' of class 'Company' is of type "
        + "'System.String', but non-virtual properties must be of type 'Remotion.Data.DomainObjects.ObjectID'.")]
    public void PropertyOfWrongType()
    {
      ClassDefinition companyDefinition = TestMappingConfiguration.Current.ClassDefinitions[typeof (Company)];

      new RelationEndPointDefinition (companyDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Name", false);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "Relation definition error for end point: Class 'Company' has no property 'UndefinedProperty'.")]
    public void UndefinedProperty()
    {
      ClassDefinition companyDefinition = TestMappingConfiguration.Current.ClassDefinitions[typeof (Company)];

      new RelationEndPointDefinition (companyDefinition, "UndefinedProperty", false);
    }
  }
}