# Sprint 4: Interaction Systems

**Duration:** 2 weeks (Jan 20 - Feb 2, 2025)  
**Branch:** sprint-4-interactions  
**Status:** In Progress

## Sprint Goal
Enable player to interact with NPCs and inspect properties, creating the foundation for financial education gameplay.

## Deliverables
- [X] Mission system foundation
- [ ] Lap completion tracking + timing
- [ ] NPC dialogue system with 1992 aesthetic
- [ ] Property inspection mechanics
- [ ] Coffee Run mission integration
- [ ] Testing & polish

## Key Features
1. **Mission 1:** "First Lap" - Complete timed lap, earn $500
- polish needed: 
       - changing from driving to walking doesnt work anymore
       - Add visual enhancements:
       - ✨ Mission completion UI panel (match 1992 aesthetic)
       - 🏁 Lap time display during race
       - 📊 Show checkpoint progress (1/2, 2/2)
       - 🎵 Sound effects for checkpoint/completion
2. **Mission 2:** "Coffee Run" - Talk to Maria & Uncle Ray, learn financial concepts
3. **Dialogue System:** 1992 retro aesthetic (monospace font, scanlines)
4. **Property Inspection:** View cafe & garage financial data on proximity
5. **NPC Interactions:** Maria (Cash Flow), Uncle Ray (Assets/Liabilities)

## Technical Components
- MissionManager (Singleton, persistent)
- DialogueManager (Singleton, persistent)
- LapTimer (lap completion detection)
- NPCController (proximity-based interaction)
- PropertyInspector (building info display)

## Forced Mission Sequence
```
START → Mission 1: First Lap (Required)
       ↓
       Mission 2: Coffee Run (Unlocks after First Lap)
```

## 1992 Aesthetic Guidelines
- Font: Monospace/pixel-style
- Colors: Dark teal background (#0A2540), cyan text (#00FFFF)
- Border: Thick pixel borders, no rounded corners
- Effects: Optional CRT scanlines

## Success Criteria
✅ Player completes First Lap, earns $500  
✅ Coffee Run mission unlocks after First Lap  
✅ Player can talk to Maria (Cash Flow dialogue)  
✅ Player can talk to Ray (Asset/Liability dialogue)  
✅ Player can inspect cafe and garage properties  
✅ Mission UI displays with 1992 retro aesthetic  
✅ State persists across track ↔ town transitions  

---

**Next Update:** End of Phase 1 (Mission System Foundation)