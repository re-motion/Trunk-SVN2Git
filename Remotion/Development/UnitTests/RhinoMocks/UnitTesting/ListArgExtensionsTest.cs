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
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Development.RhinoMocks.UnitTesting;

namespace Remotion.Development.UnitTests.RhinoMocks.UnitTesting
{
  [TestFixture]
  public class ListArgExtensionsTest
  {
    [Test]
    public void Equivalent ()
    {
      var myMock = MockRepository.GenerateMock<IMyInterface>();

      var equal = false;
      var equivalent = false;
      var different = false;

      myMock.Expect (mock => mock.SomeMethod (Arg<IEnumerable<string>>.List.Equivalent ("a", "b", "c")))
            .WhenCalled (mi => equal = true);
      myMock.Expect (mock => mock.SomeMethod (Arg<IEnumerable<string>>.List.Equivalent ("d", "e", "f")))
            .WhenCalled (mi => equivalent = true);
      myMock.Expect (mock => mock.SomeMethod (Arg<IEnumerable<string>>.List.Equivalent ("g", "h", "i")))
            .WhenCalled (mi => different = true);

      myMock.SomeMethod (new[] { "a", "b", "c" });
      myMock.SomeMethod (new List<string> { "f", "e", "d" });
      myMock.SomeMethod (new[] { "g", "h", "j" });

      Assert.That (equal, Is.True);
      Assert.That (equivalent, Is.True);
      Assert.That (different, Is.False);
      Assert.That (() => myMock.VerifyAllExpectations(), Throws.Exception);
    }
  }

  public interface IMyInterface
  {
    void SomeMethod (IEnumerable<string> parameters);
  }
}