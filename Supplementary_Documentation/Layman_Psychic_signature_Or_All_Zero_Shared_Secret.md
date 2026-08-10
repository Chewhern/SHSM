# Layman explanation for Java's psychic signature vulnerability or all zero shared secret
What experts say: A software bug can make a digital signature invalid in a way that’s not obvious, or cause two parties to “agree” on a shared secret that’s actually all zeros.

Layman explanation:

Imagine you’re signing a contract.

Normally, you sign it and the contract is valid.

But if your pen is faulty — say the ink cartridge is empty — you go through the motions, but nothing actually ends up on the paper. Later, someone can forge your signature and no one can tell the difference.

Another example: You and a friend agree on a secret code word.

You both say "blue", but the software glitches and the actual stored value becomes an empty string. You think you’ve agreed on a secret, but in reality you’ve agreed on nothing.

That’s what people call a psychic signature or an all‑zero shared secret — it looks like security is in place, but underneath, nothing is actually protected.
