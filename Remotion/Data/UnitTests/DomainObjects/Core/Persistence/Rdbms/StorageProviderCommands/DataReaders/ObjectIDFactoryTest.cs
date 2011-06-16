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
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.DataReaders;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.StorageProviderCommands.DataReaders
{
  [TestFixture]
  public class ObjectIDFactoryTest : SqlProviderBaseTest
  {
    private MockRepository _mockRepository;
    private IDataReader _dataReaderMock;
    private ObjectIDFactory _factory;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();
      _dataReaderMock = _mockRepository.StrictMock<IDataReader> ();
      _factory = new ObjectIDFactory (Provider.CreateValueConverter ());
    }

    [Test]
    public void CreateObjectID ()
    {
      SetupObject (DomainObjectIDs.OrderTicket1, null);
      _mockRepository.ReplayAll();

      var result = _factory.CreateObjectID (_dataReaderMock);

      Assert.That (result, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      _mockRepository.VerifyAll();
    }

    [Test]
    public void CreateOrderObjectIDCollection ()
    {
      SetupObject (DomainObjectIDs.OrderTicket1, true);
      SetupObject (DomainObjectIDs.OrderTicket2, true);
      SetupObject (DomainObjectIDs.OrderTicket3, false);

      _mockRepository.ReplayAll ();

      var result = _factory.CreateObjectIDCollection (_dataReaderMock);

      Assert.That (result, Is.EqualTo(new[]{DomainObjectIDs.OrderTicket1, DomainObjectIDs.OrderTicket2}));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void CreateOrderObjectIDCollection_NoData ()
    {
      var result = _factory.CreateObjectIDCollection (_dataReaderMock);

      Assert.That (result, Is.Empty);
    }

    private void SetupObject (ObjectID id, bool? readData)
    {
      using (_mockRepository.Unordered ())
      {
        if (readData.HasValue)
        {
          Expect.Call (_dataReaderMock.Read ()).Return (readData.Value);
          if(!readData.Value) return;
        }
        Expect.Call (_dataReaderMock.GetOrdinal ("ID")).Return (0);
        Expect.Call (_dataReaderMock.GetOrdinal ("ClassID")).Return (1);
        Expect.Call (_dataReaderMock.IsDBNull (1)).Return (false);
        Expect.Call (_dataReaderMock.GetString (1)).Return (id.ClassID);
        Expect.Call (_dataReaderMock.GetValue (0)).Return (id.Value);
      }
    }
  }
}