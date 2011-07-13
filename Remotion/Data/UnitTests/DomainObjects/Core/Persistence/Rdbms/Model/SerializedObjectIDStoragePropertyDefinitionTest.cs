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
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class SerializedObjectIDStoragePropertyDefinitionTest
  {
    private SimpleStoragePropertyDefinition _serializedIDProperty;
    private SerializedObjectIDStoragePropertyDefinition _serializedObjectIDStoragePropertyDefinition;

    [SetUp]
    public void SetUp ()
    {
      _serializedIDProperty = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty();
      _serializedObjectIDStoragePropertyDefinition = new SerializedObjectIDStoragePropertyDefinition (_serializedIDProperty);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_serializedObjectIDStoragePropertyDefinition.SerializedIDProperty, Is.SameAs (_serializedIDProperty));
    }

    [Test]
    public void GetColumnForLookup ()
    {
      Assert.That (_serializedObjectIDStoragePropertyDefinition.GetColumnForLookup(), Is.SameAs (_serializedIDProperty.ColumnDefinition));
    }

    [Test]
    public void GetColumnForForeignKey ()
    {
      Assert.That (_serializedObjectIDStoragePropertyDefinition.GetColumnForForeignKey(), Is.Null);
    }

    [Test]
    public void GetColumns ()
    {
      Assert.That (_serializedObjectIDStoragePropertyDefinition.GetColumns(), Is.EqualTo (_serializedIDProperty.GetColumns()));
    }

    [Test]
    public void Name ()
    {
      Assert.That (_serializedObjectIDStoragePropertyDefinition.Name, Is.EqualTo (_serializedIDProperty.Name));
    }

    [Test]
    public void Equals_Null ()
    {
      Assert.That (_serializedObjectIDStoragePropertyDefinition.Equals (null), Is.False);
    }

    [Test]
    public void Equals_OtherType ()
    {
      Assert.That (_serializedObjectIDStoragePropertyDefinition.Equals (MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>()), Is.False);
    }

    [Test]
    public void Equals_False ()
    {
      Assert.That (
          _serializedObjectIDStoragePropertyDefinition.Equals (
              new SerializedObjectIDStoragePropertyDefinition (SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty())),
          Is.False);
    }

    [Test]
    public void Equals_True ()
    {
      Assert.That (
          _serializedObjectIDStoragePropertyDefinition.Equals (new SerializedObjectIDStoragePropertyDefinition (_serializedIDProperty)),
          Is.True);
    }
  }
}