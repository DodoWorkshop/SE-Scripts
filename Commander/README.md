# Commander

## 📘 Description

This script allows you to trigger actions on targeted blocks using flexible filters.
It also provides access to some actions that are not available through the in-game interface.

**Features:**

- Apply a list of actions to all blocks matching one or more of the following filters:
  - tag: filters blocks whose name contains the provided string
  - name: filters blocks with an exact name match
  - data: filters blocks whose Custom Data contains the provided string (TODO)
  - grid: if true, only blocks from the current grid are considered (default: false)
  - group: filters blocks that belong to the specified block group
- Set the HUD text of antennas
- Set the text of text panels (TODO)
- List all available actions for a group of blocks

## ⚙️ Usage

Commands are sent to the programmable block via its argument field.

### 🆘 Show Help

Displays all available commands and usage examples:

```
help
```

### ⚡ Apply Actions

Applies one or several actions to blocks matching the given filters.

**Example:**

Turns on every block that:

- belongs to the _same grid_ as the programmable block,

- is in the group _Group1_,

- and contains _MyBloc_ in its name.

Also applies the Recharge action to every matching battery.

```
ap -tag=MyBloc -grid=true -group=Group1 OnOff_On Recharge
```

### 📋 List Actions

Lists all possible actions (both common and unique) for blocks matching the given filters.

**Example:**

Lists every available action for the group named _Group1_.

```
la -group=Group1
```

### 🔁 List Common Actions

Lists only common actions shared by all matching blocks (excludes unique block actions).

**Example:**

Lists every action shared by all blocks on the current grid.

```
lca -grid=true
```

### 📡 Set Antenna HUD Text

Changes the text displayed on the HUD by the targeted antennas.

**Example:**

Displays _"Waow! Awesome!"_ on every antenna named _"My awesome Antenna"_.

```
sat -name="My awesome Antenna" "Waow! Awesome!"
```