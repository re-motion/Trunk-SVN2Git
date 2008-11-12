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
    private ToTextBuilder _logToTextBuilder;
    private ToTextBuilder _errorToTextBuilder;

    private readonly ITextWriterFactory _textWriterFactory;
    public string DirectoryUsed { get; private set; }

    public AclExpanderApplication (ITextWriterFactory textWriterFactory)
    {
      ArgumentUtility.CheckNotNull ("textWriterFactory", textWriterFactory);
      _textWriterFactory = textWriterFactory;
    }

    public AclExpanderApplication () : this(new StreamWriterFactory()) {}


    public AclExpanderApplicationSettings Settings
    {
      get { return _settings; }
    }


    private void Init (AclExpanderApplicationSettings settings, TextWriter errorWriter, TextWriter logWriter)
    {
      _settings = settings;
      _logToTextBuilder = new ToTextBuilder (To.ToTextProvider, logWriter);
      _errorToTextBuilder = new ToTextBuilder (To.ToTextProvider, errorWriter);
    }


    public void Run (AclExpanderApplicationSettings settings, TextWriter errorWriter, TextWriter logWriter)
    {
      ArgumentUtility.CheckNotNull ("settings", settings);
      ArgumentUtility.CheckNotNull ("errorWriter", errorWriter);
      ArgumentUtility.CheckNotNull ("logWriter", logWriter);
      Init (settings, errorWriter, logWriter);

      string cultureName = Settings.CultureName;
      if (String.IsNullOrEmpty (cultureName))
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
            _logToTextBuilder.nl (2).s ("AclExpander").nl().s ("==========").nl();
            _logToTextBuilder.e (Settings);
            LogAclExpansion (aclExpansion);
          }

          WriteAclExpansionAsHtmlSpikeToStreamWriter (aclExpansion);
        }
      }
    }

    private void LogAclExpansion (List<AclExpansionEntry> aclExpansion)
    {
      foreach (AclExpansionEntry entry in aclExpansion)
      {
        _logToTextBuilder.nl().e (entry);
      }
    }


    public void WriteAclExpansionAsHtmlSpikeToStreamWriter (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);
      if (Settings.UseMultipleFileOutput)
      {
        WriteAclExpansionAsMultiFileHtml(aclExpansion);
      }
      else
      {
        WriteAclExpansionAsSingleFileHtml(aclExpansion);
      }
    }

    private void WriteAclExpansionAsSingleFileHtml (List<AclExpansionEntry> aclExpansion)
    {
      _textWriterFactory.Extension = "html";
      DirectoryUsed = Settings.Directory;
      _textWriterFactory.Directory = DirectoryUsed;
      var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (_textWriterFactory.NewTextWriter ("AclExpansion_" + FileNameTimestampNow ()), true);
      aclExpansionHtmlWriter.Settings.OutputRowCount = Settings.OutputRowCount;
      aclExpansionHtmlWriter.WriteAclExpansionAsHtml (aclExpansion);
    }

    private void WriteAclExpansionAsMultiFileHtml (List<AclExpansionEntry> aclExpansion)
    {
      _textWriterFactory.Extension = "html";
      DirectoryUsed = Path.Combine (Settings.Directory, "AclExpansion_" + AclExpanderApplication.FileNameTimestampNow ());
      _textWriterFactory.Directory = DirectoryUsed;
      var aclExpansionMultiFileHtmlWriter = new AclExpansionMultiFileHtmlWriter (_textWriterFactory, true);
      aclExpansionMultiFileHtmlWriter.WriteAclExpansionAsHtml (aclExpansion);
      File.Copy (Path.Combine (".", CssFileName), Path.Combine (DirectoryUsed, CssFileName), true);
    }


    public static string FileNameTimestamp (DateTime dt)
    {
      return StringUtility.ConcatWithSeparator (new [] { dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond }, "_");
    }

    public static string FileNameTimestampNow ()
    {
      return FileNameTimestamp (DateTime.Now);
    }


    private List<AclExpansionEntry> GetAclExpansion ()
    {
      var aclExpander = 
          new AclExpander (
            new AclExpanderUserFinder (Settings.UserFirstName, Settings.UserLastName, Settings.UserName), new AclExpanderAclFinder ()
          );

      return aclExpander.GetAclExpansionEntryListSortedAndDistinct();
    }
    

  }
}