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
using System.Reflection;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using NUnit.Framework;
using Remotion.Logging;

namespace Remotion.UnitTests.Logging.Log4NetLogTests
{
  public class BaseTest
  {
    private ILogger _logger;
    private ILog _log;
    private MemoryAppender _memoryAppender;

    [SetUp]
    public virtual void SetUp ()
    {
      _memoryAppender = new MemoryAppender ();
      BasicConfigurator.Configure (_memoryAppender);

      _logger = LoggerManager.GetLogger (Assembly.GetCallingAssembly (), "The Name");
      _log = new Log4NetLog (_logger);
    }

    [TearDown]
    public virtual void TearDown ()
    {
      LoggerManager.Shutdown ();
    }

    protected ILog Log
    {
      get { return _log; }
    }

    protected ILogger Logger
    {
      get { return _logger; }
    }

    protected LoggingEvent[] GetLoggingEvents ()
    {
      return _memoryAppender.GetEvents ();
    }

    protected void SetLoggingThreshold (Level threshold)
    {
      _logger.Repository.Threshold = threshold;
    }
  }
}
