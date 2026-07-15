SHSM currently recommends and supports the following languages as clients:
- Java
- Node.js / TypeScript
- Go
- Python

These languages are chosen because they provide data types (such as ```byte[], Buffer, or []byte```) that can represent raw binary data in a way similar to C's ```unsigned char*, uint_8*```.
This capability is necessary for securely handling cryptographic keys and clearing them from memory after use.

Languages that do not provide this level of memory control, or that rely heavily on immutable string types for binary data (e.g., PHP, Ruby, or pure JavaScript without Buffer), are not currently supported as SHSM clients.
Using them would increase the risk of key material remaining in memory after use.
