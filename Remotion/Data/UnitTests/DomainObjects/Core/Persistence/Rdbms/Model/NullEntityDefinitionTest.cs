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
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class NullEntityDefinitionTest
  {
    private NullEntityDefinition _nullEntityDefinition;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("SPID");
      _nullEntityDefinition = new NullEntityDefinition(_storageProviderDefinition);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_nullEntityDefinition.StorageProviderDefinition, Is.SameAs (_storageProviderDefinition));
      Assert.That (_nullEntityDefinition.StorageProviderID, Is.SameAs (_storageProviderDefinition.Name));
      Assert.That (_nullEntityDefinition.ViewName, Is.Null);
    }

    [Test]
    public void ObjectIDColumn ()
    {
      Assert.That (_nullEntityDefinition.IDColumn, Is.Null);
    }

    [Test]
    public void ClassIDColumn ()
    {
      Assert.That (_nullEntityDefinition.ClassIDColumn, Is.Null);
    }

    [Test]
    public void TimestampColumn ()
    {
      Assert.That (_nullEntityDefinition.TimestampColumn, Is.Null);
    }

    [Test]
    public void DataColumns ()
    {
      Assert.That (_nullEntityDefinition.DataColumns, Is.Empty);
    }

    [Test]
    public void IDProperty ()
    {
      Assert.That (_nullEntityDefinition.ObjectIDProperty, Is.Null);
    }

    [Test]
    public void TimestampProperty ()
    {
      Assert.That (_nullEntityDefinition.TimestampProperty, Is.Null);
    }

    [Test]
    public void DataProperties ()
    {
      Assert.That (_nullEntityDefinition.DataProperties, Is.Empty);
    }

    [Test]
    public void GetAllProperties ()
    {
      var result = _nullEntityDefinition.GetAllProperties ();

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void GetAllColumns ()
    {
      var result = _nullEntityDefinition.GetAllColumns();

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void Indexes ()
    {
      var result = _nullEntityDefinition.Indexes;

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void Synonyms ()
    {
      var result = _nullEntityDefinition.Synonyms;

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<IEntityDefinitionVisitor> ();

      visitorMock.Expect (mock => mock.VisitNullEntityDefinition(_nullEntityDefinition));
      visitorMock.Replay ();

      _nullEntityDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations ();
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((INullObject) _nullEntityDefinition).IsNull, Is.True);
    }
  }
}