using System;
using System.IO;
using System.Threading;

namespace Remotion.Utilities
{
  // TODO: comment http://at-vie-svn.int.rubicon-it.com/wiki/pmwiki.php/KnowledgeBase/FileDeleteKannAsynchronSein
  public static class FileUtility
  {
    public static void DeleteOnDemandAndWaitForCompletion (string fileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("fileName", fileName);

      if (File.Exists (fileName))
        DeleteAndWaitForCompletion (fileName);
    }

    public static void DeleteAndWaitForCompletion (string fileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("fileName", fileName);

      File.Delete (fileName);
      while (File.Exists (fileName))
        Thread.Sleep (10);
    }

    public static void MoveAndWaitForCompletion (string sourceFileName, string destinationFileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sourceFileName", sourceFileName);
      ArgumentUtility.CheckNotNullOrEmpty ("destinationFileName", destinationFileName);

      File.Move (sourceFileName, destinationFileName);

      if (Path.GetFullPath (sourceFileName) == Path.GetFullPath (destinationFileName))
        return;

      while (File.Exists (sourceFileName) || !File.Exists (destinationFileName))
        Thread.Sleep (10);
    }
  }
}
