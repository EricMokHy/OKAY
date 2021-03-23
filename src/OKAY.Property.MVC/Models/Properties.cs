using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OKAY.Property.MVC.Models
{
    public class Property
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [MaxLength(200)]
        [Required]
        public string name { get; set; }
        public int bedroom { get; set; }
        [Required]
        public bool isAvailable { get; set; }
        [Required]
        public decimal leasePrice { get; set; }
        [Required]
        [MaxLength(128)]
        public string userId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}