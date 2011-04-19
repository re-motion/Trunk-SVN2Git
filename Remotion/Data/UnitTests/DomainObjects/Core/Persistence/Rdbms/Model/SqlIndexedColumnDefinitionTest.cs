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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class SqlIndexedColumnDefinitionTest
  {
    private SimpleColumnDefinition _innerColumn;
    private SqlIndexedColumnDefinition _indexedColumn;

    [SetUp]
    public void SetUp ()
    {
      _innerColumn = new SimpleColumnDefinition ("InnerColumn", typeof (string), "varchar", true, false);
      _indexedColumn = new SqlIndexedColumnDefinition (_innerColumn, IndexOrder.Desc);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_indexedColumn.Columnn, Is.SameAs (_innerColumn));
      Assert.That (_indexedColumn.IndexOrder, Is.EqualTo (IndexOrder.Desc));
    }

    [Test]
    public void Initialization_WithNullIndexOrder ()
    {
      var indexedColumn = new SqlIndexedColumnDefinition (_innerColumn);

      Assert.That (indexedColumn.IndexOrder, Is.Null);
    }

    [Test]
    public void Name ()
    {
      var result = _indexedColumn.Name;

      Assert.That (result, Is.EqualTo (_innerColumn.Name));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_indexedColumn.IsNull, Is.False);
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<IColumnDefinitionVisitor> ();

      visitorMock.Expect (mock => mock.VisitSqlIndexedColumnDefinition (_indexedColumn));
      visitorMock.Replay ();

      _indexedColumn.Accept (visitorMock);

      visitorMock.VerifyAllExpectations ();
    }

    [Test]
    public void Equals_True ()
    {
      var other = new SqlIndexedColumnDefinition (_innerColumn, IndexOrder.Desc);

      Assert.That (_indexedColumn.Equals (other), Is.True);
      Assert.That (_indexedColumn.Equals ((object) other), Is.True);
    }

    [Test]
    public void Equals_False_DifferentIndexOrder ()
    {
      var other = new SqlIndexedColumnDefinition (_innerColumn, IndexOrder.Asc);

      Assert.That (_indexedColumn.Equals (other), Is.False);
      Assert.That (_indexedColumn.Equals ((object) other), Is.False);
    }

    [Test]
    public void Equals_False_DifferentType ()
    {
      var other = new NullColumnDefinition ();

      Assert.That (_indexedColumn.Equals (other), Is.False);
      Assert.That (_indexedColumn.Equals ((object) other), Is.False);
    }

    [Test]
    public void Equals_False_DifferentInnerColumn ()
    {
      var innerColumn = new SimpleColumnDefinition ("ObjectID", typeof (int), "uniqueidentifier", false, false);
      var other = new SqlIndexedColumnDefinition(innerColumn);

      Assert.That (_indexedColumn.Equals (other), Is.False);
      Assert.That (_indexedColumn.Equals ((object) other), Is.False);
    }

    [Test]
    public void Equals_False_Null ()
    {
      Assert.That (_indexedColumn.Equals (null), Is.False);
      Assert.That (_indexedColumn.Equals ((object) null), Is.False);
    }

    [Test]
    public void GetHashCode_EqualObjects ()
    {
      var innerColumn = new SimpleColumnDefinition ("InnerColumn", typeof (string), "varchar", true, false);
      var other = new SqlIndexedColumnDefinition (innerColumn, IndexOrder.Desc);

      Assert.That (_indexedColumn.GetHashCode (), Is.EqualTo (other.GetHashCode ()));
    }
  }
}