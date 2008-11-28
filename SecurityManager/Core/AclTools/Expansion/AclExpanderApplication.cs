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
using Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpanderApplication : IApplicationRunner<AclExpanderApplicationSettings> 
  {
    public const string CssFileName = "AclExpansion.css";
    private AclExpanderApplicationSettings _settings;
    private ToTextBuilder _logToTextBuilder; // TODO AE: Consider using ILog
    private ToTextBuilder _errorToTextBuilder; // TODO AE: Remove unused field

    private readonly ITextWriterFactory _textWriterFactory;
    public string DirectoryUsed { get; private set; } // TODO AE: Member is always set and read in the same methods, replace by local variables.

    public AclExpanderApplication (ITextWriterFactory textWriterFactory)
    {
      ArgumentUtility.CheckNotNull ("textWriterFactory", textWriterFactory);
      _textWriterFactory = textWriterFactory;
    }

    public AclExpanderApplication () : this(new StreamWriterFactory()) {} // TODO AE: Consider moving default constructors first.


    public AclExpanderApplicationSettings Settings
    {
      get { return _settings; }
    }


    // TODO AE: Move private methods below their usages.
    private void Init (AclExpanderApplicationSettings settings, TextWriter errorWriter, TextWriter logWriter)
    {
      _settings = settings;
      _logToTextBuilder = new ToTextBuilder (To.ToTextProvider, logWriter);
      _errorToTextBuilder = new ToTextBuilder (To.ToTextProvider, errorWriter);
    }


    // TODO AE: Avoid setting class-level state just for one method (Run). If possible, prefer immutable classes and set parameters in ctor. 
    // TODO AE: In this case, consider passing the required parameters to the respective methods.
    public void Run (AclExpanderApplicationSettings settings, TextWriter errorWriter, TextWriter logWriter)
    {
      ArgumentUtility.CheckNotNull ("settings", settings);
      ArgumentUtility.CheckNotNull ("errorWriter", errorWriter);
      ArgumentUtility.CheckNotNull ("logWriter", logWriter);
      Init (settings, errorWriter, logWriter);

      string cultureName = Settings.CultureName;
      // TODO AE: Test this case.
      if (String.IsNullOrEmpty (cultureName)) // TODO AE: Consider checking just for empty string.
      {
        cultureName = null; // Passing null to CultureScope-ctor below means "keep current culture".
      }

      using (new CultureScope (cultureName))
      {
        using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
        {
          List<AclExpansionEntry> aclExpansion = GetAclExpansion();

          if (Settings.Verbose)
          {
            // TODO AE: Consider pulling this block and LogAclExpansion together. (Either in one method or inlined.)
            _logToTextBuilder.nl (2).s ("AclExpander").nl().s ("==========").nl();
            _logToTextBuilder.e (Settings);
            LogAclExpansion (aclExpansion);
          }

          WriteAclExpansionAsHtmlToStreamWriter (aclExpansion);
        }
      }
    }

    private void LogAclExpansion (List<AclExpansionEntry> aclExpansion)
    {
      foreach (AclExpansionEntry entry in aclExpansion)
      { // TODO AE: braces
        _logToTextBuilder.nl().e (entry);
      }
    }


    public void WriteAclExpansionAsHtmlToStreamWriter (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);
      if (Settings.UseMultipleFileOutput)
      { // TODO AE: braces
        WriteAclExpansionAsMultiFileHtml(aclExpansion);
      }
      else
      { // TODO AE: braces
        WriteAclExpansionAsSingleFileHtml(aclExpansion);
      }
    }

    private void WriteAclExpansionAsSingleFileHtml (List<AclExpansionEntry> aclExpansion)
    {
      _textWriterFactory.Extension = "html";
      DirectoryUsed = Settings.Directory;
      _textWriterFactory.Directory = DirectoryUsed;
      using (var textWriter = _textWriterFactory.NewTextWriter ("AclExpansion_" + FileNameTimestampNow ()))
      {
        var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (aclExpansion, textWriter, true);
        aclExpansionHtmlWriter.Settings = CreateAclExpansionHtmlWriterSettings();
        aclExpansionHtmlWriter.WriteAclExpansionAsHtml(); // TODO AE: Inconsistent naming with multi-file writer? (WriteAclExpansion)
      }
      WriteCssFile ();
    }

    private void WriteAclExpansionAsMultiFileHtml (List<AclExpansionEntry> aclExpansion)
    {
      //_textWriterFactory.Extension = "html";
      // TODO AE: Directory called "xxx.html" seems strange.
      DirectoryUsed = Path.Combine (Settings.Directory, "AclExpansion_" + AclExpanderApplication.FileNameTimestampNow () + ".html");
      _textWriterFactory.Directory = DirectoryUsed;

      var aclExpansionMultiFileHtmlWriter = new AclExpansionMultiFileHtmlWriter (_textWriterFactory, true); // TODO AE: Consider shorter name (writer).
      aclExpansionMultiFileHtmlWriter.DetailHtmlWriterSettings = CreateAclExpansionHtmlWriterSettings();
      aclExpansionMultiFileHtmlWriter.WriteAclExpansion (aclExpansion);

      // TODO AE: Remove commented code. (Do not commit.)
      //File.Copy (Path.Combine (".", CssFileName), Path.Combine (DirectoryUsed, CssFileName), true);
      WriteCssFile ();
    }

    // TODO AE: Remove commented code. (Do not commit.)
    //private void WriteCssFile (string directory)
    //{
    //  FileUtility.WriteEmbeddedStringResourceToFile (GetType (), "Data.AclExpansion.css", Path.Combine (directory, CssFileName));
    //}

    private void WriteCssFile ()
    {
      using (var cssTextWriter = _textWriterFactory.NewTextWriter (CssFileName))
      {
        cssTextWriter.Write (GetEmbeddedStringResource ("Data.AclExpansion.css")); // TODO AE: Move CSS file to the namespace of the type used to retrieve it.
      }
    }


    private string GetEmbeddedStringResource (string name)
    {
      Type type = GetType();
      Assembly assembly = type.Assembly;
      using (StreamReader reader = new StreamReader (assembly.GetManifestResourceStream (type, name)))
      {
        return reader.ReadToEnd ();
      }
    }


    // Returns an AclExpansionHtmlWriterSettings initialized from the AclExpanderApplication Settings.
    // TODO AE: Consider moving this method to AclExpanderApplicationSettings or AclExpansionHtmlWriterSettings.
    private AclExpansionHtmlWriterSettings CreateAclExpansionHtmlWriterSettings ()
    {
      var aclExpansionHtmlWriterSettings = new AclExpansionHtmlWriterSettings ();
      aclExpansionHtmlWriterSettings.OutputRowCount = Settings.OutputRowCount;
      aclExpansionHtmlWriterSettings.OutputDeniedRights = Settings.OutputDeniedRights;
      return aclExpansionHtmlWriterSettings;
    }


    // TODO AE: Move statics to top.
    public static string FileNameTimestamp (DateTime dt)
    {
      return StringUtility.ConcatWithSeparator (new [] { dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond }, "_");
    }

    // TODO AE: Move statics to top.
    public static string FileNameTimestampNow ()
    {
      return FileNameTimestamp (DateTime.Now);
    }

    private List<AclExpansionEntry> GetAclExpansion ()
    {
      // TODO AE: Consider using local variables.
      var aclExpander = 
          new AclExpander (
            new AclExpanderUserFinder (Settings.UserFirstName, Settings.UserLastName, Settings.UserName), new AclExpanderAclFinder ()
          );

      return aclExpander.GetAclExpansionEntryListSortedAndDistinct();
    }
    

  }
}