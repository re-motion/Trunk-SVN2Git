// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using System.ComponentModel.Design;
using NUnit.Framework;
using Remotion.Design;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Design
{
  [TestFixture]
  public class DesignerUtilityTest
  {
    [TearDown]
    public void TearDown ()
    {
      DesignerUtility.ClearDesignMode ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void GetDesignModeType_NoDesignMode ()
    {
      DesignerUtility.GetDesignModeType ("Foo");
    }

    [Test]
    public void GetDesignModeType_UsesHost ()
    {
      MockRepository mockRepository = new MockRepository ();
      IDesignModeHelper helperStub = mockRepository.Stub<IDesignModeHelper>();
      IDesignerHost hostMock = mockRepository.StrictMock<IDesignerHost> ();
      SetupResult.For (helperStub.DesignerHost).Return (hostMock);
      DesignerUtility.SetDesignMode (helperStub);

      Expect.Call (hostMock.GetType ("Foo")).Return (typeof (int));
      mockRepository.ReplayAll ();

      Assert.That (DesignerUtility.GetDesignModeType ("Foo"), Is.SameAs (typeof (int)));
      mockRepository.VerifyAll ();
    }
  }
}
