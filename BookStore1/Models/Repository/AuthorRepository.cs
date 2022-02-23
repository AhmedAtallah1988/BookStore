using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore1.Models.Repository
{
    public class AuthorRepository : IBookStoreRepository<Author>
    {
        private readonly BookStoreDbContext db;

        public AuthorRepository(BookStoreDbContext _db)
        {
            db = _db;
        }

        public void Add(Author entity)
        {
            db.Authors.Add(entity);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var author = Find(id);
            db.Authors.Remove(author);
            db.SaveChanges();
        }

        public Author Find(int id)
        {
            var author = db.Authors.SingleOrDefault(x => x.Id == id);
            return author;
        }

        public IList<Author> List()
        {
            return db.Authors.ToList();
        }

        public List<Author> Search(string term)
        {
            var result = db.Authors.Where(x => x.FullName.Contains(term)).ToList();

            return result;
        }

        public void Update(int id, Author newAuthor)
        {
            db.Authors.Update(newAuthor);
            db.SaveChanges();
        }
    }
}
