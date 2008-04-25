using System;
using System.Xml.Schema;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Schemas;

namespace Remotion.Data.DomainObjects.UnitTests.Schemas
{
  [TestFixture]
  public class TypesSchemaLoaderTest
  {
    [Test]
    public void Initialize ()
    {
      Assert.AreEqual ("http://www.re-motion.org/Data/DomainObjects/Types", TypesSchemaLoader.Instance.SchemaUri);
    }

    [Test]
    public void LoadSchemaSet ()
    {
      XmlSchemaSet schemaSet = TypesSchemaLoader.Instance.LoadSchemaSet ();

      Assert.IsNotNull (schemaSet);
      Assert.AreEqual (1, schemaSet.Count);
      Assert.IsTrue (schemaSet.Contains ("http://www.re-motion.org/Data/DomainObjects/Types"));
    }
  }
}
