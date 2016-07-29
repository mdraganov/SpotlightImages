using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
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
            Console.WriteLine("Saving spotlight images in: \"" + destinationFolder + "\"");
            Console.Write("Would you like to change the folder? y/n: ");
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

            List<FileInfo> existingLandscapeImages = new List<FileInfo>();
            List<FileInfo> existingPortraitImages = new List<FileInfo>();
            try
            {
                if (!Directory.Exists(this.destinationFolder))
                {
                    Console.WriteLine("\"" + this.destinationFolder + "\" folder created.");
                    Directory.CreateDirectory(this.destinationFolder);
                    Directory.CreateDirectory(this.destinationFolder + "\\Landscape");
                    Directory.CreateDirectory(this.destinationFolder + "\\Portrait");
                }
                else
                {
                    if (!Directory.Exists(this.destinationFolder + "\\Landscape"))
                    {
                        Console.WriteLine("\"" + this.destinationFolder + "\\Landscape" + "\" folder created.");
                        Directory.CreateDirectory(this.destinationFolder + "\\Landscape");
                    }

                    if (!Directory.Exists(this.destinationFolder + "\\Portrait"))
                    {
                        Console.WriteLine("\"" + this.destinationFolder + "\\Portrait" + "\" folder created.");
                        Directory.CreateDirectory(this.destinationFolder + "\\Portrait");
                    }
                }

                existingLandscapeImages = new DirectoryInfo(this.destinationFolder + "\\Landscape").EnumerateFiles().ToList();
                existingPortraitImages = new DirectoryInfo(this.destinationFolder + "\\Portrait").EnumerateFiles().ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "Press enter to exit.");
                Console.ReadLine();
                Environment.Exit(1);
            }

            var copiedImagesCount = 0;
            var existingImagesCount = 0;

            if (!Directory.Exists(this.spotlightImagesFolder))
            {
                Console.WriteLine("\"" + this.destinationFolder + "\" does not exist. Press enter to exit.");
                Console.ReadLine();
                Environment.Exit(1);
            }

            IEnumerable<FileInfo> files = new DirectoryInfo(this.spotlightImagesFolder).EnumerateFiles();

            foreach (var file in files)
            {
                if (existingLandscapeImages.Any(x => x.Name == file.Name + ".jpg") ||
                    existingPortraitImages.Any(x => x.Name == file.Name + ".jpg"))
                {
                    existingImagesCount++;
                    continue;
                }

                try
                {
                    var size = this.GetJpegImageSize(file.FullName);

                    // todo: decide what's portrait and what's landscape
                    if (size.Height < size.Width)
                    {
                        this.CopyFile(file.FullName, destinationFolder + "\\Landscape\\" + file.Name + ".jpg");
                    }
                    else
                    {
                        this.CopyFile(file.FullName, destinationFolder + "\\Portrait\\" + file.Name + ".jpg");
                    }
                }
                catch (Exception)
                {
                    continue;
                }

                copiedImagesCount++;
            }

            Console.WriteLine(copiedImagesCount + " new images copied and " + existingImagesCount + " already existing images skipped. Press enter to exit.");
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
            var appsettingsPath = Directory.GetCurrentDirectory() + "\\appsettings.json";

            if (!File.Exists(appsettingsPath))
            {
                var jsonContent = "{\n\r \"destinationFolder\" : \"" + this.defaultDestinationFolder + "\",\n\r \"spotlightImagesFolder\" : \"" + this.spotlightImagesFolder.Replace('\\', '/') + "\"\n\r }";
                File.WriteAllText(appsettingsPath, jsonContent);
            }

            try
            {
                config = new ConfigurationBuilder().AddJsonFile(appsettingsPath).Build();

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
                Console.WriteLine("Invalid appsettings.json . Press enter to exit.");
                Console.ReadLine();
                Environment.Exit(1);
            }
        }

        // todo: validate json
        private void SetConfiguration(string key, string value)
        {
            this.config[key] = value;
        }

        public Size GetJpegImageSize(string filename)
        {
            FileStream stream = null;
            BinaryReader rdr = null;
            try
            {
                stream = File.OpenRead(filename);
                rdr = new BinaryReader(stream);
                // keep reading packets until we find one that contains Size info
                for (;;)
                {
                    byte code = rdr.ReadByte();
                    if (code != 0xFF) throw new Exception("Unexpected value in file " + filename);
                    code = rdr.ReadByte();
                    switch (code)
                    {
                        // filler byte
                        case 0xFF:
                            stream.Position--;
                            break;
                        // packets without data
                        case 0xD0:
                        case 0xD1:
                        case 0xD2:
                        case 0xD3:
                        case 0xD4:
                        case 0xD5:
                        case 0xD6:
                        case 0xD7:
                        case 0xD8:
                        case 0xD9:
                            break;
                        // packets with size information
                        case 0xC0:
                        case 0xC1:
                        case 0xC2:
                        case 0xC3:
                        case 0xC4:
                        case 0xC5:
                        case 0xC6:
                        case 0xC7:
                        case 0xC8:
                        case 0xC9:
                        case 0xCA:
                        case 0xCB:
                        case 0xCC:
                        case 0xCD:
                        case 0xCE:
                        case 0xCF:
                            ReadBEUshort(rdr);
                            rdr.ReadByte();
                            ushort h = ReadBEUshort(rdr);
                            ushort w = ReadBEUshort(rdr);
                            return new Size(w, h);
                        // irrelevant variable-length packets
                        default:
                            int len = ReadBEUshort(rdr);
                            stream.Position += len - 2;
                            break;
                    }
                }
            }
            finally
            {
                if (rdr != null) rdr.Dispose();
                if (stream != null) stream.Dispose();
            }
        }

        private static ushort ReadBEUshort(BinaryReader rdr)
        {
            ushort hi = rdr.ReadByte();
            hi <<= 8;
            ushort lo = rdr.ReadByte();
            return (ushort)(hi | lo);
        }

    }
}
