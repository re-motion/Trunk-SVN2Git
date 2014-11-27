using System;
using ActaNova.WebTesting.Infrastructure;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ActaNovaHeaderControlObjectTest : ActaNovaWebTestBase
  {
    [Test]
    public void TestGetCurrentUser ()
    {
      var home = Start();

      Assert.That (home.Header.GetCurrentUser(), Is.EqualTo ("Muster Max, Ing."));
    }

    [Test]
    public void TestGetCurrentGroup ()
    {
      var home = Start();

      Assert.That (home.Header.GetCurrentGroup(), Is.EqualTo ("EG/1"));
    }

    [Test]
    public void TestGetCurrentApplicationContext ()
    {
      var home = Start();

      Assert.That (home.Header.GetCurrentApplicationContext(), Is.Null);
    }

    [Test]
    public void TestDefaultGroupControl ()
    {
      var home = Start();

      var defaultGroupControl = home.Header.OpenDefaultGroupControlWhenStandardIsDisplayed();
      Assert.That (defaultGroupControl.GetText(), Is.Empty);

      defaultGroupControl.SelectOption().WithDisplayText ("Kanzlei (Kanzlei)");

      defaultGroupControl = home.Header.OpenDefaultGroupControl();
      Assert.That (defaultGroupControl.GetText(), Is.EqualTo ("Kanzlei (Kanzlei)"));

      defaultGroupControl.SelectOption().WithIndex (1);

      defaultGroupControl = home.Header.OpenDefaultGroupControlWhenStandardIsDisplayed();
      Assert.That (defaultGroupControl.GetText(), Is.Empty);
    }

    [Test]
    public void TestCurrentTenantControl ()
    {
      var home = Start();

      var currentTenantControl = home.Header.OpenCurrentTenantControl();
      Assert.That (currentTenantControl.GetText(), Is.EqualTo ("Acta Nova Gemeinde"));

      currentTenantControl.SelectOption().WithDisplayText ("Acta Nova Ortsteil 1", Opt.ContinueWhen (ActaNovaCompletion.OuterInnerOuterUpdated));

      currentTenantControl = home.Header.OpenCurrentTenantControl();
      Assert.That (currentTenantControl.GetText(), Is.EqualTo ("Acta Nova Ortsteil 1"));

      currentTenantControl.SelectOption().WithIndex (1, Opt.ContinueWhen (ActaNovaCompletion.OuterInnerOuterUpdated));

      currentTenantControl = home.Header.OpenCurrentTenantControl();
      Assert.That (currentTenantControl.GetText(), Is.EqualTo ("Acta Nova Gemeinde"));
    }

    [Test]
    public void TestGetBreadCrumbs ()
    {
      var home = Start();

      Assert.That (home.Header.GetNumberOfBreadCrumbs(), Is.EqualTo (1));
      Assert.That (home.Header.GetBreadCrumb(1).GetText(), Is.EqualTo ("Eigener AV"));

      // Note: tests for ActaNovaBreadCrumbControlObject are found in ActaNovaBreadCrumbControlObjectTest, this test only tests the Header property.
    }
  }
}