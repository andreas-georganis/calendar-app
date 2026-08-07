namespace Calendar.API.Endpoints;

// todo: implement folder API endpoints
public static class FolderApi
{
    public static RouteGroupBuilder MapFolderApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/folders").RequireAuthorization();


        // group.MapPost("", async Task<Created<Domain.Model.File>> (
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
       //         var fileUpload = new Domain.Model.File
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
       //                 Uri = f.Path
       //             }).ToList()
       //         };
       //
       //         dbContext.Files.Add(fileUpload);
       //         
       //         await dbContext.SaveChangesAsync(cancellationToken);
       //
       //         return TypedResults.Created($"/file-uploads/{fileUpload.Id}", fileUpload);
       //         
       //         
       //     });

        return group;
    }
}
