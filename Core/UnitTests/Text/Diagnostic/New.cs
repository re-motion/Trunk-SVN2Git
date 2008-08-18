using System.Collections.Generic;

namespace Remotion.UnitTests.Text.Diagnostic
{
  /// <summary>
  /// Supplies factories to easily create instances of complex objects such as containers.
  /// </summary>
  /// <example><code>
  /// <![CDATA[  
  /// var listList = New.List( New.List(1,2), New.List(3,4) );
  /// var queue = New.Queue("process","emit0","wait");
  /// var d = New.Dictionary("A",1, "B",2, "C",3); // d["A"]=1, d["B"]=2,...
  /// ]]>
  /// </code></example>
  public partial class New
  {
    public static List<T> List<T> (params T[] values)
    {
      var container = new List<T>(values);
      return container;
    }

    
    
    public static Queue<T> Queue<T> (params T[] values)
    {
      var container = new Queue<T> (values);
      return container;
    }



    public static KeyValuePair<TKey, TValue> KeyValue<TKey, TValue> (TKey key, TValue value)
    {
      return new KeyValuePair<TKey, TValue> (key, value);
    }

    public static KeyValuePair<TKey, TValue> KV<TKey, TValue> (TKey key, TValue value)
    {
      return KeyValue<TKey, TValue> (key, value);
    }

    public static Dictionary<TKey, TValue> Dictionary<TKey, TValue> (params KeyValuePair<TKey, TValue>[] keyValuePairs)
    {
      var container = new Dictionary<TKey, TValue> (keyValuePairs.Length);
      foreach (var pair in keyValuePairs)
      {
        container.Add (pair.Key,pair.Value);
      }

      return container;
    }

    public static Dictionary<TKey, TValue> Dictionary<TKey, TValue> (TKey key0, TValue value0)
    {
      var container = new Dictionary<TKey, TValue> (1);
      container[key0] = value0;
      return container;
    }

    public static Dictionary<TKey, TValue> Dictionary<TKey, TValue> (TKey key0, TValue value0, TKey key1, TValue value1)
    {
      var container = new Dictionary<TKey, TValue> (2);
      container[key0] = value0;
      container[key1] = value1;
      return container;
    }

    public static Dictionary<TKey, TValue> Dictionary<TKey, TValue> (TKey key0, TValue value0, TKey key1, TValue value1, TKey key2, TValue value2)
    {
      var container = new Dictionary<TKey, TValue> (3);
      container[key0] = value0;
      container[key1] = value1;
      container[key2] = value2;
      return container;
    }

    public static Dictionary<TKey, TValue> Dictionary<TKey, TValue> (TKey key0, TValue value0, TKey key1, TValue value1, TKey key2, TValue value2, TKey key3, TValue value3)
    {
      var container = new Dictionary<TKey, TValue> (4);
      container[key0] = value0;
      container[key1] = value1;
      container[key2] = value2;
      container[key3] = value3;
      return container;
    }
  
  }
}