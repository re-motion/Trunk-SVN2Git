using System;
using Remotion.Security;
using System.Web.UI;

namespace Remotion.Web.UI
{
  //TODO FS: Move to SecurityInterfaces
  //verwendet in web-controls um security abfragen zu tun.
  public interface IWebSecurityAdapter : ISecurityAdapter
  {
    //verwendet fuer buttons etc, secObj = isntanz fur die sec gecheckt wird. handler ist eventhandler von butonclock etc der geschuetz werden soll.
    bool HasAccess (ISecurableObject securableObject, Delegate handler);
    //bool HasStatelessAccess (Type functionType);
    //void CheckAccess (ISecurableObject securableObject, Delegate handler);
  }
}
