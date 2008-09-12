using System;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Xml;
using Remotion.Diagnostics.ToText;
using Remotion.Logging;

namespace Remotion.Diagnostics
{
  [Synchronization]
  public static class To
  {
    [ThreadStatic]
    private static ToTextProvider _toTextProvider = new ToTextProvider (GetTypeHandlers(), GetInterfaceHandlers());

    private static readonly string tempPath = System.IO.Path.GetTempPath ();
   
    private static readonly ToTextBuilder _toTextBuilderConsole = new ToTextBuilder (_toTextProvider, System.Console.Out);
    private static readonly ToTextBuilder _toTextBuilderError = new ToTextBuilder (_toTextProvider, System.Console.Error);
    //private static readonly ToTextBuilder _toTextBuilderLog = new ToTextBuilder (toTextProvider, new StreamWriter(Path.GetTempFileName()));
    private static readonly ToTextBuilder _toTextBuilderLog; // = new ToTextBuilder (_toTextProvider, new StreamWriter (System.IO.Path.GetTempPath() + "\\remotion.log"));
    private static readonly ToTextBuilderXml _toTextBuilderLogXml; // = new ToTextBuilderXml (_toTextProvider, XmlWriter.Create new StreamWriter (System.IO.Path.GetTempPath () + "\\remotion.log.xml"));
    private static readonly XmlWriterSettings _xmlWriterSettings;

    private static ToTextSpecificHandlerMap<IToTextSpecificTypeHandler> _typeHandlerMap;
    private static ToTextSpecificHandlerMap<IToTextSpecificInterfaceHandler> _interfaceHandlerMap;


    static To ()
    {
      _toTextBuilderLog = new ToTextBuilder (_toTextProvider, new StreamWriter (TempPath + "\\remotion.log"));
      
      _xmlWriterSettings = new XmlWriterSettings ();
      _xmlWriterSettings.OmitXmlDeclaration = false;
      _xmlWriterSettings.Indent = true;
      _xmlWriterSettings.NewLineOnAttributes = false;

      var xmlWriter = XmlWriter.Create (new StreamWriter (TempPath + "\\remotion.log.xml"), _xmlWriterSettings);
      _toTextBuilderLogXml = new ToTextBuilderXml (_toTextProvider, xmlWriter);
    }

    public static ToTextProvider ToTextProvider { get { return _toTextProvider; } }


    public static ToTextBuilder Console 
    {
      get
      {
        return _toTextBuilderConsole;
      }
    }

    public static ToTextBuilder Error 
    {
      get {
        return _toTextBuilderError;
      }
    }

    public static ToTextBuilder TempLog
    {
      get
      {
        return _toTextBuilderLog;
      }
    }

    public static ToTextBuilderXml TempLogXml
    {
      // TODO: Must make sure ToTextBuilderXml.End is called. finalize ToTextBuilderXml ?
      get
      {
        return _toTextBuilderLogXml;
      }
    }

    //public static void Stream (TextWriter textWriter)
    //{

    //}

    public static ToTextBuilder String
    {
      get
      {
        return new ToTextBuilder (_toTextProvider, new StringWriter ());
      }
    }


    public static string TempPath
    {
      get { return tempPath; }
    }

    public static string Text (object obj)
    {
      return _toTextProvider.ToTextString (obj);
    }

    public static ToTextSpecificHandlerMap<IToTextSpecificTypeHandler> GetTypeHandlers()
    {
      if (_typeHandlerMap == null)
      {
        var handlerCollector = new ToTextSpecificHandlerCollector ();
        _typeHandlerMap = handlerCollector.CollectTypeHandlers ();
      }
      return _typeHandlerMap;
    }

    public static ToTextSpecificHandlerMap<IToTextSpecificInterfaceHandler> GetInterfaceHandlers ()
    {
      if (_interfaceHandlerMap == null)
      {
        var handlerCollector = new ToTextSpecificHandlerCollector ();
        _interfaceHandlerMap = handlerCollector.CollectInterfaceHandlers();
      }
      return _interfaceHandlerMap;
    }

    public static void RegisterHandler<T> (Action<T, IToTextBuilderBase> handler)
    {
      _toTextProvider.RegisterSpecificTypeHandler<T> (handler);
    }

    public static void ClearHandlers ()
    {
      _toTextProvider.ClearSpecificTypeHandlers();
    }


    public static void TextEnableAutomatics (bool enable)
    {
      var settings = _toTextProvider.Settings;
      settings.UseAutomaticObjectToText = enable;
      settings.UseAutomaticStringEnclosing = enable;
      settings.UseAutomaticCharEnclosing = enable;
      settings.UseInterfaceHandlers = enable;
    }

 
  }


  static class ToTextILogExtensionMethods
  {
    static void Log(this ILog log, LogLevel logLevel, ToTextBuilder toTextBuilder)
    {
      log.Log (logLevel, toTextBuilder.CheckAndConvertToString());
    }
  }
}