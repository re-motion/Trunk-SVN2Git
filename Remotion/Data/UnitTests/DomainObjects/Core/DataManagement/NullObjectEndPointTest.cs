// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class NullObjectEndPointTest : ClientTransactionBaseTest
  {
    private IRelationEndPointDefinition _realEndPointDefinition;
    private NullObjectEndPoint _realEndPoint;

    public override void SetUp ()
    {
      base.SetUp();
      _realEndPointDefinition = DomainObjectIDs.OrderTicket1.ClassDefinition.GetRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order");
      _realEndPoint = new NullObjectEndPoint (_realEndPointDefinition);
    }

    [Test]
    public void Touch ()
    {
      _realEndPoint.Touch();
      Assert.That (_realEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetOppositeObjectID ()
    {
      Assert.That (_realEndPoint.OppositeObjectID, Is.Null);
      _realEndPoint.OppositeObjectID = DomainObjectIDs.Order3;
      Assert.That (_realEndPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Order3));
    }

    [Test]
    public void CreateRemoveModification ()
    {
      var removedRelatedObject = Order.GetObject (DomainObjectIDs.Order3);
      var modification = _realEndPoint.CreateRemoveModification (removedRelatedObject);
      Assert.That (modification, Is.InstanceOfType (typeof (NullEndPointModification)));
      Assert.That (modification.OldRelatedObject, Is.SameAs (removedRelatedObject));
      Assert.That (modification.NewRelatedObject, Is.Null);
    }

    [Test]
    public void CreateSelfReplaceModification ()
    {
      var newRelatedObject = Order.GetObject (DomainObjectIDs.Order3);
      var modification = _realEndPoint.CreateSelfReplaceModification (newRelatedObject);
      Assert.That (modification, Is.InstanceOfType (typeof (NullEndPointModification)));
      Assert.That (modification.OldRelatedObject, Is.SameAs (newRelatedObject));
      Assert.That (modification.NewRelatedObject, Is.SameAs (newRelatedObject));
    }

    [Test]
    public void CreateSetModification ()
    {
      var newRelatedObject = Order.GetObject (DomainObjectIDs.Order3);
      var modification = _realEndPoint.CreateSetModification (newRelatedObject);
      Assert.That (modification, Is.InstanceOfType (typeof (NullEndPointModification)));
      Assert.That (modification.OldRelatedObject, Is.Null);
      Assert.That (modification.NewRelatedObject, Is.SameAs (newRelatedObject));
    }
  }
}