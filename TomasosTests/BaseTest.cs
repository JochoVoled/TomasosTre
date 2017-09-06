using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TomasosTests
{
    [TestClass]
    public class BaseTest
    {
        public BaseTest()
        {
            
            InitDb();
        }

        public virtual void InitDb()
        {
            
        }
    }
}
