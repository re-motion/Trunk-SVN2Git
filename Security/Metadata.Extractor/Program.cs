using System;
using System.Globalization;
using Remotion.Text.CommandLine;

namespace Remotion.Security.Metadata.Extractor
{
  public class Program
  {
    public static int Main (string[] args)
    {
      CommandLineArguments arguments = GetArguments (args);
      if (arguments == null)
        return 1;

      return ExtractMetadata (arguments);
    }

    private static CommandLineArguments GetArguments (string[] args)
    {
      CommandLineClassParser parser = new CommandLineClassParser (typeof (CommandLineArguments));

      try
      {
        return (CommandLineArguments) parser.Parse (args);
      }
      catch (CommandLineArgumentException e)
      {
        Console.WriteLine (e.Message);
        WriteUsage (parser);

        return null;
      }
    }

    private static void WriteUsage (CommandLineClassParser parser)
    {
      Console.WriteLine ("Usage:");

      string commandName = Environment.GetCommandLineArgs ()[0];
      Console.WriteLine (parser.GetAsciiSynopsis (commandName, Console.BufferWidth));
    }

    private static int ExtractMetadata (CommandLineArguments arguments)
    {
      try
      {
        MetadataExtractor extractor = new MetadataExtractor (GetMetadataConverter (arguments));

        extractor.AddAssembly (arguments.DomainAssemblyName);
        extractor.Save (arguments.MetadataOutputFile);
      }
      catch (Exception e)
      {
        HandleException (e, arguments.Verbose);
        return 1;
      }

      return 0;
    }

    private static void HandleException (Exception exception, bool verbose)
    {
      if (verbose)
      {
        Console.Error.WriteLine ("Execution aborted. Exception stack:");

        for (; exception != null; exception = exception.InnerException)
        {
          Console.Error.WriteLine ("{0}: {1}\n{2}", exception.GetType ().FullName, exception.Message, exception.StackTrace);
        }
      }
      else
      {
        Console.Error.WriteLine ("Execution aborted: {0}", exception.Message);
      }
    }

    private static IMetadataConverter GetMetadataConverter (CommandLineArguments arguments)
    {
      MetadataConverterBuilder converterBuilder = new MetadataConverterBuilder ();

      if (arguments.Languages.Length > 0)
      {
        string[] languages = arguments.Languages.Split (',');
        foreach (string language in languages)
          converterBuilder.AddLocalization (language);
      }

      if (arguments.InvariantCulture)
        converterBuilder.AddLocalization (CultureInfo.InvariantCulture);

      converterBuilder.ConvertMetadataToXml = !arguments.SuppressMetadata;
      return converterBuilder.Create ();
    }
  }
}
