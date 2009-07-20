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
using System.IO;
using System.Web;
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
using Remotion.ObjectBinding.Web.UI.Controls.Rendering;
using Remotion.Web.Configuration;
using Remotion.Web.UI.Controls.Rendering;

namespace OBWTest
{
  public class Global : HttpApplication // , IResourceUrlResolver
  {
    private WaiConformanceLevel _waiConformanceLevelBackup;

    public Global ()
    {
      //  Initialize Logger
      LogManager.GetLogger (typeof (Global));
      InitializeComponent();
    }

    public static bool PreferStandardModeRendering { get; private set; }

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
      PreferStandardModeRendering = true;

      string objectPath = Server.MapPath ("~/objects");
      if (!Directory.Exists (objectPath))
        Directory.CreateDirectory (objectPath);

      XmlReflectionBusinessObjectStorageProvider provider = new XmlReflectionBusinessObjectStorageProvider (objectPath);
      XmlReflectionBusinessObjectStorageProvider.SetCurrent (provider);
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>().AddService (typeof (IGetObjectService), provider);
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>().AddService (
          typeof (ISearchAvailableObjectsService), new BindableXmlObjectSearchService());
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>().AddService (
          typeof (IBusinessObjectWebUIService), new ReflectionBusinessObjectWebUIService());

      IWindsorContainer container = new WindsorContainer();

      if (PreferStandardModeRendering)
        RegisterRendererFactories (container, "StandardMode");
      else
        RegisterRendererFactories (container, "QuirksMode");
      
      Application.Set (typeof (IServiceLocator).AssemblyQualifiedName, new WindsorServiceLocator (container));
      ServiceLocator.SetLocatorProvider (() => (IServiceLocator) Application.Get (typeof (IServiceLocator).AssemblyQualifiedName));
    }

    private void RegisterRendererFactories (IWindsorContainer container, string namespaceQualifier)
    {
      // Remotion.Web.Core
      container.Register (
          AllTypes.Pick()
              .FromAssembly (typeof (RendererBase<>).Assembly)
              .If (t => t.Namespace.EndsWith (string.Format (".{0}.Factories", namespaceQualifier)))
              .WithService.Select ((t, b) => t.GetInterfaces())
              .Configure (c => c.Named (c.ServiceType.Name)));
      // Remotion.ObjectBinding.Web
      container.Register (
          AllTypes.Pick()
              .FromAssembly (typeof (BocRendererBase<>).Assembly)
              .If (t => t.Namespace.EndsWith (string.Format (".{0}.Factories", namespaceQualifier)))
              .WithService.Select ((t, b) => t.GetInterfaces())
              .Configure (c => c.Named (c.ServiceType.Name)));
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
    }

    #endregion
  }
}