// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Queries;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DataReaders
{
  [TestFixture]
  public class QueryResultRowReaderTest
  {
    private IStorageTypeInformationProvider _storageTypeInformationProviderStub;
    private QueryResultRowReader _queryResultRowReader;
    private IDataReader _dataReaderStrictMock;

    [SetUp]
    public void SetUp ()
    {
      _storageTypeInformationProviderStub = MockRepository.GenerateStub<IStorageTypeInformationProvider> ();
      _dataReaderStrictMock = MockRepository.GenerateStrictMock<IDataReader>();

      _queryResultRowReader = new QueryResultRowReader (_storageTypeInformationProviderStub);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_queryResultRowReader.StorageTypeInformationProvider, Is.SameAs (_storageTypeInformationProviderStub));
    }

    [Test]
    public void Read ()
    {
      _dataReaderStrictMock.Expect (mock => mock.Read ()).Return (true);
      _dataReaderStrictMock.Replay();

      var result = _queryResultRowReader.Read (_dataReaderStrictMock);

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.TypeOf (typeof (QueryResultRow)));
      CheckQueryResultRow(result);
      _dataReaderStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void Read_NoData ()
    {
      _dataReaderStrictMock.Expect (mock => mock.Read()).Return (false);
      _dataReaderStrictMock.Replay();

      var result = _queryResultRowReader.Read (_dataReaderStrictMock);

      Assert.That (result, Is.Null);
      _dataReaderStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void ReadSequence ()
    {
      _dataReaderStrictMock.Expect (mock => mock.Read ()).Return (true).Repeat.Times (3);
      _dataReaderStrictMock.Expect (mock => mock.Read ()).Return (false).Repeat.Once ();
      _dataReaderStrictMock.Replay();

      var result = _queryResultRowReader.ReadSequence (_dataReaderStrictMock).OfType<QueryResultRow>().ToList();

      Assert.That (result, Is.Not.Null);
      Assert.That (result.Count, Is.EqualTo (3));
      CheckQueryResultRow (result[0]);
      CheckQueryResultRow (result[1]);
      _dataReaderStrictMock.VerifyAllExpectations();

    }

    private void CheckQueryResultRow (IQueryResultRow result)
    {
      Assert.That (((QueryResultRow) result).StorageTypeInformationProvider, Is.SameAs (_storageTypeInformationProviderStub));
      Assert.That (((QueryResultRow) result).DataReader, Is.SameAs (_dataReaderStrictMock));
    }

  }
}