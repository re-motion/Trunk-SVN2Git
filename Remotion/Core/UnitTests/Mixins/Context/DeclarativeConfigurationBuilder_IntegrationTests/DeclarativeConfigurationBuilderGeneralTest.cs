// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Design;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Utilities;
using Rhino.Mocks;
using ReflectionUtility=Remotion.Mixins.Utilities.ReflectionUtility;

namespace Remotion.UnitTests.Mixins.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class DeclarativeConfigurationBuilderGeneralTest
  {
    [Test]
    public void BuildFromAssemblies()
    {
      var assemblies = new[] { typeof (BaseType1).Assembly, typeof (object).Assembly };
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (assemblies);

      Assert.That (configuration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)), Is.True);
      Assert.That (configuration.ClassContexts.ContainsWithInheritance (typeof (object)), Is.False);
    }

    [Test]
    public void BuildFromAssemblies_WithParentConfiguration ()
    {
      var parentConfiguration = new MixinConfiguration (new ClassContextCollection (new ClassContext (typeof (object))));

      var assemblies = new[] { typeof (BaseType1).Assembly, typeof (object).Assembly };
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (parentConfiguration, assemblies);

      Assert.That (configuration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)), Is.True);
      Assert.That (configuration.ClassContexts.ContainsWithInheritance (typeof (object)), Is.True);
      Assert.That (configuration.GetContext (typeof (BaseType6)).CompleteInterfaces.ContainsKey (typeof (ICBT6Mixin1)), Is.True);
      Assert.That (configuration.ResolveCompleteInterface (typeof (ICBT6Mixin1)), Is.SameAs (configuration.GetContext (typeof (BaseType6))));
    }

    [Test]
    public void DuplicateAssembliesAreIgnored ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (
          Assembly.GetExecutingAssembly (), 
          Assembly.GetExecutingAssembly ());

      ClassContext classContext = configuration.GetContext (typeof (BaseType1));
      Assert.That (classContext.Mixins.Count, Is.EqualTo (2));

      Assert.That (classContext.Mixins.ContainsKey (typeof (BT1Mixin1)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (BT1Mixin2)), Is.True);
    }

    [Test]
    public void DuplicateTypesAreIgnored ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder (null)
          .AddType (typeof (BaseType1))
          .AddType (typeof (BaseType1))
          .AddType (typeof (BT1Mixin1))
          .AddType (typeof (BT1Mixin1))
          .AddType (typeof (BT1Mixin2))
          .AddType (typeof (BT1Mixin2))
          .BuildConfiguration ();

      ClassContext classContext = configuration.GetContext (typeof (BaseType1));
      Assert.That (classContext.Mixins.Count, Is.EqualTo (2));

      Assert.That (classContext.Mixins.ContainsKey (typeof (BT1Mixin1)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (BT1Mixin2)), Is.True);
    }

    [Test]
    public void BuildDefault()
    {
      MixinConfiguration ac = DeclarativeConfigurationBuilder.BuildDefaultConfiguration();
      Assert.That (ac, Is.Not.Null);
      Assert.That (ac.ClassContexts.Count, Is.Not.EqualTo (0));
    }

    [Test]
    public void BuildDefault_DoesNotLockPersistedFile ()
    {
      ConcreteTypeBuilder.SetCurrent (null);
      TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration);
      string[] paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope();
      try
      {
        Assert.That (paths.Length, Is.EqualTo (1));
        ContextAwareTypeDiscoveryUtility.DefaultNonDesignModeService = null;
        DeclarativeConfigurationBuilder.BuildDefaultConfiguration ();
      }
      finally
      {
        File.Delete (paths[0]);
      }
    }

    [Test]
    public void IgnoreForMixinConfiguration()
    {
      MixinConfiguration ac = DeclarativeConfigurationBuilder.BuildConfigurationFromTypes (
          null, 
          new[] { typeof (BaseType1), typeof (BT1Mixin1), typeof (MixinWithIgnoreForMixinConfigurationAttribute) });

      Assert.That (ac.ClassContexts.GetExact (typeof (BaseType1)).Mixins.ContainsKey (typeof (MixinWithIgnoreForMixinConfigurationAttribute)), 
          Is.False);
    }

    [Test]
    public void FilterExcludesSystemAssemblies ()
    {
      var service =
          (AssemblyFinderTypeDiscoveryService)
              PrivateInvoke.InvokeNonPublicStaticMethod (typeof (DeclarativeConfigurationBuilder), "GetTypeDiscoveryService");
      Assert.That (service.AssemblyFinder.Filter.ShouldConsiderAssembly (typeof (object).Assembly.GetName ()), Is.False);
      Assert.That (service.AssemblyFinder.Filter.ShouldConsiderAssembly (typeof (Uri).Assembly.GetName ()), Is.False);
    }

    [Test]
    public void FilterExcludesGeneratedAssemblies ()
    {
      var service =
          (AssemblyFinderTypeDiscoveryService)
              PrivateInvoke.InvokeNonPublicStaticMethod (typeof (DeclarativeConfigurationBuilder), "GetTypeDiscoveryService");
      
      Assembly signedAssembly = TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration).Assembly;
      Assembly unsignedAssembly = TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration).Assembly;

      Assert.That (ReflectionUtility.IsAssemblySigned (signedAssembly), Is.True);
      Assert.That (ReflectionUtility.IsAssemblySigned (unsignedAssembly), Is.False);

      Assert.That (service.AssemblyFinder.Filter.ShouldIncludeAssembly (signedAssembly), Is.False);
      Assert.That (service.AssemblyFinder.Filter.ShouldIncludeAssembly (unsignedAssembly), Is.False);
    }

    [Test]
    public void FilterIncludesAllNormalAssemblies ()
    {
      var service =
          (AssemblyFinderTypeDiscoveryService)
              PrivateInvoke.InvokeNonPublicStaticMethod (typeof (DeclarativeConfigurationBuilder), "GetTypeDiscoveryService");

      Assert.That (service.AssemblyFinder.Filter.ShouldConsiderAssembly (typeof (DeclarativeConfigurationBuilderGeneralTest).Assembly.GetName ()), Is.True);
      Assert.That (service.AssemblyFinder.Filter.ShouldConsiderAssembly (typeof (DeclarativeConfigurationBuilder).Assembly.GetName ()), Is.True);
      Assert.That (service.AssemblyFinder.Filter.ShouldConsiderAssembly (new AssemblyName ("whatever")), Is.True);

      Assert.That (service.AssemblyFinder.Filter.ShouldIncludeAssembly (typeof (DeclarativeConfigurationBuilderGeneralTest).Assembly), Is.True);
      Assert.That (service.AssemblyFinder.Filter.ShouldIncludeAssembly (typeof (DeclarativeConfigurationBuilder).Assembly), Is.True);
    }

    [Test]
    public void DesignModeIsDetected ()
    {
      var service =
          (ITypeDiscoveryService) PrivateInvoke.InvokeNonPublicStaticMethod (typeof (DeclarativeConfigurationBuilder), "GetTypeDiscoveryService");
      Assert.That (service, Is.InstanceOfType (typeof (AssemblyFinderTypeDiscoveryService)));

      var repository = new MockRepository();
      var designModeHelperMock = repository.StrictMock<IDesignModeHelper> ();
      var designerHostMock = repository.StrictMock<IDesignerHost>();
      var designerServiceMock = repository.StrictMock<ITypeDiscoveryService> ();

      Expect.Call (designModeHelperMock.DesignerHost).Return (designerHostMock);
      Expect.Call (designerHostMock.GetService (typeof (ITypeDiscoveryService))).Return (designerServiceMock);

      repository.ReplayAll();
      
      DesignerUtility.SetDesignMode (designModeHelperMock);
      try
      {
        service = (ITypeDiscoveryService) PrivateInvoke.InvokeNonPublicStaticMethod (typeof (DeclarativeConfigurationBuilder), "GetTypeDiscoveryService");

        Assert.That (service, Is.Not.InstanceOfType (typeof (AssemblyFinderTypeDiscoveryService)));
        Assert.That (service, Is.SameAs (designerServiceMock));
      }
      finally
      {
        DesignerUtility.ClearDesignMode();
      }

      repository.VerifyAll ();
    }

    [Test]
    public void MixinAttributeOnTargetClass ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());

      ClassContext classContext = configuration.GetContext (typeof (TargetClassWithAdditionalDependencies));
      Assert.That (classContext, Is.Not.Null);

      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinWithAdditionalClassDependency)), Is.True);
      Assert.That (classContext.Mixins[typeof (MixinWithAdditionalClassDependency)].ExplicitDependencies.ContainsKey (typeof (MixinWithNoAdditionalDependency)), Is.True);
    }

    [Test]
    public void MixinAttributeOnMixinClass ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());

      ClassContext classContext = configuration.GetContext (typeof (BaseType1));
      Assert.That (classContext, Is.Not.Null);

      Assert.That (classContext.Mixins.ContainsKey (typeof (BT1Mixin1)), Is.True);
    }

    [Test]
    public void CompleteInterfaceConfiguredViaAttribute ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());

      ClassContext classContext = configuration.GetContext (typeof (BaseType6));
      Assert.That (classContext, Is.Not.Null);

      Assert.That (classContext.CompleteInterfaces.ContainsKey (typeof (ICBT6Mixin1)), Is.True);
      Assert.That (classContext.CompleteInterfaces.ContainsKey (typeof (ICBT6Mixin2)), Is.True);
      Assert.That (classContext.CompleteInterfaces.ContainsKey (typeof (ICBT6Mixin3)), Is.True);
    }

    [Extends (typeof (BaseType1))]
    [IgnoreForMixinConfiguration]
    public class MixinWithIgnoreForMixinConfigurationAttribute { }
  }
}
