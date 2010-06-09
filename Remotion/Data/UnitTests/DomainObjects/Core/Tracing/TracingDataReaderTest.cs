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
using System.Data;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Tracing;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Tracing
{
  [TestFixture]
  public class TracingDataReaderTest
  {
    private MockRepository _mockRepository;
    private IDataReader _innerDataReader;
    private IPersistenceListener _listenerMock;
    private Guid _connectionID;
    private Guid _queryID;
    private TracingDataReader _dataReader;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();
      _innerDataReader = _mockRepository.StrictMock<IDataReader>();
      _listenerMock = _mockRepository.StrictMock<IPersistenceListener> ();
      _connectionID = Guid.NewGuid ();
      _queryID = Guid.NewGuid();

      _dataReader = new TracingDataReader (_innerDataReader, _listenerMock, _connectionID, _queryID);
    }

    [Test]
    public void Dispose ()
    {
      using (_mockRepository.Ordered ())
      {
        _listenerMock.Expect (
            mock =>
            mock.QueryCompleted (
                Arg<Guid>.Matches (p => p == _connectionID),
                Arg<Guid>.Matches (p => p == _queryID),
                Arg<TimeSpan>.Matches (p => p.Milliseconds > 0),
                Arg<int>.Is.Anything));
        _innerDataReader.Expect (mock => mock.Dispose());
      }
      _mockRepository.ReplayAll();

      _dataReader.Dispose();
      _mockRepository.VerifyAll();
    }

    [Test]
    public void Close ()
    {
      using (_mockRepository.Ordered ())
      {
        _listenerMock.Expect (
            mock =>
            mock.QueryCompleted (
                Arg<Guid>.Matches (p => p == _connectionID),
                Arg<Guid>.Matches (p => p == _queryID),
                Arg<TimeSpan>.Matches (p => p.Milliseconds > 0),
                Arg<int>.Is.Anything));
        _innerDataReader.Expect (mock => mock.Close ());
      }
      _mockRepository.ReplayAll ();

      _dataReader.Close ();
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void CloseAndDispose ()
    {
      using (_mockRepository.Ordered ())
      {
        _listenerMock.Expect (
            mock =>
            mock.QueryCompleted (
                Arg<Guid>.Matches (p => p == _connectionID),
                Arg<Guid>.Matches (p => p == _queryID),
                Arg<TimeSpan>.Matches (p => p.Milliseconds > 0),
                Arg<int>.Is.Anything));
        _innerDataReader.Expect (mock => mock.Close ());
        _innerDataReader.Expect (mock => mock.Dispose ());
      }
      _mockRepository.ReplayAll ();

      _dataReader.Close ();
      _dataReader.Dispose ();

      _mockRepository.VerifyAll ();
    }
    
    [Test]
    public void Read_HasRecord ()
    {
      _innerDataReader.Expect (mock => mock.Read()).Return(true);
      _mockRepository.ReplayAll();

      var hasRecord =_dataReader.Read();
      
      _mockRepository.VerifyAll();
      Assert.That (hasRecord, Is.True);
    }

    [Test]
    public void Read_NoRecord ()
    {
      _innerDataReader.Expect (mock => mock.Read ()).Return (false);
      _mockRepository.ReplayAll ();

      var hasRecord = _dataReader.Read ();

      _mockRepository.VerifyAll ();
      Assert.That (hasRecord, Is.False);
    }

  }
}