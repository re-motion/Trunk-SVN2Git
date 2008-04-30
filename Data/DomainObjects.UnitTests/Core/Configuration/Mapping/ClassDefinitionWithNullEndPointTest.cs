using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping
{
  [TestFixture]
  public class ClassDefinitionWithNullEndPointTest : StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    private ClassDefinition _clientClass;
    private AnonymousRelationEndPointDefinition _clientEndPoint;
    private ClassDefinition _locationClass;
    private RelationEndPointDefinition _locationEndPoint;

    // construction and disposing

    public ClassDefinitionWithNullEndPointTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _clientClass = TestMappingConfiguration.Current.ClassDefinitions.GetMandatory ("Client");
      _locationClass = TestMappingConfiguration.Current.ClassDefinitions.GetMandatory ("Location");

      RelationDefinition relation = TestMappingConfiguration.Current.RelationDefinitions.GetMandatory ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client");
      _clientEndPoint = (AnonymousRelationEndPointDefinition) relation.EndPointDefinitions[0];
      _locationEndPoint = (RelationEndPointDefinition) relation.EndPointDefinitions[1];
    }

    [Test]
    public void GetRelationDefinitions ()
    {
      Assert.IsTrue (_locationClass.GetRelationDefinitions ().Contains ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client"));
      Assert.IsFalse (_clientClass.GetRelationDefinitions ().Contains ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client"));
    }

    [Test]
    public void GetRelationEndPointDefinitions ()
    {
      Assert.IsTrue (Contains (_locationClass.GetRelationEndPointDefinitions (), _locationEndPoint));
      Assert.IsFalse (Contains (_clientClass.GetRelationEndPointDefinitions (), _clientEndPoint));
    }

    [Test]
    public void GetMyRelationEndPointDefinitions ()
    {
      Assert.IsTrue (Contains (_locationClass.GetMyRelationEndPointDefinitions (), _locationEndPoint));
      Assert.IsFalse (Contains (_clientClass.GetMyRelationEndPointDefinitions (), _clientEndPoint));
    }

    private bool Contains (IRelationEndPointDefinition[] endPoints, IRelationEndPointDefinition value)
    {
      foreach (IRelationEndPointDefinition endPoint in endPoints)
      {
        if (ReferenceEquals (endPoint, value))
          return true;
      }

      return false;
    }
  }
}
