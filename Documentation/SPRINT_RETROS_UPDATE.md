# SPRINT_RETROS.md - Sprint 3 Addition

## Copy-paste this into your SPRINT_RETROS.md file (after Sprint 2)

---

## Sprint 3: Player State & Scene Management (January 12, 2025)

### Deliverable Achievement
✅ DELIVERED: Player can drive, exit vehicle, walk, and transition between scenes

### What Went Well
- State machine architecture cleanly separates concerns (PlayerStateManager singleton)
- DontDestroyOnLoad pattern works reliably with `transform.root.gameObject` approach
- Async scene loading with proper timing prevents spawn point initialization issues
- Vehicle detection solved elegantly with `GetComponentInParent()` approach
- Camera priority system eliminates black screen transitions during state changes
- Event-driven architecture (UnityEvents) makes state changes predictable and testable
- Extensive debugging and console logging led to robust, reusable solutions
- Spawn point system flexible enough for both walking and driving modes
- Character positioning works correctly with CharacterController disable/enable pattern

### Challenges & Solutions

**Challenge 1: DontDestroyOnLoad failed with child GameObjects**
- **Problem:** SceneTransitionManager destroyed during scene transitions (was child of GameManager)
- **Solution:** Changed from `DontDestroyOnLoad(gameObject)` to `DontDestroyOnLoad(transform.root.gameObject)`
- **Learning:** DontDestroyOnLoad only works on root GameObjects; use transform.root for children

**Challenge 2: Spawn points not found after scene load**
- **Problem:** `FindObjectsByType<SceneSpawnPoint>()` returned empty array immediately after AsyncOperation.isDone
- **Solution:** Added `yield return new WaitForSeconds(0.2f)` + `yield return new WaitForEndOfFrame()` before spawning
- **Learning:** AsyncOperation.isDone ≠ scene objects fully initialized; need additional wait frames

**Challenge 3: Vehicle colliders couldn't trigger transition zones**
- **Problem:** CarController component on parent GameObject, but child colliders (wheels, body) were triggering zones
- **Solution:** Changed from `GetComponent<CarController>()` to `GetComponentInParent<CarController>()`
- **Learning:** Always use GetComponentInParent() when child colliders might trigger events but components are on parents

**Challenge 4: CharacterController position wouldn't set directly**
- **Problem:** CharacterController has built-in collision preventing direct `transform.position` changes
- **Solution:** Temporarily disable CharacterController during teleportation: `cc.enabled = false` → set position → `cc.enabled = true`
- **Learning:** CharacterController requires disable/enable pattern for teleportation

### Technical Learnings

1. **Unity Scene Loading:** `AsyncOperation.isDone` doesn't mean objects are fully initialized; wait multiple frames
2. **DontDestroyOnLoad:** Only works on root GameObjects; use `transform.root` for children
3. **CharacterController:** Must disable temporarily for teleportation to avoid collision interference
4. **Collider Hierarchy:** Use `GetComponentInParent()` when components are on parent GameObjects
5. **Cinemachine Priority:** Priority-based camera switching (10 vs 0) is cleaner than `SetActive()` toggling
6. **State Persistence Pattern:** Save state before scene load, restore after spawn point positioning
7. **Trigger Zone Design:** Separate "exit vehicle" behavior from "scene transition" for maximum flexibility

### Code Quality Notes

**Positive Patterns:**
- ✅ Singleton pattern used consistently across managers (PlayerStateManager, SceneTransitionManager)
- ✅ UnityEvent system enables loose coupling between systems (camera manager subscribes to state changes)
- ✅ Extensive debug logging aids troubleshooting (can be reduced via compiler directives for production)
- ✅ Proper cleanup in OnDestroy() prevents memory leaks (unsubscribe from events)
- ✅ Clear separation of concerns: state management, scene loading, vehicle interaction
- ✅ XML documentation comments on all public methods

**Areas for Improvement:**
- Debug logging is very verbose (80+ log statements) - should add #if DEBUG wrapper
- Some magic numbers (e.g., exitSideOffset = 1.5f, disableDelay = 0.3f) could be public fields
- OnGUI() debug display should be removed or hidden behind debug mode toggle
- Scene transition could benefit from fade-to-black UI (currently instant)

### Design Decisions

1. **Priority-based camera switching** vs GameObject.SetActive()
   - Rationale: Smoother blending, cameras stay alive during transitions
   - Result: No black screens, seamless transitions

2. **Delayed GameObject disabling (0.3s)** after state transitions
   - Rationale: Allow camera blend to complete before disabling inactive player mode
   - Result: Prevents jarring camera cuts

3. **Trigger zones** vs distance checks
   - Rationale: More precise control, handles complex geometries better
   - Result: Works reliably even with irregular zone shapes

4. **Separated "exit vehicle" from "scene transition"**
   - Rationale: Allows exiting vehicle within scene vs transitioning to new scene
   - Result: Flexible system supports both use cases with one component

5. **Character spawns 1.5m to right of vehicle on exit**
   - Rationale: Feels natural, prevents clipping with vehicle
   - Result: Player appears beside driver's door

### Action Items for Sprint 4

**Critical:**
- [ ] Fix HUD persistence across scenes (currently disappears)
- [ ] Add visual UI prompts for interactions (E to Enter/Exit/Talk)

**Important:**
- [ ] Implement proximity-based dialogue system
- [ ] Create property inspection UI panel
- [ ] Place NPCs in TownScene

**Nice to Have:**
- [ ] Remove debug keybinds (1/2/3) before release
- [ ] Add fade-to-black transitions between scenes
- [ ] Reduce debug logging verbosity

### Velocity Notes

- Sprint completed in **6 days** (target was 14 days = ahead of schedule)
- All **6/6 core deliverables** met
- **4 major technical blockers** resolved (DontDestroyOnLoad, spawn timing, vehicle detection, CharacterController)
- **7 new scripts** created (~1,270 lines of code)
- **2 scripts** modified (HUDController, CameraSwitcher)
- **0 console errors** after fixes
- **~8-10 Git commits** across sprint phases

### Sprint 3 Metrics

**Development:**
- Files Created: 7 (PlayerStateManager, StateAwareCarController, StateAwareCharacterController, CameraStateManager, VehicleInteraction, SceneTransitionManager, SceneTransitionZone, SceneSpawnPoint)
- Files Modified: 2 (HUDController - added prompt methods, CameraSwitcher - updated for priority)
- Lines of Code: ~1,270 new lines
- Unity Packages Added: 0 (used existing Starter Assets)

**Quality:**
- Console Errors: 0 (after fixes)
- Console Warnings: 0 (after DontDestroyOnLoad fix)
- Build Targets Tested: 2 (PC Standalone, WebGL)
- Scenes Created: 1 (TownScene)
- Spawn Points Configured: 3 (TownEntrance, TrackStart, TrackGarage)

**Version Control:**
- Git Commits: ~8-10 commits
- Branches: sprint-3-scene-transitions
- Tags: v0.3.0-sprint3

### Sprint 3 Success Criteria: ACHIEVED ✅

**Target:** Player switches between driving and walking, moves between scenes

**Achieved:**
- ✅ Player can drive car (from Sprint 1+2)
- ✅ Player can exit car and walk
- ✅ Player can re-enter car from walking mode
- ✅ Player can transition between scenes (track ↔ town)
- ✅ State persists correctly (driving/walking, cash, last scene)
- ✅ Cameras switch appropriately for each state (driving/walking)
- ✅ Spawn points work for both walking and driving modes
- ✅ Character spawns at correct locations in new scenes

**Sprint 3 Grade:** A+ (All core goals met, robust solutions implemented, ahead of schedule, ready for Sprint 4)

---

**Next Update:** After Sprint 4 completion (estimated January 26, 2025)
