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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Transport;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transport
{
  [TestFixture]
  public class TransportTransactionListenerTest : ClientTransactionBaseTest
  {
    private DomainObjectTransporter _transporter;
    private TransportTransactionListener _listener;

    public override void SetUp ()
    {
      base.SetUp ();
      _transporter = new DomainObjectTransporter();
      _listener = new TransportTransactionListener (_transporter);
    }

    [Test]
    public void Serializable ()
    {
      Serializer.SerializeAndDeserialize (_listener);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot use the transported transaction for changing properties after " 
        + "it has been deserialized.")]
    public void Serialization_AndMethodCalled ()
    {
      TransportTransactionListener listener = Serializer.SerializeAndDeserialize (_listener);
      listener.PropertyValueChanging (null, null, null, null);
    }

    [Test]
    public void ModifyingProperty_Loaded ()
    {
      _transporter.Load (DomainObjectIDs.Computer1);

      Computer source = (Computer) _transporter.GetTransportedObject(DomainObjectIDs.Computer1);
      _listener.PropertyValueChanging (source.InternalDataContainer,
          source.InternalDataContainer.PropertyValues[MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (Computer), "SerialNumber")], null, null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Object 'Computer|c7c26bf5-871d-48c7-822a-e9b05aac4e5a|System.Guid' " 
        + "cannot be modified for transportation because it hasn't been loaded yet. Load it before manipulating it.")]
    public void ModifyingProperty_NotLoaded ()
    {
      _transporter.Load (DomainObjectIDs.Computer2);

      Computer source = _transporter.GetTransportedObject (DomainObjectIDs.Computer2)
          .ClientTransaction.GetObjects<Computer> (DomainObjectIDs.Computer1)[0];
      _listener.PropertyValueChanging (source.InternalDataContainer,
          source.InternalDataContainer.PropertyValues[MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (Computer), "SerialNumber")], null, null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The transport transaction cannot be committed.")]
    public void CommitingTransaction ()
    {
      _transporter.Load (DomainObjectIDs.Computer2).ClientTransaction.Commit();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The transport transaction cannot be rolled back.")]
    public void RollingBackTransaction ()
    {
      _transporter.Load (DomainObjectIDs.Computer2).ClientTransaction.Rollback();
    }
  }
}
