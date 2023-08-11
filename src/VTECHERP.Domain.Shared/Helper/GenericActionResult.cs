using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VTECHERP.Helper
{
    public class GenericActionResult : ActionResult, IActionResult
    {
        public int HttpStatusCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public object ObjCount { get; set; }

        public GenericActionResult()
        {
            Success = true;
            HttpStatusCode = 200;
        }

        public GenericActionResult(int status, bool success, string message = "", object data = null)
        {
            HttpStatusCode = status;
            Success = success;
            Message = message;
            Data = data;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            ObjectResult objectResult;

            if (HttpStatusCode == 200)
                objectResult = new OkObjectResult(this);
            else if (HttpStatusCode == 404)
                objectResult = new NotFoundObjectResult(this);
            else if (HttpStatusCode == 400)
                objectResult = new BadRequestObjectResult(this);
            else
                objectResult = new ObjectResult(this);

            await objectResult.ExecuteResultAsync(context);
        }
  
        public GenericActionResult(object objCount, object data)
        {
            Success = true;
            HttpStatusCode = 200;
            ObjCount = objCount;
            Data = data;
        }
    }
}
