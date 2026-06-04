# Space Map

Detects entities around your ship via camera raycast and displays them on configurable screens. Ships can synchronize their database with a nearby Server via Inter-Grid Communication (IGC).

## Installation

1. Copy the compiled script into a **Programmable Block**.
2. Name your camera and display blocks with the `[SM]` tag (see [Block Naming](#block-naming)).
3. Set the `Mode` at the top of the script.
4. Run or recompile — screens initialize automatically.

## Configuration

All settings are at the top of the script, inside the `SETTINGS` region. Edit them before deploying.

| Setting                    | Default      | Description                                             |
|----------------------------|--------------|---------------------------------------------------------|
| `Mode`                     | `"Ship"`     | Operating mode: `Ship`, `Server`, or `ServerShip`       |
| `SyncChannel`              | `"SM_SYNC"`  | IGC broadcast channel for ship↔server sync              |
| `SyncBroadcastInterval`    | `360`        | Update10 ticks between ship broadcasts (~60 s)          |
| `SearchBlockInterval`      | `100`        | Ticks between block discovery scans                     |
| `DefaultDetectionDistance` | `10000`      | Initial raycast range (meters)                          |
| `DefaultMapScale`          | `5000`       | Initial map display radius (meters)                     |
| `NewEntryThresholdSeconds` | `60`         | Duration (s) an entry is marked as new (`[!]`)          |
| `MapPerspectiveStrength`   | `0.2`        | Depth offset intensity on the Map view                  |
| `Map3DEllipseRatio`        | `0.4`        | Vertical compression of the plane ellipse on Map3D      |
| `Map3DDepthScale`          | `0.65`       | Altitude axis scale on Map3D                            |

`DefaultDetectionDistance` and `DefaultMapScale` can also be changed at runtime via commands and are persisted across restarts.

## Operating Modes

| Mode         | Detects | Stores data | Sync behaviour                              |
|--------------|---------|-------------|---------------------------------------------|
| `Ship`       | ✓       | Local DB    | Broadcasts database to servers in range     |
| `Server`     | —       | Merged DB   | Receives and merges databases from ships    |
| `ServerShip` | ✓       | Merged DB   | Both broadcasts and receives                |

## Block Naming

Add the `[SM]` tag to any block you want SpaceMap to manage.

### Camera
Any camera with `[SM]` in its name is used for raycasting.
```
Wide Lens Camera [SM]
```

### Displays

**Single-surface blocks** (basic LCD panels):
```
LCD Panel [SM]            → Map mode (default)
LCD Panel [SM:Map3D]      → 3D perspective map
LCD Panel [SM:Database]   → Full entry list
LCD Panel [SM:Detection]  → Live raycast info
LCD Panel [SM:Help]       → Command reference
LCD Panel [SM:Sync]       → Synchronization status
LCD Panel [SM:Cycle]      → Cycles via disp_next command
```

**Multi-surface blocks** (cockpits, terminals, ...):
```
Cockpit [SM]              → Surface 0: Map · 1: Map3D · 2: Database
Cockpit [SM:Auto]         → Same as above
Cockpit [SM:0-Map3D;1-Detection]  → Surface 0: Map3D · Surface 1: Detection
```

## Display Modes

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
Full table of all stored entries: name, entity ID, distance, and time since last detection. Supports sorting and scrolling via commands.

### Detection
Live raycast info: current target, distance, ray charge percentage, and detection status.

### Help
In-game quick reference listing all available commands and their syntax.

### Sync
Synchronization status screen.

- **Server / ServerShip mode**: Lists all ships that have recently broadcast their database — source name, age of last sync, and entry count per source.
- **Ship / ServerShip mode**: Shows the timestamp and entry count of the last broadcast sent to nearby servers.

## Synchronization

In `Ship` and `ServerShip` modes, the script periodically broadcasts its full local database over IGC on the configured `SyncChannel`. The interval is controlled by `SyncBroadcastInterval` (in Update10 ticks — default 360 ≈ 60 s). No manual setup is required; antenna range determines reach.

When the server receives a broadcast it merges the incoming entries:
- Entries not yet in the database are added as-is.
- For known entries, the most recent `UpdateDate` wins.
- Custom names set locally are never overwritten by an incoming empty name.

## Commands

Run via the programmable block's argument field, a button, or a timer block. Use `[SM:Help]` on any screen for the in-game reference.

Commands follow a `category_action` naming pattern. All have short aliases for button/timer use.

```
map_scale <meters|+delta|-delta>        alias: ms
  Set or adjust the map display radius (Map and Map3D views).
  Example: map_scale 5000 | ms +1000 | ms -500

detect_range <meters|+delta|-delta>     alias: dr
  Set or adjust the camera detection range.
  Example: detect_range 8000 | dr +2000 | dr -1000

entry_rename <identifier> <newName>     alias: ren
  Assign a custom name to a map entry.
  Identifier can be:
    detected (or d) — the entity currently in the detection crosshair
    <id>            — numeric entity ID (shown in Database mode)
    <name>          — base name (e.g. AXK-42) or existing custom name
  Example: entry_rename detected "Gold Mine"
  Example: ren AXK-42 "Iron Cache"

db_scroll (up|down|<n>)                 alias: dbs
  Scroll the Database view. up/down moves by 5 entries.
  Example: db_scroll down | dbs up | dbs -3

db_sort <mode>                          alias: dbso
  Set the sort order for the Database view. Resets scroll to top.
  Modes: distance, name, age, new
  Example: db_sort name | dbso distance

db_sort_next                            alias: dbsn
  Cycle to the next database sort mode.
  Order: Distance → Name → Age → New → Distance → ...
  Example: db_sort_next

disp_next [mode]                        alias: dn
  Cycle the display mode on screens tagged [SM:Cycle],
  or jump to a specific mode.
  Modes: Map, Map3D, Database, Detection, Help, Sync
  Example: disp_next | dn Map3D | dn Sync
```

## Detection Age Indicators

All displays show how recently each entity was detected:

| Indicator       | Meaning                                             |
|-----------------|-----------------------------------------------------|
| `[!]` / green   | Detected within the last `NewEntryThresholdSeconds` |
| `2m` / `2m ago` | Last seen 2 minutes ago                             |
| `1h` / `1h ago` | Last seen 1 hour ago                                |
| `?`             | Detection time unknown                              |
