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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ObjectIDWithClassIDColumnDefinitionTest
  {
    private SimpleColumnDefinition _objectIDColumn;
    private SimpleColumnDefinition _classIDColumn;
    private ObjectIDWithClassIDColumnDefinition _columnDefinition;

    [SetUp]
    public void SetUp ()
    {
      _objectIDColumn = new SimpleColumnDefinition ("ObjectID", typeof (ObjectID), "uniqueidentifier", false);
      _classIDColumn = new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", false);
      _columnDefinition = new ObjectIDWithClassIDColumnDefinition (_objectIDColumn, _classIDColumn);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_columnDefinition.ObjectIDColumn, Is.SameAs (_objectIDColumn));
      Assert.That (_columnDefinition.ClassIDColumn, Is.SameAs (_classIDColumn));
      Assert.That (_columnDefinition.Name, Is.EqualTo ("ObjectID"));
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<IColumnDefinitionVisitor> ();

      visitorMock.Expect (mock => mock.VisitObjectIDWithClassIDColumnDefinition (_columnDefinition));
      visitorMock.Replay ();

      _columnDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations ();
    }
  }
}