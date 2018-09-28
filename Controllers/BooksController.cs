﻿using System;
using System.Collections.Generic;
using AutoMapper;
using LibraryAPI.Entities;
using LibraryAPI.Models;
using LibraryAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        private readonly ILibraryRepository _libraryRepository;

        public BooksController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpGet]
        public IActionResult GetBooksForAuthor(Guid authorId)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var booksForAuthorFromRepo = _libraryRepository.GetBooksForAuthor(authorId);

            return Ok(Mapper.Map<IEnumerable<BookDto>>(booksForAuthorFromRepo));
        }

        [HttpGet("{id}")]
        public IActionResult GetBookForAuthor(Guid authorId, Guid id)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);

            return bookForAuthorFromRepo == null ? (IActionResult) NotFound() : Ok(bookForAuthorFromRepo);
        }
    }
}