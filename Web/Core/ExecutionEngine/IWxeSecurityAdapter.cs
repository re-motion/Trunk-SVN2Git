using System;
using Remotion.Security;

namespace Remotion.Web.ExecutionEngine
{
  //TODO FS: Move to SecurityInterfaces
  //verwendet in wxe um security abfragen zu tun.
  public interface IWxeSecurityAdapter : ISecurityAdapter
  {
    // verwendet wenn function läuft. 
    bool HasAccess (WxeFunction function);
    //verwendet bevor wxefunction initialisiert wurde und nur typ bekannt ist.
    bool HasStatelessAccess (Type functionType);
    // verwendet wenn function läuft. zb um zurgriffe auf urls (= wxefunction) zu schützen.
    void CheckAccess (WxeFunction function);
  }
}
