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
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Configuration;
using Remotion.Configuration.TypeDiscovery;
using Remotion.Development.UnitTesting.Configuration;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;

namespace Remotion.UnitTests.Configuration.TypeDiscovery
{
  [TestFixture]
  public class TypeDiscoveryConfigurationTest
  {
    private const string _xmlFragmentDefault = @"<typeDiscovery xmlns=""..."" />";
    private const string _xmlFragmentWithAutoRootAssemblyFinder = @"<typeDiscovery mode=""Automatic"" xmlns=""..."">
      </typeDiscovery>";
    private const string _xmlFragmentWithCustomRootAssemblyFinder = @"<typeDiscovery mode=""CustomRootAssemblyFinder"" xmlns=""..."">
        <customRootAssemblyFinder type=""Remotion.UnitTests::Configuration.TypeDiscovery.FakeRootAssemblyFinder""/>
      </typeDiscovery>";
    private const string _xmlFragmentWithMissingCustomRootAssemblyFinder = @"<typeDiscovery mode=""CustomRootAssemblyFinder"" xmlns=""..."" />";
    private const string _xmlFragmentWithSpecificRootAssemblies = @"<typeDiscovery mode=""SpecificRootAssemblies"" xmlns=""..."">
        <specificRootAssemblies>
          <byName>
            <include name=""mscorlib""/>
          </byName>
        </specificRootAssemblies>
      </typeDiscovery>";
    private const string _xmlFragmentWithSpecificEmptyRootAssemblies = @"<typeDiscovery mode=""SpecificRootAssemblies"" xmlns=""..."">
        <specificRootAssemblies />
      </typeDiscovery>";
    private const string _xmlFragmentWithCustomTypeDiscoveryService = @"<typeDiscovery mode=""CustomTypeDiscoveryService"" xmlns=""..."">
        <customTypeDiscoveryService type=""Remotion.UnitTests::Configuration.TypeDiscovery.FakeTypeDiscoveryService""/>
      </typeDiscovery>";
    private const string _xmlFragmentWithMissingCustomTypeDiscoveryService = @"<typeDiscovery mode=""CustomTypeDiscoveryService"" xmlns=""..."" />";

    [Test]
    public void Deserialization_Default ()
    {
      var section = Deserialize (_xmlFragmentDefault);
      Assert.That (section.Mode, Is.EqualTo (TypeDiscoveryMode.Automatic));
    }

    [Test]
    public void Deserialization_Auto ()
    {
      var section = Deserialize (_xmlFragmentWithAutoRootAssemblyFinder);
      Assert.That (section.Mode, Is.EqualTo (TypeDiscoveryMode.Automatic));
    }

    [Test]
    public void Deserialization_CustomSpecificRootAssemblyFinder ()
    {
      var section = Deserialize (_xmlFragmentWithCustomRootAssemblyFinder);
      Assert.That (section.Mode, Is.EqualTo (TypeDiscoveryMode.CustomRootAssemblyFinder));
      Assert.That (section.CustomRootAssemblyFinder.Type, Is.SameAs (typeof (FakeRootAssemblyFinder)));
    }

    [Test]
    public void Deserialization_SpecificRootAssemblies ()
    {
      var section = Deserialize (_xmlFragmentWithSpecificRootAssemblies);
      Assert.That (section.Mode, Is.EqualTo (TypeDiscoveryMode.SpecificRootAssemblies));
      Assert.That (section.SpecificRootAssemblies.ByName.Single().Name, Is.EqualTo ("mscorlib"));
    }

    [Test]
    public void Deserialization_SpecificEmptyRootAssemblies ()
    {
      var section = Deserialize (_xmlFragmentWithSpecificEmptyRootAssemblies);
      Assert.That (section.Mode, Is.EqualTo (TypeDiscoveryMode.SpecificRootAssemblies));
      Assert.That (section.SpecificRootAssemblies.ByName.Count, Is.EqualTo (0));
      Assert.That (section.SpecificRootAssemblies.ByFile.Count, Is.EqualTo (0));
    }

    [Test]
    public void Deserialization_CustomTypeDiscoveryService ()
    {
      var section = Deserialize (_xmlFragmentWithCustomTypeDiscoveryService);
      Assert.That (section.Mode, Is.EqualTo (TypeDiscoveryMode.CustomTypeDiscoveryService));
      Assert.That (section.CustomTypeDiscoveryService.Type, Is.EqualTo (typeof (FakeTypeDiscoveryService)));
    }

    [Test]
    public void CreateTypeDiscoveryService_Auto ()
    {
      var section = Deserialize (_xmlFragmentWithAutoRootAssemblyFinder);

      var service = section.CreateTypeDiscoveryService ();

      Assert.That (service, Is.InstanceOfType (typeof (AssemblyFinderTypeDiscoveryService)));
      var assemblyFinder = (AssemblyFinder) ((AssemblyFinderTypeDiscoveryService) service).AssemblyFinder;
      Assert.That (assemblyFinder.RootAssemblyFinder, Is.InstanceOfType (typeof (SearchPathRootAssemblyFinder)));

      var searchPathRootAssemblyFinder = (SearchPathRootAssemblyFinder) assemblyFinder.RootAssemblyFinder;
      Assert.That (searchPathRootAssemblyFinder.BaseDirectory, Is.EqualTo (AppDomain.CurrentDomain.BaseDirectory));

      Assert.That (assemblyFinder.AssemblyLoader, Is.InstanceOfType (typeof (FilteringAssemblyLoader)));

      var castLoader = (FilteringAssemblyLoader) assemblyFinder.AssemblyLoader;
      Assert.That (castLoader.Filter, Is.SameAs (ApplicationAssemblyLoaderFilter.Instance));
    }

    [Test]
    public void CreateTypeDiscoveryService_CustomRootAssemblyFinder ()
    {
      var section = Deserialize (_xmlFragmentWithCustomRootAssemblyFinder);

      var service = section.CreateTypeDiscoveryService ();

      Assert.That (service, Is.InstanceOfType (typeof (AssemblyFinderTypeDiscoveryService)));
      var assemblyFinder = (AssemblyFinder) ((AssemblyFinderTypeDiscoveryService) service).AssemblyFinder;
      Assert.That (assemblyFinder.RootAssemblyFinder, Is.InstanceOfType (typeof (FakeRootAssemblyFinder)));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException), 
        ExpectedMessage = "In CustomRootAssemblyFinder mode, a custom root assembly finder must be specified in the type discovery configuration.", 
        MatchType = MessageMatch.Contains)]
    public void CreateTypeDiscoveryService_CustomRootAssemblyFinder_MissingType ()
    {
      var section = Deserialize (_xmlFragmentWithMissingCustomRootAssemblyFinder);
      section.CreateTypeDiscoveryService ();
    }

    [Test]
    public void CreateTypeDiscoveryService_SpecificRootAssemblies ()
    {
      var section = Deserialize (_xmlFragmentWithSpecificRootAssemblies);

      var service = section.CreateTypeDiscoveryService ();

      Assert.That (service, Is.InstanceOfType (typeof (AssemblyFinderTypeDiscoveryService)));
      var assemblyFinder = (AssemblyFinder) ((AssemblyFinderTypeDiscoveryService) service).AssemblyFinder;
      Assert.That (assemblyFinder.RootAssemblyFinder, Is.InstanceOfType (typeof (CompositeRootAssemblyFinder)));

      var rootAssemblyFinder = (CompositeRootAssemblyFinder) assemblyFinder.RootAssemblyFinder;
      Assert.That (rootAssemblyFinder.InnerFinders.Length, Is.EqualTo (2));
      Assert.That (rootAssemblyFinder.InnerFinders[0], Is.InstanceOfType (typeof (NamedRootAssemblyFinder)));

      var namedFinder = ((NamedRootAssemblyFinder) rootAssemblyFinder.InnerFinders[0]);
      Assert.That (namedFinder.Specifications.First().AssemblyName.ToString(), Is.EqualTo ("mscorlib"));

      var filePatternFinder = ((FilePatternRootAssemblyFinder) rootAssemblyFinder.InnerFinders[1]);
      Assert.That (filePatternFinder.Specifications.ToArray(), Is.Empty);
    }

    [Test]
    public void CreateTypeDiscoveryService_CustomTypeDiscoveryService ()
    {
      var section = Deserialize (_xmlFragmentWithCustomTypeDiscoveryService);

      var service = section.CreateTypeDiscoveryService ();

      Assert.That (service, Is.InstanceOfType (typeof (FakeTypeDiscoveryService)));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "In CustomTypeDiscoveryService mode, a custom type discovery service must be specified in the type discovery configuration.",
        MatchType = MessageMatch.Contains)]
    public void CreateTypeDiscoveryService_CustomTypeDiscoveryService_MissingType ()
    {
      var section = Deserialize (_xmlFragmentWithMissingCustomTypeDiscoveryService);
      section.CreateTypeDiscoveryService ();
    }

    private TypeDiscoveryConfiguration Deserialize (string xmlFragment)
    {
      var section = new TypeDiscoveryConfiguration ();
      ConfigurationHelper.DeserializeSection (section, xmlFragment);
      return section;
    }
  }
}
