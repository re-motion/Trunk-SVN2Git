using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Persistence.Rdbms
{
  [TestFixture]
  public class ObjectIDLoaderTest : SqlProviderBaseTest
  {
    private ObjectIDLoader _loader;

    public override void SetUp ()
    {
      base.SetUp ();
      Provider.Connect ();

      _loader = new ObjectIDLoader (Provider);
    }

    public override void TearDown ()
    {
      base.TearDown ();
      Provider.Disconnect ();
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreSame (Provider, _loader.Provider);
    }

    [Test]
    public void LoadObjectIDsFromCommandBuilder ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem));
      PropertyDefinition propertyDefinition = classDefinition.GetMandatoryPropertyDefinition (ReflectionUtility.GetPropertyName (typeof (OrderItem), "Order"));
      UnionSelectCommandBuilder builder = UnionSelectCommandBuilder.CreateForRelatedIDLookup (
          Provider, classDefinition, propertyDefinition, DomainObjectIDs.Order1);
      List<ObjectID> objectIDs = _loader.LoadObjectIDsFromCommandBuilder (builder);
      Assert.That (objectIDs, Is.EquivalentTo (new ObjectID[] { DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2 }));
    }
  }
}