using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class SessionParams
    {
        /// <summary>
        /// Contruct the SessionParams to control things like, page, pagesize, sort order, etc.
        /// </summary>
        /// <param name="cacheKey">SessionParams is kept in memory, we need a unique cacheKey for each, usually something like moduleid.</param>
        public SessionParams(SimplisityInfo paramInfo)
        {
            Info = new SimplisityInfo(); // need blank record to stop object error.

            Page = 1;

            var rocketsessionkey = paramInfo.GetXmlProperty("genxml/hidden/rocketsessionkey"); // use rocketsessionkey so we don't use Info when new assigned
            if (rocketsessionkey == "") rocketsessionkey = GeneralUtils.GetGuidKey();

            Info = (SimplisityInfo)CacheUtilsDNN.GetCache(rocketsessionkey);
            if (Info == null) Info = new SimplisityInfo();
            RocketSessionKey = rocketsessionkey;

            //Populate any values and overwrites
            foreach (var d in paramInfo.ToDictionary())
            {
                Info.SetXmlProperty("genxml/hidden/" + d.Key, d.Value);
            }
            CacheUtilsDNN.SetCache(rocketsessionkey, Info);
        }
        public SimplisityInfo Info { get; set; }

        public void Update()
        {
            CacheUtilsDNN.SetCache(RocketSessionKey, Info);
        }
        public void Delete()
        {
            CacheUtilsDNN.RemoveCache(RocketSessionKey);
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

        public string RocketSessionKey { get { return Info.GUIDKey; } set { Info.GUIDKey = value.ToString(); } }
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
