using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Model
{
    public class Player
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int playerID { get; set; }
        public string nickname { get; set; }
  
        private DateTime? date;

        public DateTime? Timeof
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
        public int scores { get; set; }
    }
}
