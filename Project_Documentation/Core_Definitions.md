# Core Definitions
1. It is a "behaviorally constrained" service, not a "physically protected" device.

SHSM's security assumptions differ from those of hardware HSM. Hardware HSM relies on physical tamper resistance (such as FIPS 140-2 Level 3), while SHSM relies on "behavioral determinism": the same input always produces the same output, and its behavior can be fully audited through code. SHSM does not claim resistance to physical attacks, but claims its software logic is public, verifiable, and predictable.

2. The SHSM server does not store keys; it only receives keys provided at runtime.

The SHSM server (the part simulating hardware) does not store plaintext copies of keys or private keys in hard-coded or file-based formats. It only receives keys or private keys provided by the client at runtime. Memory locking and automatic memory zeroing are core mechanisms to ensure maximum security.

On the client side, the developer's goal should be to minimize the exposure of keys or private keys. Except for the private key used for login, all other sensitive data or keys should be processed in a way that only the SHSM server can decrypt.

3. It supports cross-language calls, but client language type is limited by memory safety capabilities.

SHSM's API is provided via HTTP and is language-independent. However, due to software side-channel attack considerations, it currently only supports the following client programming languages: Node.js/TypeScript, Java, Python, and Go. These languages ​​can provide C-like pointers or uint8_t* data structures to support basic memory operation needs.

4. Auditability of key operations (not currently considered)

The current version does not involve anchoring key operations to Arweave or generating tamper-proof audit logs. This feature will be evaluated in future versions. It is not considered at this stage because it mainly involves considerations of ensuring data integrity. The most economical and relatively secure option currently is to use services using Western public blockchain technologies. However, it is possible that people will use server hosting providers that can ensure end-to-end server security. The latter is often more expensive than the former, which is a major drawback in terms of cost-effectiveness. It may also be used for some government or highly confidential purposes, in which case relying on blockchain or server hosting is basically not feasible.

5. Threat Model

SHSM's threat model is clearly defined:

- No Defense: Physical access attacks, chip-level side-channel attacks, operating system kernel-level intrusions.

- Defense: Application memory leaks, cross-language key remnants, key exposure due to misconfiguration (such as exposing services to insecure networks, using weak cryptographic parameters), and software-level side-channel attacks.

Users should understand this set of boundaries before deploying SHSM.

6. License and Distribution Model

SHSM is licensed under the AGPL, allowing anyone to deploy, modify, and audit its code. It is not primarily distributed as a hosted service, but rather exists as "self-deployable software."

7. Deployment Prerequisites

The core security prerequisite for SHSM is that every device deploying the SHSM server (not the client) must be configured not to use cryptographic algorithms that require specific hardware for security (e.g., disabling AES-GCM if AES-NI support is unavailable). Following auditing standards like FIPS could trigger export bans similar to those imposed on early Cisco devices due to their use of cryptographic techniques. This clause aims to prevent the introduction of new security risks due to improper algorithm deployment. Of course, if the device has specific hardware, supporting specific cryptographic algorithms is also possible. However, the default is to not use or rely on cryptographic algorithms that require specific hardware for security.

8. This primarily provides a practically executable framework and a full-stack technology chain. It aims to provide locally filtered HSM functionality while simultaneously providing security features such as API key protection.
