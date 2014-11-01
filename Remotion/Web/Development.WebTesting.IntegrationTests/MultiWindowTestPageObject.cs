using System;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  public class MultiWindowTestPageObject : RemotionPageObject
  {
    public MultiWindowTestPageObject (PageObjectContext context)
        : base (context)
    {
    }

    public RemotionPageObject Frame
    {
      get
      {
        var frameScope = Scope.FindFrame ("frame");
        return new RemotionPageObject (Context.CloneForFrame (frameScope));
      }
    }
  }
}