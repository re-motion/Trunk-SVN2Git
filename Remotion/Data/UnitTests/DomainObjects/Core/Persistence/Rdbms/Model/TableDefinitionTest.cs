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
  public class TableDefinitionTest
  {
    private SimpleColumnDefinition[] _columns;
    private TableDefinition _tableDefintion;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private PrimaryKeyConstraintDefinition[] _constraints;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("SPID");
      _columns = new[] { new SimpleColumnDefinition ("COL1", typeof (string), "varchar", true, false) };
      _constraints = new[]
                     {
                         new PrimaryKeyConstraintDefinition (
                             "PK_Table", true, new[] { new SimpleColumnDefinition ("ID", typeof (ObjectID), "uniquidentifier", false, false) })
                     };
      _tableDefintion = new TableDefinition (
          _storageProviderDefinition, new EntityNameDefinition (null, "Test"), new EntityNameDefinition (null, "TestView"), _columns, _constraints);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_tableDefintion.TableName.EntityName, Is.EqualTo ("Test"));
      Assert.That (_tableDefintion.StorageProviderID, Is.EqualTo ("SPID"));
      Assert.That (_tableDefintion.StorageProviderDefinition, Is.SameAs (_storageProviderDefinition));
    }

    [Test]
    public void Initialization_ViewNameNull ()
    {
      var tableDefinition = new TableDefinition (_storageProviderDefinition, new EntityNameDefinition(null, "Test"), null, _columns, _constraints);
      Assert.That (tableDefinition.ViewName, Is.Null);
    }

    [Test]
    public void LegacyEntityName ()
    {
      Assert.That (_tableDefintion.LegacyEntityName, Is.EqualTo ("Test"));
    }

    [Test]
    public void LegacyViewName ()
    {
      Assert.That (_tableDefintion.LegacyViewName, Is.EqualTo ("TestView"));
    }

    [Test]
    public void Constraints ()
    {
      var result = _tableDefintion.Constraints;

      Assert.That (result, Is.EqualTo (_constraints));
    }

    [Test]
    public void GetColumns ()
    {
      var result = _tableDefintion.GetColumns();

      Assert.That (result, Is.EqualTo (_columns));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_tableDefintion.IsNull, Is.False);
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<IEntityDefinitionVisitor>();

      visitorMock.Expect (mock => mock.VisitTableDefinition (_tableDefintion));
      visitorMock.Replay();

      _tableDefintion.Accept (visitorMock);

      visitorMock.VerifyAllExpectations();
    }
  }
}