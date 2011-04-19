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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer
{
  [TestFixture]
  public class SqlPrimaryXmlIndexDefinitionTest
  {
    private EntityNameDefinition _objectName;
    private SimpleColumnDefinition _xmlColumn;
    private SqlPrimaryXmlIndexDefinition _indexDefinition;

    [SetUp]
    public void SetUp ()
    {
      _objectName = new EntityNameDefinition ("objectSchema", "objectName");
      _xmlColumn = new SimpleColumnDefinition ("XmlColumn", typeof (string), "xml", true, false);
      _indexDefinition = new SqlPrimaryXmlIndexDefinition ("IndexName", _objectName, _xmlColumn);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_indexDefinition.IndexName, Is.EqualTo("IndexName"));
      Assert.That (_indexDefinition.ObjectName, Is.SameAs (_objectName));
      Assert.That (_indexDefinition.XmlColumn, Is.SameAs (_xmlColumn));
    }

    [Test]
    public void Accept_IndexDefinitionVisitor ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<IIndexDefinitionVisitor> ();
      visitorMock.Replay ();

      _indexDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations ();
    }

    [Test]
    public void Accept_SqlIndexDefinitionVisitor ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<ISqlIndexDefinitionVisitor> ();
      visitorMock.Expect (mock => mock.VisitPrimaryXmlIndexDefinition (_indexDefinition));
      visitorMock.Replay ();

      _indexDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations ();
    }
  }
}