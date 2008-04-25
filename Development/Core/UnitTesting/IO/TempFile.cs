using System;
using System.IO;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.IO
{
  /// <summary>
  /// The <see cref="TempFile"/> class represents a disposable temp file created via the <see cref="Path.GetTempFileName"/> method.
  /// </summary>
  public class TempFile : DisposableBase
  {
    private string _fileName;

    public TempFile ()
    {
      _fileName = Path.GetTempFileName ();
    }

    protected override void Dispose (bool disposing)
    {
      if (_fileName != null && File.Exists (_fileName))
      {
        File.Delete (_fileName);
        _fileName = null;
      }
    }

    public string FileName
    {
      get
      {
        //TODO: Use AssertNotDisposed once new DisposableBase is commited
        if (Disposed)
          throw new InvalidOperationException ("Object disposed.");
        return _fileName;
      }
    }

    public void WriteStream (Stream stream)
    {
      ArgumentUtility.CheckNotNull ("stream", stream);

      using (StreamReader streamReader = new StreamReader (stream))
      {
        using (StreamWriter streamWriter = new StreamWriter (_fileName))
        {
          while (!streamReader.EndOfStream)
          {
            char[] buffer = new char[100];
            streamWriter.Write (buffer, 0, streamReader.Read (buffer, 0, buffer.Length));
          }
        }
      }
    }

    public void WriteAllBytes (byte[] bytes)
    {
      ArgumentUtility.CheckNotNull ("bytes", bytes);

      File.WriteAllBytes (_fileName, bytes);
    }
  }
}