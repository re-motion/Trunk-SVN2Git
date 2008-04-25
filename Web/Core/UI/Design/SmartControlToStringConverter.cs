using System;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UI.Design
{

public class SmartControlToStringConverter: ControlToStringConverter
{
  public SmartControlToStringConverter ()
    : base (typeof (ISmartControl))
  {
  }
}

}
