SHSM currently recommends and supports the following languages as clients:
- Java
- Node.js / TypeScript
- Go
- Python

These languages are chosen because they provide data types (such as ```byte[]```, ```Buffer```, or ```[]byte```) that can represent raw binary data in a way similar to C's ```unsigned char*``` or ```uint8_t*```. This capability is necessary for securely handling cryptographic keys and clearing them from memory after use.

The selection is based on a practical engineering consideration: **to use SHSM effectively, the client language must be able to handle raw binary data without relying exclusively on immutable string types for cryptographic material.**

Languages that primarily handle binary data as immutable strings, or that lack direct access to raw byte buffers, are not currently supported as SHSM clients. Using them would increase the risk of key material remaining in memory after use.

This design choice is not intended to exclude specific languages, but to ensure that the security properties described in the SHSM core definitions can be reasonably maintained on the client side.
