using System;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using System.Web.UI;

namespace Remotion.Web.UI.Controls
{

public interface IUserControl: ITemplateControl, IAttributeAccessor, IUserControlDesignerAccessor
{
  void DesignerInitialize();
  void InitializeAsUserControl(Page page);
  string MapPath(string virtualPath);

  HttpApplicationState Application { get; }
  AttributeCollection Attributes { get; }
  Cache Cache { get; }
  bool IsPostBack { get; }
  HttpRequest Request { get; }
  HttpResponse Response { get; }
  HttpServerUtility Server { get; }
  HttpSessionState Session { get; }
  TraceContext Trace { get; }
}

}
