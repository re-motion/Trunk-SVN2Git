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
  public class UnsupportedStorageTypeColumnDefinitionTest
  {
    private UnsupportedStorageTypeColumnDefinition _columnDefinition;

    [SetUp]
    public void SetUp ()
    {
      _columnDefinition = new UnsupportedStorageTypeColumnDefinition();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Unsupported properties cannot be visited.")]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<IColumnDefinitionVisitor>();
      visitorMock.Replay();

      _columnDefinition.Accept (visitorMock);
    }

    [Test]
    public void Equals_True ()
    {
      var other = new UnsupportedStorageTypeColumnDefinition();

      Assert.That (_columnDefinition.Equals (other), Is.True);
      Assert.That (_columnDefinition.Equals ((object) other), Is.True);
    }

    [Test]
    public void Equals_False_DifferentType ()
    {
      var other = new SimpleColumnDefinition ("Test", typeof (int), "kk", false);

      Assert.That (_columnDefinition.Equals (other), Is.False);
      Assert.That (_columnDefinition.Equals ((object) other), Is.False);
    }

    [Test]
    public void Equals_False_Null ()
    {
      Assert.That (_columnDefinition.Equals ((IColumnDefinition) null), Is.False);
      Assert.That (_columnDefinition.Equals ((object) null), Is.False);
    }

    [Test]
    public void GetHashCode_EqualObjects ()
    {
      var other = new UnsupportedStorageTypeColumnDefinition();

      Assert.That (_columnDefinition.GetHashCode (), Is.EqualTo (other.GetHashCode ()));
    }
  }
}