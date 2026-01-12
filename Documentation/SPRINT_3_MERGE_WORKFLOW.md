# Sprint 3: Professional Merge & Release Workflow

## Prerequisites Checklist

Before merging, ensure:
- [ ] All Sprint 3 features are tested and working
- [ ] Console has no critical errors
- [ ] All files are committed to sprint-3-scene-transitions branch
- [ ] Sprint retrospective is written (SPRINT_RETROS.md)
- [ ] PROJECT_STATUS.md is updated

---

## Step-by-Step Merge Workflow

### 1. Final Commit on Sprint Branch

```bash
# Ensure you're on the sprint branch
git branch

# Stage all changes
git add .

# Final commit
git commit -m "[Sprint-3] Complete: Scene transitions and state management system"

# Push to GitHub
git push origin sprint-3-scene-transitions
```

---

### 2. Switch to Develop Branch

```bash
# Switch to develop
git checkout develop

# Pull latest changes (if working with others)
git pull origin develop

# Verify you're on develop
git branch
```

---

### 3. Merge Sprint 3 into Develop

```bash
# Merge sprint-3 into develop
git merge sprint-3-scene-transitions

# If merge conflicts occur:
# 1. Resolve conflicts in Unity/VS Code
# 2. For .unity scene files, typically accept the incoming (sprint branch) version
# 3. Stage resolved files: git add .
# 4. Complete merge: git commit

# Push merged develop branch
git push origin develop
```

---

### 4. Create Professional Release Tag

```bash
# Create annotated tag with detailed message
git tag -a v0.3.0-sprint3 -m "Sprint 3 Complete: Scene Transitions - Player state management, vehicle entry/exit, async scene loading with spawn system"

# Push tag to GitHub
git push --tags
```

**Tag Breakdown:**
- `v0.3.0` = Version (major.minor.patch)
- `-sprint3` = Sprint identifier
- Message format: `"Sprint X Complete: [Feature Category] - [Key deliverables]"`

---

### 5. Verify Merge Success

```bash
# View recent commit history with graph
git log --oneline --graph --decorate -10

# Check working directory is clean
git status

# Verify tag was created
git tag -l

# View tag details
git show v0.3.0-sprint3

# Verify remote has tag
git ls-remote --tags origin
```

**Expected Output:**
```
* commit abc1234 (HEAD -> develop, tag: v0.3.0-sprint3, origin/develop, origin/sprint-3-scene-transitions)
| [Sprint-3] Complete: Scene transitions and state management system
|
* commit def5678
| [Sprint-3] Fix: Vehicle detection with GetComponentInParent
|
* commit ghi9012
| [Sprint-3] Fix: DontDestroyOnLoad for child GameObjects
```

---

### 6. Update Documentation

#### Update PROJECT_STATUS.md

```markdown
## ✅ Completed Sprints

### Sprint 3: Player State & Scene Management (v0.3.0-sprint3)
**Completed:** January 12, 2025  
**Branch:** sprint-3-scene-transitions → develop

**Deliverables:**
- ✅ Player state machine (Driving/Walking/InBuilding)
- ✅ Enter/exit vehicle interaction (E key)
- ✅ Scene loading system (async with state persistence)
- ✅ Character controller for walking mode
- ✅ Spawn point system for scene transitions
- ✅ Camera state management (priority-based switching)

**Key Achievement:** Player can drive, exit vehicle, walk, and transition between scenes with state persistence

**Technical Foundation:**
- PlayerStateManager (singleton state machine)
- SceneTransitionManager (async scene loading)
- SceneTransitionZone (trigger-based transitions)
- StateAware controllers (car/character)
- Spawn point system (walking/driving modes)

---

## 🚧 Current Sprint

### Sprint 4: Interaction Systems (Starting Jan 13, 2025)
**Duration:** 2 weeks (Jan 13 - Jan 26)
**Branch:** sprint-4-interactions
**Status:** Not yet started
```

#### Update SPRINT_RETROS.md

```markdown
---

## Sprint 3: Player State & Scene Management (January 12, 2025)

### Deliverable Achievement
✅ DELIVERED: Player can drive, exit vehicle, walk, and transition between scenes

### What Went Well
- State machine architecture cleanly separates concerns
- DontDestroyOnLoad pattern works reliably with transform.root
- Async scene loading with proper timing prevents spawn issues
- Vehicle detection solved with GetComponentInParent() approach
- Camera priority system eliminates black screen transitions
- Event-driven architecture makes state changes predictable
- Extensive debugging led to robust, reusable solutions

### Challenges & Solutions
- **Challenge:** DontDestroyOnLoad failed with child GameObjects
  - **Solution:** Use transform.root.gameObject to persist entire hierarchy
- **Challenge:** Spawn points not found after scene load
  - **Solution:** Wait multiple frames (WaitForSeconds + WaitForEndOfFrame) for initialization
- **Challenge:** Vehicle colliders couldn't trigger zones (component on parent)
  - **Solution:** GetComponentInParent() searches up hierarchy from child colliders
- **Challenge:** CharacterController position wouldn't set directly
  - **Solution:** Temporarily disable CharacterController during teleportation

### Technical Learnings
- AsyncOperation.isDone ≠ scene objects initialized (need additional wait time)
- DontDestroyOnLoad only works on root GameObjects
- CharacterController requires disable/enable pattern for teleportation
- GetComponentInParent() essential when child colliders trigger events
- Cinemachine priority switching cleaner than GameObject.SetActive()
- State persistence pattern: Save → Transition → Spawn → Restore

### Code Quality Notes
- Singleton pattern used consistently across managers
- UnityEvent system enables loose coupling between systems
- Extensive debug logging aids troubleshooting (can be reduced for production)
- Proper cleanup in OnDestroy() prevents memory leaks
- Clear separation: state management, scene loading, vehicle interaction

### Design Decisions
- Chose priority-based camera switching over SetActive() for smoother blending
- Implemented delayed GameObject disabling (0.3s) to allow camera transitions
- Used trigger zones instead of distance checks for more precise control
- Separated "exit vehicle" behavior from "scene transition" for flexibility
- Character spawns 1.5m to right of vehicle on exit (feels natural)

### Action Items for Sprint 4
- Fix HUD persistence across scenes (currently disappears)
- Add visual UI prompts for interactions (E to Enter/Exit/Talk)
- Implement proximity-based dialogue system
- Create property inspection UI panel
- Place NPCs in TownScene
- Remove debug keybinds (1/2/3) before release

### Velocity Notes
- Sprint completed in 6 days (target was 14 days)
- All 6 core deliverables met
- 4 major technical blockers resolved
- 7 new scripts created (~1,270 lines of code)

### Sprint 3 Metrics
- **Files Created:** 7 (PlayerStateManager, StateAwareCarController, StateAwareCharacterController, CameraStateManager, VehicleInteraction, SceneTransitionManager, SceneTransitionZone, SceneSpawnPoint)
- **Files Modified:** 2 (HUDController, CameraSwitcher)
- **Lines of Code:** ~1,270 new lines
- **Unity Packages Added:** 0 (used existing Starter Assets)
- **Console Errors:** 0 (after fixes)
- **Git Commits:** ~8-10 commits across sprint phases
```

---

### 7. Create Sprint 4 Branch

```bash
# Ensure you're on develop
git checkout develop

# Create new sprint branch from develop
git checkout -b sprint-4-interactions

# Push new branch to GitHub
git push -u origin sprint-4-interactions

# Verify branch creation
git branch -a
```

---

## 📋 Post-Merge Verification Checklist

After merging, verify everything works:

### In Unity:
- [ ] Open CircuitDreams101 scene
- [ ] Press Play
- [ ] Walk to car and press E (should enter vehicle)
- [ ] Drive car, press E (should exit vehicle)
- [ ] Walk to TownCube, press E (should load TownScene)
- [ ] Verify character spawns at correct location in TownScene
- [ ] Check console for errors
- [ ] Test return transition to CircuitDreams101

### In Git:
- [ ] `git status` shows clean working directory
- [ ] `git log` shows Sprint 3 commits on develop
- [ ] `git tag -l` shows v0.3.0-sprint3
- [ ] GitHub shows tag in releases section
- [ ] sprint-4-interactions branch exists and is active

---

## 🎯 Summary of Commands

```bash
# 1. Final commit on sprint branch
git add .
git commit -m "[Sprint-3] Complete: Scene transitions and state management system"
git push origin sprint-3-scene-transitions

# 2. Switch to develop
git checkout develop
git pull origin develop

# 3. Merge sprint into develop
git merge sprint-3-scene-transitions
git push origin develop

# 4. Create and push tag
git tag -a v0.3.0-sprint3 -m "Sprint 3 Complete: Scene Transitions - Player state management, vehicle entry/exit, async scene loading with spawn system"
git push --tags

# 5. Verify
git log --oneline --graph --decorate -10
git status
git tag -l
git show v0.3.0-sprint3

# 6. Create Sprint 4 branch
git checkout -b sprint-4-interactions
git push -u origin sprint-4-interactions
```

---

## 📊 Release Notes Template (for GitHub)

After pushing the tag, create a release on GitHub:

**Release Title:** `v0.3.0-sprint3 - Scene Transitions & State Management`

**Release Notes:**
```markdown
# CD26 v0.3.0 - Sprint 3: Scene Transitions Complete

## 🎮 New Features

- **Player State System**: Seamless transitions between driving and walking modes
- **Vehicle Interaction**: Enter/exit vehicles using E key with proximity detection
- **Scene Transitions**: Load between multiple scenes (Circuit ↔ Town) with async loading
- **State Persistence**: Player state, cash, and progress persist across scene changes
- **Spawn System**: Configurable spawn points for both walking and driving modes
- **Camera Management**: Priority-based camera switching prevents black screens

## 🔧 Technical Improvements

- Implemented singleton managers (PlayerStateManager, SceneTransitionManager)
- Fixed DontDestroyOnLoad with child GameObjects using transform.root pattern
- Solved spawn point timing with proper async loading delays
- Vehicle detection uses GetComponentInParent for child colliders
- CharacterController teleportation with disable/enable pattern

## 🎯 Sprint 3 Goals

- ✅ Player state machine (Driving/Walking/InBuilding)
- ✅ Enter/exit vehicle interaction
- ✅ Scene loading system
- ✅ Basic character controller
- ✅ State persistence across scenes

## 📝 Known Issues

- HUD (speed/cash display) disappears after scene transition (fix planned for Sprint 4)
- Interaction prompts only show in console (visual UI planned for Sprint 4)
- Debug keybinds (1/2/3) still active (will remove before release)

## 🚀 What's Next (Sprint 4)

- Proximity-based dialogue system
- NPC interactions
- Property inspection UI
- Visual interaction prompts
- HUD persistence fix

---

**Build Tested:** Unity 6.3 LTS  
**Platforms:** PC Standalone, WebGL  
**Sprint Duration:** 6 days (Jan 6-12, 2025)  
**Commits:** 8-10 commits  
**Lines of Code:** ~1,270 new lines
```

---

## 🎉 Sprint 3 Complete!

**Achievement Unlocked:** Scene Transitions & State Management System ✅

**Next Sprint:** Sprint 4 - Interaction Systems (starting sprint-4-interactions branch)

---

**Last Updated:** January 12, 2025  
**Branch:** develop (tag: v0.3.0-sprint3)  
**Status:** ✅ Ready for Sprint 4
