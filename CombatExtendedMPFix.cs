using Verse;
using Multiplayer.API;
using CombatExtended;

namespace CombatExtendedMPFix
{
    [StaticConstructorOnStartup]
    public static class CombatExtendedMPFix
    {
        static CombatExtendedMPFix()
        {
            if (!MP.enabled) return;
            //manually registering his old sync workers since register all doesn't do sync workers anymore
            //refer https://github.com/CombatExtended-Continued/CombatExtended/blob/Development/Source/MultiplayerCompat/MultiplayerCompat/MultiplayerCompat.cs"
            MP.RegisterSyncWorker<CompAmmoUser>(SyncCompAmmoUser);
            MP.RegisterSyncWorker<CompFireModes>(SyncCompFireModes);
            MP.RegisterSyncWorker<Loadout>(SyncLoadout);
            MP.RegisterSyncWorker<LoadoutSlot>(SyncLoadoutSlot);
            MP.RegisterSyncWorker<ITab_Inventory>(SyncITab_Inventory, shouldConstruct: true);
        }

        static void SyncCompAmmoUser(SyncWorker sync, ref CompAmmoUser comp)
        {
            //this one was easy at least! it's calling the weapon after which it will identify if CompAmmoUser is being referenced in any synced action
            //ex: ammo switching, reloading, etc
            Thing parent = comp?.parent;
            sync.Bind(ref parent);
            if (!sync.isWriting)
            {
                comp = parent?.TryGetComp<CompAmmoUser>();
            }
        }
        static void SyncCompFireModes(SyncWorker sync, ref CompFireModes comp)
        {
            //We are doing the same as above basically just with CompFireModes, they have their own syncmethod (THANK U)
            Thing parent = comp?.parent;
            sync.Bind(ref parent);
            if (!sync.isWriting)
            {
                comp = parent?.TryGetComp<CompFireModes>();
            }
        }
        static void SyncLoadout(SyncWorker sync, ref Loadout loadout)
        {
            //we couldn't use his unique id because it's a private value 
            //we used his label on the loadout instead to register loadouts
            string label = loadout != null ? loadout.label : null;
            sync.Bind(ref label);
            if (!sync.isWriting)
            {
                loadout = LoadoutManager.GetLoadoutByLabel(label);
            }
        }
        static void SyncLoadoutSlot(SyncWorker sync, ref LoadoutSlot slot)
        {
            //idk how tf this worked i know we are combing through the list on the loadout manager and copying what matches the loadout details
            //we then identify it by the index position on that loadout's OwnSlots list
            LoadoutSlot slotCopy = slot;
            Loadout owningLoadout = LoadoutManager.Loadouts?.FirstOrDefault(l => l.OwnSlots.Contains(slotCopy));
            string label = owningLoadout != null ? owningLoadout.label : null;
            int index = owningLoadout != null ? owningLoadout.OwnSlots.IndexOf(slot) : -1;
            sync.Bind(ref label);
            sync.Bind(ref index);
            if (!sync.isWriting)
            {
                owningLoadout = LoadoutManager.GetLoadoutByLabel(label);
                slot = owningLoadout.OwnSlots[index];
            }
        }
        static void SyncITab_Inventory(SyncWorker sync, ref ITab_Inventory inventory)
        {
            //he just cared about should construct:true so leave blank just for method calling
        }
    }
}
