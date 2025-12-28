# Sprint Retrospectives - CD26 Project

## Sprint 1: Foundation Setup (December 28, 2025)

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