using System;
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class CommandControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var command = home.GetCommand().ByID ("body_Command1");
      Assert.That (command.Scope.Id, Is.EqualTo ("body_Command1"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var command = home.GetCommand().ByIndex (2);
      Assert.That (command.Scope.Id, Is.EqualTo ("body_Command2"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var command = home.GetCommand().ByLocalID ("Command1");
      Assert.That (command.Scope.Id, Is.EqualTo ("body_Command1"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var command = home.GetCommand().First();
      Assert.That (command.Scope.Id, Is.EqualTo ("body_Command1"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start();
      var scope = new ScopeControlObject (home.Context.CloneForControl (home, home.Scope.FindId ("scope")));

      var command = scope.GetCommand().Single();
      Assert.That (command.Scope.Id, Is.EqualTo ("body_Command2"));

      try
      {
        home.GetCommand().Single();
        Assert.Fail ("Should not be able to unambigously find a command.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestClick ()
    {
      var home = Start();

      var command1 = home.GetCommand().ByLocalID ("Command1");
      home = command1.Click().Expect<RemotionPageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("Command1ItemID"));

      var command2 = home.GetCommand().ByLocalID ("Command2");
      home = command2.Click().Expect<RemotionPageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);
    }

    private RemotionPageObject Start ()
    {
      return Start ("CommandTest.wxe");
    }
  }
}