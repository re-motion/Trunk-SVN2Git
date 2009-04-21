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

      Assert.That (Path.IsPathRooted (configPath), configPath);
      Assert.That (File.Exists (configPath), configPath);
      
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

      Assert.That (Path.IsPathRooted (configPath), configPath);
      Assert.That (File.Exists (configPath), Is.False, configPath);

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

      Assert.That (Path.IsPathRooted (configPath), configPath);
      Assert.That (Path.IsPathRooted (parameter.ConfigFile), Is.False, parameter.ConfigFile);
      Assert.That (File.Exists (configPath), configPath);
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

      Assert.That (Path.IsPathRooted (configPath), configPath);
      Assert.That (Path.IsPathRooted (parameter.ConfigFile), Is.False, parameter.ConfigFile);
      Assert.That (File.Exists (Path.Combine (Environment.CurrentDirectory, parameter.ConfigFile)), Is.False);

      RdbmsToolsRunner.CreateAppDomainSetup (parameter);
    }
  }
}
