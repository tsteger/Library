using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.API.Helpers;
using AutoMapper;
using Library.API.Entities;
using Microsoft.AspNetCore.Http;

namespace Library.API.Controllers
{
    [Route("api/authors")]
    public class AuthorController : Controller
    {
        private ILibraryRepository _libraryRepository;

        public AuthorController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }


        [HttpGet()]
        public IActionResult GetAuthors()
        {
            
              var authorsFromRepo = _libraryRepository.GetAuthors();
              var authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);
              return Ok(authors);
           
        }
        [HttpGet("{id}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid id)
        {
            var authorFromRepo = _libraryRepository.GetAuthor(id);
            if(authorFromRepo == null)
            {
                return NotFound();
            }
            var author = Mapper.Map<AuthorDto>(authorFromRepo);
            return Ok(author);
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] AuthorForCreationDto author)
        {
            if(author == null)
            {
                return BadRequest();
            }

            var authorEntity = Mapper.Map<Author>(author);
            _libraryRepository.AddAuthor(authorEntity);
            if(!_libraryRepository.Save())
            {
                throw new Exception("Creating an author faild on save");
               // return StatusCode(500,"A problem happend with handling your request");
            }

            var authorToReturn = Mapper.Map<AuthorDto>(authorEntity);
            return CreatedAtRoute("GetAuthor", new { id = authorToReturn.Id}, authorToReturn);
        }
        [HttpPost("{id}")]
        public IActionResult BlockAuthorCreation(Guid id)
        {
            if (_libraryRepository.AuthorExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAuthor(Guid id)
        {
            var authorFromRepo = _libraryRepository.GetAuthor(id);
            if(authorFromRepo == null)
            {
                return NotFound();
            }

            _libraryRepository.DeleteAuthor(authorFromRepo);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Deleting autor {id} faild on save");
            }
            return NoContent();
        }
    }
}
