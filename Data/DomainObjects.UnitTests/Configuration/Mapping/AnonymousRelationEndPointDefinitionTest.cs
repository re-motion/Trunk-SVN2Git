using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class AnonymousRelationEndPointDefinitionTest : StandardMappingTest
  {
    private ClassDefinition _clientDefinition;
    private AnonymousRelationEndPointDefinition _definition;

    public override void SetUp ()
    {
      base.SetUp ();

      _clientDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Client));
      _definition = new AnonymousRelationEndPointDefinition (_clientDefinition);
    }

    [Test]
    public void Initialize ()
    {

      Assert.IsNotNull (_definition as IRelationEndPointDefinition);
      Assert.IsNotNull (_definition as INullObject);
      Assert.AreSame (_clientDefinition, _definition.ClassDefinition);
      Assert.AreEqual (CardinalityType.Many, _definition.Cardinality);
      Assert.AreEqual (false, _definition.IsMandatory);
      Assert.AreEqual (true, _definition.IsVirtual);
      Assert.IsNull (_definition.PropertyName);
      Assert.IsNull (_definition.PropertyType);
      Assert.AreEqual (_clientDefinition.IsClassTypeResolved, _definition.IsPropertyTypeResolved);
      Assert.IsNull (_definition.PropertyTypeName);
      Assert.IsTrue (_definition.IsNull);
    }

    [Test]
    public void CorrespondsToTrue ()
    {
      Assert.IsTrue (_definition.CorrespondsTo (_clientDefinition.ID, null));
    }

    [Test]
    public void CorrespondsToFalse ()
    {
      Assert.IsFalse (_definition.CorrespondsTo (_clientDefinition.ID, "PropertyName"));
    }

    [Test]
    public void RelationDefinitionNull ()
    {
      AnonymousRelationEndPointDefinition definition = new AnonymousRelationEndPointDefinition (MappingConfiguration.Current.ClassDefinitions[typeof (Client)]);

      Assert.IsNull (definition.RelationDefinition);
    }

    [Test]
    public void RelationDefinitionNotNull ()
    {
      RelationEndPointDefinition oppositeEndPoint = new RelationEndPointDefinition (
          MappingConfiguration.Current.ClassDefinitions[typeof (Location)], "Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client", true);

      RelationDefinition relationDefinition = new RelationDefinition ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client", _definition, oppositeEndPoint);

      Assert.IsNotNull (_definition.RelationDefinition);
    }
  }
}
