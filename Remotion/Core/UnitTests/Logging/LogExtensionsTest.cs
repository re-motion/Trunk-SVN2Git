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
using Remotion.Logging;
using Rhino.Mocks;

namespace Remotion.UnitTests.Logging
{
  [TestFixture]
  public class LogExtensionsTest
  {
    [Test]
    public void LogAndReturn_ReturnsValue ()
    {
      var logMock = MockRepository.GenerateMock<ILog> ();

      var result = "test".LogAndReturn (logMock, LogLevel.Debug, value => string.Format ("x{0}y", value));
      Assert.That (result, Is.EqualTo ("test"));
    }

    [Test]
    public void LogAndReturn_DoesNotLog_IfNotConfigured ()
    {
      var logMock = MockRepository.GenerateMock<ILog> ();

      "test".LogAndReturn (logMock, LogLevel.Debug, value => string.Format ("x{0}y", value));
      logMock.AssertWasNotCalled (mock => mock.Log (Arg<LogLevel>.Is.Anything, Arg<int?>.Is.Anything, Arg<object>.Is.Anything, Arg<Exception>.Is.Anything));
    }

    [Test]
    public void LogAndReturn_Logs_IfConfigured ()
    {
      var logMock = MockRepository.GenerateMock<ILog> ();
      logMock.Expect (mock => mock.IsEnabled (LogLevel.Debug)).Return (true);
      logMock.Replay ();

      "test".LogAndReturn (logMock, LogLevel.Debug, value => string.Format ("x{0}y", value));
      logMock.AssertWasCalled (mock => mock.Log (LogLevel.Debug, (int?) null, "xtesty", (Exception) null));
    }
  }
}
