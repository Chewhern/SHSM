# Important
SHSM primarily separates HTTP API services into two main categories.

- Root
- Sudo

Any integrations will only be considered safe if and only **sudo** operations were done on client chosen devices. 

Next sub section will strictly refer to SHSM's client side compiled application. Starting from **Registration**, the right hand side of the client application will
act like a makeshift developer and development guidance. One can use it for easier development. 

The operations marked as **root** kindly make sure the secret and private keys never ended up in any client side's server side code. This'll also help to enforce
stricter cryptographic key usage policies.

## IP Config
All operations were considered **sudo** by default.

## Registration
Both operations were considered to be **root** by default.

**Sub_DSA** private key be it ED25519 (libsodium) or ED448 (BouncyCastle) must be copied out and use for latter **sudo** operations.

**There're no password involvement, kindly keep ```Sub_DSA``` private key in a safe and private space.**

## ETLS
The prerequisite is SHSM user was successfully registered to the SHSM server side in memory.

**Initiate ETLS** operation had no specific permission. However, it's best to treat it as **root** for safety reason.

**Delete ETLS** operation is equivalent **root**.

## Public Key Cryptography
**Import Keys** operation is equivalent to **root** and the prerequisites were SHSM user after registration,
choose an available ETLS algorithm and let the server side registered in memory.

```If the file format was confusing, kindly let the application does its work and you can then memory the file's naming syntax or something similar. So that, it'll be easier to recognize next time.```

**Extend Duration** operation is equivalent to **root** and the prerequisite is it's only executable after private key was imported.

**Sign Data, Sealedbox decrypt data** operations were equivalent to **sudo**. Using them also needs to pre-import corresponding private key.

## Secret Key Cryptography
**Initialize a pair of secret keys, encrypt data, decrypt data** operations will be treated as **sudo** by default.

```Note: You can either initialize a pair of secret keys then export them out or import a pair of secret keys you had exported previously.```

**Initialize a pair of secret keys, Import a pair of secret keys** both initialize the SHSM for secret key cryptography operations. To use these operations, the same SHSM user registration requirement needs to be met.

**Import a pair of secret keys** has one more requirement which is having an ETLS session with the SHSM server side.

**Import a pair of secret keys, Export a pair of secret keys, Extend duration** operations kindly assumed them as **root**.

## Arweave
The prerequisite for using this function needs to fulfill SHSM user registration and import RSA key via **Public Key Cryptography's ```Import Keys```**.

This operation was **sudo** by default after having the RSA key within SHSM server's memory.

## SHSM
The prerequisites for using this were SHSM user registration and either Secret Key Cryptography or Public Key Cryptography was initialized.

This operation was **root** by default as it forcibly deletes the whole user SHSM session from the SHSM server's memory. 
