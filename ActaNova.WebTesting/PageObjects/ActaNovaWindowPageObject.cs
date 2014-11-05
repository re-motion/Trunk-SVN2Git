using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.PageObjects
{
  // ReSharper disable once ClassNeverInstantiated.Global
  public class ActaNovaWindowPageObject : ActaNovaPageObject
  {
    public ActaNovaWindowPageObject ([NotNull] PageObjectContext context)
        : base(context)
    {
    }

    public void Close ()
    {
      Context.CloseWindow();
    }
  }
}