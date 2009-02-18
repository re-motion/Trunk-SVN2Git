// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class NullObjectEndPointTest : StandardMappingTest
  {
    private IRelationEndPointDefinition _realEndPointDefinition;
    private NullObjectEndPoint _realEndPoint;

    public override void SetUp ()
    {
      base.SetUp ();
      _realEndPointDefinition = DomainObjectIDs.OrderTicket1.ClassDefinition.GetRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order");
      _realEndPoint = new NullObjectEndPoint (_realEndPointDefinition);
    }

    [Test]
    public void Touch ()
    {
      _realEndPoint.Touch ();
      Assert.That (_realEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetOppositeObjectID ()
    {
      Assert.That (_realEndPoint.OppositeObjectID, Is.Null);
      _realEndPoint.OppositeObjectID = DomainObjectIDs.Order3;
      Assert.That (_realEndPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Order3));
    }
  }
}