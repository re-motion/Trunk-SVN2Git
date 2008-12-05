// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory;
using Remotion.Utilities;
using Remotion.Utilities.ConsoleApplication;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpanderApplication : IApplicationRunner<AclExpanderApplicationSettings>
  {
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
      using (var textWriter = _textWriterFactory.CreateTextWriter ("AclExpansion_" + StringUtility.GetFileNameTimestampNow ()))
      {
        var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (textWriter, true, CreateAclExpansionHtmlWriterSettings ());
        aclExpansionHtmlWriter.WriteAclExpansion(aclExpansion);
      }
      WriteCssFile();
    }

    private void WriteAclExpansionAsMultiFileHtml (List<AclExpansionEntry> aclExpansion)
    {
      string directoryUsed = Path.Combine (Settings.Directory, "AclExpansion_" + StringUtility.GetFileNameTimestampNow ());
      _textWriterFactory.Directory = directoryUsed;
      _textWriterFactory.Extension = "html";

      var multiFileHtmlWriter = new AclExpansionMultiFileHtmlWriter (_textWriterFactory, true);
      multiFileHtmlWriter.DetailHtmlWriterSettings = CreateAclExpansionHtmlWriterSettings();
      multiFileHtmlWriter.WriteAclExpansion (aclExpansion);

      WriteCssFile();
    }

    private void WriteCssFile ()
    {
      using (var cssTextWriter = _textWriterFactory.CreateTextWriter (_textWriterFactory.Directory,CssFileName,"css"))
      {
        string resource = GetEmbeddedStringResource ("AclExpansion.css");
        Assertion.IsNotNull (resource);
        cssTextWriter.Write (resource);
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