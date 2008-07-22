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
using System.IO;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests
{
  using Text = NUnit.Framework.SyntaxHelpers.Text;

  [TestFixture]
  public class RdbmsToolsRunnerTest
  {
    [Test]
    public void CreateAppDomainSetup ()
    {
      RdbmsToolsParameter parameter = new RdbmsToolsParameter ();
      parameter.BaseDirectory = @"c:\foobar";
      AppDomainSetup setup = RdbmsToolsRunner.CreateAppDomainSetup (parameter);
      Assert.That (setup.ApplicationName, Is.EqualTo ("RdbmsTools"));
      Assert.That (setup.ApplicationBase, Is.EqualTo (@"c:\foobar"));
      Assert.That (setup.DynamicBase, Text.StartsWith (Path.Combine (Path.GetTempPath (), "Remotion")));
      Assert.That (setup.ConfigurationFile, Is.Null);
    }

    [Test]
    public void CreateAppDomainSetup_WithAbsoluteConfigFilePath ()
    {
      RdbmsToolsParameter parameter = new RdbmsToolsParameter ();
      parameter.BaseDirectory = @"c:\foobar";
      string codeBaseUri = Assembly.GetExecutingAssembly().EscapedCodeBase;
      string configPath = Path.Combine (Path.GetDirectoryName (new Uri (codeBaseUri).AbsolutePath), "Test.config");
      parameter.ConfigFile = configPath;

      Assert.That (Path.IsPathRooted (configPath));
      Assert.That (File.Exists (configPath));
      
      AppDomainSetup setup = RdbmsToolsRunner.CreateAppDomainSetup (parameter);
      Assert.That (setup.ConfigurationFile, Is.EqualTo (configPath));
    }

    [Test]
    [ExpectedException (typeof (FileNotFoundException), 
        ExpectedMessage = "The configuration file supplied by the 'config' parameter was not found.", 
        MatchType = MessageMatch.Contains)]
    public void CreateAppDomainSetup_WithAbsoluteConfigFilePath_FileNotFound ()
    {
      RdbmsToolsParameter parameter = new RdbmsToolsParameter ();
      parameter.BaseDirectory = @"c:\foobar";
      string codeBaseUri = Assembly.GetExecutingAssembly ().EscapedCodeBase;
      string configPath = Path.Combine (Path.GetDirectoryName (new Uri (codeBaseUri).AbsolutePath), "Test12313.config");
      parameter.ConfigFile = configPath;

      Assert.That (Path.IsPathRooted (configPath));
      Assert.That (File.Exists (configPath), Is.False);

      RdbmsToolsRunner.CreateAppDomainSetup (parameter);
    }

    [Test]
    public void CreateAppDomainSetup_WithPathRelativeToCurrentDirectory ()
    {
      RdbmsToolsParameter parameter = new RdbmsToolsParameter ();
      parameter.BaseDirectory = @"c:\foobar";
      string codeBaseUri = Assembly.GetExecutingAssembly ().EscapedCodeBase;
      string configPath = Path.Combine (Path.GetDirectoryName (new Uri (codeBaseUri).AbsolutePath), "Test.config");

      Environment.CurrentDirectory = Path.GetDirectoryName (configPath);
      parameter.ConfigFile = Path.GetFileName (configPath);

      Assert.That (Path.IsPathRooted (configPath));
      Assert.That (Path.IsPathRooted (parameter.ConfigFile), Is.False);
      Assert.That (File.Exists (configPath));
      Assert.That (File.Exists (Path.Combine (Environment.CurrentDirectory, parameter.ConfigFile)));

      AppDomainSetup setup = RdbmsToolsRunner.CreateAppDomainSetup (parameter);
      Assert.That (setup.ConfigurationFile, Is.EqualTo (configPath));
    }

    [Test]
    [ExpectedException (typeof (FileNotFoundException), 
        ExpectedMessage = "The configuration file supplied by the 'config' parameter was not found.", 
        MatchType = MessageMatch.Contains)]
    public void CreateAppDomainSetup_WithPathRelativeToCurrentDirectory_FileNotFound ()
    {
      RdbmsToolsParameter parameter = new RdbmsToolsParameter ();
      parameter.BaseDirectory = @"c:\foobar";
      string codeBaseUri = Assembly.GetExecutingAssembly ().EscapedCodeBase;
      string configPath = Path.Combine (Path.GetDirectoryName (new Uri (codeBaseUri).AbsolutePath), "Test123.config");

      Environment.CurrentDirectory = Path.GetDirectoryName (configPath);
      parameter.ConfigFile = Path.GetFileName (configPath);

      Assert.That (Path.IsPathRooted (configPath));
      Assert.That (Path.IsPathRooted (parameter.ConfigFile), Is.False);
      Assert.That (File.Exists (Path.Combine (Environment.CurrentDirectory, parameter.ConfigFile)), Is.False);

      RdbmsToolsRunner.CreateAppDomainSetup (parameter);
    }
  }
}