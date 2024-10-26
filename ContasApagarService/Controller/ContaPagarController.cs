using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContasApagarService.Controller
{
    [Route("[controller]")]
    public class ContaPagarControlller : ControllerBase
    {
       [HttpGet]
       public IActionResult Get()
       {
          return Ok("Serviço de Contas a Pagar funcionando");
       }
    }
}