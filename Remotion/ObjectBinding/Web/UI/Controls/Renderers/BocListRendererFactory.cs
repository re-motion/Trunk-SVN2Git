using System;
using System.Reflection;
using System.Web.UI;

namespace Remotion.ObjectBinding.Web.UI.Controls.Renderers
{
  public class BocListRendererFactory
  {
    private readonly BocList _list;
    private readonly HtmlTextWriter _writer;

    public BocListRendererFactory (BocList list, HtmlTextWriter writer)
    {
      _list = list;
      _writer = writer;
    }

    public BocListRenderer GetRenderer ()
    {
      return new BocListRenderer (List, Writer, this);
    }

    public BocListMenuBlockRenderer GetMenuBlockRenderer ()
    {
      return new BocListMenuBlockRenderer (List, Writer);
    }

    public BocListNavigatorRenderer GetNavigatorRenderer ()
    {
      return new BocListNavigatorRenderer (List, Writer);
    }

    public BocRowRenderer GetRowRenderer ()
    {
      return new BocRowRenderer (List, Writer, this);
    }

    public IBocColumnRenderer GetColumnRenderer (BocColumnDefinition column)
    {
      foreach (Type type in Assembly.GetExecutingAssembly ().GetTypes ())
      {
        if (type.IsAbstract)
          continue;

        if (!type.BaseType.IsGenericType)
          continue;

        if (type.BaseType.GetGenericArguments ().Length != 1)
          continue;

        Type genericArgument = type.BaseType.GetGenericArguments ()[0];
        if (genericArgument.IsAssignableFrom (column.GetType ()))
        {
          ConstructorInfo constuctor = type.GetConstructor (new[] { typeof (BocList), typeof (HtmlTextWriter), genericArgument });
          return (IBocColumnRenderer) constuctor.Invoke (new object[] { List, Writer, column });
        }
      }
      throw new NotImplementedException (String.Format ("No renderer for column type '{0}' found.", column.GetType ().Name));
    }

    private BocList List
    {
      get { return _list; }
    }

    private HtmlTextWriter Writer
    {
      get { return _writer; }
    }

  }
}
