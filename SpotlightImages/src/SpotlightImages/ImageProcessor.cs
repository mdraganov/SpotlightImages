using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SpotlightImages
{
    public class ImageProcessor
    {
        private string spotlightImagesFolder;
        private string defaultDestinationFolder;
        private string destinationFolder;
        private IConfigurationRoot config;

        public ImageProcessor(string spotlightImagesFolder, string defaultDestinationFolder)
        {
            this.spotlightImagesFolder = spotlightImagesFolder;
            this.defaultDestinationFolder = defaultDestinationFolder;
            this.GetConfiguration();
        }

        public void RetrieveImages()
        {
            Console.WriteLine("Saving spotlight images in: " + destinationFolder);
            Console.Write("Would you like to change the location? y/n: ");
            var input = Console.ReadLine();

            while (input != "n" && input != "N" && input != "y" && input != "Y")
            {
                Console.Write("Invalid input. Please enter y or n: ");
                input = Console.ReadLine();
            }
            
            // todo: validate input
            if (input == "y" || input == "Y")
            {
                Console.Write("Please enter new location path: ");
                this.destinationFolder = Console.ReadLine().Replace('\\', '/');

                this.SetConfiguration("destinationFolder", destinationFolder);
            }

            List<FileInfo> filesInDestination;
            try
            {
                filesInDestination = new DirectoryInfo(this.destinationFolder).EnumerateFiles().ToList();
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid destination path. Press enter to exit.");
                Console.ReadLine();
                return;
            }
            
            var copiedImagesCount = 0;
            var existingImagesCount = 0;
            IEnumerable<FileInfo> files = new DirectoryInfo(this.spotlightImagesFolder).EnumerateFiles();

            foreach (var file in files)
            {
                if (filesInDestination.Any(x => x.Name == file.Name + ".jpg"))
                {
                    existingImagesCount++;
                    continue;
                }

                this.CopyFile(file.FullName, destinationFolder + "\\" + file.Name + ".jpg");
                copiedImagesCount++;
            }

            Console.WriteLine(copiedImagesCount + " new images copied and " + existingImagesCount + " images already existing. Press enter to exit.");
            Console.ReadLine();
        }

        // todo: handle exceptions
        private void CopyFile(string sourceFileName, string destFileName)
        {
            File.Copy(sourceFileName, destFileName);
        }

        // todo: spotlightImagesFolder key
        private void GetConfiguration()
        {
            if (!Directory.GetFiles(Directory.GetCurrentDirectory()).Contains("appsettings.json"))
            {
                var jsonContent = "{\n\r \"destinationFolder\" : \"" + this.defaultDestinationFolder + "\",\n\r \"spotlightImagesFolder\" : \"" + this.spotlightImagesFolder.Replace('\\', '/') + "\"\n\r }";
                File.WriteAllText(Directory.GetCurrentDirectory() + "\\appsettings.json", jsonContent);
            }

            try
            {
                config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                if (!config.AsEnumerable().Any(x => x.Key == "destinationFolder"))
                {
                    this.SetConfiguration("destinationFolder", this.defaultDestinationFolder);
                }

                this.destinationFolder = config["destinationFolder"];
            }
            //catch (FileNotFoundException)
            //{
            //    Console.WriteLine("Missing appsettings.json");
            //    Console.ReadLine();
            //    return;
            //}
            catch (Exception)
            {
                Console.WriteLine("Invalid appsettings.json");
                Console.ReadLine();
                Environment.Exit(1);
            }
        }

        // todo: validate json
        private void SetConfiguration(string key, string value)
        {
            config[key] = value;
        }
    }
}
