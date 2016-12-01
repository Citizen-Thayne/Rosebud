using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using RoseBud.Models;
using System.Web;

namespace RoseBud.Controllers
{
    public class MovieApiController : ApiController
    {
        private RoseBudContext db = new RoseBudContext();
        private string uri = "http://www.omdbapi.com/";
        private HttpClient getImdbClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(uri);
            return client;
        }

        public class SearchParams
        {
            public string i { get; set; }
            public string t { get; set; }
            public string type { get; set; }
            public string s { get; set; }
        }
        public class SearchResultList
        {
            public IEnumerable<Movie> Search { get; set; }
            public int totalResults { get; set; }
            public string Response { get; set; }
        }
        // GET: api/Movies
        [HttpGet]
        public async Task<IHttpActionResult> search([FromUri] SearchParams param)
        {
            string title = param.t;
            string id = param.i;
            string search = param.s;

            if (id == null && title == null && search == null)
            {
                return BadRequest();
            }
            UriBuilder builder = new UriBuilder(uri);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["i"] = id;
            query["t"] = title;
            query["s"] = search;
            builder.Query = query.ToString();

            HttpClient client = getImdbClient();
            HttpResponseMessage response = await client.GetAsync(builder.ToString());
            if (response.IsSuccessStatusCode)
            {
                if (search != null)
                {
                    var movies = await response.Content.ReadAsAsync<SearchResultList>();
                    return Ok(movies);
                }
                else
                {
                    var movie = await response.Content.ReadAsAsync<Movie>();
                    return Ok(movie);
                }
            }
            else
            {
                throw new Exception("Something blew up");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MovieExists(string id)
        {
            return db.Movies.Count(e => e.ImdbID == id) > 0;
        }
    }
}