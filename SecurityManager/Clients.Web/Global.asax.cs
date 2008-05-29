using System;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;
using Remotion.Security;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.Security.UI;
using Remotion.Web.UI;

namespace Remotion.SecurityManager.Clients.Web
{
  public class Global : SecurityManagerHttpApplication
  {
    protected void Application_Start (object sender, EventArgs e)
    {
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), new ObjectSecurityAdapter());
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), new WebSecurityAdapter());
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), new WxeSecurityAdapter());
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (new GroupPropertiesSearchService());
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (new UserPropertiesSearchService());
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (new RolePropertiesSearchService());
    }

    protected void Application_End (object sender, EventArgs e)
    {
    }
  }
}