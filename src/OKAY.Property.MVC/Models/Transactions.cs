using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OKAY.Property.MVC.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public int propertyId { get; set; }
        [Required]
        [MaxLength(128)]
        public string userId { get; set; }
        [Required]
        public DateTime TransactionDate { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Property Property { get; set; }
    }
}