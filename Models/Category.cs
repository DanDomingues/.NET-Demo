﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ASP.NET_Debut.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(12), DisplayName("Category Name")]
        public string Name { get; set; }
        [Range(1, 100), DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}

//Notes
//Model___ denotes the following as the primary key. [Key] can also be used
//[Required] ensures a DB entry has a default value for a property
//Through websites such as https://icons.getbootstrap.com/, one can install and use icons, which include their install(cdn) and usage (html) lines
//pt and pb correspond to padding top and padding bottom