using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping
{
  [TestFixture]
  public class ReflectionBasedVirtualRelationEndPointDefinitionTest : StandardMappingTest
  {
    [Test]
    public void PropertyInfo ()
    {
      ClassDefinition employeeClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Employee));
      ReflectionBasedVirtualRelationEndPointDefinition relationEndPointDefinition =
          (ReflectionBasedVirtualRelationEndPointDefinition) employeeClassDefinition.GetRelationEndPointDefinition (typeof (Employee) + ".Computer");
      Assert.AreEqual (typeof (Employee).GetProperty ("Computer"), relationEndPointDefinition.PropertyInfo);
    }
  }
}