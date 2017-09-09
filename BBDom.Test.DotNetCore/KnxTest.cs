using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BBDom.Test.DotNetCore
{
    [TestClass]
    public class KnxTest
    {
        [TestMethod]
        public void KnxTestTestMethod1()
        {
            var knx = new Biz.DotNetStandard.Knx();
            knx.Connect();
        }
    }
}
