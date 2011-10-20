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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class AlreadyExistingLoadedObjectTest : StandardMappingTest
  {
    private DataContainer _dataContainer;
    private AlreadyExistingLoadedObject _loadedObject;

    public override void SetUp ()
    {
      base.SetUp ();

      _dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _dataContainer.SetDomainObject (DomainObjectMother.CreateFakeObject<Order> (_dataContainer.ID));
      ClientTransactionTestHelper.RegisterDataContainer (ClientTransaction.CreateRootTransaction (), _dataContainer);

      _loadedObject = new AlreadyExistingLoadedObject (_dataContainer);
    }
    
    [Test]
    public void Initialization ()
    {
      Assert.That (_loadedObject.ExistingDataContainer, Is.SameAs (_dataContainer));
      Assert.That (_loadedObject.ObjectID, Is.EqualTo (_dataContainer.ID));
    }

    [Test]
    public void Initialization_WithoutClientTransaction_Throws ()
    {
      var existingDataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      existingDataContainer.SetDomainObject (DomainObjectMother.CreateFakeObject<Order> (existingDataContainer.ID));
      
      Assert.That (
          () => new AlreadyExistingLoadedObject (existingDataContainer),
          Throws.ArgumentException.With.Message.EqualTo (
              "The DataContainer must have been registered with a ClientTransaction.\r\nParameter name: existingDataContainer"));
    }
    
    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<ILoadedObjectVisitor>();
      visitorMock.Expect (mock => mock.VisitAlreadyExistingLoadedObject (_loadedObject));
      visitorMock.Replay();

      _loadedObject.Accept (visitorMock);

      visitorMock.VerifyAllExpectations();
    }
  }
}