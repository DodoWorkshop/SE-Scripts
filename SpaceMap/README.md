# Space Map

Detects entities around your ship via camera raycast and displays them on configurable screens. Supports multiple display modes, custom naming, and optional server synchronization.

## Installation

1. Copy the compiled script into a **Programmable Block**.
2. Name your camera and display blocks with the `[SM]` tag (see [Block Naming](#block-naming)).
3. Set the `Mode` at the top of the script.
4. Run or recompile — screens initialize automatically.

## Configuration

All settings are at the top of the script, inside the `SETTINGS` region. Edit them before deploying.

| Setting                    | Default  | Description                                        |
|----------------------------|----------|----------------------------------------------------|
| `Mode`                     | `"Ship"` | Operating mode: `Ship`, `Server`, or `ServerShip`  |
| `SearchBlockInterval`      | `100`    | Ticks between block discovery scans                |
| `DefaultDetectionDistance` | `10000`  | Initial raycast range (meters)                     |
| `DefaultMapScale`          | `5000`   | Initial map display radius (meters)                |
| `NewEntryThresholdSeconds` | `60`     | Duration (s) an entry is marked as new (`[!]`)     |
| `MapPerspectiveStrength`   | `0.2`    | Depth offset intensity on the Map view             |
| `Map3DEllipseRatio`        | `0.4`    | Vertical compression of the plane ellipse on Map3D |
| `Map3DDepthScale`          | `0.65`   | Altitude axis scale on Map3D                       |

`DefaultDetectionDistance` and `DefaultMapScale` can also be changed at runtime via commands and are persisted across restarts.

## Operating Modes

| Mode         | Detects | Stores data | Syncs                     |
|--------------|---------|-------------|---------------------------|
| `Ship`       | ✓       | Local DB    | Sends to servers in range |
| `Server`     | —       | Merged DB   | Receives from ships       |
| `ServerShip` | ✓       | Merged DB   | Both                      |

## Block Naming

Add the `[SM]` tag (or your custom `NameTag`) to any block you want SpaceMap to manage.

### Camera
Any camera with `[SM]` in its name is used for raycasting.
```
Wide Lens Camera [SM]
```

### Displays

**Single-surface blocks** (basic LCD panels):
```
LCD Panel [SM]            → General mode (default)
LCD Panel [SM:Map]        → 2D radar map
LCD Panel [SM:Map3D]      → 3D perspective map
LCD Panel [SM:Database]   → Full entry list
LCD Panel [SM:Detection]  → Live raycast info
LCD Panel [SM:Cycle]      → Cycles via disp_next command
```

**Multi-surface blocks** (cockpits, terminals, ...):
```
Cockpit [SM]              → Surface 0: General · 1: Map · 2: Database
Cockpit [SM:Auto]         → Same as above
Cockpit [SM:0-Map3D;1-Detection]  → Surface 0: Map3D · Surface 1: Detection
```

## Display Modes

### General
Overview list of all known entities with distance and detection age.

### Map
Top-down 2D radar circle. The ship is at center. Entity positions are projected onto the ship's horizontal plane. A subtle depth offset (configurable via `MapPerspectiveStrength`) hints at altitude differences — objects above shift toward the upper-right, objects below toward the lower-left.

Color coding:
- White dot — known entity
- Green dot + outer ring — detected less than `NewEntryThresholdSeconds` ago

### Map3D
Isometric 3D perspective view. The horizontal plane is shown as a squished ellipse. Each entity is:
- Projected as a shadow dot on the plane
- Rendered at its true altitude above/below the plane
- Connected to its shadow by a dashed vertical line

`Map3DEllipseRatio` controls how flat the ellipse looks. `Map3DDepthScale` controls how much altitude stretches vertically on screen.

### Database
Full table of all stored entries: name, entity ID, distance, and time since last detection.

### Detection
Live raycast info: current target, distance, ray charge percentage, and detection status.

## Commands

Run via the programmable block's argument field, a button, or a timer block.

Commands follow a `category_action` naming pattern. All have short aliases for button/timer use.

```
help
  List all available commands.

map_scale <meters>              alias: ms
  Set the map display radius.
  Example: map_scale 10000

entry_rename <id> <newName>     alias: ren
  Assign a custom name to a map entry.
  Identifier can be:
    detected (or d) — the entity currently in the detection crosshair
    <id>            — numeric entity ID (shown in Database mode)
    <name>          — base name (e.g. AXK-42) or existing custom name
  Example: entry_rename detected "Gold Mine"
  Example: ren AXK-42 "Iron Cache"

disp_next [mode]                alias: dn
  Cycle the display mode on screens tagged [SM:Cycle], or jump to a specific mode.
  Modes: General, Map, Map3D, Database, Detection
  Example: disp_next
  Example: disp_next Map3D

db_scroll (up|down|<n>)         alias: dbs
  Scroll the Database view. up/down moves by 5 entries; provide a number for a specific delta.
  Example: db_scroll down
  Example: dbs -3

db_sort <mode>                  alias: dbso
  Set the sort order for the Database view. Resets scroll to the top.
  Modes: distance, name, age, new
  Example: db_sort name
  Example: dbso distance
```

## Detection Age Indicators

All displays show how recently each entity was detected:

| Indicator       | Meaning                                             |
|-----------------|-----------------------------------------------------|
| `[!]` / green   | Detected within the last `NewEntryThresholdSeconds` |
| `2m` / `2m ago` | Last seen 2 minutes ago                             |
| `1h` / `1h ago` | Last seen 1 hour ago                                |
| `?`             | Detection time unknown                              |
