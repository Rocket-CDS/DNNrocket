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

        public static Dictionary<string,string> GetCategoriesDict(int portalId, string editlang, string systemprovider, bool showdisabled = false, bool showhidden = true, string searchtext = "", bool addEmpty = true)
        {
            var catDict = new Dictionary<string, string>();
            var filter = "";
            if (!showdisabled)
            {
                filter += " and (R1.XMLData.value('(genxml/checkbox/disable)[1]','nvarchar(max)') = 'false') ";
            }
            if (!showhidden)
            {
                filter += " and (R1.XMLData.value('(genxml/checkbox/hidden)[1]','nvarchar(max)') = 'false') ";
            }
            var categoryList = CategoryUtils.GetCategoryList(portalId, editlang, systemprovider, searchtext, filter);

            if (addEmpty)
            {
                catDict.Add("-1", "");
            }

            foreach (var cat in categoryList)
            {
                var catdisplay = cat.Name;
                if (catdisplay == "")
                {
                    catdisplay = cat.Ref;
                }
                catDict.Add(cat.Info.ItemID.ToString(), cat.IndentPrefix() + catdisplay);
            }

            return catDict;
        }

        public static List<Category> GetCategoryList(int portalId, string editlang, string systemprovider, string searchtext = "", string filter = "")
        {
            if (systemprovider != "")
            {
                filter += " and R1.GuidKey = '" + systemprovider + "' ";
            }
            if (filter == "" && searchtext != "")
            {
                filter += " and (categoryname.GuidKey like '%" + searchtext + "%' or categoryref.GuidKey like '%" + searchtext + "%') ";
            }


            var objCtrl = new DNNrocketController();
            var listcount = objCtrl.GetListCount(portalId, -1, "CATEGORY", filter, editlang);
            var list = objCtrl.GetList(portalId, -1, "CATEGORY", filter, editlang, "order by R1.XrefItemId", 0, 0, 0, listcount);

            // create a populated list of categories with children.
            var categoryList = new List<Category>();
            foreach (SimplisityInfo sip in list)
            {
                var c = new Category(sip);
                c.PopulateChildren(list);
                categoryList.Add(c);
            }

            return categoryList;

        }


    }
}
