using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi
{
    public class Game
    {
        public string original;
        public List<Guess> list = new List<Guess>();
        public int kokonaispisteet;
    }
}
