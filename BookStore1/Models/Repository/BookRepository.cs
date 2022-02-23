using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore1.Models.Repository
{
    public class BookRepository : IBookStoreRepository<Book>
    {
        private readonly BookStoreDbContext db;

        public BookRepository(BookStoreDbContext _db)
        {
            db = _db;
        }

        public void Add(Book entity)
        {
            db.Books.Add(entity);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var book = Find(id);
            db.Books.Remove(book);
            db.SaveChanges();
        }

        public Book Find(int id)
        {
            var book = db.Books.Include(x => x.Author).SingleOrDefault(x => x.Id == id);
            return book;
        }

        public IList<Book> List()
        {
            return db.Books.Include(x => x.Author).ToList();
        }

        public void Update(int id, Book newBook)
        {
            db.Books.Update(newBook);
            db.SaveChanges();
        }

        public List<Book> Search(string term)
        {
            var result = db.Books.Include(x => x.Author).Where(x => x.Title.Contains(term) || x.Description.Contains(term) || x.Author.FullName.Contains(term)).ToList();

            return result;
        }
    }
}
