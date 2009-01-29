// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Diagnostics;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Utilities;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins
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
      ConcreteTypeBuilder.SetCurrent (new ConcreteTypeBuilder());
      Assert.That (CodeGenerationStatistics.CurrentUnsignedAssemblyBuilder, Is.Null);
    }

    [Test]
    public void CurrentUnsignedAssemblyBuilder_NotNull ()
    {
      ObjectFactory.Create<BaseType1>(ParamList.Empty);
      Assert.That (CodeGenerationStatistics.CurrentUnsignedAssemblyBuilder, Is.Not.Null);
      Assert.That (CodeGenerationStatistics.CurrentUnsignedAssemblyBuilder, 
          Is.SameAs (((ModuleManager) ConcreteTypeBuilder.Current.Scope).Scope.WeakNamedModule.Assembly));
    }

    [Test]
    public void CurrentSignedAssemblyBuilder_Null ()
    {
      Assert.That (CodeGenerationStatistics.CurrentSignedAssemblyBuilder, Is.Null);
    }

    [Test]
    public void CurrentSignedAssemblyBuilder_Null_WithExistingConcreteTypeBuilder ()
    {
      ConcreteTypeBuilder.SetCurrent (new ConcreteTypeBuilder ());
      Assert.That (CodeGenerationStatistics.CurrentSignedAssemblyBuilder, Is.Null);
    }

    [Test]
    public void CurrentSignedAssemblyBuilder_NotNull ()
    {
      ObjectFactory.Create<object> (ParamList.Empty, GenerationPolicy.ForceGeneration);
      Assert.That (CodeGenerationStatistics.CurrentSignedAssemblyBuilder, Is.Not.Null);
      Assert.That (CodeGenerationStatistics.CurrentSignedAssemblyBuilder,
          Is.SameAs (((ModuleManager) ConcreteTypeBuilder.Current.Scope).Scope.StrongNamedModule.Assembly));
    }

    [Test]
    public void GetTypesInCurrentUnsignedBuilder_NullBuilder ()
    {
      Assert.That (CodeGenerationStatistics.GetTypesInCurrentSignedBuilder(), Is.Empty);
    }

    [Test]
    public void GetTypesInCurrentUnsignedBuilder_NonNullBuilder ()
    {
      Type t1 = TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration);
      Type t2 = TypeFactory.GetConcreteType (typeof (BaseType2), GenerationPolicy.ForceGeneration);
      Assert.That (CodeGenerationStatistics.GetTypesInCurrentUnsignedBuilder (), List.Contains (t1));
      Assert.That (CodeGenerationStatistics.GetTypesInCurrentUnsignedBuilder (), List.Contains (t2));
    }

    [Test]
    public void GetTypesInCurrentSignedBuilder_NullBuilder ()
    {
      Assert.That (CodeGenerationStatistics.GetTypesInCurrentSignedBuilder (), Is.Empty);
    }

    [Test]
    public void GetTypesInCurrentSignedBuilder_NonNullBuilder ()
    {
      Type t1 = TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration);
      Type t2 = TypeFactory.GetConcreteType (typeof (List<int>), GenerationPolicy.ForceGeneration);
      Assert.That (CodeGenerationStatistics.GetTypesInCurrentSignedBuilder (), List.Contains (t1));
      Assert.That (CodeGenerationStatistics.GetTypesInCurrentSignedBuilder (), List.Contains (t2));
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
              TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration);
              Assert.That (CodeGenerationStatistics.CreatedAssemblyCount, Is.EqualTo (countBefore + 1));
              TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration);
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
              TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration);
              Assert.That (CodeGenerationStatistics.GetBuiltTypeCount (), Is.EqualTo (countBefore + 2)); // generated type + base call proxy
              TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration);
              Assert.That (CodeGenerationStatistics.GetBuiltTypeCount (), Is.EqualTo (countBefore + 4)); // generated type + base call proxy
            }
          });
    }
  }
}
