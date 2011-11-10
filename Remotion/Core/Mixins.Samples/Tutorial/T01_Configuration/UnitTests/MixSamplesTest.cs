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
using NUnit.Framework;
using Remotion.Mixins.Samples.Tutorial.T01_Configuration.Core.Mix.MixSamples;
using Remotion.Reflection;

namespace Remotion.Mixins.Samples.Tutorial.T01_Configuration.UnitTests
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

      Assert.That (numberedFile.GetObjectID(), Is.StringMatching ("........-....-....-....-............"));
      Assert.That (numberedCarFile.GetObjectID(), Is.StringMatching ("........-....-....-....-............"));
      Assert.That (numberedPersonFile.GetObjectID(), Is.StringMatching("........-....-....-....-............"));
    }
  }
}
