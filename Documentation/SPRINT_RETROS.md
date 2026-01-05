# Sprint Retrospectives - CD26 Project

## Sprint 1: Foundation Setup (December 28, 2024)

### Deliverable Achievement
✅ DELIVERED: Player can drive one car around a basic track with camera controls

### What Went Well
- Unity 6.3 LTS project setup smooth
- GitHub integration working cleanly
- PolyStang car drives responsively
- Camera switching implemented (bonus feature that came with polystang vehicle asset!)
- Asset management strategy established
- Material upgrade process needed for URP conversion

### What Could Be Improved
- Camera clips through terrain on steep slopes (deferred to Sprint 2)
- Setting up the repo and initial commit

### Technical Learnings
- Unity Store assets require URP material conversion
- .gitignore strategy: exclude large asset folders, commit .meta files only
- Camera follow needs collision detection for terrain clipping

### Action Items for Sprint 2
- Implement Cinemachine for robust camera system
- Add raycast collision detection to camera controller
- Test on multiple input devices (keyboard, controller, WebGL)

### Velocity Notes
- Sprint completed in 1 day
- All core deliverables met

---

## Sprint 2: Multi-Platform Controls & Basic HUD (January 5, 2025)

### Deliverable Achievement
✅ DELIVERED: Game controllable on PC and WebGL with functional HUD and pause menu

### What Went Well
- Unity Input System implementation smooth (keyboard + gamepad support ready)
- UI Toolkit HUD created with speedometer (km/h) and cash display
- Pause menu with volume control functional
- Cinemachine camera system eliminated terrain clipping
- Console-style auto-follow camera behavior achieved
- WebGL build successful and playable in browser
- All Sprint 2 phases completed methodically (4 phases over ~2 days)

### Challenges & Solutions
- **Challenge:** Namespace collision with `Cursor` class in UI Toolkit
  - **Solution:** Used `UnityEngine.Cursor` to specify correct namespace between UIElements and UnityEngine
- **Challenge:** Camera clipping through terrain on steep hills
  - **Solution:** Cinemachine Collider extension with proper layer configuration
- **Challenge:** Camera not following car during turns (FreeLook default is orbit-style)
  - **Solution:** Changed Heading Definition to "Target Forward" and increased X Axis Max Speed to 100+
- **Challenge:** "No cameras rendering" error when first setting up Cinemachine
  - **Solution:** Ensured Main Camera had CinemachineBrain component attached
- **Challenge:** Camera getting stuck under car with collision detection
  - **Solution:** Set "Collide Against" to only terrain layer (excluded car's layer), increased Minimum Distance

### Technical Learnings
- UI Toolkit workflow: UXML (structure) + USS (styling) + C# (logic)
- Cinemachine FreeLook camera requires explicit heading configuration for auto-follow behavior
- FreeLook "Mouse X/Y" input axis names must be blank for console-style cameras (no manual orbit)
- Cinemachine Collider "Collide Against" must exclude car's layer to prevent self-collision
- WebGL builds require web server (can't open HTML directly in browser)
- UI Toolkit's modular CSS-like styling makes customization straightforward
- CinemachineBrain component acts as the bridge between virtual cameras and physical Camera

### Code Quality Notes
- Clean separation: HUDController (display logic) + PauseMenuController (menu logic)
- Both scripts properly unregister callbacks in OnDestroy (prevents memory leaks)
- Time.timeScale properly reset when scene unloads (prevents frozen game states)
- Used proper null checking before UI element access
- Documented public methods with XML comments for future reference

### Design Decisions
- Chose console-style auto-follow camera over PC orbit camera for better driving feel
- Positioned speedometer bottom-left, cash top-right for minimal obstruction
- Used semi-transparent HUD panels for visibility without blocking gameplay
- Pause menu freezes time completely (Time.timeScale = 0) for clean pause state
- Volume slider defaults to 75% for balanced audio experience

### Action Items for Sprint 3
- Add player state machine (driving/walking transitions)
- Implement scene loading system (track → town)
- Create basic character controller for walking mode
- Test enter/exit vehicle interaction
- Consider adding visual feedback when car is nearby (for future enter/exit prompt)

### Velocity Notes
- Sprint completed in approximately 2 days (Dec 29 start - Jan 5 complete)
- All core deliverables met
- WebGL build functional on first attempt
- 4 phases completed: Input System, UI Toolkit HUD, Cinemachine Camera, WebGL Testing

### Sprint 2 Metrics
- **Files Created:** 7 (2 UXML/USS, 3 C# scripts, 1 Panel Settings, 1 Input Actions asset)
- **Unity Packages Added:** 2 (Input System, Cinemachine)
- **Build Targets Tested:** 2 (PC Standalone, WebGL)
- **Console Errors:** 0 (after fixing Cursor namespace collision)
- **Git Commits:** ~6-8 commits across sprint phases
- **Lines of Code:** ~350 (HUDController: 110, PauseMenuController: 180, FreeLookCameraInput: 60)
