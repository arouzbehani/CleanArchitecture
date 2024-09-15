using DomainCore.Entities;

public interface IDocumentRepository
{
    Task<Document> Add(Document doc);
    Task<Document> Get(int id);
    Task<IEnumerable<Document>> GetAll();
    Task<Document> Update(Document doc);
    Task Delete(int id);
}