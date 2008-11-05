/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using log4net.Config;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Remotion.Collections;
using Remotion.Text.CommandLine;

namespace Remotion.Web.ExecutionEngine.CodeGenerator
{
  internal class Program
  {
    private readonly Arguments _arguments;
    private LanguageProvider _inputProvider;

    private static int Main (string[] args)
    {
      // parse arguments / show usage info
      CommandLineClassParser<Arguments> parser = new CommandLineClassParser<Arguments>();
      Arguments arguments = null;
      try
      {
        arguments = parser.Parse (args);
        new Program (arguments).Process();
      }
      catch (CommandLineArgumentException e)
      {
        string appName = Path.GetFileName (Environment.GetCommandLineArgs()[0]);
        Console.Error.WriteLine ("re:call function generator");
        Console.Error.Write (e.Message);
        Console.Error.WriteLine ("Usage: " + parser.GetAsciiSynopsis (appName, 79));
        return 1;
      }
      catch (InputException e)
      {
        Console.Error.WriteLine (
            "{0}({1},{2}): error WG{3:0000}: {4}",
            e.Path,
            e.Line,
            e.Position,
            e.ErrorCode,
            e.Message.Replace ("\r", "\\r"));
        return 1;
      }
      catch (Exception e)
      {
        // write error info accpording to /verbose option
        Console.Error.WriteLine ("Execution aborted: {0}", e.Message);
        if (arguments != null && arguments.Verbose)
        {
          Console.Error.WriteLine ("Detailed exception information:");
          for (Exception current = e; current != null; current = current.InnerException)
          {
            Console.Error.WriteLine ("{0}: {1}", e.GetType().FullName, e.Message);
            Console.Error.WriteLine (e.StackTrace);
          }
        }
        return 1;
      }

      return 0;
    }

    private Program (Arguments args)
    {
      _arguments = args;
    }

    private void Process ()
    {
      if (_arguments.Verbose)
        BasicConfigurator.Configure();

      // select correct CodeDOM provider and configure syntax
      CodeDomProvider codeDomProvider;
      switch (_arguments.Language)
      {
        case Language.CSharp:
          codeDomProvider = new CSharpCodeProvider();
          _inputProvider = new CSharpProvider();

          break;
        case Language.VB:
          codeDomProvider = new VBCodeProvider();
          _inputProvider = new VBProvider();
          break;

        default:
          throw new Exception ("Unknown language " + _arguments.Language);
      }

      // generate classes for each [WxePageFunction] class
      CodeCompileUnit unit = new CodeCompileUnit();

      string fileMask = Path.Combine (Directory.GetCurrentDirectory(), _arguments.FileMask);
      DirectoryInfo directory = new DirectoryInfo (Path.GetDirectoryName (fileMask));
      FileInfo[] files = directory.GetFiles (
          Path.GetFileName (fileMask),
          _arguments.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

      bool outputUpToDate = false;
      FileInfo outputFile = new FileInfo (_arguments.OutputFile);
      if (_arguments.ProjectFile != null)
      {
        if (outputFile.Exists)
          outputUpToDate = true;

        FileInfo projectFile = new FileInfo (_arguments.ProjectFile);
        if (! projectFile.Exists)
          throw new ApplicationException ("Project file " + _arguments.ProjectFile + " not found.");

        if (outputUpToDate && projectFile.LastWriteTimeUtc > outputFile.LastWriteTimeUtc)
          outputUpToDate = false;
      }

      char[] whitespace = new char[] { ' ', '\t' };
      FileInfo file = null;
      try
      {
        for (int idxFile = 0; idxFile < files.Length; ++idxFile)
        {
          file = files[idxFile];

          if (outputUpToDate && file.LastWriteTimeUtc > outputFile.LastWriteTimeUtc)
            outputUpToDate = false;

          var fileProcessor = new FileProcessor(_inputProvider, _arguments.FunctionBaseType);
          fileProcessor.ProcessFile (file, whitespace, unit);
        }
      }
      catch (Exception e)
      {
        if (e is InputException || file == null)
          throw;
        else
          throw new InputException (InputError.Unknown, file.FullName, 1, 1, e);
      }

      // write generated code
      if (! outputUpToDate)
      {
        using (TextWriter writer = new StreamWriter (_arguments.OutputFile, false, Encoding.Unicode))
        {
          Console.WriteLine ("Writing classes to " + _arguments.OutputFile);
          CodeGeneratorOptions options = new CodeGeneratorOptions();
          ICodeGenerator generator = codeDomProvider.CreateGenerator (_arguments.OutputFile);
          generator.GenerateCodeFromCompileUnit (unit, writer, options);
        }
      }
    }

  }
}