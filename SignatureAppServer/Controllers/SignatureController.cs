// Controllers/SignatureController.cs
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace SignatureAppServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignatureController : ControllerBase
    {
        private static SignatureData _signatureData = new SignatureData();

        [HttpPost]
        public IActionResult Post([FromForm] SignatureData data)
        {
            _signatureData = data;
            return Ok("Data received and stored.");
        }

        [HttpGet]
        public ActionResult<SignatureData> Get()
        {
            return _signatureData;
        }

        [HttpPut]
        public IActionResult Put([FromForm] string signature)
        {
            _signatureData.Signature = signature;
            return Ok("Signature updated.");
        }
    }

}
