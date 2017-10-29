using Microsoft.VisualStudio.TestTools.UnitTesting;
using DMessenger;
using System.Security.Cryptography;
using System.IO;
using System.Xml.Linq;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var rsa = RSA.Create())
            {
                var thr = new MessageThread();

                var msg = new Message()
                {
                    Value = new XElement("value", new XText("Hello JPHACKS"))
                };
                thr.AddOrUpdate(msg);
                var xml = msg.ToXml();
                var bin = MessageEncoder.Encode(xml, rsa);
                xml = MessageEncoder.Decode(new MemoryStream(bin));
                var msg2 = new Message(xml);
            }
        }
    }
}
