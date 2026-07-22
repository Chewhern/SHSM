# Integration Cost
SHSM is designed as a service that isolates cryptographic operations. For most client applications, integrating with SHSM means replacing existing local key handling with HTTP API calls. The actual integration effort typically requires changing the way keys are loaded and used.

## Before integration (typical local crypto code)
Most applications handle keys directly in memory:


```
// C# (Kindly treat the following as pseudocode as the actual SHSM client and server code will be different)
// Typical local key handling (C# example)
byte[] privateKey = File.ReadAllBytes("key.txt"); //ED25519 or ED448 in binary data format
byte[] data = Encoding.UTF8.GetBytes("hello world");
byte[] signature = SodiumPublicKeyAuth.Sign(privateKey, data);
```
This approach leaves the private key exposed in the application's memory space.

## After integration with SHSM
```
//Assumes that SHSM had been running successfully in advance
//Assumes that keys had been imported to SHSM server side in advance
//Assumes that authentication private key had also been used to sign the 'challenge' retrieved by the server
//Assume there's HTTP Client that's equivalent to curl that can make HTTP request to SHSM server side. 
String DatasignatureMessageBase64 = await client.SignAsync(User_ID, SignedChallengeBase64Data,Base64Data);
```

In this pattern, the private key is only passed to SHSM and is not retained in the application's memory after the import call.

# Typical code changes required
For most applications, the changes are limited to two areas:

1. Key loading code

- Before: Load key from file or environment variable.
- After: Store the required authentication digital signature private key (in binary data format instead of Base64) in a secure location chosen by the deploying application, call the required cryptographic service from SHSM server with **user_id** and **signedchallengebase64** and **necessary JSON parameters if it's post** or **necessary query parameters if it's GET** to import the keys.

2. Cryptographic operations
- Before: Call local crypto library functions.
- After: Similar flow like the **After** in **Key loading code**

# Estimated effort
For an application with a modular key management layer, integration typically requires 2 to 4 hours to set up and test the first key operation.

For applications with tightly coupled crypto code, the effort may increase, but it generally does not require rewriting the entire crypto architecture. The main work is redirecting existing key operations to SHSM endpoints.

# Dependencies
Client side requires:

- A libsodium wrapper library for the programming language used (e.g., `sodium-native` for Node.js, `pysodium/pynacl` for Python, `libsodium-jna/lazysodium-java/lazysodium-android` bindings for Java, or an equivalent library).
- A BouncyCastle or equivalent cryptography library that can perform signature message generation and verification similar to **BCASodium**'s **SecureED448**.
- HTTP client capability (e.g., `fetch`, `requests`, `HttpClient`) and JSON serialization/deserialization support.
