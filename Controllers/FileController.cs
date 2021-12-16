using Doctors.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Doctors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {

        #region CTOR
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public FileController(IConfiguration configuration,
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager
            )
        {
            _configuration = configuration;
            _context = context;
            _userManager = userManager;
        }
        #endregion



        [HttpPost("upload-company-image")]
        public async Task<IActionResult> UploadCompanyImage(IFormFile file,
            [FromForm] string altText, [FromForm] string type, [FromForm] string companyId)
        {
            var fileName = Path.GetFileName(file.FileName);

            string folder = "companyProfile";
            if (type == "logo")
                folder = "companyLogo";

            string filePath = "";
            try
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", $"Images/{folder}", fileName);

            }
            catch (Exception ex)
            {

                return await JsonH.ErrorAsync(ex.Message);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            //save filePath to database
            string fileAccessPath = Path.Combine(_configuration["ServerAddress"], $"Images/{folder}", fileName);
            Company comp = await _context.Companies.FirstOrDefaultAsync(S => S.Id == companyId);
            CompanyImage newImage = new CompanyImage
            {
                Id = Guid.NewGuid().ToString(),
                CompanyId = comp.Id,
                Address = fileAccessPath,
                AltText = altText
            };
            _context.CompanyImages.Add(newImage);

            if (type == "logo")
            {
                newImage.ImageType = ImageType.largehumbnail;
                comp.LogoImage = newImage;
            }
            else
            {
                newImage.ImageType = ImageType.catalog;
                comp.ProfileImage = (newImage);
            }

            _context.Entry(comp).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return JsonH.Success(new { address = filePath });
        }




        #region File upload


        /*
        [Obsolete]
        [HttpPost("upload-serduct-image/{serductId}")]
        public async Task<IActionResult> UploadSerductImage([FromQuery] IFormFile file, string serductId)
        {
            //upload files to wwwroot
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(_hostingEnv.WebRootPath, "images", fileName);

            using (var fileSteam = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileSteam);
            }
            //your logic to save filePath to database, for example


            SerductImage newImage = new SerductImage();
            newImage.id = Guid.NewGuid().ToString();
            newImage.address = filePath;
            newImage.altText = filePath;



            _context.SerductImages.Add(newImage);
            Serduct ser = await _context.Serducts.FirstOrDefaultAsync(S => S.id == serductId);
            if (ser.images == null)
            {
                ser.images = new List<SerductImage>();
            }
            ser.images.Add(newImage);
            var art = "sljdnv";

            _context.Entry(ser).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                art = ex.Message;
            }
            return JsonH.Success(art);
        }

        */

        [Obsolete]


        // GET Image
        [HttpGet("get-serduct-image/{id}")]
        public async Task<ActionResult<SerductImage>> GetSerductImage(string id)
        {
            var serductImage = await _context.SerductImages.FindAsync(id);

            if (serductImage == null)
            {
                return NotFound();
            }

            return serductImage;
        }




        // GET Image
        [HttpGet("get-all-serduct-images")]
        public async Task<ActionResult<IEnumerable<SerductImage>>> GetAllSerductImages()
        {
            return await _context.SerductImages.ToListAsync();
        }

        #endregion
    }
}
