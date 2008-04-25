using System;
using Remotion.Mixins;
using Remotion.Mixins.Samples.PhotoStuff;
using Remotion.Mixins.Samples.PhotoStuff.Variant2;
using NUnit.Framework;
using System.Reflection;

namespace Remotion.Mixins.Samples.UnitTests.PhotoStuff
{
  [TestFixture]
  public class Variant2
  {
    [Test]
    public void StoredMembers()
    {
      Photo photo = ObjectFactory.Create<Photo>().With();
      Assert.IsNotNull (photo.Document);
      PropertyInfo[] properties = Array.FindAll (photo.GetType ().GetProperties (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
          delegate (PropertyInfo pi)
          {
            return pi.IsDefined (typeof (StoredAttribute), false);
          });

      Assert.AreEqual (1, properties.Length);
    }

    [Test]
    public void InitializeWithConcreteDocument()
    {
      Document doc = new Document();
      doc.CreatedAt = new DateTime (2006, 01, 01);
      Photo photo = ObjectFactory.Create<Photo>().With ();
      Mixin.Get<DocumentMixin> (photo).Document = doc;
      Assert.IsNotNull (photo.Document);
      Assert.AreEqual (new DateTime (2006, 01, 01), photo.Document.CreatedAt);
    }
  }
}
