using System;

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// Specifies if a property or field is read only.
  /// </summary>
  [AttributeUsage (AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class ObjectBindingAttribute : Attribute
  {
    private bool _readOnly = false;
    private bool _visible = true;
    public ObjectBindingAttribute ()
    {
    }

    public bool ReadOnly
    {
      get { return _readOnly; }
      set { _readOnly = value; }
    }

    public bool Visible
    {
      get { return _visible; }
      set { _visible = value; }
    }
  }
}