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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory;
using Remotion.SecurityManager.Domain.AccessControl;
using Rhino.Mocks;
using User=Remotion.SecurityManager.Domain.OrganizationalStructure.User;
using System.Collections.Generic;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpanderApplicationTest : AclToolsTestBase
  {
    private const string c_cssFileContent = 
    #region 
        @"table 
{
  background-color: white;
  border-color: black;
  border-style: solid;
  border-width: 1px;
  table-layout: auto;
  border-collapse: collapse;
  border-spacing: 10px;
  empty-cells: show;
  caption-side: top;
  font-family: Arial, Helvetica, sans-serif;
  vertical-align: text-top;
  text-align: left;
}

th, td
{
  border-style: solid;
  border-color: black;
  border-width: 1px;
  table-layout: auto;
  border-collapse: collapse;
  border-spacing: 1px;
  padding: 5px;
} 

th
{
   background-color: #CCCCCC;
}    ";
    #endregion


    [SetUp]
    public override void SetUp () // TODO AE: Remove.
    {
      base.SetUp();
    }

    // TODO AE: Make private and move to bottom.
    public List<AclExpansionEntry> CreateAclExpanderApplicationAndCallGetAclExpansion (AclExpanderApplicationSettings settings)
    {
      var application = new AclExpanderApplication();
      application.Init (settings, TextWriter.Null, TextWriter.Null);
      //PrivateInvoke.InvokeNonPublicMethod (application, "Init", settings, TextWriter.Null,TextWriter.Null);

      // TODO AE: Consider making GetAclExpansion public. Wouldn't break encapsulation (IMO, it's only a get method anyway) and would also enable you 
      // to mock the user finder and acl finder.
      return (List<AclExpansionEntry>) PrivateInvoke.InvokeNonPublicMethod (application, "GetAclExpansion");
      
      //foreach (AclExpansionEntry entry in aclExpansion)
      //{
      //  To.ConsoleLine.e (entry);
      //}
    }

    // TODO AE: Note: These tests test against the database, so they are actually integration tests.
    // TODO AE: Consider mocking the user finder, ACL finder, and/or ACL expander to get non-integrative unit tests. (And move the integration tests
    // TODO AE: to an AclExpanderApplicationIntegrationTest class.)

    [Test]
    public void FindAllUsersTest ()
    {
      var settings = new AclExpanderApplicationSettings();
      var aclExpansion = CreateAclExpanderApplicationAndCallGetAclExpansion (settings);
      //To.ConsoleLine.e (() => aclExpansion);
      Assert.That (aclExpansion.Count, Is.EqualTo (16));
    }


    [Test]
    public void FirstNameFilterTest ()
    {
      const string firstName = "test";
      var settings = new AclExpanderApplicationSettings ();
      settings.UserFirstName = firstName;
      var aclExpansion = CreateAclExpanderApplicationAndCallGetAclExpansion (settings);
      Assert.That (aclExpansion.Count, Is.EqualTo (8));
      foreach (AclExpansionEntry entry in aclExpansion)
      {
        Assert.That (entry.User.FirstName, Is.EqualTo (firstName));
      }
    }


    [Test]
    public void LastNameFilterTest ()
    {
      const string lastName = "user1";
      var settings = new AclExpanderApplicationSettings ();
      settings.UserLastName = lastName;
      var aclExpansion = CreateAclExpanderApplicationAndCallGetAclExpansion (settings);
      Assert.That (aclExpansion.Count, Is.EqualTo (4));
      foreach (AclExpansionEntry entry in aclExpansion)
      {
        Assert.That (entry.User.LastName, Is.EqualTo (lastName));
      }
    }


    [Test]
    public void UserNameFilterTest ()
    {
      const string userName = "test.user";
      var settings = new AclExpanderApplicationSettings ();
      settings.UserName = userName;
      var aclExpansion = CreateAclExpanderApplicationAndCallGetAclExpansion (settings);
      Assert.That (aclExpansion.Count, Is.EqualTo (8));
      foreach (AclExpansionEntry entry in aclExpansion)
      {
        Assert.That (entry.User.UserName, Is.EqualTo (userName));
      }
    }



    [Test]
    public void AllNamesFilterTest ()
    {
      const string firstName = "test";
      const string lastName = "user";
      const string userName = "test.user";
      var settings = new AclExpanderApplicationSettings ();
      settings.UserFirstName = firstName;
      settings.UserLastName = lastName;
      settings.UserName = userName;
      var aclExpansion = CreateAclExpanderApplicationAndCallGetAclExpansion (settings);
      Assert.That (aclExpansion.Count, Is.EqualTo (8));
      foreach (AclExpansionEntry entry in aclExpansion)
      {
        //To.ConsoleLine.e (() => entry);
        Assert.That (entry.User.FirstName, Is.EqualTo (firstName));
        Assert.That (entry.User.LastName, Is.EqualTo (lastName));
        Assert.That (entry.User.UserName, Is.EqualTo (userName));
      }
    }


    [Test]
    public void AllNamesFilterTest2 ()
    {
      const string firstName = "test";
      const string lastName = "user";
      const string userName = "group1/user2";
      var settings = new AclExpanderApplicationSettings ();
      settings.UserFirstName = firstName;
      settings.UserLastName = lastName;
      settings.UserName = userName;
      var aclExpansion = CreateAclExpanderApplicationAndCallGetAclExpansion (settings);
      Assert.That (aclExpansion.Count, Is.EqualTo (0));
    }



    [Test]
    public void RunSingleFileOutputDirectoryAndExtensionSettingTest ()
    {
      const string directory = "The Directory";

      var mocks = new MockRepository();
      var textWriterFactoryMock = mocks.DynamicMock<ITextWriterFactory> ();

      textWriterFactoryMock.Expect (mock => mock.Directory = directory); 
      textWriterFactoryMock.Expect (mock => mock.CreateTextWriter (Arg<String>.Is.Anything)).Return (new StringWriter ());
      textWriterFactoryMock.Expect (mock => mock.CreateTextWriter (Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).Return (new StringWriter ());
      
      textWriterFactoryMock.Replay();

      var settings = new AclExpanderApplicationSettings ();
      settings.UseMultipleFileOutput = false;
      settings.Directory = directory;
      var application = new AclExpanderApplication (textWriterFactoryMock);

      application.Run (settings, TextWriter.Null, TextWriter.Null);

      textWriterFactoryMock.VerifyAllExpectations ();
    }


    [Test]
    public void InitTest ()
    {
      var textWriterFactorStub = MockRepository.GenerateStub<ITextWriterFactory> ();
      var application = new AclExpanderApplication (textWriterFactorStub);
      var settings = new AclExpanderApplicationSettings ();
      var logWriter = new StringWriter();
      var errorWriter = new StringWriter();
      application.Init (settings, errorWriter, logWriter);
      Assert.That (application.Settings, Is.EqualTo (settings));
      const string errorText = "837r498 2735";
      application.ErrorToTextBuilder.s (errorText);
      Assert.That (errorWriter.ToString (), Is.EqualTo (errorText));

      const string logText = "KEUZHFI 47zw89";
      application.LogToTextBuilder.s (logText);
      Assert.That (logWriter.ToString (), Is.EqualTo (logText));
    }

    [Test]
    public void GetCultureNameTest ()
    {
      AssertGetCultureName(null,null);
      AssertGetCultureName ("", null);
      AssertGetCultureName ("de-AT", "de-AT");
      AssertGetCultureName ("en-US", "en-US");
    }



    [Test]
    public void MultipleFileOutputWritingTest ()
    {
      var streamWriterFactory = new StreamWriterFactory ();

      var settings = new AclExpanderApplicationSettings ();
      settings.UseMultipleFileOutput = true;
      //string path = Path.GetTempPath ();
      string path = "c:\\temp";
      string testDirectory = Path.Combine(path, "TestDirectory");
      settings.Directory = testDirectory;
      var application = new AclExpanderApplication (streamWriterFactory);
      application.Run (settings, TextWriter.Null, TextWriter.Null);

      string outputDirectory = streamWriterFactory.Directory;
      AssertFileExists (outputDirectory, "group0_user1.html");
      AssertFileExists (outputDirectory, "group0_user2.html");
      AssertFileExists (outputDirectory, "group1_user1.html");
      AssertFileExists (outputDirectory, "group1_user2.html");

      AssertFileExists (outputDirectory, "_AclExpansionMain_.html");
      AssertFileExists (outputDirectory, "AclExpansion.css");

      AssertFileExists (outputDirectory, "test.user.html"); 
    }



    // TODO: Adapt test to use StreamWriterFactory and turn into integration test
    // TODO AE: But still keep a respective unit test.
    [Test]
    public void MultipleFileOutputCssFileWritingTest ()
    {
      var stringWriterFactory = new StringWriterFactory ();

      var settings = new AclExpanderApplicationSettings ();
      settings.UseMultipleFileOutput = true;
      settings.Directory = "";
      var application = new AclExpanderApplication (stringWriterFactory);
      application.Run (settings, TextWriter.Null, TextWriter.Null);

      // Multifile HTML output => expect at least 3 files (CSS, main HTML, detail HTML files)
      Assert.That (stringWriterFactory.Count, Is.GreaterThanOrEqualTo (3));

      const string cssFileName = AclExpanderApplication.CssFileName;  
      TextWriterData cssTextWriterData;
      bool cssFileExists = stringWriterFactory.NameToTextWriterData.TryGetValue(cssFileName,out cssTextWriterData);
      Assert.That (cssFileExists, Is.True);

      string result = cssTextWriterData.TextWriter.ToString();
      //Clipboard.SetText (AclExpansionHtmlWriterTest.CreateLiteralResultExpectedString (result));
      Assert.That (result, Is.EqualTo (c_cssFileContent));
    }

    // TODO: Adapt test to use StreamWriterFactory and turn into integration test
    // TODO AE: But still keep a respective unit test.
    [Test]
    public void SingleFileOutputCssFileWritingTest ()
    {
      var stringWriterFactory = new StringWriterFactory ();

      var settings = new AclExpanderApplicationSettings ();
      settings.UseMultipleFileOutput = false;
      settings.Directory = "";
      var application = new AclExpanderApplication (stringWriterFactory);
      application.Run (settings, TextWriter.Null, TextWriter.Null);

      // Single file HTML output => expect 2 files (CSS, HTML file)
      Assert.That (stringWriterFactory.Count, Is.EqualTo (2));

      const string cssFileName = AclExpanderApplication.CssFileName;
      TextWriterData cssTextWriterData;
      bool cssFileExists = stringWriterFactory.NameToTextWriterData.TryGetValue (cssFileName, out cssTextWriterData);
      Assert.That (cssFileExists, Is.True);

      string result = cssTextWriterData.TextWriter.ToString ();
      //Clipboard.SetText (AclExpansionHtmlWriterTest.CreateLiteralResultExpectedString (result));
      Assert.That (result, Is.EqualTo (c_cssFileContent));
    }



    private static void AssertGetCultureName (string cultureNameIn, string cultureNameOut)
    {
      var textWriterFactoryStub = MockRepository.GenerateStub<ITextWriterFactory> ();
      var application = new AclExpanderApplication (textWriterFactoryStub);
      var settings = new AclExpanderApplicationSettings ();
      settings.CultureName = cultureNameIn;
      application.Init (settings, TextWriter.Null, TextWriter.Null);
      string cultureName = application.GetCultureName ();
      Assert.That (cultureName, Is.EqualTo (cultureNameOut));
    }

    private static void AssertFileExists (string testDirectory, string fileName)
    {
      string fileNameExpected = Path.Combine (testDirectory, fileName);
      //To.ConsoleLine.e (() => fileNameExpected);
      Assert.That (File.Exists (fileNameExpected), Is.True);
    }


  }
}