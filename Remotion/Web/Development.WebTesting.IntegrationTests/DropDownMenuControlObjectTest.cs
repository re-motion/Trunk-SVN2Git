﻿using System;
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  /// <remarks>
  /// Note that the <see cref="DropDownMenu.Mode"/>=<see cref="MenuMode.ContextMenu"/> option is tested indirectly by the BocTreeViewControlObjectTest.
  /// </remarks>
  [TestFixture]
  public class DropDownMenuControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start ("DropDownMenuTest.wxe");

      var dropDownMenu = home.GetDropDownMenu().ByID ("body_MyDropDownMenu");
      Assert.That (dropDownMenu.Scope.Id, Is.EqualTo ("body_MyDropDownMenu"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start ("DropDownMenuTest.wxe");

      var dropDownMenu = home.GetDropDownMenu().ByIndex (2);
      Assert.That (dropDownMenu.Scope.Id, Is.EqualTo ("body_MyDropDownMenu2"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start ("DropDownMenuTest.wxe");

      var dropDownMenu = home.GetDropDownMenu().ByLocalID ("MyDropDownMenu");
      Assert.That (dropDownMenu.Scope.Id, Is.EqualTo ("body_MyDropDownMenu"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start ("DropDownMenuTest.wxe");

      var dropDownMenu = home.GetDropDownMenu().First();
      Assert.That (dropDownMenu.Scope.Id, Is.EqualTo ("body_MyDropDownMenu"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start ("DropDownMenuTest.wxe");
      var scope = new ScopeControlObject ("scope", home.Context.CloneForScope (home.Scope.FindId ("scope")));

      var dropDownMenu = scope.GetDropDownMenu().Single();
      Assert.That (dropDownMenu.Scope.Id, Is.EqualTo ("body_MyDropDownMenu2"));

      try
      {
        home.GetDropDownMenu().Single();
        Assert.Fail ("Should not be able to unambigously find a list menu.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestClickItem ()
    {
      var home = Start ("DropDownMenuTest.wxe");

      var dropDownMenu = home.GetDropDownMenu().ByLocalID ("MyDropDownMenu");

      dropDownMenu.ClickItem ("ItemID5");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("ItemID5|Event"));

      dropDownMenu.ClickItem (2);
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);

      dropDownMenu.ClickItemByHtmlID ("body_MyDropDownMenu_3");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("ItemID4|WxeFunction"));

      dropDownMenu.ClickItemByText ("EventItem");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("ItemID1|Event"));
    }
  }
}