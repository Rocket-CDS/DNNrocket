# AI Reference: DNNrocket `system.rules` File

## What is `system.rules`?

Every DNNrocket module (and plugin module) contains a `system.rules` file in its root folder.  
It is plain XML wrapped in `<genxml>` and is loaded at application startup by `SystemLimpet`.  
The file is the **single source of truth** for how a module registers itself with the framework —
what backend services it provides, what navigation items it exposes, and what search/sort indexes it needs.

`SystemLimpet` is a singleton-per-system-key (managed by `SystemSingleton`).  
It reads the file, processes the lists, and builds the typed in-memory objects the rest of the framework queries.

---

## Top-Level Structure

```xml
<genxml>
  <systemkey>rocketintra</systemkey>       <!-- host/parent system key -->
  <plugin>true</plugin>                    <!-- marks this as a plugin to the parent system -->

  <sqlindex list="true"> ... </sqlindex>   <!-- DNNrocket SQL index definitions -->
  <groupsdata list="true"> ... </groupsdata>
  <provtypesdata list="true"> ... </provtypesdata>
  <providerdata list="true"> ... </providerdata>
  <interfacedata list="true"> ... </interfacedata>
</genxml>
```

| Element | Purpose |
|---|---|
| `systemkey` | Identifies which host system this module belongs to (its parent). |
| `plugin` | `true` = this module is a plugin; it registers into a parent system. |
| `sqlindex` | Registers XPath fields into the DNNrocket SQL index for searching/sorting without side tables. |
| `groupsdata` | Defines named navigation groups that can be used to bucket `interfacedata` entries. |
| `provtypesdata` | Defines custom provider type codes (rarely used). |
| `providerdata` | **Backend service registration** — see section below. |
| `interfacedata` | **UI/Navigation registration** — see section below. |

---

## Plugin Loading

When a base system (e.g. `rocketintra`) loads, `SystemLimpet` also scans its own `Plugins/` sub-folder for `*.rules` files.  
Each plugin's `.rules` file contributes its `providerdata`, `interfacedata`, and `groupsdata` entries directly into the base system's merged lists.  

Additionally, when a system declares a `<basesystemkey>`, `SystemSingleton` merges all `interfacedata` entries from the base system into the plugin's own `InterfaceList`, enabling shared navigation.

---

## `<providerdata>` — Backend Service Registration

### Purpose

`providerdata` wires **backend service providers** into the framework.  
Each entry tells the system: *"load this class, of this service type, from this assembly."*

At startup, `SystemLimpet.InitSystem()` iterates `providerdata` and routes each entry into a typed list based on `<providertype>`:

| `providertype` value | Typed list in `SystemLimpet` | Used for |
|---|---|---|
| `plugin` | `PluginList` | API command routing (dispatches HTTP commands to `namespaceclass`) |
| `eventprovider` | `EventList` | Cross-module event hooks |
| `scheduler` | `SchedulerList` | DNN scheduled tasks |
| `handlebars` | `HandleBarsList` | Handlebars template helpers |
| `razor` | `RazorList` | Razor template processors |
| `searchindex` | `ProviderList` | DNN site search index integration |

> Entries are only loaded when `<active>true</active>`, `<assembly>` is non-empty, and `<namespaceclass>` is non-empty.  
> Entries are sorted by `genxml/config/sortorder` before processing.

### Schema

```xml
<genxml>
  <textbox>
    <interfacekey>events-rocketintracrm</interfacekey>         <!-- unique key -->
    <namespaceclass>RocketIntraCRM.Components.Events</namespaceclass>
    <assembly>RocketIntraCRM</assembly>
    <interfaceicon></interfaceicon>
    <defaultcommand></defaultcommand>
    <relpath>/DesktopModules/DNNrocketModules/RocketIntraCRM</relpath>
  </textbox>
  <providertype>eventprovider</providertype>                   <!-- REQUIRED: non-empty -->
  <dropdownlist>
    <group></group>
  </dropdownlist>
  <checkbox>
    <onmenu>false</onmenu>
    <active>true</active>
  </checkbox>
  <radio>
    <securityrolesadministrators>1</securityrolesadministrators>
    <securityrolesmanager>0</securityrolesmanager>
    <securityroleseditor>0</securityroleseditor>
    <securityrolesclienteditor>0</securityrolesclienteditor>
    <securityrolesregisteredusers>0</securityrolesregisteredusers>
    <securityrolessubscribers>0</securityrolessubscribers>
    <securityrolesall>0</securityrolesall>
  </radio>
</genxml>
```

### Key Rule

> **`<providertype>` is always non-empty in `providerdata`.**  
> The value determines which typed runtime list the entry lands in.

---

## `<interfacedata>` — UI / Navigation Registration

### Purpose

`interfacedata` defines **navigable views and menu entries** within a system or plugin.  
Each entry tells the framework: *"there is a page/view with this key, rendered by this class, accessible to these roles."*

At startup, `SystemLimpet.InitSystem()` loads all `interfacedata` entries into `InterfaceList` (a `Dictionary<string, RocketInterface>` keyed by `interfacekey`).

This list is used to:
- Build the navigation menu (entries where `<onmenu>true</onmenu>`)
- Resolve which API class handles a given `interfacekey` request
- Enforce role-based security before rendering a view
- Support the `basesystemkey` merge — the base system's `interfacedata` is inherited by plugin systems

### Schema

The XML structure is **identical to `providerdata`**, with one important difference:

```xml
<genxml>
  <textbox>
    <interfacekey>rocketintraclients</interfacekey>
    <namespaceclass>RocketIntraCRM.API.StartConnect</namespaceclass>
    <assembly>RocketIntraCRM</assembly>
    <interfaceicon>account_box</interfaceicon>            <!-- Material icon name -->
    <defaultcommand>rocketintraclients_list</defaultcommand>
    <relpath>/DesktopModules/DNNrocketModules/RocketIntraCRM</relpath>
  </textbox>
  <providertype></providertype>                          <!-- ALWAYS EMPTY in interfacedata -->
  <dropdownlist>
    <group></group>                                      <!-- optional navigation group ref -->
  </dropdownlist>
  <checkbox>
    <onmenu>true</onmenu>                                <!-- true = appears in nav menu -->
    <active>true</active>
  </checkbox>
  <radio>
    <securityrolesadministrators>1</securityrolesadministrators>
    <securityrolesmanager>1</securityrolesmanager>
    <securityroleseditor>1</securityroleseditor>
    <securityrolesclienteditor>1</securityrolesclienteditor>
    <securityrolesregisteredusers>0</securityrolesregisteredusers>
    <securityrolessubscribers>0</securityrolessubscribers>
    <securityrolesall>0</securityrolesall>
  </radio>
</genxml>
```

### Key Rule

> **`<providertype>` is always empty in `interfacedata`.**  
> There is no backend service type — these are purely UI/routing definitions.

---

## `providerdata` vs `interfacedata` — Side-by-Side Comparison

| Aspect | `providerdata` | `interfacedata` |
|---|---|---|
| **Role** | Backend service wiring | UI navigation and command routing |
| **`providertype`** | Always non-empty (`plugin`, `scheduler`, `eventprovider`, etc.) | Always empty |
| **Runtime result** | Populates `PluginList`, `EventList`, `SchedulerList`, etc. | Populates `InterfaceList` dictionary |
| **`onmenu`** | Typically `false` (backend, not visible) | `true` for menu items, `false` for hidden routes |
| **Security roles** | Controls which roles can invoke the provider | Controls which roles can see/access the view |
| **`interfacekey`** | Unique ID for the provider within its typed list | Unique ID for the navigation entry / route |
| **`defaultcommand`** | The API command the plugin handles | The default command to run when the view is loaded |
| **Framework method** | `SystemLimpet.GetProvider(key)` | `SystemLimpet.GetInterface(key)` |
| **Loaded by** | `InitSystem()` → typed provider lists | `InitSystem()` → `InterfaceList` |
| **Inherited via basesystemkey?** | No | Yes — base system `interfacedata` merges into plugin |

---

## Why the Same `interfacekey` Appears in Both Lists

In plugin modules (e.g. `RocketIntraCRM`), it is normal for the same `interfacekey` to appear in both `providerdata` and `interfacedata`:

- The **`providerdata`** entry (with `providertype=plugin`) registers the API handler class with the **host system's plugin dispatcher** so it can route HTTP commands to the correct `namespaceclass`.
- The **`interfacedata`** entry defines the **navigation and view configuration** for that same key within the module's own UI — what icon, what default command, what roles can access it.

These are two separate concerns: one is about *how the framework calls your code*, the other is about *what the user sees and can navigate to*.

---

## `sqlindex` — Search Index Registration

```xml
<sqlindex list="true">
  <genxml>
    <systemkey>rocketintra</systemkey>
    <ref>clientreminderdate</ref>
    <xpath>genxml/textbox/reminderdate</xpath>
    <typecode>RocketIntraCRMCLIENT</typecode>
  </genxml>
</sqlindex>
```

Each entry tells the DNNrocket SQL index system to maintain a searchable/sortable indexed value for the given `xpath` on records of the given `typecode`. This avoids side tables or custom triggers — the index is managed automatically via `DNNrocketController.GetList()` filter parameters.

---

## Security Roles Reference

| Field | Meaning |
|---|---|
| `securityrolesadministrators` | DNN Administrators |
| `securityrolesmanager` | RocketIntra Manager role |
| `securityroleseditor` | RocketIntra Editor role |
| `securityrolesclienteditor` | RocketIntra Client Editor role |
| `securityrolesregisteredusers` | Any registered DNN user |
| `securityrolessubscribers` | DNN Subscribers role |
| `securityrolesall` | All users including anonymous |

Value `1` = role has access, `0` = no access.

---

## Quick Reference: Common `providertype` Values

| Value | Purpose |
|---|---|
| `plugin` | General API plugin — routes HTTP commands via `StartConnect` |
| `eventprovider` | Hooks into the cross-module event system |
| `scheduler` | Registers a DNN scheduled task |
| `searchindex` | Integrates with DNN site search indexing |
| `handlebars` | Registers a Handlebars template helper class |
| `razor` | Registers a Razor template processor class |
