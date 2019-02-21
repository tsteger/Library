using AutoMapper;
using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    [Route("api/authorcollections")]
    public class AuthorCollectionsController : Controller
    {
        private ILibraryRepository _libraryRepository;

        public AuthorCollectionsController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpPost]
        public IActionResult CreateAuthorCollection(
            [FromBody] IEnumerable<AuthorForCreationDto> authorCollection)
        {
            if(authorCollection == null)
            {
                return BadRequest();
            }

            var authorEntitis = Mapper.Map<IEnumerable<Author>>(authorCollection);

            foreach (var author in authorEntitis)
            {
                _libraryRepository.AddAuthor(author);
            }
            if (!_libraryRepository.Save())
            {
                throw new Exception("Creating an athor collection faild on save");
            }

            var authorCollectionToReturn = Mapper.Map<IEnumerable<AuthorDto>>(authorEntitis);
            var idsAsString = string.Join(",",
                authorCollectionToReturn.Select(a => a.Id));
            return CreatedAtRoute("GetAuthorCollection",
                new { ids = idsAsString },
                authorCollectionToReturn);
           // return Ok();
        }
        [HttpGet("({ids})", Name ="GetAuthorCollection")]
        public IActionResult GetAuthorCollection([ModelBinder(BinderType  = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if(ids == null)
            {
                return BadRequest();
            }

            var authorEntites = _libraryRepository.GetAuthors(ids);

            if(ids.Count() != authorEntites.Count())
            {
                return NotFound();
            }

            var authorsToReturn = Mapper.Map<IEnumerable<AuthorDto>>(authorEntites);
            return Ok(authorsToReturn);
        }
       
    }
}
