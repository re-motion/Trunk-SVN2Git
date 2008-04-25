using System;
using Remotion.Collections;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.ExecutionEngine
{

public interface IWxeTemplateControl: ITemplateControl
{
  NameObjectCollection Variables { get; }
  WxePageStep CurrentStep { get; }
  WxeFunction CurrentFunction { get; }
}

}
