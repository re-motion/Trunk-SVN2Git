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
  public class RootAssembliesElementTest
  {
    private const string _emptyXmlFragment = @"<rootAssemblies />";
    private const string _xmlFragmentWithFilesAndNames = @"<rootAssemblies>
            <byName>
              <include name=""mscorlib"" />
            </byName>
            <byFile>
              <include file=""ActaNova.*.dll"" />
            </byFile>
          </rootAssemblies>";

    [Test]
    public void Deserialization_WithEmptyFragment ()
    {
      var element = DeserializeFromXmlFragment (_emptyXmlFragment);
      Assert.That (element.ByName.Count, Is.EqualTo (0));
      Assert.That (element.ByFile.Count, Is.EqualTo (0));
    }

    [Test]
    public void Deserialization_WithNonEmptyFragment ()
    {
      RootAssembliesElement element = DeserializeFromXmlFragment(_xmlFragmentWithFilesAndNames);
      Assert.That (element.ByName.Count, Is.EqualTo (1));
      Assert.That (element.ByName.First().Name, Is.EqualTo ("mscorlib"));
      Assert.That (element.ByFile.Count, Is.EqualTo (1));
      Assert.That (element.ByFile.First().File, Is.EqualTo ("ActaNova.*.dll"));
    }

    private RootAssembliesElement DeserializeFromXmlFragment (string xmlFragment)
    {
      var element = new RootAssembliesElement ();
      ConfigurationHelper.DeserializeElement (element, xmlFragment);
      return element;
    }
  }
}