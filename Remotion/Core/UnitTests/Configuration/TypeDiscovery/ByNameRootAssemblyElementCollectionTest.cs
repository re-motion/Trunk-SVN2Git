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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Configuration.TypeDiscovery;
using Remotion.Development.UnitTesting.Configuration;
using System.Linq;

namespace Remotion.UnitTests.Configuration.TypeDiscovery
{
  [TestFixture]
  public class ByNameRootAssemblyElementCollectionTest
  {
    private const string _xmlFragment = @"<byName>
              <include name=""mscorlib"" />
              <include name=""System.Xml, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" includeReferencedAssemblies=""true"" />
            </byName>";

    [Test]
    public void Deserialization ()
    {
      ByNameRootAssemblyElement[] result = DeserializeFromXmlFragment();
      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (result[0].Name, Is.EqualTo ("mscorlib"));
      Assert.That (result[1].Name, Is.EqualTo ("System.Xml, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"));
    }

    [Test]
    public void IncludeReferencedAssemblies_FalseByDefault ()
    {
      var collection = new ByNameRootAssemblyElementCollection ();
      ConfigurationHelper.DeserializeElement (collection, _xmlFragment);

      ByNameRootAssemblyElement[] result = collection.ToArray ();
      Assert.That (result[0].IncludeReferencedAssemblies, Is.False);
    }

    [Test]
    public void IncludeReferencedAssemblies_TrueIfSpecified ()
    {
      var collection = new ByNameRootAssemblyElementCollection ();
      ConfigurationHelper.DeserializeElement (collection, _xmlFragment);

      ByNameRootAssemblyElement[] result = collection.ToArray ();
      Assert.That (result[1].IncludeReferencedAssemblies, Is.True);
    }

    private ByNameRootAssemblyElement[] DeserializeFromXmlFragment ()
    {
      var collection = new ByNameRootAssemblyElementCollection ();
      ConfigurationHelper.DeserializeElement (collection, _xmlFragment);

      return collection.ToArray ();
    }
  }
}