
# AssignDataModel
**Description**: Assigns the data model for Razor by calling the base class's AssignDataModel method. This makes the template easier to build by populating various data properties from the SimplisityRazor model.
**Signature**
```csharp
public new string AssignDataModel(SimplisityRazor sModel)
```
**Example**
```csharp
@{ AssignDataModel(Model); }
```
