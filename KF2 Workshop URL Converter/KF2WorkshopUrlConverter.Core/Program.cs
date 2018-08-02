using HtmlAgilityPack;
using Mono.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using KF2WorkshopUrlConverter.Core.SteamWorkshop;

namespace KF2WorkshopUrlConverter.Core
{
    class Program
    {
        private static readonly string appVersion = "1.1";
        private static string dllFileName;

        static void Main(string[] args)
        {
            #region List and catch arguments
            bool help = false;
            bool version = false;
            string url = null;
            string path = null;
            dllFileName = Path.GetFileName(Assembly.GetEntryAssembly().Location);
            List<string> extra;

            OptionSet options = new OptionSet() {
                { "u|url=", "The Url of the Workshop Collection. (required)",
                    v => url = v },
                { "o|output=", "Path of a text file to export. (optional)",
                    v => path = v },
                { "v|version", "Version info.",
                    v => version = v != null },
                { "h|help",  "Show this message and exit.",
                    v => help = v != null }
            };
            
            try
            {
                extra = options.Parse(args);
            }
            catch (Exception e)
            {
                ShowError(e);
                return;
            }
            
            if (help)
            {
                ShowHelp(options);
                return;
            }

            if(version)
            {
                ShowVersion();
                return;
            }

            if (url == null)
            {
                ShowError("Missing required option u|url=");
                return;
            }
            #endregion

            #region Program
            Collection collection;
            try
            {
                collection = new Collection(url);
            }
            catch(Exception e)
            {
                ShowError(e.Message);
                return;
            }

            //List Format
            string title = collection.Name;
            string header = $"### {title} ###" + Environment.NewLine + 
                            $"### Coll URL: {url} ###" + Environment.NewLine + 
                            $"### {collection.getNumberOfItems()} Items | Last Query: {DateTime.Now} ###";
            string dotIniUrlFormat = "ServerSubscribedWorkshopItems=";
            string footer = $"## END of {title} ##" + Environment.NewLine;
            var ItemsArray = collection.Items;

            if (path == null)
            {
                Console.WriteLine(header);
            }
            else
            {
                using (FileStream fs = new FileStream(path, new FileInfo(path).Exists ? FileMode.Append : FileMode.OpenOrCreate))
                {
                    using (StreamWriter file = new StreamWriter(fs))
                    {
                        file.WriteLine(header);
                    }
                }
            }

            foreach (Item n in ItemsArray)
            {
                string output = $"{dotIniUrlFormat}{n.ID} # {n.Name}";

                if(path == null)
                {
                    Console.WriteLine(output);
                    if(ItemsArray.IndexOf(n) == collection.getNumberOfItems() - 1)
                    {
                        Console.WriteLine(footer);
                    }
                }
                else
                {
                    using (FileStream fs = new FileStream(path, FileMode.Append))
                    {
                        using (StreamWriter file = new StreamWriter(fs))
                        {
                            file.WriteLine(output);
                            if(ItemsArray.IndexOf(n) == collection.getNumberOfItems() - 1)
                            {
                                file.WriteLine(footer);
                            }
                        }
                    }
                }
            }
            if(path != null)
            {
                Console.WriteLine($"Success! File Saved to \"{path}\"" + Environment.NewLine);
            }
            #endregion
        }

        private static void ShowVersion()
        {
            Console.WriteLine($"KF2 Workshop Collection URL Converter v{appVersion}");
            Console.WriteLine("Project Page: https://github.com/DouglasAntunes/KF2-Workshop-Collection-URL-Converter");
            Console.WriteLine($"Try `dotnet {dllFileName} --help' for more information.");
        }

        private static void ShowHelp(OptionSet p)
        {
            Console.WriteLine($"Usage: dotnet {dllFileName} [OPTIONS]");
            Console.WriteLine("Converts the URL of a Steam Workshop Collection to the format that the file \"PCServer-KFEngine.ini\" accepts.");
            Console.WriteLine("Requires URL on format like https://steamcommunity.com/sharedfiles/filedetails/?id=XXXXXXXXX... (http:// is accepted as well).");
            Console.WriteLine("## For more info or updates, go to https://github.com/DouglasAntunes/KF2-Workshop-Collection-URL-Converter");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        private static void ShowError(Exception e)
        {
            Console.Write($"KF2 Workshop Collection URL Converter v{appVersion}: ");
            Console.WriteLine(e.Message);
            Console.WriteLine($"Try `dotnet {dllFileName} --help' for more information.");
        }

        private static void ShowError(string error)
        {
            Console.Write($"KF2 Workshop Collection URL Converter v{appVersion}: ");
            Console.WriteLine(error);
            Console.WriteLine($"Try `dotnet {dllFileName} --help' for more information.");
        }
    }
}