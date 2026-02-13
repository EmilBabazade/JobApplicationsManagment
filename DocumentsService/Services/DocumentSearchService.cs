using Grpc.Core;
using DocumentsService.Protos;

namespace DocumentsService.Services;

public class DocumentSearchService(ILogger<DocumentSearchService> logger) : DocumentSearch.DocumentSearchBase
{
    private readonly List<Document> _documents = [
        new Document() {
            File = "cv.pdf",
            PersonId = "11111111-1111-1111-1111-111111111111",
            Id = "11111111-1111-1111-1111-111111111111"
        },
        new Document() {
            File = "motivation_letter.pdf",
            PersonId = "11111111-1111-1111-1111-111111111111",
            Id = "22222222-2222-2222-2222-222222222222"
        }
    ];

    public override async Task<DocumentList> Get(DocumentSearchRequest request, ServerCallContext context)
    {
        var documentList = new DocumentList();
        var documents = _documents.Where(x => x.PersonId == request.PersonId).ToList();
        documentList.Documents.AddRange(documents);
        return documentList;
    }
}