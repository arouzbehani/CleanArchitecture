using DomainCore.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{

    public class DocumentRepository : IDocumentRepository
    {
        private readonly AppDbContext _context;

        public DocumentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Document> Get(int id)
        {
            return await _context.Documents.FindAsync(id);
        }

        public async Task<Document> Add(Document document)
        {
            await _context.Documents.AddAsync(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<IEnumerable<Document>> GetAll()
        {
            return await _context.Documents.ToListAsync();
        }

        public async Task Delete(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }
        public async Task<Document> Update(Document doc)
        {
            _context.Documents.Update(doc);
            await _context.SaveChangesAsync();
            return doc;
        }

 
    }

}