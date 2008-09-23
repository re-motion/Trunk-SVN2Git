/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
      ObjectFactory.Create<BaseType1>().With();
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
      ObjectFactory.Create<object> (GenerationPolicy.ForceGeneration).With ();
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