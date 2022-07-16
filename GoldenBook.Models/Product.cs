using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBook.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [Range(1,10000)]
        [Display(Name = "List Price")]
        public int ListPrice { get; set; }
        [Required]
        [Range(1, 10000)]
        [Display(Name = "Price for 1-5")]
        public int Price { get; set; }
        [Required]
        [Range(1, 10000)]
        [Display(Name = "Price for 6-10")]
        public int Price5 { get; set; }
        [Required]
        [Range(1, 10000)]
        [Display(Name = "Price for 10+")]
        public int Price10 { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; }
        [Display(Name ="Cover Type")]
        public int CoverTypeId { get; set; }
        [ValidateNever]
        public CoverType CoverType { get; set; }

    }   
}
