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
  public class IDColumnDefinitionTest
  {
    private SimpleColumnDefinition _objectIDColumn;
    private SimpleColumnDefinition _classIDColumn;
    private IDColumnDefinition _columnDefinition;
    private IDColumnDefinition _columnDefinitionWithoutClassID;

    [SetUp]
    public void SetUp ()
    {
      _objectIDColumn = new SimpleColumnDefinition ("ObjectID", typeof (ObjectID), "uniqueidentifier", false, false);
      _classIDColumn = new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", false, false);
      _columnDefinition = new IDColumnDefinition (_objectIDColumn, _classIDColumn);
      _columnDefinitionWithoutClassID = new IDColumnDefinition (_objectIDColumn, null);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_columnDefinition.ObjectIDColumn, Is.SameAs (_objectIDColumn));
      Assert.That (_columnDefinition.ClassIDColumn, Is.SameAs (_classIDColumn));
      Assert.That (((IColumnDefinition) _columnDefinition).Name, Is.EqualTo ("ObjectID"));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_columnDefinition.IsNull, Is.False);
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<IColumnDefinitionVisitor> ();

      visitorMock.Expect (mock => mock.VisitIDColumnDefinition (_columnDefinition));
      visitorMock.Replay ();

      _columnDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations ();
    }

    [Test]
    public void Equals_True_WithClassIDColumns ()
    {
      var objectIDColumn = new SimpleColumnDefinition ("ObjectID", typeof (ObjectID), "uniqueidentifier", false, false);
      var classIDColumn = new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", false, false);
      var other = new IDColumnDefinition (objectIDColumn, classIDColumn);

      Assert.That (_columnDefinition.Equals (other), Is.True);
      Assert.That (_columnDefinition.Equals ((object) other), Is.True);
    }

    [Test]
    public void Equals_True_NoClassIDColumns ()
    {
      var objectIDColumn = new SimpleColumnDefinition ("ObjectID", typeof (ObjectID), "uniqueidentifier", false, false);
      var other = new IDColumnDefinition (objectIDColumn, null);

      Assert.That (_columnDefinitionWithoutClassID.Equals (other), Is.True);
      Assert.That (_columnDefinitionWithoutClassID.Equals ((object) other), Is.True);
    }

    [Test]
    public void Equals_False_DifferentType ()
    {
      var other = new NullColumnDefinition ();

      Assert.That (_columnDefinition.Equals (other), Is.False);
      Assert.That (_columnDefinition.Equals ((object) other), Is.False);
    }

    [Test]
    public void Equals_False_DifferentObjectIDColumn ()
    {
      var objectIDColumn = new SimpleColumnDefinition ("ObjectID", typeof (int), "uniqueidentifier", false, false);
      var classIDColumn = new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", false, false);
      var other = new IDColumnDefinition (objectIDColumn, classIDColumn);

      Assert.That (_columnDefinition.Equals (other), Is.False);
      Assert.That (_columnDefinition.Equals ((object) other), Is.False);
    }

    [Test]
    public void Equals_False_DifferentClassIDColumn ()
    {
      var objectIDColumn = new SimpleColumnDefinition ("ObjectID", typeof (ObjectID), "uniqueidentifier", false, false);
      var classIDColumn = new SimpleColumnDefinition ("ClassID", typeof (int), "varchar", false, false);
      var other = new IDColumnDefinition (objectIDColumn, classIDColumn);

      Assert.That (_columnDefinition.Equals (other), Is.False);
      Assert.That (_columnDefinition.Equals ((object) other), Is.False);
    }

    [Test]
    public void Equals_False_NullClassIDColumn ()
    {
      var objectIDColumn = new SimpleColumnDefinition ("ObjectID", typeof (ObjectID), "uniqueidentifier", false, false);
      var other = new IDColumnDefinition (objectIDColumn, null);

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
      var objectIDColumn = new SimpleColumnDefinition ("ObjectID", typeof (ObjectID), "uniqueidentifier", false, false);
      var classIDColumn = new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", false, false);
      var other = new IDColumnDefinition (objectIDColumn, classIDColumn);

      Assert.That (_columnDefinition.GetHashCode (), Is.EqualTo (other.GetHashCode ()));
    }

    [Test]
    public void GetHashCode_EqualObjects_NoClassIDColumn ()
    {
      var objectIDColumn = new SimpleColumnDefinition ("ObjectID", typeof (ObjectID), "uniqueidentifier", false, false);
      var other = new IDColumnDefinition (objectIDColumn, null);

      Assert.That (_columnDefinitionWithoutClassID.GetHashCode (), Is.EqualTo (other.GetHashCode ()));
    }
  }
}