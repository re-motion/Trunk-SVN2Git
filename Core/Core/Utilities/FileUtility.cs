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
using System.IO;
using System.Reflection;
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
    


    /// <summary>
    /// Copies the complete content of one stream into another.
    /// </summary>
    /// <param name="input">The input stream.</param>
    /// <param name="output">The output stream.</param>
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


    /// <summary>
    /// Writes a string resource embedded in an assemby into a file with the passed filename.
    /// </summary>
    /// <param name="typeWhoseNamespaceTheStringResourceResidesIn"><see cref="Type"/> in whose assembly and namespace the string resource is located.</param>
    /// <param name="stringResourceName">Name of the string resource, relative to namespace of the passed <see cref="Type"/>.</param>
    /// <param name="filePath">The path of the file the string resource will be written into.</param>
    public static void WriteEmbeddedStringResourceToFile (Type typeWhoseNamespaceTheStringResourceResidesIn, string stringResourceName, string filePath)
    {
      ArgumentUtility.CheckNotNull ("typeWhoseNamespaceTheStringResourceResidesIn", typeWhoseNamespaceTheStringResourceResidesIn);
      ArgumentUtility.CheckNotNull ("stringResourceName", stringResourceName);
      ArgumentUtility.CheckNotNull ("filePath", filePath);
      Assembly assembly = typeWhoseNamespaceTheStringResourceResidesIn.Assembly;
      using (
        Stream from = assembly.GetManifestResourceStream (typeWhoseNamespaceTheStringResourceResidesIn, stringResourceName),
        to = new FileStream (filePath, FileMode.Create))
      {
        FileUtility.CopyStream (from, to);
      }
    }

  }
}
