using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Doctors
{
    public static class JsonH
    {
        // ---------- Synchronous Methods  :
        public static JsonResult Success()
        {
            return new JsonResult(new { status = "Succeed." });
        }


        public static JsonResult Success(object returnData)
        {
            return new JsonResult(new { status = "Succeed.", data = returnData });
        }

        public static JsonResult NotFound()
        {
            return new JsonResult(new { status = "Not Found." });
        }

        public static JsonResult NotFound(object returnData)
        {
            return new JsonResult(new { status = "Not Found.", data = returnData });
        }

        public static JsonResult Error()
        {
            return new JsonResult(new { status = "Error." });
        }

        public static JsonResult Error(object returnData)
        {
            return new JsonResult(new { status = "Error.", data = returnData });
        }


        // ---------- Asynchronous Methods  :

        public static async Task<JsonResult> SuccessAsync()
        {
            return new JsonResult(new { status = "Succeed." });
        }

        public static async Task<JsonResult> SuccessAsync(object returnData)
        {
            return new JsonResult(new { status = "Succeed.", data = returnData });
        }

        public static async Task<JsonResult> NotFoundAsync()
        {
            return new JsonResult(new { status = "Not Found." });
        }

        public static async Task<JsonResult> NotFoundAsync(object returnData)
        {
            return new JsonResult(new { status = "Not Found.", data = returnData });
        }

        public static async Task<JsonResult> ErrorAsync()
        {
            return new JsonResult(new { status = "Error." });
        }

        public static async Task<JsonResult> ErrorAsync(object returnData)
        {
            return new JsonResult(new { status = "Error.", data = returnData });
        }
    }
}
