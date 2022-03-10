using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Simplisity;
using System.Globalization;

namespace DNNrocketAPI.Components
{
    public class CurrencyUtils
    {
        public static bool IsNumeric(string value)
        {
            int i = 0;
            return int.TryParse(value, out i); //i now = value 
        }
        public static int CurrencyConvertCents(string value, string currencyCultureCode)
        {
            var rtn = CurrencyConvertToCulture(value, currencyCultureCode);
            var rtnStr = Regex.Replace(rtn.ToString(), "[^0-9]", ""); // remove ALL non numeric
            if (IsNumeric(rtnStr)) return Convert.ToInt32(rtnStr);
            return 0;
        }
        public static string CurrenyEdit(int intValue, string currencyCultureCode)
        {
            return CurrencyCentsToDollars(intValue, currencyCultureCode).ToString();
        }
        public static string CurrenyDisplay(decimal value, string currencyCultureCode)
        {
            return value.ToString("C", CultureInfo.GetCultureInfo(currencyCultureCode));
        }
        public static decimal CurrencyCentsToDollars(int cents, string currencyCultureCode)
        {
            var currencyData = new CurrencyData(currencyCultureCode);
            var minus = false;
            if (cents < 0) minus = true;

            var multiplyer = "1";
            var lp = 0;
            while (lp < currencyData.CurrencyDecimalDigits)
            {
                multiplyer += "0";
                lp += 1;
            }
            var rtn = Convert.ToDecimal(cents) / Convert.ToDecimal(multiplyer);
            if (minus) rtn = (rtn * -1);
            return CurrencyConvertToCulture(rtn.ToString(), currencyData);
        }
        public static decimal CurrencyConvertToCulture(string value, string currencyCultureCode)
        {
            var currencyData = new CurrencyData(currencyCultureCode);
            return CurrencyConvertToCulture(value, currencyData);
        }
        public static decimal CurrencyConvertToCulture(string value, CurrencyData currencyData)
        {
            // Reformat the amount, to ensure it is a valid currency. 
            // very often the entered decimal seperator is the group seperator.
            // so we convert to try and help, but may still be wrong.  
            // We remove all non-numeric and then enter the decimal seperator at the correct place for the shop currencyculturecode.
            // !!! There is probably a better way to do this !!!
            var minus = false;
            if (value.TrimStart(' ').StartsWith("-")) minus = true;
            if (IsNumeric(value))
            {
                // FIX: 78  --> 78.00
                // no decimal seperator, pad the string to allow for that.
                string padzero = new String('0', currencyData.CurrencyDecimalDigits);
                value = value + padzero;
            }
            else
            {
                // FIX: 78.3  --> 78.30
                // find out how many decimal numbers after seperator.
                // We do not know what the sepeartor is, so take the last non-numeric as the seperator. (Reverse loop, so first in code.)
                var seperatorCount = 0;
                for (int i = 0; i < value.Length; i++)
                {
                    if (!char.IsNumber(value[i])) seperatorCount = (value.Length - i) - 1;
                }
                if (seperatorCount < currencyData.CurrencyDecimalDigits)
                {
                    value = value.PadRight(value.Length + (currencyData.CurrencyDecimalDigits - seperatorCount), '0');
                }
            }
            value = Regex.Replace(value, "[^0-9]", ""); // remove ALL non numeric
            if (value.Length < (currencyData.CurrencyDecimalDigits + 1)) value = value.PadRight((currencyData.CurrencyDecimalDigits + 1), '0');
            var newamount = "";
            for (int i = 0; i < value.Length; i++)
            {
                if ((value.Length - i) == (currencyData.CurrencyDecimalDigits))
                {
                    newamount += currencyData.CurrencyDecimalSeparator;
                }
                newamount += value[i];
            }
            var rtn = Convert.ToDecimal(newamount, CultureInfo.GetCultureInfo(currencyData.CurrencyCultureCode));
            if (minus) rtn = (rtn * -1);
            return rtn;
        }

    }


}
