using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Components
{
    public class W3Utils
    {

        public static Dictionary<string, string> W3colors()
        {
            var rtn = new Dictionary<string, string>();
            var sList = "w3-text-red, w3-text-pink, w3-text-purple, w3-text-deep-purple, w3-text-indigo, w3-text-blue, w3-text-light-blue, w3-text-cyan, w3-text-aqua, w3-text-teal, w3-text-green, w3-text-light-green, w3-text-lime, w3-text-sand, w3-text-khaki, w3-text-yellow, w3-text-amber, w3-text-orange, w3-text-deep-orange, w3-text-blue-gray, w3-text-brown, w3-text-light-gray, w3-text-gray, w3-text-dark-gray, w3-text-black, w3-text-white";
            foreach (string s in sList.Split(','))
            {
                rtn.Add(s.Trim(' '), s.Trim(' '));
            }
            return rtn;
        }

        public static Dictionary<string, string> W3sizes()
        {
            var rtn = new Dictionary<string, string>();
            var sList = "w3-tiny,w3-small,w3-medium,w3-large,w3-xlarge,w3-xxlarge,w3-xxxlarge,w3-jumbo";
            foreach (string s in sList.Split(','))
            {
                rtn.Add(s.Trim(' '), s.Trim(' '));
            }
            return rtn;
        }
        public static Dictionary<string, string> W3displayPosition()
        {
            var rtn = new Dictionary<string, string>();
            var sList = "w3-display-topleft,w3-display-topright,w3-display-bottomleft,w3-display-bottomright,w3-display-left,w3-display-right,w3-display-middle,w3-display-topmiddle,w3-display-bottommiddle";
            foreach (string s in sList.Split(','))
            {
                rtn.Add(s.Trim(' '), s.Trim(' '));
            }
            return rtn;
        }


    }
}
