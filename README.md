# Horse Racing Betting Simulator

Test assignment project: Horse racing simulation with betting mechanics and procedural animation system.

## Overview

Simulation game where players bet on horses and watch races with dynamically generated speed patterns. Built to demonstrate gameplay systems implementation and procedural animation.

## Core Mechanics

### Betting System

Players place bets on horses before each race. Win multipliers based on placement. Includes loan system for bankrupt players.

Economy tracking:
- Player balance
- Win/loss statistics
- Bet history
- Loan management

### Procedural Animation

Horses use dynamically generated AnimationCurves for speed variation. Each race has unique pacing - no two races are identical.

```csharp
AnimationCurve GenerateRandomCurve()
{
    // Creates procedural speed curve
    // Random keyframes with tangents
    // Ensures varied race outcomes
}
```

Speed randomization:
- Base speed with variance
- Curve duration randomization
- Smooth transitions between curves
- Natural acceleration/deceleration

### Race System

Three horses per race. First across finish line wins. Camera follows selected horse during race. Results shown with placement and winnings.

### UI Flow

Main Menu → Horse Selection → Bet Placement → Race Start Timer → Race → Results → Repeat

## Technical Implementation

### State Management

PlayerDataManager handles persistent data via PlayerPrefs:
- Money
- Win/loss counts
- Current bet
- Selected horse

### Camera System

Dynamic camera with smooth transitions:
- Main menu camera (idle)
- Race camera (follows selected horse)
- Winner camera (shows finish)
- Fade transitions between views

### Race Logic

FinishCollider tracks race completion:
- Detects horse crossings
- Tracks finish order
- Triggers camera switch when all finish
- Manages race state

## Project Structure

```
Assets/Scripts/
├── GameManager.cs           # Core game logic
├── PlayerDataManager.cs     # Persistent data
├── Horse.cs                 # Procedural animation
├── FinishCollider.cs        # Race completion
├── UI/
│   ├── UI_Manager.cs        # UI state machine
│   ├── Bet_UI_Manager.cs    # Betting interface
│   └── Finish_UI_Manager.cs # Results display
└── CameraManager.cs         # Camera transitions
```

## Key Features

**Procedural System**
No hand-animated races - all movement generated at runtime via curves.

**Economy Simulation**
Full betting economy with loans, win/loss tracking, balance management.

**Camera Work**
Smooth transitions and dynamic following during races.

**State Persistence**
PlayerPrefs saves progress between sessions.

## Tech Stack

- Unity
- C#
- AnimationCurve system
- PlayerPrefs for saves
- DOTween for UI transitions
- Custom camera system

## Assignment Context

Created as technical test to demonstrate:
- Procedural animation implementation
- Game state management
- UI/UX flow design
- Economy system design
- Camera control systems

Time constraint made visual polish secondary to systems implementation.

## Code Quality Notes

This was an early project with tight deadline. Code shows working systems but has areas for improvement:
- GameScript.cs could be split into smaller classes
- Some commented code remains from iteration
- UI references could use better architecture

Kept as-is to show progression in later projects.

## What I Learned

- Procedural animation with AnimationCurves
- Runtime curve generation and evaluation
- Camera state management
- Economy system balancing
- Quick prototyping under time pressure
