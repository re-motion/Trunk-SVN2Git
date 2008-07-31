/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Web.Configuration;

namespace Remotion.Development.Web.UnitTesting.Configuration
{
  public static class WebConfigurationFactory
  {
    public static WebConfiguration GetExecutionEngineWithDefaultWxeHandler ()
    {
      WebConfigurationMock config = new WebConfigurationMock ();
      config.ExecutionEngine.DefaultWxeHandler = "WxeHandler.ashx";
      return config;
    }

    public static WebConfiguration GetExecutionEngineUrlMapping ()
    {
      WebConfigurationMock config = new WebConfigurationMock ();
      config.ExecutionEngine.UrlMappingFile = @"Res\UrlMapping.xml";
      return config;
    }

    public static WebConfiguration GetExecutionEngineMappingWithNoFilename ()
    {
      WebConfigurationMock config = new WebConfigurationMock ();
      config.ExecutionEngine.UrlMappingFile = null;
      return config;
    }

    public static WebConfiguration GetDebugExceptionLevelA ()
    {
      WebConfigurationMock config = new WebConfigurationMock ();
      config.Wcag.Debugging = WcagDebugMode.Exception;
      config.Wcag.ConformanceLevel = WaiConformanceLevel.A;
      return config;
    }

    public static WebConfiguration GetDebugLoggingLevelA ()
    {
      WebConfigurationMock config = new WebConfigurationMock ();
      config.Wcag.Debugging = WcagDebugMode.Logging;
      config.Wcag.ConformanceLevel = WaiConformanceLevel.A;
      return config;
    }

    public static WebConfiguration GetDebugExceptionLevelDoubleA ()
    {
      WebConfigurationMock config = new WebConfigurationMock ();
      config.Wcag.Debugging = WcagDebugMode.Exception;
      config.Wcag.ConformanceLevel = WaiConformanceLevel.DoubleA;
      return config;
    }

    public static WebConfiguration GetDebugExceptionLevelUndefined ()
    {
      WebConfigurationMock config = new WebConfigurationMock ();
      config.Wcag.Debugging = WcagDebugMode.Exception;
      config.Wcag.ConformanceLevel = WaiConformanceLevel.Undefined;
      return config;
    }

    public static WebConfiguration GetLevelA ()
    {
      WebConfigurationMock config = new WebConfigurationMock ();
      config.Wcag.Debugging = WcagDebugMode.Disabled;
      config.Wcag.ConformanceLevel = WaiConformanceLevel.A;
      return config;
    }

    public static WebConfiguration GetLevelDoubleA ()
    {
      WebConfigurationMock config = new WebConfigurationMock ();
      config.Wcag.Debugging = WcagDebugMode.Disabled;
      config.Wcag.ConformanceLevel = WaiConformanceLevel.DoubleA;
      return config;
    }

    public static WebConfiguration GetLevelUndefined ()
    {
      WebConfigurationMock config = new WebConfigurationMock ();
      config.Wcag.Debugging = WcagDebugMode.Disabled;
      config.Wcag.ConformanceLevel = WaiConformanceLevel.Undefined;
      return config;
    }
  }
}