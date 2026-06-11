# Large File Upload Process Documentation

## Overview
This document explains the pattern for implementing large file uploads in the DNNrocket system. It covers web.config configuration, client-side upload handling, progress tracking, and server-side command execution after upload completion.

## Purpose
Enable AI assistants to understand and implement/modify large file upload functionality that:
1. Allows users to upload large files (e.g., ZIP files for import)
2. Tracks upload progress with visual feedback
3. Executes server-side commands after successful upload
4. Handles web.config requirements for large file sizes and long execution times

---

## Architecture Components

### 1. Web.config Configuration Requirements

Large file uploads require two critical web.config settings:

#### httpRuntime Settings
```xml
<system.web>
  <httpRuntime maxRequestLength="102400" executionTimeout="3600" />
</system.web>
```

**Settings Explained:**
- **maxRequestLength**: Maximum file upload size in KB (e.g., 102400 KB = 100 MB)
- **executionTimeout**: Maximum execution time in seconds (e.g., 3600 = 60 minutes)

#### Reading Current Web.config Values (Server-Side C#)
```csharp
using System.Web.Configuration;

int maxUploadSizeMB = 100; // Default
int executionTimeoutMinutes = 60; // Default

try
{
    var config = WebConfigurationManager.OpenWebConfiguration("~");
    var httpRuntimeSection = (HttpRuntimeSection)config.GetSection("system.web/httpRuntime");

    if (httpRuntimeSection != null)
    {
        // maxRequestLength is in KB, convert to MB
        maxUploadSizeMB = httpRuntimeSection.MaxRequestLength / 1024;

        // executionTimeout is in seconds, convert to minutes
        executionTimeoutMinutes = (int)httpRuntimeSection.ExecutionTimeout.TotalSeconds / 60;
    }
}
catch (Exception ex)
{
    LogUtils.LogException(ex);
}
```

**Key Points:**
- Always wrap in try-catch to handle configuration read errors
- Provide sensible defaults (100 MB, 60 minutes)
- Convert units appropriately (KB→MB, seconds→minutes)

---

### 2. Client-Side HTML Structure

#### File Input Element
```html
<input id="importfileupload" class="w3-hide" type="file" name="files[]" 
       style="display:none;" accept=".zip">
```

**Properties:**
- `id`: Unique identifier for JavaScript targeting
- `type="file"`: Standard file input
- `name="files[]"`: Array notation for server-side processing
- `accept=".zip"`: File type restriction (optional, can be changed or removed)
- `class="w3-hide"` + `style="display:none;"`: Hidden from view

#### Upload Trigger Button
```html
<span class="w3-button w3-teal fileuploadaction" 
      onclick="$('#importfileupload').trigger('click');">
    Upload Select Import File
</span>
```

**Purpose:** Provides a styled button that triggers the hidden file input

#### Progress Bar Modal
```html
<div id="simplisity-file-progress-bar" class="w3-modal w3-padding" 
     style="z-index:9999;background-color:grey;display:none;">
    <div class="w3-modal-content w3-card-4 w3-display-middle w3-padding" style="width: 60%;">
        <div class="w3-center">
            <h4 id="progessaction">Uploading File...</h4>
            <div class="w3-row w3-light-grey w3-margin-top">
                <div class="w3-blue simplisity-file-progress-bar" 
                     style="width:0%;height:24px;line-height:24px;text-align:center;">0%</div>
            </div>
        </div>
    </div>
</div>
```

**Properties:**
- Modal overlay with high z-index (9999) to appear above all content
- Progress bar div with class `simplisity-file-progress-bar` for JavaScript targeting
- Initially hidden (`display:none;`)
- Width as percentage for visual progress indication

#### Server Command Trigger Element
```html
<div id="processimportupload" 
     s-cmd="rockettools_importprebuildexe" 
     s-timeout="900000" 
     s-post="#sectiondata" 
     s-return="#toolsmodalcontainer" 
     class="w3-hide"></div>
```

**Custom Attributes (Simplisity Framework):**
- `s-cmd`: Server-side command name to execute after upload
- `s-timeout`: Command timeout in milliseconds (900000 = 15 minutes)
- `s-post`: jQuery selector for data to post with command
- `s-return`: jQuery selector where server response should be rendered
- `class="w3-hide"`: Hidden element, only used for configuration

**IMPORTANT**: When modifying existing code, always check what server command name already exists and use that exact name in the `s-cmd` attribute. Do not invent new command names unless you are also creating the corresponding server-side handler.

---

### 3. JavaScript Upload Implementation

#### Complete Upload Handler Pattern
```javascript
$(document).ready(function() {
    $('#importfileupload').on('change', function(e) {
        var files = e.target.files;
        if (files.length === 0) return;
        
        var file = files[0];
        var formData = new FormData();
        formData.append('files[]', file);
        
        // Show progress bar
        $('#simplisity-file-progress-bar').show();
        $('.simplisity-file-progress-bar').css('width', '0%').text('0%');
        
        // Create XMLHttpRequest
        var xhr = new XMLHttpRequest();
        
        // Track upload progress
        xhr.upload.addEventListener('progress', function(e) {
            if (e.lengthComputable) {
                var percentComplete = Math.round((e.loaded / e.total) * 100);
                $('.simplisity-file-progress-bar').css('width', percentComplete + '%').text(percentComplete + '%');
            }
        }, false);
        
        // Handle completion
        xhr.addEventListener('load', function() {
            console.log('Upload complete, status:', xhr.status);
            $('#simplisity-file-progress-bar').hide();

            if (xhr.status === 200) {
                // Upload successful, trigger the server command
                console.log('Upload successful, triggering command...');
                simplisity_callserver($('#processimportupload'));
            } else if (xhr.status === 413) {
                alert('Upload failed: File is too large');
            } else {
                alert('Upload failed with status: ' + xhr.status);
            }
        }, false);
        
        // Handle errors
        xhr.addEventListener('error', function() {
            console.error('Upload error');
            $('#simplisity-file-progress-bar').hide();
            alert('Upload failed');
        }, false);
        
        // Handle abort
        xhr.addEventListener('abort', function() {
            console.log('Upload aborted');
            $('#simplisity-file-progress-bar').hide();
        }, false);
        
        // Send the request
        xhr.open('POST', '/Desktopmodules/dnnrocket/api/fileupload/upload', true);
        xhr.send(formData);
    });
});
```

#### Step-by-Step Breakdown

**Step 1: File Selection Event**
```javascript
$('#importfileupload').on('change', function(e) {
    var files = e.target.files;
    if (files.length === 0) return;
```
- Triggered when user selects a file
- Validates that a file was actually selected

**Step 2: Prepare FormData**
```javascript
var file = files[0];
var formData = new FormData();
formData.append('files[]', file);
```
- Create FormData object for multipart/form-data encoding
- Append file with array notation `files[]`

**Step 3: Initialize Progress Bar**
```javascript
$('#simplisity-file-progress-bar').show();
$('.simplisity-file-progress-bar').css('width', '0%').text('0%');
```
- Display progress modal
- Reset progress bar to 0%

**Step 4: Create XMLHttpRequest**
```javascript
var xhr = new XMLHttpRequest();
```
- Use XMLHttpRequest instead of jQuery AJAX for progress tracking capability

**Step 5: Track Upload Progress**
```javascript
xhr.upload.addEventListener('progress', function(e) {
    if (e.lengthComputable) {
        var percentComplete = Math.round((e.loaded / e.total) * 100);
        $('.simplisity-file-progress-bar').css('width', percentComplete + '%').text(percentComplete + '%');
    }
}, false);
```
- Monitor `xhr.upload.progress` event
- Calculate percentage: `(loaded / total) * 100`
- Update progress bar width and text in real-time

**Step 6: Handle Upload Completion**
```javascript
xhr.addEventListener('load', function() {
    $('#simplisity-file-progress-bar').hide();

    if (xhr.status === 200) {
        simplisity_callserver($('#processimportupload'));
    } else if (xhr.status === 413) {
        alert('Upload failed: File is too large');
    } else {
        alert('Upload failed with status: ' + xhr.status);
    }
}, false);
```
- Hide progress bar when upload completes
- Check HTTP status code:
  - **200**: Success → trigger server command
  - **413**: Payload Too Large → notify user
  - **Other**: General error → notify user

**Step 7: Handle Errors**
```javascript
xhr.addEventListener('error', function() {
    $('#simplisity-file-progress-bar').hide();
    alert('Upload failed');
}, false);
```
- Network errors, timeouts, etc.
- Hide progress bar and notify user

**Step 8: Send Request**
```javascript
xhr.open('POST', '/Desktopmodules/dnnrocket/api/fileupload/upload', true);
xhr.send(formData);
```
- POST to file upload API endpoint
- Third parameter `true` = asynchronous

---

### 4. Server-Side Command Execution

After successful file upload, the JavaScript triggers a server-side command:

```javascript
simplisity_callserver($('#processimportupload'));
```

This calls the Simplisity framework function with the configuration element:
- **Command**: `rockettools_importprebuildexe` (from `s-cmd` attribute)
- **Timeout**: 900000 ms = 15 minutes (from `s-timeout` attribute)
- **Post Data**: Data from `#sectiondata` element (from `s-post` attribute)
- **Response Target**: `#toolsmodalcontainer` (from `s-return` attribute)

#### Server-Side Command Implementation Pattern

The server command should:
1. Locate the uploaded file in the temporary upload directory
2. Process the file (e.g., extract ZIP, import data)
3. Provide progress feedback if possible
4. Return success/failure status

**Example Command Structure (C#):**
```csharp
public class RocketToolsInterface
{
    public string rockettools_importprebuildexe(Simplisity.SimplisityInfo systemInfo)
    {
        try
        {
            // Get uploaded file path
            var uploadPath = DNNrocketUtils.MapPath("/portals/0/rocketuploads/");
            var zipFiles = Directory.GetFiles(uploadPath, "*.zip");
            
            if (zipFiles.Length == 0)
            {
                return "Error: No upload file found";
            }
            
            var zipFile = zipFiles[0];
            
            // Process file (extract, import, etc.)
            // ... processing logic ...
            
            // Clean up
            File.Delete(zipFile);
            
            // Update status
            systemInfo.SetSetting("importprebuild", "true");
            
            return RenderTemplate("prebuildimport.cshtml");
        }
        catch (Exception ex)
        {
            LogUtils.LogException(ex);
            return "Error: " + ex.Message;
        }
    }
}
```

---

## Implementation Checklist

When implementing or modifying a large file upload feature:

### Configuration
- [ ] Verify web.config has appropriate `maxRequestLength` (in KB)
- [ ] Verify web.config has appropriate `executionTimeout` (in seconds)
- [ ] Consider if settings need to be configurable via UI

### HTML Elements
- [ ] Hidden file input with unique ID
- [ ] Trigger button to open file dialog
- [ ] Progress bar modal with proper styling
- [ ] Server command trigger element with all required attributes
- [ ] **IMPORTANT**: If modifying existing code, use the existing server command name in `s-cmd` attribute - do not change it unless you are also updating the server-side command handler

### JavaScript
- [ ] File input change event handler
- [ ] FormData preparation with correct field name
- [ ] XMLHttpRequest creation (not jQuery AJAX)
- [ ] Progress event listener with percentage calculation
- [ ] Load event listener with status code handling
- [ ] Error event listener
- [ ] Proper API endpoint URL
- [ ] Server command trigger on success

### Server-Side
- [ ] File upload API endpoint configured
- [ ] Command handler implemented
- [ ] Timeout sufficient for processing
- [ ] Error handling and logging
- [ ] File cleanup after processing
- [ ] Status feedback to user

### Testing
- [ ] Test with small file (< 1 MB)
- [ ] Test with large file (near maxRequestLength limit)
- [ ] Test with file exceeding maxRequestLength (should get 413 error)
- [ ] Test with slow connection (verify progress updates)
- [ ] Test server command execution after upload
- [ ] Test error scenarios (network failure, server error, timeout)

---

## Common HTTP Status Codes

- **200 OK**: Upload successful
- **400 Bad Request**: Invalid request format
- **413 Payload Too Large**: File exceeds maxRequestLength
- **500 Internal Server Error**: Server-side processing error
- **504 Gateway Timeout**: Upload exceeded executionTimeout

---

## Customization Points

### File Type Restrictions
Change the `accept` attribute:
```html
<!-- ZIP only -->
<input accept=".zip">

<!-- Images only -->
<input accept="image/*">

<!-- Multiple types -->
<input accept=".zip,.xml,.json">

<!-- No restriction -->
<input>
```

### Upload Endpoint
Change the API endpoint URL:
```javascript
xhr.open('POST', '/your/custom/upload/endpoint', true);
```

### Progress Bar Styling
Modify CSS classes and inline styles:
```html
<div class="w3-blue simplisity-file-progress-bar" 
     style="width:0%;height:24px;line-height:24px;text-align:center;">0%</div>
```

### Server Command Configuration
Modify attributes on the command trigger element:
```html
<div id="processimportupload" 
     s-cmd="your_custom_command" 
     s-timeout="1800000" 
     s-post="#yourdata" 
     s-return="#yourcontainer" 
     class="w3-hide"></div>
```

### Timeout Values
- **s-timeout**: Client-side JavaScript timeout in milliseconds
- **executionTimeout**: Server-side web.config timeout in seconds
- Rule: `s-timeout` should be less than `executionTimeout * 1000`

---

## Troubleshooting

### Upload Fails with 413 Error
- Increase `maxRequestLength` in web.config
- Ensure value is in KB (100 MB = 102400 KB)
- Restart application pool after web.config change

### Upload Succeeds but Times Out
- Increase `executionTimeout` in web.config
- Increase `s-timeout` attribute (in milliseconds)
- Optimize server-side processing for large files

### Progress Bar Not Updating
- Verify using `XMLHttpRequest`, not jQuery AJAX
- Check `xhr.upload.addEventListener('progress', ...)` is attached
- Ensure progress bar element has class `simplisity-file-progress-bar`

### Server Command Not Executing
- Verify `simplisity_callserver()` function is available
- Check browser console for JavaScript errors
- Verify command name in `s-cmd` attribute matches server implementation
- Ensure upload completed with status 200

### File Not Found in Server Command
- Check upload API saves file to expected location
- Verify file naming convention
- Check file permissions on upload directory

---

## Security Considerations

1. **File Type Validation**: Always validate file type server-side, not just client-side
2. **File Size Limits**: Enforce reasonable limits to prevent DoS attacks
3. **File Content Scanning**: Consider virus scanning for uploaded files
4. **Authentication**: Ensure upload endpoint requires appropriate authentication
5. **Authorization**: Verify user has permission to upload files
6. **Path Traversal**: Sanitize file names to prevent directory traversal attacks
7. **Temporary File Cleanup**: Always clean up uploaded files after processing

---

## Example: Adapting for Different File Types

### XML Configuration Import
```html
<input id="xmlfileupload" type="file" accept=".xml" style="display:none;">
<div id="processxmlupload" 
     s-cmd="rockettools_importxmlconfig" 
     s-timeout="300000" 
     s-post="#configdata" 
     s-return="#resultcontainer" 
     class="w3-hide"></div>
```

### Image Upload with Preview
```html
<input id="imageupload" type="file" accept="image/*" style="display:none;">
<img id="imagepreview" src="" style="max-width:200px;display:none;">
<div id="processimageupload" 
     s-cmd="media_processimage" 
     s-timeout="60000" 
     s-post="#imagedata" 
     s-return="#gallerycontainer" 
     class="w3-hide"></div>
```

### Multiple File Upload
```javascript
$('#multifileupload').on('change', function(e) {
    var files = e.target.files;
    if (files.length === 0) return;
    
    var formData = new FormData();
    
    // Append all selected files
    for (var i = 0; i < files.length; i++) {
        formData.append('files[]', files[i]);
    }
    
    // ... rest of upload logic
});
```

---

## Integration with Simplisity Framework

The DNNrocket system uses the Simplisity framework for server communication:

### simplisity_callserver() Function
This function handles AJAX communication with server commands:
- Reads configuration from data attributes (`s-cmd`, `s-post`, `s-return`, etc.)
- Serializes data from specified elements
- Makes AJAX POST request
- Handles response rendering

### Data Attributes Convention
- `s-cmd`: Server command name
- `s-post`: jQuery selector for data source
- `s-return`: jQuery selector for response target
- `s-timeout`: Request timeout in milliseconds
- `s-update`: Auto-update behavior

### Example Server Command Handler
```csharp
public class YourInterface
{
    public string your_command_name(Simplisity.SimplisityInfo systemInfo)
    {
        // Access posted data
        var data = systemInfo.GetXmlProperty("genxml/textbox/yourfield");
        
        // Process request
        // ...
        
        // Return HTML response or template
        return RenderTemplate("yourtemplate.cshtml");
    }
}
```

---

## Summary

This pattern provides a robust, user-friendly large file upload experience with:
- Real-time progress feedback
- Proper error handling
- Configurable size and timeout limits
- Seamless integration with server-side processing
- Clear separation of concerns (upload → process)

When implementing, follow the checklist and ensure all components are properly configured and tested.
