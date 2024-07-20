using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
namespace GDriveDuplicateFinder
{
    public class DuplicatesFinder
    {
        static string[] Scopes = { DriveService.Scope.DriveReadonly };
        static string ApplicationName = "Google Drive Duplicate File Finder";

        public void ReadAndFindDuplicates()
        {
            UserCredential credential;

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // List files.
            ListFiles(service);
        }

        static void ListFiles(DriveService service)
        {
            var request = service.Files.List();
            request.PageSize = 1000;
            request.Fields = "nextPageToken, files(id, name, mimeType, size, md5Checksum)";

            var result = request.Execute();
            var files = result.Files;

            if (files == null || files.Count == 0)
            {
                Console.WriteLine("No files found.");
                return;
            }

            Console.WriteLine("Files:");
            foreach (var file in files)
            {
                Console.WriteLine("{0} ({1}) - {2} bytes - MD5: {3}", file.Name, file.Id, file.Size, file.Md5Checksum);
            }

            // Detect duplicates based on file content (MD5 checksum).
            DetectDuplicates(files);
        }

        static void DetectDuplicates(IList<Google.Apis.Drive.v3.Data.File> files)
        {
            var duplicates = files.GroupBy(f => f.Md5Checksum)
                                  .Where(g => g.Count() > 1)
                                  .SelectMany(g => g);

            Console.WriteLine("\nDuplicate Files:");
            foreach (var file in duplicates)
            {
                Console.WriteLine("{0} ({1}) - {2} bytes - MD5: {3}", file.Name, file.Id, file.Size, file.Md5Checksum);
            }
        }
    }
}