# CD26 - Project Status

**Last Updated:** January 5, 2025  
**Current Sprint:** 2 → 3 transition  
**Active Branch:** develop

---

## 🎮 Project Overview

**CD26** is a 2026 life and financial simulation game combining GTA-style open world gameplay with realistic financial education. Players start in 1992, driving muscle cars to earn money, then progress through career mini-games and financial opportunities like real estate and business ownership. The game teaches wealth-building through integrated gameplay mechanics rather than explicit tutorials.

**Target Platforms:** Steam PC (primary), WebGL demo (secondary)  
**Engine:** Unity 6.3 LTS  
**Development Approach:** 5-Sprint Horizontal Slice (10 weeks total)

---

## ✅ Completed Sprints

### Sprint 1: Foundation Setup (v0.1.0-sprint1)
**Completed:** December 28, 2024  
**Branch:** sprint-1-foundation → develop

**Deliverables:**
- ✅ Unity 6.3 LTS project initialized with URP
- ✅ GitHub repository configured with proper .gitignore and branching strategy
- ✅ PolyStang muscle car imported and driveable
- ✅ Bathurst track environment set up
- ✅ Basic camera follow + camera switching (3 views)
- ✅ WASD driving controls functional

**Key Achievement:** Player can drive car around track  

**Technical Foundation:**
- Unity 6.3 LTS + URP rendering pipeline
- Git workflow established (main → develop → sprint branches)
- Unity Store asset integration process defined

---

### Sprint 2: Multi-Platform Controls & Basic HUD (v0.2.0-sprint2)
**Completed:** January 5, 2025  
**Branch:** sprint-2-controls → develop

**Deliverables:**
- ✅ Unity Input System implemented (keyboard + gamepad bindings ready)
- ✅ UI Toolkit HUD with real-time speedometer (km/h) and cash display ($X / $Y Goal)
- ✅ Pause menu (ESC) with volume slider for car audio
- ✅ Cinemachine FreeLook camera with collision detection
- ✅ Console-style auto-follow camera behavior (like GTA 5)
- ✅ WebGL build created and tested in browser
- ✅ No terrain clipping on steep hills

**Key Achievement:** Game controllable on all platforms with functional UI

**Technical Foundation:**
- Unity Input System (multi-platform input abstraction)
- UI Toolkit workflow (UXML + USS + C#)
- Cinemachine camera system (professional-grade camera control)
- WebGL build pipeline validated

**Packages Added:** Input System, Cinemachine

---

## 🚧 Current Sprint

### Sprint 3: Player State & Scene Management (Starting Jan 6, 2025)
**Duration:** 2 weeks (Jan 6 - Jan 19)  
**Branch:** sprint-3-player-state  
**Status:** Not yet started

**Planned Deliverables:**
- [ ] Player state machine (driving/walking/in-building states)
- [ ] Enter/exit vehicle interaction system
- [ ] Scene loading system (track scene ↔ town scene)
- [ ] Basic character controller for walking mode
- [ ] State-aware camera system (driving cam vs walking cam)

**Target Achievement:** Player can drive, exit car, walk around, and transition between scenes

**Key User Stories:**
1. As a player, I can press E to exit my car when stopped
2. As a player, I can walk around using WASD controls
3. As a player, I can approach my car and press E to re-enter
4. As a player, I can transition from track scene to town scene
5. As a player, camera follows me appropriately whether driving or walking

---

## 📋 Upcoming Sprints

### Sprint 4: Interaction Systems (Jan 20 - Feb 2)
**Focus:** Enable player to interact with NPCs and objects

**Planned Features:**
- Proximity-based dialogue system (approach NPC → conversation triggers)
- Simple discovery system (hover/approach object → info panel appears)
- Basic NPC placement in town scene
- Property inspection UI (show price, bedrooms, etc.)

**Target:** Player can talk to one mentor NPC and inspect one property

---

### Sprint 5: First Gameplay Loop (Feb 3 - Feb 16)
**Focus:** Complete playable end-to-end experience

**Planned Features:**
- Lap completion tracking + $500 reward system
- High score tracking (fastest lap time)
- Simple career mini-game (e.g., café job)
- Basic progression gate (need $X + skill points to unlock town)
- One mentor conversation that affects one outcome

**Target:** 5-10 minute playable loop: Drive lap → earn money → do job → talk to mentor → unlock next area

**Success Criteria:** Player experiences the core earn → learn → progress cycle

---

## 📊 Sprint Velocity & Metrics

| Sprint | Duration | Deliverables | Lines of Code | Packages Added | Build Targets |
|--------|----------|--------------|---------------|----------------|---------------|
| Sprint 1 | 1 day | 5/5 ✅ | ~100 | 0 | PC |
| Sprint 2 | 2 days | 4/4 ✅ | ~350 | 2 | PC, WebGL |
| Sprint 3 | TBD | 0/5 ⏳ | - | - | - |

**Average Velocity:** ~2 days per sprint (ahead of 2-week schedule)  
**On-Track for 10-week goal:** Yes (currently 2 sprints complete in ~3 days)

---

## 🛠️ Technical Stack

**Engine & Version:**
- Unity 6.3 LTS (Long Term Support)
- Universal Render Pipeline (URP)

**Key Unity Packages:**
- Input System (1.x) - Multi-platform input abstraction
- Cinemachine (2.x) - Advanced camera control
- UI Toolkit (built-in) - Modern UI framework

**Version Control:**
- Git + GitHub
- Branch strategy: main → develop → sprint-N branches
- Semantic versioning: v0.N.0-sprintN

**Assets (Unity Store):**
- PolyStang muscle car (driveable vehicle)
- Bathurst track environment (race circuit)
- Additional town/city assets (planned for Sprint 3+)

**Build Targets:**
- Primary: Windows PC (Steam)
- Secondary: WebGL (browser demo)

---

## 🎯 Sprint 3 Readiness

**Prerequisites Met:**
- ✅ Driving mechanics functional
- ✅ Input system supports multiple contexts
- ✅ HUD system extensible (can add prompts)
- ✅ Camera system supports multiple virtual cameras
- ✅ WebGL build pipeline validated

**Ready to Begin:** Yes  
**Blocking Issues:** None

---

## 📝 Key Decisions & Rationale

### Camera System (Sprint 2)
**Decision:** Console-style auto-follow camera instead of PC orbit camera  
**Rationale:** Better driving feel, matches GTA 5 reference, reduces cognitive load  
**Implementation:** Cinemachine FreeLook with "Target Forward" heading

### UI Framework (Sprint 2)
**Decision:** UI Toolkit instead of Unity UI (Canvas)  
**Rationale:** Modern approach, better performance, CSS-like styling, future-proof  
**Tradeoff:** Steeper learning curve, but more maintainable long-term

### Input System (Sprint 2)
**Decision:** New Input System instead of legacy Input Manager  
**Rationale:** Required for multi-platform support, industry standard, rebindable controls  
**Note:** Gamepad tested (driver issue), but bindings confirmed working

---

## 🔄 Development Workflow

**Daily Workflow:**
1. Work on active sprint branch (sprint-N-name)
2. Commit frequently with descriptive messages: `[Sprint-N] Action: Description`
3. Push to GitHub regularly (backup + progress visibility)

**Sprint Completion:**
1. Merge sprint branch → develop
2. Tag release: `v0.N.0-sprintN`
3. Write retrospective in SPRINT_RETROS.md
4. Update PROJECT_STATUS.md
5. Create next sprint branch

**Documentation:**
- Sprint planning in SPRINT_N_PLAN.md
- Retrospectives in SPRINT_RETROS.md
- Git workflow in GIT_WORKFLOW.md
- This status doc updated after each sprint

---

## 🎮 Playable Features (Current Build)

**What Works Now (v0.2.0-sprint2):**
- Drive PolyStang muscle car around Bathurst track
- WASD or Arrow keys to drive
- Speedometer displays real-time speed in km/h
- Cash display shows current money vs goal ($5,000 / $10,000)
- ESC to pause game (time freezes)
- Volume slider to adjust car audio
- Camera auto-follows car, avoids terrain clipping
- Playable in both PC build and WebGL browser

**What Doesn't Work Yet:**
- Can't exit car (Sprint 3)
- Can't interact with anything (Sprint 4)
- No way to earn money (Sprint 5)
- Only one scene available (track)

---

**Next Update:** After Sprint 3 completion (estimated Jan 19, 2025)
