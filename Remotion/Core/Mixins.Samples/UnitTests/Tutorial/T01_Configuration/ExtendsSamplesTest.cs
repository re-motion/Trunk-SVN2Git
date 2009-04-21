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
using Remotion.Mixins.Samples.Tutorial.T01_Configuration.ExtendsSamples;
using Remotion.Reflection;

namespace Remotion.Mixins.Samples.UnitTests.Tutorial.T01_Configuration
{
  [TestFixture]
  public class ExtendsSamplesTest
  {
    [Test]
    public void File_IsNumbered ()
    {
      var numberedFile = (INumberedFile) ObjectFactory.Create<File> (ParamList.Empty);
      var numberedCarFile = (INumberedFile) ObjectFactory.Create<CarFile> (ParamList.Empty);
      var numberedPersonFile = (INumberedFile) ObjectFactory.Create<PersonFile> (ParamList.Empty);

      Assert.That (numberedFile.GetFileNumber(), NUnit.Framework.SyntaxHelpers.Text.Matches ("........-....-....-....-............"));
      Assert.That (numberedCarFile.GetFileNumber(), NUnit.Framework.SyntaxHelpers.Text.Matches ("........-....-....-....-............"));
      Assert.That (numberedPersonFile.GetFileNumber(), NUnit.Framework.SyntaxHelpers.Text.Matches ("........-....-....-....-............"));
    }
  }
}