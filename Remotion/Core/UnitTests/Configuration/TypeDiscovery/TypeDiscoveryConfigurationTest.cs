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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Configuration.TypeDiscovery;
using Remotion.Development.UnitTesting.Configuration;

namespace Remotion.UnitTests.Configuration.TypeDiscovery
{
  [TestFixture]
  public class TypeDiscoveryConfigurationTest
  {
    private const string _xmlFragmentDefault = @"<typeDiscovery xmlns=""..."">
      </typeDiscovery>";
    private const string _xmlFragmentWithAutoRootAssemblyFinder = @"<typeDiscovery kind=""Automatic"" xmlns=""..."">
      </typeDiscovery>";
    private const string _xmlFragmentWithCustomRootAssemblyFinder = @"<typeDiscovery kind=""CustomRootAssemblyFinder"" xmlns=""..."">
        <customRootAssemblyFinder type=""Remotion.UnitTests::Configuration.TypeDiscovery.FakeRootAssemblyFinder""/>
      </typeDiscovery>";
    private const string _xmlFragmentWithSpecificRootAssemblies = @"<typeDiscovery kind=""SpecificRootAssemblies"" xmlns=""..."">
        <specificRootAssemblies>
          <byName>
            <include name=""mscorlib""/>
          </byName>
        </specificRootAssemblies>
      </typeDiscovery>";
    private const string _xmlFragmentWithSpecificEmptyRootAssemblies = @"<typeDiscovery kind=""SpecificRootAssemblies"" xmlns=""..."">
        <specificRootAssemblies />
      </typeDiscovery>";
    private const string _xmlFragmentWithCustomTypeDiscoveryService = @"<typeDiscovery kind=""CustomTypeDiscoveryService"" xmlns=""..."">
        <customTypeDiscoveryService type=""Remotion.UnitTests::Configuration.TypeDiscovery.FakeTypeDiscoveryService""/>
      </typeDiscovery>";

    [Test]
    public void Deserialization_Default ()
    {
      var section = Deserialize (_xmlFragmentDefault);
      Assert.That (section.Kind, Is.EqualTo (TypeDiscoveryKind.Automatic));
    }

    [Test]
    public void Deserialization_Auto ()
    {
      var section = Deserialize (_xmlFragmentWithAutoRootAssemblyFinder);
      Assert.That (section.Kind, Is.EqualTo (TypeDiscoveryKind.Automatic));
    }

    [Test]
    public void DeserializationCustomSpecificRootAssemblyFinder ()
    {
      var section = Deserialize (_xmlFragmentWithCustomRootAssemblyFinder);
      Assert.That (section.Kind, Is.EqualTo (TypeDiscoveryKind.CustomRootAssemblyFinder));
      Assert.That (section.CustomRootAssemblyFinder.Type, Is.SameAs (typeof (FakeRootAssemblyFinder)));
    }

    [Test]
    public void Deserialization_SpecificRootAssemblies ()
    {
      var section = Deserialize (_xmlFragmentWithSpecificRootAssemblies);
      Assert.That (section.Kind, Is.EqualTo (TypeDiscoveryKind.SpecificRootAssemblies));
      Assert.That (section.SpecificRootAssemblies.ByName.Single().Name, Is.EqualTo ("mscorlib"));
    }

    [Test]
    public void Deserialization_SpecificEmptyRootAssemblies ()
    {
      var section = Deserialize (_xmlFragmentWithSpecificEmptyRootAssemblies);
      Assert.That (section.Kind, Is.EqualTo (TypeDiscoveryKind.SpecificRootAssemblies));
      Assert.That (section.SpecificRootAssemblies.ByName.Count, Is.EqualTo (0));
      Assert.That (section.SpecificRootAssemblies.ByFile.Count, Is.EqualTo (0));
    }

    [Test]
    public void Deserialization_CustomTypeDiscoveryService ()
    {
      var section = Deserialize (_xmlFragmentWithCustomTypeDiscoveryService);
      Assert.That (section.Kind, Is.EqualTo (TypeDiscoveryKind.CustomTypeDiscoveryService));
      Assert.That (section.CustomTypeDiscoveryService.Type, Is.EqualTo (typeof (FakeTypeDiscoveryService)));
    }

    private TypeDiscoveryConfiguration Deserialize (string xmlFragment)
    {
      var section = new TypeDiscoveryConfiguration ();
      ConfigurationHelper.DeserializeSection (section, xmlFragment);
      return section;
    }
  }
}