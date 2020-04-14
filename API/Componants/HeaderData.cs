using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class HeaderData
    {
        /// <summary>
        /// Contruct the headerdata to control things like, page, pagesize, sort order, etc.
        /// </summary>
        /// <param name="cacheKey">HeaderData is kept in memory, we need a unique cacheKey for each header, usually something like moduleid. All header cahce is kepts in a cache groupid of "headerdata"</param>
        public HeaderData(SimplisityInfo paramInfo)
        {
            Info = new SimplisityInfo(); // need blank record to stop object error.

            Page = 1;

            var headerCacheKey = paramInfo.GetXmlProperty("genxml/hidden/headercachekey"); // use headerCacheKey so we don't use Info when new assigned
            if (headerCacheKey == "") headerCacheKey = GeneralUtils.GetGuidKey();

            Info = (SimplisityInfo)CacheUtilsDNN.GetCache(headerCacheKey);
            if (Info == null) Info = new SimplisityInfo();
            HeaderCacheKey = headerCacheKey;

            //Populate any values and overwrites
            foreach (var d in paramInfo.ToDictionary())
            {
                Info.SetXmlProperty("genxml/hidden/" + d.Key, d.Value);
            }
            CacheUtilsDNN.SetCache(HeaderCacheKey, Info);
        }
        public SimplisityInfo Info { get; set; }

        public void Update()
        {
            CacheUtilsDNN.SetCache(HeaderCacheKey, Info);
        }
        public void Delete()
        {
            CacheUtilsDNN.RemoveCache(HeaderCacheKey);
        }

        public void ActivateItemSort(int itemid)
        {
            SortActivate = itemid;
        }
        public void CancelItemSort()
        {
            SortActivate = 0;
        }

        #region "properties"

        public string HeaderCacheKey { get { return Info.GUIDKey; } set { Info.GUIDKey = value.ToString(); } }
        //Order by
        public int OrderByIndex { get { return Info.GetXmlPropertyInt("genxml/hidden/orderbyindex"); } set { Info.SetXmlProperty("genxml/hidden/orderbyindex", value.ToString()); } }
        public int SortActivate { get { return Info.GetXmlPropertyInt("genxml/hidden/sortorderactivate"); } set { Info.SetXmlProperty("genxml/hidden/sortorderactivate", value.ToString()); } }
        // Paging
        public int PageSize { get { return Info.GetXmlPropertyInt("genxml/hidden/pagesize"); } set { Info.SetXmlProperty("genxml/hidden/pagesize", value.ToString()); } }
        public int Page { get { return Info.GetXmlPropertyInt("genxml/hidden/page"); } set { Info.SetXmlProperty("genxml/hidden/page", value.ToString()); } }
        public int RowCount { get { return Info.GetXmlPropertyInt("genxml/hidden/rowcount"); } set { Info.SetXmlProperty("genxml/hidden/rowcount", value.ToString()); } }

        //Filter SQL
        public int FilterIndex { get { return Info.GetXmlPropertyInt("genxml/hidden/filterindex"); } set { Info.SetXmlProperty("genxml/hidden/filterindex", value.ToString()); } }

        #endregion

    }

}
