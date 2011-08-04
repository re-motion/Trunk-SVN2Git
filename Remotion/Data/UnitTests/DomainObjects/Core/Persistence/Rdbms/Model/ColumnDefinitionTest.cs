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
using System.ComponentModel;
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ColumnDefinitionTest
  {
    private Type _type;
    private StringConverter _stringConverter;
    private StorageTypeInformation _storageTypeInformation;

    private ColumnDefinition _columnDefinition;

    [SetUp]
    public void SetUp ()
    {
      _type = typeof (string);
      _stringConverter = new StringConverter();
      _storageTypeInformation = new StorageTypeInformation ("varchar", DbType.String, typeof (string), _stringConverter);

      _columnDefinition = new ColumnDefinition ("Name", _type, _storageTypeInformation, true, true);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_columnDefinition.Name, Is.EqualTo ("Name"));
      Assert.That (_columnDefinition.PropertyType, Is.SameAs (_type));
      Assert.That (_columnDefinition.StorageTypeInfo, Is.SameAs (_storageTypeInformation));
      Assert.That (_columnDefinition.IsNullable, Is.True);
      Assert.That (_columnDefinition.IsPartOfPrimaryKey, Is.True);
    }

    [Test]
    public void To_String ()
    {
      Assert.That (_columnDefinition.ToString(), Is.EqualTo ("Name varchar NULL"));
    }
  }
}