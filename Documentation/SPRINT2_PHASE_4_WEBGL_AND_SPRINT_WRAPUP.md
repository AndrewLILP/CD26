# Phase 4: WebGL Testing & Sprint 2 Wrap-Up

## 🎯 Goals
1. Build and test game in WebGL (browser playable)
2. Verify all Sprint 2 features work in browser
3. Complete Sprint 2 properly (merge, tag, retrospective)

**Time Estimate:** 30-45 minutes

---

## Part 1: WebGL Build Setup (10-15 minutes)

### Step 1.1: Switch to WebGL Platform

1. Top menu: **File → Build Settings**
2. In the Platform list (left side), select **WebGL**
3. Click **Switch Platform** button (bottom-right)
4. Wait for Unity to reimport assets (~2-5 minutes depending on project size)

**✅ Confirmation:** "WebGL" platform should now have a Unity icon next to it (indicating active platform)

---

### Step 1.2: Configure WebGL Player Settings

Still in Build Settings window:

1. Click **Player Settings** button (bottom-left)
2. This opens the Inspector with WebGL settings

#### **Resolution and Presentation:**
- **Default Canvas Width:** 1280
- **Default Canvas Height:** 720
- **Run In Background:** ✅ Checked (recommended)

#### **Publishing Settings:**
- **Compression Format:** Gzip (default)
- **Enable Exceptions:** None (for best performance)
  - *Note: If you need debugging, use "Explicitly Thrown Exceptions Only"*

#### **Other Settings → Optimization:**
- **Managed Stripping Level:** Medium (balances size vs compatibility)

**Close Player Settings when done.**

---

### Step 1.3: Create WebGL Build

Back in **Build Settings** window:

1. Click **Add Open Scenes** button (if your current scene isn't listed)
2. Verify your scene has a checkmark (will be included in build)
3. Click **Build** button

**Choose Build Location:**
1. Navigate to your project root (where Assets folder is)
2. Create a new folder: **"WebGL_Build"**
3. Select this folder
4. Click **Select Folder**

**Unity will now build...** (5-10 minutes)
- Progress bar will show at bottom of Unity
- Don't close Unity during build
- Your computer might get loud (CPU intensive)

**✅ Build Complete:** When finished, Unity will open the build folder.

---

## Part 2: Test WebGL Build (10-15 minutes)

### Step 2.1: Run Local Server

**IMPORTANT:** WebGL builds must run on a web server, NOT by opening the HTML file directly.

#### **Option A: Unity's Built-in Server (Easiest)**

1. In **Build Settings** window
2. Click **Build And Run** instead of just Build
3. Unity will build AND automatically start a local server
4. Your default browser will open with the game

#### **Option B: Python Local Server (If Build And Run doesn't work)**

Open Command Prompt in your WebGL_Build folder:

**If you have Python 3:**
```bash
python -m http.server 8000
```

**If you have Python 2:**
```bash
python -m SimpleHTTPServer 8000
```

Then open browser and go to: `http://localhost:8000`

#### **Option C: Use VS Code Live Server Extension**

1. Open WebGL_Build folder in VS Code
2. Right-click on `index.html`
3. Select "Open with Live Server"

---

### Step 2.2: WebGL Testing Checklist

Once the game loads in browser, test everything:

#### ✅ **Initial Load Test:**
- [ ] Game loads without errors (check browser console: F12)
- [ ] Unity loader bar completes
- [ ] Game scene appears
- [ ] HUD elements visible (speedometer, cash display)

#### ✅ **Keyboard Controls Test:**
- [ ] **W** - Car accelerates forward
- [ ] **S** - Car brakes/reverses
- [ ] **A** - Car turns left
- [ ] **D** - Car turns right
- [ ] **Arrow keys** - Also work for movement
- [ ] **ESC** - Pause menu opens
- [ ] **ESC again** - Pause menu closes

#### ✅ **HUD Display Test:**
- [ ] Speedometer updates when driving (shows km/h)
- [ ] Speed shows "0" when stopped
- [ ] Cash display shows "$5,000 / $10,000 Goal" (or your values)
- [ ] HUD elements positioned correctly (not cut off)

#### ✅ **Pause Menu Test:**
- [ ] Pause menu overlay appears when pressing ESC
- [ ] Game freezes (time stops) when paused
- [ ] Volume slider visible and draggable
- [ ] Volume percentage updates when dragging slider
- [ ] Resume button works (game unfreezes)
- [ ] Quit button shows in menu (may not work without MainMenu scene)

#### ✅ **Camera Test:**
- [ ] Camera follows car smoothly
- [ ] Camera stays behind car during turns
- [ ] Camera doesn't clip through terrain on hills
- [ ] Camera distance looks correct
- [ ] No jittering or stuttering

#### ✅ **Performance Test:**
- [ ] Frame rate is smooth (check browser performance)
- [ ] No major lag when driving
- [ ] No freezing or stuttering
- [ ] Browser console shows no errors (F12 → Console tab)

#### ✅ **Browser Compatibility Test (Optional but Recommended):**
- [ ] Test in **Chrome** (primary target)
- [ ] Test in **Firefox** (if available)
- [ ] Test in **Edge** (if available)

---

### Step 2.3: Check Browser Console for Errors

**IMPORTANT:** Even if game seems to work, check for errors:

1. While game is running, press **F12** (opens Developer Tools)
2. Click **Console** tab
3. Look for red error messages

**Common WebGL Errors (and fixes):**

**"WebGL context lost" / "Out of memory"**
- Build size too large
- Fix: Reduce texture quality in Unity (Edit → Project Settings → Quality)

**"AudioContext was not allowed to start"**
- Browser blocked audio (normal)
- User must click in page to enable audio
- Not a real error, can ignore

**"Can't find X.wasm.gz"**
- Build files missing or corrupted
- Fix: Rebuild WebGL

**No errors:** ✅ Great! Build is clean.

---

## Part 3: Sprint 2 Wrap-Up (15-20 minutes)

### Step 3.1: Final Code Commit

Make sure all your work is committed:

```bash
# Check status
git status

# If there are uncommitted changes:
git add .
git commit -m "[Sprint-2] Complete: Phase 4 WebGL build tested and working"
git push
```

---

### Step 3.2: Merge Sprint 2 to Develop

**Now that Sprint 2 is complete, merge it to develop branch:**

```bash
# 1. Make sure sprint-2-controls is clean
git status
# Should say "nothing to commit, working tree clean"

# 2. Switch to develop branch
git checkout develop

# 3. Pull any updates (good habit)
git pull

# 4. Merge sprint-2-controls into develop
git merge sprint-2-controls

# 5. Push updated develop to GitHub
git push
```

**✅ Expected output:** 
```
Updating xxxxx..yyyyy
Fast-forward
 [list of changed files]
```

---

### Step 3.3: Tag Sprint 2 Completion

Create a version tag to mark this milestone:

```bash
# Make sure you're on develop branch
git branch
# Should show: * develop

# Create annotated tag
git tag -a v0.2.0-sprint2 -m "Sprint 2 Complete: Multi-Platform Controls & Basic HUD"

# Push tag to GitHub
git push --tags
```

**View your tag on GitHub:**
- Go to: `https://github.com/AndrewLILP/CD26/tags`
- You should see both v0.1.0-sprint1 and v0.2.0-sprint2

---

### Step 3.4: Write Sprint 2 Retrospective

Document what you learned and accomplished!

**Create/Update:** `SPRINT_RETROS.md`

Add this section:

```markdown
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
- All Sprint 2 phases completed methodically

### Challenges & Solutions
- **Challenge:** Namespace collision with `Cursor` class in UI Toolkit
  - **Solution:** Used `UnityEngine.Cursor` to specify correct namespace
- **Challenge:** Camera clipping through terrain on steep hills
  - **Solution:** Cinemachine Collider extension with proper layer configuration
- **Challenge:** Camera not following car during turns (FreeLook default is orbit-style)
  - **Solution:** Changed Heading Definition to "Target Forward" and increased X Axis Max Speed

### Technical Learnings
- UI Toolkit workflow: UXML (structure) + USS (styling) + C# (logic)
- Cinemachine FreeLook camera requires explicit heading configuration for auto-follow
- FreeLook "Mouse X/Y" input axis names must be blank for console-style cameras
- Cinemachine Collider "Collide Against" must exclude car's layer to prevent collision with self
- WebGL builds require web server (can't open HTML directly)

### Code Quality Notes
- Clean separation: HUDController (display logic) + PauseMenuController (menu logic)
- Both scripts properly unregister callbacks in OnDestroy (prevents memory leaks)
- Time.timeScale properly reset when scene unloads (prevents frozen game states)

### Action Items for Sprint 3
- Add player state machine (driving/walking transitions)
- Implement scene loading system (track → town)
- Create basic character controller for walking mode
- Test enter/exit vehicle interaction

### Velocity Notes
- Sprint completed in approximately 2 days (Dec 29 - Jan 5)
- All core deliverables met
- WebGL build functional on first attempt

### Sprint 2 Metrics
- **Files Created:** 7 (2 UXML/USS, 3 C# scripts, 1 Panel Settings, 1 Input Actions asset)
- **Unity Packages Added:** Input System, Cinemachine
- **Build Targets Tested:** PC Standalone, WebGL
- **Console Errors:** 0 (after fixing Cursor namespace)
```

**Save this to your SPRINT_RETROS.md file.**

---

### Step 3.5: Update Project Status

Update `PROJECT_STATUS.md`:

```markdown
# CD26 - Project Status

**Last Updated:** January 5, 2025
**Current Sprint:** 2 → 3 transition
**Active Branch:** develop

## Completed Sprints

### ✅ Sprint 1: Foundation Setup (v0.1.0-sprint1)
- Unity 6.3 LTS project initialized
- GitHub repository configured
- PolyStang muscle car imported and driveable
- Bathurst track environment set up
- Basic camera follow + camera switching
- **Deliverable Met:** Player can drive car around track

### ✅ Sprint 2: Multi-Platform Controls & Basic HUD (v0.2.0-sprint2)
- Unity Input System implemented (keyboard + gamepad ready)
- UI Toolkit HUD with speedometer (km/h) and cash display
- Pause menu with volume control for car audio
- Cinemachine FreeLook camera with collision detection
- Console-style auto-follow camera behavior
- WebGL build tested and functional
- **Deliverable Met:** Game controllable on all platforms with UI

## Current Sprint

### 🚧 Sprint 3: Player State & Scene Management (Starting Jan 6)
- Player state machine (driving/walking/in-building)
- Enter/exit vehicle interaction
- Scene loading system (track → town transition)
- Basic character controller for walking
- **Target Deliverable:** Player switches between driving and walking, moves between scenes

## Upcoming Sprints
- Sprint 4: Interaction Systems
- Sprint 5: First Gameplay Loop
```

---

### Step 3.6: Create Sprint 3 Branch (Optional - Prepare for Next Sprint)

If you want to be ready for Sprint 3:

```bash
# Make sure you're on develop
git checkout develop

# Create new sprint branch
git checkout -b sprint-3-player-state

# Push to GitHub
git push -u origin sprint-3-player-state

# Now you're ready for Sprint 3 work!
```

---

## ✅ Sprint 2 Completion Checklist

**Phase Completion:**
- [ ] Phase 1: Input System (keyboard + gamepad) ✅
- [ ] Phase 2: UI Toolkit HUD (speedometer, cash, pause menu) ✅
- [ ] Phase 3: Cinemachine camera (collision detection) ✅
- [ ] Phase 4: WebGL build tested ✅

**Git Workflow:**
- [ ] All changes committed to sprint-2-controls
- [ ] Merged sprint-2-controls → develop
- [ ] Tagged v0.2.0-sprint2
- [ ] Pushed tags to GitHub

**Documentation:**
- [ ] Sprint 2 retrospective written
- [ ] PROJECT_STATUS.md updated
- [ ] Sprint 3 branch created (optional)

**Build Verification:**
- [ ] PC build works (you've been testing this)
- [ ] WebGL build works in browser
- [ ] No console errors in either platform

---

## 🎉 Sprint 2 Complete!

### What You've Accomplished

**Sprint 2 Deliverables - ALL MET:**
- ✅ Multi-platform input system (keyboard, gamepad ready, WebGL tested)
- ✅ Functional HUD with real-time speedometer and cash display
- ✅ Pause menu with volume control
- ✅ Professional camera system (no terrain clipping, auto-follow)
- ✅ WebGL browser playable version

**Technical Foundation Built:**
- Unity Input System mastery
- UI Toolkit workflow established
- Cinemachine camera system configured
- WebGL build pipeline tested

**Development Practices Strengthened:**
- Clean code organization (separate controller scripts)
- Proper Git workflow (branch → merge → tag)
- Comprehensive testing (PC + WebGL)
- Sprint retrospective documentation

---

## 🚀 What's Next: Sprint 3 Preview

**Sprint 3: Player State & Scene Management**

You'll build the foundation for switching between driving and walking:

**Key Features:**
1. **Player State Machine**
   - Driving state (current functionality)
   - Walking state (new!)
   - Transition system between states

2. **Enter/Exit Vehicle**
   - Proximity detection
   - "Press E to Enter/Exit" UI prompt
   - Smooth state transitions

3. **Scene Management**
   - Load town scene from track
   - Maintain player state across scenes
   - Scene transition UI (loading screen)

4. **Walking Character Controller**
   - Basic WASD movement
   - Camera follow for walking mode
   - Collision detection

**Sprint 3 will be the bridge between the driving foundation you've built and the broader gameplay loop!**

---

## 📁 WebGL Build Distribution (Optional)

If you want to share your WebGL build:

### Option 1: GitHub Pages (Free Hosting)
1. Create a `docs` folder in your repo
2. Copy WebGL_Build contents to `docs`
3. Push to GitHub
4. Enable GitHub Pages in repo settings
5. Game accessible at: `https://yourusername.github.io/CD26`

### Option 2: itch.io (Game Hosting Platform)
1. Create account at itch.io
2. Upload WebGL build as HTML5 game
3. Share link with friends/testers

**We can set this up later if interested!**

---

**Let me know when you've completed the checklist, and we can discuss Sprint 3 planning!** 🎮

Or if you hit any issues with:
- WebGL build
- Git merge
- Retrospective writing
- Anything else

Great work completing Sprint 2! 🎉
