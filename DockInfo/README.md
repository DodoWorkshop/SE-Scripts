# Dock Info

![cover](thumb.png)

## 📘 Description

Allows the user to create connection groups around a connector
and control related blocks and displays — for example, to
customize a landing pad.

It provides the following features:
- For text panels linked to a group:
    - Displays the group name when no ship is connected.
    - Shows detailed information about the connected ship.
- On connection/disconnection: timer blocks can be automatically triggered.
- Connection history: can be displayed on a screen for monitoring recent dockings.

## ⚙️ Usage

### 1 - Group Definition
You need to create groups of blocks around a connector.
To do this, edit the connector’s name and add the following tag:
[SI:{group name}]

**Example:**
A connector named _"My Connector [SI:Zone n1] Left"_ will belong to a group called _"Zone n1"_.

** /!\ Only one connector is allowed per group! /!\ **

### 2 - Assigning Group Blocks
Once a group is defined, simply add the same tag to the name of any block you want to link to that group.

Currently supported block types:
- Text panels
- Timer blocks

### 3 - Configuring Group Blocks
When a block is assigned to a group, the script automatically adds configuration sections to its Custom Data.
Each section defines the block’s behavior when a ship connects or disconnects from the group’s connector.
You can customize these values to change how the block reacts.

**Example (Timer Block Custom Data):**

```
------------[Custom Data]------------
[Connected]
Action=Start

[Disconnected]
Action=None
-------------------------------------
```

This means that when a ship connects to the group’s connector, the timer block will start, and nothing will happen when the ship disconnects.

### 4 - History Panel Configuration
The connection history can be displayed on a text panel.
To enable this feature, simply include _[SI-History]_ in the name of the text panel where you want the history to appear.

The history is paginated, and the following commands can be provided as arguments to the programmable block to perform actions:
- **hnext** – go to the next page
- **hprev** – go to the previous page
- **hfirst** – jump to the first page
- **hreset** – clear the history