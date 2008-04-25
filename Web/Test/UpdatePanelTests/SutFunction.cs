using System;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Test.UpdatePanelTests
{
  public class SutFunction : WxeFunction
  {
    private WxeStep Step1 = new WxePageStep ("~/UpdatePanelTests/SutForm.aspx");
  }
}