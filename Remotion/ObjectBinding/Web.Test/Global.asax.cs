// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web;
using Autofac;
using AutofacContrib.CommonServiceLocator;
using log4net;
using log4net.Config;
using Microsoft.Practices.ServiceLocation;
using Remotion.Implementation;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.Factories;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Reflection.TypeDiscovery;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Configuration;
using Remotion.Web.Legacy.UI.Controls;
using Remotion.Web.Utilities;

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

    public static bool PreferQuirksModeRendering { get; private set; }

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
      PreferQuirksModeRendering = false;

      string objectPath = Server.MapPath ("~/objects");
      if (!Directory.Exists (objectPath))
        Directory.CreateDirectory (objectPath);

      XmlReflectionBusinessObjectStorageProvider provider = new XmlReflectionBusinessObjectStorageProvider (objectPath);
      XmlReflectionBusinessObjectStorageProvider.SetCurrent (provider);
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>().AddService (typeof (IGetObjectService), provider);
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>()
          .AddService (typeof (ISearchAvailableObjectsService), new BindableXmlObjectSearchService());
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>()
          .AddService (typeof (IBusinessObjectWebUIService), new ReflectionBusinessObjectWebUIService());

      if (PreferQuirksModeRendering)
      {
        var builder = new ContainerBuilder();

        var typeDiscoveryService = ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService();
        var configuration = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (typeDiscoveryService);
        foreach (var entry in configuration)
        {
          if (entry.Lifetime == LifetimeKind.Singleton)
            builder.RegisterType (entry.ImplementationType).As (entry.ServiceType).InstancePerLifetimeScope();
          else
            builder.RegisterType (entry.ImplementationType).As (entry.ServiceType).InstancePerDependency();
        }

        builder.RegisterAssemblyTypes (typeof (QuirksModeRendererBase<>).Assembly, typeof (BocQuirksModeRendererBase<>).Assembly)
            .Where (t => t.Namespace.EndsWith (".Factories")).AsImplementedInterfaces().SingleInstance();

        var autofacServiceLocator = new AutofacServiceLocator (builder.Build());
        ServiceLocator.SetLocatorProvider (() => autofacServiceLocator);

        Assertion.IsTrue (SafeServiceLocator.Current.GetInstance<IBocListRendererFactory>() is BocListQuirksModeRendererFactory);
      }
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

      try
      {
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture (Request.UserLanguages[0]);
      }
      catch (ArgumentException)
      {
      }
      try
      {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo (Request.UserLanguages[0]);
      }
      catch (ArgumentException)
      {
      }
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