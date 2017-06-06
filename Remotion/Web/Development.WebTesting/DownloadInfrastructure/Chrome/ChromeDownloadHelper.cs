﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using log4net;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.DownloadInfrastructure.Chrome
{
  /// <summary>
  /// Responsible for handling a download with Chrome.
  /// </summary>
  public class ChromeDownloadHelper : DownloadHelperBase
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (ChromeDownloadHelper));

    private const string c_partialFileEnding = ".crdownload";
    private readonly string _downloadDirectory;
    private readonly TimeSpan _downloadStartedGracePeriod;
    private readonly bool _cleanUpDownloadFolderOnError;

    /// <summary>
    /// Creates a new <see cref="ChromeDownloadHelper"/>.
    /// </summary>
    /// <param name="downloadDirectory">
    /// Directory where the browser saves the downloaded files. Must not be <see langword="null" /> or empty.
    /// </param>
    /// <param name="downloadStartedTimeout">
    /// Specifies how long the <see cref="DownloadHelperBase"/> should wait before looking for the downloaded file.
    /// </param>
    /// <param name="downloadUpdatedTimeout">
    /// Specifies how long the <see cref="DownloadHelperBase"/> should wait for a downloaded partial file to update.
    /// </param>
    /// <param name="downloadStartedGracePeriod">
    /// Specifies the max time between triggering the download and calling <see cref="DownloadedFileFinder"/>.<see cref="DownloadedFileFinder.WaitForDownloadCompleted"/>. 
    /// All files created before this time span will be ignored.
    /// </param>
    /// <param name="cleanUpDownloadFolderOnError">
    /// Clean up the download folder on error.
    /// </param>
    public ChromeDownloadHelper (
        [NotNull] string downloadDirectory,
        TimeSpan downloadStartedTimeout,
        TimeSpan downloadUpdatedTimeout,
        TimeSpan downloadStartedGracePeriod,
        bool cleanUpDownloadFolderOnError)
        : base (downloadStartedTimeout, downloadUpdatedTimeout)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("downloadDirectory", downloadDirectory);

      _downloadDirectory = downloadDirectory;
      _downloadStartedGracePeriod = downloadStartedGracePeriod;
      _cleanUpDownloadFolderOnError = cleanUpDownloadFolderOnError;
    }

    public string DownloadDirectory
    {
      get { return _downloadDirectory; }
    }

    public TimeSpan DownloadStartedGracePeriod
    {
      get { return _downloadStartedGracePeriod; }
    }

    public bool CleanUpDownloadFolderOnError
    {
      get { return _cleanUpDownloadFolderOnError; }
    }

    protected override IDownloadedFile HandleDownload (
        DownloadedFileFinder downloadedFileFinder,
        TimeSpan downloadStartedTimeout,
        TimeSpan downloadUpdatedTimeout)
    {
      ArgumentUtility.CheckNotNull ("downloadedFileFinder", downloadedFileFinder);

      EnsureDownloadDirectoryExists (_downloadDirectory);

      //Empty list, as our infrastructure should keep the download directory clean by moving downloaded files away, so we can assume the download directory is empty.
      //We need this assumption as Chrome downloads files without prompt, making it impossible to get the directory state before the download starts. 
      var filesInDownloadDirectoryBeforeDownload = new List<string>();

      DownloadedFile downloadedFile = null;

      try
      {
        downloadedFile = downloadedFileFinder.WaitForDownloadCompleted (
            downloadStartedTimeout,
            downloadUpdatedTimeout,
            filesInDownloadDirectoryBeforeDownload);
      }
      catch (DownloadResultNotFoundException ex)
      {
        if (_cleanUpDownloadFolderOnError)
          CleanUpUnmatchedDownloadedFiles (ex.GetUnmatchedFilesInDownloadDirectory().ToList());

        throw;
      }

      return MoveDownloadedFile (downloadedFile);
    }

    protected override DownloadedFileFinder CreateDownloadedFileFinderForExpectedFileName (string fileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("fileName", fileName);

      return new DownloadedFileFinder (
          _downloadDirectory,
          c_partialFileEnding,
          _downloadStartedGracePeriod,
          new ChromeNamedExpectedFileNameFinderStrategy (fileName));
    }

    protected override DownloadedFileFinder CreateDownloadedFileFinderForUnknownFileName ()
    {
      return new DownloadedFileFinder (
          _downloadDirectory,
          c_partialFileEnding,
          _downloadStartedGracePeriod,
          new ChromeUnknownFileNameFinderStrategy (c_partialFileEnding));
    }

    protected override void BrowserSpecificCleanup ()
    {
      if (Directory.Exists (_downloadDirectory))
      {
        try
        {
          Directory.Delete (_downloadDirectory, true);
        }
        catch (IOException ex)
        {
          s_log.WarnFormat (
              @"Could not delete '{0}'.
{1}",
              _downloadDirectory,
              ex);
        }
      }
    }


    private void EnsureDownloadDirectoryExists (string downloadDirectory)
    {
      if (!Directory.Exists (downloadDirectory))
        Directory.CreateDirectory (downloadDirectory);
    }

    private void CleanUpUnmatchedDownloadedFiles ([NotNull] IEnumerable<string> unmatchedFiles)
    {
      ArgumentUtility.CheckNotNull ("unmatchedFiles", unmatchedFiles);

      foreach (var file in unmatchedFiles)
      {
        var fullFilePath = Path.Combine (_downloadDirectory, file);

        try
        {
          //We don't wait for the file to be deleted, as we expect it to be deleted in time
          File.Delete (fullFilePath);
        }
        catch (IOException ex)
        {
          s_log.WarnFormat (
              @"Could not delete '{0}'.
{1}",
              fullFilePath,
              ex);
        }
      }
    }
  }
}