using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IronPython;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using RestApi.Model;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;


namespace RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class sorsaController : ControllerBase
    {
        SanasorsaContext db = new SanasorsaContext();
        
        private readonly IConfiguration _config;

        public sorsaController(IConfiguration config)
        {
            _config = config;
        }

        // GET: api/sorsa
        [HttpGet]
        public string Get()
        {
            return APIUtils.GetRandomWord();

            //alla vain APIn testaukseen käytettävä metodi (kovakoodatut promptisanat)
            //return APIUtils.GetRandomWordHardcoded();
        }

        [HttpGet("Top10", Name ="Top10")]
        public string GetLeaderBoard()
        {
            var a = db.Player.OrderByDescending(player => player.scores).Take(10).ToList(); 
            return JsonConvert.SerializeObject(a);
        }

        //[Route("token")]
        [HttpGet("getToken", Name ="getToken")]
        public string GetAzureCognitiveToken()
        {
            string token = "";
            string key = _config.GetValue<string>("AzureCognitive:Key");
            string region = _config.GetValue<string>("AzureCognitive:Region");

            using (var client = new HttpClient())
            {
                string[] headers = new string[] { "Content-Type: application/json", $"Ocp-Apim-Subscription-Key: {key}" };
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Add("Access-Control-Allow-Origin", "*");
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

                var content = new StringContent("{}", Encoding.UTF8, "application/json");
                var response = client.PostAsync($"https://{region}.api.cognitive.microsoft.com/sts/v1.0/issueToken", content).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;
                token = responseString;

            }
                return token;
        }
        
        [HttpPost("Tallennus", Name = "Tallennus")]
        public HttpResponseMessage Tallennus(Player p)
        {
            if (!string.IsNullOrEmpty(p.nickname))
            {
            db.Add(p);
            db.SaveChanges();
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            }else
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }

        }

        [HttpPost]
        public string Post(Wordlist sanalista)
        {
            if (sanalista.Guesses.Length <1||(sanalista.Guesses.Length==1&&string.IsNullOrWhiteSpace(sanalista.Guesses[0])))
            {
                Game t = new Game() { kokonaispisteet = 0, original = sanalista.Original, list = new List<Guess>() };
                string tyhjäpeli = JsonConvert.SerializeObject(t);
                Statistics s = new Statistics() { json = tyhjäpeli, Time = null };
                db.Add(s);
                db.SaveChanges();
                //palauta yllä oleva peliToJson klientille
                return tyhjäpeli;
            }
            else
            {
            sanalista=APIUtils.PoistaTurhat(sanalista); //poistaa turhat merkit
            var guessesilmantuplia = sanalista.Guesses.Distinct().ToArray();
            Wordlist distinctlist = new Wordlist() { Guesses = guessesilmantuplia, Original = sanalista.Original };
            sanalista = distinctlist;
            string json = JsonConvert.SerializeObject(sanalista);
            //APIUtils.RemovePunctuation(json);
            // postaa olio azurefunktiolle
            Scoretable scoresFromAzure = GetScores(json); //tähän tulee azuresta palautuva json (joka on pelkkä numero-array)

            //yhdistä sanat ja pisteet Guess-olioiksi, jotka laitetaan Peli-olioon

            List<Guess> listToPeliClass = new List<Guess>();
            // tee sanoista guess-olioita, jotka tallennetaan listaan.
            int kokonaispisteet = 0;
            for (int i = 0; i < sanalista.Guesses.Length; i++)
            {
                Guess g = new Guess();

                g.word = sanalista.Guesses[i];
                if (sanalista.Guesses[i] == sanalista.Original)
                {
                    g.score = 0;
                }
                
                else
                {
                    g.score = scoresFromAzure.ScoreTable[i];
                }

                kokonaispisteet += scoresFromAzure.ScoreTable[i];
                if (kokonaispisteet < 0)
                    kokonaispisteet = 0;
                listToPeliClass.Add(g);
            }

            Game peli = new Game()
            {
                original = sanalista.Original,
                list = listToPeliClass,
                kokonaispisteet=kokonaispisteet
            };
           
            string peliToJson = JsonConvert.SerializeObject(peli);
            Statistics stat = new Statistics() {json=peliToJson, Time=null };
            db.Add(stat);
            db.SaveChanges();
            //palauta yllä oleva peliToJson klientille

            return peliToJson;

            }
            // parseroi azurefunkion palauttama json peli-olioksi(original, guesses-taulukko)
        }

        private Scoretable GetScores(string json)
        {
            string palautus = "";
            string azureFunctionKey = _config.GetValue<string>("ModelFunctionKey");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
              
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PostAsync($"https://sanasorsa-helper.azurewebsites.net/api/get_distances?code={azureFunctionKey}", content).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;
                palautus = responseString;
            }

            ScoreHelper listofscores = JsonConvert.DeserializeObject<ScoreHelper>(palautus);
            Scoretable st = new Scoretable(listofscores.data);
           
            return st;
        }

    }

}

