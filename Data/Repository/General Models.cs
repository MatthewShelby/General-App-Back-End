using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Doctors
{
    //public class UsersCompany
    //{
    //    public string UserId { get; set; }

    //    public string CompanyId { get; set; }
    //}

    public class Company
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string OwnerId { get; set; }


        [Required]
        public string CompanyName { get; set; }


        public string CompanyTitle { get; set; }

        public string CompanyShortBio { get; set; }

        public string CompanyLongBio { get; set; }

        public CompanyImage ProfileImage { get; set; }

        public CompanyImage LogoImage { get; set; }

        public virtual ICollection<ContactInfo> ContactInfos { get; set; }

        public ICollection<Serduct> Serducts { get; set; }

    }


    public class Serduct
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public SerductType SerductType { get; set; }

        public string Title { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public ICollection<SerductImage> Images { get; set; }

    }

    public enum SerductType
    {
        Service,
        Product,
    }


    public class SerductImage
    {

        [Key]
        public string Id { get; set; }

        [Required]
        public ImageType ImageType { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string AltText { get; set; }

    }

    public class CompanyImage
    {

        [Key]
        public string Id { get; set; }

        [Required]
        public ImageType ImageType { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string AltText { get; set; }

    }

    public enum ImageType
    {
        smaillThumbnail,       // 50*50
        largehumbnail,         // 75*75
        card,                  // 255*255  -  220 * 280 would be great
        catalog,               // 640*640
        cover,                 // 400*1400
        full,                  // 700*1400
    }


    public class ContactInfo
    {
        [Key]
        public string Id { get; set; }

        [JsonIgnore]
        public Company Company { get; set; }

        //public string ownerId;

        public ContactInfoType Type { get; set; }

        public string Value { get; set; }
    }
    public enum ContactInfoType
    {
        country,
        city,
        address,
        postalCode,
        phoneNumber,
        cellNumber,
        email,
        website,
        socialNetwork
    }
}
