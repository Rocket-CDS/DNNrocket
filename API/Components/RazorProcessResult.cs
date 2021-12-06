using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Components
{
    public class RazorProcessResult
    {
        public RazorProcessResult()
        {
            RenderedText = "";
            StatusCode = "00";
            ErrorMsg = "";
        }
        public string RenderedText { set; get; }
        public string StatusCode { set; get; }
        public string ErrorMsg { set; get; }
        public bool IsValid { get { if (StatusCode == "00") return true; else return false; } }
    }
}
