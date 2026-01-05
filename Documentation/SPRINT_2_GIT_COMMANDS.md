# Sprint 2 Wrap-Up - Git Commands Quick Reference

## ✅ Copy-Paste These Commands

**Execute these in order after you've tested WebGL build:**

---

### Step 1: Final Commit on Sprint Branch

```bash
# Check what's uncommitted
git status

# If there are changes, commit them
git add .
git commit -m "[Sprint-2] Complete: Phase 4 WebGL build tested successfully"
git push
```

---

### Step 2: Merge Sprint 2 to Develop

```bash
# Switch to develop branch
git checkout develop

# Pull any remote changes (good habit)
git pull

# Merge sprint-2-controls into develop
git merge sprint-2-controls

# Push merged develop to GitHub
git push
```

**✅ Expected output:**
```
Updating [hash]...[hash]
Fast-forward
 Assets/UI/GameHUD.uxml                       | 42 +++++++++
 Assets/UI/GameHUD.uss                        | 156 ++++++++++++++++++++++
 Assets/UI/Scripts/HUDController.cs           | 110 ++++++++++++++++
 Assets/UI/Scripts/PauseMenuController.cs     | 180 +++++++++++++++++++++++++
 ... (more files)
```

---

### Step 3: Tag Sprint 2 Release

```bash
# Create version tag
git tag -a v0.2.0-sprint2 -m "Sprint 2 Complete: Multi-Platform Controls & Basic HUD"

# Push tag to GitHub
git push --tags
```

**✅ Expected output:**
```
Counting objects: 1, done.
Writing objects: 100% (1/1), 195 bytes | 195.00 KiB/s, done.
Total 1 (delta 0), reused 0 (delta 0)
To https://github.com/AndrewLILP/CD26.git
 * [new tag]         v0.2.0-sprint2 -> v0.2.0-sprint2
```

---

### Step 4: Create Sprint 3 Branch (Optional)

```bash
# Make sure you're on develop
git branch
# Should show: * develop

# Create sprint-3 branch from develop
git checkout -b sprint-3-player-state

# Push new branch to GitHub
git push -u origin sprint-3-player-state
```

**✅ Expected output:**
```
Switched to a new branch 'sprint-3-player-state'
Branch 'sprint-3-player-state' set up to track remote branch 'sprint-3-player-state' from 'origin'.
```

---

## 🔍 Verify Everything Worked

### Check on GitHub:

**1. View Tags:**
Go to: `https://github.com/AndrewLILP/CD26/tags`
- Should see: v0.1.0-sprint1 and v0.2.0-sprint2

**2. View Branches:**
Go to: `https://github.com/AndrewLILP/CD26/branches`
- Should see: main, develop, sprint-1-foundation, sprint-2-controls, sprint-3-player-state

**3. View Commits on Develop:**
Go to: `https://github.com/AndrewLILP/CD26/commits/develop`
- Should see all your Sprint 2 commits merged in

---

## 🐛 Troubleshooting

### "Already on 'develop'" when switching
- You're already on develop, skip to Step 2 (merge command)

### "Your branch is ahead of origin/develop"
- Just means local is ahead of remote
- The `git push` will sync them

### "Merge conflict"
- Unlikely if you're solo dev, but if it happens:
  1. Open conflicted file in code editor
  2. Look for `<<<<<<<` markers
  3. Keep the code you want
  4. Remove conflict markers
  5. `git add .` then `git commit`

### "Nothing to commit, working tree clean"
- Good! Everything is already committed
- Skip the `git add .` and `git commit` commands

---

## 📋 Quick Status Checks

**See current branch:**
```bash
git branch
```
The one with `*` is active.

**See uncommitted changes:**
```bash
git status
```

**See recent commits:**
```bash
git log --oneline -10
```

**See all branches (local + remote):**
```bash
git branch -a
```

**See all tags:**
```bash
git tag
```

---

## ✅ When You're Done

After running all commands:
- [ ] `git branch` shows you're on sprint-3-player-state (or develop if you skipped Step 4)
- [ ] GitHub shows v0.2.0-sprint2 tag
- [ ] GitHub shows sprint-3-player-state branch
- [ ] develop branch has all Sprint 2 commits

**Then you're ready to update documentation and plan Sprint 3!**

---

## 💡 Pro Tip

**Before starting new work in Sprint 3:**
```bash
# Always make sure you're on the right branch
git branch

# If not on sprint-3-player-state:
git checkout sprint-3-player-state

# Pull latest (good habit)
git pull

# Now start working!
```
