using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.PageObjects
{
  public class ActaNovaPopupWindowPageObject : ActaNovaPageObject
  {
    public ActaNovaPopupWindowPageObject ([NotNull] TestObjectContext context)
        : base (context)
    {
    }

    public override string GetTitle ()
    {
      return Scope.FindId ("TitleLabel").Text;
    }
  }
}