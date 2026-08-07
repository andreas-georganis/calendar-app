using System.Collections.Concurrent;
using Calendar.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using NodaTime;

namespace Calendar.API.Endpoints;

// todo: previews, thumbnails, etc.
internal static class FileApii
{
    const string StorageRoot = @"/storage/uploads/";

    internal static RouteGroupBuilder MapFileUploadApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/file-uploads").RequireAuthorization();
        
        group.MapPost("/", async Task<Created<Domain.Model.FileUpload>> (
            IFormFile file, 
            CurrentUser currentUser, 
            CalendarDbContext dbContext,
            IClock clock, 
            CancellationToken cancellationToken) =>
            {
                var path = await SaveToDisk(file, cancellationToken);

                if (path is not null)
                {
                    savedFiles.Add(
                        (file.FileName, 
                            path, 
                            file.ContentType, 
                            file.ContentDisposition,
                            file.Length));
                }

                var fileUpload = new Domain.Model.FileUpload
                {
                    UserId = currentUser.Id,
                    Created = clock.GetCurrentInstant(),
                    Files = savedFiles.Select(f => new Domain.Model.File
                    {
                        OriginalName = f.OriginalName,
                        ContentType = f.ContentType,
                        ContentDisposition = f.ContentDisposition,
                        Size = f.Size,
                        StorageProvider = "Physical",
                        ObjectKey = f.Path
                    }).ToList()
                };

                dbContext.FileUploads.Add(fileUpload);
                
                await dbContext.SaveChangesAsync(cancellationToken);

                return TypedResults.Created($"/file-uploads/{fileUpload.Id}", fileUpload);
                
                
            });

        // group.MapPost("", async Task<Created<Domain.Model.FileUpload>> (
        //     IFormFileCollection files, 
        //     CurrentUser currentUser, 
        //     CalendarDbContext dbContext,
        //     IClock clock, 
        //     CancellationToken cancellationToken) =>
        //     {
        //         using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        //
        //         var savedFiles = new ConcurrentBag<(
        //             string OriginalName,
        //             string Path,
        //             string ContentType,
        //             string ContentDisposition,
        //             long Size)>();
        //         
        //         var options = new ParallelOptions
        //         {
        //             CancellationToken = cts.Token,
        //             MaxDegreeOfParallelism = Environment.ProcessorCount
        //         };
        //
        //         try
        //         {
        //             await Parallel.ForEachAsync(files, options, async (file, ct) =>
        //             {
        //                 var path = await SaveToDisk(file, ct);
        //
        //                 if (path is not null)
        //                 {
        //                     savedFiles.Add(
        //                         (file.FileName, 
        //                             path, 
        //                             file.ContentType, 
        //                             file.ContentDisposition,
        //                             file.Length));
        //                 }
        //             });
        //         }
        //         catch
        //         {
        //             await cts.CancelAsync();
        //
        //             foreach (var file in savedFiles)
        //             {
        //                 try
        //                 {
        //                     File.Delete(file.Path);
        //                 }
        //                 catch
        //                 {
        //                     // log
        //                 }
        //             }
        //
        //             throw;
        //         }
        //
        //         var fileUpload = new Domain.Model.FileUpload
        //         {
        //             UserId = currentUser.Id,
        //             Created = clock.GetCurrentInstant(),
        //             Files = savedFiles.Select(f => new Domain.Model.File
        //             {
        //                 OriginalName = f.OriginalName,
        //                 ContentType = f.ContentType,
        //                 ContentDisposition = f.ContentDisposition,
        //                 Size = f.Size,
        //                 StorageProvider = "Physical",
        //                 ObjectKey = f.Path
        //             }).ToList()
        //         };
        //
        //         dbContext.FileUploads.Add(fileUpload);
        //         
        //         await dbContext.SaveChangesAsync(cancellationToken);
        //
        //         return TypedResults.Created($"/file-uploads/{fileUpload.Id}", fileUpload);
        //         
        //         
        //     });
        
        static async Task<string?> SaveToDisk(IFormFile file, CancellationToken cancellationToken)
        {
            if (file is { Length: 0 })
            {
                return null;
            }
                    
            Directory.CreateDirectory(StorageRoot);

            var filePath = Path.Combine(StorageRoot, Path.GetRandomFileName());

            await using var stream = System.IO.File.Create(filePath);

            await file.CopyToAsync(stream, cancellationToken);

            return filePath;
        }

        return group;
    }
}

