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
using System.Configuration;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting.IO;

namespace Remotion.Data.UnitTests.DomainObjects.Factories
{
  public class ConfigurationFactory
  {
    public static System.Configuration.Configuration LoadConfigurationFromFile (TempFile tempFile, byte[] content)
    {
      tempFile.WriteAllBytes (content);

      ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
      fileMap.ExeConfigFilename = tempFile.FileName;
      return ConfigurationManager.OpenMappedExeConfiguration (fileMap, ConfigurationUserLevel.None);
    }
  }
}