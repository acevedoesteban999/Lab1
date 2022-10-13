using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary;

namespace ConsoleApp
{
    class Program
    {
        public static void InterfazConsola(StackForm stackform)
        {
            string dataX,dataY;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Elementos Existentes:");
                stackform.Draw();
                Console.WriteLine("\n\nVerificar una Coordenada:");
                Console.WriteLine("Escribir \"Exit\" para salir");
                Console.WriteLine("Coordenada X:");
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                dataX = Console.ReadLine();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (dataX!=null && dataX.Equals("Exit"))
                    return;
                
                Console.WriteLine("Coordenada Y:");
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                dataY = Console.ReadLine();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (dataY!=null && dataY.Equals("Exit"))
                    return;
                try
                {
                    int datoX=Convert.ToInt32(dataX);
                    int datoY= Convert.ToInt32(dataY);
                    stackform.Log_Pressed(datoX, datoY);
                    Console.ReadLine();
                }
                catch(Exception e)
                {
                    Console.WriteLine(dataX+","+dataY+ " --> Comando no reconocido,pulse Enter para continuar...ErrorMessage:"+e.Message);
                    Console.ReadLine();
                    continue;
                }
            }
        }
        static void Main(string[] args)
        {
            StackForm stackform=new StackForm();
            var box = new Box("Box0");
            var freeBox= new FreeBox("FreeBox0",new Coordinate(100,100,0));
            freeBox.Add(new Button("Button0"));
            freeBox.Add(new RadioButton("RadioButton0"));
            box.Add(new RadioButtonGroup("RadioButtonGroup0"));
            box.Add("RadioButtonGroup0", new RadioButton("RadioButton0"));
            box.Add("RadioButtonGroup0", new RadioButton("RadioButton1"));
            box.Add(new ChectBox("ChectBox0"));
            box.Add(new Label("Label0","TextLabel0"));
            stackform.Add(box);
            stackform.Add(freeBox);
            stackform.Add(new Label("label1","TextLabel1",new Coordinate(100,0,0)));
            stackform.Add(new ChectBox("ChectBox1",new Coordinate(150, 0, 0)));
            InterfazConsola(stackform);
            

        }
    }
}
