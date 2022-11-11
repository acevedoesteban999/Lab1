using Microsoft.AspNetCore.Mvc;
using ClassLibrary;
using Repository;
using System.Net;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FormsController : ControllerBase
    {
        private readonly ILogger<FormsController> _logger;

        private readonly IFormRepository _FormRepository;

        public FormsController(ILogger<FormsController> logger, IFormRepository FormRepository)
        {
            _logger = logger;
            _FormRepository = FormRepository;
        }

        [HttpPost("PostForm/{name},{formType},{coordX},{coordY},{coordZ}", Name = "PostForm")]
        public ActionResult<Form> PostForm(string name, int formType, int coordX, int coordY, int coordZ)
        {
            _FormRepository.BeginTransaction();
            var form = _FormRepository.CreateForm(name, Enum.GetValues<FormsType>()[formType], new Coordinate(coordX, coordY, coordZ));
            _FormRepository.CommitTransaction();
            if (form == null)
            {
                _logger.LogError($"{nameof(FormsController.PostForm)} -> cannot create Form");
                return NotFound();
            }
            return form;
        }

        [HttpGet("GetForms", Name = "GetForms")]
        public ActionResult<IEnumerable<Form>> GetForms()
        {
            _FormRepository.BeginTransaction();
            var form = _FormRepository.GetForms();
            _FormRepository.CommitTransaction();
            if (form == null)
            {
                _logger.LogError($"{nameof(FormsController.GetForm)} -> Form not found");
                return NotFound();
            }
            return form;
        }

        [HttpGet("GetForm/{id}", Name = "GetForm")]
        public ActionResult<Form> GetForm(int id)
        {
            _FormRepository.BeginTransaction();
            var form = _FormRepository.GetForm(id);
            _FormRepository.CommitTransaction();
            if (form == null)
            {
                _logger.LogError($"{nameof(FormsController.GetForm)} -> Form not found");
                return NotFound();
            }
            return form;
        }
    }
}