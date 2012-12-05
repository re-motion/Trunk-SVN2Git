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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.UnitTests.Core.CodeGeneration;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Mixins.Utilities;
using Remotion.Reflection;

namespace Remotion.Mixins.UnitTests.Core
{
  [TestFixture]
  public class CodeGenerationStatisticsTest
  {
    [SetUp]
    public void SetUp ()
    {
      ConcreteTypeBuilder.SetCurrent (null);
    }

    [TearDown]
    public void TearDown ()
    {
      ConcreteTypeBuilder.SetCurrent (null);     
    }

    [Test]
    public void CurrentUnsignedAssemblyBuilder_Null ()
    {
      Assert.That (CodeGenerationStatistics.CurrentUnsignedAssemblyBuilder, Is.Null);
    }

    [Test]
    public void CurrentUnsignedAssemblyBuilder_Null_WithExistingConcreteTypeBuilder ()
    {
      ConcreteTypeBuilder.SetCurrent (ConcreteTypeBuilderObjectMother.CreateConcreteTypeBuilder());
      Assert.That (CodeGenerationStatistics.CurrentUnsignedAssemblyBuilder, Is.Null);
    }

    [Test]
    public void CurrentUnsignedAssemblyBuilder_NotNull ()
    {
      ObjectFactory.Create<BaseType1>(ParamList.Empty);
      Assert.That (CodeGenerationStatistics.CurrentUnsignedAssemblyBuilder, Is.Not.Null);
      Assert.That (CodeGenerationStatistics.CurrentUnsignedAssemblyBuilder,
          Is.SameAs (ConcreteTypeBuilderTestHelper.GetModuleManager (ConcreteTypeBuilder.Current).Scope.WeakNamedModule.Assembly));
    }

    [Test]
    public void CurrentSignedAssemblyBuilder_Null ()
    {
      Assert.That (CodeGenerationStatistics.CurrentSignedAssemblyBuilder, Is.Null);
    }

    [Test]
    public void CurrentSignedAssemblyBuilder_Null_WithExistingConcreteTypeBuilder ()
    {
      ConcreteTypeBuilder.SetCurrent (ConcreteTypeBuilderObjectMother.CreateConcreteTypeBuilder());
      Assert.That (CodeGenerationStatistics.CurrentSignedAssemblyBuilder, Is.Null);
    }

    [Test]
    public void CurrentSignedAssemblyBuilder_NotNull ()
    {
      TypeGenerationHelper.ForceTypeGeneration (typeof (object));
      Assert.That (CodeGenerationStatistics.CurrentSignedAssemblyBuilder, Is.Not.Null);
      Assert.That (CodeGenerationStatistics.CurrentSignedAssemblyBuilder,
          Is.SameAs (ConcreteTypeBuilderTestHelper.GetModuleManager (ConcreteTypeBuilder.Current).Scope.StrongNamedModule.Assembly));
    }

    [Test]
    public void GetTypesInCurrentUnsignedBuilder_NullBuilder ()
    {
      Assert.That (CodeGenerationStatistics.GetTypesInCurrentSignedBuilder(), Is.Empty);
    }

    [Test]
    public void GetTypesInCurrentUnsignedBuilder_NonNullBuilder ()
    {
      Type t1 = TypeGenerationHelper.ForceTypeGeneration (typeof (BaseType1));
      Type t2 = TypeGenerationHelper.ForceTypeGeneration (typeof (BaseType2));
      Assert.That (CodeGenerationStatistics.GetTypesInCurrentUnsignedBuilder (), Has.Member(t1));
      Assert.That (CodeGenerationStatistics.GetTypesInCurrentUnsignedBuilder (), Has.Member(t2));
    }

    [Test]
    public void GetTypesInCurrentSignedBuilder_NullBuilder ()
    {
      Assert.That (CodeGenerationStatistics.GetTypesInCurrentSignedBuilder (), Is.Empty);
    }

    [Test]
    public void GetTypesInCurrentSignedBuilder_NonNullBuilder ()
    {
      Type t1 = TypeGenerationHelper.ForceTypeGeneration (typeof (object));
      Type t2 = TypeGenerationHelper.ForceTypeGeneration (typeof (List<int>));
      Assert.That (CodeGenerationStatistics.GetTypesInCurrentSignedBuilder (), Has.Member(t1));
      Assert.That (CodeGenerationStatistics.GetTypesInCurrentSignedBuilder (), Has.Member(t2));
    }

    [Test]
    public void CreatedAssemblyCount ()
    {
      AppDomainRunner.Run (
          delegate
          {
            using (MixinConfiguration.BuildNew ().EnterScope ())
            {
              ConcreteTypeBuilder.SetCurrent (null);
              int countBefore = CodeGenerationStatistics.CreatedAssemblyCount;
              TypeGenerationHelper.ForceTypeGeneration (typeof (object));
              Assert.That (CodeGenerationStatistics.CreatedAssemblyCount, Is.EqualTo (countBefore + 1));
              TypeGenerationHelper.ForceTypeGeneration (typeof (BaseType1));
              Assert.That (CodeGenerationStatistics.CreatedAssemblyCount, Is.EqualTo (countBefore + 2));
            }
          });
    }

    [Test]
    public void GetBuiltTypeCount ()
    {
      AppDomainRunner.Run (
          delegate
          {
            using (MixinConfiguration.BuildNew ().EnterScope ())
            {
              ConcreteTypeBuilder.SetCurrent (null);
              int countBefore = CodeGenerationStatistics.GetBuiltTypeCount();
              TypeGenerationHelper.ForceTypeGeneration (typeof (object));
              Assert.That (CodeGenerationStatistics.GetBuiltTypeCount (), Is.EqualTo (countBefore + 2)); // generated type + base call proxy
              TypeGenerationHelper.ForceTypeGeneration (typeof (BaseType1));
              Assert.That (CodeGenerationStatistics.GetBuiltTypeCount (), Is.EqualTo (countBefore + 4)); // generated type + base call proxy
            }
          });
    }
  }
}
