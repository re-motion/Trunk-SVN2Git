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
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net;

namespace Remotion.Data.DomainObjects.Oracle.CodeGenerator.UnitTests
{
  [TestFixture]
  public class LogUtilityTest
  {
    [SetUp]
    public void SetUp ()
    {
      if (LoggerManager.GetAllRepositories ().Length > 0)
        LoggerManager.GetAllRepositories ()[0].ResetConfiguration ();
    }

    [TearDown]
    public void TearDown ()
    {
      LogUtility.Shutdown ();
    }

    [Test]
    public void LogError ()
    {
      MemoryAppender memoryAppender = new MemoryAppender ();
      BasicConfigurator.Configure (memoryAppender);
      
      LogUtility.LogError ("Test message", new ApplicationException ("Test"));

      LoggingEvent[] events = memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      Assert.AreEqual (Level.Error, events[0].Level);
      Assert.AreEqual ("Test message", events[0].RenderedMessage);
      Assert.IsNotNull (events[0].ExceptionObject);
    }

    [Test]
    public void LogWarning ()
    {
      MemoryAppender memoryAppender = new MemoryAppender ();
      BasicConfigurator.Configure (memoryAppender);

      LogUtility.LogWarning ("Test message");

      LoggingEvent[] events = memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      Assert.AreEqual (Level.Warn, events[0].Level);
      Assert.AreEqual ("Test message", events[0].RenderedMessage);
    }

    [Test]
    public void LogToDefaultAppender ()
    {
      Assert.AreEqual (0, LoggerManager.RepositorySelector.GetAllRepositories ()[0].GetAppenders ().Length, "Appender exists.");
      
      LogUtility.LogWarning ("Test message");

      Assert.AreEqual (1, LoggerManager.RepositorySelector.GetAllRepositories ()[0].GetAppenders ().Length, "No Appender exists.");
      Assert.IsInstanceOfType (typeof (ConsoleAppender), LoggerManager.RepositorySelector.GetAllRepositories ()[0].GetAppenders()[0]);
    }
  }
}
