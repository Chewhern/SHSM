# HSM Use Cases
HSM is typically used in environments where budget is not the primary constraint and where compliance or high-security requirements mandate hardware-level protection:

- Constantly encrypting or decrypting PII, banking information, or national secrets (e.g., e-commerce, law enforcement, military/national defense).
- Constantly signing public keys (e.g., Certificate Authorities like Let's Encrypt or subscription-based CA services).

# SHSM Use Cases
SHSM addresses scenarios where hardware HSM is unavailable, unaffordable, or overkill. Its primary use cases include:

- Missing cryptographic functions: Providing a secure software container that offers cryptographic functions not available in other languages.
- Secure key handling: Securely handling cryptographic secret keys (e.g., RSA keys or other private/secret keys encoded in Base64 and stored or retrieved as Strings).
- API key management: Enabling safer API key management when keys are stored or retrieved as Strings.
- Cross-language reach: Extending cryptographic security to intermediary languages (TypeScript, Node.js, GoLang, Java, Python) that can serve as clients.
- Pointer-level permissions: Providing permission controls (readonly, read-write, no-execute) for cryptographic secret keys.
- Password manager support: Enabling development of password managers with fewer programming language oriented side-channel attacks. This is currently only feasible in C, C++, and C#, because they provide mechanisms (such as GCHandle and manual memory control) to clear immutable String copies from memory after use.

The client languages listed above are chosen because they support data types (e.g., ```pointers```, ```byte[]```, or ```Buffer```) comparable to C's ```unsigned char*``` or ```uint8_t*```. 
