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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class UnsupportedStoragePropertyDefinitionTest
  {
    private UnsupportedStoragePropertyDefinition _columnDefinition;

    [SetUp]
    public void SetUp ()
    {
      _columnDefinition = new UnsupportedStoragePropertyDefinition (typeof (int), "Message");
    }

    [Test]
    public void Message ()
    {
      Assert.That (_columnDefinition.Message, Is.EqualTo ("Message"));
    }

    [Test]
    public void GetColumns ()
    {
      _columnDefinition.GetColumns();
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void GetColumnsForComparison ()
    {
      _columnDefinition.GetColumnsForComparison();
    }

    [Test]
    [ExpectedException(typeof(NotSupportedException))]
    public void Read ()
    {
      _columnDefinition.Read (MockRepository.GenerateStub<IDataReader>(), MockRepository.GenerateStub<IColumnOrdinalProvider>());
    }

    [Test]
    [ExpectedException(typeof(NotSupportedException))]
    public void SplitValue ()
    {
      _columnDefinition.SplitValue (null);
    }

    [Test]
    [ExpectedException(typeof(NotSupportedException))]
    public void SplitValueForComparison ()
    {
      _columnDefinition.SplitValueForComparison (null);
    }
  }
}