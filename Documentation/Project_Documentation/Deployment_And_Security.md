# SHSM Server Side
You can configure the port to be any number stated in Program.cs.

You can configure and enable HTTPS using Kestrel, and ensure the certificate and private key received from a CA (Certificate Authority) are in .pfx format.
For instructions on converting certificates to .pfx using the openssl command line, you may refer to LLMs or online documentation.

# Build, Debug, and Host/Deploy
You can check SPKI on how to build and debug the application. The same steps apply here.

However, for safety, kindly check the .csproj file from the applications to know which .NET version they are using. Then download and install the required .NET version accordingly.

# Security
There are about two types of security one can do:

- Host and deploy
- Cryptographic key generation

## Host and Deploy Security
There are several types of security settings:

- Highest
- Medium
- Lowest

### Highest Security (Private)
1. Ensure that the PC/Laptop acting as the SHSM Server has HTTPS enabled and uses certificates signed by any CA (Certificate Authority).
2. Ensure that the router or networking devices use open-source router software (e.g., OpenWrt, DD‑WRT). This helps reduce the risk of backdoors or vulnerabilities in proprietary firmware.
3. Ensure that the SHSM Server has DNSSEC properly configured. DNSSEC helps prevent DNS spoofing and ensures that clients connect to the intended SHSM server. This may be harder to do in a private setting.
4. Ensure that the VPS disabled **swap partitions**.

### Highest Security (Public)
Same applies, but without step 2 (open-source router software).

### Medium Security
Refer to Highest Security, but without DNSSEC and open-source router software.

### Lowest Security
Host and deploy in a pure HTTP environment.

## Cryptographic Key Generation
The strongest security will always be relying on pure randomness or as random as one can get for now.

However, in situations that need key recoverability or custom RNG, please bear your own consequences.

If a password is used for key generation, then ensure that the password has strong entropy. For deriving keys from passwords, it is recommended to use a key derivation function such as Argon2 or PBKDF2 with sufficient iterations and a random salt.
