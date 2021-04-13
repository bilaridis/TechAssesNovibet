using DynamicLinkLibrary;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechAssesNovibet.ServiceLayer;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TechAssesNovibet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IPDetailsController : ControllerBase
    {
        private IServiceModel _service;
        public IPDetailsController(IServiceModel service)
        {
            _service = service;
        }

        // GET api/<IPDetailsController>/5
        [HttpGet("{id}")]
        public string Get(string id)
        {
            try
            {
                return _service.GetByIP(id).ToJson();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
