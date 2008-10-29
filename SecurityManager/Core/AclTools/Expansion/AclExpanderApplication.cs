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
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.AclTools.Expansion.ConsoleApplication;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpanderApplication : IApplicationRunner<AclExpanderApplicationSettings> 
  {
    private AclExpanderApplicationSettings _settings;
    private ToTextBuilder _logToTextBuilder;
    private ToTextBuilder _errorToTextBuilder;

    public AclExpanderApplication ()
    {
    }



    public void Init (AclExpanderApplicationSettings settings, TextWriter errorWriter, TextWriter logWriter)
    {
      ArgumentUtility.CheckNotNull ("settings", settings);
      ArgumentUtility.CheckNotNull ("errorWriter", errorWriter);
      ArgumentUtility.CheckNotNull ("logWriter", logWriter);

      _settings = settings;
      _logToTextBuilder = new ToTextBuilder (To.ToTextProvider, logWriter);
      _errorToTextBuilder = new ToTextBuilder (To.ToTextProvider, errorWriter);
    }


    public void Run ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        _logToTextBuilder.nl (2).s ("AclExpander").nl().s("==========").nl();
        _logToTextBuilder.sbLiteral("Filter: ",", ","").e("user first name",_settings.UserFirstName).e("user last name",_settings.UserLastName).e("user name",_settings.UserName).se();
        List<AclExpansionEntry> aclExpansion = GetAclExpansion ();
        WriteAclExpansionAsHtmlSpikeToStreamWriter (aclExpansion);
        LogAclExpansion(aclExpansion);
      }
    }

    private void LogAclExpansion (List<AclExpansionEntry> aclExpansion)
    {
      foreach (AclExpansionEntry entry in aclExpansion)
      {
        _logToTextBuilder.nl().e (entry);
      }
    }


    public void WriteAclExpansionAsHtmlSpikeToStringWriter (List<AclExpansionEntry> aclExpansion)
    {
      var stringWriter = new StringWriter ();
      var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (stringWriter, true);
      aclExpansionHtmlWriter.WriteAclExpansionAsHtml (aclExpansion);
      _logToTextBuilder.s (stringWriter.ToString ());
    }

    public void WriteAclExpansionAsHtmlSpikeToStreamWriter (List<AclExpansionEntry> aclExpansion)
    {
      string aclExpansionFileName = "c:\\temp\\AclExpansion_" + FileNameTimestampNow () + ".html";
      using (var streamWriter = new StreamWriter (aclExpansionFileName))
      {
        var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (streamWriter, true);
        aclExpansionHtmlWriter.WriteAclExpansionAsHtml (aclExpansion);
      }
    }

    private string FileNameTimestamp (DateTime dt)
    {
      return StringUtility.ConcatWithSeparator (new [] { dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond }, "_");
    }

    private string FileNameTimestampNow ()
    {
      return FileNameTimestamp (DateTime.Now);
    }


    private List<AclExpansionEntry> GetAclExpansion ()
    {
      var aclExpander = 
          new AclExpander (
            new AclExpanderUserFinder (_settings.UserFirstName, _settings.UserLastName, _settings.UserName), new AclExpanderAclFinder ()
          );

      return aclExpander.GetAclExpansionEntryListSortedAndDistinct();
      //return aclExpander.GetAclExpansionEntryList ();
    }
    

  }
}