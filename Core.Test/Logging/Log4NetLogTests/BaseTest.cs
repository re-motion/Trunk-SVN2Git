/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
