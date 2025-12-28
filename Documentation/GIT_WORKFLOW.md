# Git Branch Workflow Guide

## Branch Structure Overview

```
main               → Production-ready code only (merge from develop when stable)
├── develop        → Integration branch (merge completed sprints here)
    └── sprint-1-foundation  → Current work (DAY-TO-DAY COMMITS HERE)
    └── sprint-2-controls    → Next sprint (created after Sprint 1)
    └── sprint-3-...         → Future sprints
```

---

## Current Branch Status

✅ **main** - Initial Unity project committed  
✅ **develop** - Integration branch created  
✅ **sprint-1-foundation** - Active working branch (YOU ARE HERE)

---

## Daily Workflow (During Sprint 1)

You should already be on `sprint-1-foundation`. Verify with:

```bash
git branch
```

The `*` shows your current branch.

### Making Changes

```bash
# 1. Work in Unity (add assets, write code, etc.)

# 2. Stage your changes
git add .

# 3. Commit with descriptive message
git commit -m "[Sprint-1] Add: Basic car controller script"

# 4. Push to GitHub (backup + show progress)
git push
```

### Commit Message Templates

```
[Sprint-1] Add: CarController script with basic movement
[Sprint-1] Import: Muscle car asset from Unity Store
[Sprint-1] Fix: Wheel collider radius causing jitter
[Sprint-1] Setup: Track scene with start position
[Sprint-1] Config: Physics materials for track surface
[Sprint-1] Refactor: Move vehicle scripts to Vehicle folder
[Sprint-1] Update: Camera follow script with smooth damping
[Sprint-1] Test: Verify car drives around complete lap
```

---

## End of Sprint Workflow

### When Sprint 1 is Complete and Tested:

```bash
# 1. Make sure all changes are committed and pushed
git status  # Should show "nothing to commit, working tree clean"

# 2. Switch to develop branch
git checkout develop

# 3. Merge Sprint 1 work into develop
git merge sprint-1-foundation

# 4. Push updated develop to GitHub
git push

# 5. (Optional but Recommended) Tag the sprint completion
git tag -a v0.1.0-sprint1 -m "Sprint 1 Complete: Foundation Setup"
git push --tags
```

---

## Starting a New Sprint

### Example: Starting Sprint 2

```bash
# 1. Make sure you're on develop (this ensures new branch has latest code)
git checkout develop

# 2. Pull any updates (if working with others)
git pull

# 3. Create new sprint branch
git checkout -b sprint-2-controls

# 4. Push new branch to GitHub
git push -u origin sprint-2-controls

# 5. Start working!
```

---

## Common Git Commands

### Checking Status

```bash
# See current branch
git branch

# See all branches (including remote)
git branch -a

# See what's changed
git status

# See commit history
git log --oneline
```

### Switching Branches

```bash
# Switch to existing branch
git checkout develop
git checkout sprint-1-foundation

# Create and switch to new branch
git checkout -b sprint-2-controls
```

### Viewing Changes

```bash
# See uncommitted changes
git diff

# See what's staged for commit
git diff --cached

# See specific file changes
git diff path/to/file.cs
```

### Undoing Changes

```bash
# Discard changes to a specific file (CAREFUL!)
git checkout -- path/to/file.cs

# Unstage a file (keeps changes)
git reset path/to/file.cs

# Undo last commit (keep changes)
git reset --soft HEAD~1

# Undo last commit (discard changes) - DANGEROUS!
git reset --hard HEAD~1
```

---

## Sprint Completion Checklist

Before merging a sprint to develop:

- [ ] All sprint tasks completed
- [ ] Code runs without errors
- [ ] All changes committed and pushed to sprint branch
- [ ] Tested deliverable (e.g., "Can drive car around track")
- [ ] No console errors or warnings
- [ ] Sprint retrospective notes documented

---

## Branch Naming Convention

```
main                    → Production releases
develop                 → Integration branch
sprint-N-name           → Sprint work (e.g., sprint-1-foundation)
hotfix-description      → Emergency fixes to main
feature-description     → Optional sub-features within a sprint
```

---

## GitHub Repository Info

**Repository Name:** CD26
**Remote URL:** Check with `git remote -v`

### Viewing on GitHub

After pushing, view your work at:
- Main branch: https://github.com/AndrewLILP/CD26
- All branches: https://github.com/AndrewLILP/CD26/branches
- Commits: https://github.com/AndrewLILP/CD26/commits

---

## Troubleshooting

### "I'm on the wrong branch!"

```bash
# See where you are
git branch

# Switch to correct branch
git checkout sprint-1-foundation
```

### "I committed to the wrong branch!"

```bash
# If you haven't pushed yet:
# 1. Note the commit hash
git log --oneline

# 2. Undo the commit (keeps changes)
git reset --soft HEAD~1

# 3. Switch to correct branch
git checkout sprint-1-foundation

# 4. Commit again
git add .
git commit -m "[Sprint-1] Your message"
```

### "I need to update .gitignore"

```bash
# 1. Edit .gitignore file
notepad .gitignore

# 2. If files were already tracked, remove from Git
git rm -r --cached path/to/folder/

# 3. Commit the changes
git add .gitignore
git commit -m "[Sprint-1] Update: .gitignore to exclude X"
git push
```

### "Git says files are modified but I didn't change them"

This often happens with line endings (CRLF vs LF). Your `.gitattributes` file should prevent this, but if it happens:

```bash
# See what changed
git diff path/to/file

# If it's just line endings, this is normal and safe to commit
```

---

## Sprint Progression Example

### Sprint 1 (Current):
```bash
git checkout sprint-1-foundation  # Work here
# ... make changes ...
git add .
git commit -m "[Sprint-1] Add: Feature X"
git push

# When sprint complete:
git checkout develop
git merge sprint-1-foundation
git push
git tag -a v0.1.0-sprint1 -m "Sprint 1 Complete"
git push --tags
```

### Sprint 2 (Next):
```bash
git checkout develop
git checkout -b sprint-2-controls
git push -u origin sprint-2-controls
# ... work on Sprint 2 ...
```

---

## Best Practices

### DO:
- ✅ Commit frequently (after each task)
- ✅ Write clear commit messages
- ✅ Push to GitHub regularly (backup!)
- ✅ Work on sprint branches, merge to develop when complete
- ✅ Test before merging to develop

### DON'T:
- ❌ Commit directly to `main` (only merge from develop)
- ❌ Force push (`git push -f`) unless you know what you're doing
- ❌ Commit huge files (>100MB) - use Git LFS or exclude them
- ❌ Delete branches immediately (keep for reference)
- ❌ Work on multiple sprints simultaneously on same branch

---

## Quick Reference Card

| Task | Command |
|------|---------|
| See current branch | `git branch` |
| Switch branch | `git checkout branch-name` |
| Create new branch | `git checkout -b new-branch-name` |
| See changes | `git status` |
| Stage all changes | `git add .` |
| Commit | `git commit -m "message"` |
| Push to GitHub | `git push` |
| Pull from GitHub | `git pull` |
| Merge branch | `git merge branch-name` |
| See history | `git log --oneline` |

---

**Last Updated:** December 28, 2024  
**Sprint:** 1 - Foundation Setup  
**Current Branch:** sprint-1-foundation
