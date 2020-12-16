using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;
using DNNrocketAPI.Components;
using DotNetNuke.Entities.Tabs;

namespace DNNrocketAPI.Components
{
    public class MetaSEO
    {
        public MetaSEO()
        {
        }
        public string Title { get; set; }
        public string Description { get; set; }
        public string KeyWords { get; set; }
    }
}
