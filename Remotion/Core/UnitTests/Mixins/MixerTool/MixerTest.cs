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
using System.IO;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.MixerTool;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.CodeGeneration.DynamicProxy;
using Remotion.UnitTests.Mixins.SampleTypes;
using Rhino.Mocks;
using System.Linq;
using ErrorEventArgs=Remotion.Mixins.MixerTool.ErrorEventArgs;

namespace Remotion.UnitTests.Mixins.MixerTool
{
  [Serializable]
  [TestFixture]
  public class MixerTest : MixerToolBaseTest
  {
    [Test]
    public void Execute_Unsigned ()
    {
      Assert.That (File.Exists (UnsignedAssemblyPath), Is.False);

      using (MixinConfiguration.BuildNew ().ForClass (typeof (MixerTest)).AddMixin (typeof (object)).EnterScope ())
      {
        var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
        mixer.Execute ();
      }
      Assert.That (File.Exists (UnsignedAssemblyPath), Is.True);
    }

    [Test]
    public void Execute_Signed ()
    {
      Assert.That (File.Exists (SignedAssemblyPath), Is.False);
      using (MixinConfiguration.BuildNew().ForClass (typeof (List<List<List<int>>>)).AddMixin(typeof (object)).EnterScope())
      {
        var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
        mixer.Execute();
      }
      Assert.That (File.Exists (SignedAssemblyPath), Is.True);
    }

    [Test]
    public void GeneratesFileInRightDirectory ()
    {
      string outputDirectory = Path.Combine (Environment.CurrentDirectory, "MixinTool.Output");
      if (Directory.Exists (outputDirectory))
        Directory.Delete (outputDirectory, true);

      string outputPath = Path.Combine (outputDirectory, Parameters.UnsignedAssemblyName + ".dll");
      Assert.That (File.Exists (outputPath), Is.False);
      var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, outputDirectory);
      mixer.Execute();
      Assert.That (File.Exists (outputPath), Is.True);
    }

    [Test]
    public void MixerCanBeExecutedTwice ()
    {
      var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      mixer.Execute ();
      mixer.Execute ();
    }

    [Test]
    public void MixerToolCanBeRunTwice_InIsolation ()
    {
      AppDomainRunner.Run (
          delegate
          {
            var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            mixer.Execute ();
          });
      AppDomainRunner.Run (
          delegate
          {
            var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            mixer.Execute ();
          });
    }

    [Test]
    public void DefaultConfigurationIsProcessed ()
    {
        var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
        mixer.Execute ();

        Set<ClassContext> contextsFromConfig = GetExpectedDefaultContexts ();
        Assert.That (mixer.ProcessedContexts.Values, Is.EquivalentTo (contextsFromConfig));
    }

    [Test]
    public void TypesAreGeneratedForProcessedContexts ()
    {
      AppDomainRunner.Run (
          delegate
          {
            var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            mixer.Execute();
            
            Set<ClassContext> contextsFromTypes = GetContextsFromGeneratedTypes (Assembly.LoadFile (UnsignedAssemblyPath));
            contextsFromTypes.AddRange (GetContextsFromGeneratedTypes (Assembly.LoadFile (SignedAssemblyPath)));
            
            Set<ClassContext> contextsFromConfig = GetExpectedDefaultContexts();
            Assert.That (contextsFromTypes, Is.EquivalentTo (contextsFromConfig));
          });
    }

    [Test]
    public void NoErrorsInDefaultConfiguration ()
    {
      var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      mixer.Execute ();
      Assert.IsEmpty (mixer.Errors);
    }

    [Test]
    public void GenericTypeDefinitionsAreIgnored ()
    {
      var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      mixer.Execute ();
      Assert.That (
                  new List<ClassContext> (mixer.ProcessedContexts.Values).TrueForAll (c => !c.Type.IsGenericTypeDefinition), Is.True);
    }

    [Test]
    public void InterfacesAreIgnored ()
    {
      var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      mixer.Execute ();
      Assert.That (new List<ClassContext> (mixer.ProcessedContexts.Values).TrueForAll (c => !c.Type.IsInterface), Is.True);
    }

    [Test]
    public void TypesWithIgnoreForMixinConfiguration_AreIgnored ()
    {
      var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      mixer.Execute ();
      Assert.That (mixer.ProcessedContexts.Values.Select (c => c.Type).ToArray(), List.Not.Contains (typeof (FakeConcreteMixedType)));
    }

    [Test]
    public void ActiveMixinConfigurationIsProcessed ()
    {
      var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      using (MixinConfiguration.BuildNew().ForClass<BaseType1>().Clear().AddMixins (typeof (BT1Mixin1)).EnterScope())
      {
        mixer.Execute ();
        Assert.That (mixer.ProcessedContexts.Count, Is.EqualTo (1));
        Assert.That(mixer.ProcessedContexts.Values, Is.EquivalentTo (MixinConfiguration.ActiveConfiguration.ClassContexts));
      }
    }

    [Test]
    public void ProcessesSubclassesOfTargetTypes ()
    {
      var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      
      using (MixinConfiguration.BuildNew().ForClass<NullTarget>().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        Assert.That (MixinTypeUtility.HasMixin (typeof (DerivedNullTarget), typeof (NullMixin)), Is.True);
        mixer.Execute ();
      }

      Assert.That (mixer.ProcessedContexts.ContainsKey (typeof (DerivedNullTarget)), Is.True);
    }

    [Test]
    public void HandlesClosedGenericTypes ()
    {
      var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      using (MixinConfiguration.BuildNew ().ForClass (typeof (List<List<List<int>>>)).Clear ().AddMixins (typeof (NullMixin)).EnterScope ())
      {
        mixer.Execute ();
        Assert.IsEmpty (mixer.Errors);
      }

      Assert.That (mixer.FinishedTypes.ContainsKey (typeof (List<List<List<int>>>)), Is.True);
    }

    [Test]
    public void MixerLeavesCurrentTypeBuilderUnchanged ()
    {
      try
      {
        var repository = new MockRepository ();
        ConcreteTypeBuilder builder = ConcreteTypeBuilder.Current;
        var scopeMock = repository.StrictMock<IModuleManager> ();
        builder.Scope = scopeMock;

        // expect no calls on scope

        repository.ReplayAll ();

        new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory).Execute ();

        repository.VerifyAll ();

        Assert.That (ConcreteTypeBuilder.Current, Is.SameAs (builder));
        Assert.That (ConcreteTypeBuilder.Current.Scope, Is.SameAs (scopeMock));
      }
      finally
      {
        ConcreteTypeBuilder.SetCurrent (null);
      }
    }
    
    [Test]
    public void MixerIgnoresInvalidTypes ()
    {
      var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        using (MixinConfiguration.BuildNew ()
            .ForClass<BaseType1> ().Clear ().AddMixins (typeof (BT1Mixin1))  // valid
            .ForClass<BaseType2>().Clear().AddMixins (typeof (BT1Mixin1))  // invalid
            .EnterScope())
        {
          mixer.Execute ();
          Assert.That (mixer.Errors.Count, Is.EqualTo (1));
          Assert.That (mixer.Errors[0].A, Is.EqualTo (MixinConfiguration.ActiveConfiguration.ClassContexts.GetExact (typeof (BaseType2))));
          Assert.That (mixer.FinishedTypes.Keys, Is.EquivalentTo (new object[] { typeof (BaseType1) }));
        }
      }
    }

    [Test]
    public void ValidationErrorOccurred ()
    {
      var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      using (MixinConfiguration.BuildNew ()
          .ForClass<BaseType1> ().Clear ().AddMixins (typeof (BaseType1))  // yields ValidationException
          .EnterScope ())
      {
        object eventSender = null;
        ValidationErrorEventArgs eventArgs = null;
        mixer.ValidationErrorOccurred += (sender, args) => { eventSender = sender; eventArgs = args; };

        mixer.Execute ();

        Assert.That (eventSender, Is.EqualTo (mixer));
        Assert.That (eventArgs.ValidationException.ValidationLog.GetNumberOfFailures (), Is.GreaterThan (0));
      }
    }

    [Test]
    public void ErrorOccurred ()
    {
      var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      using (MixinConfiguration.BuildNew ()
          .ForClass<BaseType2> ().Clear ().AddMixins (typeof (BT1Mixin1))  // yields ConfigurationException
          .EnterScope ())
      {
        object eventSender = null;
        ErrorEventArgs eventArgs = null;
        mixer.ErrorOccurred += (sender, args) => { eventSender = sender; eventArgs = args; };

        mixer.Execute ();

        Assert.That (eventSender, Is.EqualTo (mixer));
        Assert.That (eventArgs.Exception, Is.InstanceOfType (typeof (ConfigurationException)));
      }
    }

    [Test]
    public void MixerRaisesEventForEachClassContextBeingProcessed ()
    {
      var classContextsBeingProcessed = new List<ClassContext>();

      // only use this assembly for this test case
      using (DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (typeof (MixerTest).Assembly).EnterScope())
      {
        var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
        mixer.ClassContextBeingProcessed +=
            ((sender, args) => classContextsBeingProcessed.Add (args.ClassContext));
        mixer.Execute();
        Assert.That (classContextsBeingProcessed, Is.EquivalentTo (mixer.ProcessedContexts.Values));
      }
    }

    private class FooNameProvider : INameProvider
    {
      public string GetNewTypeName (ClassDefinitionBase configuration)
      {
        return "Foo";
      }
    }

    [Test]
    public void UsesGivenNameProvider ()
    {
      var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      mixer.NameProvider = new FooNameProvider();
      using (MixinConfiguration.BuildNew().ForClass<BaseType1>().Clear().AddMixins (typeof (BT1Mixin1)).EnterScope())
      {
        mixer.Execute ();
        Assert.That (mixer.FinishedTypes[typeof (BaseType1)].FullName, Is.EqualTo ("Foo"));
      }
    }

    [Test]
    public void SameInputAndOutputDirectory ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (List<List<List<int>>>)).AddMixin (typeof (object))
          .ForClass (typeof (MixerTest)).AddMixin (typeof (object))
          .EnterScope ())
      {
        Assert.That (File.Exists (UnsignedAssemblyPath), Is.False);
        Assert.That (File.Exists (SignedAssemblyPath), Is.False);

        var mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, AppDomain.CurrentDomain.BaseDirectory);
        mixer.Execute ();

        Assert.That (File.Exists (UnsignedAssemblyPath), Is.True);
        Assert.That (File.Exists (SignedAssemblyPath), Is.True);

        ContextAwareTypeDiscoveryUtility.SetDefaultService (null); // trigger reloading of assemblies

        // trigger reanalysis of the default mixin configuration
        MixinConfiguration.SetActiveConfiguration (null);
        MixinConfiguration.ResetMasterConfiguration();

        mixer.Execute (); // this should _not_ load/lock the generated files

        File.Delete (UnsignedAssemblyPath);
        File.Delete (SignedAssemblyPath);
      }
    }

    private Set<ClassContext> GetExpectedDefaultContexts ()
    {
      var contextsFromConfig = new Set<ClassContext> ();
      foreach (ClassContext context in MixinConfiguration.ActiveConfiguration.ClassContexts)
      {
        if (!context.Type.IsGenericTypeDefinition && !context.Type.IsInterface)
        {
          contextsFromConfig.Add (context);
        }
      }

      foreach (Type t in ContextAwareTypeDiscoveryUtility.GetInstance ().GetTypes (null, false))
      {
        if (!t.IsGenericTypeDefinition && !t.IsInterface && !t.IsDefined (typeof (IgnoreForMixinConfigurationAttribute), false))
        {
          ClassContext context = MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (t);
          if (context != null)
            contextsFromConfig.Add (context);
        }
      }
      return contextsFromConfig;
    }
  }
}
