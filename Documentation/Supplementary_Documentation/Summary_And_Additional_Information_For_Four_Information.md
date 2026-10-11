# Summary of Cryptographic side‑channel attacks, Psychic signature / all‑zero shared secret, Mutable and Immutable Data Types, Prevent physical and virtual theft
Cryptographic side-channel attacks have been largely mitigated by cryptographers, but currently,
the AES algorithm itself requires corresponding hardware support to be secure.
Details can be found in the AES256GCM explanation in the libsodium library.

If we're talking about cryptanalytic signature attacks, then this is essentially an oversight by the developers.
It can be categorized as a software-based cryptographic side-channel attack.

If we're talking about securely handling keys or private keys..., this largely falls under the category of programming language-based side-channel attacks.

Hardware security modules have already addressed points one through four.
However, regarding point three, programming language-based side-channel attacks, HSM itself only covers half of it.
Because when a user uses the import function or the web API, memory leaks have already occurred, either on the user's device or in the memory itself.

A purely software-based simulation can only achieve complete coverage of points one through three at best.
It cannot prevent physical and virtual theft, the last point.
However, this is often what many highly confidential government agencies, national defense, military,
large multinational corporations, certificate authorities, and e-commerce platforms (if involving information linked to bank accounts or cards) require.
