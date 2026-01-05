# Phase 3: Cinemachine Camera Setup - Fix Terrain Clipping

## 🎯 Goal
Replace basic camera follow with Cinemachine FreeLook camera that never clips through terrain.

**Problem we're solving:** Camera passes through terrain on steep hills/banks.

**Solution:** Cinemachine's built-in collision detection pushes camera forward when obstacles detected.

---

## Part 1: Install Cinemachine Package

### Step 1.1: Open Package Manager
1. Top menu: **Window → Package Manager**
2. In Package Manager window, click dropdown (top-left) and select: **Unity Registry**

### Step 1.2: Find and Install Cinemachine
1. Search bar: Type **"Cinemachine"**
2. Click on **Cinemachine** in the results
3. Click **Install** button (bottom-right)
4. Wait for installation to complete (5-10 seconds)
5. Close Package Manager when done

**✅ Confirmation:** You should now see "Cinemachine" in the top menu bar.

---

## Part 2: Set Up FreeLook Camera

### Step 2.1: Disable/Remove Old Camera System

**Important:** We need to disable your current camera setup to avoid conflicts.

**Option A: If you have a custom camera follow script:**
1. Find your car's camera GameObject in Hierarchy
2. Select it
3. In Inspector, **uncheck** the script component (disable it)
4. Don't delete it yet - we might want to reference it later

**Option B: If using PolyStang's built-in camera:**
1. Find the camera switcher/follow scripts on your car or camera
2. Disable those components
3. Keep the actual Camera component enabled

**Result:** Car should still be visible, but camera won't follow anymore when you play.

---

### Step 2.2: Create Cinemachine FreeLook Camera

1. **In Hierarchy**, right-click → **Cinemachine → FreeLook Camera**
2. Rename it: **"CM_FreeLook_DrivingCam"**

**What just happened:**
- A new GameObject with Cinemachine Virtual Camera component was created
- Unity also created a "CM vcam1" brain (or similar) - this is normal

---

### Step 2.3: Configure FreeLook Camera - Basic Setup

Select **CM_FreeLook_DrivingCam** in Hierarchy.

#### Inspector → Cinemachine Virtual Camera Component:

**Follow & Look At Targets:**
- **Follow:** Drag your **car GameObject** here (the root car object, not a wheel)
- **Look At:** Drag your **car GameObject** here (same object)

**Lens Settings:**
- **Field of View:** 60 (default, adjust later for feel)

**Rig Settings** (these are the 3 orbital rings):

**Top Rig:**
- **Height:** 4
- **Radius:** 6

**Middle Rig:**
- **Height:** 2
- **Radius:** 10

**Bottom Rig:**
- **Height:** 0.5
- **Radius:** 6

**Binding Mode:** 
- Set to **World Space** (prevents camera rotation issues)

**X Axis (Horizontal rotation):**
- **Value:** 0
- **Max Speed:** 300 (how fast camera orbits)
- **Accel/Decel Time:** 0.1

**Y Axis (Vertical movement between rigs):**
- **Value:** 0.5 (starts at middle rig)
- **Max Speed:** 2
- **Accel/Decel Time:** 0.2

---

### Step 2.4: Test Basic Functionality

**Before adding collision detection, let's make sure it works:**

1. Enter **Play Mode**
2. Hold **Right Mouse Button** and move mouse
   - Left/Right: Camera should orbit around car
   - Up/Down: Camera should move between high/middle/low positions
3. Drive the car - camera should follow smoothly

**✅ Expected:** Camera follows car smoothly, you can orbit with right-click.

**❌ If it doesn't work:**
- Check Follow/Look At are assigned to your car
- Make sure old camera scripts are disabled
- Verify car Rigidbody isn't kinematic

---

## Part 3: Add Collision Detection (Fix Terrain Clipping!)

### Step 3.1: Add Cinemachine Collider Extension

Still on **CM_FreeLook_DrivingCam**:

1. In Inspector, scroll to bottom of component
2. Click **Add Extension** dropdown
3. Select: **CinemachineCollider**

**A new "Extensions" section appears with CinemachineCollider.**

---

### Step 3.2: Configure Collision Settings

**Cinemachine Collider Settings:**

**Collide Against:**
- Click the dropdown
- Select: **Default** and **Terrain** (or whatever layer your track is on)
  - If your track uses a custom layer, select that instead
  - You can check your track's layer by selecting it and looking at top-right of Inspector

**Ignore Tag:**
- Leave blank (or set to "Player" if you tagged your car)

**Minimum Distance From Target:**
- Set to: **0.2**
- (Prevents camera from getting TOO close when pushed forward)

**Avoid Obstacles:**
- **Distance Limit:** 2 (how far ahead to check for obstacles)
- **Camera Radius:** 0.2 (size of the "sphere" that detects collisions)

**Strategy:**
- **Pull Camera Forward** (default - best for driving games)
- This pushes camera toward car when hitting terrain

**Maximum Effort:**
- Set to: **4** (how hard it tries to maintain view)

**Smoothing Time:**
- Set to: **0.5** (how smoothly it recovers from collisions)

**Damping:**
- **Damping:** 0.5
- **Damping When Occluded:** 0

---

### Step 3.3: CRITICAL - Set Up Layers Correctly

**This is the most common reason collision detection fails!**

#### Ensure Your Track Has Colliders:

1. Select your track GameObject in Hierarchy
2. Check Inspector - does it have a **Mesh Collider** or **Terrain Collider**?
   - ✅ **YES:** Good, proceed
   - ❌ **NO:** You need to add one

**To add collider to track (if missing):**
- Select track → Add Component → **Mesh Collider**
- Check **Convex** if needed (for complex meshes)

#### Verify Layers:

1. Select your track
2. Inspector → Top-right corner → **Layer** dropdown
3. Note which layer it's on (probably "Default")
4. Make sure CinemachineCollider's "Collide Against" includes this layer

---

## Part 4: Testing & Refinement

### Test 4.1: Test Collision Detection

1. **Enter Play Mode**
2. Drive toward a steep hill or wall
3. **Watch the camera:**
   - ✅ **Success:** Camera stops before clipping, pushes forward
   - ❌ **Fail:** Camera still clips through terrain

**If it still clips:**
- Double-check "Collide Against" layers
- Verify track has colliders
- Increase "Camera Radius" to 0.5
- Increase "Distance Limit" to 3

---

### Test 4.2: Bathurst Track Steep Sections

**Your Bathurst track should have some steep banking/hills - perfect for testing!**

1. Drive to the steepest part of the track
2. Try to force camera into the hillside by:
   - Driving along steep banks
   - Stopping on slopes
   - Reversing into hills
3. Right-click and try to rotate camera into terrain

**✅ Camera should never fully clip through - it should push forward instead.**

---

### Test 4.3: Fine-Tune Camera Feel

Now that collision works, adjust for comfort:

**Camera too far/close?**
- Adjust **Top/Middle/Bottom Rig Radius** values
- Larger = camera farther from car

**Camera too high/low?**
- Adjust **Top/Middle/Bottom Rig Height** values

**Camera orbits too fast/slow?**
- Adjust **X Axis Max Speed** (horizontal orbit)

**Camera feels "floaty"?**
- Reduce **Accel/Decel Time** on X and Y axes

**Camera pushes away too aggressively?**
- Reduce **Maximum Effort** (try 2-3)
- Increase **Smoothing Time** (try 1.0)

**Suggested "sporty" driving feel:**
```
Top Rig:    Height: 4,   Radius: 8
Middle Rig: Height: 2.5, Radius: 12
Bottom Rig: Height: 0.8, Radius: 7

X Axis Max Speed: 250
Y Axis Max Speed: 2
```

**Suggested "cinematic" feel:**
```
Top Rig:    Height: 6,   Radius: 10
Middle Rig: Height: 3,   Radius: 15
Bottom Rig: Height: 1,   Radius: 8

X Axis Max Speed: 150 (slower, more dramatic)
Y Axis Max Speed: 1.5
```

Try different values and see what feels best!

---

## Part 5: Re-Enable Camera Switching (Optional)

**If you want to keep multiple camera views** (like the PolyStang asset had):

You have two options:

### Option A: Use Cinemachine Priority System

1. Create additional FreeLook cameras with different settings:
   - CM_FreeLook_FarView (higher radius)
   - CM_FreeLook_HoodCam (low height, close radius)
   - CM_FreeLook_ChaseView (your current one)

2. Create a simple script to toggle priorities:

```csharp
// Example - swap which camera is active
public class CinemachineCameraSwitcher : MonoBehaviour
{
    public CinemachineVirtualCamera[] cameras;
    private int currentIndex = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) // Or use your Input Action
        {
            // Disable current
            cameras[currentIndex].Priority = 0;
            
            // Next camera
            currentIndex = (currentIndex + 1) % cameras.Length;
            
            // Enable new
            cameras[currentIndex].Priority = 10;
        }
    }
}
```

### Option B: Keep PolyStang Cameras as Backup

- Keep your Cinemachine camera as the main view
- Leave old cameras disabled
- Can re-enable later if needed for comparison

**For now, recommend sticking with one Cinemachine camera until Sprint 2 complete.**

---

## 🐛 Troubleshooting

### "Camera still clips through terrain!"

**Check 1: Layers**
- Track layer matches "Collide Against" in CinemachineCollider?
- Track has a collider component?

**Check 2: Collision Settings**
- Camera Radius: Try increasing to 0.5 or 1.0
- Distance Limit: Try increasing to 3 or 4

**Check 3: Track Collider**
- Select track → Does it have MeshCollider/TerrainCollider?
- If MeshCollider, is it baked properly? (reimport asset if needed)

### "Camera jitters/stutters"

**Fix:**
- Cinemachine Brain component should be on your Main Camera
- Select Main Camera → Check for Cinemachine Brain component
- Set **Update Method** to **Smart Update** or **Fixed Update**

### "Camera feels too stiff/robotic"

**Fix:**
- Increase Damping values on X/Y axes
- Increase Accel/Decel times
- Adjust Rig transitions (Spline Curvature)

### "Camera doesn't orbit with mouse"

**You need to add input!** By default, FreeLook needs manual input setup.

**Quick Fix - Add this script to CM_FreeLook_DrivingCam:**

```csharp
using UnityEngine;
using Cinemachine;

public class FreeLookCameraInput : MonoBehaviour
{
    private CinemachineFreeLook freeLook;

    void Start()
    {
        freeLook = GetComponent<CinemachineFreeLook>();
    }

    void Update()
    {
        // Only rotate camera when right mouse button held
        if (Input.GetMouseButton(1))
        {
            freeLook.m_XAxis.m_InputAxisValue = Input.GetAxis("Mouse X");
            freeLook.m_YAxis.m_InputAxisValue = Input.GetAxis("Mouse Y");
        }
        else
        {
            // Stop rotation when not holding right-click
            freeLook.m_XAxis.m_InputAxisValue = 0;
            freeLook.m_YAxis.m_InputAxisValue = 0;
        }
    }
}
```

Save as `FreeLookCameraInput.cs`, attach to CM_FreeLook_DrivingCam.

---

## ✅ Phase 3 Completion Checklist

- [ ] Cinemachine package installed
- [ ] FreeLook camera created and following car
- [ ] CinemachineCollider extension added
- [ ] Collision detection configured (layers correct)
- [ ] Track has collider component
- [ ] Camera tested on steep terrain - no clipping
- [ ] Camera feel adjusted to preference
- [ ] Mouse input working (can orbit camera)
- [ ] Old camera scripts disabled (no conflicts)
- [ ] No console errors or warnings

---

## 🚀 Commit to Git

After testing and confirming everything works:

```bash
git add .
git commit -m "[Sprint-2] Add: Cinemachine FreeLook camera with collision detection"
git push
```

---

## 📊 What's Next

**After Phase 3 is complete:**

Phase 4 will be **WebGL Build Testing** - ensuring everything works in the browser:
- Test keyboard controls in WebGL
- Verify HUD displays correctly
- Check performance
- Make any necessary optimizations

**Once Phase 4 is done, Sprint 2 is complete!** Then we'll:
1. Merge to develop branch
2. Write Sprint 2 retrospective
3. Start Sprint 3: Player State & Scene Management

---

**Let me know when you're ready to start, or if you hit any issues during setup!** 🎮
