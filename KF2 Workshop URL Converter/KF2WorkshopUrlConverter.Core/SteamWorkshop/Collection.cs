using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KF2WorkshopUrlConverter.Core.SteamWorkshop
{
    class Collection
    {
        public String ID { get; private set; }
        public String Name { get; private set; }
        public List<Item> Items { get; private set; }

        public Collection() {
            Items = new List<Item>();
        }

        public Collection(String url) : this() => FetchDataFromURL(url);

        public int getNumberOfItems() => Items.Count();

        public void FetchDataFromURL(String Url)
        {
            if (!Url.Contains("steamcommunity.com/sharedfiles/filedetails/?id="))
            {
                throw new Exception("Not a Steam Workshop URL");
            }
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc;
            try
            {
                doc = web.Load(Url);
            }
            catch (UriFormatException)
            {
                throw new UriFormatException("Must contain http:// or https:// on the URL.");
            }
            if (!IsASteamWorkshopCollection(Url))
            {
                throw new Exception("Not a Collection.");
            }

            ID = Regex.Replace(Url, @"[^0-9]", "");
            Name = doc.DocumentNode.SelectNodes("//div[@class='workshopItemTitle']")[0].InnerText;

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='collectionItemDetails']");
            foreach (HtmlNode n in nodes)
            {
                String ItemUrl = n.SelectSingleNode(".//a").Attributes["href"].Value;
                String ItemID = Regex.Replace(ItemUrl, @"[^0-9]", "");
                String ItemName = n.SelectSingleNode(".//div[@class='workshopItemTitle']").InnerText;
                Items.Add(new Item(ItemID, ItemName));
            }
        }

        public static bool IsASteamWorkshopCollection(String Url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc;
            try
            {
                doc = web.Load(Url);
            }
            catch (UriFormatException)
            {
                throw new Exception("Must contain http:// or https:// on the URL.");
            }
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='collectionItemDetails']");
            return nodes != null;
        }

        public String getUrl()
        {
            return $"https://steamcommunity.com/sharedfiles/filedetails/?id={ID}";
        }
    }
}
