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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class SerializedObjectIDStoragePropertyDefinitionTest : StandardMappingTest
  {
    private IRdbmsStoragePropertyDefinition _serializedIDPropertyStub;
    private SerializedObjectIDStoragePropertyDefinition _serializedObjectIDStoragePropertyDefinition;
    private IDataReader _dataReaderStub;
    private IColumnOrdinalProvider _columnOrdinalProviderStub;
    private IDbCommand _dbCommandStub;
    private IDbDataParameter _dbDataParameterStub;
    private ColumnDefinition _columnDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _columnDefinition = ColumnDefinitionObjectMother.CreateColumn();

      _serializedIDPropertyStub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
      _serializedIDPropertyStub.Stub (stub => stub.Name).Return ("ID");
      _serializedIDPropertyStub.Stub (stub => stub.GetColumnForLookup()).Return (_columnDefinition);
      _serializedIDPropertyStub.Stub (stub => stub.GetColumnForForeignKey()).Return (_columnDefinition);
      _serializedIDPropertyStub.Stub (stub => stub.GetColumns()).Return (new[] { _columnDefinition });

      _serializedObjectIDStoragePropertyDefinition = new SerializedObjectIDStoragePropertyDefinition (_serializedIDPropertyStub);

      _dataReaderStub = MockRepository.GenerateStub<IDataReader>();
      _columnOrdinalProviderStub = MockRepository.GenerateStub<IColumnOrdinalProvider>();
      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      _dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter>();
      _dbCommandStub.Stub (stub => stub.CreateParameter()).Return (_dbDataParameterStub).Repeat.Once();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_serializedObjectIDStoragePropertyDefinition.SerializedIDProperty, Is.SameAs (_serializedIDPropertyStub));
    }

    [Test]
    public void GetColumnForLookup ()
    {
      Assert.That (_serializedObjectIDStoragePropertyDefinition.GetColumnForLookup(), Is.SameAs (_columnDefinition));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "String-serialized ObjectID values cannot be used as foreign keys.")]
    public void GetColumnForForeignKey ()
    {
      _serializedObjectIDStoragePropertyDefinition.GetColumnForForeignKey();
    }

    [Test]
    public void GetColumns ()
    {
      Assert.That (_serializedObjectIDStoragePropertyDefinition.GetColumns(), Is.EqualTo (_serializedIDPropertyStub.GetColumns()));
    }

    [Test]
    public void Name ()
    {
      Assert.That (_serializedObjectIDStoragePropertyDefinition.Name, Is.EqualTo (_serializedIDPropertyStub.Name));
    }

    [Test]
    public void Read ()
    {
      _serializedIDPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _columnOrdinalProviderStub)).Return (DomainObjectIDs.Order1.ToString());

      var result = _serializedObjectIDStoragePropertyDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);

      Assert.That (result, Is.TypeOf (typeof (ObjectID)));
      Assert.That (((ObjectID) result).Value.ToString(), Is.EqualTo (DomainObjectIDs.Order1.Value.ToString()));
      Assert.That (((ObjectID) result).ClassID, Is.EqualTo ("Order"));
    }

    [Test]
    public void Read_ValueIsNull_ReturnsNull ()
    {
      _serializedIDPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _columnOrdinalProviderStub)).Return (null);

      var result = _serializedObjectIDStoragePropertyDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void SplitValue ()
    {
      var columnValue = new ColumnValue (_columnDefinition, DomainObjectIDs.OrderItem1);

      _serializedIDPropertyStub.Stub (stub => stub.SplitValue (DomainObjectIDs.OrderItem1.ToString())).Return (new[] { columnValue });

      var result = _serializedObjectIDStoragePropertyDefinition.SplitValue (DomainObjectIDs.OrderItem1);

      Assert.That (result, Is.EqualTo (new[] { columnValue }));
    }

    [Test]
    public void SplitValue_NullObjectID ()
    {
      var columnValue = new ColumnValue (_columnDefinition, DomainObjectIDs.OrderItem1);

      _serializedIDPropertyStub.Stub (stub => stub.SplitValue (null)).Return (new[] { columnValue });

      var result = _serializedObjectIDStoragePropertyDefinition.SplitValue (null);

      Assert.That (result, Is.EqualTo (new[] { columnValue }));
    }

    [Test]
    public void SplitValueForComparison ()
    {
      var columnValue1 = new ColumnValue (_columnDefinition, null);
      _serializedIDPropertyStub.Stub (stub => stub.SplitValue (DomainObjectIDs.Order1.ToString())).Return (new[] { columnValue1 });

      var result = _serializedObjectIDStoragePropertyDefinition.SplitValueForComparison (DomainObjectIDs.Order1).ToArray ();

      Assert.That (result, Is.EqualTo (new[] { columnValue1 }));
    }

    [Test]
    public void SplitValueForComparison_NullValue ()
    {
      var columnValue1 = new ColumnValue (_columnDefinition, null);
      _serializedIDPropertyStub.Stub (stub => stub.SplitValue (null)).Return (new[] { columnValue1 });

      var result = _serializedObjectIDStoragePropertyDefinition.SplitValueForComparison (null).ToArray ();

      Assert.That (result, Is.EqualTo (new[] { columnValue1 }));
    }
  }
}