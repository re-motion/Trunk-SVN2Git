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
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class SimpleColumnDefinitionFindingVisitorTest
  {
    private SimpleColumnDefinitionFindingVisitor _visitor;
    private SimpleColumnDefinition _simpleColumn1;
    private SimpleColumnDefinition _simpleColumn2;
    private SimpleColumnDefinition _simpleColumn3;
    private SimpleColumnDefinition _simpleColumn4;
    private IDColumnDefinition _idColumn1;
    private IDColumnDefinition _idColumn2;
    
    [SetUp]
    public void SetUp ()
    {
      _simpleColumn1 = new SimpleColumnDefinition ("Simple1", typeof (string), "varchar", true, false);
      _simpleColumn2 = new SimpleColumnDefinition ("Simple2", typeof (int), "integer", false, false);
      _simpleColumn3 = new SimpleColumnDefinition ("Simple3", typeof (int), "integer", false, false);
      _simpleColumn4 = new SimpleColumnDefinition ("Simple4", typeof (int), "integer", false, false);
      _idColumn1 = new IDColumnDefinition (_simpleColumn1, _simpleColumn2);
      _idColumn2 = new IDColumnDefinition (_simpleColumn3, _simpleColumn4);

      _visitor = new SimpleColumnDefinitionFindingVisitor();
    }

    [Test]
    public void VisitSimpleColumnDefinition_Once ()
    {
      _visitor.VisitSimpleColumnDefinition (_simpleColumn1);

      var result = _visitor.GetSimpleColumns().ToArray();

      Assert.That (result.Length, Is.EqualTo (1));
      Assert.That (result[0], Is.SameAs (_simpleColumn1));
    }

    [Test]
    public void VisitSimpleColumnDefinition_Twice ()
    {
      _visitor.VisitSimpleColumnDefinition (_simpleColumn1);
      _visitor.VisitSimpleColumnDefinition (_simpleColumn2);

      var result = _visitor.GetSimpleColumns ().ToArray ();

      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (result[0], Is.SameAs (_simpleColumn1));
      Assert.That (result[1], Is.SameAs (_simpleColumn2));
    }

    [Test]
    public void VisitIDColumnDefinition_Once ()
    {
      _visitor.VisitIDColumnDefinition (_idColumn1);

      var result = _visitor.GetSimpleColumns ().ToArray ();

      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (result[0], Is.SameAs (_simpleColumn1));
      Assert.That (result[1], Is.SameAs (_simpleColumn2));
    }

    [Test]
    public void VisitIDColumnDefinition_Twice ()
    {
      _visitor.VisitIDColumnDefinition (_idColumn1);
      _visitor.VisitIDColumnDefinition (_idColumn2);

      var result = _visitor.GetSimpleColumns ().ToArray ();

      Assert.That (result.Length, Is.EqualTo (4));
      Assert.That (result[0], Is.SameAs (_simpleColumn1));
      Assert.That (result[1], Is.SameAs (_simpleColumn2));
      Assert.That (result[2], Is.SameAs (_simpleColumn3));
      Assert.That (result[3], Is.SameAs (_simpleColumn4));
    }

    [Test]
    public void VisitNullColumnDefinition ()
    {
      _visitor.VisitNullColumnDefinition (new NullColumnDefinition());

      var result = _visitor.GetSimpleColumns ().ToArray ();

      Assert.That (result.Length, Is.EqualTo (0));
    }

  }
}