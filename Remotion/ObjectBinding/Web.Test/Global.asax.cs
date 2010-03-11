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
using System.Reflection;
using System.Threading;
using System.Web;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using log4net;
using log4net.Config;
using log4net.Layout;
using Microsoft.Practices.ServiceLocation;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web;
using Remotion.Web.Configuration;
using Remotion.Web.UI.Controls;
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
      var resourceTheme = Remotion.Web.ResourceTheme.Legacy;
      PreferQuirksModeRendering = resourceTheme == Remotion.Web.ResourceTheme.Legacy;

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

      if (PreferQuirksModeRendering)
      {
        RegisterRendererFactories (container,Assembly.Load (new AssemblyName("Remotion.Web.Legacy")));
        RegisterRendererFactories (container, Assembly.Load (new AssemblyName ("Remotion.ObjectBinding.Web.Legacy")));
     }

      RegisterRendererFactories (container, Assembly.Load (new AssemblyName ("Remotion.Web")));
      RegisterRendererFactories (container, Assembly.Load (new AssemblyName ("Remotion.ObjectBinding.Web")));
      container.Register (Component.For<IScriptUtility> ().ImplementedBy<ScriptUtility> ().LifeStyle.Singleton);
      container.Register (Component.For<ResourceTheme> ().Instance (resourceTheme));
      
      Application.Set (typeof (IServiceLocator).AssemblyQualifiedName, new WindsorServiceLocator (container));
      ServiceLocator.SetLocatorProvider (() => (IServiceLocator) Application.Get (typeof (IServiceLocator).AssemblyQualifiedName));
    }

    private void RegisterRendererFactories (IWindsorContainer container, Assembly assembly)
    {
      container.Register (
          AllTypes.Pick()
              .FromAssembly (assembly)
              .If (t => t.Namespace.EndsWith (".Factories"))
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

      try
      {
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture (Request.UserLanguages[0]);
      }
      catch (ArgumentException)
      { }
      try
      {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo (Request.UserLanguages[0]);
      }
      catch (ArgumentException)
      { }

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
