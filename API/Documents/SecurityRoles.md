# How `.rule` Security Roles Work in `SecurityCheckUser`

## Overview

`SecurityCheckUser(int portalId, int userid)` decides whether a given DNN user is allowed to access a plugin/interface, based on settings stored in the plugin's `.rule` XML file (`Info` on the `RocketInterface` instance).

The check is role-driven and fully dynamic — it does not hardcode a fixed list of roles in code. Instead, it inspects the roles the user actually belongs to in DNN, and looks for a matching setting in the `.rule` XML.

## The `.rule` XML Settings

Each role has a corresponding radio-button setting in the `.rule` XML, stored under a key with this pattern: `genxml/radio/securityroles{rolekey}`

Where `{rolekey}` is the DNN role name, lower-cased and with spaces removed.

| DNN Role Name             | XML Key                        |
|----------------------------|----------------------------------|
| Administrators              | securityrolesadministrators    |
| Manager                      | securityrolesmanager           |
| Editor                       | securityroleseditor            |
| ClientEditor                 | securityrolesclienteditor      |
| Registered Users             | securityrolesregisteredusers   |
| Subscribers                   | securityrolessubscribers      |
| *(any future role, e.g. Owner)* | securityrolesowner *(automatic, no code change)* |

There is also a special, non-role-specific setting: `genxml/radio/securityrolesall`

This applies to **all** users regardless of role (e.g. "Super Users only" / open access).

## Setting Values

Each `securityroles*` setting is an integer with 4 possible states:

| Value | Meaning                                         |
|-------|---------------------------------------------------|
| 0     | Not set / role ignored (no opinion)               |
| 1     | **Allow** access for this role                     |
| 2     | **Block** access for this role                     |
| 3     | **Force False** — always deny, even overriding an Allow from another role |

## Evaluation Order

1. **Super Users** always have access — checked first, before any role logic.
2. `securityrolesall == 1` → access is allowed immediately for everyone.
3. **Force False pass**: the user's DNN roles are checked first. If *any* role the user belongs to has its setting `== 3`, access is **denied immediately** — this overrides any Allow the user might otherwise get from another role.
4. **Allow pass**: if no Force False matched, the user's roles are checked again. If *any* role has its setting `== 1`, access is **granted immediately**.
5. **Block pass**: if neither of the above matched, the user's roles are checked once more. If *any* role has its setting `== 2` (or `3`, as a safety net), access is **denied immediately**.
6. `securityrolesall == 2` → if nothing else matched, this blocks access (effectively "Super Users only").
7. If none of the above apply, access defaults to **denied**.

> **Important:** The order is Force False → Allow → Block. This means:
> - A role set to `3` (Force False) always wins over a role set to `1` (Allow) on the same user.
> - Among `1` (Allow) and `2` (Block) only, Allow wins, since Allow is checked before Block.

## Why This Is Dynamic (No Recompile Needed)

Instead of a hardcoded list of roles in C#, the method:

1. Reads the user's actual DNN role memberships (`userInfo.Roles`).
2. For each role name, builds the expected XML key by stripping spaces and lower-casing (e.g. "Registered Users" becomes securityrolesregisteredusers).
3. Looks up that key directly in the `.rule` XML via `GetSecurityRoleSetting`.

This means adding a brand-new role (e.g. creating an "Owner" role in DNN and adding a `securityrolesowner` radio control to the `.rule` template) works immediately — no assembly recompile or redeployment required.

## Case-Insensitivity

Role name comparisons and XML key lookups are both treated as case-insensitive, so variations like "Manager", "manager", or "MANAGER" all resolve to the same `securityrolesmanager` setting.