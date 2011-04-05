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
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.InvalidObjects
{
  [TestFixture]
  public class SubInvalidDomainObjectManagerTest : StandardMappingTest
  {
    private DomainObject _domainObject1;
    private DomainObject _domainObject2;
    private SubInvalidDomainObjectManager _manager;
    private IInvalidDomainObjectManager _parentManagerMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _domainObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      _domainObject2 = DomainObjectMother.CreateFakeObject<Order> ();
      _parentManagerMock = MockRepository.GenerateStrictMock<IInvalidDomainObjectManager> ();
      _manager = new SubInvalidDomainObjectManager (_parentManagerMock);
    }

    [Test]
    public void Serializable ()
    {
      var parentManager = new RootInvalidDomainObjectManager ();
      var subManager = new SubInvalidDomainObjectManager (parentManager);

      parentManager.MarkInvalid (_domainObject1);
      subManager.MarkInvalid (_domainObject2);
      
      var deserializedInstance = Serializer.SerializeAndDeserialize (subManager);

      Assert.That (deserializedInstance.IsInvalid (_domainObject2.ID), Is.True);
      Assert.That (deserializedInstance.ParentTransactionManager.IsInvalid (_domainObject1.ID), Is.True);
    }

    [Test]
    public void MarkInvalidThroughHierarchy ()
    {
      _parentManagerMock.Expect (mock => mock.MarkInvalidThroughHierarchy (_domainObject1));
      _parentManagerMock.Replay ();

      _manager.MarkInvalidThroughHierarchy (_domainObject1);

      Assert.That (_manager.IsInvalid (_domainObject1.ID), Is.True);
      _parentManagerMock.VerifyAllExpectations();
    }
  }
}