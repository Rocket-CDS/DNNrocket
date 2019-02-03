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

        public int Level
        {
            get { return Info.GetXmlPropertyInt("genxml/level"); }
            set { Info.SetXmlProperty("genxml/level", value.ToString()); }
        }

        public string IndentPrefix(char prefixChar = '.')
        {
            return new String(prefixChar, Level);
        }

        public bool HasChildren
        {
            get { return Children.Any(); }
        }

        public bool IsRoot
        {
            get { return (Info.ParentItemId == 0); }
        }

        public string Name
        {
            get { return Info.GetXmlProperty("genxml/lang/genxml/textbox/name"); }
        }

        public string Ref
        {
            get { return Info.GetXmlProperty("genxml/textbox/ref"); }
        }

        public bool IsHidden
        {
            get { return Info.GetXmlPropertyBool("genxml/checkbox/hidden"); }
        }
        public bool IsDisabled
        {
            get { return Info.GetXmlPropertyBool("genxml/checkbox/disable"); }
        }

    }

}
