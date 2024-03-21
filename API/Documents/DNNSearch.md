﻿# DNN Search
The DNN search can be used by rocket modules.  

**Before starting ensure the "Search: Site Crawler " is active on the DNN scheduler.** 

## RocketContent
The xpath of the fields required are defined by using the dependancy file of the AppTheme. [Dependancies](https://docs.rocket-cds.org/integration/dependancies)   
Each row in the module will have it's own entry in the search results.  The header data will be used if a row title is empty.  

*NOTE: To stop RocketContent from indexing remove the "sqlindex" nodes from the AppTheme dependacy file.*

## RocketDirectory
RocketDirectory or any sub-systems of RocketDirectory will index each article.  The data fields for the search are pre-defined by the ArticleLimpet class.

```
title = articleData.Name ("genxml/lang/genxml/textbox/articlename")
bodydata = articleData.Summary ("genxml/lang/genxml/textbox/articlesummary")
description = articleData.RichText ("genxml/lang/genxml/textbox/articlerichtext")
```
To activate the DNN search index, define a search module in the system Admin settings.

If extra data fields are required they can be added to the "DNN Search extra" field in Admin settings.  The values are the xpath of the data in the article XML, in a CSV format for multiple fields.  The extra fields are added onto the end of the search summary field.  

*NOTE: To stop RocketDirectory or sub-systems from indexing do not select a search module in the Admin settings*

## Re-Index 
The re-index of the system can be done from the system Admin panel.
