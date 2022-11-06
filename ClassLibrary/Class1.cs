using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace ClassLibrary
{

    public enum FormsType
    {
        BUTTON,RADIOBUTTON,CHECTBOX,RADIOBUTTONGROUP,BOX,FREEBOX,LABEL
    }
    public struct Coordinate
    {
        public Coordinate(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public int x;
        public int y;
        public int z;
        public static Coordinate operator +(Coordinate coord1, Coordinate coord2)
        {
            return new Coordinate(coord1.x + coord2.x, coord1.y + coord2.y, coord1.z + coord2.z);
        }
        public void Add(int x, int y, int z)
        {
            this.x += x;
            this.y += y;
            this.z += z;
        }
    }

    public abstract class  Form
    {
        
        private static int _cont;
        public string _name;
        protected int ID { get; set; }
        public Coordinate _coordinate; 
        protected int _wigth;
        protected int _height;
        protected FormsType _formsType;
        public Form(string name,Coordinate crd=new Coordinate(),int wigth=0,int height=0)
        {
            this._name = name;
            this._coordinate = crd;
            this._wigth = wigth;
            this._height = height;
            this.ID = _cont++;
        }
        ~Form()
        {
            _cont--;
        }
        
        public virtual void Add(Form form){}
        public virtual void Add(string ElementName,Form form) { }
        public  virtual void Draw()
        {
            Console.WriteLine("ID:"+ID+"->"+_name +"->("+ _coordinate.x.ToString() + ";" + _coordinate.y.ToString() + ";" + _coordinate.z.ToString()+")");
        }
        /// <summary>
        /// Poner  una  nueva coordenada y sumarle los valores de entrada X,Y,Z
        /// </summary>
        /// <param name="crd"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        public virtual void Set_Coordinate(Coordinate crd,int  X=0,int Y=0,int Z=0)
        {
            crd.Add(X,Y,Z);
            this._coordinate = crd;
        }
        public abstract void OnClick();
        public virtual bool Pressed(int x, int y)
        {
            return (_coordinate.x <= x && _coordinate.x + _wigth >= x && _coordinate.y <= y && _coordinate.y + _wigth >= y);
        }
    }


    public class  Button:Form
    {
        public Button(string name, Coordinate crd=new Coordinate(),int wigth=0,int height=0)
            : base(name,crd,wigth,height)
        {
            this._formsType = FormsType.BUTTON;
        }
        public override void OnClick()
        {

        }
        
    }

    abstract public class  SelectionForm : Form 
    {
        private bool Check{get;set;}

        public SelectionForm(string name, Coordinate crd = new Coordinate(), int wigth = 0, int height = 0)
            : base(name,crd,wigth,height)
        { 
        }
        public abstract override void OnClick();
         public override void Draw()
        {
 	        base.Draw();
            Console.WriteLine("Check"+Check.ToString());
            
        }
    }
    public class  RadioButton:SelectionForm
    {
       
        public RadioButton(string name, Coordinate crd = new Coordinate())
            : base(name,crd,15,15)
        {
            this._formsType = FormsType.RADIOBUTTON;
        }
        public override void OnClick()
        {

        }
    }
    public class  ChectBox : SelectionForm
    {
        public ChectBox(string name, Coordinate crd = new Coordinate())
            : base(name, crd,15,15)
        {
            this._formsType = FormsType.CHECTBOX;
        }
        public override void OnClick()
        {

        }
    }
    
    public class  RadioButtonGroup:Form
    {
        private List<RadioButton> radioButtons;

        public RadioButtonGroup(string name, Coordinate crd = new Coordinate())
            : base(name,crd)
        {
            radioButtons = new List<RadioButton>();
            this._formsType = FormsType.RADIOBUTTONGROUP;
        }
        public override void Draw()
        {
            base.Draw();
            int cont = 0;
            Console.WriteLine("Elements:" + radioButtons.Count);
            foreach (RadioButton RB in radioButtons)
            {
                Console.WriteLine("[RadioButton" + (cont++) + "]");
                RB.Draw();
            }
        }
        public override void OnClick()
        {

        }
        public override void Add(Form form)
        {
            form.Set_Coordinate(radioButtons.Count!=0?radioButtons.Last()._coordinate:this._coordinate, 0, 5);
           radioButtons.Add((RadioButton)form);
        }
        public override bool Pressed(int x, int y)
        {
            foreach (RadioButton rb in radioButtons)
                if (rb.Pressed(x,y))
                    return true;
            return false;
        }
    }
    public class  Label:Form
    {
        private string TEXT{set;get;}
         public Label(string name,string text="NullText", Coordinate crd = new Coordinate())
            : base(name,crd,0,15)
        { 
             this.TEXT=text;
             this._wigth = text.Count() * 10;
            this._formsType = FormsType.LABEL;
        }
         public override void OnClick()
         {

         }
         public override void Draw()
         {
             base.Draw();
             Console.WriteLine("Text:" + TEXT);
         }
    }

    abstract public class  Container:Form
    {
        protected List<Form> forms;
        public Container(string name, Coordinate crd = new Coordinate())
            : base(name,crd)
        { 
            forms= new List<Form>();
        }
        public override void Add(Form form)
        {
            forms.Add(form);
        }
        public override void Add(string ElementName, Form form)
        {
            foreach (Form _form in forms)
            {
                if (string.Equals(ElementName, _form._name))
                {
                    _form.Add(form);
                    return;
                }
            }
        }
        public override bool Pressed(int x, int y)
        {
            foreach (Form form in forms)
                if (form.Pressed(x, y))
                    return true;
            return false;
        }
        public abstract override void OnClick();
        public override void Draw()
        {
            base.Draw();
            int cont = 0;
            Console.WriteLine("Elements:"+forms.Count);
            foreach (Form form in forms)
            {
                Console.WriteLine("(Element" + (cont++)+")");
                form.Draw();
            }
        }
    }
    public class  Box : Container
    {
        public Box(string name, Coordinate crd=new Coordinate())
            : base(name,crd)
        {
            this._formsType = FormsType.BOX;
        }
        public override void Add(Form form)
        {
            form.Set_Coordinate((forms.Count!=0?forms.Last()._coordinate:this._coordinate), 0, 5);
            forms.Add(form);
        }
        public override void OnClick()
        {
        }
    }
    public class  FreeBox : Container
    {
        public FreeBox(string name, Coordinate crd = new Coordinate())
           : base(name,crd)
        {
            this._formsType = FormsType.FREEBOX;
        }
        public override void OnClick()
        {
        }
    }
    public class  StackForm
    {
        private List<Form> forms;
        public StackForm()
        {
            forms = new List<Form>();
        }
        
        public void Log_Pressed(int x, int y)
        {
            foreach (Form f in forms)
            {
                if (f.Pressed(x,y))
                {
                    Console.WriteLine("Form Presioando:");
                    f.Draw();
                    return;
                }
            }
            Console.WriteLine("No Form Presionado");
        }
        public int Get_Count()
        {
            return forms.Count;
        }
        public void Add(Form form)
        {
            forms.Add(form);
        }
        public void Sub(string name)
        {
            foreach (Form f in forms)
                if (f._name.Equals(name))
                {
                    forms.Remove(f);
                    return;
                }
        }
        public void Draw()
        {
            foreach (Form f in forms)
                f.Draw();
        }
    }
}