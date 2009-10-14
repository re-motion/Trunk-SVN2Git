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
using System.Configuration;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Configuration.TypeDiscovery;
using Remotion.Development.UnitTesting.Configuration;
using System.Linq;

namespace Remotion.UnitTests.Configuration.TypeDiscovery
{
  [TestFixture]
  public class ByFileRootAssemblyElementCollectionTest
  {
    private const string _xmlFragment = @"<byFile>
              <include file=""ActaNova.*.dll"" />
              <include file=""Remotion.*.dll"" includeReferencedAssemblies=""true"" />
              <exclude file=""Remotion.*.Utilities.dll"" />
            </byFile>";

    [Test]
    public void Deserialization ()
    {
      ByFileRootAssemblyElementBase[] result = DeserializeFromXmlFragment();
      Assert.That (result.Length, Is.EqualTo (3));
      Assert.That (result[0].File, Is.EqualTo ("ActaNova.*.dll"));
      Assert.That (result[1].File, Is.EqualTo ("Remotion.*.dll"));
      Assert.That (result[2].File, Is.EqualTo ("Remotion.*.Utilities.dll"));
    }

    [Test]
    public void Deserialization_Types ()
    {
      ByFileRootAssemblyElementBase[] result = DeserializeFromXmlFragment ();
      Assert.That (result.Length, Is.EqualTo (3));
      Assert.That (result[0], Is.InstanceOfType (typeof (ByFileIncludeRootAssemblyElement)));
      Assert.That (result[1], Is.InstanceOfType (typeof (ByFileIncludeRootAssemblyElement)));
      Assert.That (result[2], Is.InstanceOfType (typeof (ByFileExcludeRootAssemblyElement)));
    }

    [Test]
    public void IncludeReferencedAssemblies_FalseByDefault ()
    {
      var collection = new ByFileRootAssemblyElementCollection ();
      ConfigurationHelper.DeserializeElement (collection, _xmlFragment);

      ByFileRootAssemblyElementBase[] result = collection.ToArray ();
      Assert.That (((ByFileIncludeRootAssemblyElement) result[0]).IncludeReferencedAssemblies, Is.False);
    }

    [Test]
    public void IncludeReferencedAssemblies_TrueIfSpecified ()
    {
      var collection = new ByFileRootAssemblyElementCollection ();
      ConfigurationHelper.DeserializeElement (collection, _xmlFragment);

      ByFileRootAssemblyElementBase[] result = collection.ToArray ();
      Assert.That (((ByFileIncludeRootAssemblyElement) result[1]).IncludeReferencedAssemblies, Is.True);
    }

    [Test]
    [ExpectedException (
        typeof (ConfigurationErrorsException), 
        ExpectedMessage = "Unrecognized attribute 'includeReferencedAssemblies'.", 
        MatchType = MessageMatch.Contains)]
    public void IncludeReferencedAssemblies_NotValidWithExclude ()
    {
      const string xmlFragment = @"<byFile>
              <exclude file=""Remotion.*.Utilities.dll"" includeReferencedAssemblies=""true""/>
            </byFile>";

      var collection = new ByFileRootAssemblyElementCollection ();
      ConfigurationHelper.DeserializeElement (collection, xmlFragment);
    }

    private ByFileRootAssemblyElementBase[] DeserializeFromXmlFragment ()
    {
      var collection = new ByFileRootAssemblyElementCollection ();
      ConfigurationHelper.DeserializeElement (collection, _xmlFragment);

      return collection.ToArray ();
    }
  }
}