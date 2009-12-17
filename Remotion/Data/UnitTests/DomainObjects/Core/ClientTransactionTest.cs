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
using System.Runtime.Serialization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  [TestFixture]
  public class ClientTransactionTest : ClientTransactionBaseTest
  {
    [Test]
    public void GetObjectForDataContainer_EnlistedObject ()
    {
      var creator = (InterceptedDomainObjectCreator) MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order)).GetDomainObjectCreator ();
      var orderType = creator.Factory.GetConcreteDomainObjectType (typeof (Order));

      var enlisted = (DomainObject) FormatterServices.GetSafeUninitializedObject (orderType);
      enlisted.Initialize (DomainObjectIDs.Order1, ClientTransactionMock);
      ClientTransactionMock.EnlistDomainObject (enlisted);

      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      dataContainer.RegisterWithTransaction (ClientTransactionMock);

      var retrieved = ClientTransactionMock.GetObjectForDataContainer (dataContainer);
      Assert.That (retrieved, Is.SameAs (enlisted));
    }

    [Test]
    public void GetObjectForDataContainer_NoEnlistedObject_CreatesNew ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      dataContainer.RegisterWithTransaction (ClientTransactionMock);

      var retrieved = ClientTransactionMock.GetObjectForDataContainer (dataContainer);

      Assert.That (retrieved, Is.InstanceOfType (typeof (Order)));
      Assert.That (retrieved.ID, Is.EqualTo (dataContainer.ID));
    }

    [Test]
    public void GetObjectForDataContainer_NoEnlistedObject_UsesCreator ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      dataContainer.RegisterWithTransaction (ClientTransactionMock);

      var retrieved = ClientTransactionMock.GetObjectForDataContainer (dataContainer);

      var expectedCreator = (InterceptedDomainObjectCreator) MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order)).GetDomainObjectCreator ();
      expectedCreator.Factory.WasCreatedByFactory (((object) retrieved).GetType ());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "The data container must be registered with the ClientTransaction before an object is retrieved for it.")]
    public void GetObjectForDataContainer_NonRegisteredObject ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      ClientTransactionMock.GetObjectForDataContainer (dataContainer);
    }
  }
}