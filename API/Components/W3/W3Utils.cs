using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Components
{
    public class W3Utils
    {
        public static Dictionary<string, string> W3textcolors()
        {
            var rtn = new Dictionary<string, string>();
            var sList = ",w3-text-theme, w3-text-red, w3-text-pink, w3-text-purple, w3-text-deep-purple, w3-text-indigo, w3-text-blue, w3-text-light-blue, w3-text-cyan, w3-text-aqua, w3-text-teal, w3-text-green, w3-text-light-green, w3-text-lime, w3-text-sand, w3-text-khaki, w3-text-yellow, w3-text-amber, w3-text-orange, w3-text-deep-orange, w3-text-blue-gray, w3-text-brown, w3-text-light-gray, w3-text-gray, w3-text-dark-gray, w3-text-black, w3-text-white";
            foreach (string s in sList.Split(','))
            {
                rtn.Add(s.Trim(' '), s.Trim(' '));
            }
            return rtn;
        }
        public static Dictionary<string, string> W3colors()
        {
            var rtn = new Dictionary<string, string>();
            var sList = ",w3-theme, w3-red, w3-pink, w3-purple, w3-deep-purple, w3-indigo, w3-blue, w3-light-blue, w3-cyan, w3-aqua, w3-teal, w3-green, w3-light-green, w3-lime, w3-sand, w3-khaki, w3-yellow, w3-amber, w3-orange, w3-deep-orange, w3-blue-gray, w3-brown, w3-light-gray, w3-gray, w3-dark-gray, w3-black, w3-white,w3-pale-red,w3-pale-yellow,w3-pale-green,w3-pale-blue";
            foreach (string s in sList.Split(','))
            {
                rtn.Add(s.Trim(' '), s.Trim(' '));
            }
            return rtn;
        }

        public static Dictionary<string, string> W3sizes()
        {
            var rtn = new Dictionary<string, string>();
            var sList = ",w3-tiny,w3-small,w3-medium,w3-large,w3-xlarge,w3-xxlarge,w3-xxxlarge,w3-jumbo";
            foreach (string s in sList.Split(','))
            {
                rtn.Add(s.Trim(' '), s.Trim(' '));
            }
            return rtn;
        }
        public static Dictionary<string, string> W3displayPosition()
        {
            var rtn = new Dictionary<string, string>();
            var sList = ",w3-display-topleft,w3-display-topright,w3-display-bottomleft,w3-display-bottomright,w3-display-left,w3-display-right,w3-display-middle,w3-display-topmiddle,w3-display-bottommiddle";
            foreach (string s in sList.Split(','))
            {
                rtn.Add(s.Trim(' '), s.Trim(' '));
            }
            return rtn;
        }
        public static Dictionary<string, string> W3alignment()
        {
            var rtn = new Dictionary<string, string>();
            var sList = ",w3-left,w3-center,w3-right";
            foreach (string s in sList.Split(','))
            {
                rtn.Add(s.Trim(' '), s.Trim(' '));
            }
            return rtn;
        }
        public static Dictionary<string, string> W3opacity()
        {
            var rtn = new Dictionary<string, string>();
            var sList = ",w3-opacity-min,w3-opacity,w3-opacity-max,w3-grayscale-min,w3-grayscale,w3-grayscale-max,w3-sepia-min,w3-sepia,w3-sepia-max,w3-hover-opacity-off ";
            foreach (string s in sList.Split(','))
            {
                rtn.Add(s.Trim(' '), s.Trim(' '));
            }
            return rtn;
        }


    }
}
