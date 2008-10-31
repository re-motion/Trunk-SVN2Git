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
using System.Threading;
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
    private readonly List<AccessControlList> _aclList = new List<AccessControlList>();
    private readonly List<SecurityManager.Domain.OrganizationalStructure.User> _userList = new List<User> ();

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
    }


    //[user="test.user",role=["test.user","testOwningGroup","Manager"],{["FirstAccessType"]},conditions=[]]
    //[user="test.user",role=["test.user","testRootGroup","Official"],{["FirstAccessType"]},conditions=[]]
    //[user="test.user",role=["test.user","testGroup","Manager"],{["FirstAccessType"]},conditions=[]]
    //[user="test.user",role=["test.user","testGroup","Official"],{["FirstAccessType"]},conditions=[]]
    //[user="group1/user1",role=["group1/user1","parentGroup1","Manager"],{["FirstAccessType"]},conditions=[]]
    //[user="group0/user1",role=["group0/user1","parentGroup0","Manager"],{["FirstAccessType"]},conditions=[]]
    //[user="group1/user2",role=["group1/user2","parentGroup1","Official"],{["FirstAccessType"]},conditions=[]]
    //[user="group0/user2",role=["group0/user2","parentGroup0","Official"],{["FirstAccessType"]},conditions=[]]


    public List<AclExpansionEntry> CreateAclExpanderApplicationAndCallGetAclExpansion (AclExpanderApplicationSettings settings)
    {
      var application = new AclExpanderApplication();
      application.Init (settings, new StringWriter(), new StringWriter());
      return  (List<AclExpansionEntry>) PrivateInvoke.InvokeNonPublicMethod (application, "GetAclExpansion");
      
      //foreach (AclExpansionEntry entry in aclExpansion)
      //{
      //  To.ConsoleLine.e (entry);
      //}
    }

    [Test]
    public void FindAllUsersTest ()
    {
      var settings = new AclExpanderApplicationSettings();
      var aclExpansion = CreateAclExpanderApplicationAndCallGetAclExpansion (settings);
      Assert.That (aclExpansion.Count, Is.EqualTo (8));
    }


    [Test]
    public void FirstNameFilterTest ()
    {
      const string firstName = "test";
      var settings = new AclExpanderApplicationSettings ();
      settings.UserFirstName = firstName;
      var aclExpansion = CreateAclExpanderApplicationAndCallGetAclExpansion (settings);
      Assert.That (aclExpansion.Count, Is.EqualTo (4));
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
      Assert.That (aclExpansion.Count, Is.EqualTo (2));
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
      Assert.That (aclExpansion.Count, Is.EqualTo (4));
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
      Assert.That (aclExpansion.Count, Is.EqualTo (4));
      foreach (AclExpansionEntry entry in aclExpansion)
      {
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


    //[Test]
    //public void VerboseSettingTest ()
    //{
    //  const string firstName = "test";
    //  const string lastName = "user";
    //  const string userName = "group1/user2";
    //  var settings = new AclExpanderApplicationSettings ();
    //  settings.UserFirstName = firstName;
    //  settings.UserLastName = lastName;
    //  settings.UserName = userName;
    //  var aclExpansion = CreateAclExpanderApplicationAndCallGetAclExpansion (settings);
    //  Assert.That (aclExpansion.Count, Is.EqualTo (0));
    //}

    // DONE: Test if directory & extension setting is correctly passed to TextWriterFactory
    // DONE: test if StreamWriterFactory creates stream which writes to the set directory. 

    [Test]
    public void RunSingleFileOutputDirectoryAndExtensionSettingTest ()
    {
      const string directory = "The Directory";
      const string extension = "html";

      var textWriterFactoryMock = MockRepository.GenerateMock<ITextWriterFactory> ();

      textWriterFactoryMock.Expect (mock => mock.Directory = directory); //.Do (invocation => Assert.That (invocation.Arguments[0], Is.SameAs (directory)));
      textWriterFactoryMock.Expect (mock => mock.Extension = extension); //.Do (invocation => Assert.That (invocation.Arguments[0], Is.SameAs (directory)));
      textWriterFactoryMock.Expect (mock => mock.NewTextWriter (Arg<String>.Is.Anything)).Return (TextWriter.Null);
      
      textWriterFactoryMock.Replay();

      var settings = new AclExpanderApplicationSettings ();
      settings.UseMultipleFileOutput = false;
      settings.Directory = directory;
      var application = new AclExpanderApplication (textWriterFactoryMock);

      application.Init (settings, TextWriter.Null, TextWriter.Null);
      Assert.That (application.Settings.Directory, Is.EqualTo (directory));
      application.Run();

      textWriterFactoryMock.VerifyAllExpectations ();
    }

    [Test]
    public void CssFileCopyTest ()
    {
      string directory = Path.GetTempPath();

      File.Create (AclExpanderApplication.CssFileName);

      try
      {
        var settings = new AclExpanderApplicationSettings ();
        settings.UseMultipleFileOutput = true;
        settings.Directory = directory;
        var application = new AclExpanderApplication ();
        application.Init (settings, TextWriter.Null, TextWriter.Null);
        application.Run ();

        Assert.That (File.Exists (Path.Combine (application.DirectoryUsed, AclExpanderApplication.CssFileName)), Is.True);
      }
      finally
      {
        File.Delete (AclExpanderApplication.CssFileName);
      }
    }


  }
}