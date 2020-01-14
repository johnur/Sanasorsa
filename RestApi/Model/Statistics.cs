using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Model
{
        [Table("Statistic")]
    public class Statistics
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int statID { get; set; }
        public string json { get; set; }
        private DateTime? date;

        public DateTime? Time
        {
            get { return date; }
            set
            {
                if (value == null)
                {
                    date = DateTime.Now;
                }
                else
                {
                    date = value;

                }

            }
        }
    }
}
