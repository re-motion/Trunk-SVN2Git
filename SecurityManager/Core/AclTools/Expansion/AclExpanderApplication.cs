// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
// 
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion.ConsoleApplication;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpanderApplication : IApplicationRunner<AclExpanderApplicationSettings>
  {
    //public const string CssFileName = "AclExpansion.css";
    public const string CssFileName = "AclExpansion";
    private AclExpanderApplicationSettings _settings;
    private ToTextBuilder _logToTextBuilder; // TODO QAE: Consider using ILog; MGi: Advantage ?
    private ToTextBuilder _errorToTextBuilder; // TODO QAE: Consider using ILog; MGi: Advantage ?

    private readonly ITextWriterFactory _textWriterFactory;


    public AclExpanderApplication ()
        : this (new StreamWriterFactory())
    {
    }

    public AclExpanderApplication (ITextWriterFactory textWriterFactory)
    {
      ArgumentUtility.CheckNotNull ("textWriterFactory", textWriterFactory);
      _textWriterFactory = textWriterFactory;
    }


    public AclExpanderApplicationSettings Settings
    {
      get { return _settings; }
    }

    public ToTextBuilder LogToTextBuilder
    {
      get { return _logToTextBuilder; }
    }

    public ToTextBuilder ErrorToTextBuilder
    {
      get { return _errorToTextBuilder; }
    }

    public void Run (AclExpanderApplicationSettings settings, TextWriter errorWriter, TextWriter logWriter)
    {
      ArgumentUtility.CheckNotNull ("settings", settings);
      ArgumentUtility.CheckNotNull ("errorWriter", errorWriter);
      ArgumentUtility.CheckNotNull ("logWriter", logWriter);
      Init (settings, errorWriter, logWriter);

      string cultureName = GetCultureName();

      using (new CultureScope (cultureName))
      {
        using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
        {
          List<AclExpansionEntry> aclExpansion = GetAclExpansion();

          if (Settings.Verbose)
            LogAclExpansion (aclExpansion);

          WriteAclExpansionAsHtmlToStreamWriter (aclExpansion);
        }
      }
    }

    public string GetCultureName ()
    {
      string cultureName = Settings.CultureName;
      if (String.IsNullOrEmpty (cultureName)) 
      {
        cultureName = null; // Passing null to CultureScope-ctor below means "keep current culture".
      }
      return cultureName;
    }


    public void Init (AclExpanderApplicationSettings settings, TextWriter errorWriter, TextWriter logWriter)
    {
      _settings = settings;
      _logToTextBuilder = new ToTextBuilder (To.ToTextProvider, logWriter);
      _errorToTextBuilder = new ToTextBuilder (To.ToTextProvider, errorWriter);
    }

    private void LogAclExpansion (List<AclExpansionEntry> aclExpansion)
    {
      LogToTextBuilder.nl (2).s ("AclExpander").nl().s ("==========").nl();
      LogToTextBuilder.e (Settings);
      foreach (AclExpansionEntry entry in aclExpansion)
        LogToTextBuilder.nl().e (entry);
    }


    public void WriteAclExpansionAsHtmlToStreamWriter (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);
      if (Settings.UseMultipleFileOutput)
      {
        WriteAclExpansionAsMultiFileHtml (aclExpansion);
      }
      else
      {
        WriteAclExpansionAsSingleFileHtml (aclExpansion);
      }
    }

    private void WriteAclExpansionAsSingleFileHtml (List<AclExpansionEntry> aclExpansion)
    {
      _textWriterFactory.Extension = "html";
      string directoryUsed = Settings.Directory;
      _textWriterFactory.Directory = directoryUsed;
      using (var textWriter = _textWriterFactory.NewTextWriter ("AclExpansion_" + FileNameTimestampNow()))
      {
        var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (textWriter, true);
        aclExpansionHtmlWriter.Settings = CreateAclExpansionHtmlWriterSettings();
        aclExpansionHtmlWriter.WriteAclExpansion(aclExpansion);
      }
      WriteCssFile();
    }

    private void WriteAclExpansionAsMultiFileHtml (List<AclExpansionEntry> aclExpansion)
    {
      // TODO AE: Directory called "xxx.html" seems strange.
      //string directoryUsed = Path.Combine (Settings.Directory, "AclExpansion_" + AclExpanderApplication.FileNameTimestampNow() + ".html");
      string directoryUsed = Path.Combine (Settings.Directory, "AclExpansion_" + AclExpanderApplication.FileNameTimestampNow ());
      _textWriterFactory.Directory = directoryUsed;
      _textWriterFactory.Extension = "html";

      var multiFileHtmlWriter = new AclExpansionMultiFileHtmlWriter (_textWriterFactory, true);
      multiFileHtmlWriter.DetailHtmlWriterSettings = CreateAclExpansionHtmlWriterSettings();
      multiFileHtmlWriter.WriteAclExpansion (aclExpansion);

      WriteCssFile();
    }

    private void WriteCssFile ()
    {
      using (var cssTextWriter = _textWriterFactory.NewTextWriter (_textWriterFactory.Directory,CssFileName,"css"))
      {
        string resource = GetEmbeddedStringResource ("Data.AclExpansion.css");
        Assertion.IsNotNull (resource);
        cssTextWriter.Write (resource);
        // TODO AE: Move CSS file to the namespace of the type used to retrieve it.
      }
    }


    private string GetEmbeddedStringResource (string name)
    {
      Type type = GetType();
      Assembly assembly = type.Assembly;
      using (StreamReader reader = new StreamReader (assembly.GetManifestResourceStream (type, name)))
      {
        return reader.ReadToEnd();
      }
    }


    // Returns an AclExpansionHtmlWriterSettings initialized from the AclExpanderApplication Settings.
    private AclExpansionHtmlWriterSettings CreateAclExpansionHtmlWriterSettings ()
    {
      var aclExpansionHtmlWriterSettings = new AclExpansionHtmlWriterSettings();
      aclExpansionHtmlWriterSettings.OutputRowCount = Settings.OutputRowCount;
      aclExpansionHtmlWriterSettings.OutputDeniedRights = Settings.OutputDeniedRights;
      return aclExpansionHtmlWriterSettings;
    }


    // TODO AE: Move statics to top.
    public static string FileNameTimestamp (DateTime dt)
    {
      return StringUtility.ConcatWithSeparator (new[] { dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond }, "_");
    }

    // TODO AE: Move statics to top.
    public static string FileNameTimestampNow ()
    {
      return FileNameTimestamp (DateTime.Now);
    }

    protected virtual List<AclExpansionEntry> GetAclExpansion ()
    {
      var aclExpander =
          new AclExpander (
              new AclExpanderUserFinder (Settings.UserFirstName, Settings.UserLastName, Settings.UserName), new AclExpanderAclFinder()
              );

      return aclExpander.GetAclExpansionEntryListSortedAndDistinct();
    }
  }
}