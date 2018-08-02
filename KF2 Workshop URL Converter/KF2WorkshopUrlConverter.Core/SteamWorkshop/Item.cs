using System;
using System.Collections.Generic;
using System.Text;

namespace KF2WorkshopUrlConverter.Core.SteamWorkshop
{
    class Item
    {

        public String ID { get; private set; }
        public String Name { get; private set; }

        public Item(String ID, String Name)
        {
            this.ID = ID;
            this.Name = Name;
        }

        public Item(String Url) => FetchDataFromUrl(Url);

        public void FetchDataFromUrl(String Url)
        {

        }

        public static bool IsASteamWorkshopItem(String Url)
        {
            return false;
        }

        public String getUrl()
        {
            return $"https://steamcommunity.com/sharedfiles/filedetails/?id={ID}";
        }
    }
}
