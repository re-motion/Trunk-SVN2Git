// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class RelationEndPointDefinitionExtensionsTest : MappingReflectionTestBase
  {
    [Test]
    public void GetOppositeEndPointDefinition ()
    {
      var endPointDefinition = DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
    
      var oppositeEndPointDefinition = endPointDefinition.GetOppositeEndPointDefinition();
      
      Assert.That (oppositeEndPointDefinition, 
          Is.SameAs (DomainObjectIDs.OrderTicket1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order")));
    }

    [Test]
    public void GetMandatoryOppositeEndPointDefinition ()
    {
      var endPointDefinition = DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");

      var oppositeEndPointDefinition = endPointDefinition.GetMandatoryOppositeEndPointDefinition ();

      Assert.That (oppositeEndPointDefinition,
          Is.SameAs (DomainObjectIDs.OrderTicket1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order")));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "Relation 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem.Order' has no association with "
        + "class 'Order' and property 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer'.")]
    public void GetMandatoryOppositeEndPointDefinition_Failure ()
    {
      var fakeEndPointDefinition = new RelationEndPointDefinition (DomainObjectIDs.Order1.ClassDefinition, typeof (Order).FullName + ".Customer", true);
      fakeEndPointDefinition.SetRelationDefinition (DomainObjectIDs.Order1.ClassDefinition.GetRelationDefinition (typeof (Order).FullName + ".OrderItems"));

      fakeEndPointDefinition.GetMandatoryOppositeEndPointDefinition ();
    }

    [Test]
    public void GetOppositeClassDefinition ()
    {
      var endPointDefinition = DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");

      var oppositeEndPointDefinition = endPointDefinition.GetOppositeClassDefinition ();

      Assert.That (oppositeEndPointDefinition, Is.SameAs (DomainObjectIDs.OrderTicket1.ClassDefinition));
    }
  }
}