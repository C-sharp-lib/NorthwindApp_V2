using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApplication.Models
{
    public class Territories
    {
        [Key]
        public string TerritoryID { get; set; }
        public string TerritoryDescription { get; set; }

        public int RegionID { get; set; }
        public Region Region { get; set; }
    }
}
