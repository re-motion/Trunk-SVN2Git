/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class AnonymousEndPointTest : ClientTransactionBaseTest
  {
    private Client _client;
    private RelationDefinition _clientToLocationDefinition;
    private IRelationEndPointDefinition _clientEndPointDefinition;
    private IRelationEndPointDefinition _locationEndPointDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _client = Client.GetObject (DomainObjectIDs.Client3);
      _clientToLocationDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (Location)].GetRelationDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");
      _clientEndPointDefinition = _clientToLocationDefinition.GetEndPointDefinition ("Client", null);
      _locationEndPointDefinition = _clientToLocationDefinition.GetEndPointDefinition ("Location", "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");
    }

    [Test]
    public void InitializeWithDomainObject ()
    {
      AnonymousEndPoint endPoint = new AnonymousEndPoint (ClientTransactionMock, _client, _clientToLocationDefinition);

      Assert.IsNotNull (endPoint as INullObject);
      Assert.IsNotNull (endPoint as IEndPoint);
      Assert.IsFalse (endPoint.IsNull);
      Assert.AreSame (ClientTransactionMock, endPoint.ClientTransaction);
      Assert.AreSame (_client, endPoint.GetDomainObject ());
      Assert.AreSame (_client.InternalDataContainer, endPoint.GetDataContainer ());
      Assert.AreEqual (_client.ID, endPoint.ObjectID);

      Assert.AreSame (_clientToLocationDefinition, endPoint.RelationDefinition);
      Assert.AreSame (_clientEndPointDefinition, endPoint.Definition);
      Assert.IsNotNull (endPoint.Definition as AnonymousRelationEndPointDefinition);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The provided relation definition must contain a AnonymousRelationEndPointDefinition.\r\nParameter name: relationDefinition")]
    public void InitializeWithInvalidRelationDefinition ()
    {
      RelationDefinition invalidRelationDefinition = MappingConfiguration.Current.RelationDefinitions.GetMandatory ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");
      AnonymousEndPoint endPoint = new AnonymousEndPoint (ClientTransactionMock, Order.GetObject (DomainObjectIDs.Order1), invalidRelationDefinition);
    }

    [Test]
    public void GetDataContainerUsesStoredTransaction ()
    {
      AnonymousEndPoint endPoint = new AnonymousEndPoint (ClientTransactionMock, _client, _clientToLocationDefinition);
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        Assert.AreSame (ClientTransactionMock, endPoint.GetDataContainer ().ClientTransaction);
      }
    }
  }
}
