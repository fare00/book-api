using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using books_api.Models;
using Microsoft.EntityFrameworkCore;

namespace books_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly DatabaseContext _context;
        public AuthorController(DatabaseContext context) => _context = context;
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AuthorModelDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthorModelDto>> Get() {
            var authors = await _context.Authors.Include("Books").ToListAsync();
            List<AuthorModelDto> newAuthors = new List<AuthorModelDto>();

            foreach(var author in authors) {
                AuthorModelDto newAuthor = new AuthorModelDto();
                newAuthor.Id = author.Id;
                newAuthor.FirstName = author.FirstName;
                newAuthor.LastName = author.LastName;
                foreach(var b in author.Books) {
                    BookModelDto nb = new BookModelDto();
                    nb.Id = b.Id;
                    nb.Title = b.Title;
                    nb.Description = b.Description;
                    newAuthor.Books.Add(nb);
                }
                newAuthors.Add(newAuthor);
            }

            return Ok(newAuthors);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AuthorModelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id) {
            var author = await _context.Authors.Include("Books").FirstOrDefaultAsync(a => a.Id == id);

            if(author == null) return NotFound();

            AuthorModelDto newAuthor = new AuthorModelDto();
            newAuthor.Id = author.Id;
            newAuthor.FirstName = author.FirstName;
            newAuthor.LastName = author.LastName;
            foreach(var b in author.Books) {
                BookModelDto nb = new BookModelDto();
                nb.Id = b.Id;
                nb.Title = b.Title;
                nb.Description = b.Description;
                newAuthor.Books.Add(nb);
            }

            return Ok(newAuthor);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(AuthorModelDtoPost author) {
            var newAuthor = new AuthorModel();

            newAuthor.Id = author.Id;
            newAuthor.FirstName = author.FirstName;
            newAuthor.LastName = author.LastName;

            await _context.Authors.AddAsync(newAuthor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new {id = newAuthor.Id}, newAuthor);
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, AuthorModelDtoPost author) {
            if(id != author.Id) return BadRequest();

            var newAuthor = new AuthorModel();

            newAuthor.Id = author.Id;
            newAuthor.FirstName = author.FirstName;
            newAuthor.LastName = author.LastName;

            _context.Entry(newAuthor).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); } catch { return BadRequest(); }

            return NoContent();
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id) {
            var author = await _context.Authors.FindAsync(id);
            if(author == null) return NotFound();

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}