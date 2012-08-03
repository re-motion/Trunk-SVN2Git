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
using NUnit.Framework;
using Remotion.Configuration.TypeDiscovery;
using Remotion.Development.UnitTesting.Configuration;

namespace Remotion.UnitTests.Configuration.TypeDiscovery.IntegrationTests.SpecificRootAssemblies
{
  [TestFixture]
  public class SpecificRootAssembliesAreNotSubjectToApplicationFilterTest
  {
    private const string _xmlFragmentWithMscorlib = @"<typeDiscovery mode=""SpecificRootAssemblies"" xmlns=""..."">
        <specificRootAssemblies>
          <byName>
            <include name=""mscorlib""/>
          </byName>
        </specificRootAssemblies>
      </typeDiscovery>";

    [Test]
    [Ignore ("TODO 2572")]
    public void Deserialization_SpecificRootAssemblies ()
    {
      var section = Deserialize (_xmlFragmentWithMscorlib);

      var service = section.CreateTypeDiscoveryService();
      
      var types = service.GetTypes (null, false);
      Assert.That (types, Has.Member (typeof (object)));
    }

    private TypeDiscoveryConfiguration Deserialize (string xmlFragment)
    {
      var section = new TypeDiscoveryConfiguration ();
      ConfigurationHelper.DeserializeSection (section, xmlFragment);
      return section;
    }
  }
}
