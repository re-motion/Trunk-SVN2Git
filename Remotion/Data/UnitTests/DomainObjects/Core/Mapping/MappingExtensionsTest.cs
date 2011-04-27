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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class MappingExtensionsTest
  {
    [Test]
    public void GetMandatory_ForClassType_ValueFound ()
    {
      var classDefinition = CreateClassDefinition();

      var classDefinitions = new Dictionary<Type, ClassDefinition>();
      classDefinitions.Add (typeof (DomainObject), classDefinition);

      Assert.That (classDefinitions.GetMandatory2 (typeof (DomainObject)), Is.SameAs (classDefinition));
    }

    [Test]
    public void GetMandatory_ForClassType_ValueNotFound ()
    {
      var classDefinitions = new Dictionary<Type, ClassDefinition>();

      Assert.That (
          () => classDefinitions.GetMandatory2 (typeof (DomainObject)),
          Throws.Exception.InstanceOf<MappingException>()
              .And.Message.EqualTo (string.Format ("Mapping does not contain class '{0}'.", typeof (DomainObject))));
    }

    [Test]
    public void GetMandatory_ForClassID_ValueFound ()
    {
      var classDefinition = CreateClassDefinition();

      var classDefinitions = new Dictionary<string, ClassDefinition>();
      classDefinitions.Add ("ID", classDefinition);

      Assert.That (classDefinitions.GetMandatory2 ("ID"), Is.SameAs (classDefinition));
    }

    [Test]
    public void GetMandatory_ForClassID_ValueNotFound ()
    {
      var classDefinitions = new Dictionary<string, ClassDefinition>();

      Assert.That (
          () => classDefinitions.GetMandatory2 ("ID"),
          Throws.Exception.InstanceOf<MappingException>()
              .And.Message.EqualTo ("Mapping does not contain class 'ID'."));
    }

    private ClassDefinition CreateClassDefinition ()
    {
      return new ClassDefinition ("ID", typeof (DomainObject), true, null, null, MockRepository.GenerateStub<IPersistentMixinFinder>());
    }
  }
}