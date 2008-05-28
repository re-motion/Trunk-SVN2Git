using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Web.Test.Domain;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.DomainObjects.Web.Test
{
  /// <summary>
  /// Summary description for Global.
  /// </summary>
  public class Global : System.Web.HttpApplication
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    public Global ()
    {
      InitializeComponent();
    }

    protected void Application_Start (Object sender, EventArgs e)
    {
      MappingConfiguration mappingConfiguration = MappingConfiguration.Current;
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (typeof (ISearchAvailableObjectsService), new BindableDomainObjectSearchService ());
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (typeof (IGetObjectService), new BindableDomainObjectGetObjectService ());
    }

    protected void Session_Start (Object sender, EventArgs e)
    {
    }

    protected void Application_BeginRequest (Object sender, EventArgs e)
    {
    }

    protected void Application_EndRequest (Object sender, EventArgs e)
    {
    }

    protected void Application_AuthenticateRequest (Object sender, EventArgs e)
    {
    }

    protected virtual void Application_PreRequestHandlerExecute (Object sender, EventArgs e)
    {
    }

    protected void Application_PostRequestHandlerExecute (Object sender, EventArgs e)
    {
    }

    protected void Application_Error (Object sender, EventArgs e)
    {
    }

    protected void Session_End (Object sender, EventArgs e)
    {
    }

    protected void Application_End (Object sender, EventArgs e)
    {
    }

    #region Web Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent ()
    {
      this.components = new System.ComponentModel.Container();
    }

    #endregion
  }
}