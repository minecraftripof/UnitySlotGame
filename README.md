# Slot Machine

## Game Overview

A simple slot machine game created in Unity.

The player starts with 100 credits and spends 10 credits per spin. Each of the three reels independently selects a symbol using weighted random selection. A win occurs when all three reels land on the same symbol.

Winning spins award credits based on the matching symbol's payout multiplier. The player wins the game by reaching 250 credits and loses when they can no longer afford another spin.

## Controls

Click the slot machine lever to spin.

## Symbol Probabilities and Payouts

| Symbol | Weight | Payout |

| Seven | 40 | 5x bet |
| Cherry | 30 | 7x bet |
| Bell | 20 | 12x bet |
| BAR | 10 | 20x bet |

The listed probabilities are the probability of each symbol appearing on an individual reel. Since all three reels must match to win, the probability of a winning combination is lower.

## WebGL Instructions

The WebGL build is included in:

`Build/WebGL`

Because Unity WebGL builds should be served through a local web server rather than opening `index.html` directly, you can:

1. Open a terminal inside `Build/WebGL`.
2. Run:

   `python -m http.server 8000`

3. Open `http://localhost:8000` in a web browser.

## Extra Features

- Weighted symbol probabilities
- Credit and betting system
- Symbol-specific payouts
- Jackpot objective
- Game over and replay system
- Animated machine lever
- Sequential reel stopping
- Winning symbol rain effect
- Sound effects for the lever, reels, wins, jackpot and game over

## Thought Process

The game was structured so that gameplay logic and visual reel animation are kept separate.

Each spin determines its result using weighted random selection before the reel animation finishes. The reels then animate toward those predetermined results. This ensures that visual animation does not affect the fairness or reliability of the outcome.

Reels reuse a fixed set of UI symbol objects rather than continuously creating new objects while spinning. Symbols that leave the visible reel area are repositioned above the reel and given a new decorative sprite, allowing the reels to scroll continuously.

The project also separates responsibilities between classes for slot machine logic, reel animation, UI, popup behaviour, audio and winning visual effects.
