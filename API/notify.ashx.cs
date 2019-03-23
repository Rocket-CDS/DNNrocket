using System;
using System.Web;

namespace DNNrocketAPI
{
    /// <summary>
    /// Summary description for XMLconnector
    /// </summary>
    public class Notify : IHttpHandler
    {
        private String _lang = "";




        /// <summary>
        /// This function needs to process and returned message from the bank.
        /// This processing may vary widely between banks.
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            var modCtrl = new NBrightBuyController();
            var info = modCtrl.GetPluginSinglePageData("OSPayPalpayment", "OSPayPalPAYMENT", Utils.GetCurrentCulture());

            try
            {

                var ipn = new PayPalIpnParameters(context.Request);
                var debugMode = info.GetXmlPropertyBool("genxml/checkbox/debug.mode");
                var debugMsg = "START CALL" + DateTime.Now.ToString("s") + " </br>";
                var rtnMsg = "version=2" + Environment.NewLine + "cdr=1";

                // ------------------------------------------------------------------------
                // In this case the payment provider passes back data via form POST.
                // Get the data we need.
                string returnmessage = "";
                int OSPayPalStoreOrderID = 0;
                string OSPayPalCartID = "";
                string OSPayPalClientLang = "";


                if (Utils.IsNumeric(ipn.item_number))
                {

                    var validateUrl = info.GetXmlProperty("genxml/textbox/paymenturl") + "?" + ipn.PostString;

                    // check the record exists
                    debugMsg += "OrderId: " + ipn.item_number + " </br>";
                    var nbi = modCtrl.Get(Convert.ToInt32(ipn.item_number), "ORDER");
                    if (nbi != null)
                    {
                        var orderData = new OrderData(nbi.ItemID);
                        debugMsg += "validateUrl: " + validateUrl + " </br>";
                        if (ProviderUtils.VerifyPayment(ipn, validateUrl))
                        {
                            //set order status to Payed
                            debugMsg += "PaymentOK </br>";
                            orderData.PaymentOk();
                        }
                        else
                        {
                            if (ipn.IsValid)
                            {
                                debugMsg += "NOT VALIDATED BY PAYPAL </br>";
                                //set order status to Not verified
                                orderData.PaymentOk("050");
                            }
                            else
                            {
                                if (orderData.OrderStatus == "020" || orderData.OrderStatus == "010" || orderData.OrderStatus == "030")
                                {
                                    debugMsg += "PAYMENT FAIL </br>";
                                    orderData.PaymentFail();
                                }
                                else
                                {
                                    debugMsg += "INVALID UPDATE ACTION</br>";
                                }
                            }
                        }
                    }
                    else
                    {
                        debugMsg += "ORDER does not exists";
                    }
                    if (debugMode)
                    {
                        info.SetXmlProperty("genxml/debugmsg", debugMsg);
                        modCtrl.Update(info);
                    }
                }

                if (debugMode)
                {
                    debugMsg += "Return Message: " + rtnMsg;
                    info.SetXmlProperty("genxml/debugmsg", debugMsg);
                    modCtrl.Update(info);
                }


                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Write(rtnMsg);
                HttpContext.Current.Response.ContentType = "text/plain";
                HttpContext.Current.Response.CacheControl = "no-cache";
                HttpContext.Current.Response.Expires = -1;
                HttpContext.Current.Response.End();

            }
            catch (Exception ex)
            {
                if (!ex.ToString().StartsWith("System.Threading.ThreadAbortException")) // we expect a thread abort from the End response.
                {
                    info.SetXmlProperty("genxml/debugmsg", "OS_PayPal ERROR: " + ex.ToString());
                    modCtrl.Update(info);
                }
            }


        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


    }
}