# Introduction

SHSM (Software-Emulated Hardware Security Module) is an open-source software service that provides secure cryptographic key handling and execution in software-only environments.

It is **not** intended to replace certified hardware HSMs. Instead, SHSM serves as an engineering alternative for deployments where dedicated hardware HSMs are unavailable, impractical, or cost-prohibitive. Its goal is to improve the security of cryptographic key management on commodity hardware while remaining transparent, portable, and open source.

---

# What is a Hardware Security Module (HSM)?

Hardware Security Modules (HSMs) are dedicated cryptographic devices designed to generate, store, and use cryptographic keys within a physically protected execution environment.

They are commonly deployed in environments such as:

- Certificate Authorities (CA)
- Government agencies
- Military systems
- Financial institutions
- E-commerce platforms
- Large enterprises
- Code signing infrastructure

Modern HSMs combine several complementary security mechanisms:

- Strong cryptographic primitives.
- Dedicated tamper-resistant hardware.
- Physical isolation of sensitive key material.
- Secure firmware and controlled execution environments.
- Hardware certifications (for example, FIPS 140) depending on the product.

These mechanisms provide stronger protection against both software and physical attacks while allowing long-term protection of highly sensitive cryptographic keys.

---

# Limitations Outside the HSM Boundary

Although HSMs provide strong protection for keys inside the hardware security boundary, applications interacting with an HSM still have responsibilities.

For example:

- Applications often import keys from PEM or Base64-encoded data.
- Many programming languages expose immutable string types that cannot be securely erased after use.
- Temporary copies of sensitive material may remain in application memory before being transferred into the HSM.

These risks exist outside the HSM itself and are generally considered application-level responsibilities rather than weaknesses of the HSM.

---

# What is SHSM?

SHSM focuses on improving software-side cryptographic key handling where dedicated hardware HSM deployment is not feasible.

Its objectives include:

- Providing secure in-memory handling of cryptographic keys.
- Reducing unnecessary exposure of sensitive material during application processing.
- Supporting secure execution of cryptographic operations in software.
- Offering an open-source alternative suitable for a wide range of deployment environments.

Unlike hardware HSMs, SHSM operates entirely in software and therefore cannot provide physical tamper resistance or hardware-backed security guarantees.

---

# Relationship to Hardware HSMs

SHSM should be viewed as complementary to hardware HSMs rather than as a replacement.

Where appropriate, SHSM attempts to reduce software-side risks that commonly occur before sensitive material reaches a hardware HSM.

Examples include:

- Clearing mutable copies of sensitive data where possible.
- Using protected memory for cryptographic material.
- Reducing the lifetime of sensitive information in application memory.

These techniques improve the software handling of cryptographic keys but cannot eliminate every possible memory copy created by managed runtimes or language implementations.

Consequently, SHSM offers lower security guarantees than a certified hardware HSM but provides a practical improvement over conventional software-only key handling.

---

# Threat Model

Hardware HSMs primarily address:

- Physical theft of cryptographic keys.
- Physical tampering.
- Hardware-based side-channel attacks.
- Secure long-term storage of sensitive keys.

SHSM primarily addresses:

- Software-based cryptographic operations.
- Secure memory management.
- Reduction of application-side key exposure.
- Practical improvements to software-only deployments.

SHSM does **not** attempt to provide:

- Physical tamper resistance.
- Protection against device theft.
- Hardware security certifications.
- Equivalent security guarantees to certified HSMs.

---

# Positioning Summary

SHSM is designed for organisations and developers who require stronger software-based key protection but cannot justify deploying dedicated hardware HSMs.

Typical deployment scenarios include:

- Small and medium enterprises.
- Open-source infrastructure.
- Development and testing environments.
- Containerised workloads.
- Cloud-native services.
- Research projects.
- Cost-sensitive deployments.

Rather than replacing hardware HSMs, SHSM fills the gap between conventional software key handling and dedicated hardware security modules.

Its software-only architecture also makes it easier to inspect, extend, and distribute as open-source infrastructure, encouraging transparency and community review.

Export and distribution considerations may differ from hardware cryptographic products because SHSM contains no specialised hardware. However, this is a secondary consequence of the design rather than a primary project objective.
