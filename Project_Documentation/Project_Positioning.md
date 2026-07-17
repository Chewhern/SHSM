# Introduction
SHSM is a service that provides secure key execution in a pure software environment. It is not a replacement for hardware HSM, but rather an engineering alternative when hardware HSM is unavailable or unaffordable. The following explains its relationship to HSM.

# What's HSM (Hardware Security Module)?
Traditionally, HSM was used in military, government agencies, e-commerce(binding financial information such as debit or credit cards), law enforcements, certificate authorities and big multi-national corporates.

The reason for using HSM is that in these use cases, HSM provides a secure environment that allows private and secret keys to be used as long as possible in runtime while preventing both software and hardware theft of keys.

# What's SHSM (Software Emulated Hardware Security Module)?
In HSM, it did use two kinds of security concepts. First being security via obscurity (products include DRM, cheat, anti-cheat, malware and anti-malware) and the second being cryptography. 

Security via obscurity achieves security by ensuring no one knows how it works. Cryptography is the exact opposite of security via obscurity.

In summary, HSM did address on 4 issues.

1. Cryptographic algorithms’ side channel attacks (Eg, AES256GCM)
2. Psychic signature or key exchange’s all zero shared secret (Software related cryptographic side-channel attacks)
3. Handling of cryptographic secret keys in memory or in a secure manner (Programming language related side-channel attacks)
4. Prevent physical and virtual theft of cryptographic secret keys (Eg, CD key in earlier video games)

SHSM though on the other hand address on different sets of issues.
1. Solve the first three issues of HSM.
2. Provide a cheaper and open source software-based alternative but not replacement to an actual HSM (the security guarantees won’t be as good as an actual HSM but it’s considered to be an available option)
3. Less likely to be affected by export bans (One can refer to Cisco's devices ban from the US government on the early days due to the use of cryptographic techniques on their newer devices).
