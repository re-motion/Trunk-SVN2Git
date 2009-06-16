using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Remotion.Web.Utilities;

namespace Remotion.Web.Test.ExecutionEngine
{
  public class RedirectHandler : IHttpHandler
  {
    public void ProcessRequest (HttpContext context)
    {
      string target = context.Request.QueryString["RedirectTo"];
      if (string.IsNullOrEmpty (target))
        throw new InvalidOperationException ("Url-Parameter 'RedirectTo' is missing.");


      context.Response.Redirect (UrlUtility.ResolveUrl (target));
    }

    public bool IsReusable
    {
      get
      {
        return false;
      }
    }
  }
}
