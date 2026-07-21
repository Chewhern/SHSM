# Who will adopt SHSM?
Given the nature of SHSM. 

Possible use cases include:

- API keys (OAuth's client_id and client_secret) secure management on client's side.
- Cryptographic secret or private keys that were encoded in Base64/PEM (Possible future integrations if SHSM had enough funds to go for **FIPS-140-2** or **FIPS-140-3** 's level one compliance out of 4 levels).
- Secure password handling (Possible future integrations/customized logic integrated directly into SHSM's server code)


Programming languages support (By having data type as close as one can be to C's ```unsigned char*``` or ```uint_8*``` include:

- C# (Modern version)
- Java
- Go
- Python
- TypeScript/NodeJS

There may be other programming languages that I missed. So far, these are the ones i know.

Potential actual use cases include:
- Open-source PKI projects
- Identity providers
- CI/CD signing systems
- Password managers
- Secure messaging software
- Secrets management tools
- Code signing infrastructure

# What existing project(s) could integrate SHSM
- Keycloak extensions
- EJBCA
- Smallstep
- OpenBao
- SoftHSM-related tooling

## CLI-Based Access (Potential Future Direction)
Languages without libsodium bindings or BCASodium oriented implementations cannot currently act as SHSM clients. While a **sudo-level CLI tool** could bridge this gap, its development is not part of the current Alpha phase. Depending on demand, it may be considered in a later stage.
