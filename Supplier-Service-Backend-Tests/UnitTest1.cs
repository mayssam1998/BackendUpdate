using System.Collections.Generic;
using NUnit.Framework;
using SuS.Domain.Entities.SuSModels;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Supplier sup = new Supplier();
            sup.Name = "New Sup";
            sup.Type=new Type();
            sup.Branch=new List<Branch>();
            sup.Number = "1001";
            
            


          //  Assert.AreEqual(100, _accountBalanceService.GetAccountBalanceByUser(user));
            Assert.Pass();
        }
    }
}