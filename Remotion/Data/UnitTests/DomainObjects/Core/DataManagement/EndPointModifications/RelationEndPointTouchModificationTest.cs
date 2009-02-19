// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.EndPointModifications
{
  [TestFixture]
  public class RelationEndPointTouchModificationTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _id;
    private ObjectEndPoint _modifiedEndPoint;
    private RelationEndPointTouchModification _modification;
    private Employee _relatedObject;
    private Computer _domainObject;

    public override void SetUp ()
    {
      base.SetUp ();
      _domainObject = Computer.GetObject (DomainObjectIDs.Computer1);
      _id = new RelationEndPointID (
          _domainObject.ID,
          MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (Computer), "Employee"));

      _relatedObject = Employee.GetObject (DomainObjectIDs.Employee3);
      _modifiedEndPoint = new ObjectEndPoint (ClientTransactionMock, _id, _relatedObject.ID);
      _modification = new RelationEndPointTouchModification (_modifiedEndPoint);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_modification.ModifiedEndPoint, Is.SameAs (_modifiedEndPoint));
      Assert.That (_modification.OldRelatedObject, Is.Null);
      Assert.That (_modification.NewRelatedObject, Is.Null);
    }

    [Test]
    public void Begin ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      _domainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      _domainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      _modification.Begin ();

      Assert.That (relationChangingCalled, Is.False);
      Assert.That (relationChangedCalled, Is.False);
    }

    [Test]
    public void End ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      _domainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      _domainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      _modification.End ();

      Assert.That (relationChangingCalled, Is.False);
      Assert.That (relationChangedCalled, Is.False);
    }

    [Test]
    public void Perform ()
    {
      _modification.Perform ();

      Assert.That (_modifiedEndPoint.OppositeObjectID, Is.EqualTo (_relatedObject.ID));
      Assert.That (_modifiedEndPoint.HasBeenTouched, Is.True);
      Assert.That (_domainObject.InternalDataContainer.PropertyValues[_modifiedEndPoint.PropertyName].HasBeenTouched, Is.True);
    }

    [Test]
    public void CreateBidirectionalModification_WithoutOppositeEndPoints ()
    {
      var modification = new RelationEndPointTouchModification (_modifiedEndPoint);

      var steps = modification.CreateBidirectionalModification ().GetEndPointModifications ();
      Assert.That (steps.Count, Is.EqualTo (1));

      Assert.That (steps[0], Is.SameAs (modification));
    }

    [Test]
    public void CreateBidirectionalModification_WithOppositeEndPoints ()
    {
      var oppositeEndPoint = ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (
          _relatedObject, _modifiedEndPoint.OppositeEndPointDefinition);
      var modification = new RelationEndPointTouchModification (_modifiedEndPoint, oppositeEndPoint);
      
      var steps = modification.CreateBidirectionalModification ().GetEndPointModifications ();
      Assert.That (steps.Count, Is.EqualTo (2));

      Assert.That (steps[0], Is.SameAs (modification));

      Assert.That (steps[1], Is.InstanceOfType (typeof (RelationEndPointTouchModification)));
      Assert.That (steps[1].ModifiedEndPoint, Is.SameAs (oppositeEndPoint));
      Assert.That (((RelationEndPointTouchModification) steps[1]).OppositeEndPoints, Is.Empty);
    }
  }
}