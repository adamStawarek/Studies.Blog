using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Http;
using File = Google.Apis.Drive.v3.Data.File;

namespace Blog.Helpers
{
    public class DriveApi
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        static string[] Scopes = { DriveService.Scope.Drive };
        static string ApplicationName = "Drive API .NET Quickstart";
        private static readonly DriveService Service;

        static DriveApi()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Debug.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            Service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            FilesResource.ListRequest listRequest = Service.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;
            Debug.WriteLine("Files:");
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    Debug.WriteLine("{0} ({1})", file.Name, file.Id);
                }
            }
            else
            {
                Debug.WriteLine("No files found.");
            }

        }

        public static async Task<string> UploadFile(IFormFile formFile)
        {
            var fileMetadata = new File()
            {
                Name = formFile.FileName,
                Parents = new List<string>
                {
                    GetParentFolderId()
                }
            };

            using (var stream = new MemoryStream())
            {
                await formFile.CopyToAsync(stream);
                stream.Position = 0;
                var extension = Path.GetExtension(formFile.FileName).Replace("jpg", "jpeg");
                var request = Service.Files.Create(
                    fileMetadata, stream, $"image/{extension}");

                request.Fields = "id";
                await request.UploadAsync();

                var file = request.ResponseBody;
                Debug.WriteLine("File ID: " + file.Id);

                return file.Id;
            }          
        }

        private static string GetParentFolderId()
        {
            var pathToApiSettings= "driveApiConfig.txt";
            if (System.IO.File.Exists(pathToApiSettings))
                return System.IO.File.ReadAllText(pathToApiSettings);
            var fileMetadata = new File()
            {
                Name = "Blog",
                MimeType = "application/vnd.google-apps.folder"
            };
            var request = Service.Files.Create(fileMetadata);
            request.Fields = "id";
            var file = request.Execute();
            Debug.WriteLine("Folder ID: " + file.Id);
            System.IO.File.WriteAllText(pathToApiSettings,file.Id);
            return file.Id;
        }
    }    
}
