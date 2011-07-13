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
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DataReaders
{
  [TestFixture]
  public class DictionaryBasedColumnOrdinalProviderTest
  {
    private ColumnDefinition _columnDefinition;
    private DictionaryBasedColumnOrdinalProvider _dictionaryBasedColumnOrdinalProvider;
    private IDataReader _dataReaderStub;
    private Dictionary<ColumnDefinition, int> _ordinals;

    [SetUp]
    public void SetUp ()
    {
      _columnDefinition = ColumnDefinitionObjectMother.CreateColumn ("Testcolumn");
      _dataReaderStub = MockRepository.GenerateStub<IDataReader> ();
      _ordinals = new Dictionary<ColumnDefinition, int>();
      _dictionaryBasedColumnOrdinalProvider = new DictionaryBasedColumnOrdinalProvider (_ordinals);
    }

    [Test]
    public void GetOrdinal ()
    {
      _ordinals[_columnDefinition] = 5;

      var result = _dictionaryBasedColumnOrdinalProvider.GetOrdinal (_columnDefinition, _dataReaderStub);

      Assert.That (result, Is.EqualTo (5));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "The column 'Testcolumn' was not found in the query result.")]
    public void GetOrdinal_IndexOutOfRange_ThrowsException ()
    {
      _dictionaryBasedColumnOrdinalProvider.GetOrdinal (_columnDefinition, _dataReaderStub);
    }
  }
}