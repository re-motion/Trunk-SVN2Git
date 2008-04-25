using System;
using Remotion.Web.UI.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.ExecutionEngine;

namespace $PROJECT_ROOTNAMESPACE$.Classes
{
  [WebMultiLingualResources ("$PROJECT_ROOTNAMESPACE$.Globalization.Global")]
  public class BaseControl: DataEditUserControl
  {
    public IWxePage WxePage 
    {
      get { return Page as IWxePage; }
    }
  }
}
