# Who Will Adopt SHSM?
SHSM is designed for organizations and developers that require stronger software-based protection for cryptographic key material but cannot justify deploying dedicated hardware HSMs.

Potential adopters include:

- Open-source PKI projects
- Identity providers
- CI/CD signing systems
- Password managers
- Secure messaging software
- Secrets management tools
- Code signing infrastructure
- Small and medium-sized organizations requiring stronger software-based key protection

---

# Possible Use Cases

Possible use cases include:

- Secure management of API credentials (e.g., OAuth `client_id` and `client_secret`) on the client side.
- Secure handling of cryptographic secret keys and private keys, particularly those originating from Base64 or PEM encoded data.
- Secure password handling through future server-side extensions or customized SHSM logic.

Future enterprise-oriented integrations may become possible if SHSM eventually achieves **FIPS 140-2** or **FIPS 140-3 Level 1** compliance. However, FIPS certification is outside the scope of the current Alpha phase.

---

# Existing Projects That Could Integrate SHSM

Potential integration targets include:

- Keycloak extensions
- EJBCA
- Smallstep
- OpenBao
- SoftHSM-related tooling

These examples are intended to demonstrate potential interoperability rather than planned or officially supported integrations.

---

# Programming Language Support

SHSM is intended for programming languages that can securely interact with native cryptographic libraries such as libsodium and BouncyCastle (or equivalent implementations).

Current target languages include:

- C# (.NET)
- Java
- Go
- Python
- TypeScript / Node.js

Additional languages may become supported as suitable cryptographic libraries and secure integration approaches become available.

---

# CLI-Based Access (Potential Future Direction)
Some programming languages currently lack mature libsodium bindings or BCASodium-oriented implementations.

A privileged (**sudo-level**) CLI tool could provide an alternative integration mechanism for these languages. However, CLI support is **not part of the current Alpha phase** and will only be considered based on future demand and available development resources.
