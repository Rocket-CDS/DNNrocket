# DNN Search
The DNN search can be used by rocket modules.  

## RocketContent
The xpath of the fields required are defined by using the dependancy file of the AppTheme. [Dependancies](https://docs.rocket-cds.org/integration/dependancies)   
Each row in the module will have it's own entry in the search results.  The header data will be used if a row title is empty.  

*NOTE: To stop RocketContent from indexing remove the "sqlindex" nodes from the AppTheme dependacy file.*

## RocketDirectory
RocketDirectory or any sub-systems of RocketDirectory will index each article.  The data fields ofr hte search are pre-defined by the ArticleLimpet class.

```
title = articleData.Name ("genxml/lang/genxml/textbox/articlename")
bodydata = articleData.Summary ("genxml/lang/genxml/textbox/articlesummary")
description = articleData.RichText ("genxml/lang/genxml/textbox/articlerichtext")
```

To activate the DNN search index, define a search module in the system Admin settings.

*NOTE: To stop RocketDirectory or sub-systems from indexing do not select a search module in the Admin settings*

## Langauges
Languages in the DNN search are grouped into a single search database.   