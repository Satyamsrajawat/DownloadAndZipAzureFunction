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


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data1 = JsonConvert.DeserializeObject<List<Data>>(requestBody);
            

            // Delete all files in a directory    
            string[] files = Directory.GetFiles(@"C:\DownloadAndZip\Downloads");
            foreach (string file in files)
            {

                File.Delete(file);
            }

         

            WebClient webClient = new WebClient();

            foreach(var file in data1)
            {
                webClient.DownloadFile(file.url, @"C:\DownloadAndZip\Downloads\" + file.name+"."+file.type);
            }


            string date = DateTime.Now.ToShortDateString();

            string startpath = @"C:\DownloadAndZip\Downloads\";
            string zipPath = @"C:\DownloadAndZip\Zip\Sharepoint_"+date+".zip";

            ZipFile.CreateFromDirectory(startpath, zipPath);


            return new OkObjectResult(zipPath);
        }
    }
}
