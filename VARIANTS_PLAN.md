# Spades (`Spades/`) — Variants Plan

**Game:** Spades (partnership trick-taking, spades = trump)
**Project root:** `/Users/debarghy/Desktop/GamesV2/Spades/`
**Date:** 2026-05-18

---

## 1. Where the rules live today

| Concern | File |
|---|---|
| Turn / round state | `Assets/Scripts/callbreak/gameplay/SpadeGameState.cs` |
| Match state | `Assets/Scripts/callbreak/gameplay/SpadeMatchState.cs` |
| Bidding | `Assets/Scripts/callbreak/bidding/CallbreakBidding.cs` (reused) |
| Scoring | `Assets/Scripts/callbreak/score/CallbreakSetScore.cs` (with sandbag/nil) |
| Variant enum | `Assets/Scripts/callbreak/gameplay/GameVariant.cs` — currently `{HEARTS, BRAY}` |
| Settings sidecar | `Assets/Scripts/callbreak/gameplay/GameVariantSettings.cs` — has `isSandBagEnabled` |
| Settings UI | `Assets/Scripts/UIScripts/settings/SettingsScript.cs`, `SettingsManager.cs` |

> The Spades engine inherits the `callbreak/` engine; the variant enum here doesn't yet list Spades-specific values. Add a `SpadesRuleset` enum or extend `GameVariant`.

---

## 2. Variant catalogue (priority order)

### P0 — Joker Spades (54-card)
- **What changes:** Add Big Joker + Little Joker; both rank as the top two spades. Deck = 54; one player gets 14 cards (some rules drop the 2♣ + 2♥ to keep 13 each).
- **Market:** Standard option on most Spades apps. Top option on VIP Spades after Classic.
- **Where to edit:**
  - `Card.cs` / deck builder — add joker card type
  - `SpadeGameState.cs` — card-rank evaluator: Big Joker > Little Joker > A♠ > K♠…
  - Suit-following: jokers count as spades for follow-suit purposes
  - Art: joker card sprite asset
  - `CallbreakBidding.cs` AI: jokers should count toward bid estimation
- **Complexity:** Medium. The art + suit-following logic are the careful parts.

### P0 — Mirror Spades (bid = exact spades count)
- **What changes:** Each player must bid **exactly** the number of spades in their hand. No nil unless zero spades (then nil is forced).
- **Market:** Top-3 mode on VIP Spades and CardzMania.
- **Where to edit:**
  - `CallbreakBidding.cs` — biddable values reduce to `{ spadeCount }` (or `{ 0 }` if none)
  - `ComputerBidder.cs` — trivial: count and return
  - UI: hide bidder input, auto-bid and reveal
- **Complexity:** Low. Bid UX is the main work.

### P0 — Nil / Blind Nil bonus structure
- **What changes:** Confirm and expose the standard +100/-100 (Nil) and +200/-200 (Blind Nil) bonus structure. Most likely already implemented — just wire it as toggles.
- **Market:** Universal. Without nil, casual players think the app is broken.
- **Where to edit:** `CallbreakSetScore.cs` already has nil logic. Add `allowBlindNil` and `nilValue` to ruleset config.
- **Complexity:** Trivial if existing; low otherwise.

### P1 — Whiz Spades (bid your spades or nil; no blind nil)
- **What changes:** Each player must bid the exact number of spades they hold **or** bid nil. No blind nil.
- **Market:** Hardcore Spades crowd; signature mode on NeuralPlay.
- **Where to edit:** Same as Mirror, but bidder is allowed to pick `{spadeCount, 0}`.
- **Complexity:** Low.

### P1 — Suicide Spades
- **What changes:** Partnership Spades + one partner per team **must** bid nil; the other bids at least 4. No blind nil.
- **Market:** Hardcore mode; well-known on VIP Spades.
- **Where to edit:**
  - `CallbreakBidding.cs` — turn order changes: each partnership designates the nil-bidder first
  - `ComputerBidder.cs` — AI for "I must protect my partner who bid nil"
- **Complexity:** Medium. The AI for partner-protection is the hard part.

### P1 — Solo Spades (Cutthroat, no teams)
- **What changes:** Four players, no partnerships. Each plays for themselves.
- **Market:** Big crowd that hates partnerships ("dragged down by a bad partner"). Default of many beginner Spades apps.
- **Where to edit:**
  - `SpadeMatchState.cs` — score tracked per player not per pair
  - Bidding bag accumulation — per-player
  - AI: opponent modelling changes (no partner signalling)
- **Complexity:** Medium.

### P1 — Bag (sandbag) policy
- **What changes:** Configure the "10-bag penalty" — every 10 overtricks costs -100. Variants: (a) reset bags at penalty, (b) carry remainder; (c) higher threshold (5-bag tournament mode).
- **Where to edit:** `CallbreakSetScore.cs` — already has sandbag handling; expose threshold + reset behaviour.
- **Complexity:** Low.

### P2 — 3-player Spades
- **What changes:** 17 cards each, one card discarded face-down. No partnerships.
- **Where to edit:** Deal logic; seat layout; AI.
- **Complexity:** Medium (UI).

### P2 — 2-player Spades
- **What changes:** Reveal-and-pick deal mechanic — players draw from a deck, choosing to take or discard. 13 cards each, no kitty.
- **Market:** Niche but a popular casual mode.
- **Where to edit:** Deal logic is a new sub-state machine; UI for draw-and-pick.
- **Complexity:** High.

### P2 — Target-score customisation (200 / 500 / 1000)
- **What changes:** Just the win threshold. Common pref.
- **Complexity:** Trivial.

### P3 — Bid Whist / Bid Whist Joker
- **What changes:** Different game entirely (trump declared by bid); shares engine. Ship as a sibling, not a variant — but the engine reuse is real.
- **Complexity:** Medium-High.

### P3 — Wheel of Variants (random ruleset per match)
- Experimental social mode.

---

## 3. How to switch variants in the running game

```csharp
SpadesRulesetConfig ruleset = new SpadesRulesetConfig {
    biddingMode      = BiddingMode.Standard,     // or Mirror, Whiz, Suicide
    partnership      = Partnership.Pairs,        // or Solo
    deck             = DeckType.Standard52,      // or JokerDeck54
    allowNil         = true,
    allowBlindNil    = true,
    nilValue         = 100,
    blindNilValue    = 200,
    sandbagThreshold = 10,
    sandbagPenalty   = -100,
    sandbagReset     = true,
    targetScore      = 500,
};
```

UX recommendation:
- Lobby chip group: **Classic · Joker · Mirror · Solo · Custom**
- "Bidding" advanced row: Whiz, Suicide
- HUD chip: always show active ruleset

---

## 4. Things to be aware of

1. **Engine borrows from Callbreak.** Many functions (`CallbreakBidding.cs`, `CallbreakSetScore.cs`) are reused. Don't fork them; add ruleset branches inside.
2. **Spades-broken rule** — universal in Spades; verify it's implemented and that variant rulesets respect it. Mirror/Whiz can lead to weird scenarios where a player has no choice but to break spades early; tutorial should mention.
3. **Joker deck balance.** Adding two jokers to a 52-card deck means one player gets 14 cards. Two common solutions:
   - Drop the two of clubs and two of hearts → 52-card "Joker" deck. Most US apps do this.
   - Deal 14 to one and 12 to another → asymmetric; rare.
   - Pick ONE and stick with it; document.
4. **AI rebuild for Mirror/Whiz.** The AI no longer chooses a bid — but it still chooses cards. A Mirror AI that wastes its spades is hilarious in a bad way.
5. **Nil-protection AI** is genuinely hard for Suicide. Plan for visible AI imperfection in the first ship.
6. **Bag (sandbag) UI** — the running bag count must be visible at all times; many players don't know this rule and will complain about the surprise -100.
7. **Solo Spades changes social tone.** The "team chat" UI in partnership mode needs to be hidden in Solo.
8. **Save-game versioning** — add a `rulesetVersion` int.
9. **Inclusive-language sweep** — "Whiz" / "Suicide" are the established names but "Suicide" is a sensitive term in some markets; consider a localised label like "All-In Nil" while keeping the canonical name in the help screen.

---

## Sources

- [Wikipedia: Spades](https://en.wikipedia.org/wiki/Spades_(card_game))
- [Pagat: Spades variations](https://www.pagat.com/invented/spades_vars.html)
- [VIP Spades: Most popular variations](https://vipspades.com/academy/basics/most-popular-spades-variations/)
- [VIP Spades: All variations](https://vipspades.com/blog/different-variations-of-spades/)
- [VIP Spades: Cutthroat rules](https://vipspades.com/blog/cutthroat-spades-rules/)
- [NeuralPlay Spades](https://www.neuralplay.com/spades.html)
- [SuitedGames: Spades variants](https://suitedgames.com/spades/variants)
