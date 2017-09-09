using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BBDom.Test.DotNet
{
    [TestClass]
    public class KnxTest
    {
        [TestMethod]
        public void TestConnection()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
            }
            catch (Exception) { }

            var knx = new BBDom.Biz.DotNet.Knx();
            
            knx.Test();
        }
    }
}
