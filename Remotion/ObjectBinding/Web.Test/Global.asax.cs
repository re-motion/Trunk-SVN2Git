// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Web;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using log4net;
using log4net.Config;
using Microsoft.Practices.ServiceLocation;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering;
using Remotion.Reflection;
using Remotion.Web.Configuration;

namespace OBWTest
{
  public class Global : HttpApplication // , IResourceUrlResolver
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

    private WaiConformanceLevel _waiConformanceLevelBackup;

    public Global ()
    {
      //  Initialize Logger
      LogManager.GetLogger (typeof (Global));
      InitializeComponent();
    }

    public XmlReflectionBusinessObjectStorageProvider XmlReflectionBusinessObjectStorageProvider
    {
      get { return XmlReflectionBusinessObjectStorageProvider.Current; }
    }

    //  public string GetResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
    //  {
    //    return this.Request.ApplicationPath + "/" + resourceType.Name + "/" + relativeUrl;
    //  }

    protected void Application_Start (Object sender, EventArgs e)
    {
      XmlConfigurator.Configure();

      string objectPath = Server.MapPath ("~/objects");
      if (!Directory.Exists (objectPath))
        Directory.CreateDirectory (objectPath);

      XmlReflectionBusinessObjectStorageProvider provider = new XmlReflectionBusinessObjectStorageProvider (objectPath);
      XmlReflectionBusinessObjectStorageProvider.SetCurrent (provider);
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>().AddService (typeof (IGetObjectService), provider);
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>().AddService (typeof (ISearchAvailableObjectsService), new BindableXmlObjectSearchService ());
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>().AddService (typeof (IBusinessObjectWebUIService), new ReflectionBusinessObjectWebUIService ());

      ServiceLocator.SetLocatorProvider (GetContainer);
    }

    private IServiceLocator GetContainer ()
    {
      IWindsorContainer container = new WindsorContainer ();

      container.Register (
          AllTypes.Pick ()
              .FromAssembly (typeof (RendererBase<>).Assembly)
              .If (t => t.Namespace.EndsWith (".QuirksMode.Factories"))
              .WithService.Select ((t, b) => t.GetInterfaces ())
              .Configure (c => c.Named ("default." + c.ServiceType.Name)));
      
      return new WindsorServiceLocator (container);
    }

    protected void Session_Start (Object sender, EventArgs e)
    {
    }

    protected void Application_BeginRequest (Object sender, EventArgs e)
    {
    }

    protected void Application_AuthenticateRequest (Object sender, EventArgs e)
    {
    }

    protected void Application_PreRequestHandlerExecute (Object sender, EventArgs e)
    {
      _waiConformanceLevelBackup = WebConfiguration.Current.Wcag.ConformanceLevel;
    }

    protected void Application_PostRequestHandlerExecute (Object sender, EventArgs e)
    {
      WebConfiguration.Current.Wcag.ConformanceLevel = _waiConformanceLevelBackup;
    }

    protected void Application_EndRequest (Object sender, EventArgs e)
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
