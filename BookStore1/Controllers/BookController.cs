using BookStore1.Models;
using BookStore1.Models.Repository;
using BookStore1.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore1.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookStoreRepository<Book> bookRepository;
        private readonly IBookStoreRepository<Author> authorRepository;
        private readonly IHostingEnvironment hosting;
        public BookController(IBookStoreRepository<Book> bookRepository, IBookStoreRepository<Author> authorRepository, IHostingEnvironment hosting)
        {
            this.bookRepository = bookRepository;
            this.authorRepository = authorRepository;
            this.hosting = hosting;
        }

        // GET: BookController
        public ActionResult Index()
        {
            var book = bookRepository.List();
            return View(book);
        }

        // GET: BookController/Details/5
        public ActionResult Details(int id)
        {
            var book = bookRepository.Find(id);
            return View(book);
        }

        // GET: BookController/Create
        public ActionResult Create()
        {
            var model = new BookAuthorViewModel
            {
                Authors = FillSelectList()
            };
            return View(model);
        }

        // POST: BookController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BookAuthorViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string fileName = UploadFile(model.File) ?? string.Empty;

                    if (model.AuthorId == -1)
                    {
                        ViewBag.Message = "Please select an author from the list!";

                        return View(GetAllAuthors());
                    }
                    var author = authorRepository.Find(model.AuthorId);

                    Book book = new Book
                    {
                        Id = model.BookId,
                        Title = model.Title,
                        Description = model.Description,
                        ImageUrl = fileName,
                        Author = author
                    };
                    bookRepository.Add(book);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    var x = ex.Message;
                    return View();
                }
            }

            ModelState.AddModelError("", "You have to fill all the required fields!");
            return View(GetAllAuthors());
        }

        // GET: BookController/Edit/5
        public ActionResult Edit(int id)
        {
            var book = bookRepository.Find(id);
            var authorId = book.Author == null ? book.Author.Id = 0 : book.Author.Id;

            var viewModel = new BookAuthorViewModel
            {
                BookId = book.Id,
                Title = book.Title,
                Description = book.Description,
                AuthorId = authorId,
                Authors = authorRepository.List().ToList(),
                ImageUrl = book.ImageUrl
            };

            return View(viewModel);
        }

        // POST: BookController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BookAuthorViewModel model)
        {
            try
            {
                string fileName = UploadFile(model.File, model.ImageUrl);

                var author = authorRepository.Find(model.AuthorId);

                Book book = new Book
                {
                    Id = model.BookId,
                    Title = model.Title,
                    Description = model.Description,
                    Author = author,
                    ImageUrl = fileName
                };

                bookRepository.Update(model.BookId, book);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var e = ex.Message;
                return View();
            }
        }

        // GET: BookController/Delete/5
        public ActionResult Delete(int id)
        {
            var book = bookRepository.Find(id);
            return View(book);
        }

        // POST: BookController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(int id)
        {
            try
            {
                bookRepository.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        List<Author> FillSelectList()
        {
            var author = authorRepository.List().ToList();
            author.Insert(0, new Author { Id = -1, FullName = "--- Please select an author ---" });

            return author;
        }

        BookAuthorViewModel GetAllAuthors()
        {
            var Model = new BookAuthorViewModel
            {
                Authors = FillSelectList()
            };

            return Model;
        }

        public string UploadFile(IFormFile file)
        {
            if (file != null)
            {
                string uploads = Path.Combine(hosting.WebRootPath, "Uploads");
                string fullPath = Path.Combine(uploads, file.FileName);
                file.CopyTo(new FileStream(fullPath, FileMode.Create));

                return file.FileName;
            }

            return null;
        }

        public string UploadFile(IFormFile file, string imageUrl)
        {
            if (file != null)
            {
                string uploads = Path.Combine(hosting.WebRootPath, "Uploads");
                string newPath = Path.Combine(uploads, file.FileName);

                //Old path
                string oldPath = Path.Combine(uploads, imageUrl);

                if (oldPath != newPath)
                {
                    //delete the old file
                    System.IO.File.Delete(oldPath);

                    //Save the new file
                    file.CopyTo(new FileStream(newPath, FileMode.Create));
                }

                return file.FileName;
            }

            return imageUrl;
        }

        public ActionResult Search(string term)
        {
            var result = bookRepository.Search(term);

            return View("Index", result);
        }
    }
}
