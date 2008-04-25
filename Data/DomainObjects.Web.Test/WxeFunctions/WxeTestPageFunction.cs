using System;

using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test.WxeFunctions
{
[Serializable]
public class WxeTestPageFunction : WxeFunction
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public WxeTestPageFunction ()
  {
  }

  // methods and properties

  public ClientTransaction CurrentClientTransaction
  {
    get { return (ClientTransaction) Variables["CurrentClientTransaction"]; }
    set { Variables["CurrentClientTransaction"] = value;}
  }

  private WxePageStep Step1 = new WxePageStep ("WxeTestPage.aspx");
}
}
