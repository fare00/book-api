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
    public class BookController : ControllerBase
    {
        private readonly DatabaseContext _context;
        public BookController(DatabaseContext context) => _context = context;
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookModelDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<BookModelDto>> Get() {
            var books = await _context.Books.Include("Author").ToListAsync();
            List<BookModelDto> newBooks = new List<BookModelDto>();

            foreach(var book in books) {
                BookModelDto newBook = new BookModelDto();
                newBook.Id = book.Id;
                newBook.Title = book.Title;
                newBook.Description = book.Description;
                newBook.Author = new AuthorModelDto();
                newBook.Author.Id = book.Author.Id;
                newBook.Author.FirstName = book.Author.FirstName;
                newBook.Author.LastName = book.Author.LastName;
                foreach(var b in book.Author.Books) {
                    BookModelDto nb = new BookModelDto();
                    nb.Title = b.Title;
                    nb.Description = b.Description;
                    nb.Id = b.Id;
                    newBook.Author.Books.Add(nb);
                }
                newBooks.Add(newBook);
            }

            return Ok(newBooks);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BookModelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id) {
            var book = await _context.Books.Include("Author").FirstOrDefaultAsync(b => b.Id == id);

            if(book == null) return NotFound();

            BookModelDto newBook = new BookModelDto();
            newBook.Id = book.Id;
            newBook.Title = book.Title;
            newBook.Description = book.Description;
            newBook.Author = new AuthorModelDto();
            newBook.Author.Id = book.Author.Id;
            newBook.Author.FirstName = book.Author.FirstName;
            newBook.Author.LastName = book.Author.LastName;
            foreach(var b in book.Author.Books) {
                BookModelDto nb = new BookModelDto();
                nb.Title = b.Title;
                nb.Description = b.Description;
                nb.Id = b.Id;
                newBook.Author.Books.Add(nb);
            }

            return Ok(newBook);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(BookModelDtoPost book) {
            var bookToAdd = new BookModel();
            bookToAdd.Title = book.Title;
            bookToAdd.Description = book.Description;
            bookToAdd.Id = book.Id;
            bookToAdd.AuthorId = book.AuthorId;

            await _context.Books.AddAsync(bookToAdd);
            try { await _context.SaveChangesAsync(); } catch { return BadRequest(); }

            return CreatedAtAction(nameof(GetById), new {Id = bookToAdd.Id}, bookToAdd);
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, BookModelDtoPost book) {
            if(id != book.Id) return BadRequest();

            var bookToUpdate = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);

            bookToUpdate.Title = book.Title;
            bookToUpdate.Description = book.Description;
            bookToUpdate.AuthorId = book.AuthorId;

            try { await _context.SaveChangesAsync(); } catch { return BadRequest(); }

            return NoContent();
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id) {
            var book = await _context.Books.FindAsync(id);
            if(book == null) return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}