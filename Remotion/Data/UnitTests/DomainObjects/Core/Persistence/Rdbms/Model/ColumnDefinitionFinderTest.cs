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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ColumnDefinitionFinderTest
  {
    private ColumnDefinitionFinder _finder;
    private SimpleColumnDefinition _availableColumn1;
    private SimpleColumnDefinition _availableColumn2;
    private SimpleColumnDefinition _notAvailableColumn;
    private IDColumnDefinition _availableIdColumn;
    private IDColumnDefinition _notAvailableIdColumn1;
    private IDColumnDefinition _notAvailableIdColumn2;
    private SqlIndexedColumnDefinition _availableIndexedColum;
    private SqlIndexedColumnDefinition _notAvailableIndexedColumn1;
    private SqlIndexedColumnDefinition _notAvailableIndexedColumn2;

    [SetUp]
    public void SetUp ()
    {
      _availableColumn1 = new SimpleColumnDefinition ("Column1", typeof (string), "varchar", true, false);
      _availableColumn2 = new SimpleColumnDefinition ("Column2", typeof (string), "varchar", false, false);
      _notAvailableColumn = new SimpleColumnDefinition ("Column3", typeof (string), "varchar", false, false);
      _availableIndexedColum = new SqlIndexedColumnDefinition (_availableColumn1);
      _notAvailableIndexedColumn1 = new SqlIndexedColumnDefinition (_availableColumn2);
      _notAvailableIndexedColumn2 = new SqlIndexedColumnDefinition (_notAvailableColumn);
      _availableIdColumn = new IDColumnDefinition (_availableColumn1, _availableColumn2);
      _notAvailableIdColumn1 = new IDColumnDefinition (_availableColumn2, _availableColumn1 );
      _notAvailableIdColumn2 = new IDColumnDefinition (_notAvailableColumn, _notAvailableColumn);
      
      _finder = new ColumnDefinitionFinder (new IColumnDefinition[] { _availableIndexedColum, _availableIdColumn, _availableColumn1, _availableColumn2 });
    }

    [Test]
    public void VisitSimpleColumnDefinition_ColumnDefinitionIsAvailable_ColumnIsReturned ()
    {
      var result = _finder.FindColumn (_availableColumn1);

      Assert.That (result, Is.SameAs (_availableColumn1));
    }

    [Test]
    public void VisitSimpleColumnDefinition_ColumnDefinitionIsNotAvailable_NullColumnDefinitionIsReturned ()
    {
      var result = _finder.FindColumn (_notAvailableColumn);

      Assert.That (result, Is.Not.SameAs(_notAvailableColumn));
      Assert.That (result, Is.TypeOf(typeof (NullColumnDefinition)));
    }

    [Test]
    public void VisitSqlIndexedColumnDefinition_ColumnDefinitonIsAvailable_ColumnIsReturned ()
    {
      var result = _finder.FindColumn (_availableIndexedColum);

      Assert.That (result, Is.SameAs (_availableIndexedColum));
    }

    [Test]
    public void VisitSqlIndexedColumnDefinition_InnerColumnsAreAvailable_NewSqlIndexedColumnWithAvailableColumnsIsReturned ()
    {
      var result = _finder.FindColumn (_notAvailableIndexedColumn1);

      Assert.That (result, Is.Not.SameAs (_notAvailableIndexedColumn1));
      Assert.That (result, Is.TypeOf (typeof (SqlIndexedColumnDefinition)));
      Assert.That (((SqlIndexedColumnDefinition) result).Columnn, Is.SameAs (_availableColumn2));
    }

    [Test]
    public void VisitSqlIndexedColumnDefinition_InnerColumnIsNotAvailable_NewSqlIndexedColumnWithNullColumnsIsReturned ()
    {
      var result = _finder.FindColumn (_notAvailableIndexedColumn2);

      Assert.That (result, Is.Not.SameAs (_notAvailableIndexedColumn2));
      Assert.That (result, Is.TypeOf (typeof (SqlIndexedColumnDefinition)));
      Assert.That (((SqlIndexedColumnDefinition) result).Columnn, Is.Not.SameAs (_notAvailableColumn));
      Assert.That (((SqlIndexedColumnDefinition) result).Columnn, Is.TypeOf (typeof (NullColumnDefinition)));
    }
    
    [Test]
    public void VisitIDColumnDefinition_ColumnDefinitonIsAvailable_ColumnIsReturned ()
    {
      var result = _finder.FindColumn (_availableIdColumn);

      Assert.That (result, Is.SameAs (_availableIdColumn));
    }

    [Test]
    public void VisitIDColumnDefinition_InnerColumnsAreAvailable_NewIDColumnWithAvailableColumnsIsReturned ()
    {
      var result = _finder.FindColumn (_notAvailableIdColumn1);

      Assert.That (result, Is.Not.SameAs (_notAvailableIdColumn1));
      Assert.That (result, Is.TypeOf(typeof(IDColumnDefinition)));
      Assert.That (((IDColumnDefinition) result).ObjectIDColumn, Is.SameAs (_availableColumn2));
      Assert.That (((IDColumnDefinition) result).ClassIDColumn, Is.SameAs (_availableColumn1));
    }

    [Test]
    public void VisitIDColumnDefinition_InnerColumnsAreNotAvailable_NewIDColumnWithNullColumnsIsReturned ()
    {
      var result = _finder.FindColumn (_notAvailableIdColumn2);

      Assert.That (result, Is.Not.SameAs (_notAvailableIdColumn2));
      Assert.That (result, Is.TypeOf (typeof (IDColumnDefinition)));
      Assert.That (((IDColumnDefinition) result).ObjectIDColumn, Is.Not.SameAs (_notAvailableColumn));
      Assert.That (((IDColumnDefinition) result).ObjectIDColumn, Is.TypeOf(typeof(NullColumnDefinition)));
      Assert.That (((IDColumnDefinition) result).ClassIDColumn, Is.Not.SameAs (_notAvailableColumn));
      Assert.That (((IDColumnDefinition) result).ClassIDColumn, Is.TypeOf (typeof (NullColumnDefinition)));
    }

    [Test]
    public void VisitNullColumnDefinition ()
    {
      var result = _finder.FindColumn (new NullColumnDefinition());

      Assert.That (result, Is.TypeOf (typeof (NullColumnDefinition)));
    }
  }
}