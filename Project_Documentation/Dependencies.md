# Dependencies
SHSM relies on the following external components:

## Software Libraries
- AvaloniaUI (C#) – Licensed under MIT
- Libsodium (C) – Licensed under ISC
- BouncyCastle (C#/Java) – Licensed under MIT

## DBMS
- MySQL

If you look at the server side code, there's nothing much to be changed or hack with the SQL code or database and it's purposely designed that way.

Its purpose is to store and delete challenge (From challenge and response authentication mechanism with digital signature as its prompt) for each registered user.

## Online Service
Arweave (Blockchain) – Used for data anchoring (see Definition 4) and a basic form of IPS (Intrusion Prevention System) or cryptographic key commitment on registration's data.

*Note: Because this component relies on an external online service, any nation-state or institution that requires full control over its security infrastructure may need to modify or replace this dependency to mitigate potential national security risks.*
