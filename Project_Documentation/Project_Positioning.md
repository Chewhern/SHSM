# Introduction
SHSM is a service that provides secure key execution in a pure software environment. It is not a replacement for hardware HSM, but rather an engineering alternative when hardware HSM is unavailable or unaffordable.

# What is a Hardware Security Module (HSM)?
HSMs are traditionally used in military, government agencies, e-commerce, law enforcement, certificate authorities, and large multinational corporations. In these use cases, HSMs provide a secure environment that allows private and secret keys to be used securely over long periods, while preventing both software and hardware theft of keys.

HSMs combine two security concepts:

- Cryptography: The core of key protection.
- Security through obscurity: Mechanisms (such as DRM, anti-cheat, or anti-malware) that rely on hiding implementation details.

SHSM does not rely on security through obscurity.

HSMs are designed to address four main issues:

- Cryptographic side-channel attacks (e.g., timing or power analysis on AES256GCM without hardware support).
- Software-related side-channel attacks (e.g., psychic signature or all-zero shared secret in key exchange).
- Memory handling of cryptographic keys (addressed inside the HSM device; the remaining risk is on the client side, where keys are often imported via immutable `String` types in languages like Java, Python, NodeJS/TypeScript or Go).
- Physical and virtual theft of keys (e.g., CD-key extraction in early video games).

# What is SHSM (Software-Emulated Hardware Security Module)?
SHSM addresses a different set of issues:

- SHSM addresses the first two issues that HSMs aim to solve.
- It attempts to address the third issue where HSMs typically leave the client-side risk unhandled. When keys are imported via languages with immutable string types (Base64/PEM), SHSM tries to clear the last reachable copy of that data in memory. This does not eliminate all possible copies, but provides a practical improvement over leaving them untouched. If HSMs were to adopt this approach, the third issue would also be reduced. However, doing so would limit their compatibility with a wider range of client languages, as it would require stricter handling of key material at the client side.
- It provides a cheaper, open-source, software-based alternative — not a replacement. The security guarantees are lower than a physical HSM, but it remains a usable option for many environments.

SHSM does not attempt to solve the fourth issue — physical and virtual key theft.

# Positioning Summary
SHSM is designed for software environments where hardware HSM deployment is not feasible or cost-effective. It focuses on improving key handling in client-side software, especially in languages that do not provide secure memory management for immutable string types.

From an export and distribution perspective, SHSM's software-only nature may present fewer barriers compared to hardware-based cryptographic products. However, this is not a primary design goal.
