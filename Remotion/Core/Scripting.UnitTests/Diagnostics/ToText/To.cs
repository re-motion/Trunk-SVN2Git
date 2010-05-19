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
using System.IO;
using System.Linq.Expressions;
using System.Runtime.Remoting.Contexts;
using Remotion.Diagnostics.ToText.Infrastructure;

namespace Remotion.Diagnostics.ToText
{
  /// <summary>
  /// <para>
  /// Gives convenient access to the transformation of arbitrary objects into human readable text form using 
  /// <see cref="Remotion.Diagnostics.ToText.ToTextBuilder"/> and <see cref="Remotion.Diagnostics.ToText.ToTextProvider"/>.
  /// </para>
  /// </summary>
  /// 
  /// <remarks>
  /// <para>
  /// The <see cref="To"/> class supplies the following functionality:
  /// <list type="number">
  /// <item>Provides convenient access to a <see cref="Remotion.Diagnostics.ToText.ToTextProvider"/> instance through <see cref="Text"/>.</item> 
  /// <item>Automatically registers type and interface handlers for use by <see cref="ToText.ToTextProvider"/> (see examples below)</item>
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
  ///   public override void ToText (ToTextTest t, IToTextBuilder toTextBuilder)
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
  ///   public override void ToText (ITestSimpleName t, IToTextBuilder toTextBuilder)
  ///   {
  ///     toTextBuilder.sb().e ("TestSimple").e (t.Name).se ();
  ///   }
  /// }  
  /// ]]>
  /// </code>
  /// </para>
  /// </example>
  /// </remarks>

  // TODO: Initalize _toTextProvider in ToTextProvider property. Otherwise it will only get initialized for the first thread that uses it (see ThreadStaticAttribute description).
  // TODO: Make _toTextBuilderConsole, etc [ThreadStatic] (for indent, etc).
  // TODO: Make _toTextBuilderLog [ThreadStatic], but make sure all instances share the same file.
  
  [Synchronization]
  public static class To
  {
    [ThreadStatic]
    private static ToTextProvider _toTextProvider = new ToTextProvider (GetTypeHandlers(), GetInterfaceHandlers());

    private static readonly string s_LogFileDirectory = System.IO.Path.GetTempPath ();
   
    private static readonly ToTextBuilder _toTextBuilderConsole = new ToTextBuilder (_toTextProvider, System.Console.Out);
    private static readonly ToTextBuilder _toTextBuilderError = new ToTextBuilder (_toTextProvider, System.Console.Error);
    private static ToTextBuilder _toTextBuilderLog; 

    private static ToTextSpecificHandlerMap<IToTextSpecificTypeHandler> _typeHandlerMap;
    private static ToTextSpecificHandlerMap<IToTextSpecificInterfaceHandler> _interfaceHandlerMap;


    static To ()
    {
      _toTextProvider.Settings.UseAutomaticObjectToText = false;

      //_toTextBuilderLog = new ToTextBuilder (_toTextProvider, new StreamWriter (LogFilePath));

      ToTextBuilderSequenceBegin (_toTextBuilderConsole);
      ToTextBuilderSequenceBegin (_toTextBuilderError);
    }


    /// <summary>
    /// <para>The thread-static <see cref="ToText.ToTextProvider"/>. 
    /// See <see cref="To"/>.<see cref="Text"/> for convenient way to call <see cref="ToText.ToTextProvider.ToTextString"/> for the <see cref="ToTextProvider"/> </para>
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
    /// <para>Returns a <see cref="ToTextBuilder"/> which writes to the console output stream (<see cref="System.Console.Out"/>).</para>
    /// </summary>
    public static ToTextBuilder Console 
    {
      get
      {
        return _toTextBuilderConsole;
      }
    }

    /// <summary>
    /// <para>Returns a <see cref="ToTextBuilder"/> which writes to the console output stream (<see cref="System.Console.Out"/>).
    /// A newline is emitted before returning the <see cref="ToTextBuilder"/>.</para>
    /// </summary>
    public static ToTextBuilder ConsoleLine
    {
      get
      {
        _toTextBuilderConsole.nl();
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
        if (_toTextBuilderLog == null)
        {
          _toTextBuilderLog = new ToTextBuilder (_toTextProvider, new StreamWriter (LogFilePath));
          ToTextBuilderSequenceBegin (_toTextBuilderLog);
        }
        return _toTextBuilderLog;
      }
    }


    /// <summary>
    /// <para>Returns a new <see cref="ToTextBuilder"/> preconfigured to write to a <see cref="StringWriter"/>.
    /// Call <see cref="ToTextBuilder.ToString()"/> on the returned <see cref="ToTextBuilder"/> to get the resulting string.
    /// </para>
    /// </summary>    
    public static ToTextBuilder String
    {
      get
      {
        var toTextBuilder = new ToTextBuilder (_toTextProvider, new StringWriter ());
        ToTextBuilderSequenceBegin(toTextBuilder);
        return toTextBuilder;
      }
    }

    private static void ToTextBuilderSequenceBegin (ToTextBuilder toTextBuilder)
    {
      toTextBuilder.WriteSequenceLiteralBegin ("", "", "", "", " ", "");
    }


    /// <summary>
    /// <para>Returns the path to the logfiles written to by <see cref="TempLog"/> and <see cref="TempLogXml"/>.</para>
    /// </summary>    
    public static string LogFileDirectory
    {
      get { return s_LogFileDirectory; }
    }

    /// <summary>
    /// <para>Returns the path to the XML logfile written to by <see cref="TempLogXml"/>.</para>
    /// </summary>    
    public static string LogFilePath
    {
      get { return LogFileDirectory + "\\remotion.log"; }
    }

    /// <summary>
    /// <para>Returns the path to the plain text logfile written to by <see cref="TempLog"/>.</para>
    /// </summary>    
    public static string XmlLogFilePath
    {
      get { return LogFileDirectory + "\\remotion.log.xml"; }
    }

 


    /// <summary>
    /// <para>Returns all autoregistered type handlers (see <see cref="IToTextSpecificTypeHandler"/>) in the system. 
    /// Type handlers which carry the <see cref="ToTextSpecificHandlerAttribute"/> are autoregistered at 
    /// the first call to <see cref="GetTypeHandlers"/> through reflection. 
    /// Consecutive calls return the cached <see cref="ToTextSpecificHandlerMap{T}"/> of the autoregistered type handlers.
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

    /// <summary>
    /// <para>Returns all autoregistered interface handlers (see <see cref="IToTextSpecificInterfaceHandler"/>) in the system. 
    /// Interface handlers which carry the <see cref="ToTextSpecificHandlerAttribute"/> are autoregistered at 
    /// the first call to <see cref="GetInterfaceHandlers"/> through reflection. 
    /// Consecutive calls return the cached <see cref="ToTextSpecificHandlerMap{T}"/> of the autoregistered interface handlers.
    /// </para>
    /// </summary>  
    public static ToTextSpecificHandlerMap<IToTextSpecificInterfaceHandler> GetInterfaceHandlers ()
    {
      if (_interfaceHandlerMap == null)
      {
        var handlerCollector = new ToTextSpecificHandlerCollector ();
        _interfaceHandlerMap = handlerCollector.CollectInterfaceHandlers();
      }
      return _interfaceHandlerMap;
    }

    ///// <summary>
    ///// <para>Registers
    ///// </para>
    ///// </summary>  
    //public static void RegisterHandler<T> (Action<T, IToTextBuilder> handler)
    //{
    //  _toTextProvider.RegisterSpecificTypeHandler (handler);
    //}

    //public static void ClearHandlers ()
    //{
    //  _toTextProvider.ClearSpecificTypeHandlers();
    //}


    /// <summary>
    /// <para>Enables/disables automatic settings in the <see cref="ToTextProvider"/>
    /// (<see cref="ToTextProviderSettings.UseAutomaticObjectToText"/>,
    /// <see cref="ToTextProviderSettings.UseAutomaticStringEnclosing"/>,
    /// <see cref="ToTextProviderSettings.UseAutomaticCharEnclosing"/>,
    /// <see cref="ToTextProviderSettings.UseInterfaceHandlers"/>),
    /// according to the passed argument.
    /// </para>
    /// </summary>   
    public static void TextEnableAutomatics (bool enable)
    {
      var settings = _toTextProvider.Settings;
      settings.UseAutomaticObjectToText = enable;
      settings.UseAutomaticStringEnclosing = enable;
      settings.UseAutomaticCharEnclosing = enable;
      settings.UseInterfaceHandlers = enable;
    }
  }
}
