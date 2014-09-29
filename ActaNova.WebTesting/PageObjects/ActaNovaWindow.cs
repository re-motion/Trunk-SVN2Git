using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.PageObjects
{
  public class ActaNovaWindow : ActaNovaPageObject
  {
    public ActaNovaWindow ([NotNull] TestObjectContext context)
        : base(context)
    {
    }
  }
}