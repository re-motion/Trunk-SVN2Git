using System;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Xml;
using Remotion.Diagnostics.ToText;
using Remotion.Logging;

namespace Remotion.Diagnostics
{
  /// <summary>
  /// <para>
  /// Gives convenient access to the transformation of arbitrary objects into human readable text form through the following:
  /// <list type="number">
  /// <item>Provides convenient access to a <see cref="Remotion.Diagnostics.ToText.ToTextProvider"/> instance through <see cref="Text"/>.</item> 
  /// <item>Automatically registers type and interface handlers for use by <see cref="ToTextProvider"/> (see examples below)</item>
  /// <item>Supplies several <see cref="Remotion.Diagnostics.ToText.ToTextBuilder"/> instances preconfigured for specific usage scenarios.</item>
  /// </list>
  /// </para>
  /// 
  /// <example>
  /// <para>
  /// The following shows a sample of an autoregistered type handler. 
  /// All type handlers must implement <see cref="IToTextSpecificTypeHandler"/> 
  /// (or derive from a class that does, e.g. <see cref="ToTextSpecificTypeHandler{T}"/>). 
  /// The <see cref="ToTextSpecificHandlerAttribute"/> marks the handler for autoregistration.
  /// <code>
  /// <![CDATA[
  /// [ToTextSpecificHandler]
  /// public class ToTextTestToTextSpecificTypeHandler : ToTextSpecificTypeHandler<ToTextTest>
  /// {
  ///   public override void ToText (ToTextTest t, IToTextBuilderBase toTextBuilder)
  ///   {
  ///     toTextBuilder.s ("handled by ToTextTestToTextSpecificTypeHandler");
  ///   }
  /// }
  /// ]]>
  /// </code>
  /// </para>
  /// </example>
  /// 
  /// <example>
  /// <para>
  /// The following shows a sample of an autoregistered interface handler. 
  /// All interface handlers must implement <see cref="IToTextSpecificInterfaceHandler"/> 
  /// (or derive from a class that does, e.g. <see cref="ToTextSpecificInterfaceHandler{T}"/>). 
  /// The <see cref="ToTextSpecificHandlerAttribute"/> marks the handler for autoregistration.
  /// <code>
  /// <![CDATA[
  /// [ToTextSpecificHandler]
  /// class ITestSimpleNameToTextSpecificInterfaceHandler : ToTextSpecificInterfaceHandler<ITestSimpleName>
  /// {
  ///   public override void ToText (ITestSimpleName t, IToTextBuilderBase toTextBuilder)
  ///   {
  ///     toTextBuilder.sbLiteral ("[", ",", "]").e ("TestSimple").e (t.Name).se ();
  ///   }
  /// }  
  /// ]]>
  /// </code>
  /// </para>
  /// </example>
  /// 
  /// </summary>
  [Synchronization]
  public static class To
  {
    [ThreadStatic]
    private static ToTextProvider _toTextProvider = new ToTextProvider (GetTypeHandlers(), GetInterfaceHandlers());

    private static readonly string s_LogFilePath = System.IO.Path.GetTempPath ();
   
    private static readonly ToTextBuilder _toTextBuilderConsole = new ToTextBuilder (_toTextProvider, System.Console.Out);
    private static readonly ToTextBuilder _toTextBuilderError = new ToTextBuilder (_toTextProvider, System.Console.Error);
    private static readonly ToTextBuilder _toTextBuilderLog; 
    private static readonly ToTextBuilderXml _toTextBuilderLogXml;
    private static readonly XmlWriterSettings _xmlWriterSettings;

    private static ToTextSpecificHandlerMap<IToTextSpecificTypeHandler> _typeHandlerMap;
    private static ToTextSpecificHandlerMap<IToTextSpecificInterfaceHandler> _interfaceHandlerMap;


    static To ()
    {
      _toTextBuilderLog = new ToTextBuilder (_toTextProvider, new StreamWriter (LogFilePath + "\\remotion.log"));
      
      _xmlWriterSettings = new XmlWriterSettings ();
      _xmlWriterSettings.OmitXmlDeclaration = false;
      _xmlWriterSettings.Indent = true;
      _xmlWriterSettings.NewLineOnAttributes = false;

      var xmlWriter = XmlWriter.Create (new StreamWriter (LogFilePath + "\\remotion.log.xml"), _xmlWriterSettings);
      _toTextBuilderLogXml = new ToTextBuilderXml (_toTextProvider, xmlWriter);
    }

    /// <summary>
    /// <para>The thread-static <see cref="ToText.ToTextProvider"/>. 
    /// See <see cref="Text"/> for convenient way to call <see cref="ToText.ToTextProvider.ToTextString"/> for the <see cref="ToTextProvider"/> </para>
    /// </summary>
    public static ToTextProvider ToTextProvider { get { return _toTextProvider; } }


    /// <summary>
    /// <para>Returns a human-readable text representation (using a <see cref="ToTextProvider"/> instance; 
    /// see <see cref="ToText.ToTextProvider"/>) of the passed argument.</para>
    /// </summary>
    public static string Text (object obj)
    {
      return _toTextProvider.ToTextString (obj);
    }

    /// <summary>
    /// <para>Returns a <see cref="ToTextBuilder"/> preconfigured to write to the console output stream (<see cref="System.Console.Out"/>).</para>
    /// </summary>
    public static ToTextBuilder Console 
    {
      get
      {
        return _toTextBuilderConsole;
      }
    }

    /// <summary>
    /// <para>Returns a <see cref="ToTextBuilder"/> preconfigured to write to the console error stream (<see cref="System.Console.Error"/>).</para>
    /// </summary>    
    public static ToTextBuilder Error 
    {
      get {
        return _toTextBuilderError;
      }
    }

    /// <summary>
    /// <para>Returns a <see cref="ToTextBuilder"/> preconfigured to write to a logfile in the users temp directory (<see cref="System.IO.Path.GetTempPath()"/>).</para>
    /// </summary>    
    public static ToTextBuilder TempLog
    {
      get
      {
        return _toTextBuilderLog;
      }
    }

    /// <summary>
    /// <para>Returns a <see cref="ToTextBuilder"/> preconfigured to write to an XML-logfile
    /// in the users temp directory (<see cref="System.IO.Path.GetTempPath()"/>) through an <see cref="ToTextBuilderXml"/>.</para>
    /// </summary>    
    public static ToTextBuilderXml TempLogXml
    {
      // TODO: Must make sure ToTextBuilderXml.End is called. finalize ToTextBuilderXml ?
      get
      {
        return _toTextBuilderLogXml;
      }
    }



    /// <summary>
    /// <para>Returns a new <see cref="ToTextBuilder"/> preconfigured to write to a <see cref="StringWriter"/>.
    /// Call <see cref="ToTextBuilder.CheckAndConvertToString()"/> on the returned <see cref="ToTextBuilder"/> to get the resulting string.
    /// </para>
    /// </summary>    
    public static ToTextBuilder String
    {
      get
      {
        return new ToTextBuilder (_toTextProvider, new StringWriter ());
      }
    }


    /// <summary>
    /// <para>Returns the path to the logfiles written to by <see cref="TempLog"/> and <see cref="TempLogXml"/>.</para>
    /// </summary>    
    public static string LogFilePath
    {
      get { return s_LogFilePath; }
    }


    /// <summary>
    /// <para>Returns all autoregistered specific type handlers (see <see cref="IToTextSpecificTypeHandler"/>) in the system. 
    /// Handlers which carry the <see cref="ToTextSpecificHandlerAttribute"/> are autoregistered at 
    /// the first call to <see cref="GetTypeHandlers"/> through reflection. 
    /// Consecutive calls return the cached <see cref="ToTextSpecificHandlerMap{T}"/> of the autoregistered handlers.
    /// </para>
    /// </summary>    
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