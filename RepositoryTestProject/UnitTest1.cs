using Repository;
using ClassLibrary;
namespace RepositoryTestProject
{
    [TestClass]
    public class FormRepositoryTests
    {
        IFormRepository _repository;

        public FormRepositoryTests()
        {
            var connectionString = @"Data Source=ESTEBAN-PC\SQLEXPRESS;AttachDBFilename=C:\Users\Esteban\Desktop\Esteban\Programacion\C#\BDForm.mdf;Initial Catalog=BDForm;User ID=sa;Password=qwerty";
            _repository = new DBRepository(new ConnectionString(connectionString));
        }

        [TestMethod]
        public void Can_CreateForm_Test()
        {
            // arrange
            string name = "Form1";
            FormsType formtype = FormsType.BUTTON;
            Coordinate coord=new Coordinate();
            // act
            _repository.BeginTransaction();
            Form form = _repository.CreateForm(name, formtype, coord);
            _repository.CommitTransaction();

            // assert
            Assert.IsNotNull(form);
            Assert.AreNotEqual(0, form.ID);
            Assert.AreEqual(name, form._name);
            Assert.AreEqual(formtype, FormsType.BUTTON);
        }

        [TestMethod]
        public void Can_UpdateForm_Test()
        {
            // arrange
            _repository.BeginTransaction();
            var form = _repository.FindForm("Form1");
            _repository.CommitTransaction();

            var newType = FormsType.BOX;
            form._formsType = newType;

            // act
            _repository.BeginTransaction();
            _repository.UpdateForm(form);
            _repository.PartialCommit();

            // assert
            var newForm = _repository.GetForm(form.ID);
            _repository.CommitTransaction();
            Assert.IsNotNull(newForm);
            Assert.AreNotEqual(0, newForm.ID);
            Assert.AreEqual(newType, form._formsType);
        }

        [TestMethod]
        public void Can_DeleteForm_Test()
        {
            // arrange
            _repository.BeginTransaction();
            var form = _repository.FindForm("Juan");
            _repository.CommitTransaction();

            // act
            _repository.BeginTransaction();
            _repository.DeleteForm(form);
            _repository.PartialCommit();

            // assert
            var deletedForm = _repository.GetForm(form.ID);
            _repository.CommitTransaction();
            Assert.IsNull(deletedForm);
        }
    }
}