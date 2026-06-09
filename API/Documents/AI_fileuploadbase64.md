# Simplicity Base64 File Upload - Developer Guide

## Purpose
This document explains how to implement file and image uploads using the Simplicity Base64 upload system in the DNNrocket framework. This guide is designed for both AI assistants and human developers to understand the complete upload pattern, including client-side HTML and server-side C# implementation.

---

## System Overview

The Simplicity Base64 upload system provides an automatic file upload mechanism that:
- Converts selected files to base64 encoding on the client side
- Sends files via AJAX to the server
- Processes files on the server with C# handlers
- Returns HTML to display results dynamically

**Key Features:**
- Automatic base64 encoding
- Multiple file upload support
- Progress tracking capability
- No form submission required
- Seamless integration with Simplicity framework

---

## Quick Start - Minimal Example

### Client-Side HTML
```html
<!-- Hidden file input with Simplicity attributes -->
<input 
  id="myFileUpload" 
  class="simplisity_base64upload" 
  type="file" 
  s-cmd="mycommand_upload" 
  s-post="#formData" 
  s-return="#results" 
  s-fields='{"itemid":"123"}' 
  style="display:none;">

<!-- Visible upload button -->
<button type="button" onclick="$('#myFileUpload').click();">
  Choose Files
</button>

<!-- Results display area -->
<div id="results"></div>

<!-- Form data container (referenced by s-post) -->
<div id="formData">
  <input type="hidden" name="itemid" value="123">
</div>
```

NOTE: The s-return attr on the input is optional, it depends on the html being returned.  Usually the s-return is not added and the html return goes into the default elelment #simplisity_startpanel.

### Server-Side C# Handler
```csharp
// In your API Controller ProcessCommand method
case "mycommand_upload":
    strOut = HandleFileUpload();
    break;

// Handler implementation
private string HandleFileUpload()
{
    // 1. Get parameters
    var itemId = _paramInfo.GetXmlPropertyInt("genxml/hidden/itemid");

    // 2. Get uploaded file data
    var fileNames = _postInfo.GetXmlProperty("genxml/hidden/fileuploadlist");
    var fileData = _postInfo.GetXmlProperty("genxml/hidden/fileuploadbase64");

    // 3. Check if files exist
    if (string.IsNullOrEmpty(fileData))
        return "<div>No files uploaded</div>";

    // 4. Split multiple files (separated by *)
    var nameArray = fileNames.Split('*');
    var dataArray = fileData.Split('*');

    // 5. Save each file
    for (int i = 0; i < nameArray.Length; i++)
    {
        if (!string.IsNullOrEmpty(nameArray[i]))
        {
            var fileName = nameArray[i];
            var base64Data = dataArray[i];
            var bytes = Convert.FromBase64String(base64Data);
            var savePath = Path.Combine("your/upload/path", fileName);
            File.WriteAllBytes(savePath, bytes);
        }
    }

    // 6. Return HTML response
    return "<div>Files uploaded successfully!</div>";
}
```

---

## How It Works

### Client-Side Flow
1. **User clicks button** → File picker dialog opens
2. **User selects files** → Files are automatically converted to base64
3. **Files are posted** → AJAX sends data to server via Simplicity framework
4. **Server responds** → HTML response is injected into the `s-return` container

### Server-Side Flow
1. **Command routing** → `ProcessCommand` method receives `s-cmd` value
2. **Data extraction** → Get `fileuploadlist` (filenames) and `fileuploadbase64` (file data)
3. **File processing** → Decode base64 and save files to disk
4. **Response** → Return HTML to display in browser

### Data Flow Diagram
```
User → Button Click → File Input
                         ↓
                   Base64 Encoding
                         ↓
                   AJAX POST (Simplicity)
                         ↓
              Server ProcessCommand(s-cmd)
                         ↓
              Extract fileuploadlist + fileuploadbase64
                         ↓
              Decode Base64 → Save Files
                         ↓
              Return HTML
                         ↓
              Display in s-return Container
```

---

## Required HTML Attributes

### Essential Attributes

| Attribute | Required | Description | Example |
|-----------|----------|-------------|---------|
| `class` | **Yes** | Must be `simplisity_base64upload` to activate handler | `class="simplisity_base64upload"` |
| `type` | **Yes** | Must be `file` | `type="file"` |
| `s-cmd` | **Yes** | Server command name (routes to ProcessCommand) | `s-cmd="article_addimage"` |
| `s-post` | **Yes** | CSS selector for form data container | `s-post="#formData"` |
| `s-return` | **Yes** | CSS selector where server response displays | `s-return="#imageContainer"` |
| `s-fields` | **Yes** | JSON object with required parameters | `s-fields='{"articleid":"123"}'` |

### Important: HTML Structure Pattern

**The file input does NOT need to be inside the s-post container.**

✅ **Correct Pattern:**
```html
<!-- File input (can be anywhere) -->
<input id="fileUpload" class="simplisity_base64upload" 
       s-post="#formData" s-return="#results" ...>

<!-- Form data container (referenced by s-post) -->
<div id="formData">
  <input name="articleid" value="123">
</div>

<!-- Results container (referenced by s-return) -->
<div id="results"></div>
```

❌ **Not Required (but won't break):**
```html
<!-- File input doesn't need to be inside the form -->
<div id="formData">
  <input id="fileUpload" class="simplisity_base64upload" s-post="#formData"...>
  <input name="articleid" value="123">
</div>
```

**Key Points:**
- `s-post` - **WHERE TO FIND** form data to send with the upload
- `s-return` - **WHERE TO DISPLAY** the server response
- The file input itself can be located anywhere in the DOM

### Optional Attributes

| Attribute | Description | Example |
|-----------|-------------|---------|
| `s-reload` | Reload page after upload (`true`/`false`) | `s-reload="false"` |
| `multiple` | Allow multiple file selection | `multiple` |
| `accept` | File type filter | `accept="image/*"` or `accept=".pdf,.doc"` |
| `s-list` | Additional data lists to send | `s-list=".imagelist,.doclist"` |
| `style` | Recommended to hide input | `style="display:none;"` |

---

## Server-Side Implementation

### Command Routing Pattern

In your API controller (e.g., `StartConnect.cs`, `ArticleConnect.cs`), implement command routing:

```csharp
public string ProcessCommand(string cmd, SimplisityInfo systemInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
{
    var strOut = "";

    // Initialize your data objects
    _postInfo = postInfo;
    _paramInfo = paramInfo;

    switch (cmd.ToLower())
    {
        case "article_addimage":
            strOut = AddArticleImage();
            break;
        case "article_uploaddoc":
            strOut = AddArticleDocument();
            break;
        // ... other commands
    }

    return strOut;
}
```

### Data Extraction Methods

#### Getting Upload Data
```csharp
// Get comma or asterisk-separated list of filenames
var fileNames = _postInfo.GetXmlProperty("genxml/hidden/fileuploadlist");

// Get base64-encoded file data (separated by asterisks for multiple files)
var fileData = _postInfo.GetXmlProperty("genxml/hidden/fileuploadbase64");
```

#### Getting Parameters from s-fields
```csharp
// From s-fields='{"articleid":"123", "categoryid":"456"}'
var articleId = _paramInfo.GetXmlPropertyInt("genxml/hidden/articleid");
var categoryId = _paramInfo.GetXmlPropertyInt("genxml/hidden/categoryid");
var someText = _paramInfo.GetXmlProperty("genxml/hidden/sometext");
```

#### Getting Form Data from s-post Container
```csharp
// From <input name="title" value="My Title">
var title = _postInfo.GetXmlProperty("genxml/textbox/title");

// From <input name="description" value="Description text">
var description = _postInfo.GetXmlProperty("genxml/textbox/description");

// From <input type="hidden" name="itemid" value="789">
var itemId = _postInfo.GetXmlPropertyInt("genxml/hidden/itemid");
```

### Complete Upload Handler Examples

#### Example 1: Simple Image Upload
```csharp
public string AddArticleImage()
{
    // 1. Get article ID from s-fields parameter
    var articleId = _paramInfo.GetXmlPropertyInt("genxml/hidden/articleid");

    // 2. Get uploaded file data from postInfo
    var fileNames = _postInfo.GetXmlProperty("genxml/hidden/fileuploadlist");
    var fileData = _postInfo.GetXmlProperty("genxml/hidden/fileuploadbase64");

    // 3. Validate upload
    if (string.IsNullOrEmpty(fileData))
    {
        return "<div class='error'>No files were uploaded</div>";
    }

    // 4. Split multiple files
    var nameArray = fileNames.Split('*');
    var dataArray = fileData.Split('*');

    // 5. Create destination directory
    var destDir = PortalUtils.ImageFolderMapPath() + "\\" + articleId;
    if (!Directory.Exists(destDir))
        Directory.CreateDirectory(destDir);

    // 6. Save each file
    var savedFiles = new List<string>();
    for (int i = 0; i < nameArray.Length; i++)
    {
        if (!string.IsNullOrEmpty(nameArray[i]))
        {
            var fileName = nameArray[i];
            var base64Data = dataArray[i];
            var fileBytes = Convert.FromBase64String(base64Data);
            var filePath = Path.Combine(destDir, fileName);

            File.WriteAllBytes(filePath, fileBytes);
            savedFiles.Add(fileName);

            // Optional: Save to database or XML
            // articleData.AddImage(fileName);
        }
    }

    // 7. Return HTML response
    var html = "<div class='upload-success'>";
    html += $"<p>Successfully uploaded {savedFiles.Count} image(s)</p>";
    foreach (var file in savedFiles)
    {
        html += $"<img src='/Images/{articleId}/{file}' width='100' />";
    }
    html += "</div>";

    return html;
}
```

#### Example 2: Image Upload with DNNrocket Helper Method
```csharp
public string AddArticleImage()
{
    // 1. Get parameters
    var articleId = _paramInfo.GetXmlPropertyInt("genxml/hidden/articleid");
    var articleData = GetActiveArticle(articleId);

    // 2. Save any form data first
    articleData.Save(_postInfo);

    // 3. Get uploaded files
    var fileuploadlist = _postInfo.GetXmlProperty("genxml/hidden/fileuploadlist");
    var fileuploadbase64 = _postInfo.GetXmlProperty("genxml/hidden/fileuploadbase64");

    // 4. Process base64 images using DNNrocket helper
    if (!string.IsNullOrEmpty(fileuploadbase64))
    {
        var filenameList = fileuploadlist.Split('*');
        var filebase64List = fileuploadbase64.Split('*');

        // Set up paths
        var baseFileMapPath = PortalUtils.TempDirectoryMapPath() + "\\" + GeneralUtils.GetGuidKey();
        var destDir = _dataObject.PortalContent.ImageFolderMapPath + "\\" + articleData.ArticleId;

        // Create directory if needed
        if (!Directory.Exists(destDir))
            Directory.CreateDirectory(destDir);

        // Get image resize setting
        var imgsize = _postInfo.GetXmlPropertyInt("genxml/hidden/imageresize");
        if (imgsize == 0)
            imgsize = _dataObject.PortalContent.ImageResize;

        // Upload and resize images
        var imgList = RocketUtils.ImgUtils.UploadBase64Image(
            filenameList, 
            filebase64List, 
            baseFileMapPath, 
            destDir, 
            imgsize
        );

        // Add each image to article data
        foreach (var imgFileMapPath in imgList)
        {
            articleData.AddImage(Path.GetFileName(imgFileMapPath));
        }

        // Store in data object for rendering
        _dataObject.SetDataObject("articledata", articleData);
    }

    // 5. Render response using Razor template
    var razorTempl = GetSystemTemplate("Articleimages.cshtml");
    var pr = RenderRazorUtils.RazorProcessData(
        razorTempl, 
        articleData, 
        _dataObject.DataObjects, 
        _dataObject.Settings, 
        _sessionParams, 
        true
    );

    if (pr.ErrorMsg != "")
        return pr.ErrorMsg;

    return pr.RenderedText;
}
```

#### Example 3: Document Upload
```csharp
public string AddArticleDocument()
{
    // 1. Get parameters
    var articleId = _paramInfo.GetXmlPropertyInt("genxml/hidden/articleid");

    // 2. Get uploaded files
    var fileNames = _postInfo.GetXmlProperty("genxml/hidden/fileuploadlist");
    var fileData = _postInfo.GetXmlProperty("genxml/hidden/fileuploadbase64");

    if (string.IsNullOrEmpty(fileData))
        return "<p>No documents selected</p>";

    // 3. Split files
    var nameArray = fileNames.Split('*');
    var dataArray = fileData.Split('*');

    // 4. Set up document folder
    var docDir = PortalUtils.DocumentFolderMapPath() + "\\" + articleId;
    if (!Directory.Exists(docDir))
        Directory.CreateDirectory(docDir);

    // 5. Save documents
    var uploadedDocs = new List<string>();
    for (int i = 0; i < nameArray.Length; i++)
    {
        if (!string.IsNullOrEmpty(nameArray[i]))
        {
            var fileName = GeneralUtils.SanitizeFileName(nameArray[i]);
            var base64Data = dataArray[i];
            var bytes = Convert.FromBase64String(base64Data);
            var filePath = Path.Combine(docDir, fileName);

            File.WriteAllBytes(filePath, bytes);
            uploadedDocs.Add(fileName);

            // Save document reference
            // articleData.AddDocument(fileName);
        }
    }

    // 6. Return document list
    var html = "<div class='document-list'>";
    html += "<h4>Uploaded Documents</h4><ul>";
    foreach (var doc in uploadedDocs)
    {
        html += $"<li><a href='/Documents/{articleId}/{doc}'>{doc}</a></li>";
    }
    html += "</ul></div>";

    return html;
}
```

---

## Complete Working Examples

**Note:** In all examples below, notice that the file input is separate from the form data container. The `s-post` attribute points to the container with form data, but the file input itself doesn't need to be inside it. This is the standard pattern used in DNNrocket.

### Example 1: Product Image Upload

#### HTML (Client-Side)
```html
<div id="productUploadSection">
    <h3>Upload Product Images</h3>

    <!-- Upload Button -->
    <button type="button" class="btn-upload" onclick="$('#productImageUpload').click();">
        <i class="fa fa-upload"></i> Select Images
    </button>

    <!-- Hidden File Input -->
    <input 
        id="productImageUpload" 
        class="simplisity_base64upload" 
        type="file" 
        accept="image/*" 
        multiple 
        s-cmd="product_addimages" 
        s-post="#productFormData" 
        s-return="#productImageGallery" 
        s-reload="false" 
        s-fields='{"productid":"@Model.ProductId", "categoryid":"@Model.CategoryId"}' 
        style="display:none;">

    <!-- Form Data Container -->
    <div id="productFormData">
        <input type="hidden" name="productid" value="@Model.ProductId">
        <input type="hidden" name="categoryid" value="@Model.CategoryId">
        <input type="hidden" name="imageresize" value="1200">
    </div>

    <!-- Image Gallery Display -->
    <div id="productImageGallery" class="image-gallery">
        <!-- Uploaded images will appear here -->
    </div>
</div>
```

#### C# Server Handler
```csharp
// In StartConnect.cs or ProductConnect.cs
public string ProcessCommand(string cmd, SimplisityInfo systemInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
{
    var strOut = "";
    _postInfo = postInfo;
    _paramInfo = paramInfo;

    switch (cmd.ToLower())
    {
        case "product_addimages":
            strOut = AddProductImages();
            break;
    }

    return strOut;
}

private string AddProductImages()
{
    // Get product ID and category ID from s-fields
    var productId = _paramInfo.GetXmlPropertyInt("genxml/hidden/productid");
    var categoryId = _paramInfo.GetXmlPropertyInt("genxml/hidden/categoryid");

    // Get image resize setting from form
    var imageResize = _postInfo.GetXmlPropertyInt("genxml/hidden/imageresize");
    if (imageResize == 0) imageResize = 1200; // default

    // Get uploaded file data
    var fileNames = _postInfo.GetXmlProperty("genxml/hidden/fileuploadlist");
    var fileData = _postInfo.GetXmlProperty("genxml/hidden/fileuploadbase64");

    if (string.IsNullOrEmpty(fileData))
        return "<p class='error'>No images selected</p>";

    // Split multiple files
    var nameArray = fileNames.Split('*');
    var dataArray = fileData.Split('*');

    // Create product image directory
    var uploadPath = Server.MapPath($"~/Images/Products/{productId}/");
    if (!Directory.Exists(uploadPath))
        Directory.CreateDirectory(uploadPath);

    // Save images
    var savedImages = new List<string>();
    for (int i = 0; i < nameArray.Length; i++)
    {
        if (!string.IsNullOrEmpty(nameArray[i]))
        {
            var fileName = GeneralUtils.SanitizeFileName(nameArray[i]);
            var base64Data = dataArray[i];
            var bytes = Convert.FromBase64String(base64Data);

            // Save original
            var filePath = Path.Combine(uploadPath, fileName);
            File.WriteAllBytes(filePath, bytes);

            // Optional: Create thumbnail
            var thumbPath = Path.Combine(uploadPath, "thumb_" + fileName);
            ImageUtils.ResizeImage(filePath, thumbPath, 200, 200);

            savedImages.Add(fileName);
        }
    }

    // Build HTML response
    var html = new StringBuilder();
    html.Append("<div class='upload-success'>");
    html.Append($"<p>Successfully uploaded {savedImages.Count} image(s)</p>");
    html.Append("<div class='image-grid'>");

    foreach (var img in savedImages)
    {
        html.Append("<div class='image-item'>");
        html.Append($"<img src='/Images/Products/{productId}/{img}' alt='{img}' />");
        html.Append($"<p>{img}</p>");
        html.Append("</div>");
    }

    html.Append("</div></div>");
    return html.ToString();
}
```

### Example 2: User Avatar Upload (Single Image)

#### HTML
```html
<div id="avatarSection">
    <div id="currentAvatar">
        <img src="@Model.AvatarUrl" alt="Avatar" width="150">
    </div>

    <button type="button" onclick="$('#avatarUpload').click();">
        Change Avatar
    </button>

    <input 
        id="avatarUpload" 
        class="simplisity_base64upload" 
        type="file" 
        accept="image/*" 
        s-cmd="user_updateavatar" 
        s-post="#userForm" 
        s-return="#currentAvatar" 
        s-reload="false" 
        s-fields='{"userid":"@Model.UserId"}' 
        style="display:none;">

    <div id="userForm">
        <input type="hidden" name="userid" value="@Model.UserId">
    </div>
</div>
```

#### C# Handler
```csharp
case "user_updateavatar":
    strOut = UpdateUserAvatar();
    break;

private string UpdateUserAvatar()
{
    var userId = _paramInfo.GetXmlPropertyInt("genxml/hidden/userid");

    var fileNames = _postInfo.GetXmlProperty("genxml/hidden/fileuploadlist");
    var fileData = _postInfo.GetXmlProperty("genxml/hidden/fileuploadbase64");

    if (string.IsNullOrEmpty(fileData))
        return "<p>No image selected</p>";

    // Get first file only (single upload)
    var nameArray = fileNames.Split('*');
    var dataArray = fileData.Split('*');

    if (nameArray.Length > 0 && !string.IsNullOrEmpty(nameArray[0]))
    {
        var fileName = $"avatar_{userId}.jpg";
        var bytes = Convert.FromBase64String(dataArray[0]);

        var avatarPath = Server.MapPath($"~/Images/Avatars/");
        if (!Directory.Exists(avatarPath))
            Directory.CreateDirectory(avatarPath);

        var filePath = Path.Combine(avatarPath, fileName);
        File.WriteAllBytes(filePath, bytes);

        // Resize to standard size
        ImageUtils.ResizeImage(filePath, filePath, 150, 150);

        // Update user record
        // userService.UpdateAvatarUrl(userId, $"/Images/Avatars/{fileName}");

        return $"<img src='/Images/Avatars/{fileName}?v={DateTime.Now.Ticks}' alt='Avatar' width='150'>";
    }

    return "<p>Upload failed</p>";
}
```

### Example 3: Article Document Upload with List

#### HTML
```html
<div id="documentSection">
    <h4>Article Documents</h4>

    <button type="button" onclick="$('#docUpload').click();">
        <i class="fa fa-file"></i> Upload Documents
    </button>

    <input 
        id="docUpload" 
        class="simplisity_base64upload" 
        type="file" 
        accept=".pdf,.doc,.docx,.xls,.xlsx" 
        multiple 
        s-cmd="article_uploaddocs" 
        s-post="#articleData" 
        s-return="#documentList" 
        s-reload="false" 
        s-fields='{"articleid":"@articleData.ArticleId"}' 
        style="display:none;">

    <div id="articleData">
        <input type="hidden" name="articleid" value="@articleData.ArticleId">
        <input type="hidden" name="maxfilesize" value="10485760"><!-- 10MB -->
    </div>

    <div id="documentList">
        @* Existing documents will be displayed here *@
    </div>
</div>
```

#### C# Handler
```csharp
case "article_uploaddocs":
    strOut = AddArticleDocuments();
    break;

private string AddArticleDocuments()
{
    var articleId = _paramInfo.GetXmlPropertyInt("genxml/hidden/articleid");
    var maxFileSize = _postInfo.GetXmlPropertyInt("genxml/hidden/maxfilesize");

    var fileNames = _postInfo.GetXmlProperty("genxml/hidden/fileuploadlist");
    var fileData = _postInfo.GetXmlProperty("genxml/hidden/fileuploadbase64");

    if (string.IsNullOrEmpty(fileData))
        return "<p>No documents selected</p>";

    var nameArray = fileNames.Split('*');
    var dataArray = fileData.Split('*');

    var docPath = Server.MapPath($"~/Documents/Articles/{articleId}/");
    if (!Directory.Exists(docPath))
        Directory.CreateDirectory(docPath);

    var html = new StringBuilder();
    html.Append("<ul class='document-list'>");

    for (int i = 0; i < nameArray.Length; i++)
    {
        if (!string.IsNullOrEmpty(nameArray[i]))
        {
            var fileName = GeneralUtils.SanitizeFileName(nameArray[i]);
            var bytes = Convert.FromBase64String(dataArray[i]);

            // Check file size
            if (maxFileSize > 0 && bytes.Length > maxFileSize)
            {
                html.Append($"<li class='error'>{fileName} - File too large (max {maxFileSize / 1024 / 1024}MB)</li>");
                continue;
            }

            var filePath = Path.Combine(docPath, fileName);
            File.WriteAllBytes(filePath, bytes);

            // Get file icon based on extension
            var extension = Path.GetExtension(fileName).ToLower();
            var icon = GetFileIcon(extension);

            // Format file size
            var fileSize = FormatFileSize(bytes.Length);

            html.Append("<li>");
            html.Append($"<i class='fa {icon}'></i> ");
            html.Append($"<a href='/Documents/Articles/{articleId}/{fileName}' target='_blank'>{fileName}</a>");
            html.Append($"<span class='filesize'>({fileSize})</span>");
            html.Append("</li>");
        }
    }

    html.Append("</ul>");
    return html.ToString();
}

private string GetFileIcon(string extension)
{
    switch (extension)
    {
        case ".pdf": return "fa-file-pdf";
        case ".doc":
        case ".docx": return "fa-file-word";
        case ".xls":
        case ".xlsx": return "fa-file-excel";
        default: return "fa-file";
    }
}

private string FormatFileSize(long bytes)
{
    if (bytes < 1024) return $"{bytes} B";
    if (bytes < 1048576) return $"{bytes / 1024} KB";
    return $"{bytes / 1048576} MB";
}
```

---

## Common Patterns and Use Cases

### Pattern 1: Image Gallery with Multiple Upload
```html
<input 
  class="simplisity_base64upload" 
  type="file" 
  accept="image/*" 
  multiple 
  s-cmd="gallery_addimages" 
  s-post="#galleryForm" 
  s-return="#imageGallery">
```

### Pattern 2: Single File Upload with Validation
```html
<input 
  class="simplisity_base64upload" 
  type="file" 
  accept=".pdf" 
  s-cmd="contract_uploadpdf" 
  s-post="#contractForm" 
  s-return="#pdfPreview">
```

### Pattern 3: Profile Photo with Crop
```html
<input 
  class="simplisity_base64upload" 
  type="file" 
  accept="image/*" 
  s-cmd="profile_updatephoto" 
  s-post="#profileData" 
  s-return="#photoDisplay" 
  s-fields='{"userid":"123", "cropsize":"200"}'>
```

### Pattern 4: Document Upload with Category
```html
<input 
  class="simplisity_base64upload" 
  type="file" 
  accept=".pdf,.doc,.docx" 
  multiple 
  s-cmd="document_upload" 
  s-post="#docForm" 
  s-return="#docList" 
  s-fields='{"categoryid":"456", "projectid":"789"}'>
```

---

## Best Practices

### Client-Side Best Practices

1. **Always hide the file input**
   ```html
   <input type="file" style="display:none;">
   ```

2. **Use descriptive IDs**
   ```html
   <input id="productImageUpload" ...>
   <input id="userAvatarUpload" ...>
   ```

3. **Specify file types**
   ```html
   accept="image/*"  <!-- All images -->
   accept=".pdf,.doc,.docx"  <!-- Specific types -->
   ```

4. **Set reload appropriately**
   ```html
   s-reload="false"  <!-- Use for AJAX updates -->
   s-reload="true"   <!-- Use when full page refresh needed -->
   ```

5. **Use multiple for batch uploads**
   ```html
   <input multiple ...>  <!-- Allow multiple files -->
   ```

### Server-Side Best Practices

1. **Always validate input**
   ```csharp
   if (string.IsNullOrEmpty(fileData))
       return "<p>No files uploaded</p>";

   if (articleId <= 0)
       return "<p>Invalid article ID</p>";
   ```

2. **Sanitize filenames**
   ```csharp
   var safeFileName = GeneralUtils.SanitizeFileName(fileName);
   var safeFileName = Path.GetFileName(fileName); // Remove path
   ```

3. **Create directories safely**
   ```csharp
   if (!Directory.Exists(uploadPath))
       Directory.CreateDirectory(uploadPath);
   ```

4. **Handle exceptions**
   ```csharp
   try
   {
       File.WriteAllBytes(filePath, bytes);
   }
   catch (Exception ex)
   {
       LogUtils.LogException(ex);
       return "<p>Upload failed</p>";
   }
   ```

5. **Return meaningful HTML**
   ```csharp
   // Good: Shows what was uploaded
   return "<div><img src='/path/to/image.jpg'><p>Upload complete</p></div>";

   // Bad: No feedback
   return "";
   ```

6. **Check file sizes**
   ```csharp
   var bytes = Convert.FromBase64String(base64Data);
   if (bytes.Length > maxFileSize)
       return $"<p>File too large (max {maxFileSize}MB)</p>";
   ```

7. **Use secure paths**
   ```csharp
   // Good: Controlled path
   var uploadPath = Server.MapPath("~/Uploads/");

   // Bad: User-controlled path
   var uploadPath = userInput; // Security risk!
   ```

---

## Troubleshooting Guide

### Common Issues and Solutions

#### Issue: Files not uploading
**Solution:**
- Check `class="simplisity_base64upload"` is present
- Verify `s-cmd` matches server-side case statement
- Check browser console for JavaScript errors

#### Issue: Server command not executing
**Solution:**
- Verify command name in `s-cmd` matches `ProcessCommand` case
- Check spelling and casing
- Ensure `ProcessCommand` method is public

#### Issue: No response displayed
**Solution:**
- Verify `s-return` selector exists in DOM
- Check that handler returns HTML string
- Inspect browser network tab for response

#### Issue: Multiple files not working
**Solution:**
- Add `multiple` attribute to input
- Split data using `*` separator: `nameArray.Split('*')`
- Loop through all array elements

#### Issue: File data is empty
**Solution:**
- Check `s-post` selector is correct
- Verify `_postInfo` is initialized
- Use correct XML path: `genxml/hidden/fileuploadbase64`

#### Issue: Invalid base64 string error
**Solution:**
- Check data is not corrupted
- Verify complete base64 string is received
- Handle empty strings before decoding

---

## Testing Checklist

### Client-Side Testing
- [ ] File input is hidden from view
- [ ] Upload button triggers file picker
- [ ] File picker accepts correct file types
- [ ] Multiple files can be selected (if `multiple` attribute)
- [ ] `s-cmd` attribute is set correctly
- [ ] `s-post` selector points to valid element
- [ ] `s-return` selector points to valid element
- [ ] `s-fields` contains valid JSON

### Server-Side Testing
- [ ] Command routing works (ProcessCommand receives correct cmd)
- [ ] `fileuploadlist` is extracted correctly
- [ ] `fileuploadbase64` is extracted correctly
- [ ] Parameters from `s-fields` are retrieved
- [ ] Form data from `s-post` is retrieved
- [ ] Files are decoded from base64
- [ ] Files are saved to correct location
- [ ] Directory is created if not exists
- [ ] Filenames are sanitized
- [ ] HTML response is returned
- [ ] Response displays in `s-return` container

### Security Testing
- [ ] File types are validated
- [ ] File sizes are checked
- [ ] Filenames are sanitized
- [ ] Upload paths are secure
- [ ] User permissions are verified
- [ ] No path traversal vulnerabilities

---

## Integration with DNNrocket Framework

### Using Framework Methods

The DNNrocket framework provides helper methods for common upload scenarios:

#### Image Upload with Resize
```csharp
var imgList = RocketUtils.ImgUtils.UploadBase64Image(
    filenameList,       // string[] of filenames
    filebase64List,     // string[] of base64 data
    baseFileMapPath,    // Temp path for processing
    destDir,            // Final destination directory
    imgsize             // Max dimension (width/height)
);
```

#### Portal Content Paths
```csharp
// Get portal image folder
var imageFolder = _dataObject.PortalContent.ImageFolderMapPath;

// Get portal document folder
var docFolder = _dataObject.PortalContent.DocFolderMapPath;

// Get temp directory
var tempPath = PortalUtils.TempDirectoryMapPath();
```

#### Article/Category Methods
```csharp
// Add image to article
articleData.AddImage(fileName);

// Add document to article
articleData.AddDocument(fileName);

// Save article data
articleData.Save(_postInfo);

// Clear cache after changes
articleData.ClearCache();
```

---

## Advanced Scenarios

### Scenario 1: Upload with Progress Bar

For large file uploads, add a progress bar:

```html
<div id="progressBar" style="display:none;">
    <div class="progress">
        <div class="progress-bar" style="width: 0%">0%</div>
    </div>
</div>

<script>
// Show progress during upload
$('#myFileUpload').on('change', function() {
    $('#progressBar').show();
    $('.progress-bar').css('width', '50%').text('Uploading...');
});

// Hide after completion
$(document).on('simplisitypostgetcompleted', function() {
    $('#progressBar').hide();
});
</script>
```

### Scenario 2: Upload with Validation

Add client-side validation before upload:

```html
<script>
function validateUpload() {
    var fileInput = document.getElementById('myFileUpload');
    var files = fileInput.files;

    if (files.length === 0) {
        alert('Please select a file');
        return false;
    }

    // Check file size (5MB limit)
    var maxSize = 5 * 1024 * 1024;
    for (var i = 0; i < files.length; i++) {
        if (files[i].size > maxSize) {
            alert('File ' + files[i].name + ' is too large (max 5MB)');
            return false;
        }
    }

    return true;
}

// Trigger validation before upload
$('#uploadButton').click(function() {
    if (validateUpload()) {
        $('#myFileUpload').click();
    }
});
</script>
```

### Scenario 3: Upload with Callback Function

Execute custom code after upload completes:

```html
<input 
  class="simplisity_base64upload" 
  s-cmd="article_addimage" 
  s-after="refreshGallery,updateCounter">

<script>
function refreshGallery() {
    console.log('Gallery refreshed');
    // Custom gallery refresh logic
}

function updateCounter() {
    var count = $('#imageGallery img').length;
    $('#imageCount').text(count + ' images');
}
</script>
```

---

## API Reference

### Client-Side Attributes

```html
<input 
  id="unique-id"
  class="simplisity_base64upload"
  type="file"
  name="file[]"
  accept="mime-type"
  multiple
  s-cmd="command_name"
  s-cmdurl="/api/endpoint"
  s-post="#form-selector"
  s-return="#result-selector"
  s-fields='{"key":"value"}'
  s-list=".list1,.list2"
  s-reload="true|false"
  s-after="function1,function2"
  s-before="validateFunction"
  s-showloader="true|false"
  s-timeout="120000"
  style="display:none;">
```

### Server-Side Data Paths

```csharp
// Upload data
_postInfo.GetXmlProperty("genxml/hidden/fileuploadlist")
_postInfo.GetXmlProperty("genxml/hidden/fileuploadbase64")

// Parameters from s-fields
_paramInfo.GetXmlProperty("genxml/hidden/{fieldname}")
_paramInfo.GetXmlPropertyInt("genxml/hidden/{fieldname}")

// Form data from s-post
_postInfo.GetXmlProperty("genxml/textbox/{inputname}")
_postInfo.GetXmlProperty("genxml/hidden/{inputname}")
_postInfo.GetXmlPropertyInt("genxml/textbox/{inputname}")
```

---

## Summary

### For AI Assistants

When implementing file uploads in DNNrocket/Simplicity:

1. **Client-side**: Add hidden file input with `class="simplisity_base64upload"` and required `s-*` attributes
2. **HTML Structure**: File input does NOT need to be inside the `s-post` container - they are separate elements
3. **Command routing**: Add case in `ProcessCommand` method matching `s-cmd` value
4. **Data extraction**: Get `fileuploadlist` and `fileuploadbase64` from `_postInfo`
5. **Processing**: Split by `*`, decode base64, save files
6. **Response**: Return HTML to display in `s-return` container

### For Human Developers

The Simplicity Base64 upload system provides:
- Automatic file-to-base64 conversion
- AJAX-based upload without form submission
- Server-side C# processing
- Dynamic HTML response injection
- Multiple file support
- Integration with DNNrocket framework

**Key Points:**
- Always use `class="simplisity_base64upload"`
- File input is separate from the form data container
- `s-post` tells Simplicity WHERE to find form data
- `s-return` tells Simplicity WHERE to display results
- Match `s-cmd` to server-side case statement
- Extract data from correct XML paths
- Return HTML for dynamic display
- Validate and sanitize all inputs

---

## Additional Resources

### Related Documentation
- Large File Upload: `AI_LargeFileUpload.md`
- Simplicity Framework: `Simplisity/README.md`
- Image Processing: DNNrocket ImageUtils documentation

### Example Files
- `ArticleImageSelect.cshtml` - Image upload UI example
- `ArticleConnect.cs` - Server-side upload handlers
- `StartConnect.cs` - Command routing examples

---

**Document Version:** 1.0  
**Last Updated:** 2025  
**Framework:** DNNrocket Simplicity  
**Target Framework:** .NET Framework 4.7.2, .NET Standard 2.0
