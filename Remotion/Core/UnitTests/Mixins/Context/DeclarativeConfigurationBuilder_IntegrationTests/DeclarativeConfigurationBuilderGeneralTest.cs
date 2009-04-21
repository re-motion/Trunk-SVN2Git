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
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using NUnit.Framework;
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
    public void BuildFromClassContexts()
    {
      MixinConfiguration ac = DeclarativeConfigurationBuilder.BuildConfigurationFromClasses (null, new ClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      Assert.IsFalse (ac.ClassContexts.ContainsWithInheritance (typeof (BaseType2)));

      MixinConfiguration ac2 = DeclarativeConfigurationBuilder.BuildConfigurationFromClasses (ac, new ClassContext (typeof (BaseType2)), new ClassContext (typeof (BaseType3)));
      Assert.IsTrue (ac2.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      Assert.IsTrue (ac2.ClassContexts.ContainsWithInheritance (typeof (BaseType2)));
      Assert.AreEqual (0, ac2.ClassContexts.GetWithInheritance (typeof (BaseType2)).Mixins.Count);
      Assert.IsTrue (ac2.ClassContexts.ContainsWithInheritance (typeof (BaseType3)));

      MixinConfiguration ac3 = DeclarativeConfigurationBuilder.BuildConfigurationFromClasses (ac2, new ClassContext (typeof (BaseType2), typeof (BT2Mixin1)));
      Assert.IsTrue (ac3.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      Assert.IsTrue (ac3.ClassContexts.ContainsWithInheritance (typeof (BaseType2)));
      Assert.AreEqual (1, ac3.ClassContexts.GetWithInheritance (typeof (BaseType2)).Mixins.Count);
      Assert.IsTrue (ac3.ClassContexts.ContainsWithInheritance (typeof (BaseType3)));
    }

    [Test]
    public void BuildFromAssemblies()
    {
      MixinConfiguration ac = DeclarativeConfigurationBuilder.BuildConfigurationFromClasses (null, new ClassContext(typeof (object)));
      Assembly[] assemblies = new Assembly[] { typeof (BaseType1).Assembly, typeof (object).Assembly };
      MixinConfiguration ac2 = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (assemblies);
      Assert.IsTrue (ac2.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      Assert.IsFalse (ac2.ClassContexts.ContainsWithInheritance (typeof (object)));

      MixinConfiguration ac3 = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (ac, assemblies);
      Assert.IsTrue (ac3.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      Assert.IsTrue (ac3.ClassContexts.ContainsWithInheritance (typeof (object)));
      Assert.IsTrue (ac3.ClassContexts.GetWithInheritance (typeof (BaseType6)).CompleteInterfaces.ContainsKey (typeof (ICBT6Mixin1)));
      Assert.AreSame (ac3.ClassContexts.GetWithInheritance (typeof (BaseType6)), ac3.ResolveInterface (typeof (ICBT6Mixin1)));

      MixinConfiguration ac4 = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (ac, (IEnumerable<Assembly>) assemblies);
      Assert.IsTrue (ac4.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      Assert.IsTrue (ac4.ClassContexts.ContainsWithInheritance (typeof (object)));
    }

    [Test]
    public void DoubleAssembliesAreIgnored ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly (), Assembly.GetExecutingAssembly ());

      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (BaseType1));
      Assert.AreEqual (2, classContext.Mixins.Count);

      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (BT1Mixin1)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (BT1Mixin2)));
    }

    [Test]
    public void DoubleTypesAreIgnored ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (BaseType1)).AddType (typeof (BaseType1))
          .AddType (typeof (BT1Mixin1)).AddType (typeof (BT1Mixin1)).AddType (typeof (BT1Mixin2)).AddType (typeof (BT1Mixin2)).BuildConfiguration ();

      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (BaseType1));
      Assert.AreEqual (2, classContext.Mixins.Count);

      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (BT1Mixin1)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (BT1Mixin2)));
    }

    [Test]
    public void BuildDefault()
    {
      MixinConfiguration ac = DeclarativeConfigurationBuilder.BuildDefaultConfiguration();
      Assert.IsNotNull (ac);
      Assert.AreNotEqual (0, ac.ClassContexts.Count);
    }

    [Test]
    public void BuildDefault_DoesNotLockPersistedFile ()
    {
      ConcreteTypeBuilder.SetCurrent (null);
      TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration);
      string[] paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope();
      try
      {
        Assert.AreEqual (1, paths.Length);
        ContextAwareTypeDiscoveryUtility.SetDefaultService (null);
        DeclarativeConfigurationBuilder.BuildDefaultConfiguration ();
      }
      finally
      {
        File.Delete (paths[0]);
      }
    }

    [Extends (typeof (BaseType1))]
    [IgnoreForMixinConfiguration]
    public class Foo { }

    [Test]
    public void IgnoreForMixinConfiguration()
    {
      Assert.IsFalse (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)).Mixins.ContainsKey (typeof (Foo)));
    }

    [Test]
    public void FilterExcludesSystemAssemblies ()
    {
      AssemblyFinderTypeDiscoveryService service =
          (AssemblyFinderTypeDiscoveryService)
              PrivateInvoke.InvokeNonPublicStaticMethod (typeof (DeclarativeConfigurationBuilder), "GetTypeDiscoveryService");
      Assert.IsFalse (service.AssemblyFinder.Filter.ShouldConsiderAssembly (typeof (object).Assembly.GetName ()));
      Assert.IsFalse (service.AssemblyFinder.Filter.ShouldConsiderAssembly (typeof (Uri).Assembly.GetName ()));
    }

    [Test]
    public void FilterExcludesGeneratedAssemblies ()
    {
      AssemblyFinderTypeDiscoveryService service =
          (AssemblyFinderTypeDiscoveryService)
              PrivateInvoke.InvokeNonPublicStaticMethod (typeof (DeclarativeConfigurationBuilder), "GetTypeDiscoveryService");
      
      Assembly signedAssembly = TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration).Assembly;
      Assembly unsignedAssembly = TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration).Assembly;

      Assert.IsTrue (ReflectionUtility.IsAssemblySigned (signedAssembly));
      Assert.IsFalse (ReflectionUtility.IsAssemblySigned (unsignedAssembly));

      Assert.IsFalse (service.AssemblyFinder.Filter.ShouldIncludeAssembly (signedAssembly));
      Assert.IsFalse (service.AssemblyFinder.Filter.ShouldIncludeAssembly (unsignedAssembly));
    }

    [Test]
    public void FilterIncludesAllNormalAssemblies ()
    {
      AssemblyFinderTypeDiscoveryService service =
          (AssemblyFinderTypeDiscoveryService)
              PrivateInvoke.InvokeNonPublicStaticMethod (typeof (DeclarativeConfigurationBuilder), "GetTypeDiscoveryService");

      Assert.IsTrue (service.AssemblyFinder.Filter.ShouldConsiderAssembly (typeof (DeclarativeConfigurationBuilderGeneralTest).Assembly.GetName ()));
      Assert.IsTrue (service.AssemblyFinder.Filter.ShouldConsiderAssembly (typeof (DeclarativeConfigurationBuilder).Assembly.GetName ()));
      Assert.IsTrue (service.AssemblyFinder.Filter.ShouldConsiderAssembly (new AssemblyName ("whatever")));

      Assert.IsTrue (service.AssemblyFinder.Filter.ShouldIncludeAssembly (typeof (DeclarativeConfigurationBuilderGeneralTest).Assembly));
      Assert.IsTrue (service.AssemblyFinder.Filter.ShouldIncludeAssembly (typeof (DeclarativeConfigurationBuilder).Assembly));
    }

    [Test]
    public void DesignModeIsDetected ()
    {
      ITypeDiscoveryService service =
          (ITypeDiscoveryService) PrivateInvoke.InvokeNonPublicStaticMethod (typeof (DeclarativeConfigurationBuilder), "GetTypeDiscoveryService");
      Assert.IsInstanceOfType (typeof (AssemblyFinderTypeDiscoveryService), service);

      MockRepository repository = new MockRepository();
      IDesignModeHelper designModeHelperMock = repository.StrictMock<IDesignModeHelper> ();
      IDesignerHost designerHostMock = repository.StrictMock<IDesignerHost>();
      ITypeDiscoveryService designerServiceMock = repository.StrictMock<ITypeDiscoveryService> ();

      Expect.Call (designModeHelperMock.DesignerHost).Return (designerHostMock);
      Expect.Call (designerHostMock.GetService (typeof (ITypeDiscoveryService))).Return (designerServiceMock);

      repository.ReplayAll();
      
      DesignerUtility.SetDesignMode (designModeHelperMock);
      try
      {
        service = (ITypeDiscoveryService) PrivateInvoke.InvokeNonPublicStaticMethod (typeof (DeclarativeConfigurationBuilder), "GetTypeDiscoveryService");

        Assert.IsNotInstanceOfType (typeof (AssemblyFinderTypeDiscoveryService), service);
        Assert.AreSame (designerServiceMock, service);
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
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());

      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (TargetClassWithAdditionalDependencies));
      Assert.IsNotNull (classContext);

      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinWithAdditionalClassDependency)));
      Assert.IsTrue (classContext.Mixins[typeof (MixinWithAdditionalClassDependency)].ExplicitDependencies.ContainsKey (typeof (MixinWithNoAdditionalDependency)));
    }

    [Test]
    public void MixinAttributeOnMixinClass ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());

      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (BaseType1));
      Assert.IsNotNull (classContext);

      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (BT1Mixin1)));
    }

    [Test]
    public void CompleteInterfaceConfiguredViaAttribute ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());

      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (BaseType6));
      Assert.IsNotNull (classContext);

      Assert.IsTrue (classContext.CompleteInterfaces.ContainsKey (typeof (ICBT6Mixin1)));
      Assert.IsTrue (classContext.CompleteInterfaces.ContainsKey (typeof (ICBT6Mixin2)));
      Assert.IsTrue (classContext.CompleteInterfaces.ContainsKey (typeof (ICBT6Mixin3)));
    }
  }
}
