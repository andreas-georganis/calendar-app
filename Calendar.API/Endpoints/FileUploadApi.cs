namespace CalendarApp.API.Endpoints;

public static class FileUploadApi
{
    public static RouteGroupBuilder MapFileUploadApi(this RouteGroupBuilder group)
    {
        group.MapPost("/upload", async (HttpRequest request) =>
        {
            if (!request.HasFormContentType)
            {
                return Results.BadRequest("The request must be a multipart/form-data.");
            }

            var form = await request.ReadFormAsync();
            var file = form.Files.GetFile("file");

            if (file == null)
            {
                return Results.BadRequest("No file was uploaded. Please include a file with the name 'file'.");
            }

            // Here you would typically save the file to storage and return its URL or identifier.
            // For demonstration purposes, we'll just return the file name and size.

            return Results.Ok(new
            {
                FileName = file.FileName,
                Size = file.Length
            });
        }).WithName("UploadFile").WithTags("File Upload");

        return group;
    }
}
