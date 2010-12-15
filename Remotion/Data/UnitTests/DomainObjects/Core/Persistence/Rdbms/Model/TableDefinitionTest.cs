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
using NUnit.Framework.SyntaxHelpers;
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

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("SPID", typeof (UnitTestStorageObjectFactoryStub));
      _columns = new[] { new SimpleColumnDefinition ("COL1", typeof(string), "varchar", true) };
      _tableDefintion = new TableDefinition (_storageProviderDefinition, "Test", "TestView", _columns);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_tableDefintion.TableName, Is.EqualTo ("Test"));
      Assert.That (_tableDefintion.StorageProviderID, Is.EqualTo ("SPID"));
      Assert.That (_tableDefintion.StorageProviderDefinition, Is.SameAs(_storageProviderDefinition));
    }

    [Test]
    public void Initialization_ViewNameNull ()
    {
      var tableDefinition = new TableDefinition (_storageProviderDefinition, "Test", null, _columns);
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
    public void GetColumns ()
    {
      var result  = _tableDefintion.GetColumns ();

      Assert.That (result, Is.EqualTo (_columns));  
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