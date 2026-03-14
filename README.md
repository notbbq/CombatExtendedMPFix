# Combat Extended MP Fix

Fixes the 5 errors on startup which also correspond with known issues with Combat Extended in MP

## Fixes
- CompAmmoUser syncing (switching ammo types)
- CompFireModes syncing (switching fire modes)
- Loadout syncing
- LoadoutSlot syncing
- ITab_Inventory syncing (honestly didn't notice it even before the fix)

## Why
CE uses its own intermediate attribute system rather than the MP API directly,
likely so the main CE assembly doesn't need a hard dependency on MP API.
CE has its own SyncMethodAttribute that it looks for and registers sync methods with.
When CE calls MP.RegisterAll() it only processes the MP compat assembly,
and the SyncWorker registrations in that assembly fail because of how
CE's intermediate layer interacts with the current API supposedly. (Thanks to Sokyran for description)

## Requirements
- Combat Extended
- Rimworld Multiplayer
- Multiplayer Compatibility

## ATM
It's just a stopgap to use while a proper fix gets merged into MPCompat
