using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TechAssesNovibet.ServiceLayer;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TechAssesNovibet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchController : ControllerBase
    {
        private IServiceModel _service;
        public BatchController(IServiceModel service)
        {
            _service = service;
        }

        // GET api/<BatchController>/5
        [HttpGet("{id}")]
        public string Get(Guid id)
        {
            var returnId = _service.GetBatchProcess(id);
            return returnId;
        }

        // POST api/<BatchController>
        [HttpPost]
        public string Post([FromBody] List<string> ipAddresses)
        {
           return _service.RegisterBatch(ipAddresses).ToString();
        }
    }
}
