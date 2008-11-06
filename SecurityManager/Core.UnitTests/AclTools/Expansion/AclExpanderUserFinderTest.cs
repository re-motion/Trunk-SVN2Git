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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpanderUserFinderTest : AclToolsTestBase
  {
    [Test]
    [Explicit]
    public void ListAllUsers ()
    {
      var userFinder = new AclExpanderUserFinder ();
      var users = userFinder.FindUsers ();
      foreach (var user in users)
      {
        //To.ConsoleLine.sb ().e (() => user).e (user.FirstName).e (user.LastName).e (user.UserName).e (user.DisplayName).se ();
      }

      //(user=["User.Tenant2"],"User","Tenant 2","User.Tenant2","Tenant 2 User")
      //(user=["test.user"],"test","user","test.user","user test, Dipl.Ing.(FH)")
      //(user=["group1/user1"],"","user1","group1/user1","user1")
      //(user=["group0/user1"],"","user1","group0/user1","user1")
      //(user=["group0/user2"],"","user2","group0/user2","user2")
      //(user=["group1/user2"],"","user2","group1/user2","user2")
    }


    [Test]
    public void FindAllUsersTest ()
    {
      var userFinder = new AclExpanderUserFinder();
      var users = userFinder.FindUsers ();
      Assert.That (users.Count, Is.EqualTo (6));
    }


    [Test]
    public void FirstNameFilterTest ()
    {
      const string firstName = "test";
      var userFinder = new AclExpanderUserFinder (firstName, null, null);

      var users = userFinder.FindUsers();
      Assert.That (users.Count, Is.EqualTo (1));
      Assert.That (users[0].FirstName, Is.EqualTo (firstName));
    }

    [Test]
    public void LastNameFilterTest ()
    {
      const string lastName = "user2";
      var userFinder = new AclExpanderUserFinder (null, lastName, null);

      var users = userFinder.FindUsers ();

      //var expectedUserNameList = List.New ("group1/user2", "group0/user2");

      Assert.That (users.Count, Is.EqualTo (2));
      Assert.That (users[0].LastName, Is.EqualTo (lastName));
      Assert.That (users[1].LastName, Is.EqualTo (lastName));
    }

    [Test]
    public void UserNameFilterTest ()
    {
      const string userName = "group0/user1";
      var userFinder = new AclExpanderUserFinder (null, null, userName);

      var users = userFinder.FindUsers ();
      Assert.That (users.Count, Is.EqualTo (1));
      Assert.That (users[0].UserName, Is.EqualTo (userName));
    }

    [Test]
    public void AllNamesFilterTest ()
    {
      const string firstName = "User";
      const string lastName = "Tenant 2";
      const string userName = "User.Tenant2";
      var userFinder = new AclExpanderUserFinder (firstName, lastName, userName);

      var users = userFinder.FindUsers ();
      Assert.That (users.Count, Is.EqualTo (1));
      Assert.That (users[0].FirstName, Is.EqualTo (firstName));
      Assert.That (users[0].LastName, Is.EqualTo (lastName));
      Assert.That (users[0].UserName, Is.EqualTo (userName));
    }
  }
}