using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DNNrocketAPI.Components
{
    public class CurrencyData
    {
        public CurrencyData(string currencyCultureCode)
        {
            var cultureInfo = new CultureInfo(currencyCultureCode, false);
            NumberFormatInfo nfi = cultureInfo.NumberFormat;
            CurrencyDecimalDigits = nfi.CurrencyDecimalDigits;
            CurrencyDecimalSeparator = nfi.CurrencyDecimalSeparator;
            CurrencyGroupSeparator = nfi.CurrencyGroupSeparator;
            CurrencySymbol = nfi.CurrencySymbol;
            var ri = new RegionInfo(cultureInfo.LCID);
            CurrencyCode = ri.ISOCurrencySymbol;
            CurrencyCultureCode = currencyCultureCode;
        }
        public int CurrencyDecimalDigits { set; get; }
        public string CurrencyDecimalSeparator { set; get; }
        public string CurrencyGroupSeparator { set; get; }
        public string CurrencySymbol { set; get; }
        public string CurrencyCode { set; get; }
        public string CurrencyCultureCode { set; get; }

    }
}
