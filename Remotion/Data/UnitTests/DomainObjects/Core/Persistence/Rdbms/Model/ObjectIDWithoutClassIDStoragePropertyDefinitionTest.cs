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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ObjectIDWithoutClassIDStoragePropertyDefinitionTest : StandardMappingTest
  {
    private ClassDefinition _classDefinition;
    private IRdbmsStoragePropertyDefinition _valuePropertyStub;
    private ObjectIDWithoutClassIDStoragePropertyDefinition _objectIDWithoutClassIDStorageDefinition;
    private IDataReader _dataReaderStub;
    private IColumnOrdinalProvider _columnOrdinalProviderStub;
    private IDbCommand _dbCommandStub;
    private IDbDataParameter _dbDataParameterStub;
    private ColumnDefinition _columnDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinition = DomainObjectIDs.Order1.ClassDefinition;

      _columnDefinition = ColumnDefinitionObjectMother.CreateColumn();

      _valuePropertyStub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
      _valuePropertyStub.Stub (stub => stub.Name).Return ("ID");
      _valuePropertyStub.Stub (stub => stub.GetColumnForLookup()).Return (_columnDefinition);
      _valuePropertyStub.Stub (stub => stub.GetColumnForForeignKey()).Return (_columnDefinition);
      _valuePropertyStub.Stub (stub => stub.GetColumns()).Return (new[] { _columnDefinition });

      _objectIDWithoutClassIDStorageDefinition = new ObjectIDWithoutClassIDStoragePropertyDefinition (
          _valuePropertyStub, _classDefinition);

      _dataReaderStub = MockRepository.GenerateStub<IDataReader>();
      _columnOrdinalProviderStub = MockRepository.GenerateStub<IColumnOrdinalProvider>();
      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      _dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter>();
      _dbCommandStub.Stub (stub => stub.CreateParameter()).Return (_dbDataParameterStub).Repeat.Once();
    }


    [Test]
    public void Initialization ()
    {
      Assert.That (_objectIDWithoutClassIDStorageDefinition.ValueProperty, Is.SameAs (_valuePropertyStub));
      Assert.That (_objectIDWithoutClassIDStorageDefinition.ClassDefinition, Is.SameAs (_classDefinition));
    }

    [Test]
    public void GetColumnForLookup ()
    {
      Assert.That (_objectIDWithoutClassIDStorageDefinition.GetColumnForLookup(), Is.SameAs (_columnDefinition));
    }

    [Test]
    public void GetColumnForForeignKey ()
    {
      Assert.That (_objectIDWithoutClassIDStorageDefinition.GetColumnForForeignKey(), Is.SameAs (_columnDefinition));
    }

    [Test]
    public void GetColumns ()
    {
      Assert.That (_objectIDWithoutClassIDStorageDefinition.GetColumns(), Is.EqualTo (new[] { _columnDefinition }));
    }

    [Test]
    public void Name ()
    {
      Assert.That (_objectIDWithoutClassIDStorageDefinition.Name, Is.EqualTo (_valuePropertyStub.Name));
    }

    [Test]
    public void Read ()
    {
      _valuePropertyStub.Stub (stub => stub.Read (_dataReaderStub, _columnOrdinalProviderStub)).Return (DomainObjectIDs.Order1.Value);

      var result = _objectIDWithoutClassIDStorageDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);

      Assert.That (result, Is.TypeOf (typeof (ObjectID)));
      Assert.That (((ObjectID) result).Value.ToString(), Is.EqualTo (DomainObjectIDs.Order1.Value.ToString()));
      Assert.That (((ObjectID) result).ClassDefinition, Is.SameAs (_classDefinition));
    }

    [Test]
    public void Read_ValueIsNull_ReturnsNull ()
    {
      _valuePropertyStub.Stub (stub => stub.Read (_dataReaderStub, _columnOrdinalProviderStub)).Return (null);

      var result = _objectIDWithoutClassIDStorageDefinition.Read (_dataReaderStub, _columnOrdinalProviderStub);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void SplitValue ()
    {
      var columnValue = new ColumnValue (_columnDefinition, DomainObjectIDs.Order1);

      _valuePropertyStub.Stub (stub => stub.SplitValue (DomainObjectIDs.Order1.Value)).Return (new[] { columnValue });

      var result = _objectIDWithoutClassIDStorageDefinition.SplitValue (DomainObjectIDs.Order1);

      Assert.That (result, Is.EqualTo (new[] { columnValue }));
    }

    [Test]
    public void SplitValue_NullValue ()
    {
      var columnValue = new ColumnValue (_columnDefinition, DomainObjectIDs.Order1);

      _valuePropertyStub.Stub (stub => stub.SplitValue (null)).Return (new[] { columnValue });

      var result = _objectIDWithoutClassIDStorageDefinition.SplitValue (null);

      Assert.That (result, Is.EqualTo (new[] { columnValue }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The specified ObjectID has an invalid ClassDefinition.\r\nParameter name: value")]
    public void SplitValue_InvalidClassDefinition ()
    {
      var columnValue = new ColumnValue (_columnDefinition, DomainObjectIDs.OrderItem1);

      _valuePropertyStub.Stub (stub => stub.SplitValue (DomainObjectIDs.OrderItem1.Value)).Return (new[] { columnValue });

      _objectIDWithoutClassIDStorageDefinition.SplitValue (DomainObjectIDs.OrderItem1);
    }

    [Test]
    public void SplitValueForComparison ()
    {
      var columnValue1 = new ColumnValue (_columnDefinition, null);
      _valuePropertyStub.Stub (stub => stub.SplitValueForComparison (DomainObjectIDs.Order1.Value)).Return (new[] { columnValue1 });

      var result = _objectIDWithoutClassIDStorageDefinition.SplitValueForComparison (DomainObjectIDs.Order1).ToArray ();

      Assert.That (result, Is.EqualTo (new[] { columnValue1 }));
    }

    [Test]
    public void SplitValueForComparison_NullValue ()
    {
      var columnValue1 = new ColumnValue (_columnDefinition, null);
      _valuePropertyStub.Stub (stub => stub.SplitValueForComparison (null)).Return (new[] { columnValue1 });

      var result = _objectIDWithoutClassIDStorageDefinition.SplitValueForComparison (null).ToArray ();

      Assert.That (result, Is.EqualTo (new[] { columnValue1 }));
    }
  }
}