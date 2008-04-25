using System;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Security;
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.Security.UI;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
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
      BindableObjectProvider.Current.AddService (typeof (BindableDomainObjectSearchService), new BindableDomainObjectSearchService());
      BindableObjectProvider.Current.AddService (typeof (BindableDomainObjectGetObjectService), new BindableDomainObjectGetObjectService());
      BindableObjectProvider.Current.AddService (typeof (GroupPropertiesSearchService), new GroupPropertiesSearchService ());
      BindableObjectProvider.Current.AddService (typeof (UserPropertiesSearchService), new UserPropertiesSearchService ());
      BindableObjectProvider.Current.AddService (typeof (RolePropertiesSearchService), new RolePropertiesSearchService ());
    }
    
    protected void Application_End (object sender, EventArgs e)
    {
    }
  }
}