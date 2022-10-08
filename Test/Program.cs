using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary;

namespace Test
{
    [TestClass]
    class StackFormTest
    {
        private StackForm stackform;
        public StackFormTest()
        {
            stackform=new StackForm();
        }
        [TestMethod]
        public void Add_Element()
        {
            Form form=new Label("Label0","TextLabel",new Coordinate(50,50,0));
            int Count=stackform.Get_Count();
            stackform.Add_Element(form);
            Assert.AreEqual(Count + 1, stackform.Get_Count());
        }
    } 
}

