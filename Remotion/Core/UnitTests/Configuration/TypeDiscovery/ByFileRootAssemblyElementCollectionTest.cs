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
      ByFileRootAssemblyElementBase[] result = DeserializeFromXmlFragment (_xmlFragment);
      Assert.That (result.Length, Is.EqualTo (3));
      Assert.That (result[0].File, Is.EqualTo ("ActaNova.*.dll"));
      Assert.That (result[1].File, Is.EqualTo ("Remotion.*.dll"));
      Assert.That (result[2].File, Is.EqualTo ("Remotion.*.Utilities.dll"));
    }

    [Test]
    public void Deserialization_Types ()
    {
      ByFileRootAssemblyElementBase[] result = DeserializeFromXmlFragment (_xmlFragment);
      Assert.That (result.Length, Is.EqualTo (3));
      Assert.That (result[0], Is.InstanceOfType (typeof (ByFileIncludeRootAssemblyElement)));
      Assert.That (result[1], Is.InstanceOfType (typeof (ByFileIncludeRootAssemblyElement)));
      Assert.That (result[2], Is.InstanceOfType (typeof (ByFileExcludeRootAssemblyElement)));
    }

    [Test]
    public void IncludeReferencedAssemblies_FalseByDefault ()
    {
      var collection = DeserializeFromXmlFragment (_xmlFragment);

      ByFileRootAssemblyElementBase[] result = collection.ToArray ();
      Assert.That (((ByFileIncludeRootAssemblyElement) result[0]).IncludeReferencedAssemblies, Is.False);
    }

    [Test]
    public void IncludeReferencedAssemblies_TrueIfSpecified ()
    {
      var collection = DeserializeFromXmlFragment (_xmlFragment);

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

      DeserializeFromXmlFragment (xmlFragment);
    }

    [Test]
    public void Add ()
    {
      var element1 = new ByFileIncludeRootAssemblyElement { File = "*.dll", IncludeReferencedAssemblies = true };
      var element2 = new ByFileIncludeRootAssemblyElement { File = "*.exe" };
      var element3 = new ByFileExcludeRootAssemblyElement { File = "Utilities.exe" };
      
      var collection = new ByFileRootAssemblyElementCollection ();
      collection.Add (element1);
      collection.Add (element2);
      collection.Add (element3);

      ByFileRootAssemblyElementBase[] result = collection.ToArray ();
      Assert.That (result, Is.EqualTo (new ByFileRootAssemblyElementBase[] { element1, element2, element3 }));
    }

    [Test]
    public void RemoveAt ()
    {
      var element1 = new ByFileIncludeRootAssemblyElement { File = "*.dll", IncludeReferencedAssemblies = true };
      var element2 = new ByFileIncludeRootAssemblyElement { File = "*.exe" };
      var element3 = new ByFileExcludeRootAssemblyElement { File = "Utilities.exe" };

      var collection = new ByFileRootAssemblyElementCollection ();
      collection.Add (element1);
      collection.Add (element2);
      collection.Add (element3);
      collection.RemoveAt (1);

      ByFileRootAssemblyElementBase[] result = collection.ToArray ();
      Assert.That (result, Is.EquivalentTo (new ByFileRootAssemblyElementBase[] { element1, element3 }));
    }

    [Test]
    public void Clear ()
    {
      var element1 = new ByFileIncludeRootAssemblyElement { File = "*.dll", IncludeReferencedAssemblies = true };
      var element2 = new ByFileIncludeRootAssemblyElement { File = "*.exe" };
      var element3 = new ByFileExcludeRootAssemblyElement { File = "Utilities.exe" };

      var collection = new ByFileRootAssemblyElementCollection ();
      collection.Add (element1);
      collection.Add (element2);
      collection.Add (element3);
      collection.Clear ();

      ByFileRootAssemblyElementBase[] result = collection.ToArray ();
      Assert.That (result, Is.Empty);
    }

    [Test]
    [Ignore ("TODO 1643")]
    public void CreateRootAssemblyFinder ()
    {
      Assert.Fail ();

      //var collection = DeserializeFromXmlFragment (_xmlFragment);
      //var finder = (FilePatternRootAssemblyFinder) collection.CreateRootAssemblyFinder ();

      //var specs = finder.Specifications.ToArray();

      //Assert.That (specs[0].FilePattern, Is.EqualTo ("ActaNova.*.dll"));
      //Assert.That (specs[0].Kind, Is.EqualTo (FilePatternRootAssemblyFinder.Specification.SpecificationKind.IncludeNoFollow));

      //Assert.That (specs[1].FilePattern, Is.EqualTo ("Remotion.*.dll"));
      //Assert.That (specs[1].FollowReferences, Is.EqualTo (FilePatternRootAssemblyFinder.Specification.SpecificationKind.IncludeFollowReferences));

      //Assert.That (specs[2].FilePattern, Is.EqualTo ("Remotion.*.dll"));
      //Assert.That (specs[2].FollowReferences, Is.EqualTo (FilePatternRootAssemblyFinder.Specification.SpecificationKind.Exclude));
    }


    private ByFileRootAssemblyElementBase[] DeserializeFromXmlFragment (string xmlFragment)
    {
      var collection = new ByFileRootAssemblyElementCollection ();
      ConfigurationHelper.DeserializeElement (collection, xmlFragment);

      return collection.ToArray ();
    }
  }
}