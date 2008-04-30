using System.Configuration;
using Remotion.Development.UnitTesting.IO;

namespace Remotion.Data.DomainObjects.UnitTests.Factories
{
  public class ConfigurationFactory
  {
    public static System.Configuration.Configuration LoadConfigurationFromFile (TempFile tempFile, byte[] content)
    {
      tempFile.WriteAllBytes (content);

      ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
      fileMap.ExeConfigFilename = tempFile.FileName;
      return ConfigurationManager.OpenMappedExeConfiguration (fileMap, ConfigurationUserLevel.None);
    }
  }
}