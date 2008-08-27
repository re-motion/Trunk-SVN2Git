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

    public const int CopyBufferSize = 1024 * 64;
    
    public static void CopyStream (Stream input, Stream output)
    {
      ArgumentUtility.CheckNotNull ("input", input);
      ArgumentUtility.CheckNotNull ("output", output);

      byte[] buffer = new byte[CopyBufferSize];
      int bytesRead;
      do
      {
        bytesRead = input.Read (buffer, 0, buffer.Length);
        output.Write (buffer, 0, bytesRead);
      } while (bytesRead != 0);
    }
  }
}
