using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.IO.Compression;
using FunctionApp1.Models;
using System.Collections.Generic;

namespace FunctionApp1
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];
            //string type = req.Query["type"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data1 = JsonConvert.DeserializeObject<List<Data>>(requestBody);
            //name = name ?? data?.name;
            //type = type ?? data?.type;

            // Delete all files in a directory    
            string[] files = Directory.GetFiles(@"G:\ZipMade\");
            foreach (string file in files)
            {
                File.Delete(file);
                //Console.WriteLine($"{file} is deleted.");
            }



            //WebClient webClient = new WebClient();

            //foreach (var file in data1)
            //{
            //    webClient.DownloadFile(file.url, @"G:\DownloadAndZip\" + file.Id + "." + file.type);
            //}


            string startpath = @"G:\DownloadAndZip\";
            string zipPath = @"G:\ZipMade\SharePoint"+DateTime.Now.ToShortDateString()+".zip";

            //ZipFile.CreateFromDirectory(startpath, zipPath);

            // Create FileStream for output ZIP archive
            //using (FileStream zipFile = File.Open("compressed_files.zip", FileMode.Create))
            //{
            //    // File to be added to archive
            //    using (FileStream source1 = File.Open("alice29.txt", FileMode.Open, FileAccess.Read))
            //    {
            //        // File to be added to archive
            //        using (FileStream source2 = File.Open("asyoulike.txt", FileMode.Open, FileAccess.Read))
            //        {
            //            using (var archive = new Archive()
            //            {
            //                // Add files to the archive
            //                archive.CreateEntry("alice29.txt", source1);
            //                archive.CreateEntry("asyoulik3.txt", source2);
            //                // ZIP the files
            //                archive.Save(zipFile, new ArchiveSaveOptions() { Encoding = Encoding.ASCII, ArchiveComment = "two files are compressed in this archive" });
            //            }
            //        }
            //    }
            //}


            using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                //string[] filesToZip = Directory.GetFiles(@"G:\DownloadAndZip\");
                List<string> filesToZip = new List<string>();
                foreach(var ff in data1)
                {

                    filesToZip.AddRange(Directory.GetFiles(@"G:\DownloadAndZip\", ff.Id + "." + ff.type));
                }
                

                foreach (string file in filesToZip)
                {
                    archive.CreateEntryFromFile(Path.GetFullPath(file), Path.GetFileName(file));
                }
            }
            

            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(zipPath);
        }
    }
}
