# DodoWorkshop - Space Engineers Scripts

## 🎯 Purpose

This repository contains a set of custom scripts for Space Engineers, built with the MDK (Malware’s Development Kit).
Each script is fully compatible with the game’s programmable blocks and relies on shared mixins to provide common
functionality while keeping each script lightweight.

## 🚀 Overview

This repository hosts multiple independent MDK scripts, all sharing a common modular framework made of mixins.

### What’s MDK?
MDK (Malware’s Development Kit)
enables proper C# development for Space Engineers.
It provides:

- IntelliSense, refactoring, and debugging.

- Visual Studio / Rider project support.

- Automatic packaging for in-game upload.


## 🧩 Project Structure

```
/SE-Scripts
│
├── Mixins/
│   ├── Core/          → mandatory base mixin (framework core)
│   ├── Events/        → optional mixin 
│   └── ...            → other optional mixins (see list below)
├── Commander/         → a script example
└── ...                → other scripts (see list below)
```

## 🧱 Mixins and Dependencies

Mixins are shared modules you can import into any MDK project.
They are included via .projitems (or .props) imports.

*Core* → **required in all scripts**.
Provides the base system, dependency injection, helpers, and framework core.

*Other mixins* → **optional**.
Use only what you need to avoid hitting the Space Engineers script size limit.

### Example:

In your .csproj, add one or more imports such as:

```xml

<Import Project="..\..\libs\Core\Core.projitems" Label="Shared"/>
```

or

```xml

<Import Project="..\..\libs\Events\Logging.projitems" Label="Shared"/>
```

## 🧰 Setup Guide

### 1️⃣ Install MDK

Download and install the latest version from MDK-SE releases.
Make sure your IDE (Visual Studio or Rider) recognizes MDK projects.

### 2️⃣ Clone the repository

```
git clone https://github.com/DodoWorkshop/SE-Scripts
```

### 3️⃣ Open the solution

Open the .sln file in your IDE, build, and deploy via MDK to Space Engineers.

### 4️⃣ (Optional) Create new scripts

Add a folder at the root, create a new MDK project, and import at least the Core mixin.

## 📦 Scripts Overview

Each subproject in /scripts is a self-contained Space Engineers script.
Every script can include its own README.md for usage and setup details.

### [Commander](Commander/README.md)
Provides ways to send actions to filtered blocs.

### [DockInfo](DockInfo/README.md)
Manages docks, give infos about docked ships and provides dock history.

### [SpaceMap](SpaceMap/README.md)
Displays a map of discovered asteroids. You can name them, access multiple infos about them and synchronize databases.

### [FillIt](FillIt/README.md) [WIP: just starting]
Fills container with what you want.

## 🧠 Development Notes

Always import at least the Core mixin.

When editing mixins, reload the project (Rider/VS may not detect .projitems changes).

Avoid unnecessary dependencies: keep your scripts small to stay under Space Engineers 100 000 character limit (you can
use MDK minify).

## 💡 Useful Resources

[MDK Official Wiki](http://github.com/malware-dev/MDK-SE/wiki)

## 🧑‍💻 Contributing

This repository is modular by design.
You can create new mixins, fix bugs, or improve the framework.

Place each new mixin in /mixins/.

Never commit compiled or generated files.