using System;

namespace Remotion.Web.UI.Controls
{

public interface IControlStateManager
{
  void LoadControlState (object state);
  object SaveControlState();
}

}
