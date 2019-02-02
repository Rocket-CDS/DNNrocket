using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNNrocket.Category
{

    public class Category : SimplisityInfo 
    {
        private List<Category> _children;

        public SimplisityInfo Info;

        public Category(SimplisityInfo simplisityInfo)
        {
            Info = simplisityInfo;
            _children = new List<Category>();
        }

        public List<Category> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        public bool HasChildren()
        {
            return (_children.Count > 0);
        }

        public bool IsRoot()
        {
            return (Info.ParentItemId == 0);
        }

        public void PopulateChildren(List<SimplisityInfo> simplisityList)
        {
            foreach (var s in simplisityList)
            {
                if (s.ParentItemId == Info.ItemID)
                {
                    var c = new Category(s);
                    c.PopulateChildren(simplisityList);
                    _children.Add(c);
                }
            }
        }
    }


    /// <summary>
    /// Class to hold Category data, so we can use linq and help speed up access from the memory var CategoryList
    /// </summary>
    public class GroupCategoryData
    {
        public int categoryid { get; set; }
        public string categoryref { get; set; }
        public string categoryrefGUIDKey { get; set; }
        public string grouptyperef { get; set; }
        public string attributecode { get; set; }
        public string groupname { get; set; }
        public bool archived { get; set; }
        public bool ishidden { get; set; }
        public int parentcatid { get; set; }
        public Double recordsortorder { get; set; }
        public string imageurl { get; set; }
        public string categoryname { get; set; }
        public string categorydesc { get; set; }
        public string seoname { get; set; }
        public string metadescription { get; set; }
        public string metakeywords { get; set; }
        public string seopagetitle { get; set; }
        public string breadcrumb { get; set; }
        public int depth { get; set; }
        public bool disabled { get; set; }
        public int entrycount { get; set; }
        public string url { get; set; }
        public string message { get; set; }
        public string propertyref { get; set; }
        public bool isdefault { get; set; }
        public bool isvisible
        {
            get
            {
                if (archived) return false;
                if (ishidden) return false;
                return true;
            }
        }

        public List<int> Parents { get; set; }

        public GroupCategoryData()
        {
            Parents = new List<int>();
            isdefault = false;
        }

    }

}
