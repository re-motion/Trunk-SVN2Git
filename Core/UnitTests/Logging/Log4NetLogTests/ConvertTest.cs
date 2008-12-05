// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using log4net.Core;
using NUnit.Framework;
using Remotion.Logging;

namespace Remotion.UnitTests.Logging.Log4NetLogTests
{
  [TestFixture]
  public class ConvertTest
  {
    [Test]
    public void Test_Info ()
    {
      Assert.AreEqual (Level.Info, Log4NetLog.Convert (LogLevel.Info));
    }

    [Test]
    public void Test_Debug ()
    {
      Assert.AreEqual (Level.Debug, Log4NetLog.Convert (LogLevel.Debug));
    }

    [Test]
    public void Test_Warn ()
    {
      Assert.AreEqual (Level.Warn, Log4NetLog.Convert (LogLevel.Warn));
    }

    [Test]
    public void Test_Error ()
    {
      Assert.AreEqual (Level.Error, Log4NetLog.Convert (LogLevel.Error));
    }

    [Test]
    public void Test_Fatal ()
    {
      Assert.AreEqual (Level.Fatal, Log4NetLog.Convert (LogLevel.Fatal));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "LogLevel does not support value 10.\r\nParameter name: logLevel")]
    public void Test_InvalidLevel ()
    {
      Log4NetLog.Convert ((LogLevel) 10);
    }
  }
}
