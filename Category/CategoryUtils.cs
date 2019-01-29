using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DNNrocket.Category
{
    public static class CategoryUtils
    {

        public static Dictionary<int, string> BuildCatList(int displaylevels = 20, Boolean showHidden = false, Boolean showArchived = false, int parentid = 0, string catreflist = "", string prefix = "", bool displayCount = false, bool showEmpty = true, string groupref = "", string breadcrumbseparator = ">", string lang = "",bool usecache = true)
        {
            if (lang == "") lang = DNNrocketUtils.GetCurrentCulture();

            var rtnDic = new Dictionary<int, string>();

            var strCacheKey = "NBrightBuy_BuildCatList" + DNNrocketUtils.GetPortalId() + "*" + displaylevels + "*" + showHidden.ToString(CultureInfo.InvariantCulture) + "*" + showArchived.ToString(CultureInfo.InvariantCulture) + "*" + parentid + "*" + catreflist + "*" + prefix + "*" + DNNrocketUtils.GetCurrentCulture() + "*" + showEmpty + "*" + displayCount + "*" + groupref + "*" + lang;

            var objCache = CacheUtils.GetCache(strCacheKey);

            if (objCache == null || usecache)
            {
                var grpCatCtrl = new GrpCatController(lang);

                var d = new Dictionary<int, string>();
                var rtnList = new List<GroupCategoryData>();
                rtnList = grpCatCtrl.GetTreeCategoryList(rtnList, 0, parentid, groupref, breadcrumbseparator);
                var strCount = "";
                foreach (var grpcat in rtnList)
                {
                    if (displayCount) strCount = " (" + grpcat.entrycount.ToString("") + ")";

                    if (grpcat.depth < displaylevels)
                    {
                        if (showEmpty || grpcat.entrycount > 0)
                        {
                            if (grpcat.ishidden == false || showHidden)
                            {
                                var addprefix = new string(' ', grpcat.depth).Replace(" ", prefix);
                                if (catreflist == "")
                                    rtnDic.Add(grpcat.categoryid, addprefix + grpcat.categoryname + strCount);
                                else
                                {
                                    if (grpcat.categoryref != "" &&
                                        (catreflist + ",").Contains(grpcat.categoryref + ","))
                                    {
                                        rtnDic.Add(grpcat.categoryid, addprefix + grpcat.categoryname + strCount);
                                    }
                                }
                            }
                        }
                    }
                }
                CacheUtils.SetCache(strCacheKey, rtnDic);

            }
            else
            {
                rtnDic = (Dictionary<int, string>)objCache;
            }
            return rtnDic;
        }


        public static List<GroupCategoryData> GetTreeCategoryList(List<GroupCategoryData> rtnList, int level, int parentid, string groupref, string breadcrumbseparator)
        {
            if (level > 20) return rtnList; // stop infinate loop

            var levelList = GetGrpCategories(parentid, groupref);
            foreach (GroupCategoryData tInfo in levelList)
            {
                var nInfo = tInfo;
                nInfo.breadcrumb = GetBreadCrumb(nInfo.categoryid, 50, breadcrumbseparator, false);
                nInfo.depth = level;
                rtnList.Add(nInfo);
                if (groupref == "" || groupref == "cat") GetTreeCategoryList(rtnList, level + 1, tInfo.categoryid, groupref, breadcrumbseparator);
            }

            return rtnList;
        }

    }
}
