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
using System.Linq;
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



        [HttpPost("upload-company-logo-image")]
        public async Task<IActionResult> UploadCompanyLogoImage(IFormFile file,
            [FromForm] string altText, [FromForm] string companyId)
        {
            // init CompanyImageEntity

            string fileName = Guid.NewGuid().ToString() + "-" + Path.GetFileName(file.FileName);
            string fileAccessPath = Path.Combine(_configuration["ServerAddress"], "Images\\companyLogo", fileName);

            CompanyImage newCompanyImage = new CompanyImage()
            {
                Id = Guid.NewGuid().ToString(),
                AltText = altText,
                CompanyId = companyId,
                CompanyImageType = CompanyImageType.logo,
                Address = fileAccessPath
            };


            // write file
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images\\companyLogo", fileName);
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                return await JsonH.ErrorAsync("Couldnt write the file. Error: " + ex.Message);
            }


            Company company = _context.Companies.Find(companyId);
            company.Images = _context.CompanyImages.Where(c => c.CompanyId == company.Id).ToList();
            if (company.Images == null)
                company.Images = new List<CompanyImage>();
            else
            {
                CompanyImage existingCompanyImage = company.Images.FirstOrDefault(i => i.CompanyImageType == CompanyImageType.logo);
                if (existingCompanyImage != null)
                {
                    _context.CompanyImages.Remove(existingCompanyImage);
                    company.Images.Remove(existingCompanyImage);
                }
            }
            company.Images.Add(newCompanyImage);

            _context.Entry(company).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await JsonH.ErrorAsync(ex.Message);
            }
            return await JsonH.SuccessAsync(newCompanyImage.Address);

        }


        [HttpPost("upload-company-profile-image")]
        public async Task<IActionResult> UploadCompanyProfileImage(IFormFile file,
            [FromForm] string altText, [FromForm] string companyId)
        {
            // init CompanyImageEntity

            string fileName = Guid.NewGuid().ToString() + "-" + Path.GetFileName(file.FileName);
            string fileAccessPath = Path.Combine(_configuration["ServerAddress"], "Images\\companyProfile", fileName);

            CompanyImage newCompanyImage = new CompanyImage()
            {
                Id = Guid.NewGuid().ToString(),
                AltText = altText,
                CompanyId = companyId,
                CompanyImageType = CompanyImageType.profile,
                Address = fileAccessPath
            };


            // write file
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images\\companyProfile", fileName);
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                return await JsonH.ErrorAsync("Couldnt write the file. Error: " + ex.Message);
            }


            Company company = _context.Companies.Find(companyId);
            company.Images = _context.CompanyImages.Where(c => c.CompanyId == company.Id).ToList();
            if (company.Images == null)
                company.Images = new List<CompanyImage>();
            else
            {
                CompanyImage existingCompanyImage = company.Images.FirstOrDefault(i => i.CompanyImageType == CompanyImageType.profile);
                if (existingCompanyImage != null)
                {
                    _context.CompanyImages.Remove(existingCompanyImage);
                    company.Images.Remove(existingCompanyImage);
                }
            }
            company.Images.Add(newCompanyImage);

            _context.Entry(company).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await JsonH.ErrorAsync(ex.Message);
            }
            return await JsonH.SuccessAsync(newCompanyImage.Address);

        }






        /*

        [HttpPost("upload-company-image")]
        public async Task<IActionResult> UploadCompanyImage(IFormFile file,
            [FromForm] string altText, [FromForm] string type, [FromForm] string companyId)
        {
            string idd = companyId;
            var fileName = Guid.NewGuid().ToString() + "-" + Path.GetFileName(file.FileName);

            string folder = (type == "logo") ? "companyLogo" : "companyProfile";


            //save filePath to database
            string fileAccessPath = Path.Combine(_configuration["ServerAddress"], $"Images\\{folder}", fileName);
            Company comp = await _context.Companies.FirstOrDefaultAsync(S => S.Id == idd);


            try
            {
                ICollection<CompanyImage> companyImageList = await _context.CompanyImages.ToListAsync();
                CompanyImage existingCompanyProfileImage = companyImageList.FirstOrDefault(s => s.CompanyId == companyId && s.ImageType == ImageType.catalog);

                if (existingCompanyProfileImage != null && type == "profile")
                {
                    _context.CompanyImages.Remove(existingCompanyProfileImage);
                    //await _context.SaveChangesAsync();
                }

                CompanyImage existingCompanyLogoImage = companyImageList.FirstOrDefault(s => s.CompanyId == companyId && s.ImageType == ImageType.largeThumbnail);

                if (existingCompanyLogoImage != null && type == "logo")
                {
                    _context.CompanyImages.Remove(existingCompanyLogoImage);
                    //await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                return await JsonH.ErrorAsync("Delete error: " + ex.Message);
            }

            #region set images for company
            CompanyImage comImgProf = new CompanyImage();
            CompanyImage comImgLogo = new CompanyImage();

            ICollection<CompanyImage> CI = _context.CompanyImages.Where(i => i.CompanyId == comp.Id).ToList();
            if (CI.Any())
            {
                try
                {
                    comImgProf = CI.FirstOrDefault(i => i.ImageType == ImageType.catalog);

                }
                catch (Exception)
                {

                }
                try
                {
                    comImgLogo = CI.FirstOrDefault(i => i.ImageType == ImageType.largeThumbnail);

                }
                catch (Exception)
                {

                }

            }

            if (comImgProf != null)
            {
                comp.ProfileImage = new CompanyImage()
                {
                    Id = comImgProf.Id,
                    Address = comImgProf.Address,
                    AltText = comImgProf.AltText,
                    ImageType = comImgProf.ImageType
                };
            }
            if (comImgLogo != null)
            {
                comp.ProfileImage = new CompanyImage()
                {
                    Id = comImgLogo.Id,
                    Address = comImgLogo.Address,
                    AltText = comImgLogo.AltText,
                    ImageType = comImgLogo.ImageType
                };
            }
            #endregion
            CompanyImage newImage = new CompanyImage
            {
                Id = Guid.NewGuid().ToString(),
                CompanyId = comp.Id,
                Address = fileAccessPath,
                AltText = altText,
                ImageType = ImageType.catalog
            };


            //comp.LogoImage = newImage;

            if (type == "logo")
            {
                folder = "companyLogo";
                newImage.ImageType = ImageType.largeThumbnail;
                comp.LogoImage = newImage;
            }
            else { comp.ProfileImage = newImage; }

            string filePath = "";
            try
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", $"Images\\{folder}", fileName);

            }
            catch (Exception ex)
            {
                return await JsonH.ErrorAsync(ex.Message);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }


            _context.CompanyImages.Add(newImage);
            _context.SaveChanges();
            _context.Entry(comp).State = EntityState.Modified;
            return JsonH.Success(newImage.Id);
        }


        */
        // GET recent Image
        [HttpGet("get-company-image/{id}")]
        public async Task<ActionResult<CompanyImage>> GetCompanyImage(string id)
        {
            var companyImage = await _context.CompanyImages.FindAsync(id);

            if (companyImage == null)
            {
                return NotFound();
            }

            return companyImage;
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
