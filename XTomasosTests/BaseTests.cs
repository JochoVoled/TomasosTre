using System;
using Xunit;

namespace XTomasosTests
{
    public class BaseTests
    {
        public BaseTests()
        {

            InitDb();
        }

        public virtual void InitDb()
        {

        }

        //[Fact]
        //public void Base_All_Are_Sorted()
        //{
        //    Assert.Equal(2, 0);
        //}
    }
}
