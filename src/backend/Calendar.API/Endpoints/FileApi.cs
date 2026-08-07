using System.Collections.Concurrent;
using System.Text.Encodings.Web;
using Calendar.Domain.Model;
using Calendar.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using NodaTime;

namespace Calendar.API.Endpoints;

// todo: previews, thumbnails, etc.
internal static class FileApi
{
    const string StorageRoot = @"/storage/uploads/";

    internal static RouteGroupBuilder MapFileApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/files").RequireAuthorization();
        
        group.MapPost("/", async Task<Created<Domain.Model.File>> (
            IFormFile fileData, 
            UserId userId, 
            CalendarDbContext db,
            IClock clock, 
            CancellationToken cancellationToken) =>
            {
                var saveResult = await SaveToDisk(fileData, cancellationToken);

                var originalName = Path.GetFileName(fileData.FileName);

                var file = new Domain.Model.File
                {
                    UserId = userId,
                    Created = clock.GetCurrentInstant(),
                    OriginalName = originalName,
                    ContentType = fileData.ContentType,
                    ContentDisposition = fileData.ContentDisposition,
                    Size = fileData.Length,
                    SaveResult = saveResult
                };

                db.Files.Add(file);
                
                await db.SaveChangesAsync(cancellationToken);

                return TypedResults.Created($"/files/{file.Id}", file);
            });

       
        
        static async Task<SaveResult> SaveToDisk(IFormFile file, CancellationToken cancellationToken)
        {
            if (file is { Length: 0 })
            {
                return new(false,null);
            }
                    
            Directory.CreateDirectory(StorageRoot);

            var filePath = Path.Combine(StorageRoot, Path.GetRandomFileName());

            await using var stream = System.IO.File.Create(filePath);

            await file.CopyToAsync(stream, cancellationToken);

            return new(true, new Uri(filePath));    
        }

        return group;
    }
}

