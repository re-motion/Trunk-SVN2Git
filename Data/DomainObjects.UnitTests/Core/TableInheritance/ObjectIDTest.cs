using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance
{
  [TestFixture]
  public class ObjectIDTest : TableInheritanceMappingTest
  {
    [Test]
    public void InitializeWithAbstractType ()
    {
      try
      {
        new ObjectID (typeof (DomainBase), Guid.NewGuid ());
        Assert.Fail ("ArgumentException was expected.");
      }
      catch (ArgumentException ex)
      {
        string expectedMessage = string.Format (
            "An ObjectID cannot be constructed for abstract type '{0}' of class '{1}'.\r\nParameter name: classType",
            typeof (DomainBase).AssemblyQualifiedName, "TI_DomainBase");

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }
  }
}
