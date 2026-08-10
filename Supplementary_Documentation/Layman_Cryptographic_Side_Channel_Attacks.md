# Layman explanation for cryptographic side‑channel attacks

What experts say: While a cryptographic algorithm is running, it leaks "side information" like power consumption, electromagnetic radiation, or execution time. An attacker can use these to guess the key.

Layman explanation:

Imagine you're typing your PIN at an ATM.

A normal person would think the danger is someone peeking over your shoulder (direct attack).

But a more subtle danger is someone hiding in the next room listening to the sound of the keypad.
- Each key makes a slightly different sound.
- The time it takes to type the PIN also gives hints.
- Even the electromagnetic noise from the machine can be picked up.

The attacker never sees your PIN, but by analyzing these "side signals" they can figure it out.

That's a side‑channel attack — attacking not the algorithm itself, but the traces it leaves while running.
