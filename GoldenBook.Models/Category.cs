﻿using System.ComponentModel.DataAnnotations;

namespace GoldenBook.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Display(Name ="Display Order")]
        [Range(1,100,ErrorMessage ="Display Order must be between1 and 100!")]
        public int DisplayOrder { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
    }
}
