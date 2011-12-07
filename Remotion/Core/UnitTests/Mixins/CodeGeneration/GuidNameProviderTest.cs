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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class GuidNameProviderTest
  {
    private GuidNameProvider _nameProvider;

    [SetUp]
    public void SetUp ()
    {
      _nameProvider = new GuidNameProvider();
    }

    [Test]
    public void GetNameForConcreteMixedType ()
    {
      var definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (object));
      var name = _nameProvider.GetNameForConcreteMixedType (definition);
      Assert.That (name, Is.StringStarting("System.Object_Mixed_"));
    }

    [Test]
    public void GetNameForConcreteMixedType_UniqueNames ()
    {
      var definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (object));
      var name1 = _nameProvider.GetNameForConcreteMixedType (definition);
      var name2 = _nameProvider.GetNameForConcreteMixedType (definition);

      Assert.That (name1, Is.Not.EqualTo (name2));
    }

    [Test]
    public void GetNameForConcreteMixinType ()
    {
      var identifier = new ConcreteMixinTypeIdentifier (typeof (object), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());
      var name = _nameProvider.GetNameForConcreteMixinType (identifier);
      Assert.That (name, Is.StringStarting("System.Object_GeneratedMixin_"));
    }

    [Test]
    public void GetNameForConcreteMixinType_GenericNameSimplified () // required because .NET chokes otherwise
    {
      var identifier = new ConcreteMixinTypeIdentifier (typeof (List<int>), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());
      var name = _nameProvider.GetNameForConcreteMixinType (identifier);
      Assert.That (name, Is.StringStarting("System.Collections.Generic.List`1_GeneratedMixin_"));
    }

    [Test]
    public void GetNameForConcreteMixinType_UniqueNames ()
    {
      var identifier = new ConcreteMixinTypeIdentifier (typeof (object), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());
      var name1 = _nameProvider.GetNameForConcreteMixinType (identifier);
      var name2 = _nameProvider.GetNameForConcreteMixinType (identifier);
      Assert.That (name1, Is.Not.EqualTo (name2));
    }
  }
}
