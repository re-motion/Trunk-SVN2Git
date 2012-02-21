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
using NUnit.Framework;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class CompleteInterfaceTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedClass_ImplementsCompleteInterface ()
    {
      var type = TypeFactory.GetConcreteType (typeof (ClassWithCompleteInterface));
      Assert.That (type.GetInterfaces(), Has.Member (typeof (ClassWithCompleteInterface.ICompleteInterface)));

      var instance = (ClassWithCompleteInterface.ICompleteInterface) ObjectFactory.Create<ClassWithCompleteInterface> ();
      Assert.That (instance.M1 (), Is.EqualTo ("ClassWithCompleteInterface.M1"));
      Assert.That (instance.Method (), Is.EqualTo ("MixinImplementingSimpleInterface.Method"));
    }

    [Test]
    public void GeneratedClass_ImplementsCompleteInterface_FromBase ()
    {
      var type = TypeFactory.GetConcreteType (typeof (DerivedClassWithCompleteInterface));
      Assert.That (type.GetInterfaces (), Has.Member (typeof (ClassWithCompleteInterface.ICompleteInterface)));

      var instance = (ClassWithCompleteInterface.ICompleteInterface) ObjectFactory.Create<DerivedClassWithCompleteInterface> ();
      Assert.That (instance.M1 (), Is.EqualTo ("ClassWithCompleteInterface.M1"));
      Assert.That (instance.Method (), Is.EqualTo ("MixinImplementingSimpleInterface.Method"));
    }

    [Test]
    public void GeneratedClass_ImplementsCompleteInterface_WithHasInterface_FromGenericBaseClass ()
    {
      var type = TypeFactory.GetConcreteType (typeof (ClassDerivedFromBaseClassWithHasComleteInterface));
      Assert.That (type.GetInterfaces (), Has.Member (typeof (ClassDerivedFromBaseClassWithHasComleteInterface.ICompleteInterface)));

      var instance = (ClassDerivedFromBaseClassWithHasComleteInterface.ICompleteInterface) ObjectFactory.Create<ClassDerivedFromBaseClassWithHasComleteInterface> ();
      Assert.That (instance.M1 (), Is.EqualTo ("ClassDerivedFromBaseClassWithHasComleteInterface.M1"));
      Assert.That (instance.Method (), Is.EqualTo ("MixinImplementingSimpleInterface.Method"));
    }

    [Test]
    public void GeneratedClass_ImplementsCompleteInterface_WithHasInterface_FromBase ()
    {
      var type = TypeFactory.GetConcreteType (typeof (DerivedClassDerivedFromBaseClassWithHasComleteInterface));
      Assert.That (type.GetInterfaces (), Has.Member (typeof (ClassDerivedFromBaseClassWithHasComleteInterface.ICompleteInterface)));

      var instance = (ClassDerivedFromBaseClassWithHasComleteInterface.ICompleteInterface) ObjectFactory.Create<DerivedClassDerivedFromBaseClassWithHasComleteInterface> ();
      Assert.That (instance.M1 (), Is.EqualTo ("ClassDerivedFromBaseClassWithHasComleteInterface.M1"));
      Assert.That (instance.Method (), Is.EqualTo ("MixinImplementingSimpleInterface.Method"));
    }
  }
}