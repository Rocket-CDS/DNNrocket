using DNNrocketAPI;
using RazorEngine.Text;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace RocketMod
{
    public class RocketModTokens<T> : DNNrocketAPI.render.DNNrocketTokens<T>
    {

        public IEncodedString RenderRocketModFields(SimplisityRazor model)
        {
            var strOut = "";
            var razorTempl = BuidlRocketForm(model);
            strOut = DNNrocketUtils.RazorRender(model, razorTempl, false);
            return new RawString(strOut);
        }

        private string BuidlRocketForm(SimplisityRazor model)
        {
            var moduleData = (ModuleData)model.List.First();
            var objCtrl = new DNNrocketController();
            var strOut = "";
            var fieldInfo = objCtrl.GetByType(moduleData.PortalId, moduleData.ModuleId, "ROCKETMODFIELDS", "", moduleData.CultureCode);
            if (fieldInfo == null)
            {

            }
            return strOut;
        }


    }
}
