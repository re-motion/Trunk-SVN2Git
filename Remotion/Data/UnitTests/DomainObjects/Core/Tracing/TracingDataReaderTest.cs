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
    public void GetName ()
    {
      var i = 5;
      _innerDataReader.Expect (mock => mock.GetName (i)).Return("test");
      _mockRepository.ReplayAll();

      var result = _dataReader.GetName (i);

      Assert.That (result, Is.EqualTo ("test"));
      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetDataTypeName ()
    {
      var i = 5;
      _innerDataReader.Expect (mock => mock.GetDataTypeName (i)).Return ("test");
      _mockRepository.ReplayAll ();

      var result = _dataReader.GetDataTypeName (i);

      Assert.That (result, Is.EqualTo ("test"));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetFieldType ()
    {
      var i = 5;
      var expectedType = typeof(string);

      _innerDataReader.Expect (mock => mock.GetFieldType (i)).Return (expectedType);
      _mockRepository.ReplayAll ();

      var result = _dataReader.GetFieldType (i);

      Assert.That (result, Is.EqualTo (expectedType));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetValue ()
    {
      var i = 5;
      var o = "test";
      _innerDataReader.Expect (mock => mock.GetValue (i)).Return (o);
      _mockRepository.ReplayAll ();

      var result = _dataReader.GetValue (i);
      
      Assert.That (result, Is.EqualTo (o));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetValues ()
    {
      var i = 5;
      object[] values = new object[] { "1",2, "3"};
      _innerDataReader.Expect (mock => mock.GetValues (values)).Return (i);
      _mockRepository.ReplayAll ();

      var result = _dataReader.GetValues (values);
      
      Assert.That (result, Is.EqualTo (i));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetOrdinal ()
    {
      var i = 5;
      var name = "test";
      _innerDataReader.Expect (mock => mock.GetOrdinal (name)).Return (i);
      _mockRepository.ReplayAll ();

      var result = _dataReader.GetOrdinal (name);

      Assert.That (result, Is.EqualTo (i));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetBoolean ()
    {
      var i = 5;
      _innerDataReader.Expect (mock => mock.GetBoolean (i)).Return (true);
      _mockRepository.ReplayAll ();

      var result = _dataReader.GetBoolean (i);

      Assert.That (result, Is.True);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetByte ()
    {
      var i = 5;
      var b = new Byte();

      _innerDataReader.Expect (mock => mock.GetByte (i)).Return (b);
      _mockRepository.ReplayAll ();

      var result = _dataReader.GetByte (i);

      Assert.That (result, Is.EqualTo (b));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetChar ()
    {
      var i = 5;
      var c = 't';
      _innerDataReader.Expect (mock => mock.GetChar (i)).Return (c);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetChar (i);

      Assert.That (result, Is.EqualTo (c));
      _mockRepository.VerifyAll ();
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