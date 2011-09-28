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
  public class EmptyViewDefinitionTest
  {
    private EmptyViewDefinition _emptyViewDefinition;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("SPID");
      _emptyViewDefinition = new EmptyViewDefinition (_storageProviderDefinition);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_emptyViewDefinition.StorageProviderDefinition, Is.SameAs (_storageProviderDefinition));
      Assert.That (_emptyViewDefinition.StorageProviderID, Is.SameAs (_storageProviderDefinition.Name));
      Assert.That (_emptyViewDefinition.ViewName, Is.Null);
    }

    [Test]
    public void IDProperty ()
    {
      Assert.That (_emptyViewDefinition.ObjectIDProperty, Is.Null);
    }

    [Test]
    public void TimestampProperty ()
    {
      Assert.That (_emptyViewDefinition.TimestampProperty, Is.Null);
    }

    [Test]
    public void DataProperties ()
    {
      Assert.That (_emptyViewDefinition.DataProperties, Is.Empty);
    }

    [Test]
    public void GetAllProperties ()
    {
      var result = _emptyViewDefinition.GetAllProperties ();

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void GetAllColumns ()
    {
      var result = _emptyViewDefinition.GetAllColumns();

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void Indexes ()
    {
      var result = _emptyViewDefinition.Indexes;

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void Synonyms ()
    {
      var result = _emptyViewDefinition.Synonyms;

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<IRdbmsStorageEntityDefinitionVisitor> ();

      visitorMock.Expect (mock => mock.VisitEmptyViewDefinition(_emptyViewDefinition));
      visitorMock.Replay ();

      _emptyViewDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations ();
    }
  }
}