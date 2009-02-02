// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// This framework is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this framework; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.Samples.Tutorial.T01_Configuration.MixSamples;
using Remotion.Reflection;

namespace Remotion.Mixins.Samples.UnitTests.Tutorial.T01_Configuration
{
  [TestFixture]
  public class MixSamplesTest
  {
    [Test]
    public void File_HasID ()
    {
      var numberedFile = (IIdentifiedObject) ObjectFactory.Create<File> (ParamList.Empty);
      var numberedCarFile = (IIdentifiedObject) ObjectFactory.Create<CarFile> (ParamList.Empty);
      var numberedPersonFile = (IIdentifiedObject) ObjectFactory.Create<PersonFile> (ParamList.Empty);

      Assert.That (numberedFile.GetObjectID(), NUnit.Framework.SyntaxHelpers.Text.Matches ("........-....-....-....-............"));
      Assert.That (numberedCarFile.GetObjectID(), NUnit.Framework.SyntaxHelpers.Text.Matches ("........-....-....-....-............"));
      Assert.That (numberedPersonFile.GetObjectID(), NUnit.Framework.SyntaxHelpers.Text.Matches ("........-....-....-....-............"));
    }
  }
}