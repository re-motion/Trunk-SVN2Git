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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class TypeNotFoundClassDefinitionTest
  {
    private TypeNotFoundClassDefinition _classDefinition;
    private Type _classType;

    [SetUp]
    public void SetUp ()
    {
      _classType = typeof (ClassNotInMapping);
      _classDefinition = new TypeNotFoundClassDefinition (
          "Test", _classType, typeof (string).GetProperty ("Length"));
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_classDefinition.ClassType, Is.SameAs (_classType));
      Assert.That (_classDefinition.BaseClass, Is.Null);
      Assert.That (_classDefinition.IsClassTypeResolved, Is.False);
      Assert.That (_classDefinition.IsAbstract, Is.False);
    }
  }
}