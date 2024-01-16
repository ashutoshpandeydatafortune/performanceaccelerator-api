using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace DF_EvolutionAPI.Utils
{
    public class FileUtil
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FileUtil(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public string GetFileContent(string path)
        {
            return File.ReadAllText(path);
        }

        public string GetTemplateContent(string templateName)
        {
            var templateFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Templates", templateName);

            if (File.Exists(templateFilePath))
            {
                return GetFileContent(templateFilePath);
            }

            throw new FileNotFoundException(templateName + " not found.");

        }
    }
}
