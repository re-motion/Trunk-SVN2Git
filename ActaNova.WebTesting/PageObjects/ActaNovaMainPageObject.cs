using System;
using ActaNova.WebTesting.ControlObjects;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.PageObjects
{
  public class ActaNovaMainPageObject : ActaNovaPageObject
  {
    public ActaNovaMainPageObject ([NotNull] TestObjectContext context)
        : base (context)
    {
    }

    public override string GetTitle ()
    {
      return DetailsArea.FormPageTitle;
    }

    public ActaNovaHeader Header
    {
      get
      {
        var headerScope = Scope.FindId ("HeaderPanel");
        return new ActaNovaHeader (Context.CloneForScope (headerScope));
      }
    }

    public ActaNovaMainMenu MainMenu
    {
      get
      {
        var mainMenuScope = Scope.FindId ("MainMenu");
        return new ActaNovaMainMenu (Context.CloneForScope (mainMenuScope));
      }
    }

    public ActaNovaTree Tree
    {
      get
      {
        var treeScope = Scope.FindId ("MainTreeView");
        return new ActaNovaTree (Context.CloneForScope (treeScope));
      }
    }

    public ActaNovaDetailsArea DetailsArea
    {
      get
      {
        var detailsAreaScope = Scope.FindFrame ("RightFrameContent");
        return new ActaNovaDetailsArea (Context.CloneForFrame (detailsAreaScope));
      }
    }
  }
}