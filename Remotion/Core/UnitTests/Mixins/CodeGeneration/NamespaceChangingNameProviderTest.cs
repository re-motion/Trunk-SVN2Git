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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class NamespaceChangingNameProviderTest
  {
    [Test]
    public void GetNameForConcreteMixedType_NormalNameGetsExtendedNamespace ()
    {
      var nameProvider = NamespaceChangingNameProvider.Instance;

      TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
      string newName = nameProvider.GetNameForConcreteMixedType (definition);

      Assert.That (newName, Is.EqualTo (typeof (BaseType1).Namespace + ".MixedTypes.BaseType1"));
    }

    [Test]
    public void GetNameForConcreteMixedType_GenericNameGetsExtendedNamespacePlusCharacterReplacements ()
    {
      var nameProvider = NamespaceChangingNameProvider.Instance;

      TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition_Force (
          typeof (GenericTargetClass<int>));
      string newName = nameProvider.GetNameForConcreteMixedType (definition);

      var mscorlibAssemblyName = typeof (int).Assembly.GetName ();

      var expected = string.Format (
          "{0}.MixedTypes.GenericTargetClass`1{{System_Int32/{1}/Version={2}_{3}_{4}_{5}/Culture=neutral/PublicKeyToken=b77a5c561934e089}}",
          typeof (GenericTargetClass<int>).Namespace,
          mscorlibAssemblyName.Name,
          mscorlibAssemblyName.Version.Major,
          mscorlibAssemblyName.Version.Minor,
          mscorlibAssemblyName.Version.Build,
          mscorlibAssemblyName.Version.Revision);

      Assert.That (newName, Is.EqualTo (expected));
    }
  }
}
