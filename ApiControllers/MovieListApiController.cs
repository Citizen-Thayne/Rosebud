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

namespace RoseBud.Controllers
{
    public class MovieListApiController : ApiController
    {
        private RoseBudContext db = new RoseBudContext();

        // GET: api/MovieLists
        public IQueryable<MovieList> GetMovieLists()
        {
            return db.MovieLists;
        }

        // GET: api/MovieLists/5
        [ResponseType(typeof(MovieList))]
        public async Task<IHttpActionResult> GetMovieList(int id)
        {
            MovieList movieList = await db.MovieLists.FindAsync(id);
            if (movieList == null)
            {
                return NotFound();
            }

            return Ok(movieList);
        }

        // PUT: api/MovieLists/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMovieList(int id, MovieList movieList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != movieList.Id)
            {
                return BadRequest();
            }

            db.Entry(movieList).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieListExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/MovieLists
        [ResponseType(typeof(MovieList))]
        public async Task<IHttpActionResult> PostMovieList(MovieList movieList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            movieList.CreatedDate = DateTime.Now;
            db.MovieLists.Add(movieList);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = movieList.Id }, movieList);
        }

        // DELETE: api/MovieLists/5
        [ResponseType(typeof(MovieList))]
        public async Task<IHttpActionResult> DeleteMovieList(int id)
        {
            MovieList movieList = await db.MovieLists.FindAsync(id);
            if (movieList == null)
            {
                return NotFound();
            }
            db.Movies.RemoveRange(movieList.Movies);
            db.MovieLists.Remove(movieList);
            await db.SaveChangesAsync();

            return Ok(movieList);
        }
          

        [ResponseType(typeof(MovieList))]
        public async Task<IHttpActionResult> AddMovie(int id, [FromBody] Movie movie)
        {
            MovieList movieList = await db.MovieLists.FindAsync(id);

            if (movieList == null || movie == null)
            {
                return NotFound();
            }

            movieList.Movies.Add(movie);
            db.Entry(movieList).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieListExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(movieList);
        }

        [HttpDelete]
        [ResponseType(typeof(MovieList))]
        public async Task<IHttpActionResult> RemoveMovie(int id, int movieId)
        {
            MovieList movieList = await db.MovieLists.FindAsync(id);
            Movie movie = await db.Movies.FindAsync(movieId);

            if (movieList == null || movie == null)
            {
                return NotFound();
            }
            if (movieList.Movies.Any(m => m.ID == movie.ID))
            {
                db.Movies.Remove(movie);
                movieList.Movies.Remove(movie);
            }
            else
            {
                return BadRequest("Movie was not in list");
            }
            db.Entry(movieList).State = EntityState.Modified;
            db.Entry(movie).State = EntityState.Deleted;
            await db.SaveChangesAsync();

            // Get list from db to send back fresh copy
            movieList = await db.MovieLists.FindAsync(id);
            return Ok(movieList);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MovieListExists(int id)
        {
            return db.MovieLists.Count(e => e.Id == id) > 0;
        }
    }
}