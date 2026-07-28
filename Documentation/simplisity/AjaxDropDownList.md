# Ajax DropDown List Example

DropDownLists can be populated by Ajax. This example shows 2 dropdownlists: a country list and a region list. When a country is selected, the second list is populated with region data.

```
<div>
<label>@ResourceKey("businessdirectory.country")</label>
@{
var countrydata = DNNrocket.Country.CountryUtils.CountryListCSV("amylisbusiness");
}
@DropDownList(info, "genxml/dropdownlist/country", "," + countrydata[0].Replace("'",""), "," + countrydata[1].Replace("'", ""), "class='w3-input w3-border simplisity_change' s-cmd='settingcountry_getregion' s-dropdownlist='#region'")
</div>
<div>
<label>@ResourceKey("businessdirectory.region")</label>
@{
var regiondata = DNNrocket.Country.CountryUtils.RegionListCSV(info.GetXmlProperty("genxml/dropdownlist/country"));
}
@DropDownList(info, "genxml/dropdownlist/region", regiondata[0].Replace("'",""), regiondata[1].Replace("'", ""), "class='w3-input w3-border' ")
</div>
```

> NOTE: use `s-fields='{"allowempty":"true"}'` to allow an empty selection in the dropdown.

The client-side display is a server Razor template. The country dropdown is created by a Razor token `@DropDownList` and the initial load populates the dropdown using the server-side `CountryListCSV` function.

The `simplisity_change` class on the country dropdown identifies that a Simplisity change event will be triggered when a country is selected. The `s-dropdownlist` command attribute is defined as the jQuery selector for the region dropdownlist; this identifies the dropdownlist that will be populated by Ajax.

On initial load we populate the region dropdownlist with the regions for the selected country and the Razor token `@DropDownList` deals with the selected value.

The `s-dropdownlist` command attribute requires a JSON object to be returned by the server. The server returns a JSON string and Simplisity converts it into a JSON object, which is used to populate the region dropdownlist.

#### JSON Return Format
```
{listkey: ["'key1','key2','key3'"], listvalue: ["'name1','name2','name3'"] }
```

The JSON return data should have 2 lists: `"listkey"` and `"listvalue"`. The server-side code should place the JSON string into the return dictionary with a key value of `"outputjson"`.

This functionality uses the `"activevalue"` param passed to the server by Simplisity and **must** have server-side code that returns the correct values.

```
...
case "settingcountry_getregion":
	rtnDic.Add("outputhtml", "");
	var regionlist = CountryUtils.RegionListJson(paramInfo.GetXmlProperty("genxml/hidden/activevalue"), paramInfo.GetXmlPropertyBool("genxml/hidden/allowempty"));
	rtnDic.Add("outputjson", regionlist);
	break;
...
```

```
public static object RegionListJson(string countrycode, bool allowempty = true)
{
	var jsonList = new List<ValuePair>();
	var valuePair = new ValuePair();
	if (allowempty)
	{
		valuePair.Key = "";
		valuePair.Value = "";
		jsonList.Add(valuePair);
	}
	foreach (var i in DNNrocketUtils.GetRegionList(countrycode))
	{
		valuePair = new ValuePair();
		valuePair.Key = i.Key;
		valuePair.Value = i.Value;
		jsonList.Add(valuePair);
	}
	return jsonList;
}
```
