// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
  public class UnsupportedStoragePropertyDefinitionTest
  {
    private UnsupportedStoragePropertyDefinition _columnDefinition;
    private Exception _innerException;

    [SetUp]
    public void SetUp ()
    {
      _innerException = new Exception ("Inner!");
      _columnDefinition = new UnsupportedStoragePropertyDefinition (typeof (int), "Message", _innerException);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_columnDefinition.Message, Is.EqualTo ("Message"));
      Assert.That (_columnDefinition.PropertyType, Is.SameAs (typeof (int)));
      Assert.That (_columnDefinition.InnerException, Is.SameAs (_innerException));
    }

    [Test]
    public void Initialization_WithNullInnerException ()
    {
      var columnDefinition = new UnsupportedStoragePropertyDefinition (typeof (int), "Message", null);

      Assert.That (columnDefinition.InnerException, Is.Null);
      Assert.That (
          () => columnDefinition.GetColumns(), 
          Throws.TypeOf<NotSupportedException>().With.InnerException.Null);
    }

    [Test]
    public void GetColumns ()
    {
      Assert.That (
          () => _columnDefinition.GetColumns (),
          Throws.TypeOf<NotSupportedException> ()
              .With.Message.EqualTo ("This operation is not supported because the storage property is invalid. Reason: Message")
              .And.InnerException.SameAs (_innerException));
    }

    [Test]
    public void GetColumnsForComparison ()
    {
      Assert.That (
          () => _columnDefinition.GetColumnsForComparison(), 
          Throws.TypeOf<NotSupportedException>()
              .With.Message.EqualTo ("This operation is not supported because the storage property is invalid. Reason: Message")
              .And.InnerException.SameAs (_innerException));
    }

    [Test]
    public void SplitValue ()
    {
      Assert.That (
          () => _columnDefinition.SplitValue (null),
          Throws.TypeOf<NotSupportedException> ()
              .With.Message.EqualTo ("This operation is not supported because the storage property is invalid. Reason: Message")
              .And.InnerException.SameAs (_innerException));
    }

    [Test]
    public void SplitValueForComparison ()
    {
      Assert.That (
          () => { _columnDefinition.SplitValueForComparison (null); },
          Throws.TypeOf<NotSupportedException> ()
              .With.Message.EqualTo ("This operation is not supported because the storage property is invalid. Reason: Message")
              .And.InnerException.SameAs (_innerException));
    }

    [Test]
    public void SplitValuesForComparison ()
    {
      Assert.That (
          () => _columnDefinition.SplitValuesForComparison (null),
          Throws.TypeOf<NotSupportedException> ()
              .With.Message.EqualTo ("This operation is not supported because the storage property is invalid. Reason: Message")
              .And.InnerException.SameAs (_innerException));
    }

    [Test]
    public void CombineValue ()
    {
      Assert.That (
          () => _columnDefinition.CombineValue (MockRepository.GenerateStub<IColumnValueProvider> ()),
          Throws.TypeOf<NotSupportedException> ()
              .With.Message.EqualTo ("This operation is not supported because the storage property is invalid. Reason: Message")
              .And.InnerException.SameAs (_innerException));
    }
  }
}